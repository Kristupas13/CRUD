using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.PresentationLayer;
using Microsoft.AspNetCore.Mvc;

namespace CRUDWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UniversitiesController : Controller
    {
        private readonly IUniversityService _service;

        public UniversitiesController(IUniversityService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddUnivesityDTO universityModel)
        {
            var model = await _service.AddAsync(universityModel);
            if (model == null)
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error when creating an university" }) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            return Created($"http://localhost:44325/Universities/{model.UniversityId}", model);
        }

        [HttpGet]
        public async Task<IEnumerable<UniversityDTO>> GetAll()
        {
            return await _service.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.Get(id);
            if (dto == null)
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error when fetching an university: University not found" }) { StatusCode = (int)HttpStatusCode.NotFound };
            }
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Edit(EditUniversityDTO viewModel, int id)
        {
            var model = await _service.EditAsync(viewModel);
            if (model == null)
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error when editing an university: University not found" }) { StatusCode = (int)HttpStatusCode.NotFound };
            }
            return Ok(model);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _service.RemoveAsync(id);

            if (status != -1)
                return new JsonResult(new ReturnMessage { MessageContent = "University has been deleted" }) { StatusCode = (int)HttpStatusCode.OK };
            else
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error: University not found" }) { StatusCode = (int)HttpStatusCode.NotFound };
        }
    }
}
