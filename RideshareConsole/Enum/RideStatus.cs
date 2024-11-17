﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideshareConsole.Enum
{
    public enum RideStatus
    {
        Requested,
        Assigned,
        InProgress,
        Completed,
        Cancelled,
        Rejected,
        SafetyAlert
    }
}