﻿using System;

namespace BeyondCode.Models.API.Report
{
    public class ClientSummaryRequest
    {
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public string CreatedBy { get; set; }
    }

   
}