using System;
using System.Collections.Generic;

namespace ServiceContracts.DTO
{
    public class WorkoutAddDurationRequest
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
