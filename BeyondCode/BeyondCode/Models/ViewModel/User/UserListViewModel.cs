﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyondCode.Models.ViewModel
{
    public class UserListViewModel
    {
        public UserListViewModel()
        {
            UserList = new UserListDataViewModel();
        }

        public UserListDataViewModel UserList { get; set; }
    }
}