using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CRUDWebService.PresentationLayer.Controllers
{
    [ApiController]
    [Route("universities/{universityId}/[controller]")]
    public class StudentsController : Controller
    {
        [HttpPost]
        public IEnumerable<string> Create()
        {
            return new List<string> { "Create student endpoint called" };
        }

        [HttpGet]
        public IEnumerable<string> GetAll()
        {
            return new List<string> { "GetAll student endpoint called" };
        }

        [HttpGet]
        [Route("{studentId}")]
        public IEnumerable<string> Get(int universityId, int studentId)
        {
            return new List<string> { $"Get student endpoint called with Id = {studentId}" };
        }

        [HttpPut]
        [Route("{studentId}")]
        public IEnumerable<string> Edit(int universityId, int studentId)
        {
            return new List<string> { $"Edit student endpoint called with Id = {studentId}" };
        }

        [HttpDelete]
        [Route("{studentId}")]
        public IEnumerable<string> Delete(int universityId, int studentId)
        {
            return new List<string> { $"Delete student endpoint called with Id = {studentId}" };
        }
    }
}