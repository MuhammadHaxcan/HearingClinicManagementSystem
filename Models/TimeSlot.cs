using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearingClinicManagementSystem.Models
{
    public class TimeSlot
    {
        public int TimeSlotID { get; set; }

        [Required]
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
