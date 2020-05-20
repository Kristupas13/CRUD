using CRUDWebService.BusinessLayer.DTO;
using System.Collections.Generic;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class UniversityViewModel
    {
        public int UniversityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public IList<UniversityBookInformation> Books { get; set; }
    }
}
