using System;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class UniversityModifiedViewModel
    {
        public string BookISBN { get; set; }
        public string Pavadinimas { get; set; }
        public string Autorius { get; set; }
        public int Metai { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }
}
