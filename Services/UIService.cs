using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HearingClinicManagementSystem.Services
{
    public static class UIService
    {
        // Add events for login and logout
        public static event EventHandler UserLoggedIn;
        public static event EventHandler UserLoggedOut;

        // Method to raise UserLoggedIn event
        public static void RaiseUserLoggedIn()
        {
            UserLoggedIn?.Invoke(null, EventArgs.Empty);
        }

        // Method to raise UserLoggedOut event
        public static void RaiseUserLoggedOut()
        {
            UserLoggedOut?.Invoke(null, EventArgs.Empty);
        }

        public static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static DialogResult ShowQuestion(string message)
        {
            return MessageBox.Show(
                message,
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
        }

        internal static void ShowWarning(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
