//using System.Collections;
//using System.Collections.Generic;
//using BO;

//namespace PL
//{
//    internal class CallStatusCollection : IEnumerable
//    {
//        static readonly IEnumerable<CallStatus> s_enums =
//            (CallStatus[])Enum.GetValues(typeof(CallStatus));

//        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
//    }
//}
// Enums.cs
//using System.Collections;
//using System.Collections.Generic;

//namespace PL
//{
//    internal class CallTypeCollection : IEnumerable
//    {
//        static readonly IEnumerable<BO.CallType> s_enums =
//            (System.Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

//        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
//    }
//}
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

