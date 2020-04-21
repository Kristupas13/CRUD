using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.PresentationLayer;
using CRUDWebService.PresentationLayer.ViewModels;
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
        public async Task<IActionResult> Create(CreateUniversityViewModel viewModel)
        {
            var universityDto = new AddUnivesityDTO { Name = viewModel.Name, Address = viewModel.Address };
            var model = await _service.AddAsync(universityDto);
            if (model == null)
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error when creating an university" }) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            return Created($"http://localhost:44325/Universities/{model.UniversityId}", model);
        }

        [HttpGet]
        public IEnumerable<UniversityViewModel> GetAll()
        {
            return _service.GetAll().Select(p => new UniversityViewModel { UniversityId = p.UniversityId, Name = p.Name, Address = p.Address }).ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            var dto = _service.Get(id);
            if (dto == null)
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error when fetching an university: University not found" }) { StatusCode = (int)HttpStatusCode.NotFound };
            }
            return Ok(new UniversityViewModel { UniversityId = dto.UniversityId, Address = dto.Address, Name = dto.Name });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Edit(EditUniversityViewModel viewModel, int id)
        {
            var model = await _service.EditAsync(new EditUniversityDTO { UniversityId = id, Name = viewModel.Name, Address = viewModel.Address });
            if (model == null)
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error when editing an university: University not found" }) { StatusCode = (int)HttpStatusCode.NotFound };
            }
            return Ok(new UniversityViewModel { UniversityId = model.UniversityId, Address = model.Address, Name = model.Name });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _service.RemoveAsync(new RemoveUniversityDTO { UniversityId = id });

            if (status != -1)
                return new JsonResult(new ReturnMessage { MessageContent = "University has been deleted" }) { StatusCode = (int)HttpStatusCode.OK };
            else
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error: University not found" }) { StatusCode = (int)HttpStatusCode.NotFound };
        }
    }
}
