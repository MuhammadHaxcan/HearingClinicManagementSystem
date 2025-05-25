using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Constants;
using System;
using System.Drawing;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;

namespace HearingClinicManagementSystem.UI.Forms
{
    public class DashboardForm : BaseForm
    {
        #region Fields
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private Button btnLogin;
        private LinkLabel lnkRegister;
        private Label lblWelcomeMessage;
        private Button btnLogout;
        private Panel loginPanel;
        private Panel welcomePanel;
        private Panel registrationPanel;
        private bool isInitialized = false; // Track initialization state

        // Registration fields
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private DateTimePicker dtpDateOfBirth;
        private TextBox txtRegUsername;
        private TextBox txtRegPassword;
        private CheckBox chkRegShowPassword;
        private Button btnRegister;
        private LinkLabel lnkBackToLogin;
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
                ShowLoginPanel();
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
            InitializeRegistrationPanel(mainPanel);
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
                RowCount = 5, // Increased for show password checkbox and registration link
                Padding = new Padding(20, 20, 20, 20)
            };

            // Set column styles - use fixed width for labels to ensure alignment
            loginForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Fixed width for labels
            loginForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // Remaining width for controls

            // More compact row heights
            loginForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Username row
            loginForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Password row
            loginForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Show password row
            loginForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Register link row
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

