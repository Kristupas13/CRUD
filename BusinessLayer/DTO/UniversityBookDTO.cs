using CRUDWebService.BusinessLayer.DTO.University;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CRUDWebService.BusinessLayer.DTO
{
    public class UniversityBookDTO : UniversityDTO
    {
        public IList<UniversityBookInformation> Books { get; set; }
    }

    public class UniversityBookInformation
    {
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
        public string ISBN { get; set; }
        public string Pavadinimas { get; set; }
        public string Autorius { get; set; }
        public int Metai { get; set; }

        [JsonIgnore]
        public string ErrorMessage { get; set; }

    }
}
