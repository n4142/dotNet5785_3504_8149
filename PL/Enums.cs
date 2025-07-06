using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace PL
{
    public class CallTypeCollection : IEnumerable
    {
        private static readonly IEnumerable<CallType> s_enums =
            System.Enum.GetValues(typeof(CallType)).Cast<CallType>();

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}

