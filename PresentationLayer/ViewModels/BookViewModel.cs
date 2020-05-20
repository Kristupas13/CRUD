using System;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class BookViewModel
    {
        public string ISBN { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
        public string Autorius { get; set; }
        public int Metai { get; set; }
        public string Pavadinimas { get; set; }
    }
}
