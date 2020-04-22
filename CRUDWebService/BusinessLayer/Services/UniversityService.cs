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

                return new UniversityDTO { Address = addUnivesity.Address, Name = addUnivesity.Name, UniversityId = model.UniversityId };
            }
            catch
            {
                return null;
            }
        }

        public async Task<UniversityDTO> EditAsync(EditUniversityDTO editUniversity)
        {
            try
            {
                var entity = _db.Universities.Find(editUniversity.UniversityId);
                entity.Name = string.IsNullOrEmpty(editUniversity.Name) ? entity.Name : editUniversity.Name;
                entity.Address = string.IsNullOrEmpty(editUniversity.Name) ? entity.Address : editUniversity.Name;
                await _db.SaveChangesAsync();

                return new UniversityDTO { Address = editUniversity.Address, Name = editUniversity.Name, UniversityId = entity.UniversityId };
            }
            catch
            {
                return null;
            }
        }

        public UniversityDTO Get(int id)
        {
            try
            {
                var entity = _db.Universities.Find(id);

                return new UniversityDTO { Address = entity.Address, Name = entity.Name };
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<UniversityDTO> GetAll()
        {
            return _db.Universities.Select(p => new UniversityDTO { Name = p.Name, Address = p.Address, UniversityId = p.UniversityId });
        }

        public async Task<int> RemoveAsync(RemoveUniversityDTO removeUniversity)
        {
            var university = _db.Universities.SingleOrDefault(p => p.UniversityId == removeUniversity.UniversityId);

            if (university == null)
                return -1;

            _db.Universities.Remove(university);
            await _db.SaveChangesAsync();

            return 1;
        }
        #endregion


        public async Task<IEnumerable<UniversityBookDTO>> GetUniversityBooks(int universityId)
        {
            var universityBooksISBN = _db.UniversityBooks.Where(p => p.UniversityId == universityId).Select(p => p.BookISBN);
            using (var client = new HttpClient())
            {
                var requestURI = BaseBookServiceUri.Append("books").ToString();
                var response = await client.GetAsync(requestURI);
                var convertedContent = JsonConvert.DeserializeObject<RootBookList>(await response.Content.ReadAsStringAsync());
                return convertedContent.Knygos.Where(p => universityBooksISBN.Contains(p.ISBN));
            }
        }

        public async Task<UniversityBookDTO> GetUniversityBookByISBN(int universityId, string bookISBN)
        {
            if (_db.UniversityBooks.Any(p => p.UniversityId == universityId && p.BookISBN == bookISBN))
            {
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
                    var response = await client.GetAsync(requestURI);
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new UniversityBookDTO { IsError = true, ErrorMessage = "Error: Book is missing from book database." };

                    var convertedContent = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync());
                    return convertedContent?.Knyga;
                }
            }
            else
                return new UniversityBookDTO { IsError = true, ErrorMessage = "Book does not exist in this university." };
        }

        public async Task<UniversityBookReferences> AddBookToUniversityAsync(int universityId, string bookISBN)
        {
            if (!_db.UniversityBooks.Any(p => p.UniversityId == universityId && p.BookISBN == bookISBN))
            {
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
                    var response = await client.GetAsync(requestURI);

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new UniversityBookReferences { IsError = true, ErrorMessage = "Error: Book is missing from book database." };
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _db.UniversityBooks.Add(new UniversityBook { UniversityId = universityId, BookISBN = bookISBN });
                        await _db.SaveChangesAsync();
                        return new UniversityBookReferences { IsError = false, UniversityId = universityId, BookISBN = bookISBN };
                    }

                    return new UniversityBookReferences { IsError = true, ErrorMessage = "Unexpected error" };
                }
            }
            else
                return new UniversityBookReferences { IsError = true, ErrorMessage = "Book already exists in this university." };

        }

        public async Task<UniversityBookReferences> EditUniversityBookAsync(int universityId, string bookISBN)
        {
            if (_db.UniversityBooks.Any(p => p.UniversityId == universityId && p.BookISBN == bookISBN))
            {
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
                    var response = await client.GetAsync(requestURI);

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new UniversityBookReferences { IsError = true, ErrorMessage = "Error: Book is missing from book database." };
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _db.UniversityBooks.Update(new UniversityBook { UniversityId = universityId, BookISBN = bookISBN });
                        await _db.SaveChangesAsync();
                        return new UniversityBookReferences { IsError = false, UniversityId = universityId, BookISBN = bookISBN };
                    }
                    return new UniversityBookReferences { IsError = true, ErrorMessage = "Unexpected error" };
                }
            }
            else
                return new UniversityBookReferences { IsError = true, ErrorMessage = "This book does not exist in this university." };

        }

        public async Task<UniversityBookReferences> RemoveUniversityBookAsync(int universityId, string bookISBN)
        {
            if (_db.UniversityBooks.Any(p => p.UniversityId == universityId && p.BookISBN == bookISBN))
            {
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
                    var response = await client.GetAsync(requestURI);

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new UniversityBookReferences { IsError = true, ErrorMessage = "Error: Book is missing from book database." };
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return new UniversityBookReferences { IsError = false };

                    return new UniversityBookReferences { IsError = true, ErrorMessage = "Unexpected error" };
                }
            }
            else
                return new UniversityBookReferences { IsError = true, ErrorMessage = "This book does not exist in this university." };
        }
        /*
       public async Task<UniversityBookDTO> AddBookToUniversityAsync(int universityId, UniversityBookDTO universityBook)
       {
           using (var client = new HttpClient())
           {
               var requestURI = BaseBookServiceUri.Append("books/" + universityBook.ISBN).ToString();
               var response = await client.PostAsync(requestURI, new StringContent(JsonConvert.SerializeObject(universityBook)));
               var convertedContent = JsonConvert.DeserializeObject<RootBookObject>(await response.Content.ReadAsStringAsync());
               return convertedContent.Knyga;
           }
       }

       public async Task<UniversityBookDTO> EditUniversityBookAsync(int universityId, UniversityBookDTO universityBook)
       {
           using (var client = new HttpClient())
           {
               var requestURI = BaseBookServiceUri.Append("books/" + universityBook.ISBN).ToString();
               var response = await client.PutAsync(requestURI, new StringContent(JsonConvert.SerializeObject(universityBook)));
               var convertedContent = await response.Content.ReadAsStringAsync();
               return new UniversityBookDTO() { IsError = false };
           }
       }

       public async Task<int> RemoveUniversityBookAsync(int universityId, string bookISBN)
       {
           using (var client = new HttpClient())
           {
               var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
               var response = await client.DeleteAsync(requestURI);
               if (response.StatusCode == System.Net.HttpStatusCode.OK)
                   return 1;
               else
                   return -1;
           }
       }*/
    }
}
