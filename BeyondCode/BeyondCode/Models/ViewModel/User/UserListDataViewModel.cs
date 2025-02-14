﻿using BeyondCode.Models.API;
using BeyondCode.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyondCode.Models.ViewModel
{
    public class UserListDataViewModel
    {
        public UserListDataViewModel()
        {
            UserList = new List<UserListResponse>();
            Paging = new Paging { PageSize = 10, CurrentPage = 1 };
        }
        public List<UserListResponse> UserList { get; set; }
        public Paging Paging { get; set; }
    }
}