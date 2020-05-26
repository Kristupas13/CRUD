using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.BusinessLayer.DTO.University;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Contracts
{
    public interface IUniversityService
    {
        Task<UniversityBookDTO> AddAsync(AddUnivesityDTO addUnivesity);
        Task<IEnumerable<UniversityBookDTO>> GetAll();
        Task<UniversityBookDTO> Get(int id);
        Task<EditUniversityDTO> EditAsync(EditUniversityDTO addUnivesity);
        Task<int> RemoveAsync(int universityId);
    }
}
