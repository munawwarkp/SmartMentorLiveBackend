using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Application.Features.Auth.Events;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Persistence.UOW;

namespace SmartMentorLive.Application.Features.Auth.EventHandlers
{
    public class UserLoggedInEventHandler : INotificationHandler<UserLoggedInEvent>
    {
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserLoggedInEventHandler(ILoginHistoryRepository loginHistoryRepository, IUnitOfWork unitOfWork)
        {
            _loginHistoryRepository = loginHistoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
        {
            var loginHistory = new LoginHistory
            {
                UserId = notification.UserId,
                LoggedInAt = notification.LoggedInAt,
            };
            await _loginHistoryRepository.AddAsync(loginHistory, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);
        }
    }
}
