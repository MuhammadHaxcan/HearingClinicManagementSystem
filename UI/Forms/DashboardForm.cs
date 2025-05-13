
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Constants;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HearingClinicManagementSystem.UI.Forms
{
    public class DashboardForm : BaseForm
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblWelcomeMessage;
        private Button btnLogout;
        private Panel loginPanel;
        private Panel welcomePanel;

        public event EventHandler LoginSuccess;

        public DashboardForm()
        {
            InitializeLayout();

            if (AuthService.IsLoggedIn)
            {
                DisplayWelcomeMessage();
            }
        }

        private void InitializeLayout()
        {
            this.Text = AppStrings.Titles.Dashboard;
            this.BackColor = Color.White;

            // Title Label
            var lblTitle = CreateTitleLabel(AppStrings.Titles.Dashboard);
            this.Controls.Add(lblTitle);

            // Create panels
            loginPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = !AuthService.IsLoggedIn,
                BackColor = Color.White
            };

            welcomePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = AuthService.IsLoggedIn,
                BackColor = Color.White
            };

            // Login Controls - Now properly aligned
            int controlStartY = 50; // Starting Y position below title

            // Username Label and Textbox
            var lblUsername = CreateLabel("Username:", UIConstants.Padding.Medium, controlStartY);
            txtUsername = CreateTextBox(
                lblUsername.Right + UIConstants.Padding.Small,
                lblUsername.Top - 3, // Slight vertical alignment adjustment
                width: 200);

            // Password Label and Textbox (positioned below username)
            var lblPassword = CreateLabel("Password:",
                UIConstants.Padding.Medium,
                lblUsername.Bottom + UIConstants.Padding.Medium * 2);
            txtPassword = CreateTextBox(
                lblPassword.Right + UIConstants.Padding.Small,
                lblPassword.Top - 3,
                width: 200,
                isPassword: true);

            // Login Button (centered below password)
            btnLogin = CreateButton("Login",
                (this.Width - UIConstants.Size.StandardButtonWidth) / 2,
                txtPassword.Bottom + UIConstants.Padding.Medium * 2,
                BtnLogin_Click);

            // Add controls to login panel
            loginPanel.Controls.Add(lblUsername);
            loginPanel.Controls.Add(txtUsername);
            loginPanel.Controls.Add(lblPassword);
            loginPanel.Controls.Add(txtPassword);
            loginPanel.Controls.Add(btnLogin);

            // Welcome Controls
            lblWelcomeMessage = CreateLabel("",
                UIConstants.Padding.Medium,
                UIConstants.Padding.Medium * 2,
                this.Width - (UIConstants.Padding.Medium * 2),
                ContentAlignment.MiddleCenter);
            lblWelcomeMessage.Font = new Font(this.Font.FontFamily, 14, FontStyle.Bold);

            btnLogout = CreateButton("Logout",
                (this.Width - UIConstants.Size.StandardButtonWidth) / 2,
                lblWelcomeMessage.Bottom + UIConstants.Padding.Medium * 3,
                BtnLogout_Click);

            welcomePanel.Controls.Add(lblWelcomeMessage);
            welcomePanel.Controls.Add(btnLogout);

            this.Controls.Add(loginPanel);
            this.Controls.Add(welcomePanel);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                UIService.ShowError("Please enter your username");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                UIService.ShowError("Please enter your password");
                txtPassword.Focus();
                return;
            }

            if (AuthService.AuthenticateUser(txtUsername.Text, txtPassword.Text))
            {
                DisplayWelcomeMessage();
                UIService.ShowSuccess(AppStrings.Messages.LoginSuccess);
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UIService.ShowError(AppStrings.Messages.LoginFailed);
                txtPassword.Text = "";
                txtPassword.Focus();
            }
        }

        private void DisplayWelcomeMessage()
        {
            lblWelcomeMessage.Text = string.Format(AppStrings.Labels.Welcome,
                AuthService.CurrentPatient?.User?.FirstName + " " +
                AuthService.CurrentPatient?.User?.LastName);

            loginPanel.Visible = false;
            welcomePanel.Visible = true;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            AuthService.Logout();
            txtUsername.Text = "";
            txtPassword.Text = "";
            loginPanel.Visible = true;
            welcomePanel.Visible = false;
            txtUsername.Focus();
            UIService.ShowSuccess(AppStrings.Messages.LogoutSuccess);
            LoginSuccess?.Invoke(this, EventArgs.Empty);
        }
    }
}