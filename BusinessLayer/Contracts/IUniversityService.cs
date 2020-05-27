using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.BusinessLayer.DTO.University;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Contracts
{
    [ServiceContract]
    public interface IUniversityService
    {
        [OperationContract]
        Task<UniversityBookDTO> AddAsync(AddUnivesityDTO addUnivesity);
        [OperationContract]
        Task<IEnumerable<UniversityBookDTO>> GetAll();
        [OperationContract]
        Task<UniversityBookDTO> Get(int id);
        [OperationContract]
        Task<EditUniversityDTO> EditAsync(EditUniversityDTO addUnivesity);
        [OperationContract]
        Task<int> RemoveAsync(int universityId);

        [OperationContract]
        public string Ping();
    }
}
