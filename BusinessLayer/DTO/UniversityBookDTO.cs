using CRUDWebService.BusinessLayer.DTO.University;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CRUDWebService.BusinessLayer.DTO
{
    [DataContract]
    public class UniversityBookDTO : UniversityDTO
    {
        [DataMember]
        public IList<UniversityBookInformation> Books { get; set; }
    }

    [DataContract]
    public class UniversityBookInformation
    {
        [DataMember]
        public bool IsAvailable { get; set; }
        [DataMember]
        public DateTime AvailableFrom { get; set; }
        [DataMember]
        public string ISBN { get; set; }
        [DataMember]
        public string Pavadinimas { get; set; }
        [DataMember]
        public string Autorius { get; set; }
        [DataMember]
        public int Metai { get; set; }
        [DataMember]

        [JsonIgnore]
        public string ErrorMessage { get; set; }

    }
}
