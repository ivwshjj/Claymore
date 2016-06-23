using System;
using System.Collections.Generic;
using System.Web;

namespace WebPSO
{
    public class XMode {
        public string FF;
    }
    public class AjaxTest
    {
        [Claymore.Action]
        public string Ajax(string name)
        {
            return name + DateTime.Now.ToString();
        }

        [Claymore.Action]
        public string Test(XMode m)
        {
            return m.FF;
        }
    }
}