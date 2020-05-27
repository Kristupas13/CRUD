using CRUDWebService.PresentationLayer.ViewModels;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CRUDWebService.BusinessLayer.DTO.University
{
    [DataContract]
    public class EditUniversityDTO
    {
        [JsonIgnore]
        [DataMember]
        public int UniversityId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public IList<UniversityBookInformation> Books { get; set; }
    }
}
