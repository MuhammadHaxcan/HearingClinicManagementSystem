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
        private static User _currentUser;
        private static Patient _currentPatient;
        private static Audiologist _currentAudiologist;
        private static Receptionist _currentReceptionist;
        private static InventoryManager _currentInventoryManager;
        private static ClinicManager _currentClinicManager;

        public static event EventHandler UserLoggedIn;
        public static event EventHandler UserLoggedOut;

        public static bool IsLoggedIn => _currentUser != null;

        public static User CurrentUser => _currentUser;
        public static Patient CurrentPatient => _currentPatient;
        public static Audiologist CurrentAudiologist => _currentAudiologist;
        public static Receptionist CurrentReceptionist => _currentReceptionist;
        public static InventoryManager CurrentInventoryManager => _currentInventoryManager;
        public static ClinicManager CurrentClinicManager => _currentClinicManager;

        public static bool Login(string username, string password)
        {
            var repository = HearingClinicRepository.Instance;
            var user = repository.GetUserByUsername(username);

            if (user != null && user.PasswordHash == password) // In production, use proper password hashing
            {
                _currentUser = user;

                switch (user.Role)
                {
                    case "Patient":
                        _currentPatient = repository.GetPatientByUserId(user.UserID);
                        break;
                    case "Audiologist":
                        _currentAudiologist = repository.GetAudiologistByUserId(user.UserID);
                        break;
                    case "Receptionist":
                        _currentReceptionist = repository.GetReceptionistByUserId(user.UserID);
                        break;
                    case "InventoryManager":
                        _currentInventoryManager = repository.GetInventoryManagerByUserId(user.UserID);
                        break;
                    case "ClinicManager":
                        _currentClinicManager = repository.GetClinicManagerByUserId(user.UserID);
                        break;
                }

                UserLoggedIn?.Invoke(null, EventArgs.Empty);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Authenticates a user by username and password
        /// </summary>
        /// <param name="username">The username to authenticate</param>
        /// <param name="password">The password to authenticate</param>
        /// <returns>True if authentication is successful, false otherwise</returns>
        public static bool AuthenticateUser(string username, string password)
        {
            // This method is essentially the same as Login but allows for separation
            // of authentication from the actual login process if needed in the future
            return Login(username, password);
        }

        public static void Logout()
        {
            _currentUser = null;
            _currentPatient = null;
            _currentAudiologist = null;
            _currentReceptionist = null;
            _currentInventoryManager = null;
            _currentClinicManager = null;

            UserLoggedOut?.Invoke(null, EventArgs.Empty);
        }

        public static void RefreshCurrentUser()
        {
            if (_currentUser == null || _currentPatient == null) return;

            var repository = HearingClinicRepository.Instance;
            
            // Refresh patient data
            _currentPatient = repository.GetPatientById(_currentPatient.PatientID);
            
            // Refresh user data
            if (_currentPatient != null && _currentPatient.UserID > 0)
            {
                _currentUser = repository.GetUserById(_currentPatient.UserID);
            }
        }
    }
}
