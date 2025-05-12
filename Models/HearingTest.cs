using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class HearingTest
    {
        public int TestID { get; set; }

        [Required]
        public int RecordID { get; set; }
        public MedicalRecord MedicalRecord { get; set; }

        /// <summary>
        /// Type of hearing test performed
        /// Options: PureTone, Speech, Tympanometry
        /// </summary>
        [Required, StringLength(50)]
        public string TestType { get; set; }

        [Required]
        public DateTime TestDate { get; set; } = DateTime.Now;

        [StringLength(2000)]
        public string TestNotes { get; set; }

        public ICollection<AudiogramData> AudiogramData { get; set; }
    }
}