using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;

namespace HearingClinicManagementSystem.Services
{
    public static class AuthService
    {
        public static bool IsLoggedIn { get; private set; }
        public static Patient CurrentPatient { get; private set; }
        public static User CurrentUser { get; private set; }
        public static Audiologist CurrentAudiologist { get; private set; }
        public static Receptionist CurrentReceptionist { get; private set; }

        public static bool AuthenticateUser(string username, string password)
        {
            // Find the user by username
            User user = StaticDataProvider.Users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user != null && user.PasswordHash == password && user.IsActive)
            {
                CurrentUser = user;

                // Set the appropriate user based on role
                if (user.Role == "Patient")
                {
                    CurrentPatient = StaticDataProvider.Patients.FirstOrDefault(p => p.UserID == user.UserID);
                }
                else if (user.Role == "Audiologist")
                {
                    CurrentAudiologist = StaticDataProvider.Audiologists.FirstOrDefault(a => a.UserID == user.UserID);
                }
                else if (user.Role == "Receptionist")
                {
                    CurrentReceptionist = StaticDataProvider.Receptionists.FirstOrDefault(r => r.UserID == user.UserID);
                }
                // Add other roles as needed...

                IsLoggedIn = true;
                return true;
            }

            return false;
        }

        public static void Logout()
        {
            IsLoggedIn = false;
            CurrentUser = null;
            CurrentPatient = null;
            CurrentAudiologist = null;
            CurrentReceptionist = null;
            // Reset other role-specific users...
        }
    }
}
