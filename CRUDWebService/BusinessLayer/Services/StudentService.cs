using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO.Student;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.DataLayer.Context;
using CRUDWebService.DataLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Services
{
    public class StudentService : IStudentService
    {
        private readonly UniversityContext _db;

        public StudentService(UniversityContext db)
        {
            _db = db;
        }

        public async Task<StudentDTO> AddAsync(AddStudentDTO addStudent)
        {
            try
            {
                var model = _db.Students.Add(new Student
                {
                    UniversityId = addStudent.UniversityId,
                    FirstName = addStudent.FirstName,
                    LastName = addStudent.LastName,
                    AverageGrade = addStudent.AverageGrade
                }).Entity;
                await _db.SaveChangesAsync();

                return new StudentDTO { StudentId = model.StudentId, FirstName = model.FirstName, LastName = model.LastName, AverageGrade = model.AverageGrade, UniversityId = model.UniversityId };
            }
            catch
            {
                return null;
            }
        }

        public async Task<StudentDTO> EditAsync(EditStudentDTO editStudent)
        {
            try
            {
                var entity = await _db.Students.FindAsync(editStudent.StudentId);

                entity.FirstName = string.IsNullOrEmpty(editStudent.FirstName) ? entity.FirstName : editStudent.FirstName;
                entity.LastName = string.IsNullOrEmpty(editStudent.LastName) ? entity.LastName : editStudent.FirstName;
                entity.AverageGrade = editStudent.AverageGrade.HasValue ? entity.AverageGrade : editStudent.AverageGrade.Value;
                entity.UniversityId = editStudent.UniversityId.HasValue ? entity.AverageGrade : editStudent.UniversityId.Value;

                await _db.SaveChangesAsync();

                return new StudentDTO { StudentId = entity.StudentId, FirstName = entity.FirstName, LastName = entity.LastName, AverageGrade = entity.AverageGrade };
            }
            catch
            {
                return null;
            }

        }

        public StudentDTO Get(int studentId, int universityId)
        {
            try
            {
                var entity = _db.Students.Find(studentId);

                return new StudentDTO { StudentId = entity.StudentId, FirstName = entity.FirstName, LastName = entity.LastName, AverageGrade = entity.AverageGrade, UniversityId = entity.UniversityId };
            }
            catch
            {
                return null;
            }
          }

        public IEnumerable<StudentDTO> GetAll(int universityId)
        {
            return _db.Students.Where(p => p.UniversityId == universityId).Select(p => new StudentDTO { StudentId = p.StudentId, FirstName = p.FirstName, LastName = p.LastName, AverageGrade = p.AverageGrade, UniversityId = p.UniversityId });
        }

        public async Task<int> RemoveAsync(RemoveStudentDTO removeStudent)
        {
            var student = _db.Students.SingleOrDefault(p => p.StudentId == removeStudent.StudentId);

            if (student == null)
                return -1;

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();

            return 1;
        }
    }
}
