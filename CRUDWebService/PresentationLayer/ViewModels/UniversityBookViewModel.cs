using System;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class UniversityBookViewModel
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
    }

    public class UniversityBookReferenceViewModel
    {
        public int UniversityId { get; set; }
        public string BookISBN { get; set; }
    }
}
