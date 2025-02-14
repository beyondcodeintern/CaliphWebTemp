﻿using BeyondCode.Models.API.Report;
using BeyondCode.Models.Data;
using BeyondCode.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyondCode.Models.ViewModel
{
    public class ClientListViewModel
    {
        public ClientListViewModel()
        {
            Status = new List<MasterData>();
            ClientData = new ClientData();
        }
        public List<MasterData> Status { get; set; }
        public ClientData ClientData { get; set; }
    }
    public class ClientData
    {
        public ClientData()
        {
            Clients = new List<Client>();
            Paging = new Paging { PageSize = 10, CurrentPage = 1 };
        }
        public List<Client> Clients { get; set; }
        public Paging Paging { get; set; }
    }
}