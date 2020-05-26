using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UniversitiesController(IUniversityService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
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


        [HttpGet]
        [Route("{universityId}/books")]
        public async Task<IActionResult> GetUniversityBooks(int universityId)
        {
            var universityBooks = await _service.GetUniversityBooks(universityId);
            if (universityBooks == null)
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error has occured. Library service could not be found or is not running." }) { StatusCode = (int)HttpStatusCode.ServiceUnavailable };
            else
            {
                if (!universityBooks.Any())
                    return NoContent();

                return Ok(universityBooks);
            }
        }

        [HttpGet]
        [Route("{universityId}/books/{bookISBN}")]
        public async Task<IActionResult> GetUniversityBookByISBN(int universityId, string bookISBN)
        {
            var universityBook = await _service.GetUniversityBookByISBN(universityId, bookISBN);
            if (universityBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = universityBook.ErrorMessage }) { StatusCode = (int)universityBook.StatusCode };
            else
            {
                return Ok(universityBook);
            }
        }

        [HttpPost]
        [Route("{universityId}/books")]
        public async Task<IActionResult> AddBookToUniversity(int universityId, UniversityBookDTO universityBookModel)
        {
            var addedBook = await _service.AddBookToUniversityAsync(universityBookModel);
            if (addedBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = addedBook.ErrorMessage }) { StatusCode = (int)addedBook.StatusCode };
            else
            {
                return Created($"http://localhost:8777/Universities/{addedBook.UniversityId}/books/{addedBook.BookISBN}", addedBook);
            }
        }

        [HttpPut]
        [Route("{universityId}/books")]
        public async Task<IActionResult> EditBookUniversity(UniversityBookModifiedDTO universityBookEdited, int universityId)
        {
            var editedBook = await _service.EditUniversityBookAsync(universityBookEdited);
            if (editedBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = editedBook.ErrorMessage }) { StatusCode = (int)editedBook.StatusCode };
            else
            {
                return Ok(editedBook);
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
