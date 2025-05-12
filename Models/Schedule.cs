using System.Collections.Generic;
using System;

namespace HearingClinicManagementSystem.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int AudiologistID { get; set; }
        public Audiologist Audiologist { get; set; }
        public string DayOfWeek { get; set; } // "Monday", "Tuesday", etc.

        // Navigation property
        public ICollection<TimeSlot> TimeSlots { get; set; }
    }

    public class TimeSlot
    {
        public int TimeSlotID { get; set; }
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}