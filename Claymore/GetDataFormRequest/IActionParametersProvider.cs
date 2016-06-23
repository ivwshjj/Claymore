using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore
{
    interface IActionParametersProvider
    {
        object[] GetParameters(HttpRequest request, ActionDescription action);
    }
}
