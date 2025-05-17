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
        #region Fields
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblWelcomeMessage;
        private Button btnLogout;
        private Panel loginPanel;
        private Panel welcomePanel;
        private bool isInitialized = false; // Track initialization state
        #endregion

        public event EventHandler LoginSuccess;

        public DashboardForm()
        {
            // Only initialize components once
            if (!isInitialized)
            {
                InitializeComponents();
                isInitialized = true;
            }

            // Set visibility based on login state
            if (AuthService.IsLoggedIn)
            {
                DisplayWelcomeMessage();
            }
            else
            {
                loginPanel.Visible = true;
                welcomePanel.Visible = false;
            }
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = AppStrings.Titles.Dashboard;

            // Form title
            var lblTitle = CreateTitleLabel(AppStrings.Titles.Dashboard);
            lblTitle.Dock = DockStyle.Top;

            // Main layout panel
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(10)
            };

            // Create and initialize panels
            InitializeLoginPanel(mainPanel);
            InitializeWelcomePanel(mainPanel);

            Controls.Add(mainPanel);
            Controls.Add(lblTitle);
        }

        private void InitializeLoginPanel(TableLayoutPanel parent)
        {
            // Login Panel Container
            loginPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Visible = !AuthService.IsLoggedIn
            };

            // Create a header for login section
            var lblLoginHeader = CreateLabel("User Login", 0, 0);
            lblLoginHeader.Dock = DockStyle.Top;
            lblLoginHeader.Font = new Font(lblLoginHeader.Font, FontStyle.Bold);
            lblLoginHeader.Height = 30;
            lblLoginHeader.TextAlign = ContentAlignment.MiddleLeft;

            // Create a table layout for the login form - reduce vertical spacing
            TableLayoutPanel loginForm = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(20, 20, 20, 20) // Reduced padding (was 20, 40, 20, 40)
            };

            // Set column styles - use fixed width for labels to ensure alignment
            loginForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Fixed width for labels
            loginForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // Remaining width for controls

            // More compact row heights
            loginForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Username row
            loginForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Password row
            loginForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Button row

            // Username row with better vertical alignment
            var lblUsername = CreateLabel("Username:", 0, 0);
            lblUsername.Dock = DockStyle.Fill;
            lblUsername.TextAlign = ContentAlignment.MiddleRight; // Right align for visual alignment with textbox
            lblUsername.Margin = new Padding(5, 10, 10, 0); // Add right padding

            txtUsername = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 5, 0), // Adjust top margin for vertical alignment
                Font = new Font(this.Font.FontFamily, 10)
            };

            loginForm.Controls.Add(lblUsername, 0, 0);
            loginForm.Controls.Add(txtUsername, 1, 0);

            // Password row with better vertical alignment
            var lblPassword = CreateLabel("Password:", 0, 0);
            lblPassword.Dock = DockStyle.Fill;
            lblPassword.TextAlign = ContentAlignment.MiddleRight; // Right align for visual alignment
            lblPassword.Margin = new Padding(5, 10, 10, 0); // Add right padding

            txtPassword = new TextBox
            {
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true,
                Margin = new Padding(0, 8, 5, 0), // Adjust top margin for vertical alignment
                Font = new Font(this.Font.FontFamily, 10)
            };

            loginForm.Controls.Add(lblPassword, 0, 1);
            loginForm.Controls.Add(txtPassword, 1, 1);

            // Button row with right alignment
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft, // Right alignment
                WrapContents = false,
                Margin = new Padding(0),
                Padding = new Padding(5, 5, 5, 0)
            };

            btnLogin = CreateButton("Login", 0, 0, BtnLogin_Click, 150, 35);
            ApplyButtonStyle(btnLogin);
            buttonPanel.Controls.Add(btnLogin);

            // Add button panel to second column only (not spanning)
            loginForm.Controls.Add(buttonPanel, 1, 2);

            // Add all to login panel
            loginPanel.Controls.Add(loginForm);
            loginPanel.Controls.Add(lblLoginHeader);

            parent.Controls.Add(loginPanel, 0, 0);
        }

        private void InitializeWelcomePanel(TableLayoutPanel parent)
        {
            // Welcome Panel Container
            welcomePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Visible = AuthService.IsLoggedIn
            };

            // Create a header for welcome section
            var lblWelcomeHeader = CreateLabel("Welcome to Hearing Clinic", 0, 0);
            lblWelcomeHeader.Dock = DockStyle.Top;
            lblWelcomeHeader.Font = new Font(lblWelcomeHeader.Font, FontStyle.Bold);
            lblWelcomeHeader.Height = 30;
            lblWelcomeHeader.TextAlign = ContentAlignment.MiddleLeft;

            // Create a table for the welcome content
            TableLayoutPanel welcomeContent = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20)
            };

            welcomeContent.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            welcomeContent.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            // Welcome message in a panel with styling
            Panel messagePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(240, 248, 255) // Light blue background
            };

            lblWelcomeMessage = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 150)
            };

            messagePanel.Controls.Add(lblWelcomeMessage);
            welcomeContent.Controls.Add(messagePanel, 0, 0);

            // Logout button in a flow layout panel for centering
            FlowLayoutPanel logoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 20, 0, 0)
            };

            btnLogout = CreateButton("Logout", 0, 0, BtnLogout_Click, 150, 35);
            ApplyButtonStyle(btnLogout);
            logoutPanel.Controls.Add(btnLogout);

            welcomeContent.Controls.Add(logoutPanel, 0, 1);

            // Add all to welcome panel
            welcomePanel.Controls.Add(welcomeContent);
            welcomePanel.Controls.Add(lblWelcomeHeader);

            parent.Controls.Add(welcomePanel, 0, 0);
        }
        #endregion

        #region Event Handlers
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
                UIService.RaiseUserLoggedIn(); // Add this line
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UIService.ShowError(AppStrings.Messages.LoginFailed);
                txtPassword.Text = "";
                txtPassword.Focus();
            }
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
            UIService.RaiseUserLoggedOut(); // Add this line
        }
        #endregion

        #region Helper Methods
        private void DisplayWelcomeMessage()
        {
            string name = string.Empty;
            string role = AuthService.CurrentUser?.Role ?? "User";

            if (AuthService.CurrentUser != null)
            {
                if (AuthService.CurrentPatient != null)
                {
                    name = $"{AuthService.CurrentPatient.User.FirstName} {AuthService.CurrentPatient.User.LastName}";
                }
                else if (AuthService.CurrentReceptionist != null)
                {
                    name = $"{AuthService.CurrentUser.FirstName} {AuthService.CurrentUser.LastName}";
                }
                // Add other roles as needed...
                else
                {
                    name = $"{AuthService.CurrentUser.FirstName} {AuthService.CurrentUser.LastName}";
                }
            }

            lblWelcomeMessage.Text = $"Welcome, {name}!\nYou are logged in as {role}.";

            loginPanel.Visible = false;
            welcomePanel.Visible = true;
        }

        private void ApplyButtonStyle(Button button)
        {
            // Use a more professional blue color
            button.BackColor = Color.FromArgb(51, 122, 183);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;

            // Add a subtle border
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(40, 96, 144);

            // Add white inner border by setting padding
            button.Padding = new Padding(2);
            button.FlatAppearance.BorderColor = Color.White;

            // Improve text appearance
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
            button.TextAlign = ContentAlignment.MiddleCenter;

            // Add visual feedback for interactions
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            // Add hover and press effects
            button.MouseEnter += (s, e) => {
                button.BackColor = Color.FromArgb(40, 96, 144); // Darker blue on hover
            };

            button.MouseLeave += (s, e) => {
                button.BackColor = Color.FromArgb(51, 122, 183); // Return to original blue
            };

            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 71, 109); // Even darker when pressed

            // Create double-border effect (outer blue border + inner white border)
            button.Paint += (s, e) => {
                var btn = (Button)s;
                var borderRect = new Rectangle(1, 1, btn.Width - 3, btn.Height - 3);
                ControlPaint.DrawBorder(e.Graphics, borderRect,
                    Color.White, 2, ButtonBorderStyle.Solid,
                    Color.White, 2, ButtonBorderStyle.Solid,
                    Color.White, 2, ButtonBorderStyle.Solid,
                    Color.White, 2, ButtonBorderStyle.Solid);
            };
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DashboardForm
            // 
            this.ClientSize = new System.Drawing.Size(1212, 785);
            this.Name = "DashboardForm";
            this.ResumeLayout(false);

        }
    }
}