using System.Collections.Generic;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class CreateUniversityViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public IList<BookViewModel> Books { get; set; }
    }
}
