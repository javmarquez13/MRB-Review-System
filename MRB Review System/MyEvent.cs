using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRB_Review_System
{
    class MyEvent : EventArgs
    {
        private string EventInfo;
        public MyEvent(string Text)
        {
            EventInfo = Text;
        }
    }
}
