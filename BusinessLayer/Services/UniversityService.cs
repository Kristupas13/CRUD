using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.DataLayer.Context;
using CRUDWebService.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly UniversityContext _db;
        //     private StringBuilder BaseBookServiceUri = new StringBuilder(@"http://external:80/");
        private StringBuilder BaseBookServiceUri = new StringBuilder(@"http://localhost:4777/");


        public UniversityService(UniversityContext db)
        {
            _db = db;
        }

        public string Ping()
        {
            return "Hello world";
        }

        #region FirstTask

        public async Task<UniversityBookDTO> AddAsync(AddUnivesityDTO addUnivesity)
        {
            try
            {
                var newUniversityEntity = await AddNewUniversityEntityAsync(addUnivesity.Address, addUnivesity.Name).ConfigureAwait(false);

                var bookDto = new UniversityBookDTO
                {
                    UniversityId = newUniversityEntity.UniversityId,
                    Address = addUnivesity.Address,
                    Name = addUnivesity.Name,
                    Books = new List<UniversityBookInformation>()
                };

                foreach (var book in addUnivesity.Books)
                {
                    await AddNewUniversityBook(newUniversityEntity.UniversityId, book.ISBN, book.IsAvailable, book.AvailableFrom);

                    var bookToCompose = new BookDTO();

                    var bookFromExternalService = await GetBookInformationFromExternalServiceAsync(book.ISBN);
                    if (bookFromExternalService == null)
                    {
                        bookToCompose = new BookDTO { Autorius = book.Autorius, Metai = book.Metai, ISBN = book.ISBN, Pavadinimas = book.Pavadinimas };
                        await AddBookToExternalServiceAsync(bookToCompose);
                    }
                    else
                    {
                        bookToCompose = bookFromExternalService;
                    }

                    bookDto.Books.Add(ComposeBookInformation(bookToCompose, book.AvailableFrom, book.IsAvailable));
                }

                return bookDto;
            }
            catch
            {
                return null;
            }
        }

        public async Task<EditUniversityDTO> EditAsync(EditUniversityDTO editUniversity)
        {
            var universityEntity = await EditUniversityEntityAsync(editUniversity.UniversityId, editUniversity.Address, editUniversity.Name);
            var externalServiceBooksInformation = await GetBooksInformationFromExternalServiceAsync();
            await RemoveAllUniversityBooks(editUniversity.UniversityId);

            try
            {

                foreach (var booksToEdit in editUniversity.Books)
                {
                    await AddNewUniversityBook(universityEntity.UniversityId, booksToEdit.ISBN, booksToEdit.IsAvailable, booksToEdit.AvailableFrom);

                    if (externalServiceBooksInformation.Any(p => p.ISBN == booksToEdit.ISBN))
                        await EditExternalServiceBookAsync(new BookDTO { Autorius = booksToEdit.Autorius, ISBN = booksToEdit.ISBN, Metai = booksToEdit.Metai, Pavadinimas = booksToEdit.Pavadinimas });
                    else
                        await AddBookToExternalServiceAsync(new BookDTO { Autorius = booksToEdit.Autorius, ISBN = booksToEdit.ISBN, Metai = booksToEdit.Metai, Pavadinimas = booksToEdit.Pavadinimas });
                }

                return editUniversity;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UniversityBookDTO> Get(int id)
        {
            try
            {
                var universityEntity = _db.Universities.Find(id);
                if (universityEntity == null)
                    return new UniversityBookDTO();

                var universityBookEntities = _db.UniversityBooks.Where(p => p.UniversityId == id).ToList();

                var universityBookDto = new UniversityBookDTO
                {
                    UniversityId = universityEntity.UniversityId,
                    Address = universityEntity.Address,
                    Name = universityEntity.Name,
                    Books = new List<UniversityBookInformation>()
                };

                foreach (var book in universityBookEntities)
                {
                    var bookDto = await GetBookInformationFromExternalServiceAsync(book.BookISBN);
                    universityBookDto.Books.Add(ComposeBookInformation(bookDto, book.AvailableFrom, book.IsAvailable));
                }

                return universityBookDto;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<UniversityBookDTO>> GetAll()
        {
            try
            {
                var bookEntities = _db.Universities.ToList();

                var universityBooksDto = new List<UniversityBookDTO>();

                foreach (var book in bookEntities)
                {
                    var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == book.UniversityId).ToList();

                    var universityWithBookInformation = new UniversityBookDTO
                    {
                        UniversityId = book.UniversityId,
                        Address = book.Address,
                        Name = book.Name,
                        Books = new List<UniversityBookInformation>()
                    };

                    foreach (var universityBook in universityBooks)
                    {
                        var bookDto = await GetBookInformationFromExternalServiceAsync(universityBook.BookISBN);
                        universityWithBookInformation.Books.Add(ComposeBookInformation(bookDto, universityBook.AvailableFrom, universityBook.IsAvailable));
                    }

                    universityBooksDto.Add(universityWithBookInformation);
                }

                return universityBooksDto;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> RemoveAsync(int universityId)
        {
            var university = _db.Universities.SingleOrDefault(p => p.UniversityId == universityId);
            var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == universityId).ToList();


            if (university == null)
                return -1;

            _db.Universities.Remove(university);
            universityBooks.ForEach(p => _db.UniversityBooks.Remove(p));
            await _db.SaveChangesAsync();

            return 1;
        }
        #endregion

        private async Task<HttpResponseMessage> GetResponseAsync(string requestURI)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetAsync(requestURI);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private async Task<HttpResponseMessage> AddBookToExternalServiceAsync(BookDTO bookDto)
        {
            using (HttpClient client = new HttpClient())
            {
                var postRequestURI = BaseBookServiceUri.ToString() + "books";
                var postBook = new BookDTO
                {
                    ISBN = bookDto.ISBN,
                    Autorius = bookDto.Autorius,
                    Metai = bookDto.Metai,
                    Pavadinimas = bookDto.Pavadinimas
                };
                var json = JsonConvert.SerializeObject(postBook);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync(postRequestURI, data);
                return postResponse;
            }
        }

        private async Task<HttpResponseMessage> EditExternalServiceBookAsync(BookDTO bookDto)
        {
            using (HttpClient client = new HttpClient())
            {
                var postRequestURI = BaseBookServiceUri.ToString() + "books/" + bookDto.ISBN;
                var postBook = new BookDTO
                {
                    Autorius = bookDto.Autorius,
                    Metai = bookDto.Metai,
                    Pavadinimas = bookDto.Pavadinimas
                };
                var json = JsonConvert.SerializeObject(postBook);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await client.PutAsync(postRequestURI, data);
                return postResponse;
            }
        }

        private async Task<University> AddNewUniversityEntityAsync(string address, string name)
        {
            var model = _db.Universities.Add(new University
            {
                Address = address,
                Name = name
            }).Entity;

            await _db.SaveChangesAsync();

            return model;
        }

        private async Task<University> EditUniversityEntityAsync(int universityId, string address, string name)
        {
            var entity = _db.Universities.Find(universityId);
            entity.Name = name;
            entity.Address = address;
            _db.Universities.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }

        private async Task<UniversityBook> AddNewUniversityBook(int universityId, string isbn, bool isAvailable, DateTime availableFrom)
        {
            var model = _db.UniversityBooks.Add(new UniversityBook
            {
                UniversityId = universityId,
                BookISBN = isbn,
                IsAvailable = isAvailable,
                AvailableFrom = availableFrom
            }).Entity;

            await _db.SaveChangesAsync();

            return model;
        }

        private async Task RemoveAllUniversityBooks(int universityId)
        {
            var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == universityId).ToList();
            _db.UniversityBooks.RemoveRange(universityBooks);
            await _db.SaveChangesAsync();
        }

        private async Task<BookDTO> GetBookInformationFromExternalServiceAsync(string isbn)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestURI = BaseBookServiceUri.ToString() + "books/" + isbn;
                var response = await GetResponseAsync(requestURI);
                var bookInformation = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync())?.Knyga;
                return bookInformation;
            }
        }

        private async Task<List<BookDTO>> GetBooksInformationFromExternalServiceAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var requestURI = BaseBookServiceUri.ToString() + "books";
                var response = await GetResponseAsync(requestURI);
                var booksInformation = JsonConvert.DeserializeObject<RootBookList>(await response.Content.ReadAsStringAsync())?.Knygos;
                return booksInformation;
            }
        }

        private UniversityBookInformation ComposeBookInformation(BookDTO dto, DateTime availableFrom, bool isAvailable)
        {
            return new UniversityBookInformation
            {
                Autorius = dto?.Autorius ?? "Missing information",
                ISBN = dto?.ISBN,
                AvailableFrom = availableFrom,
                IsAvailable = isAvailable,
                Metai = dto?.Metai ?? 0,
                Pavadinimas = dto?.Pavadinimas ?? "Missing information"
            };
        }
    }
}
