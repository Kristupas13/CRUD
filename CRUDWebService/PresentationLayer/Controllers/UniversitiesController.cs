using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.DataLayer.Context;
using CRUDWebService.DataLayer.Entities;
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
        [Route("create")]
        public async Task<UniversityViewModel> Create(CreateUniversityViewModel viewModel)
        {
            var universityDto = new AddUnivesityDTO { Name = viewModel.Name, Address = viewModel.Address };
            var model = await _service.AddAsync(universityDto);
            return new UniversityViewModel { UniversityId = model.UniversityId, Address = model.Address, Name = model.Name };
        }

        [HttpGet]
        public IEnumerable<UniversityViewModel> GetAll()
        {
            return _service.GetAll().Select(p => new UniversityViewModel { UniversityId = p.UniversityId, Name = p.Name, Address = p.Address }).ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public UniversityViewModel Get(int id)
        {
            var dto = _service.Get(id);
            return new UniversityViewModel { UniversityId = dto.UniversityId, Address = dto.Address, Name = dto.Name };
        }

        [HttpPut]
        [Route("edit")]
        public async Task<UniversityViewModel> Edit(EditUniversityViewModel viewModel)
        {
            var model = await _service.EditAsync(new EditUniversityDTO { UniversityId = viewModel.UniversityId, Name = viewModel.Name, Address = viewModel.Address });
            return new UniversityViewModel { UniversityId = model.UniversityId, Address = model.Address, Name = model.Name };
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<string> Delete(int id)
        {
            var status = await _service.RemoveAsync(new RemoveUniversityDTO { UniversityId = id });

            if (status != -1)
                return $"University with Id = {id} has been deleted";
            else
                return $"Error occured when deleting university with Id = {id}";
        }
    }
}
