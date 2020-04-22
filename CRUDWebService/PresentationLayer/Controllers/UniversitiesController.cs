using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO;
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

        [HttpGet]
        [Route("{universityId}/books")]
        public async Task<IActionResult> GetUniversityBooks(int universityId)
        {
            var universityBooks = await _service.GetUniversityBooks(universityId);
            if (universityBooks == null)
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error has occured. Please try again later or contact support." }) { StatusCode = (int)HttpStatusCode.BadRequest };
            else
            {
                if (!universityBooks.Any())
                    return new JsonResult(new ReturnMessage { MessageContent = "No books found by that university id." }) { StatusCode = (int)HttpStatusCode.NotFound };

                return Ok(universityBooks.Select(p => new UniversityBookViewModel { Author = p.Autorius, Title = p.Pavadinimas, ISBN = p.ISBN, Year = p.Metai }));
            }
        }

        [HttpGet]
        [Route("{universityId}/books/{bookISBN}")]
        public async Task<IActionResult> GetUniversityBookByISBN(int universityId, string bookISBN)
        {
            var universityBook = await _service.GetUniversityBookByISBN(universityId, bookISBN);
            if (universityBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = universityBook.ErrorMessage }) { StatusCode = (int)HttpStatusCode.BadRequest };
            else
            {
                return Ok(new UniversityBookViewModel { Author = universityBook.Autorius, Title = universityBook.Pavadinimas, ISBN = universityBook.ISBN, Year = universityBook.Metai });
            }
        }

        [HttpPost]
        [Route("{universityId}/books")]
        public async Task<IActionResult> AddBookToUniversity(string bookISBN, int universityId)
        {
            var addedBook = await _service.AddBookToUniversityAsync(universityId, bookISBN);
            if (addedBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = addedBook.ErrorMessage }) { StatusCode = (int)HttpStatusCode.BadRequest };
            else
            {
                return Ok(new UniversityBookReferenceViewModel { BookISBN = addedBook.BookISBN, UniversityId = addedBook.UniversityId });
            }
        }

        [HttpPut]
        [Route("{universityId}/books")]
        public async Task<IActionResult> EditBookUniversity(string universityBook, int universityId)
        {
            var editedBook = await _service.EditUniversityBookAsync(universityId, universityBook);
            if (editedBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = editedBook.ErrorMessage }) { StatusCode = (int)HttpStatusCode.BadRequest };
            else
            {
                return Ok(new UniversityBookReferenceViewModel { BookISBN = editedBook.BookISBN, UniversityId = editedBook.UniversityId });
            }
        }

        [HttpDelete]
        [Route("{universityId}/books/{bookISBN}")]
        public async Task<IActionResult> RemoveBookFromUniversity(int universityId, string bookISBN)
        {
            var removeStatus = await _service.RemoveUniversityBookAsync(universityId, bookISBN);
            if (removeStatus.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = removeStatus.ErrorMessage }) { StatusCode = (int)HttpStatusCode.BadRequest };
            else
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Book has been removed" }) { StatusCode = (int)HttpStatusCode.OK };
            }
        }
    }
}
