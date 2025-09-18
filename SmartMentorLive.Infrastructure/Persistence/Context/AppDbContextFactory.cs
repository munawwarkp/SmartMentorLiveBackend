using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace SmartMentorLive.Infrastructure.Persistence.Context
{
    //IDesignTimeDbContextFactory  is only used for tooling (add-migratin and update database ) 
    //no need to register in program.cs
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        //for single environment
        public AppDbContext CreateDbContext(string[] args)
        {
            //throw new NotImplementedException();

            //cannot read json,only configure EF options( like sqlserver,logging,lazy loading )
            //var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();   

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  //file extention package
                .AddJsonFile("appsettings.json", optional: false) //needs package
                .Build();

            var optionBuilder = new DbContextOptionsBuilder<AppDbContext> ();
            optionBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            return new AppDbContext (optionBuilder.Options);

        }
    }
}
