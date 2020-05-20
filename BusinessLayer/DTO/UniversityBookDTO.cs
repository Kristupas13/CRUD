using System;

namespace CRUDWebService.BusinessLayer.DTO
{

    public class UniversityBookInformation
    {
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
        public string ISBN { get; set; }
        public string Pavadinimas { get; set; }
        public string Autorius { get; set; }
        public int Metai { get; set; }
    }

    public class UniversityBookDTO : BookDTO
    {
        public int UniversityId { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }
}
