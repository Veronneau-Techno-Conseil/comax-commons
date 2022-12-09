using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared
{
    public class HandlerToDelegateRelay
    {
        public Action<object, EventArgs> Delegate { get; set; } 

        public void OnEvent(object sender, EventArgs eventArgs)
        {
            if(this.Delegate != null)
            {
                Delegate(sender, eventArgs);
            }
        }
    }

    public class HandlerToDelegateRelay<TArg>
    {
        public Action<object, TArg> Delegate { get; set; }

        public void OnEvent(object sender, TArg eventArgs)
        {
            if (this.Delegate != null)
            {
                Delegate(sender, eventArgs);
            }
        }
    }
}
