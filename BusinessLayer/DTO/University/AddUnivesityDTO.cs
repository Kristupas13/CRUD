using System.Collections.Generic;

namespace CRUDWebService.BusinessLayer.DTO.University
{
    public class AddUnivesityDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public IList<UniversityBookInformation> Books { get; set; }
    }
}