            // Show password checkbox
            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                AutoSize = true,
                Dock = DockStyle.Left,
                Margin = new Padding(0, 5, 0, 0)
            };
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            loginForm.Controls.Add(new Label(), 0, 2); // Empty cell
            loginForm.Controls.Add(chkShowPassword, 1, 2);

            // Registration link
            lnkRegister = new LinkLabel
            {
                Text = "New user? Register here",
                AutoSize = true,
                Dock = DockStyle.Left,
                Margin = new Padding(0, 5, 0, 0)
            };
            lnkRegister.LinkClicked += LnkRegister_LinkClicked;

            loginForm.Controls.Add(new Label(), 0, 3); // Empty cell
            loginForm.Controls.Add(lnkRegister, 1, 3);

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
            loginForm.Controls.Add(buttonPanel, 1, 4);

            // Add all to login panel
            loginPanel.Controls.Add(loginForm);
            loginPanel.Controls.Add(lblLoginHeader);

            parent.Controls.Add(loginPanel, 0, 0);
        }

        private void InitializeRegistrationPanel(TableLayoutPanel parent)
        {
            // Registration Panel Container
            registrationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Visible = false
            };

            // Create a header for registration section
            var lblRegistrationHeader = CreateLabel("User Registration", 0, 0);
            lblRegistrationHeader.Dock = DockStyle.Top;
            lblRegistrationHeader.Font = new Font(lblRegistrationHeader.Font, FontStyle.Bold);
            lblRegistrationHeader.Height = 30;
            lblRegistrationHeader.TextAlign = ContentAlignment.MiddleLeft;

            // Create a scrollable panel for registration form
            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Create a table layout for the registration form
            TableLayoutPanel registrationForm = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 10, // All fields plus back to login link and register button
                Padding = new Padding(20),
                AutoSize = true
            };

            // Set column styles - use fixed width for labels to ensure alignment
            registrationForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Fixed width for labels
            registrationForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F)); // Fixed width for inputs

            // Row heights
            for (int i = 0; i < 9; i++)
            {
                registrationForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            }
            registrationForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Last row for button

            // First Name
            var lblFirstName = CreateLabel("First Name:", 0, 0);
            lblFirstName.Dock = DockStyle.Fill;
            lblFirstName.TextAlign = ContentAlignment.MiddleRight;

            txtFirstName = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 5, 0)
            };

            registrationForm.Controls.Add(lblFirstName, 0, 0);
            registrationForm.Controls.Add(txtFirstName, 1, 0);

            // Last Name
            var lblLastName = CreateLabel("Last Name:", 0, 0);
            lblLastName.Dock = DockStyle.Fill;
            lblLastName.TextAlign = ContentAlignment.MiddleRight;

            txtLastName = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 5, 0)
            };

            registrationForm.Controls.Add(lblLastName, 0, 1);
            registrationForm.Controls.Add(txtLastName, 1, 1);

            // Email
            var lblEmail = CreateLabel("Email:", 0, 0);
            lblEmail.Dock = DockStyle.Fill;
            lblEmail.TextAlign = ContentAlignment.MiddleRight;

            txtEmail = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 5, 0)
            };

            registrationForm.Controls.Add(lblEmail, 0, 2);
            registrationForm.Controls.Add(txtEmail, 1, 2);

            // Phone
            var lblPhone = CreateLabel("Phone:", 0, 0);
            lblPhone.Dock = DockStyle.Fill;
            lblPhone.TextAlign = ContentAlignment.MiddleRight;

            txtPhone = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 5, 0)
            };

            registrationForm.Controls.Add(lblPhone, 0, 3);
            registrationForm.Controls.Add(txtPhone, 1, 3);

            // Date of Birth
            var lblDob = CreateLabel("Date of Birth:", 0, 0);
            lblDob.Dock = DockStyle.Fill;
            lblDob.TextAlign = ContentAlignment.MiddleRight;

            dtpDateOfBirth = CreateDatePicker(0, 0);
            dtpDateOfBirth.Dock = DockStyle.Fill;
            dtpDateOfBirth.Margin = new Padding(0, 8, 5, 0);
            dtpDateOfBirth.Value = DateTime.Now.AddYears(-30); // Default age

            registrationForm.Controls.Add(lblDob, 0, 4);
            registrationForm.Controls.Add(dtpDateOfBirth, 1, 4);

            // Address
            var lblAddress = CreateLabel("Address:", 0, 0);
            lblAddress.Dock = DockStyle.Fill;
            lblAddress.TextAlign = ContentAlignment.MiddleRight;

            txtAddress = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 5, 0),
                Multiline = true,
                Height = 60
            };

            registrationForm.Controls.Add(lblAddress, 0, 5);
            registrationForm.Controls.Add(txtAddress, 1, 5);

            // Username
            var lblRegUsername = CreateLabel("Username:", 0, 0);
            lblRegUsername.Dock = DockStyle.Fill;
            lblRegUsername.TextAlign = ContentAlignment.MiddleRight;

            txtRegUsername = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 5, 0)
            };

            registrationForm.Controls.Add(lblRegUsername, 0, 6);
            registrationForm.Controls.Add(txtRegUsername, 1, 6);

            // Password
            var lblRegPassword = CreateLabel("Password:", 0, 0);
            lblRegPassword.Dock = DockStyle.Fill;
            lblRegPassword.TextAlign = ContentAlignment.MiddleRight;

            txtRegPassword = new TextBox
            {
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true,
                Margin = new Padding(0, 8, 5, 0)
            };

            registrationForm.Controls.Add(lblRegPassword, 0, 7);
            registrationForm.Controls.Add(txtRegPassword, 1, 7);

            // Show password checkbox
            chkRegShowPassword = new CheckBox
            {
                Text = "Show Password",
                AutoSize = true,
                Dock = DockStyle.Left,
                Margin = new Padding(0, 5, 0, 0)
            };
            chkRegShowPassword.CheckedChanged += ChkRegShowPassword_CheckedChanged;

            FlowLayoutPanel pnlRegLinks = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            lnkBackToLogin = new LinkLabel
            {
                Text = "Back to Login",
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 0)
            };
            lnkBackToLogin.LinkClicked += LnkBackToLogin_LinkClicked;

            pnlRegLinks.Controls.Add(chkRegShowPassword);
            pnlRegLinks.Controls.Add(new Label { Text = "   " }); // Spacer
            pnlRegLinks.Controls.Add(lnkBackToLogin);

            registrationForm.Controls.Add(new Label(), 0, 8); // Empty cell
            registrationForm.Controls.Add(pnlRegLinks, 1, 8);

            // Register Button
            FlowLayoutPanel registerButtonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };

            btnRegister = CreateButton("Register", 0, 0, BtnRegister_Click, 150, 35);
            ApplyButtonStyle(btnRegister);
            registerButtonPanel.Controls.Add(btnRegister);

            registrationForm.Controls.Add(new Label(), 0, 9); // Empty cell
            registrationForm.Controls.Add(registerButtonPanel, 1, 9);

            // Add the form to the scrollable panel
            scrollPanel.Controls.Add(registrationForm);

            // Add all to registration panel
            registrationPanel.Controls.Add(scrollPanel);
            registrationPanel.Controls.Add(lblRegistrationHeader);

            parent.Controls.Add(registrationPanel, 0, 0);
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
                UIService.RaiseUserLoggedIn();
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
            ShowLoginPanel();
            UIService.ShowSuccess(AppStrings.Messages.LogoutSuccess);
            UIService.RaiseUserLoggedOut();
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void ChkRegShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtRegPassword.UseSystemPasswordChar = !chkRegShowPassword.Checked;
        }

        private void LnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowRegistrationPanel();
        }

        private void LnkBackToLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowLoginPanel();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                UIService.ShowError("Please enter your first name");
                txtFirstName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                UIService.ShowError("Please enter your last name");
                txtLastName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                UIService.ShowError("Please enter your email");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                UIService.ShowError("Please enter your phone number");
                txtPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                UIService.ShowError("Please enter your address");
                txtAddress.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRegUsername.Text))
            {
                UIService.ShowError("Please enter a username");
                txtRegUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRegPassword.Text))
            {
                UIService.ShowError("Please enter a password");
                txtRegPassword.Focus();
                return;
            }

            // Validate username format
            string username = txtRegUsername.Text.Trim();
            if (username.Length < 4 || username.Length > 50)
            {
                UIService.ShowError("Username must be between 4 and 50 characters");
                txtRegUsername.Focus();
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9._-]+$"))
            {
                UIService.ShowError("Username can only contain letters, numbers, dots, underscores, and hyphens");
                txtRegUsername.Focus();
                return;
            }

            // Check if username already exists
            var repository = HearingClinicRepository.Instance;
            if (repository.GetUserByUsername(username) != null)
            {
                UIService.ShowError("Username already exists. Please choose another one.");
                txtRegUsername.Focus();
                return;
            }

            // Validate age
            DateTime dateOfBirth = dtpDateOfBirth.Value;
            int age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
            {
                UIService.ShowError("You must be at least 18 years old to register");
                dtpDateOfBirth.Focus();
                return;
            }

            // Register the user
            try
            {
                // Create user object
                var user = new User
                {
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Role = "Patient", // Default role
                    Username = username,
                    PasswordHash = txtRegPassword.Text, // In production, this would be hashed
                    IsActive = true
                };

                // Add user to database
                int userId = repository.AddUser(user);

                // Create patient record
                var patient = new HearingClinicManagementSystem.Models.Patient
                {
                    UserID = userId,
                    DateOfBirth = dateOfBirth,
                    Address = txtAddress.Text.Trim()
                };

                // Add patient to database
                repository.AddPatient(patient);

                UIService.ShowSuccess("Registration successful! You can now log in with your new account.");

                // Clear form and show login panel
                ClearRegistrationForm();
                ShowLoginPanel();

                // Pre-fill username for convenience
                txtUsername.Text = username;
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Registration failed: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods
        private void ShowLoginPanel()
        {
            loginPanel.Visible = true;
            registrationPanel.Visible = false;
            welcomePanel.Visible = false;
            txtUsername.Focus();
        }

        private void ShowRegistrationPanel()
        {
            loginPanel.Visible = false;
            registrationPanel.Visible = true;
            welcomePanel.Visible = false;
            txtFirstName.Focus();
        }

        private void ClearRegistrationForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            dtpDateOfBirth.Value = DateTime.Now.AddYears(-30);
            txtRegUsername.Text = "";
            txtRegPassword.Text = "";
            chkRegShowPassword.Checked = false;
        }

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
            registrationPanel.Visible = false;
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
            this.ClientSize = new System.Drawing.Size(1344, 785);
            this.Name = "DashboardForm";
            this.ResumeLayout(false);
        }
    }
}