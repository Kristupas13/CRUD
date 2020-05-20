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
        public async Task<IActionResult> Create(CreateUniversityViewModel viewModel)
        {
            var universityDto = new AddUnivesityDTO { Name = viewModel.Name, Address = viewModel.Address, Books = viewModel.Books.Select(p => new UniversityBookDTO { ISBN = p.ISBN, IsAvailable = p.IsAvailable, AvailableFrom = p.AvailableFrom, Autorius = p.Autorius, Pavadinimas = p.Pavadinimas, Metai = p.Metai }).ToList()};
            var model = await _service.AddAsync(universityDto);
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
            return Ok(new UniversityViewModel { UniversityId = dto.UniversityId, Address = dto.Address, Name = dto.Name, Books = dto.UniversityBooks });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Edit(EditUniversityViewModel viewModel, int id)
        {
            var model = await _service.EditAsync(new EditUniversityDTO { UniversityId = id, Name = viewModel.Name, Address = viewModel.Address, Books = viewModel.Books });
            if (model == null)
            {
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error when editing an university: University not found" }) { StatusCode = (int)HttpStatusCode.NotFound };
            }
            return Ok(new UniversityViewModel { UniversityId = model.UniversityId, Address = model.Address, Name = model.Name, Books = model.UniversityBooks });
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
                return new JsonResult(new ReturnMessage { MessageContent = "Unexpected error has occured. Library service could not be found or is not running." }) { StatusCode = (int)HttpStatusCode.ServiceUnavailable };
            else
            {
                if (!universityBooks.Any())
                    return NoContent();

                return Ok(universityBooks.Select(p => new UniversityBookInformationViewModel { Author = p.Autorius, Title = p.Pavadinimas, ISBN = p.ISBN, Year = p.Metai, IsAvailable = p.IsAvailable, AvailableFrom = p.AvailableFrom }));
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
                return Ok(new UniversityBookInformationViewModel { Author = universityBook.Autorius, Title = universityBook.Pavadinimas, ISBN = universityBook.ISBN, Year = universityBook.Metai, AvailableFrom = universityBook.AvailableFrom, IsAvailable = universityBook.IsAvailable  });
            }
        }

        [HttpPost]
        [Route("{universityId}/books")]
        public async Task<IActionResult> AddBookToUniversity(int universityId, BookViewModel bookViewModel)
        {
            var addedBook = await _service.AddBookToUniversityAsync(new UniversityBookDTO { UniversityId = universityId, ISBN = bookViewModel.ISBN, Autorius = bookViewModel.Autorius, Metai = bookViewModel.Metai, Pavadinimas = bookViewModel.Pavadinimas, AvailableFrom = bookViewModel.AvailableFrom, IsAvailable = bookViewModel.IsAvailable });
            if (addedBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = addedBook.ErrorMessage }) { StatusCode = (int)addedBook.StatusCode };
            else
            {
                var model = new UniversityBookViewModel { BookISBN = addedBook.BookISBN, UniversityId = addedBook.UniversityId, IsAvailable = addedBook.IsAvailable, AvailableFrom = addedBook.AvailableFrom, Autorius = addedBook.Autorius, Metai = addedBook.Metai, Pavadinimas = addedBook.Pavadinimas };
                return Created($"http://localhost:8777/Universities/{model.UniversityId}/books/{model.BookISBN}", model);
            }
        }

        [HttpPut]
        [Route("{universityId}/books")]
        public async Task<IActionResult> EditBookUniversity(UniversityModifiedViewModel universityBookEdited, int universityId)
        {
            var editedBook = await _service.EditUniversityBookAsync(new UniversityBookModifiedDTO { BookISBN = universityBookEdited.BookISBN, AvailableFrom = universityBookEdited.AvailableFrom, IsAvailable = universityBookEdited.IsAvailable, UniversityId = universityId, Autorius = universityBookEdited.Autorius, Metai = universityBookEdited.Metai, Pavadinimas = universityBookEdited.Pavadinimas });
            if (editedBook.IsError)
                return new JsonResult(new ReturnMessage { MessageContent = editedBook.ErrorMessage }) { StatusCode = (int)editedBook.StatusCode };
            else
            {
                return Ok(new UniversityBookViewModel { BookISBN = editedBook.BookISBN, UniversityId = editedBook.UniversityId, IsAvailable = editedBook.IsAvailable, AvailableFrom = editedBook.AvailableFrom, Pavadinimas = editedBook.Pavadinimas, Metai = editedBook.Metai, Autorius = editedBook.Autorius  });
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
