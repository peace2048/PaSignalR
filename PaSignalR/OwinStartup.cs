using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace PaSignalR
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}