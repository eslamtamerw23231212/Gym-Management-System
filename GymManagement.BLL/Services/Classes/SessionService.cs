
using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManangement.DAL.Models;
using GymManangement.DAL.Models.Enum;
using GymManangement.DAL.Repositories.Interfaces;
using GymManangement.DAL.Repositories.Interfaces.Classes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitofWork unitofWork;
        private readonly IMapper mapper;

        public SessionService(IUnitofWork unitofWork,IMapper mapper)
        {
            this.unitofWork = unitofWork;
            this.mapper = mapper;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel createSessionViewModel, CancellationToken ct = default)
        {
            if (createSessionViewModel.EndDate <= createSessionViewModel.StartDate) return false;
            if (createSessionViewModel.StartDate < DateTime.Now)
            {
                return false;
            }
            if(createSessionViewModel.Capacity<1 || createSessionViewModel.Capacity > 25)
            {
                return false;
            }
            var trainer = await unitofWork.GetReposioty<Trainer>().GetByIdAsync(createSessionViewModel.TrainerId, ct);
            if(trainer is null)
            {
                return false;
            }
            var category = await unitofWork.GetReposioty<Category>().GetByIdAsync(createSessionViewModel.CategoryId, ct);
            if(category is null){
                return false;
            }
            var IsValid = Enum.TryParse<Speciality>(category.CategoryName, out var speciality);
            if(!IsValid || trainer.speciality != speciality)
            {
                return false;
            }
            var session = mapper.Map<CreateSessionViewModel, Session>(createSessionViewModel);
             unitofWork.GetReposioty<Session>().AddAsync(session);
            return await unitofWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessions(
            CancellationToken cancellationToken = default)
        {
            var sessions = await unitofWork.sessionRepository
                .GetAllSessionsWithTrainerAndCategory(cancellationToken);

            if (sessions == null || !sessions.Any())
                return null;

            var mappedSessions = sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                CategoryName = s.category.CategoryName,
                TrainerName = s.trainer.Name,
                Description = s.Description,
                EndDate = s.EndDate,
                StartDate = s.StartDate,
            }).ToList();

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity -
                    await unitofWork.sessionRepository
                        .GetCountofBookedSlotsAsync(session.Id, cancellationToken);
            }

            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryFromDropDownAsync(
    CancellationToken ct = default)
        {
            var category = await unitofWork
                .GetReposioty<Category>()
                .GetAll(ct: ct);

            return mapper.Map<IEnumerable<CategorySelectViewModel>>(category);
        }

        public async Task<SessionViewModel?> GetSessionById(
    int sessionId,
    CancellationToken ct = default)
        {
            var sessions = await unitofWork.sessionRepository
                .GetAllSessionsWithTrainerAndCategory(ct);

            var session = sessions.FirstOrDefault(s => s.Id == sessionId);

            if (session is null)
            {
                return null;
            }

            var mappedSession = mapper.Map<SessionViewModel>(session);

            var bookedSlots = await unitofWork.sessionRepository
                .GetCountofBookedSlotsAsync(sessionId, ct);

            mappedSession.AvailableSlots = mappedSession.Capacity - bookedSlots;

            return mappedSession;
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(
      int sessionId,
      CancellationToken ct = default)
        {
            var session = await unitofWork
                .GetReposioty<Session>()
                .GetByIdAsync(sessionId, ct);

            if (session is null)
                return null;

            if (!await IsSessionValidForUpdatingAsync(session, ct))
                return null;

            return mapper.Map<UpdateSessionViewModel>(session);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerFromDropDownAsync(
    CancellationToken ct = default)
        {
            var trainer = await unitofWork
                .GetReposioty<Trainer>()
                .GetAll(ct: ct);

            return mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainer);
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var repo = unitofWork.GetReposioty<Session>();
            var session = await repo.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Cannot delete a session that has not yet ended.");

            var bookedCount = await unitofWork.sessionRepository.GetCountofBookedSlotsAsync(sessionId, ct);
            if (bookedCount > 0)
                return Result.Fail("Cannot delete a session that has bookings.");

            repo.DeleteAsync(session);
            var affectedRows = await unitofWork.SaveChangesAsync(ct);

            return affectedRows > 0 ? Result.Ok() : Result.Fail("Failed to Delete session.");

        }

        public async Task<Result> UpdateSessionAsync(
    int id,
    UpdateSessionViewModel updateSessionViewModel,
    CancellationToken cancellationToken = default)
        {
            var sessionRepo = unitofWork.GetReposioty<Session>();

            var session = await sessionRepo.GetByIdAsync(id, cancellationToken);

            if (session is null)
                return Result.NotFound("Session not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot edit a session that has already started.");

            var bookedCount = await unitofWork.sessionRepository
                .GetCountofBookedSlotsAsync(id, cancellationToken);

            if (bookedCount > 0)
                return Result.Fail("Cannot edit a session that already has bookings.");

            if (updateSessionViewModel.EndDate <= updateSessionViewModel.StartDate)
                return Result.Validation("End date must be after start date.");

            if (updateSessionViewModel.StartDate <= DateTime.Now)
                return Result.Validation("Start date must be in the future.");

            var trainerRepo = unitofWork.GetReposioty<Trainer>();

            var trainer = await trainerRepo.GetByIdAsync(
                updateSessionViewModel.TrainerId,
                cancellationToken);

            if (trainer is null)
                return Result.NotFound("Trainer not found.");

            var category = await unitofWork.GetReposioty<Category>()
                .GetByIdAsync(session.CategoryId, cancellationToken);

            if (category is null)
                return Result.NotFound("Category not found.");

            var isValidSpecialty = Enum.TryParse<Speciality>(
                category.CategoryName,
                true,
                out var categorySpecialty);

            if (!isValidSpecialty || trainer.speciality != categorySpecialty)
                return Result.Validation("This trainer does not match the session category.");

            mapper.Map(updateSessionViewModel, session);

            session.TrainerId = updateSessionViewModel.TrainerId;
            session.UpdatedAt = DateTime.Now;

            sessionRepo.UpdateAsync(session);

            var affectedRows = await unitofWork.SaveChangesAsync(cancellationToken);

            return affectedRows > 0
                ? Result.Ok()
                : Result.Fail("Failed to update session.");
        }
        private async Task<bool> IsSessionValidForUpdatingAsync(Session session, CancellationToken ct = default)
        {
            if (session.StartDate <= DateTime.Now) return false;
            var booked = await unitofWork.sessionRepository.GetCountofBookedSlotsAsync(session.Id, ct);
            return booked == 0;
        }
    }
}
