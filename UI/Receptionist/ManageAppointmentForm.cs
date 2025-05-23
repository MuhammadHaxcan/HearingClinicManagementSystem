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

namespace HearingClinicManagementSystem.UI.Receptionist
{
    public class ManageAppointmentForm : BaseForm
    {
        #region Fields
        private DataGridView dgvAppointments;
        private TextBox txtPatientName;
        private TextBox txtAudiologist;
        private TextBox txtSpecialization;
        private DateTimePicker dtpAppointmentDate;
        private TextBox txtAppointmentTime;
        private TextBox txtPurpose;
        private TextBox txtStatus;
        private NumericUpDown nudFee;
        private Label lblFee;
        private Button btnConfirm;
        private Button btnCancel;
        private Panel pnlDetail;
        private int selectedAppointmentId;
        private HearingClinicRepository repository;
        #endregion

        public ManageAppointmentForm()
        {
            repository = HearingClinicRepository.Instance;
            InitializeComponents();
            LoadAppointments();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Manage Appointments";
            var lblTitle = CreateTitleLabel("Manage Appointments");
            lblTitle.Dock = DockStyle.Top;

            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10),
                RowStyles = {
                    new RowStyle(SizeType.Percent, 60F),
                    new RowStyle(SizeType.Percent, 40F)
                }
            };

            InitializeAppointmentsPanel(mainPanel);
            InitializeDetailPanel(mainPanel);

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

            var lblAppointments = CreateLabel("Appointment Requests", 0, 0);
            lblAppointments.Dock = DockStyle.Top;
            lblAppointments.Font = new Font(lblAppointments.Font, FontStyle.Bold);
            lblAppointments.Height = 30;
            lblAppointments.TextAlign = ContentAlignment.MiddleLeft;
            lblAppointments.Padding = new Padding(5, 0, 0, 5);

            dgvAppointments = CreateDataGrid(0, false, true);
            dgvAppointments.Dock = DockStyle.Fill;
            dgvAppointments.MultiSelect = false;
            dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAppointments.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvAppointments.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Add columns
            dgvAppointments.Columns.Add("AppointmentID", "ID");
            dgvAppointments.Columns.Add("Date", "Date");
            dgvAppointments.Columns.Add("Time", "Time");
            dgvAppointments.Columns.Add("Patient", "Patient");
            dgvAppointments.Columns.Add("Audiologist", "Audiologist");
            dgvAppointments.Columns.Add("Specialization", "Specialization");
            dgvAppointments.Columns.Add("Purpose", "Purpose");
            dgvAppointments.Columns.Add("Status", "Status");
            dgvAppointments.Columns.Add("Fee", "Fee");
            dgvAppointments.Columns["AppointmentID"].Visible = false;

            // Set percentage-based widths relative to the total width
            int totalWidth = dgvAppointments.Width;
            dgvAppointments.Columns["Date"].Width = (int)(totalWidth * 0.10);
            dgvAppointments.Columns["Time"].Width = (int)(totalWidth * 0.11);
            dgvAppointments.Columns["Patient"].Width = (int)(totalWidth * 0.11);
            dgvAppointments.Columns["Audiologist"].Width = (int)(totalWidth * 0.16);
            dgvAppointments.Columns["Specialization"].Width = (int)(totalWidth * 0.13);
            dgvAppointments.Columns["Purpose"].Width = (int)(totalWidth * 0.22);
            dgvAppointments.Columns["Status"].Width = (int)(totalWidth * 0.12);
            dgvAppointments.Columns["Fee"].Width = (int)(totalWidth * 0.05);

            // Add resize event handler to maintain proportions when form is resized
            dgvAppointments.SizeChanged += (sender, e) => {
                int newTotalWidth = dgvAppointments.Width;
                dgvAppointments.Columns["Date"].Width = (int)(newTotalWidth * 0.10);
                dgvAppointments.Columns["Time"].Width = (int)(newTotalWidth * 0.11);
                dgvAppointments.Columns["Patient"].Width = (int)(newTotalWidth * 0.11);
                dgvAppointments.Columns["Audiologist"].Width = (int)(newTotalWidth * 0.16);
                dgvAppointments.Columns["Specialization"].Width = (int)(newTotalWidth * 0.13);
                dgvAppointments.Columns["Purpose"].Width = (int)(newTotalWidth * 0.22);
                dgvAppointments.Columns["Status"].Width = (int)(newTotalWidth * 0.12);
                dgvAppointments.Columns["Fee"].Width = (int)(newTotalWidth * 0.05);
            };

