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
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;

namespace HearingClinicManagementSystem.UI.Patient
{
    public class ManageAppointmentForm : BaseForm
    {
        #region Fields
        private DataGridView dgvAppointments;
        private ComboBox cmbAudiologists;
        private DateTimePicker dtpDate;
        private ComboBox cmbTimeSlots;
        private TextBox txtPurpose;
        private Button btnBook;
        private Button btnCancel;
        #endregion

        public ManageAppointmentForm()
        {
            InitializeComponents();
            LoadAppointments();
            LoadAudiologists();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = AppStrings.Titles.ManageAppointment;
            var lblTitle = CreateTitleLabel(AppStrings.Titles.ManageAppointment);
            lblTitle.Dock = DockStyle.Top;

            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10),
                RowStyles = {
                    new RowStyle(SizeType.Percent, 35F),
                    new RowStyle(SizeType.Percent, 65F)
                }
            };

            InitializeAppointmentsPanel(mainPanel);
            InitializeBookingPanel(mainPanel);

            Controls.Add(mainPanel);
            Controls.Add(lblTitle);
        }

        private void InitializeAppointmentsPanel(TableLayoutPanel parent)
        {
            Panel pnlAppointments = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            var lblAppointments = CreateLabel("Your Appointments", 0, 0);
            lblAppointments.Dock = DockStyle.Top;
            lblAppointments.Font = new Font(lblAppointments.Font, FontStyle.Bold);
            lblAppointments.Height = 25;
            lblAppointments.TextAlign = ContentAlignment.MiddleLeft;

            dgvAppointments = CreateDataGrid(0, false, true);
            dgvAppointments.Dock = DockStyle.Fill;
            dgvAppointments.MultiSelect = false;
            dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAppointments.Columns.Add("AppointmentID", "ID");
            dgvAppointments.Columns.Add("Date", "Date");
            dgvAppointments.Columns.Add("Time", "Time");
            dgvAppointments.Columns.Add("Audiologist", "Audiologist");
            dgvAppointments.Columns.Add("Specialization", "Specialization");
            dgvAppointments.Columns.Add("Purpose", "Purpose");
            dgvAppointments.Columns.Add("Status", "Status");
            dgvAppointments.Columns.Add("PaymentStatus", "Payment Status");
            dgvAppointments.Columns["AppointmentID"].Visible = false;

            // Removed the blue separator line

            Panel pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            btnCancel = CreateButton("Cancel Appointment", 10, 8, BtnCancel_Click, 150, 24);
            ApplyButtonStyle(btnCancel);
            pnlButtons.Controls.Add(btnCancel);

            pnlAppointments.Controls.AddRange(new Control[] {
                lblAppointments,
                dgvAppointments,
                pnlButtons
            });

            parent.Controls.Add(pnlAppointments, 0, 0);
        }

        private void InitializeBookingPanel(TableLayoutPanel parent)
        {
            Panel pnlBooking = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            var lblBooking = CreateLabel("Book a New Appointment", 0, 0);
            lblBooking.Dock = DockStyle.Top;
            lblBooking.Font = new Font(lblBooking.Font, FontStyle.Bold);
            lblBooking.Height = 30;
            lblBooking.TextAlign = ContentAlignment.MiddleLeft;

            // Create a more compact layout with less gaps
            TableLayoutPanel bookingForm = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(3)
            };

            bookingForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            bookingForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));

            // Reduced row height to minimize gaps
            for (int i = 0; i < 4; i++)
            {
                bookingForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            }

            // Audiologist Selection
            var lblAudiologist = CreateLabel("Audiologist:", 0, 0);
            lblAudiologist.Dock = DockStyle.Fill;
            lblAudiologist.TextAlign = ContentAlignment.MiddleLeft;
            lblAudiologist.Margin = new Padding(3);

            cmbAudiologists = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(3)
            };
            cmbAudiologists.SelectedIndexChanged += CmbAudiologists_SelectedIndexChanged;

            bookingForm.Controls.Add(lblAudiologist, 0, 0);
            bookingForm.Controls.Add(cmbAudiologists, 1, 0);

            // Date Selection
            var lblDate = CreateLabel("Date:", 0, 0);
            lblDate.Dock = DockStyle.Fill;
            lblDate.TextAlign = ContentAlignment.MiddleLeft;
            lblDate.Margin = new Padding(3);

            dtpDate = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Now.AddDays(1),
                Value = DateTime.Now.AddDays(1),
                Margin = new Padding(3)
            };
            dtpDate.ValueChanged += DtpDate_ValueChanged;

            bookingForm.Controls.Add(lblDate, 0, 1);
            bookingForm.Controls.Add(dtpDate, 1, 1);

            // Time Selection
            var lblTime = CreateLabel("Time:", 0, 0);
            lblTime.Dock = DockStyle.Fill;
            lblTime.TextAlign = ContentAlignment.MiddleLeft;
            lblTime.Margin = new Padding(3);

            cmbTimeSlots = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(3)
            };

            bookingForm.Controls.Add(lblTime, 0, 2);
            bookingForm.Controls.Add(cmbTimeSlots, 1, 2);

            // Purpose of Visit
            var lblPurpose = CreateLabel("Purpose:", 0, 0);
            lblPurpose.Dock = DockStyle.Fill;
            lblPurpose.TextAlign = ContentAlignment.TopLeft;
            lblPurpose.Margin = new Padding(3);

            txtPurpose = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };

            bookingForm.Controls.Add(lblPurpose, 0, 3);

            // Create a panel to hold the text box and button side by side
            TableLayoutPanel purposePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };

            purposePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            purposePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));

            purposePanel.Controls.Add(txtPurpose, 0, 0);

            // Button aligned to the right
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            btnBook = CreateButton("Book Appointment", 0, 10, BtnBook_Click, 150, 30);
            ApplyButtonStyle(btnBook);

            // Align button to the right and vertically center
            btnBook.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnBook.Location = new Point(buttonPanel.Width - btnBook.Width - 5, 10);
            buttonPanel.Controls.Add(btnBook);

            purposePanel.Controls.Add(buttonPanel, 1, 0);
            bookingForm.Controls.Add(purposePanel, 1, 3);

            pnlBooking.Controls.Add(bookingForm);
            pnlBooking.Controls.Add(lblBooking);

            parent.Controls.Add(pnlBooking, 0, 1);
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
            if (cmbAudiologists.SelectedIndex == -1 || cmbTimeSlots.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtPurpose.Text))
            {
                UIService.ShowError("Please fill all fields");
                return;
            }

            var newAppointment = new Appointment
            {
                AppointmentID = StaticDataProvider.Appointments.Count > 0 ?
                    StaticDataProvider.Appointments.Max(a => a.AppointmentID) + 1 : 1,
                PatientID = AuthService.CurrentPatient.PatientID,
                AudiologistID = StaticDataProvider.Audiologists[cmbAudiologists.SelectedIndex].AudiologistID,
                CreatedBy = AuthService.CurrentPatient.UserID,
                Date = dtpDate.Value,
                TimeSlotID = GetSelectedTimeSlotId(),
                PurposeOfVisit = txtPurpose.Text,
                Status = "Pending",
                Fee = 100.00m
            };

            StaticDataProvider.Appointments.Add(newAppointment);
            UpdateTimeSlotAvailability(newAppointment.TimeSlotID, false);
            LoadAppointments();
            ResetBookingForm();
            UIService.ShowSuccess("Appointment booked successfully!");
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count == 0)
            {
                UIService.ShowError("Please select an appointment to cancel");
                return;
            }

            int appointmentId = (int)dgvAppointments.SelectedRows[0].Cells["AppointmentID"].Value;
            string status = dgvAppointments.SelectedRows[0].Cells["Status"].Value.ToString();

            if (status == "Completed" || status == "Cancelled")
            {
                UIService.ShowError($"Cannot cancel an appointment that is already {status.ToLower()}");
                return;
            }

            var selectedAppointment = StaticDataProvider.Appointments.FirstOrDefault(a => a.AppointmentID == appointmentId);
            if (selectedAppointment != null)
            {
                selectedAppointment.Status = "Cancelled";
                UpdateTimeSlotAvailability(selectedAppointment.TimeSlotID, true);
                LoadAppointments();
                LoadAvailableTimeSlots();
                UIService.ShowSuccess("Appointment cancelled successfully");
            }
        }
        #endregion

        #region Helper Methods
        private void ApplyButtonStyle(Button button)
        {
            button.BackColor = Color.FromArgb(51, 122, 183);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(40, 96, 144);
            button.FlatAppearance.BorderColor = Color.White;
            button.Font = new Font(button.Font.FontFamily, button.Font.Size - 1, FontStyle.Regular);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
            button.MouseEnter += (s, e) => {
                button.BackColor = Color.FromArgb(40, 96, 144);
            };
            button.MouseLeave += (s, e) => {
                button.BackColor = Color.FromArgb(51, 122, 183);
            };
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 71, 109);
        }

        private void LoadAppointments()
        {
            dgvAppointments.Rows.Clear();
            if (AuthService.CurrentPatient == null)
            {
                MessageBox.Show("No patient is currently logged in", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var patientAppointments = StaticDataProvider.Appointments
                .Where(a => a.PatientID == AuthService.CurrentPatient.PatientID)
                .OrderBy(a => a.Date);

            foreach (var appointment in patientAppointments)
            {
                var timeSlot = StaticDataProvider.TimeSlots.FirstOrDefault(ts => ts.TimeSlotID == appointment.TimeSlotID);
                var audiologist = StaticDataProvider.Audiologists.FirstOrDefault(a => a.AudiologistID == appointment.AudiologistID);

                // Get payment status from invoice
                var invoice = StaticDataProvider.Invoices.FirstOrDefault(i => i.AppointmentID == appointment.AppointmentID);
                string paymentStatus = invoice != null ? invoice.Status : "Not Invoiced";

                if (timeSlot != null && audiologist != null && audiologist.User != null)
                {
                    dgvAppointments.Rows.Add(
                        appointment.AppointmentID,
                        appointment.Date.ToShortDateString(),
                        $"{timeSlot.StartTime.ToString(@"hh\:mm")} - {timeSlot.EndTime.ToString(@"hh\:mm")}",
                        $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}",
                        audiologist.Specialization ?? "General",
                        appointment.PurposeOfVisit,
                        appointment.Status,
                        paymentStatus
                    );
                }
            }

            // Customize the appearance based on status
            foreach (DataGridViewRow row in dgvAppointments.Rows)
            {
                string status = row.Cells["Status"].Value.ToString();
                string paymentStatus = row.Cells["PaymentStatus"].Value.ToString();

                if (status == "Completed")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 230); // Light green
                }
                else if (status == "Cancelled")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230); // Light red
                }

                if (paymentStatus == "Paid")
                {
                    row.Cells["PaymentStatus"].Style.ForeColor = Color.Green;
                    row.Cells["PaymentStatus"].Style.Font = new Font(dgvAppointments.Font, FontStyle.Bold);
                }
                else if (paymentStatus == "Pending")
                {
                    row.Cells["PaymentStatus"].Style.ForeColor = Color.Orange;
                }
            }

            // Select first row if available
            if (dgvAppointments.Rows.Count > 0)
            {
                dgvAppointments.Rows[0].Selected = true;
            }
        }

        private void LoadAudiologists()
        {
            cmbAudiologists.Items.Clear();
            foreach (var audiologist in StaticDataProvider.Audiologists)
            {
                cmbAudiologists.Items.Add($"Dr. {audiologist.User.FirstName} {audiologist.User.LastName} - {audiologist.Specialization}");
            }
            if (cmbAudiologists.Items.Count > 0)
                cmbAudiologists.SelectedIndex = 0;
        }

        private void LoadAvailableTimeSlots()
        {
            cmbTimeSlots.Items.Clear();
            if (cmbAudiologists.SelectedIndex == -1) return;

            var selectedAudiologist = StaticDataProvider.Audiologists[cmbAudiologists.SelectedIndex];
            var dayOfWeek = dtpDate.Value.DayOfWeek.ToString();
            var schedule = StaticDataProvider.Schedules
                .FirstOrDefault(s => s.AudiologistID == selectedAudiologist.AudiologistID && s.DayOfWeek == dayOfWeek);

            if (schedule != null)
            {
                var availableSlots = StaticDataProvider.TimeSlots
                    .Where(ts => ts.ScheduleID == schedule.ScheduleID && ts.IsAvailable)
                    .OrderBy(ts => ts.StartTime);

                foreach (var slot in availableSlots)
                {
                    cmbTimeSlots.Items.Add($"{slot.StartTime.ToString(@"hh\:mm")} - {slot.EndTime.ToString(@"hh\:mm")}");
                }

                if (cmbTimeSlots.Items.Count > 0)
                    cmbTimeSlots.SelectedIndex = 0;
                else
                    cmbTimeSlots.Items.Add("No available time slots");
            }
            else
            {
                cmbTimeSlots.Items.Add($"No schedule for {dayOfWeek}");
            }
        }

        private int GetSelectedTimeSlotId()
        {
            if (cmbAudiologists.SelectedIndex == -1 || cmbTimeSlots.SelectedIndex == -1) return -1;

            var selectedAudiologist = StaticDataProvider.Audiologists[cmbAudiologists.SelectedIndex];
            var dayOfWeek = dtpDate.Value.DayOfWeek.ToString();
            var schedule = StaticDataProvider.Schedules
                .FirstOrDefault(s => s.AudiologistID == selectedAudiologist.AudiologistID && s.DayOfWeek == dayOfWeek);

            if (schedule != null)
            {
                var timeRange = cmbTimeSlots.SelectedItem.ToString().Split('-');
                if (timeRange.Length >= 1)
                {
                    var startTime = TimeSpan.Parse(timeRange[0].Trim());
                    var slot = StaticDataProvider.TimeSlots
                        .FirstOrDefault(ts => ts.ScheduleID == schedule.ScheduleID &&
                                          ts.StartTime == startTime &&
                                          ts.IsAvailable);
                    if (slot != null)
                    {
                        return slot.TimeSlotID;
                    }
                }
            }
            return -1;
        }

        private void UpdateTimeSlotAvailability(int timeSlotId, bool isAvailable)
        {
            var timeSlot = StaticDataProvider.TimeSlots.FirstOrDefault(ts => ts.TimeSlotID == timeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsAvailable = isAvailable;
            }
        }

        private void ResetBookingForm()
        {
            if (cmbAudiologists.Items.Count > 0)
                cmbAudiologists.SelectedIndex = 0;
            dtpDate.Value = DateTime.Now.AddDays(1);
            txtPurpose.Text = "";
            LoadAvailableTimeSlots();
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ManageAppointmentForm
            // 
            this.ClientSize = new System.Drawing.Size(1250, 785);
            this.Name = "ManageAppointmentForm";
            this.ResumeLayout(false);

        }
    }
}