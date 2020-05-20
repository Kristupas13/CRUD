using CRUDWebService.BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class EditUniversityViewModel
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public IList<UniversityBookInformation> Books { get; set; }
    }
}
