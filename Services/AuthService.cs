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

        public static bool AuthenticateUser(string username, string password)
        {
            // In a real app, this would validate against a database
            var patient = StaticDataProvider.Patients.FirstOrDefault(p =>
                p.User.Username == username && p.User.PasswordHash == password);

            if (patient != null)
            {
                IsLoggedIn = true;
                CurrentPatient = patient;
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            IsLoggedIn = false;
            CurrentPatient = null;
        }
    }
}
