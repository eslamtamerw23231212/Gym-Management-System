using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManangement.DAL.Models;
using GymManangement.DAL.Repositories.Interfaces;
using GymManangement.DAL.Repositories.Interfaces.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitofWork unitofWork;
        private readonly IMapper mapper;

        public TrainerService(IUnitofWork unitofWork,IMapper mapper)
        {
            this.unitofWork = unitofWork;
            this.mapper = mapper;
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var repo = unitofWork.GetReposioty<Trainer>();

            if (await repo.AnyAsync(t => t.Email == model.Email, ct))
                return Result.Fail("A trainer with this email already exists.");

            if (await repo.AnyAsync(t => t.Phone == model.Phone, ct))
                return Result.Fail("A trainer with this phone number already exists.");

            var entity = mapper.Map<Trainer>(model);

            entity.address = new GymUser.Address
            {
                BuildingNumber = model.BuildingNumber,
                City = model.City,
                Street = model.Street
            };

            repo.AddAsync(entity);

            var result = await unitofWork.SaveChangesAsync(ct);

            return result > 0
                ? Result.Ok()
                : Result.Fail("Failed To Create Trainer");
        }


        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await unitofWork.GetReposioty<Trainer>().GetAll(ct: ct);
            return mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitofWork.GetReposioty<Trainer>().GetByIdAsync(trainerId, ct);
            return trainer is null ? null : mapper.Map<TrainerViewModel>(trainer);
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitofWork.GetReposioty<Trainer>().GetByIdAsync(trainerId, ct);
            return trainer is null ? null : mapper.Map<TrainerToUpdateViewModel>(trainer);
        }

        public async Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var repo = unitofWork.GetReposioty<Trainer>();
            var trainer = await repo.GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer not found.");

            var hasFutureSessions = await unitofWork.GetReposioty<Session>()
                .AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);
            if (hasFutureSessions)
                return Result.Fail("Cannot delete a trainer with upcoming sessions.");

            repo.DeleteAsync(trainer);
            await unitofWork.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var repo = unitofWork.GetReposioty<Trainer>();
            var trainer = await repo.GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer not found.");

            // BUG FIX from original: original code checked against MemberEntity (wrong table).
            if (await repo.AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct))
                return Result.Fail("Another trainer is already using this email.");
            if (await repo.AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct))
                return Result.Fail("Another trainer is already using this phone number.");

            mapper.Map(model, trainer);
            trainer.UpdatedAt = DateTime.Now;
            repo.UpdateAsync(trainer);

            var result = await unitofWork.SaveChangesAsync(ct);     
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update Trainer");
        }
    }
}
