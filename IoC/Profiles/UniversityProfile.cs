using AutoMapper;
using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.PresentationLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.IoC.Profiles
{
    public class UniversityProfile : Profile
    {
        public UniversityProfile()
        {
            CreateMap<CreateUniversityViewModel, AddUnivesityDTO>();
            CreateMap<AddUnivesityDTO, CreateUniversityViewModel>();

            CreateMap<UniversityDTO, UniversityViewModel>();
            CreateMap<UniversityViewModel, UniversityDTO>();

            CreateMap<EditUniversityViewModel, EditUniversityDTO>();
            CreateMap<EditUniversityDTO, EditUniversityViewModel>();

            CreateMap<EditUniversityDTO, UniversityViewModel>();
            CreateMap<UniversityViewModel, EditUniversityDTO>();

            CreateMap<UniversityDTO, UniversityBookInformationViewModel>();
            CreateMap<UniversityBookInformationViewModel, UniversityDTO>();

            CreateMap<UniversityModifiedViewModel, UniversityBookModifiedDTO>();
            CreateMap<UniversityBookModifiedDTO, UniversityModifiedViewModel>();
        }
    }
}
