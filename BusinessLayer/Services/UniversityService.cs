using CRUDWebService.BusinessLayer.Base.DTO;
using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.BusinessLayer.DTO.University;
using CRUDWebService.DataLayer.Context;
using CRUDWebService.DataLayer.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
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

        #region FirstTask

        public async Task<UniversityDTO> AddAsync(AddUnivesityDTO addUnivesity)
        {
            try
            {
                var model = _db.Universities.Add(new University
                {
                    Address = addUnivesity.Address,
                    Name = addUnivesity.Name
                }).Entity;
                await _db.SaveChangesAsync();

                IList<UniversityBookInformation> books = new List<UniversityBookInformation>();

                foreach (var book in addUnivesity.Books)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        await AddBookToLibraryService(book);

                        var requestURI = BaseBookServiceUri.ToString() + "books/" + book.ISBN;
                        var response = await GetResponseAsync(requestURI);
                        var bookInformation = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync())?.Knyga;

                        var addedBook = _db.UniversityBooks
                                    .Add(new UniversityBook
                                    {
                                        UniversityId = model.UniversityId,
                                        BookISBN = book.ISBN,
                                        AvailableFrom = book.AvailableFrom,
                                        IsAvailable = book.IsAvailable,
                                    });

                        await _db.SaveChangesAsync();
                        books.Add(new UniversityBookInformation
                        {
                            Autorius = bookInformation?.Autorius ?? "Missing information",
                            ISBN = book.ISBN,
                            AvailableFrom = book.AvailableFrom,
                            IsAvailable = book.IsAvailable,
                            Metai = bookInformation?.Metai ?? 0,
                            Pavadinimas = bookInformation?.Pavadinimas ?? "Missing information"
                        });
                    }
                }

                return new UniversityDTO { Address = addUnivesity.Address, Name = addUnivesity.Name, UniversityId = model.UniversityId, UniversityBooks = books.Select(p => new UniversityBookInformation { AvailableFrom = p.AvailableFrom, IsAvailable = p.IsAvailable, Autorius = p.Autorius, Pavadinimas = p.Pavadinimas, Metai = p.Metai, ISBN = p.ISBN }).ToList() };

            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<UniversityDTO> EditAsync(EditUniversityDTO editUniversity)
        {
            var universityEntity = _db.Universities.FirstOrDefault(p => p.UniversityId == editUniversity.UniversityId);
            universityEntity.Name = editUniversity.Name;
            universityEntity.Address = editUniversity.Address;
            await _db.SaveChangesAsync();

            IList<BookDTO> booksInformation = new List<BookDTO>();
            var universityBookEntities = _db.UniversityBooks.Where(p => p.UniversityId == editUniversity.UniversityId).ToList();
            RemoveAllUniversityBooks(editUniversity.UniversityId);
            try
            {
                IList<UniversityDTO> listOfUniversitiesWithBooks = new List<UniversityDTO>();

                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.ToString() + "books";
                    var response = await GetResponseAsync(requestURI);
                    if (response != null)
                        booksInformation = JsonConvert.DeserializeObject<RootBookList>(await response.Content.ReadAsStringAsync())?.Knygos;
                }

                foreach (var booksToEdit in editUniversity.Books)
                {
                    var universityBookEntity = universityBookEntities.FirstOrDefault(p => p.BookISBN == booksToEdit.ISBN);
                    if (universityBookEntity != null)
                    {
                        universityBookEntity.AvailableFrom = booksToEdit.AvailableFrom;
                        universityBookEntity.IsAvailable = booksToEdit.IsAvailable;
                        await EditLibraryBookService(new BookDTO { Pavadinimas = booksToEdit.Pavadinimas, Metai = booksToEdit.Metai, ISBN = booksToEdit.ISBN, Autorius = booksToEdit.Autorius });

                    }
                    else
                    {
                        // create
                        var newUniversityBook = new UniversityBook
                        {
                            UniversityId = editUniversity.UniversityId,
                            AvailableFrom = booksToEdit.AvailableFrom,
                            BookISBN = booksToEdit.ISBN,
                            IsAvailable = booksToEdit.IsAvailable
                        };
                        await AddBookToLibraryService(new BookDTO { Autorius = booksToEdit.Autorius, ISBN = booksToEdit.ISBN, Metai = booksToEdit.Metai, Pavadinimas = booksToEdit.Pavadinimas });

                        _db.UniversityBooks.Add(newUniversityBook);
                    }
                    await _db.SaveChangesAsync();
                }

                var books = await Get(editUniversity.UniversityId);

                return books;
            }
            catch
            {
                return null;
            }
        }

        private void RemoveAllUniversityBooks(int universityId)
        {
            var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == universityId).ToList();
            _db.UniversityBooks.RemoveRange(universityBooks);
            _db.SaveChanges();
        }

        public async Task<UniversityDTO> Get(int id)
        {
            UniversityDTO universityWithBooks = new UniversityDTO() { UniversityBooks = new List<UniversityBookInformation>() };
            try
            {
                var entity = _db.Universities.Find(id);
                var booksInformation = new BookDTO();
                var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == entity.UniversityId).ToList();

                foreach (var book in universityBooks)
                {
                    using (var client = new HttpClient())
                    {
                        var requestURI = BaseBookServiceUri.ToString() + "books/" + book.BookISBN;
                        var response = await GetResponseAsync(requestURI);
                        if (response != null)
                            booksInformation = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync())?.Knyga;
                    }

                    universityWithBooks.Address = entity.Address;
                    universityWithBooks.Name = entity.Name;
                    universityWithBooks.UniversityId = entity.UniversityId;
                    universityWithBooks.UniversityBooks.Add(GetBookInformation(booksInformation, book));
                }

                return universityWithBooks;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<UniversityDTO>> GetAll()
        {
            IList<UniversityDTO> listOfUniversitiesWithBooks = new List<UniversityDTO>();
            IList<BookDTO> booksInformation = new List<BookDTO>();

            using (var client = new HttpClient())
            {
                var requestURI = BaseBookServiceUri.ToString() + "books";
                var response = await GetResponseAsync(requestURI);
                if (response != null)
                    booksInformation = JsonConvert.DeserializeObject<RootBookList>(await response.Content.ReadAsStringAsync())?.Knygos;
            }

            var universities = _db.Universities.ToList();
            foreach (var university in universities)
            {
                var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == university.UniversityId).ToList();
                var universityInformation = new UniversityDTO { UniversityId = university.UniversityId, Address = university.Address, Name = university.Name, UniversityBooks = new List<UniversityBookInformation>() };

                foreach (var book in universityBooks)
                {
                    var bookInformation = booksInformation?.FirstOrDefault(p => p.ISBN == book.BookISBN);
                    universityInformation.UniversityBooks.Add(GetBookInformation(bookInformation, book));
                }
                listOfUniversitiesWithBooks.Add(universityInformation);
            }
            return listOfUniversitiesWithBooks;
        }

        public async Task<int> RemoveAsync(RemoveUniversityDTO removeUniversity)
        {
            var university = _db.Universities.SingleOrDefault(p => p.UniversityId == removeUniversity.UniversityId);
            var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == removeUniversity.UniversityId).ToList();


            if (university == null)
                return -1;

            _db.Universities.Remove(university);
            universityBooks.ForEach(p => _db.UniversityBooks.Remove(p));
            await _db.SaveChangesAsync();

            return 1;
        }
        #endregion


        public async Task<IEnumerable<UniversityBookDTO>> GetUniversityBooks(int universityId)
        {
            var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == universityId).ToList();
            using (var client = new HttpClient())
            {
                var requestURI = BaseBookServiceUri.ToString() + "books";
                var response = await GetResponseAsync(requestURI);
                if (response == null)
                    return null;

                var convertedContent = JsonConvert.DeserializeObject<RootBookList>(await response.Content.ReadAsStringAsync());
                return MergeUniversityWithBookInformation(universityBooks, convertedContent.Knygos);
            }
        }

        public async Task<UniversityBookDTO> GetUniversityBookByISBN(int universityId, string bookISBN)
        {
            var universityBook = _db.UniversityBooks.Where(p => p.UniversityId == universityId && p.BookISBN == bookISBN).FirstOrDefault();
            if (universityBook != null)
            {
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.ToString() + "books/" + bookISBN;
                    var response = await GetResponseAsync(requestURI);

                    if (response == null)
                    {
                        return new UniversityBookDTO
                        {
                            IsError = true,
                            ErrorMessage = $"Unexpected error has occured. Library service could not be found or is not running.",
                            StatusCode = System.Net.HttpStatusCode.ServiceUnavailable
                        };
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new UniversityBookDTO
                        {
                            UniversityId = universityId,
                            Autorius = "Missing information",
                            ISBN = "Missing information",
                            IsAvailable = universityBook.IsAvailable,
                            Metai = 0,
                            Pavadinimas = "Missing information",
                            AvailableFrom = universityBook.AvailableFrom,
                            IsError = false
                        };
                    }

                    var convertedContent = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync());

                    return new UniversityBookDTO
                    {
                        UniversityId = universityId,
                        Autorius = convertedContent.Knyga.Autorius,
                        ISBN = convertedContent.Knyga.ISBN,
                        IsAvailable = universityBook.IsAvailable,
                        Metai = convertedContent.Knyga.Metai,
                        Pavadinimas = convertedContent.Knyga.Pavadinimas,
                        AvailableFrom = universityBook.AvailableFrom,
                        IsError = false
                    };
                }
            }
            else
                return new UniversityBookDTO { IsError = true, ErrorMessage = "Book does not exist in this university.", StatusCode = System.Net.HttpStatusCode.BadRequest };
        }

        public async Task<UniversityBookModifiedDTO> AddBookToUniversityAsync(UniversityBookDTO universityBook)
        {
            if (!_db.UniversityBooks.Any(p => p.UniversityId == universityBook.UniversityId && p.BookISBN == universityBook.ISBN))
            {
                _db.UniversityBooks.Add(new UniversityBook { UniversityId = universityBook.UniversityId, BookISBN = universityBook.ISBN, AvailableFrom = DateTime.UtcNow, IsAvailable = true });
                await _db.SaveChangesAsync();

                await AddBookToLibraryService(new BookDTO { Autorius = universityBook.Autorius, ISBN = universityBook.ISBN, Metai = universityBook.Metai, Pavadinimas = universityBook.Pavadinimas });
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.ToString() + "books/" + universityBook.ISBN;
                    var response = await GetResponseAsync(requestURI);
                    if (response == null)
                    {
                        return new UniversityBookModifiedDTO
                        {
                            IsError = true,
                            ErrorMessage = $"Unexpected error has occured. Library service could not be found or is not running.",
                            StatusCode = System.Net.HttpStatusCode.ServiceUnavailable
                        };
                    }
                    var bookInformation = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync())?.Knyga;
                    return new UniversityBookModifiedDTO
                    { 
                        IsError = false, 
                        UniversityId = universityBook.UniversityId, 
                        BookISBN = universityBook.ISBN, 
                        AvailableFrom = DateTime.UtcNow, 
                        IsAvailable = true,
                        Autorius = bookInformation?.Autorius ?? "Missing information",
                        Metai = bookInformation?.Metai ?? 0,
                        Pavadinimas = bookInformation?.Pavadinimas ?? "Missing information"
                    };

                }
            }
            else
                return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Book already exists in this university.", StatusCode = System.Net.HttpStatusCode.BadRequest };

        }

        public async Task<UniversityBookModifiedDTO> EditUniversityBookAsync(UniversityBookModifiedDTO universityBookEdited)
        {
            var bookToUpdate = _db.UniversityBooks.Where(p => p.UniversityId == universityBookEdited.UniversityId && p.BookISBN == universityBookEdited.BookISBN).FirstOrDefault();

            if (bookToUpdate != null)
            {
                bookToUpdate.AvailableFrom = universityBookEdited.AvailableFrom;
                bookToUpdate.IsAvailable = universityBookEdited.IsAvailable;

                _db.UniversityBooks.Update(bookToUpdate);
                await _db.SaveChangesAsync();

                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.ToString() + "books/" + universityBookEdited.BookISBN;
                    var response = await GetResponseAsync(requestURI);
                    if (response == null)
                    {
                        return new UniversityBookModifiedDTO
                        {
                            IsError = true,
                            ErrorMessage = $"Unexpected error has occured. Library service could not be found or is not running.",
                            StatusCode = System.Net.HttpStatusCode.ServiceUnavailable
                        };
                    }
                    var bookInformation = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync())?.Knyga;
                    await EditLibraryBookService(new BookDTO { Pavadinimas = universityBookEdited.Pavadinimas, Metai = universityBookEdited.Metai, Autorius = universityBookEdited.Autorius, ISBN = universityBookEdited.BookISBN });


                    return new UniversityBookModifiedDTO
                    {
                        Autorius = bookInformation?.Autorius ?? "Missing information",
                        BookISBN = universityBookEdited.BookISBN,
                        AvailableFrom = universityBookEdited.AvailableFrom,
                        IsAvailable = universityBookEdited.IsAvailable,
                        Metai = bookInformation?.Metai ?? 0,
                        Pavadinimas = bookInformation?.Pavadinimas ?? "Missing information",
                        UniversityId = universityBookEdited.UniversityId
                    };

                }
            }
            else
                return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "This book does not exist in this university.", StatusCode = System.Net.HttpStatusCode.BadRequest };

        }

        public async Task<UniversityBookModifiedDTO> RemoveUniversityBookAsync(int universityId, string bookISBN)
        {
            var universityBookToRemove = _db.UniversityBooks.Where(p => p.UniversityId == universityId && p.BookISBN == bookISBN).FirstOrDefault();

            if (universityBookToRemove != null)
            {
                _db.UniversityBooks.Remove(universityBookToRemove);
                await _db.SaveChangesAsync();
                return new UniversityBookModifiedDTO { IsError = false };
            }
            else
                return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "This book does not exist in this university.", StatusCode = System.Net.HttpStatusCode.BadRequest };
        }

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

        private IEnumerable<UniversityBookDTO> MergeUniversityWithBookInformation(List<UniversityBook> universityBooks, List<BookDTO> booksInformations)
        {
            foreach (var universityBook in universityBooks)
            {
                var bookInfo = booksInformations.FirstOrDefault(p => p.ISBN == universityBook.BookISBN);

                if (bookInfo != null)
                {
                    yield return new UniversityBookDTO
                    {
                        ISBN = bookInfo.ISBN,
                        Autorius = bookInfo.Autorius,
                        IsAvailable = universityBook.IsAvailable,
                        Metai = bookInfo.Metai,
                        Pavadinimas = bookInfo.Pavadinimas,
                        AvailableFrom = universityBook.AvailableFrom,
                        UniversityId = universityBook.UniversityId,
                        IsError = false
                    };
                }
                else
                {
                    yield return new UniversityBookDTO
                    {
                        ISBN = universityBook.BookISBN,
                        IsAvailable = universityBook.IsAvailable,
                        AvailableFrom = universityBook.AvailableFrom,
                        Autorius = "Missing information",
                        Metai = 0,
                        Pavadinimas = "Missing information",
                        UniversityId = universityBook.UniversityId
                    };
                }
            }
        }

        private UniversityBookInformation GetBookInformation(BookDTO bookInformation, UniversityBook book)
        {
            return new UniversityBookInformation
            {
                Autorius = bookInformation?.Autorius ?? "Missing information",
                AvailableFrom = book.AvailableFrom,
                IsAvailable = book.IsAvailable,
                ISBN = book.BookISBN,
                Metai = bookInformation?.Metai ?? 0,
                Pavadinimas = bookInformation?.Pavadinimas ?? "Missing information",
            };
        }

        private async Task<HttpResponseMessage> AddBookToLibraryService(BookDTO bookDto)
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

        private async Task<HttpResponseMessage> EditLibraryBookService(BookDTO bookDto)
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
    }
}
