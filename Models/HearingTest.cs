using System.Collections.Generic;
using System;

namespace HearingClinicManagementSystem.Models
{
    public class HearingTest
    {
        public int TestID { get; set; }
        public int RecordID { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public string TestType { get; set; } // "PureTone", "Speech", etc.
        public DateTime TestDate { get; set; }
        public string TestNotes { get; set; }

        // Navigation properties
        public ICollection<AudiogramData> AudiogramData { get; set; }
    }

    public class AudiogramData
    {
        public int AudiogramDataID { get; set; }
        public int TestID { get; set; }
        public HearingTest HearingTest { get; set; }
        public string Ear { get; set; } // "Left" or "Right"
        public int Frequency { get; set; } // 250, 500, 1000, etc.
        public int Threshold { get; set; } // dB HL
        public string TestConditions { get; set; }
    }
}