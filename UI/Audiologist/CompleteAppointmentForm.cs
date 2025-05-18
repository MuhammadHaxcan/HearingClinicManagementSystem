using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Constants;

namespace HearingClinicManagementSystem.UI.Audiologist
{
    public class CompleteAppointmentForm : BaseForm
    {
        #region Fields
        private DateTimePicker dtpAppointmentDate;
        private ComboBox cmbAppointments;
        private Button btnRefreshAppointments;
        private Panel appointmentSelectionPanel;
        private Panel patientInfoPanel;
        private Panel diagnosisPanel;
        private Panel prescriptionPanel;
        private Label lblPatientInfo;
        private Label lblAppointmentInfo;
        private Label lblDiagnosis;
        private CheckBox chkNeedsPrescription;
        private GroupBox grpHearingAids;
        private ComboBox cmbRecommendedDevice;
        private Button btnCompleteAppointment;
        private int? selectedAppointmentId;
        private int? selectedPatientId;
        private int? selectedMedicalRecordId;
        private List<Product> products;
        #endregion

        public CompleteAppointmentForm()
        {
            products = new List<Product>();
            InitializeComponents();
            LoadAppointmentsForDate(DateTime.Today);
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Complete Appointment";
            this.Size = new Size(1000, 700);

            // Create title
            var lblTitle = CreateTitleLabel("Complete Appointment & Prescribe Treatment");
            lblTitle.Dock = DockStyle.Top;

            // Main layout panel
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White,
                Padding = new Padding(5),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };

