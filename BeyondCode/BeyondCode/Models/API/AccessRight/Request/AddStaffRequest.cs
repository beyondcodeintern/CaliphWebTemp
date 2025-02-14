﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyondCode.ViewModel.Data
{
    public class AddStaffRequest
    {
            public string Username { get; set; }
            public string DisplayName { get; set; }
            public string Fullname { get; set; }
            public int StatusId { get; set; }
            public string IcNo { get; set; }
            public int ContactNo { get; set; }
            public string Email { get; set; }
            public DateTime JoinDate { get; set; }
            public string PW { get; set; }
            public int RoleId { get; set; }
            public string UplineUsername { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }

    }

}