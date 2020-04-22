using CRUDWebService.BusinessLayer.Base.DTO;
using System;
using System.Collections.Generic;

namespace CRUDWebService.BusinessLayer.DTO
{
    public class RootBookList
    {
        public List<UniversityBookDTO> Knygos { get; set; }
    }

    public class RootBookObject
    {
        public UniversityBookDTO Knyga { get; set; }
    }

    public class UniversityBookDTO : ErrorDTO
    {
        public string ISBN { get; set; }
        public string Pavadinimas { get; set; }
        public string Autorius { get; set; }
        public int Metai { get; set; }
    }

    public class UniversityBookReferences : ErrorDTO
    {
        public int UniversityId { get; set; }
        public string BookISBN { get; set; }
    }
}
