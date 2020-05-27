using System.Runtime.Serialization;

namespace CRUDWebService.BusinessLayer.DTO.University
{
    [DataContract]
    public class UniversityDTO
    {
        [DataMember]
        public int UniversityId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Address { get; set; }
    }
}