            // Configure row styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));  // Row for appointment selection
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));   // Row for patient info
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));  // Row for diagnosis
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));  // Row for prescription - reduced height

            // Create the panels
            Panel selectionPanel = CreateSelectionPanel();
            patientInfoPanel = CreatePatientInfoPanel();
            diagnosisPanel = CreateDiagnosisPanel();
            prescriptionPanel = CreatePrescriptionPanel();

            // Add panels to the main layout
            mainPanel.Controls.Add(selectionPanel, 0, 0);
            mainPanel.Controls.Add(patientInfoPanel, 0, 1);
            mainPanel.Controls.Add(diagnosisPanel, 0, 2);
            mainPanel.Controls.Add(prescriptionPanel, 0, 3);

            // Add all controls to the form
            Controls.Add(mainPanel);
            Controls.Add(lblTitle);
        }

        private Panel CreateSelectionPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Section title
            Label lblSectionTitle = CreateLabel("Appointment Selection", 0, 0);
            lblSectionTitle.Font = new Font(lblSectionTitle.Font, FontStyle.Bold);
            lblSectionTitle.Dock = DockStyle.Top;
            lblSectionTitle.Height = 25;

            // Create appointment selection panel with no internal padding - fix for white box issue
            appointmentSelectionPanel = new Panel
            {
                Location = new Point(10, 35),
                Width = panel.Width - 20,
                Height = 90,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(0) // Set padding to 0 to avoid white box overlapping controls
            };

            // Date picker for appointment selection
            Label lblDate = new Label
            {
                Text = "Select Date:",
                Location = new Point(10, 20),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Create DateTimePicker directly with explicit location and size - fix for white box
            dtpAppointmentDate = new DateTimePicker();
            dtpAppointmentDate.Location = new Point(100, 18);
            dtpAppointmentDate.Size = new Size(150, 23);
            dtpAppointmentDate.Format = DateTimePickerFormat.Short;
            dtpAppointmentDate.Value = DateTime.Today;
            dtpAppointmentDate.ValueChanged += DtpAppointmentDate_ValueChanged;

            // Create Refresh button directly
            btnRefreshAppointments = new Button();
            btnRefreshAppointments.Text = "Refresh";
            btnRefreshAppointments.Size = new Size(90, 25);
            btnRefreshAppointments.Location = new Point(260, 17);
            btnRefreshAppointments.Click += BtnRefreshAppointments_Click;
            ApplyButtonStyle(btnRefreshAppointments, Color.FromArgb(108, 117, 125));

            // Appointments dropdown
            Label lblSelectAppointment = new Label
            {
                Text = "Select Appointment to Complete:",
                Location = new Point(370, 20),
                Size = new Size(180, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Create ComboBox directly with explicit location and size - fix for white box
            cmbAppointments = new ComboBox();
            cmbAppointments.Location = new Point(560, 18);
            cmbAppointments.Size = new Size(400, 23);
            cmbAppointments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbAppointments.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAppointments.SelectedIndexChanged += CmbAppointments_SelectedIndexChanged;

            // Add controls to appointment selection panel
            appointmentSelectionPanel.Controls.Add(lblDate);
            appointmentSelectionPanel.Controls.Add(dtpAppointmentDate);
            appointmentSelectionPanel.Controls.Add(btnRefreshAppointments);
            appointmentSelectionPanel.Controls.Add(lblSelectAppointment);
            appointmentSelectionPanel.Controls.Add(cmbAppointments);

            // Add panels to main selection panel
            panel.Controls.Add(appointmentSelectionPanel);
            panel.Controls.Add(lblSectionTitle);

            return panel;
        }

        private Panel CreatePatientInfoPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Patient info labels
            lblPatientInfo = CreateLabel("No patient selected", 10, 10);
            lblPatientInfo.Font = new Font(lblPatientInfo.Font.FontFamily, 10, FontStyle.Bold);
            lblPatientInfo.Size = new Size(panel.Width - 20, 20);
            lblPatientInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblAppointmentInfo = CreateLabel("No appointment selected", 10, 30);
            lblAppointmentInfo.Size = new Size(panel.Width - 20, 20);
            lblAppointmentInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panel.Controls.Add(lblPatientInfo);
            panel.Controls.Add(lblAppointmentInfo);

            return panel;
        }

        private Panel CreateDiagnosisPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Section title
            Label lblSectionTitle = CreateLabel("Diagnosis", 0, 0);
            lblSectionTitle.Font = new Font(lblSectionTitle.Font, FontStyle.Bold);
            lblSectionTitle.Dock = DockStyle.Top;
            lblSectionTitle.Height = 25;

            // Create diagnosis label
            lblDiagnosis = new Label
            {
                Location = new Point(10, 30),
                Size = new Size(panel.Width - 20, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11),
                Text = "No diagnosis information available"
            };

            panel.Controls.Add(lblDiagnosis);
            panel.Controls.Add(lblSectionTitle);

            return panel;
        }

        private Panel CreatePrescriptionPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Section title
            Label lblSectionTitle = CreateLabel("Treatment Recommendation", 0, 0);
            lblSectionTitle.Font = new Font(lblSectionTitle.Font, FontStyle.Bold);
            lblSectionTitle.Dock = DockStyle.Top;
            lblSectionTitle.Height = 25;

            // Prescription checkbox
            chkNeedsPrescription = new CheckBox
            {
                Text = "Patient needs hearing aid",
                Location = new Point(10, 35),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                Checked = false
            };

            chkNeedsPrescription.CheckedChanged += (s, e) =>
            {
                grpHearingAids.Enabled = chkNeedsPrescription.Checked;
            };

            // Hearing aid group
            grpHearingAids = new GroupBox
            {
                Text = "Recommended Hearing Aid",
                Location = new Point(10, 65),
                Size = new Size(panel.Width - 20, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Enabled = false
            };

            Label lblRecommendedDevice = new Label
            {
                Text = "Recommended Device:",
                Location = new Point(10, 30),
                Size = new Size(150, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbRecommendedDevice = new ComboBox
            {
                Location = new Point(170, 28),
                Size = new Size(grpHearingAids.Width - 200, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            grpHearingAids.Controls.Add(lblRecommendedDevice);
            grpHearingAids.Controls.Add(cmbRecommendedDevice);

            // Complete appointment button
            btnCompleteAppointment = new Button
            {
                Text = "Complete Appointment",
                Location = new Point(panel.Width - 200, panel.Height - 50),
                Size = new Size(180, 40),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCompleteAppointment.Click += BtnCompleteAppointment_Click;
            ApplyButtonStyle(btnCompleteAppointment, Color.FromArgb(40, 167, 69));

            panel.Controls.Add(btnCompleteAppointment);
            panel.Controls.Add(grpHearingAids);
            panel.Controls.Add(chkNeedsPrescription);
            panel.Controls.Add(lblSectionTitle);

            return panel;
        }

        private void ApplyButtonStyle(Button button, Color baseColor)
        {
            button.ForeColor = Color.White;
            button.BackColor = baseColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = ControlPaint.Dark(baseColor);
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            button.MouseEnter += (s, e) =>
            {
                button.BackColor = ControlPaint.Dark(baseColor);
            };
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = baseColor;
            };
        }
        #endregion

        #region Event Handlers
        private void DtpAppointmentDate_ValueChanged(object sender, EventArgs e)
        {
            LoadAppointmentsForDate(dtpAppointmentDate.Value.Date);
        }

        private void BtnRefreshAppointments_Click(object sender, EventArgs e)
        {
            LoadAppointmentsForDate(dtpAppointmentDate.Value.Date);
        }

        private void CmbAppointments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAppointments.SelectedItem == null || !cmbAppointments.Enabled)
                return;

            var selectedItem = cmbAppointments.SelectedItem;

            // Check if it's our special "no appointments" item
            if (selectedItem.GetType().GetProperty("IsEmpty") != null)
                return;

            dynamic appointmentItem = selectedItem;
            var appointmentInfo = appointmentItem.AppointmentInfo;

            selectedAppointmentId = appointmentInfo.AppointmentId;
            selectedPatientId = appointmentInfo.PatientId;
            selectedMedicalRecordId = appointmentInfo.MedicalRecordId;

            // Load patient info
            LoadPatientInfo(selectedPatientId.Value);

            // Load appointment info
            LoadAppointmentInfo(selectedAppointmentId.Value);

            // Load diagnosis info
            LoadDiagnosisInfo(selectedMedicalRecordId.Value);

            // Reset prescription fields
            ResetPrescriptionFields();
        }

        private void BtnCompleteAppointment_Click(object sender, EventArgs e)
        {
            if (!selectedAppointmentId.HasValue)
            {
                //UIService.ShowWarning("Please select an appointment first.");
                return;
            }

            try
            {
                // Get the appointment
                var appointment = StaticDataProvider.Appointments
                    .FirstOrDefault(a => a.AppointmentID == selectedAppointmentId.Value);

                if (appointment == null)
                {
                    UIService.ShowError("Could not find the selected appointment.");
                    return;
                }

                // Mark the appointment as completed
                appointment.Status = "Completed";

                // If a prescription is needed, create one
                if (chkNeedsPrescription.Checked)
                {
                    // Create a new prescription
                    int newPrescriptionId = StaticDataProvider.Prescriptions.Count > 0 ?
                        StaticDataProvider.Prescriptions.Max(p => p.PrescriptionID) + 1 : 1;

                    // Get the recommended device
                    Product recommendedProduct = null;
                    if (cmbRecommendedDevice.SelectedItem != null)
                    {
                        dynamic selectedDevice = cmbRecommendedDevice.SelectedItem;
                        recommendedProduct = selectedDevice.Product;
                    }

                    var prescription = new Prescription
                    {
                        PrescriptionID = newPrescriptionId,
                        AppointmentID = selectedAppointmentId.Value,
                        PrescribedBy = GetCurrentAudiologistId(),
                        PrescribedDate = DateTime.Now,
                        ProductID = recommendedProduct?.ProductID
                    };

                    // Save to data store
                    StaticDataProvider.Prescriptions.Add(prescription);
                }

                UIService.ShowSuccess("Appointment completed successfully!");

                // Refresh the appointments list
                LoadAppointmentsForDate(dtpAppointmentDate.Value.Date);

                // Reset prescription fields
                ResetPrescriptionFields();
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error completing appointment: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods
        private void LoadAppointmentsForDate(DateTime date)
        {
            // Clear and reset the appointments dropdown
            cmbAppointments.DataSource = null;
            cmbAppointments.Items.Clear();

            // Get current audiologist
            var audiologistId = GetCurrentAudiologistId();
            if (audiologistId == 0)
            {
                UIService.ShowError("Could not identify the current audiologist.");
                return;
            }

            // Find confirmed appointments with medical records for this date
            var appointmentsWithRecords = (from appointment in StaticDataProvider.Appointments
                                           join record in StaticDataProvider.MedicalRecords
                                           on appointment.AppointmentID equals record.AppointmentID
                                           join test in StaticDataProvider.HearingTests
                                           on record.RecordID equals test.RecordID
                                           where appointment.AudiologistID == audiologistId
                                           && appointment.Date.Date == date.Date
                                           && appointment.Status == "Confirmed" // Only confirmed appointments
                                           select new
                                           {
                                               Appointment = appointment,
                                               Record = record
                                           }).Distinct().ToList(); // Ensure we don't duplicate appointments

            if (appointmentsWithRecords.Count == 0)
            {
                // Add a dummy item to indicate no appointments
                var noAppointmentsItem = new
                {
                    DisplayName = "No eligible appointments for this date",
                    IsEmpty = true
                };
                cmbAppointments.Items.Add(noAppointmentsItem);
                cmbAppointments.DisplayMember = "DisplayName";
                cmbAppointments.Enabled = false;
                cmbAppointments.SelectedIndex = 0;

                // Reset fields
                selectedAppointmentId = null;
                selectedPatientId = null;
                selectedMedicalRecordId = null;
                ResetPatientFields();
                ResetPrescriptionFields();

                return;
            }

            // Create appointment items with formatted display names
            var appointmentItems = appointmentsWithRecords.Select(item =>
            {
                // Find patient
                var patient = StaticDataProvider.Patients.FirstOrDefault(p => p.PatientID == item.Appointment.PatientID);
                string patientName = patient != null && patient.User != null
                    ? $"{patient.User.FirstName} {patient.User.LastName}"
                    : "Unknown Patient";

                return new
                {
                    AppointmentInfo = new
                    {
                        AppointmentId = item.Appointment.AppointmentID,
                        PatientId = item.Appointment.PatientID,
                        MedicalRecordId = item.Record.RecordID
                    },
                    DisplayName = $"{item.Appointment.Date.ToShortTimeString()} - {patientName} - {item.Appointment.PurposeOfVisit}"
                };
            }).ToList();

            // Set up combobox
            cmbAppointments.DisplayMember = "DisplayName";
            cmbAppointments.ValueMember = "AppointmentInfo";
            cmbAppointments.DataSource = appointmentItems;
            cmbAppointments.Enabled = true;

            // If we have appointments, select the first one by default
            if (cmbAppointments.Items.Count > 0)
            {
                cmbAppointments.SelectedIndex = 0;
            }

            // Load hearing aid products for recommendations
            LoadHearingAidProducts();
        }

        private void LoadHearingAidProducts()
        {
            // Clear and reset the hearing aid products dropdown
            cmbRecommendedDevice.DataSource = null;
            cmbRecommendedDevice.Items.Clear();

            // Get all products (hearing aid models)
            products = StaticDataProvider.Products.ToList();

            if (products.Count == 0)
            {
                cmbRecommendedDevice.Enabled = false;
                return;
            }

            // Create list items for the dropdown
            var productItems = products.Select(product => new
            {
                Product = product,
                DisplayName = $"{product.Manufacturer} {product.Model} - ${product.Price:F2}"
            }).ToList();

            // Set up combobox
            cmbRecommendedDevice.DisplayMember = "DisplayName";
            cmbRecommendedDevice.ValueMember = "Product";
            cmbRecommendedDevice.DataSource = productItems;
            cmbRecommendedDevice.Enabled = true;
        }

        private void LoadPatientInfo(int patientId)
        {
            var patient = StaticDataProvider.Patients.FirstOrDefault(p => p.PatientID == patientId);
            if (patient != null && patient.User != null)
            {
                // Calculate age
                int age = DateTime.Now.Year - patient.DateOfBirth.Year;
                if (DateTime.Now.DayOfYear < patient.DateOfBirth.DayOfYear)
                    age--;

                // Format patient info
                lblPatientInfo.Text = $"Patient: {patient.User.FirstName} {patient.User.LastName} | Age: {age} | ID: {patientId}";
            }
            else
            {
                lblPatientInfo.Text = "Patient information not available";
            }
        }

        private void LoadAppointmentInfo(int appointmentId)
        {
            var appointment = StaticDataProvider.Appointments.FirstOrDefault(a => a.AppointmentID == appointmentId);
            if (appointment != null)
            {
                lblAppointmentInfo.Text = $"Appointment: {appointment.Date.ToShortDateString()} at {appointment.Date.ToShortTimeString()} - {appointment.PurposeOfVisit}";
            }
            else
            {
                lblAppointmentInfo.Text = "Appointment information not available";
            }
        }

        private void LoadDiagnosisInfo(int medicalRecordId)
        {
            var record = StaticDataProvider.MedicalRecords
                .FirstOrDefault(r => r.RecordID == medicalRecordId);

            if (record != null && !string.IsNullOrEmpty(record.Diagnosis))
            {
                lblDiagnosis.Text = record.Diagnosis;
            }
            else
            {
                lblDiagnosis.Text = "No diagnosis information available";
            }
        }

        private int GetCurrentAudiologistId()
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser == null)
                return 0;

            var audiologist = StaticDataProvider.Audiologists
                .FirstOrDefault(a => a.UserID == currentUser.UserID);

            return audiologist?.AudiologistID ?? 0;
        }

        private void ResetPatientFields()
        {
            lblPatientInfo.Text = "No patient selected";
            lblAppointmentInfo.Text = "No appointment selected";
            lblDiagnosis.Text = "No diagnosis information available";
        }

        private void ResetPrescriptionFields()
        {
            chkNeedsPrescription.Checked = false;
            grpHearingAids.Enabled = false;
            if (cmbRecommendedDevice.Items.Count > 0)
                cmbRecommendedDevice.SelectedIndex = 0;
        }
        #endregion
    }
}