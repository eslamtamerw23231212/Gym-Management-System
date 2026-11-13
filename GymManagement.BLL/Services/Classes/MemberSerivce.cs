
using AutoMapper;
using Gym_Management_System.Models;
using GymManagement.BLL.Services.Interfaces;
using GymManangement.DAL.Models;
using GymManangement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberSerivce : IMemberService
    {
        private readonly IUnitofWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IGenericRepository<Member> genericRepository;
        private readonly IGenericRepository<MemberShip> memberShipRepository;
        private readonly IGenericRepository<Plan> planRepository;
        private readonly IGenericRepository<HealthRecord> healthRepository;
        private readonly IGenericRepository<Booking> bookingRepository;

        public MemberSerivce(IUnitofWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            genericRepository = unitOfWork.GetReposioty<Member>();
            memberShipRepository = unitOfWork.GetReposioty<MemberShip>();
            planRepository = unitOfWork.GetReposioty<Plan>();
            healthRepository = unitOfWork.GetReposioty<HealthRecord>();
            bookingRepository = unitOfWork.GetReposioty<Booking>();
        }

        public async Task<bool> CreateMemberAsync(
            CreateMemberViewModel viewModel,
            CancellationToken ct = default)
        {
            var emailExists = await genericRepository.AnyAsync(
                m => m.Email == viewModel.Email, ct);

            var phoneExists = await genericRepository.AnyAsync(
                m => m.Phone == viewModel.Phone, ct);

            if (emailExists || phoneExists)
                return false;

            var member = mapper.Map<Member>(viewModel);

            member.Photo = viewModel.Photo;

            genericRepository.AddAsync(member);

            return await unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(
            CancellationToken ct = default)
        {
            var members = await genericRepository.GetAll(ct: ct);

            if (!members.Any())
                return [];

            return mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
        }

        public async Task<HealthRecordViewModel?> GetHealthRecordDetails(
            int id,
            CancellationToken cancellationToken = default)
        {
            var healthrecord = await healthRepository.FirstOrDefaultAsync(
                m => m.Id == id,
                cancellationToken: cancellationToken);

            if (healthrecord == null)
                return null;

            return mapper.Map<HealthRecord, HealthRecordViewModel>(healthrecord);
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(id, ct);

            if (member == null)
                return null;

            var model = mapper.Map<Member, MemberViewModel>(member);

            var activeMemberShip = await memberShipRepository.FirstOrDefaultAsync(
                m => m.MemberId == member.Id && m.EndDate > DateTime.Now,
                cancellationToken: ct);

            if (activeMemberShip is not null)
            {
                model.MemberShipStartDate = activeMemberShip.CreatedAt.ToString();
                model.MemberShipEndDate = activeMemberShip.EndDate.ToString();
            }

            return model;
        }

        public async Task<bool> RemoveMemberAsync(
            int memberId,
            CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(memberId, ct);

            if (member == null)
                return false;

            var hasBookings = await bookingRepository.AnyAsync(
                b => b.MemberId == memberId, ct);

            if (hasBookings)
                return false;

            genericRepository.DeleteAsync(member);

            return await unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<MemberToUpdateViewModel?> UpdateMember(
            int id,
            CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(id, ct);

            if (member == null)
                return null;

            return mapper.Map<MemberToUpdateViewModel>(member);
        }

        public async Task<bool> UpdateMemberDetailsAsync(
            int id,
            MemberToUpdateViewModel memberToUpdateViewModel,
            CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(id, ct);

            if (member is null)
                return false;

            if (string.IsNullOrWhiteSpace(memberToUpdateViewModel.Name))
                return false;

            var emailExists = await genericRepository.AnyAsync(
                m => m.Email == memberToUpdateViewModel.Email && m.Id != id, ct);

            var phoneExists = await genericRepository.AnyAsync(
                m => m.Phone == memberToUpdateViewModel.Phone && m.Id != id, ct);

            if (emailExists || phoneExists)
                return false;

            member.Name = memberToUpdateViewModel.Name;
            member.Email = memberToUpdateViewModel.Email;
            member.Phone = memberToUpdateViewModel.Phone;

            if (!string.IsNullOrWhiteSpace(memberToUpdateViewModel.Photo))
                member.Photo = memberToUpdateViewModel.Photo;

            if (member.address is null)
                member.address = new GymUser.Address();

            member.address.City = memberToUpdateViewModel.City;
            member.address.Street = memberToUpdateViewModel.Street;
            member.address.BuildingNumber = memberToUpdateViewModel.BuildingNumber;

            genericRepository.UpdateAsync(member);

            return await unitOfWork.SaveChangesAsync(ct) > 0;
        }
    }
}


