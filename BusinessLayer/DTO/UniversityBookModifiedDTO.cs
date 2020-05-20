﻿using CRUDWebService.BusinessLayer.Base.DTO;
using System;

namespace CRUDWebService.BusinessLayer.DTO
{
    public class UniversityBookModifiedDTO : ErrorDTO
    {
        public int UniversityId { get; set; }
        public string Pavadinimas { get; set; }
        public int Metai { get; set; }
        public string Autorius { get; set; }
        public string BookISBN { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }
}