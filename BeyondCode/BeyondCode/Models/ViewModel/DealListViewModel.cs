﻿using BeyondCode.Models.API.Agent;
using BeyondCode.Models.API.Deal;
using BeyondCode.Models.Data;
using BeyondCode.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyondCode.Models.ViewModel
{
    public class DealListViewModel
    {
        public DealListViewModel()
        {
            DealData = new DealData();
            Status = new List<MasterData>();
            Title = new List<MasterData>();
            Users = new List<AgentUser>();
        }
        public List<MasterData> Status { get; set; }

        public List<MasterData> Title { get; set; }

        public List<AgentUser> Users { get; set; }
        public List<Client> Clients { get; set; }

        public DealData  DealData { get; set; }
        public string ClientName { get; set; }
        public AddActivityViewModel  AddActivityViewModel { get; set; }
    }

    public class DealData
    {
        public DealData()
        {
            Deals = new List<Deal>();
            Paging = new Paging();
        }
        public List<Deal> Deals { get; set; }
        public Paging Paging { get; set; }
    }
}