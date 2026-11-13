
using AutoMapper;
using Gym_Management_System.Models;
using GymManagement.BLL.Services.Interfaces;
using GymManangement.DAL.Models;
using GymManangement.DAL.Repositories.Interfaces;
using GymManangement.DAL.Repositories.Interfaces.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitofWork unitofWork;
        private readonly IMapper mapper;

        public PlanService(IUnitofWork unitofWork, IMapper mapper)
        {
            this.unitofWork = unitofWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var planRepository = unitofWork.GetReposioty<Plan>();
            var plans = await planRepository.GetAll();

            return mapper.Map<IEnumerable<PlanViewModel>>(plans);
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitofWork.GetReposioty<Plan>().GetByIdAsync(planId, ct);

            return plan is null
                ? null
                : mapper.Map<PlanViewModel>(plan);
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitofWork.GetReposioty<Plan>().GetByIdAsync(planId, ct);

            if (plan is null)
                return null;

            return mapper.Map<UpdatePlanViewModel>(plan);
        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var repo = unitofWork.GetReposioty<Plan>();

            var plan = await repo.GetByIdAsync(planId, ct);

            if (plan is null)
                return Result.NotFound("Plan not found.");

            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct))
                return Result.Fail("Cannot deactivate a plan that has active memberships.");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            repo.UpdateAsync(plan);

            var result = await unitofWork.SaveChangesAsync(ct);

            return result > 0
                ? Result.Ok()
                : Result.Fail("Failed to Toggle Plan Status");
        }

        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var repo = unitofWork.GetReposioty<Plan>();

            var plan = await repo.GetByIdAsync(id, ct);

            if (plan is null)
                return Result.NotFound("Plan not found.");

            if (await HasActiveMembershipsAsync(id, ct))
                return Result.Fail("Cannot edit a plan that has active memberships.");

            mapper.Map(model, plan);

            plan.UpdatedAt = DateTime.Now;

            repo.UpdateAsync(plan);

            await unitofWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct)
        {
            var membershipRepository = unitofWork.GetReposioty<MemberShip>();

            return await membershipRepository.AnyAsync(
                m => m.PlanId == planId && m.EndDate > DateTime.Now,
                ct);
        }
    }
}