            // Make columns wrap text for better readability
            dgvAppointments.Columns["Purpose"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvAppointments.Columns["Audiologist"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvAppointments.Columns["Specialization"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Set minimum height for rows
            dgvAppointments.RowTemplate.MinimumHeight = 35;

            // Improve the appearance
            dgvAppointments.BorderStyle = BorderStyle.None;
            dgvAppointments.BackgroundColor = Color.White;
            dgvAppointments.GridColor = Color.FromArgb(230, 230, 230);
            dgvAppointments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvAppointments.ColumnHeadersDefaultCellStyle.Font = new Font(dgvAppointments.Font, FontStyle.Bold);
            dgvAppointments.ColumnHeadersHeight = 30;
            dgvAppointments.Margin = new Padding(0, 5, 0, 0);

            dgvAppointments.CellFormatting += DgvAppointments_CellFormatting;
            dgvAppointments.SelectionChanged += DgvAppointments_SelectionChanged;

            pnlAppointments.Controls.AddRange(new Control[] {
                lblAppointments,
                dgvAppointments
            });

            parent.Controls.Add(pnlAppointments, 0, 0);
        }

        private void InitializeDetailPanel(TableLayoutPanel parent)
        {
            // Create parent panels
            TableLayoutPanel detailContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                RowStyles = {
                    new RowStyle(SizeType.Percent, 70F),
                    new RowStyle(SizeType.Absolute, 80F)
                }
            };

            // Main details panel
            pnlDetail = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var lblDetailHeader = CreateLabel("Appointment Details", 0, 0);
            lblDetailHeader.Dock = DockStyle.Top;
            lblDetailHeader.Font = new Font(lblDetailHeader.Font, FontStyle.Bold);
            lblDetailHeader.Height = 30;
            lblDetailHeader.TextAlign = ContentAlignment.MiddleLeft;

            // Create a table layout for appointment details
            TableLayoutPanel detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(5)
            };

            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Fixed width for labels
            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < 6; i++)
            {
                detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            }

            // Patient name
            var lblPatient = CreateLabel("Patient:", 0, 0);
            lblPatient.Dock = DockStyle.Fill;
            lblPatient.TextAlign = ContentAlignment.MiddleRight;
            lblPatient.Margin = new Padding(5);

