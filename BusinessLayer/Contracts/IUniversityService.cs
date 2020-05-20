using CRUDWebService.BusinessLayer.Base.DTO;
using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Contracts
{
    public interface IUniversityService
    {
        Task<UniversityDTO> AddAsync(AddUnivesityDTO addUnivesity);
        Task<IEnumerable<UniversityDTO>> GetAll();
        Task<UniversityDTO> Get(int id);
        Task<UniversityDTO> EditAsync(EditUniversityDTO addUnivesity);
        Task<int> RemoveAsync(RemoveUniversityDTO addUnivesity);

        Task<IEnumerable<UniversityBookDTO>> GetUniversityBooks(int universityId);
        Task<UniversityBookDTO> GetUniversityBookByISBN(int universityId, string bookISBN);
        Task<UniversityBookModifiedDTO> AddBookToUniversityAsync(UniversityBookDTO universityBook);
        Task<UniversityBookModifiedDTO> EditUniversityBookAsync(UniversityBookModifiedDTO universityBookEdited);
        Task<UniversityBookModifiedDTO> RemoveUniversityBookAsync(int universityId, string bookISBN);
    }
}
