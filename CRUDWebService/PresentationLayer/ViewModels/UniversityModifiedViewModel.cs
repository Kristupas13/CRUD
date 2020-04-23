using System;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class UniversityModifiedViewModel
    {
        public string BookISBN { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }
}
