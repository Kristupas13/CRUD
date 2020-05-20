using System;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class UniversityBookInformationViewModel
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }

    public class UniversityBookViewModel
    {
        public int UniversityId { get; set; }
        public string Autorius { get; set; }
        public string Pavadinimas { get; set; }
        public int Metai { get; set; }
        public string BookISBN { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }
}
