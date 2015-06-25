﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace PaSignalR.Hubs
{
    public class PlanHub : Hub
    {
        private static int _plan = 0;
        private static int _result = 0;

        public void Update(int plan, int result)
        {
            _plan = plan;
            _result = result;
            Clients.All.Updated(plan, result);
        }

        public void GetPlan()
        {
            Clients.Caller.Updated(_plan, _result);
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}