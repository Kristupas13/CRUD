using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO.Student;
using CRUDWebService.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.PresentationLayer.Controllers
{
    [ApiController]
    [Route("universities/{universityId}/[controller]")]
    public class StudentsController : Controller
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("create")]
        public async Task<StudentViewModel> Create(CreateStudentViewModel viewModel, int universityId)
        {
            var studentDto = new AddStudentDTO { FirstName = viewModel.FirstName, LastName = viewModel.LastName, AverageGrade = viewModel.AverageGrade, UniversityId = universityId };
            var model = await _service.AddAsync(studentDto);
            return new StudentViewModel { StudentId = model.StudentId, FirstName = model.FirstName, LastName = model.LastName, AverageGrade = model.AverageGrade, UniversityId = model.UniversityId };
        }

        [HttpGet]
        public IEnumerable<StudentViewModel> GetAll(int universityId)
        {
            return _service.GetAll(universityId).Select(p => new StudentViewModel { StudentId = p.StudentId, FirstName = p.FirstName, LastName = p.LastName, AverageGrade = p.AverageGrade, UniversityId = p.UniversityId }).ToList();
        }

        [HttpGet]
        [Route("{studentId}")]
        public StudentViewModel Get(int studentId, int universityId)
        {
            var dto = _service.Get(studentId, universityId);
            return new StudentViewModel { StudentId = dto.StudentId, FirstName = dto.FirstName, LastName = dto.LastName, AverageGrade = dto.AverageGrade, UniversityId = dto.UniversityId };
        }

        [HttpPut]
        [Route("edit")]
        public async Task<StudentViewModel> Edit(EditStudentViewModel viewModel, int universityId)
        {
            var model = await _service.EditAsync(new EditStudentDTO { FirstName = viewModel.FirstName, LastName = viewModel.LastName, AverageGrade = viewModel.AverageGrade, StudentId = viewModel.StudentId, UniversityId = universityId });
            return new StudentViewModel { StudentId = model.StudentId, FirstName = model.FirstName, LastName = model.LastName, UniversityId = model.UniversityId, AverageGrade = model.AverageGrade };
        }

        [HttpDelete]
        [Route("delete/{studentId}")]
        public async Task<string> Delete(int studentId)
        {
            var status = await _service.RemoveAsync(new RemoveStudentDTO { StudentId = studentId });

            if (status != -1)
                return $"University with Id = {studentId} has been deleted";
            else
                return $"Error occured when deleting university with Id = {studentId}";
        }
    }
}