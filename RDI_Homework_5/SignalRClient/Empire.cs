using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Empire
    {
        public int Empno
        { get; set; }
        public string EGov
        { get; set; }
        public string EName
        { get; set; }

        public override string ToString()
        {
            return string.Format("The {0} are {1}", EName, EGov);
        }
    }
}
