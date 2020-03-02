using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO;
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
        private readonly UniversityContext _db;
        private readonly IUniversityService _service;

        public UniversitiesController(UniversityContext db, IUniversityService service)
        {
            _db = db;
            _service = service;
        }

        [HttpPost]
        public async Task<IEnumerable<string>> Create(CreateUniversityViewModel viewModel)
        {
            var universityDto = new AddUnivesityDTO { Name = viewModel.Name, Address = viewModel.Address };
            await _service.AddAsync(universityDto);
            return new List<string> { "Create endpoint called" };
        }

        [HttpGet]
        public IEnumerable<UniversityViewModel> GetAll()
        {
            return _service.GetAll().Select(p => new UniversityViewModel { Name = p.Name, Address = p.Address }).ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public UniversityViewModel Get(int id)
        {
            var dto = _service.Get(id);
            return new UniversityViewModel { Address = dto.Address, Name = dto.Name };
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IEnumerable<string>> Edit(EditUniversityViewModel viewModel)
        {
            await _service.EditAsync(new EditUniversityDTO { UniversityId = viewModel.UniversityId, Name = viewModel.Name, Address = viewModel.Address });
            return new List<string> { $"Edit endpoint called with Id = {viewModel.UniversityId}" };
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IEnumerable<string>> Delete(int id)
        {
            await _service.RemoveAsync(new RemoveUniversityDTO { UniversityId = id });
            return new List<string> { $"Delete endpoint called with Id = {id}" };
        }
    }
}
