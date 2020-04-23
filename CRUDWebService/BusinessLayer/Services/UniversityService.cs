﻿using CRUDWebService.BusinessLayer.Base.DTO;
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
            var universityBooks = _db.UniversityBooks.Where(p => p.UniversityId == universityId).ToList();
            using (var client = new HttpClient())
            {
                var requestURI = BaseBookServiceUri.Append("books").ToString();
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
                    var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
                    var response = await GetResponseAsync(requestURI);

                    if (response == null)
                        return new UniversityBookDTO { IsError = true, ErrorMessage = "Unexpected error has occured. Library service could not be found or is not running." };
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new UniversityBookDTO { IsError = true, ErrorMessage = "Error: Book is missing from book database." };

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
                return new UniversityBookDTO { IsError = true, ErrorMessage = "Book does not exist in this university." };
        }

        public async Task<UniversityBookModifiedDTO> AddBookToUniversityAsync(int universityId, string bookISBN)
        {
            if (!_db.UniversityBooks.Any(p => p.UniversityId == universityId && p.BookISBN == bookISBN))
            {
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
                    var response = await GetResponseAsync(requestURI);

                    if (response == null)
                        return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Unexpected error has occured. Library service could not be found or is not running." };
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Error: Book is missing from book database." };
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _db.UniversityBooks.Add(new UniversityBook { UniversityId = universityId, BookISBN = bookISBN, AvailableFrom = DateTime.UtcNow, IsAvailable = true });
                        await _db.SaveChangesAsync();
                        return new UniversityBookModifiedDTO { IsError = false, UniversityId = universityId, BookISBN = bookISBN, AvailableFrom = DateTime.UtcNow, IsAvailable = true };
                    }

                    return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Unexpected error" };
                }
            }
            else
                return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Book already exists in this university." };

        }

        public async Task<UniversityBookModifiedDTO> EditUniversityBookAsync(UniversityBookModifiedDTO universityBookEdited)
        {
            if (_db.UniversityBooks.Any(p => p.UniversityId == universityBookEdited.UniversityId && p.BookISBN == universityBookEdited.BookISBN))
            {
                using (var client = new HttpClient())
                {
                    var requestURI = BaseBookServiceUri.Append("books/" + universityBookEdited.BookISBN).ToString();
                    var response = await GetResponseAsync(requestURI);

                    if (response == null)
                        return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Unexpected error has occured. Library service could not be found or is not running." };
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Error: Book is missing from book database." };
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _db.UniversityBooks.Update(new UniversityBook { UniversityId = universityBookEdited.UniversityId, BookISBN = universityBookEdited.BookISBN });
                        await _db.SaveChangesAsync();
                        return new UniversityBookModifiedDTO { IsError = false, UniversityId = universityBookEdited.UniversityId, BookISBN = universityBookEdited.BookISBN };
                    }
                    return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Unexpected error" };
                }
            }
            else
                return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "This book does not exist in this university." };

        }

        public async Task<UniversityBookModifiedDTO> RemoveUniversityBookAsync(int universityId, string bookISBN)
        {
            if (_db.UniversityBooks.Any(p => p.UniversityId == universityId && p.BookISBN == bookISBN))
            {
                var requestURI = BaseBookServiceUri.Append("books/" + bookISBN).ToString();
                var response = await GetResponseAsync(requestURI);
                if (response == null)
                    return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Unexpected error has occured. Library service could not be found or is not running." };
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Error: Book is missing from book database." };
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return new UniversityBookModifiedDTO { IsError = false };

                return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "Unexpected error" };
            }
            else
                return new UniversityBookModifiedDTO { IsError = true, ErrorMessage = "This book does not exist in this university." };
        }

        private async Task<HttpResponseMessage> GetResponseAsync(string requestURI)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetAsync(requestURI);
                }
                catch (Exception e)
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
            }
        }
    }
}
