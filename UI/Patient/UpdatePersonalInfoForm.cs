using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;

namespace HearingClinicManagementSystem.UI.Patient
{
    public class UpdatePersonalInfoForm : BaseForm
    {
        #region Fields
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private DateTimePicker dtpDateOfBirth;
        private TextBox txtAddress;
        private Button btnSave;
        private HearingClinicRepository repository;
        private User currentUser;
        private Models.Patient currentPatient;
        #endregion

        public UpdatePersonalInfoForm()
        {
            repository = HearingClinicRepository.Instance;
            InitializeComponents();
            LoadPatientData();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = AppStrings.Titles.UpdatePersonalInfo;

            // Form title
            var lblTitle = CreateTitleLabel(AppStrings.Titles.UpdatePersonalInfo);
            lblTitle.Dock = DockStyle.Top;

            // Main layout using TableLayoutPanel for vertical centering
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10),
                RowStyles = {
                    new RowStyle(SizeType.Percent, 70F), // Adjusted to take more space
                    new RowStyle(SizeType.Percent, 30F)  // Save button section
                }
            };

            // Upper section for form fields
            TableLayoutPanel fieldsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10)
            };

            fieldsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Label column
            fieldsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // Input field column

            // Initialize form fields
            InitializeFormFields(fieldsPanel);

            // Lower section for the save button
            TableLayoutPanel buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(10)
            };

            // Initialize Save button
            btnSave = CreateButton("Save Changes", 0, 0, BtnSave_Click, 150, 30);
            ApplyButtonStyle(btnSave);
            buttonPanel.Controls.Add(btnSave);

            // Add panels to the main panel
            mainPanel.Controls.Add(fieldsPanel, 0, 0);
            mainPanel.Controls.Add(buttonPanel, 0, 1);

            // Add controls to the form
            this.Controls.Add(lblTitle);
            this.Controls.Add(mainPanel);
        }

        private void InitializeFormFields(TableLayoutPanel parent)
        {
            // First Name
            var lblFirstName = CreateLabel("First Name:", 0, 0);
            txtFirstName = CreateTextBox(lblFirstName.Right + UIConstants.Padding.Small, lblFirstName.Top);
            parent.Controls.Add(lblFirstName, 0, 0);
            parent.Controls.Add(txtFirstName, 1, 0);

            // Last Name
            var lblLastName = CreateLabel("Last Name:", 0, lblFirstName.Bottom + UIConstants.Padding.Medium);
            txtLastName = CreateTextBox(lblLastName.Right + UIConstants.Padding.Small, lblLastName.Top);
            parent.Controls.Add(lblLastName, 0, 1);
            parent.Controls.Add(txtLastName, 1, 1);

            // Email
            var lblEmail = CreateLabel("Email:", 0, lblLastName.Bottom + UIConstants.Padding.Medium);
            txtEmail = CreateTextBox(lblEmail.Right + UIConstants.Padding.Small, lblEmail.Top);
            parent.Controls.Add(lblEmail, 0, 2);
            parent.Controls.Add(txtEmail, 1, 2);

            // Phone
            var lblPhone = CreateLabel("Phone:", 0, lblEmail.Bottom + UIConstants.Padding.Medium);
            txtPhone = CreateTextBox(lblPhone.Right + UIConstants.Padding.Small, lblPhone.Top);
            parent.Controls.Add(lblPhone, 0, 3);
            parent.Controls.Add(txtPhone, 1, 3);

            // Date of Birth
            var lblDob = CreateLabel("Date of Birth:", 0, lblPhone.Bottom + UIConstants.Padding.Medium);
            dtpDateOfBirth = new DateTimePicker
            {
                Location = new Point(lblDob.Right + UIConstants.Padding.Small, lblDob.Top),
                Width = UIConstants.Size.StandardTextBoxWidth
            };
            parent.Controls.Add(lblDob, 0, 4);
            parent.Controls.Add(dtpDateOfBirth, 1, 4);

            // Address
            var lblAddress = CreateLabel("Address:", 0, lblDob.Bottom + UIConstants.Padding.Medium);
            txtAddress = new TextBox
            {
                Location = new Point(lblAddress.Right + UIConstants.Padding.Small, lblAddress.Top),
                Width = UIConstants.Size.StandardTextBoxWidth,
                Multiline = true,
                Height = 60
            };
            parent.Controls.Add(lblAddress, 0, 5);
            parent.Controls.Add(txtAddress, 1, 5);
        }

        #endregion

        #region Event Handlers
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                UIService.ShowError("Please fill all fields");
                return;
            }

            // Update user info
            currentUser.FirstName = txtFirstName.Text;
            currentUser.LastName = txtLastName.Text;
            currentUser.Email = txtEmail.Text;
            currentUser.Phone = txtPhone.Text;

            // Update patient info
            currentPatient.DateOfBirth = dtpDateOfBirth.Value;
            currentPatient.Address = txtAddress.Text;

            try
            {
                // Save changes to database
                repository.UpdateUser(currentUser);
                repository.UpdatePatient(currentPatient);

                // Refresh AuthService data
                AuthService.RefreshCurrentUser();

                UIService.ShowSuccess("Personal information updated successfully!");
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error updating information: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods
        public void ApplyButtonStyle(Button button)
        {
            // Use a more professional blue color
            button.BackColor = Color.FromArgb(51, 122, 183);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;

            // Add a subtle border
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(40, 96, 144);

            // Improve text appearance
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Italic);
            button.TextAlign = ContentAlignment.MiddleCenter;

            // Add hover and press effects
            button.MouseEnter += (s, e) => {
                button.BackColor = Color.FromArgb(40, 96, 144); // Darker blue on hover
            };

            button.MouseLeave += (s, e) => {
                button.BackColor = Color.FromArgb(51, 122, 183); // Return to original blue
            };

            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 71, 109); // Even darker when pressed

            // Create slight shadow effect using border
            button.FlatAppearance.BorderColor = Color.FromArgb(40, 96, 144);
        }

        private void LoadPatientData()
        {
            try
            {
                int patientId = AuthService.CurrentPatient?.PatientID ?? 0;
                
                if (patientId == 0)
                {
                    UIService.ShowError("No patient is currently logged in");
                    return;
                }

                // Get patient data from repository
                currentPatient = repository.GetPatientById(patientId);
                
                if (currentPatient != null && currentPatient.UserID > 0)
                {
                    // Get associated user data from repository
                    currentUser = repository.GetUserById(currentPatient.UserID);
                    
                    if (currentUser != null)
                    {
                        // Populate form fields with patient and user data
                        txtFirstName.Text = currentUser.FirstName;
                        txtLastName.Text = currentUser.LastName;
                        txtEmail.Text = currentUser.Email;
                        txtPhone.Text = currentUser.Phone;
                        dtpDateOfBirth.Value = currentPatient.DateOfBirth;
                        txtAddress.Text = currentPatient.Address;
                    }
                    else
                    {
                        UIService.ShowError("Unable to load user information");
                    }
                }
                else
                {
                    UIService.ShowError("Unable to load patient information");
                }
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error loading patient data: {ex.Message}");
            }
        }
        #endregion
    }
}
