using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearingClinicManagementSystem.Models
{
    public class AudiogramData
    {
        public int AudiogramDataID { get; set; }

        [Required]
        public int TestID { get; set; }
        public HearingTest HearingTest { get; set; }

        /// <summary>
        /// Which ear was tested
        /// Options: Left, Right
        /// </summary>
        [Required, StringLength(10)]
        public string Ear { get; set; }

        [Required]
        public int Frequency { get; set; }

        [Required]
        public int Threshold { get; set; }
    }
}
