using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;

namespace HearingClinicManagementSystem.UI.Receptionist
{
    public class CreateAppointmentForm : BaseForm
    {
        #region Fields
        private ComboBox cmbPatients;
        private ComboBox cmbAudiologists;
        private DateTimePicker dtpDate;
        private ComboBox cmbTimeSlots;
        private TextBox txtPurpose;
        private NumericUpDown nudFee;
        private Button btnBook;
        private Button btnCancel;
        private Label lblAvailableSlots;
        private HearingClinicRepository repository;
        #endregion

        public CreateAppointmentForm()
        {
            repository = HearingClinicRepository.Instance;
            InitializeComponents();
            LoadPatients();
            LoadAudiologists();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Create New Appointment";
            var lblTitle = CreateTitleLabel("Create New Appointment");
            lblTitle.Dock = DockStyle.Top;

            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(10)
            };

            InitializeBookingPanel(mainPanel);

            Controls.Add(mainPanel);
            Controls.Add(lblTitle);
        }

        private void InitializeBookingPanel(TableLayoutPanel parent)
        {
            Panel pnlBooking = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            var lblBooking = CreateLabel("Book New Walk-in Appointment", 0, 0);
            lblBooking.Dock = DockStyle.Top;
            lblBooking.Font = new Font(lblBooking.Font, FontStyle.Bold);
            lblBooking.Height = 30;
            lblBooking.TextAlign = ContentAlignment.MiddleLeft;

            // Create booking form layout with increased height for controls
            TableLayoutPanel bookingForm = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10)
            };

            bookingForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            bookingForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));

            // Set larger row heights for better visibility and touch
            for (int i = 0; i < 6; i++)
            {
                bookingForm.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6F));
            }

            // Patient Selection
            var lblPatient = CreateLabel("Patient:", 0, 0);
            lblPatient.Dock = DockStyle.Fill;
            lblPatient.TextAlign = ContentAlignment.MiddleLeft;
            lblPatient.Margin = new Padding(3);

            cmbPatients = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(3),
                Font = new Font(this.Font.FontFamily, 10),
                DisplayMember = "DisplayName",
                ValueMember = "PatientID"
            };

            bookingForm.Controls.Add(lblPatient, 0, 0);
            bookingForm.Controls.Add(cmbPatients, 1, 0);

            // Audiologist Selection
            var lblAudiologist = CreateLabel("Audiologist:", 0, 0);
            lblAudiologist.Dock = DockStyle.Fill;
            lblAudiologist.TextAlign = ContentAlignment.MiddleLeft;
            lblAudiologist.Margin = new Padding(3);

            cmbAudiologists = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(3),
                Font = new Font(this.Font.FontFamily, 10),
                DisplayMember = "DisplayName",
                ValueMember = "AudiologistID"
            };
            cmbAudiologists.SelectedIndexChanged += CmbAudiologists_SelectedIndexChanged;

            bookingForm.Controls.Add(lblAudiologist, 0, 1);
            bookingForm.Controls.Add(cmbAudiologists, 1, 1);

            // Date Selection
            var lblDate = CreateLabel("Date:", 0, 0);
            lblDate.Dock = DockStyle.Fill;
            lblDate.TextAlign = ContentAlignment.MiddleLeft;
            lblDate.Margin = new Padding(3);

            dtpDate = CreateDatePicker(0, 0);
            dtpDate.Dock = DockStyle.Fill;
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.MinDate = DateTime.Today; // Allow same-day appointments for walk-ins
            dtpDate.Value = DateTime.Today;
            dtpDate.Margin = new Padding(3);
            dtpDate.Font = new Font(this.Font.FontFamily, 10);
            dtpDate.ValueChanged += DtpDate_ValueChanged;

            bookingForm.Controls.Add(lblDate, 0, 2);
            bookingForm.Controls.Add(dtpDate, 1, 2);

            // Time Selection
            var lblTime = CreateLabel("Time:", 0, 0);
            lblTime.Dock = DockStyle.Fill;
            lblTime.TextAlign = ContentAlignment.MiddleLeft;
            lblTime.Margin = new Padding(3);

            // Time slot panel with label showing available slots
            Panel timeSlotPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };

            cmbTimeSlots = new ComboBox
            {
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "DisplayTime",
                ValueMember = "TimeSlotID",
                Location = new Point(0, 10),
                Font = new Font(this.Font.FontFamily, 10)
            };

            lblAvailableSlots = new Label
            {
                Text = "Please select an audiologist and date",
                AutoSize = true,
                Location = new Point(0, 40),
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font(this.Font, FontStyle.Italic)
            };

            timeSlotPanel.Controls.Add(cmbTimeSlots);
            timeSlotPanel.Controls.Add(lblAvailableSlots);

            bookingForm.Controls.Add(lblTime, 0, 3);
            bookingForm.Controls.Add(timeSlotPanel, 1, 3);

            // Purpose of Visit
            var lblPurpose = CreateLabel("Purpose:", 0, 0);
            lblPurpose.Dock = DockStyle.Fill;
            lblPurpose.TextAlign = ContentAlignment.MiddleLeft;
            lblPurpose.Margin = new Padding(3);

            txtPurpose = CreateTextBox(0, 0);
            txtPurpose.Dock = DockStyle.Fill;
            txtPurpose.Multiline = true;
            txtPurpose.Font = new Font(this.Font.FontFamily, 10);
            txtPurpose.Margin = new Padding(3);

            bookingForm.Controls.Add(lblPurpose, 0, 4);
            bookingForm.Controls.Add(txtPurpose, 1, 4);

            // Fee input
            var lblFee = CreateLabel("Appointment Fee ($):", 0, 0);
            lblFee.Dock = DockStyle.Fill;
            lblFee.TextAlign = ContentAlignment.MiddleLeft;
            lblFee.Margin = new Padding(3);
            lblFee.Font = new Font(lblFee.Font, FontStyle.Bold);
            lblFee.ForeColor = Color.FromArgb(192, 0, 0); // Red to indicate importance

            // Fee and buttons panel
            TableLayoutPanel feePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(3)
            };

            feePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            feePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            feePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            nudFee = CreateNumericUpDown(0, 0);
            nudFee.Dock = DockStyle.Fill;
            nudFee.Minimum = 0;
            nudFee.Maximum = 9999;
            nudFee.DecimalPlaces = 2;
            nudFee.Increment = 25M;
            nudFee.Value = 100.00M;
            nudFee.Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            nudFee.BackColor = Color.FromArgb(255, 255, 200); // Light yellow background
            nudFee.Margin = new Padding(3);

            btnBook = CreateButton("Book Appointment", 0, 0, BtnBook_Click, 150, 40);
            ApplyButtonStyle(btnBook);
            btnBook.Dock = DockStyle.Fill;
            btnBook.BackColor = Color.FromArgb(40, 167, 69); // Green for booking
            btnBook.FlatAppearance.BorderColor = Color.FromArgb(33, 136, 56);
            btnBook.Margin = new Padding(10, 3, 3, 3);

            btnCancel = CreateButton("Cancel", 0, 0, BtnCancel_Click, 100, 40);
            ApplyButtonStyle(btnCancel);
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.BackColor = Color.FromArgb(108, 117, 125); // Gray for cancel
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(90, 98, 104);
            btnCancel.Margin = new Padding(3);

            feePanel.Controls.Add(nudFee, 0, 0);
            feePanel.Controls.Add(btnBook, 1, 0);
            feePanel.Controls.Add(btnCancel, 2, 0);

            bookingForm.Controls.Add(lblFee, 0, 5);
            bookingForm.Controls.Add(feePanel, 1, 5);

            pnlBooking.Controls.Add(bookingForm);
            pnlBooking.Controls.Add(lblBooking);

            parent.Controls.Add(pnlBooking, 0, 0);
        }

        private void ApplyButtonStyle(Button button)
        {
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.Font = new Font(button.Font.FontFamily, 10, FontStyle.Bold);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            button.MouseEnter += (s, e) => {
                button.BackColor = ControlPaint.Dark(button.BackColor);
            };
            button.MouseLeave += (s, e) => {
                if (button == btnCancel)
                    button.BackColor = Color.FromArgb(108, 117, 125);
                else
                    button.BackColor = Color.FromArgb(40, 167, 69);
            };
        }
        #endregion

        #region Event Handlers
        private void CmbAudiologists_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableTimeSlots();
        }

        private void DtpDate_ValueChanged(object sender, EventArgs e)
        {
            LoadAvailableTimeSlots();
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                int patientId = (int)cmbPatients.SelectedValue;
                int audiologistId = (int)cmbAudiologists.SelectedValue;
                int timeSlotId = (int)cmbTimeSlots.SelectedValue;
                DateTime appointmentDate = dtpDate.Value;
                decimal fee = nudFee.Value;
                string purpose = txtPurpose.Text.Trim();

                // Create the new appointment (with "Confirmed" status for walk-in patients)
                var newAppointment = new Appointment
                {
                    PatientID = patientId,
                    AudiologistID = audiologistId,
                    Date = appointmentDate,
                    TimeSlotID = timeSlotId,
                    PurposeOfVisit = purpose,
                    Status = "Confirmed", // Set as confirmed by default for walk-in patients
                    Fee = fee,
                    CreatedBy = AuthService.CurrentUser.UserID // Set receptionist as creator
                };

                // Use repository to create appointment
                repository.CreateAppointment(newAppointment);

                ResetForm();
                UIService.ShowSuccess("Appointment booked successfully");
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Failed to book appointment: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Helper Methods
        private void LoadPatients()
        {
            // Use repository to get active patients
            var patients = repository.GetActivePatients();

            // Bind to combobox
            cmbPatients.DataSource = patients;
            cmbPatients.DisplayMember = "DisplayName";
            cmbPatients.ValueMember = "PatientID";
        }

        private void LoadAudiologists()
        {
            // Use repository to get active audiologists
            var audiologists = repository.GetActiveAudiologists();

            // Bind to combobox
            cmbAudiologists.DataSource = audiologists;
            cmbAudiologists.DisplayMember = "DisplayName";
            cmbAudiologists.ValueMember = "AudiologistID";
        }

        private void LoadAvailableTimeSlots()
        {
            cmbTimeSlots.DataSource = null;
            lblAvailableSlots.Text = "Loading available time slots...";
            lblAvailableSlots.ForeColor = Color.FromArgb(128, 128, 128);

            if (cmbAudiologists.SelectedValue == null)
            {
                lblAvailableSlots.Text = "Please select an audiologist";
                return;
            }

            int audiologistId = (int)cmbAudiologists.SelectedValue;
            DateTime selectedDate = dtpDate.Value.Date;

            // Use repository to get available time slots
            var availableSlots = repository.GetAvailableTimeSlots(audiologistId, selectedDate);

            cmbTimeSlots.DataSource = availableSlots;
            cmbTimeSlots.DisplayMember = "DisplayTime";
            cmbTimeSlots.ValueMember = "TimeSlotID";
            cmbTimeSlots.Enabled = availableSlots.Count > 0;

            if (availableSlots.Count > 0)
            {
                lblAvailableSlots.Text = $"{availableSlots.Count} time slots available";
                lblAvailableSlots.ForeColor = Color.FromArgb(46, 125, 50); // Dark green
            }
            else
            {
                string dayOfWeek = selectedDate.DayOfWeek.ToString();
                var schedule = repository.GetScheduleByAudiologistAndDay(audiologistId, dayOfWeek);

                if (schedule == null)
                {
                    lblAvailableSlots.Text = $"No schedule for {dayOfWeek}";
                }
                else
                {
                    lblAvailableSlots.Text = "No available time slots for this date";
                }

                lblAvailableSlots.ForeColor = Color.FromArgb(198, 40, 40); // Dark red
            }
        }

        private bool ValidateForm()
        {
            if (cmbPatients.SelectedIndex == -1)
            {
                UIService.ShowError("Please select a patient");
                cmbPatients.Focus();
                return false;
            }

            if (cmbAudiologists.SelectedIndex == -1)
            {
                UIService.ShowError("Please select an audiologist");
                cmbAudiologists.Focus();
                return false;
            }

            if (cmbTimeSlots.SelectedIndex == -1 || cmbTimeSlots.SelectedValue == null)
            {
                UIService.ShowError("Please select an available time slot");
                cmbTimeSlots.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPurpose.Text))
            {
                UIService.ShowError("Please enter the purpose of visit");
                txtPurpose.Focus();
                return false;
            }

            if (nudFee.Value <= 0)
            {
                UIService.ShowError("Please enter a valid appointment fee");
                nudFee.Focus();
                return false;
            }

            return true;
        }

        private void ResetForm()
        {
            if (cmbPatients.Items.Count > 0)
                cmbPatients.SelectedIndex = 0;

            if (cmbAudiologists.Items.Count > 0)
                cmbAudiologists.SelectedIndex = 0;

            dtpDate.Value = DateTime.Today;
            txtPurpose.Text = "";
            nudFee.Value = 100.00M; // Default fee

            LoadAvailableTimeSlots();
        }
        #endregion
    }
}