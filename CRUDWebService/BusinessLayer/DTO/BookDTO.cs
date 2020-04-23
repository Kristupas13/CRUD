using CRUDWebService.BusinessLayer.Base.DTO;
using System;
using System.Collections.Generic;

namespace CRUDWebService.BusinessLayer.DTO
{
    public class RootBookList
    {
        public List<BookDTO> Knygos { get; set; }
    }

    public class RootBookObject
    {
        public BookDTO Knyga { get; set; }
    }

    public class BookDTO : ErrorDTO
    {
        public string ISBN { get; set; }
        public string Pavadinimas { get; set; }
        public string Autorius { get; set; }
        public int Metai { get; set; }
    }
}
