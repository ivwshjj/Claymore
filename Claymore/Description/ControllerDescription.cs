using System;
using System.Collections.Generic;
using System.Text;

namespace Claymore
{
    internal sealed class ControllerDescription : BaseDescription
    {
        public Type ControllerType { get; private set; }

        public ControllerDescription(Type t)
            : base(t)
        {
            this.ControllerType = t;
        }
    }
}
