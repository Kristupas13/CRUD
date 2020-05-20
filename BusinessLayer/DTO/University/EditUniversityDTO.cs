using CRUDWebService.PresentationLayer.ViewModels;
using System.Collections.Generic;

namespace CRUDWebService.BusinessLayer.DTO.University
{
    public class EditUniversityDTO
    {
        public int UniversityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public IList<UniversityBookInformation> Books { get; set; }
    }
}
