﻿using BeyondCode.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyondCode.Models.ViewModel
{
    public class EventPaymentListViewModel
    {
        public EventPaymentListViewModel()
        {
            EventPaymentStatusList = new List<MasterData>();
            EventPaymentList = new EventPaymentListDataViewModel();
        }

        public List<MasterData> EventPaymentStatusList { get; set; }

        public EventPaymentListDataViewModel EventPaymentList { get; set; }
    }
}