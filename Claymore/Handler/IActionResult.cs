using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Claymore
{
    public interface IActionResult
    {
        void Ouput(HttpContext context);
    }
}
