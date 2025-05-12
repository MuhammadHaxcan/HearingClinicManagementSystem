using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }

        [Required]
        public int AudiologistID { get; set; }
        public Audiologist Audiologist { get; set; }

        /// <summary>
        /// Day of week for this schedule
        /// Options: Monday-Sunday
        /// </summary>
        [Required, StringLength(10)]
        public string DayOfWeek { get; set; }

        public ICollection<TimeSlot> TimeSlots { get; set; }
    }
}