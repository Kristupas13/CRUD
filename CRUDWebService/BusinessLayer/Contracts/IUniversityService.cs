using CRUDWebService.BusinessLayer.Base.DTO;
using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.BusinessLayer.DTO.University;
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
        Task<int> RemoveAsync(RemoveUniversityDTO addUnivesity);

        Task<IEnumerable<UniversityBookDTO>> GetUniversityBooks(int universityId);
        Task<UniversityBookDTO> GetUniversityBookByISBN(int universityId, string bookISBN);
        Task<UniversityBookReferences> AddBookToUniversityAsync(int universityId, string bookISBN);
        Task<UniversityBookReferences> EditUniversityBookAsync(int universityId, string bookISBN);
        Task<UniversityBookReferences> RemoveUniversityBookAsync(int universityId, string bookISBN);
    }
}
