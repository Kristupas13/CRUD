using CRUDWebService.BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Contracts
{
    public interface IUniversityService
    {
        Task<UniversityDTO> AddAsync(AddUnivesityDTO addUnivesity);
        IEnumerable<UniversityDTO> GetAll();
        UniversityDTO Get(int id);
        Task<UniversityDTO> EditAsync(EditUniversityDTO addUnivesity);
        Task RemoveAsync(RemoveUniversityDTO addUnivesity);
    }
}
