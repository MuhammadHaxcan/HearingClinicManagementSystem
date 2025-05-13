using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;

namespace HearingClinicManagementSystem.UI.Patient
{
    public class UpdatePersonalInfoForm : BaseForm
    {
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private DateTimePicker dtpDateOfBirth;
        private TextBox txtAddress;
        private Button btnSave;

        public UpdatePersonalInfoForm()
        {
            InitializeComponents();
            LoadPatientData();
        }

        private void InitializeComponents()
        {
            this.Text = AppStrings.Titles.UpdatePersonalInfo;

            var lblTitle = CreateTitleLabel(AppStrings.Titles.UpdatePersonalInfo);
            this.Controls.Add(lblTitle);

            // First Name
            var lblFirstName = CreateLabel("First Name:", UIConstants.Padding.Medium, lblTitle.Bottom + UIConstants.Padding.Medium);
            txtFirstName = CreateTextBox(lblFirstName.Right + UIConstants.Padding.Small, lblFirstName.Top);

            // Last Name
            var lblLastName = CreateLabel("Last Name:", UIConstants.Padding.Medium, lblFirstName.Bottom + UIConstants.Padding.Medium);
            txtLastName = CreateTextBox(lblLastName.Right + UIConstants.Padding.Small, lblLastName.Top);

            // Email
            var lblEmail = CreateLabel("Email:", UIConstants.Padding.Medium, lblLastName.Bottom + UIConstants.Padding.Medium);
            txtEmail = CreateTextBox(lblEmail.Right + UIConstants.Padding.Small, lblEmail.Top);

            // Phone
            var lblPhone = CreateLabel("Phone:", UIConstants.Padding.Medium, lblEmail.Bottom + UIConstants.Padding.Medium);
            txtPhone = CreateTextBox(lblPhone.Right + UIConstants.Padding.Small, lblPhone.Top);

            // Date of Birth
            var lblDob = CreateLabel("Date of Birth:", UIConstants.Padding.Medium, lblPhone.Bottom + UIConstants.Padding.Medium);
            dtpDateOfBirth = new DateTimePicker
            {
                Location = new Point(lblDob.Right + UIConstants.Padding.Small, lblDob.Top),
                Width = UIConstants.Size.StandardTextBoxWidth
            };

            // Address
            var lblAddress = CreateLabel("Address:", UIConstants.Padding.Medium, lblDob.Bottom + UIConstants.Padding.Medium);
            txtAddress = new TextBox
            {
                Location = new Point(lblAddress.Right + UIConstants.Padding.Small, lblAddress.Top),
                Width = UIConstants.Size.StandardTextBoxWidth,
                Multiline = true,
                Height = 60
            };

            // Save button
            btnSave = CreateButton("Save Changes", UIConstants.Padding.Medium, txtAddress.Bottom + UIConstants.Padding.Medium, BtnSave_Click);

            this.Controls.AddRange(new Control[] {
            lblFirstName, txtFirstName, lblLastName, txtLastName,
            lblEmail, txtEmail, lblPhone, txtPhone,
            lblDob, dtpDateOfBirth, lblAddress, txtAddress, btnSave
        });
        }

        private void LoadPatientData()
        {
            var patient = AuthService.CurrentPatient;
            var user = StaticDataProvider.Users.First(u => u.UserID == patient.UserID);

            txtFirstName.Text = user.FirstName;
            txtLastName.Text = user.LastName;
            txtEmail.Text = user.Email;
            txtPhone.Text = user.Phone;
            dtpDateOfBirth.Value = patient.DateOfBirth;
            txtAddress.Text = patient.Address;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                UIService.ShowError("Please fill all fields");
                return;
            }

            var patient = AuthService.CurrentPatient;
            var user = StaticDataProvider.Users.First(u => u.UserID == patient.UserID);

            // Update user info
            user.FirstName = txtFirstName.Text;
            user.LastName = txtLastName.Text;
            user.Email = txtEmail.Text;
            user.Phone = txtPhone.Text;

            // Update patient info
            patient.DateOfBirth = dtpDateOfBirth.Value;
            patient.Address = txtAddress.Text;

            UIService.ShowSuccess("Personal information updated successfully!");
        }
    }
}
