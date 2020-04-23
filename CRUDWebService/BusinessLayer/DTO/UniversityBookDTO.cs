﻿using System;

namespace CRUDWebService.BusinessLayer.DTO
{
    public class UniversityBookDTO : BookDTO
    {
        public int UniversityId { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }
}