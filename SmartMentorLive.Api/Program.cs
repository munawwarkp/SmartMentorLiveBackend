
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMentorLive.Infrastructure.Configuration;
using SmartMentorLive.Api.MIddleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Infrastructure.Authentication;
using SmartMentorLive.Infrastructure.Persistence.Context;
using FluentValidation;
using SmartMentorLive.Application.Features.Auth.Commands.Login;
using SmartMentorLive.Application.Features.Auth.Commands.Register;

namespace SmartMentorLive.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //read connection string from configuaration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            //register infrastructure db context
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            //jwt

            //bind JwtSetting class to jwt object in appsettings
            var jwtSettingSection = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtSettings>(jwtSettingSection);   //register settings in DI

            var jwtSettings = jwtSettingSection.Get<JwtSettings>();

            if(jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JWT settings are missing or invalid. Please check appsettings.json or user-secrets.");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

            //add mediatR
            builder.Services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


            //register middleware
            builder.Services.AddTransient<GlobalExceptionHandler>();

            //register validatoro
            builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();

        //register token genrator sevice
        builder.Services.AddScoped<IJwtTokenGenerator,JwtTokenGenerator>();

            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                //error for developers
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
