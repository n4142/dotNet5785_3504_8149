﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public interface IDal
    {
        IConfig Config { get; }
        IAssignment Assignment { get; }
        ICall Call { get; }
        IVolunteer Volunteer { get; }
        void ResetDB();

    }
}
