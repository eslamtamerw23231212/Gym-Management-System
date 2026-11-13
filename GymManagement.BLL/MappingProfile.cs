
using AutoMapper;
using Gym_Management_System.Models;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManangement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static GymManangement.DAL.Models.GymUser;

namespace GymManagement.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src =>
                        $"{src.address.BuildingNumber} {src.address.Street}, {src.address.City}"))
                .ForMember(dest => dest.Date0fBirth,
                    opt => opt.MapFrom(src => src.DateofBirth.ToString()))
                .ForMember(dest => dest.Photo,
                    opt => opt.MapFrom(src => src.Photo));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.address,
                    opt => opt.MapFrom(src => new Address()
                    {
                        BuildingNumber = src.BuildingNumber,
                        Street = src.Street,
                        City = src.City,
                    }))
                .ForMember(dest => dest.healthRecord,
                    opt => opt.MapFrom(src => src.HealthRecordViewModel))
                .ForMember(dest => dest.Photo,
                    opt => opt.MapFrom(src => src.Photo));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForMember(dest => dest.address,
                    opt => opt.MapFrom(src => new GymUser.Address
                    {
                        City = src.City,
                        Street = src.Street,
                        BuildingNumber = src.BuildingNumber
                    }));

            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();

            CreateMap<Plan, PlanViewModel>();
            CreateMap<Plan, UpdatePlanViewModel>();
            CreateMap<UpdatePlanViewModel, Plan>();

            CreateMap<Trainer, TrainerViewModel>();
            CreateMap<Trainer, TrainerToUpdateViewModel>();
            CreateMap<TrainerToUpdateViewModel, Trainer>();

            CreateMap<CreateTrainerViewModel, Trainer>();
        }
    }
}
