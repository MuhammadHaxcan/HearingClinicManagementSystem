using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearingClinicManagementSystem.UI.Constants
{
    public static class AppStrings
    {
        public static class Titles
        {
            public const string AppTitle = "Hearing Clinic Management System";
            public const string Dashboard = "Dashboard";
            public const string ManageAppointment = "Manage Appointments";
            public const string PurchaseHearingAid = "Purchase Hearing Aids";
            public const string MedicalHistory = "Medical History";
            public const string UpdatePersonalInfo = "Update Personal Info";
        }

        public static class Messages
        {
            public const string LoginSuccess = "Login successful!";
            public const string LoginFailed = "Invalid username or password";
            public const string LogoutSuccess = "Logged out successfully";
        }

        public static class Labels
        {
            public const string Welcome = "Welcome, {0}!";
        }
    }
}
