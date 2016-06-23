using System;
using System.Collections.Generic;
using System.Web;

namespace WebPSO
{
    public class PageController
    {
        [Claymore.Action]
        [Claymore.PageUrl(Url="/d.aspx")]
        public object Page()
        {
            return new Claymore.UcResult("/Controls/login.ascx", null);
        }
    }
}