            txtPatientName = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                ReadOnly = true
            };

            detailsLayout.Controls.Add(lblPatient, 0, 0);
            detailsLayout.Controls.Add(txtPatientName, 1, 0);

            // Audiologist
            var lblAudiologist = CreateLabel("Audiologist:", 0, 0);
            lblAudiologist.Dock = DockStyle.Fill;
            lblAudiologist.TextAlign = ContentAlignment.MiddleRight;
            lblAudiologist.Margin = new Padding(5);

            txtAudiologist = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                ReadOnly = true
            };

            detailsLayout.Controls.Add(lblAudiologist, 0, 1);
            detailsLayout.Controls.Add(txtAudiologist, 1, 1);

            // Specialization
            var lblSpecialization = CreateLabel("Specialization:", 0, 0);
            lblSpecialization.Dock = DockStyle.Fill;
            lblSpecialization.TextAlign = ContentAlignment.MiddleRight;
            lblSpecialization.Margin = new Padding(5);

            txtSpecialization = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                ReadOnly = true
            };

            detailsLayout.Controls.Add(lblSpecialization, 0, 2);
            detailsLayout.Controls.Add(txtSpecialization, 1, 2);

            // Date and Time
            var lblDatetime = CreateLabel("Date & Time:", 0, 0);
            lblDatetime.Dock = DockStyle.Fill;
            lblDatetime.TextAlign = ContentAlignment.MiddleRight;
            lblDatetime.Margin = new Padding(5);

            // Panel to hold date and time side by side
            TableLayoutPanel datetimePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };

            dtpAppointmentDate = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(5, 5, 2, 5),
                Enabled = false
            };

            txtAppointmentTime = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(2, 5, 5, 5),
                ReadOnly = true
            };

            datetimePanel.Controls.Add(dtpAppointmentDate, 0, 0);
            datetimePanel.Controls.Add(txtAppointmentTime, 1, 0);
            detailsLayout.Controls.Add(lblDatetime, 0, 3);
            detailsLayout.Controls.Add(datetimePanel, 1, 3);

            // Purpose
            var lblPurpose = CreateLabel("Purpose:", 0, 0);
            lblPurpose.Dock = DockStyle.Fill;
            lblPurpose.TextAlign = ContentAlignment.MiddleRight;
            lblPurpose.Margin = new Padding(5);

            txtPurpose = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                ReadOnly = true,
                Multiline = true
            };

            detailsLayout.Controls.Add(lblPurpose, 0, 4);
            detailsLayout.Controls.Add(txtPurpose, 1, 4);

            // Status
            var lblStatus = CreateLabel("Status:", 0, 0);
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            lblStatus.Margin = new Padding(5);

            txtStatus = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                ReadOnly = true
            };

            detailsLayout.Controls.Add(lblStatus, 0, 5);
            detailsLayout.Controls.Add(txtStatus, 1, 5);

            pnlDetail.Controls.Add(detailsLayout);
            pnlDetail.Controls.Add(lblDetailHeader);

            // Create fee and buttons panel (separate from details)
            Panel actionsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10, 5, 10, 5)
            };

            // Create fee section with better positioning
            lblFee = CreateLabel("Set Appointment Fee ($):", 20, 15);
            lblFee.AutoSize = true;
            lblFee.Font = new Font(lblFee.Font, FontStyle.Bold);
            lblFee.ForeColor = Color.FromArgb(192, 0, 0); // Red to make it stand out

            nudFee = new NumericUpDown
            {
                Location = new Point(200, 15),
                Size = new Size(120, 25),
                Minimum = 0,
                Maximum = 9999,
                DecimalPlaces = 2,
                Increment = 25M,
                Value = 100.00M,
                BackColor = Color.FromArgb(255, 255, 200),
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
            };

            // Add confirm and cancel buttons
            btnConfirm = CreateButton("Confirm Appointment", actionsPanel.Width - 320, 15, BtnConfirm_Click, 150, 35);
            ApplyButtonStyle(btnConfirm);
            btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnCancel = CreateButton("Cancel Appointment", actionsPanel.Width - 160, 15, BtnCancel_Click, 150, 35);
            ApplyButtonStyle(btnCancel);
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.BackColor = Color.FromArgb(220, 53, 69); // Red for cancel
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(195, 25, 45);

            actionsPanel.Controls.Add(lblFee);
            actionsPanel.Controls.Add(nudFee);
            actionsPanel.Controls.Add(btnConfirm);
            actionsPanel.Controls.Add(btnCancel);

            detailContainer.Controls.Add(pnlDetail, 0, 0);
            detailContainer.Controls.Add(actionsPanel, 0, 1);

            parent.Controls.Add(detailContainer, 0, 1);

            // Initially disable action buttons until an appointment is selected
            SetActionButtonsState(false);
        }
        #endregion

        #region Event Handlers
        private void DgvAppointments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dgvAppointments.Rows[e.RowIndex];
                string status = row.Cells["Status"].Value?.ToString();

                // Format the Fee column - show only dash (-) for pending appointments
                if (dgvAppointments.Columns[e.ColumnIndex].Name == "Fee")
                {
                    if (status == "Pending")
                    {
                        e.Value = "-";
                        e.CellStyle.ForeColor = Color.Gray;
                        e.CellStyle.Font = new Font(dgvAppointments.Font, FontStyle.Italic);
                    }
                    else if (e.Value != null && e.Value.ToString() == "0")
                    {
                        e.Value = "-";
                    }
                }

                // Apply different background colors based on status
                if (status == "Pending")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 225); // Light yellow
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(215, 155, 0); // Gold

                    // Highlight fee cell for pending appointments to indicate it needs to be set
                    if (row.Cells["Fee"] != null)
                    {
                        row.Cells["Fee"].Style.BackColor = Color.FromArgb(255, 250, 230); // Slightly highlighted
                    }
                }
                else if (status == "Confirmed")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233); // Light green
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(46, 125, 50); // Dark green
                }
                else if (status == "Cancelled")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(251, 233, 231); // Light red
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(198, 40, 40); // Dark red
                }
                else if (status == "Completed")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(232, 240, 254); // Light blue
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(30, 81, 123); // Dark blue
                }

                // Apply specific formatting to the Date column
                if (dgvAppointments.Columns[e.ColumnIndex].Name == "Date")
                {
                    if (e.Value != null && e.Value.ToString() == DateTime.Now.ToShortDateString())
                    {
                        e.CellStyle.Font = new Font(dgvAppointments.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private void DgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count > 0)
            {
                selectedAppointmentId = (int)dgvAppointments.SelectedRows[0].Cells["AppointmentID"].Value;
                LoadAppointmentDetails(selectedAppointmentId);

                // Get the appointment status
                string status = dgvAppointments.SelectedRows[0].Cells["Status"].Value.ToString();
                
                // Enable/disable buttons based on appointment status
                // Only Pending appointments can be confirmed
                bool canConfirm = status == "Pending";
                // Only Pending appointments can be cancelled - confirmed appointments cannot be cancelled
                bool canCancel = status == "Pending";
                
                // Update buttons' state
                btnConfirm.Enabled = canConfirm;
                btnCancel.Enabled = canCancel;

                // Make fee setting more obvious for pending appointments
                if (status == "Pending")
                {
                    nudFee.Enabled = true;
                    nudFee.Focus();
                    lblFee.ForeColor = Color.FromArgb(192, 0, 0); // Red to indicate required

                    // Highlight the fee field but DON'T show the annoying popup
                    nudFee.BackColor = Color.FromArgb(255, 255, 200); // Bright yellow background
                    nudFee.Select(0, nudFee.Text.Length); // Select all text for easy replacement
                }
                else
                {
                    nudFee.Enabled = false;
                    lblFee.ForeColor = Color.FromArgb(0, 100, 150); // Back to normal
                    nudFee.BackColor = SystemColors.Control; // Gray for non-editable
                }
            }
            else
            {
                ClearAppointmentDetails();
                SetActionButtonsState(false);
            }
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (selectedAppointmentId <= 0)
            {
                UIService.ShowError("Please select an appointment first");
                return;
            }

            // Show a more descriptive error if fee is not set
            if (nudFee.Value <= 0)
            {
                UIService.ShowError("Please enter a fee amount in the highlighted field before confirming");
                nudFee.Focus();
                return;
            }

            // Confirm the appointment fee with the user - simplified dialog
            DialogResult confirmResult = MessageBox.Show(
                $"Confirm appointment with fee: {nudFee.Value:C}?",
                "Confirm Appointment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.No)
            {
                return;
            }

            try
            {
                // Update appointment status and fee using repository
                bool success = repository.ConfirmAppointment(selectedAppointmentId, nudFee.Value);
                
                if (success)
                {
                    UIService.ShowSuccess($"Appointment confirmed with fee: {nudFee.Value:C}");
                    LoadAppointments();
                }
                else
                {
                    UIService.ShowError("Failed to confirm appointment. It may have already been processed.");
                }
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error confirming appointment: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (selectedAppointmentId <= 0)
            {
                UIService.ShowError("Please select an appointment first");
                return;
            }
            
            // Get the current appointment status
            string status = string.Empty;
            foreach (DataGridViewRow row in dgvAppointments.Rows)
            {
                if ((int)row.Cells["AppointmentID"].Value == selectedAppointmentId)
                {
                    status = row.Cells["Status"].Value.ToString();
                    break;
                }
            }
            
            // Check if the appointment is confirmed - if so, show error and return
            if (status == "Confirmed")
            {
                UIService.ShowError("Cannot cancel confirmed appointments. Please contact a manager if this is necessary.");
                return;
            }

            try
            {
                // Cancel appointment using repository
                bool success = repository.CancelAppointment(selectedAppointmentId);
                
                if (success)
                {
                    LoadAppointments();
                    UIService.ShowSuccess("Appointment cancelled successfully");
                }
                else
                {
                    UIService.ShowError("Failed to cancel appointment. It may have already been processed.");
                }
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error cancelling appointment: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods
        private void ApplyButtonStyle(Button button)
        {
            // Use a more professional blue color
            button.BackColor = Color.FromArgb(51, 122, 183);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(40, 96, 144);
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            // Add hover effects
            button.MouseEnter += (s, e) => {
                button.BackColor = Color.FromArgb(40, 96, 144); // Darker blue on hover
            };
            button.MouseLeave += (s, e) => {
                if (button == btnCancel)
                    button.BackColor = Color.FromArgb(220, 53, 69); // Red for cancel
                else
                    button.BackColor = Color.FromArgb(51, 122, 183); // Blue for confirm
            };
        }

        private void LoadAppointments()
        {
            dgvAppointments.Rows.Clear();
            selectedAppointmentId = 0;

            var appointments = repository.GetAllAppointmentsWithDetails();

            foreach (var appointment in appointments)
            {
                var patient = appointment.Patient;
                var audiologist = appointment.Audiologist;
                var timeSlot = appointment.TimeSlot;

                if (patient?.User != null && audiologist?.User != null && timeSlot != null)
                {
                    // Show the full purpose text - the grid will handle wrapping
                    string purpose = appointment.PurposeOfVisit;

                    // Always display "-" for pending appointments, even if fee is set
                    string feeDisplay = appointment.Status == "Pending" ?
                        "-" :
                        appointment.Fee > 0 ? appointment.Fee.ToString("C") : "-";

                    dgvAppointments.Rows.Add(
                        appointment.AppointmentID,
                        appointment.Date.ToShortDateString(),
                        $"{timeSlot.StartTime.ToString(@"hh\:mm")} - {timeSlot.EndTime.ToString(@"hh\:mm")}",
                        $"{patient.User.FirstName} {patient.User.LastName}",
                        $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}",
                        audiologist.Specialization ?? "General",  // Show specialization
                        purpose,
                        appointment.Status,
                        feeDisplay  // Display fee based on status
                    );
                }
            }

            ClearAppointmentDetails();
            SetActionButtonsState(false);

            // Apply additional styling to Status column
            foreach (DataGridViewRow row in dgvAppointments.Rows)
            {
                string status = row.Cells["Status"].Value?.ToString();

                // Make the status column text bold
                if (!string.IsNullOrEmpty(status))
                {
                    row.Cells["Status"].Style.Font = new Font(dgvAppointments.Font, FontStyle.Bold);
                }
            }
        }

        private void LoadAppointmentDetails(int appointmentId)
        {
            var appointment = repository.GetAppointmentWithDetails(appointmentId);
            if (appointment == null)
            {
                ClearAppointmentDetails();
                return;
            }

            var patient = appointment.Patient;
            var audiologist = appointment.Audiologist;
            var timeSlot = appointment.TimeSlot;

            if (patient?.User != null && audiologist?.User != null && timeSlot != null)
            {
                txtPatientName.Text = $"{patient.User.FirstName} {patient.User.LastName}";
                txtAudiologist.Text = $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}";
                txtSpecialization.Text = audiologist.Specialization ?? "General";
                dtpAppointmentDate.Value = appointment.Date;
                txtAppointmentTime.Text = $"{timeSlot.StartTime.ToString(@"hh\:mm")} - {timeSlot.EndTime.ToString(@"hh\:mm")}";
                txtPurpose.Text = appointment.PurposeOfVisit;
                txtStatus.Text = appointment.Status;

                // For pending appointments, set a default fee value for the receptionist to adjust
                // For confirmed appointments, show the actual fee that was set
                if (appointment.Status == "Pending")
                {
                    nudFee.Value = 100.00M; // Default fee suggestion
                }
                else
                {
                    nudFee.Value = appointment.Fee > 0 ? appointment.Fee : 0.00M;
                }

                // Update button state based on appointment status
                bool isActionable = appointment.Status == "Pending";
                SetActionButtonsState(isActionable);
            }
            else
            {
                ClearAppointmentDetails();
            }
        }

        private void ClearAppointmentDetails()
        {
            txtPatientName.Text = string.Empty;
            txtAudiologist.Text = string.Empty;
            txtSpecialization.Text = string.Empty;
            dtpAppointmentDate.Value = DateTime.Now;
            txtAppointmentTime.Text = string.Empty;
            txtPurpose.Text = string.Empty;
            txtStatus.Text = string.Empty;
            nudFee.Value = 100.00M; // Default value
            lblFee.Text = "Fee ($):"; // Reset label text
            lblFee.ForeColor = Color.FromArgb(0, 100, 150); // Reset label color
            selectedAppointmentId = 0;
        }

        private void SetActionButtonsState(bool enabled)
        {
            // This is now handled directly in DgvAppointments_SelectionChanged
            // Keep this method for backward compatibility but make it only affect the fee
            nudFee.Enabled = enabled;

            // Visual indication that fee is editable only for pending appointments
            if (enabled)
            {
                nudFee.BackColor = Color.FromArgb(255, 255, 240); // Light yellow to indicate editable
            }
            else
            {
                nudFee.BackColor = SystemColors.Control; // Gray for non-editable
            }
        }
        #endregion
    }
}