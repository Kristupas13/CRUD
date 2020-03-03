using CRUDWebService.BusinessLayer.DTO.Student;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Contracts
{
    public interface IStudentService
    {
        Task<StudentDTO> AddAsync(AddStudentDTO addUnivesity);
        IEnumerable<StudentDTO> GetAll(int universityId);
        StudentDTO Get(int studentId, int universityId);
        Task<StudentDTO> EditAsync(EditStudentDTO addUnivesity);
        Task<int> RemoveAsync(RemoveStudentDTO addUnivesity);
    }
}
