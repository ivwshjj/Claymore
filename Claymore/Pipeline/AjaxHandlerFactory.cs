using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Claymore
{
    public sealed class AjaxHandlerFactory : BaseActionHandlerFactory
    {
        private static readonly Regex s_urlRegex
            = new Regex(@"/(?<name>(\w[\./\w]*)?(?=Ajax)\w+)[/\.](?<method>\w+)\.[a-zA-Z]+", RegexOptions.Compiled);

        public override ControllerActionPair ParseUrl(System.Web.HttpContext context, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Match match = s_urlRegex.Match(path);
            if (match.Success == false)
                return null;

            return new ControllerActionPair
            {
                Controller = match.Groups["name"].Value.Replace("/", "."),
                Action = match.Groups["method"].Value
            };
        }

        public override bool TypeIsService(Type type)
        {
            return type.Name.StartsWith("Ajax");
        }
    }
}
