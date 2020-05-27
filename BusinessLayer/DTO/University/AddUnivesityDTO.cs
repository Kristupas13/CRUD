using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CRUDWebService.BusinessLayer.DTO.University
{
    [DataContract]
    public class AddUnivesityDTO
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public IList<UniversityBookInformation> Books { get; set; }
    }
}
