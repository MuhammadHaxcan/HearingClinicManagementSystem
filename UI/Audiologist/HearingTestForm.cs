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

namespace HearingClinicManagementSystem.UI.Audiologist {
    public class HearingTestForm : BaseForm {
        #region Fields
        private DateTimePicker dtpAppointmentDate;
        private ComboBox cmbAppointments;
        private Button btnRefreshAppointments;
        private ComboBox cmbPatients;
        private ComboBox cmbTestType;
        private ComboBox cmbEar;
        private GroupBox grpTestParameters;
        private GroupBox grpNotes;
        private NumericUpDown nudFrequency;
        private NumericUpDown nudThreshold;
        private Button btnAddDataPoint;
        private Button btnClearData;
        private Button btnSaveTest;
        private RichTextBox rtbNotes;
        private Panel patientInfoPanel;
        private Panel appointmentSelectionPanel;
        private Label lblPatientInfo;
        private Label lblAppointmentInfo;
        private DataGridView dgvTestData;
        private DataGridView dgvTestHistory;
        private int? selectedAppointmentId;
        private int? selectedPatientId;
        private readonly HearingClinicRepository _repository;
        #endregion

        public HearingTestForm(int? appointmentId = null, int? patientId = null) {
            selectedAppointmentId = appointmentId;
            selectedPatientId = patientId;
            _repository = HearingClinicRepository.Instance;
            InitializeComponents();
            LoadAppointmentsForDate(DateTime.Today);
            InitializeTestDataGrid();

            // If we have a patientId or appointmentId, try to select the appropriate appointment
            if (patientId.HasValue || appointmentId.HasValue) {
                SelectAppointmentForPatientOrAppointmentId(patientId, appointmentId);
            }
        }

        #region UI Setup
        private void InitializeComponents() {
            this.Text = "Hearing Test";
            this.Size = new Size(1200, 800);

            // Create title
            var lblTitle = CreateTitleLabel("Hearing Test");
            lblTitle.Dock = DockStyle.Top;

            // Main layout with 2 columns
            TableLayoutPanel mainPanel = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(5),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };

            // Configure column and row styles
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));  // Left column (45%)
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));  // Right column (55%)
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));        // Top row
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));        // Bottom row - chief complaints

            // Create the panels for the layout
            Panel patientPanel = CreatePatientPanel();           // Left column (patient info & history)
            Panel testInputsPanel = CreateTestInputsPanel();     // Right column, top row (test inputs)
            Panel chiefComplaintsPanel = CreateChiefComplaintsPanel(); // Bottom row (complaints)

            // Add panels to the main layout
            mainPanel.Controls.Add(patientPanel, 0, 0);
            mainPanel.Controls.Add(testInputsPanel, 1, 0);
            mainPanel.Controls.Add(chiefComplaintsPanel, 0, 1);
            mainPanel.SetColumnSpan(chiefComplaintsPanel, 2);      // Chief complaints span both columns

            // Add all controls to the form
            Controls.Add(mainPanel);
            Controls.Add(lblTitle);
        }

        private Panel CreatePatientPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Section title
            Label lblSectionTitle = new Label {
                Text = "Patient Information",
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30
            };

            // Create appointment selection panel with ZERO padding - fix white box issue
            appointmentSelectionPanel = new Panel {
                Location = new Point(10, 40),
                Width = panel.Width - 20,
                Height = 120,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(0) // Keep zero padding to prevent white boxes
            };

            // Place controls with explicit positions instead of using helper methods
            Label lblAppointmentSelectionTitle = new Label {
                Text = "Today's Appointments",
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // Date picker for appointment selection with direct creation
            Label lblDate = new Label {
                Text = "Select Date:",
                Location = new Point(10, 40),
                AutoSize = true
            };

            dtpAppointmentDate = new DateTimePicker {
                Location = new Point(120, 38),
                Size = new Size(150, 23),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            dtpAppointmentDate.ValueChanged += DtpAppointmentDate_ValueChanged;

            btnRefreshAppointments = new Button {
                Text = "Refresh",
                Size = new Size(90, 25),
                Location = new Point(appointmentSelectionPanel.Width - 110, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefreshAppointments.Click += BtnRefreshAppointments_Click;
            ApplyButtonStyle(btnRefreshAppointments, Color.FromArgb(108, 117, 125));

            // Appointments dropdown with direct creation
            Label lblSelectAppointment = new Label {
                Text = "Select Appointment:",
                Location = new Point(10, 75),
                AutoSize = true
            };

            cmbAppointments = new ComboBox {
                Location = new Point(140, 73),
                Size = new Size(appointmentSelectionPanel.Width - 160, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbAppointments.SelectedIndexChanged += CmbAppointments_SelectedIndexChanged;

            // Add controls to appointment selection panel
            appointmentSelectionPanel.Controls.Add(lblAppointmentSelectionTitle);
            appointmentSelectionPanel.Controls.Add(lblDate);
            appointmentSelectionPanel.Controls.Add(dtpAppointmentDate);
            appointmentSelectionPanel.Controls.Add(btnRefreshAppointments);
            appointmentSelectionPanel.Controls.Add(lblSelectAppointment);
            appointmentSelectionPanel.Controls.Add(cmbAppointments);

            // Patient info panel with zero padding - fix white box issue
            patientInfoPanel = new Panel {
                Location = new Point(10, 170),
                Width = panel.Width - 20,
                Height = 50,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(0) // Zero padding to prevent white boxes
            };

            // Create labels directly instead of using helper method
            lblPatientInfo = new Label {
                Text = "No patient selected",
                Location = new Point(5, 5),
                Size = new Size(patientInfoPanel.Width - 10, 20),
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Regular)
            };

            lblAppointmentInfo = new Label {
                Text = "",
                Location = new Point(5, 25),
                Size = new Size(patientInfoPanel.Width - 10, 20),
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Regular)
            };

            patientInfoPanel.Controls.Add(lblPatientInfo);
            patientInfoPanel.Controls.Add(lblAppointmentInfo);

            // Test data grid section
            Label lblCurrentData = new Label {
                Text = "Current Test Data",
                Location = new Point(10, 225),
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold),
                AutoSize = true
            };

            // Create data grid directly
            dgvTestData = new DataGridView {
                Location = new Point(10, 245),
                Size = new Size(panel.Width - 20, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ScrollBars = ScrollBars.Vertical
            };

            // Patient history section
            Label lblHistoryTitle = new Label {
                Text = "Patient Test History",
                Location = new Point(10, 400),
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold),
                AutoSize = true
            };

            Panel patientHistoryPanel = new Panel {
                Location = new Point(10, 420),
                Width = panel.Width - 20,
                Height = 70,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.LemonChiffon,
                Padding = new Padding(0) // Zero padding to prevent white boxes
            };

            // Create grid directly
            dgvTestHistory = new DataGridView {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Vertical
            };

            // Configure test history columns
            dgvTestHistory.Columns.Add("TestDate", "Test Date");
            dgvTestHistory.Columns.Add("TestType", "Test Type");
            dgvTestHistory.Columns.Add("Diagnosis", "Diagnosis");

            // Add empty data label
            Label lblNoHistory = new Label {
                Text = "No test history available for this patient",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font(Font.FontFamily, 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Visible = true,
                Tag = "NoHistoryMessage"
            };

            patientHistoryPanel.Controls.Add(lblNoHistory);
            patientHistoryPanel.Controls.Add(dgvTestHistory);

            // Add all controls to the panel
            panel.Controls.Add(patientHistoryPanel);
            panel.Controls.Add(lblHistoryTitle);
            panel.Controls.Add(dgvTestData);
            panel.Controls.Add(lblCurrentData);
            panel.Controls.Add(patientInfoPanel);
            panel.Controls.Add(appointmentSelectionPanel);
            panel.Controls.Add(lblSectionTitle);

            return panel;
        }

        private Panel CreateTestInputsPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Section title
            Label lblSectionTitle = CreateLabel("Test Parameters", 0, 0);
            lblSectionTitle.Font = new Font(lblSectionTitle.Font, FontStyle.Bold);
            lblSectionTitle.Dock = DockStyle.Top;
            lblSectionTitle.Height = 30;

            // Create test parameters area with subtle shading and border
            grpTestParameters = new GroupBox();
            grpTestParameters.Location = new Point(10, 40);
            grpTestParameters.Size = new Size(panel.Width - 20, 150);
            grpTestParameters.Text = "Test Parameters";
            grpTestParameters.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpTestParameters.BackColor = Color.FromArgb(252, 252, 252);

            // Create a table layout for more structured inputs
            TableLayoutPanel testParamsLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 5, 10, 5),
                ColumnCount = 4,
                RowCount = 3
            };

            testParamsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            testParamsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            testParamsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            testParamsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            for (int i = 0; i < 3; i++) {
                testParamsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            }

            // Test type
            Label lblTestType = new Label {
                Text = "Test Type:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbTestType = new ComboBox {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 10, 5)
            };
            cmbTestType.Items.AddRange(new object[] { "PureTone", "Speech", "Tympanometry" });
            cmbTestType.SelectedIndex = 0;

            // Ear selection
            Label lblEar = new Label {
                Text = "Ear:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbEar = new ComboBox {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 10, 5)
            };
            cmbEar.Items.AddRange(new object[] { "Right", "Left" });
            cmbEar.SelectedIndex = 0;

            // Frequency input (Hz)
            Label lblFrequency = new Label {
                Text = "Frequency (Hz):",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            nudFrequency = new NumericUpDown {
                Dock = DockStyle.Fill,
                Minimum = 125,
                Maximum = 8000,
                Increment = 125,
                Value = 1000,
                ThousandsSeparator = true,
                Margin = new Padding(0, 5, 10, 5)
            };

            // Threshold input (dB)
            Label lblThreshold = new Label {
                Text = "Threshold (dB):",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            nudThreshold = new NumericUpDown {
                Dock = DockStyle.Fill,
                Minimum = -10,
                Maximum = 120,
                Increment = 5,
                Value = 25,
                Margin = new Padding(0, 5, 10, 5)
            };

            // Add controls to the table layout
            testParamsLayout.Controls.Add(lblTestType, 0, 0);
            testParamsLayout.Controls.Add(cmbTestType, 1, 0);
            testParamsLayout.Controls.Add(lblEar, 2, 0);
            testParamsLayout.Controls.Add(cmbEar, 3, 0);
            testParamsLayout.Controls.Add(lblFrequency, 0, 1);
            testParamsLayout.Controls.Add(nudFrequency, 1, 1);
            testParamsLayout.Controls.Add(lblThreshold, 2, 1);
            testParamsLayout.Controls.Add(nudThreshold, 3, 1);

            grpTestParameters.Controls.Add(testParamsLayout);

            // Buttons for test data management
            btnAddDataPoint = CreateButton("Add Data Point", 10, 210, BtnAddDataPoint_Click, 140, 35);
            btnAddDataPoint.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ApplyButtonStyle(btnAddDataPoint, Color.FromArgb(0, 123, 255));

            btnClearData = CreateButton("Clear Data", 160, 210, BtnClearData_Click, 120, 35);
            btnClearData.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ApplyButtonStyle(btnClearData, Color.FromArgb(108, 117, 125));

            // Save button
            btnSaveTest = CreateButton("Save Hearing Test", panel.Width - 170, panel.Height - 45, BtnSaveTest_Click, 150, 40);
            btnSaveTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveTest.Font = new Font(btnSaveTest.Font, FontStyle.Bold);
            ApplyButtonStyle(btnSaveTest, Color.FromArgb(40, 167, 69));

            // Add all controls to panel
            panel.Controls.Add(btnSaveTest);
            panel.Controls.Add(btnClearData);
            panel.Controls.Add(btnAddDataPoint);
            panel.Controls.Add(grpTestParameters);
            panel.Controls.Add(lblSectionTitle);

            return panel;
        }

        private Panel CreateChiefComplaintsPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Chief Complaints section
            Label lblChiefComplaintsTitle = CreateLabel("Chief Complaints", 0, 0);
            lblChiefComplaintsTitle.Font = new Font(lblChiefComplaintsTitle.Font, FontStyle.Bold);
            lblChiefComplaintsTitle.Dock = DockStyle.Top;
            lblChiefComplaintsTitle.Height = 25;

            grpNotes = new GroupBox();
            grpNotes.Location = new Point(10, 30);
            grpNotes.Size = new Size(panel.Width - 20, panel.Height - 40);
            grpNotes.Text = "Patient's Chief Complaints";
            grpNotes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grpNotes.BackColor = Color.FromArgb(252, 252, 252);

            rtbNotes = new RichTextBox();
            rtbNotes.Location = new Point(10, 25);
            rtbNotes.Size = new Size(grpNotes.Width - 20, grpNotes.Height - 35);
            rtbNotes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            rtbNotes.BorderStyle = BorderStyle.FixedSingle;
            rtbNotes.Font = new Font("Segoe UI", 10);

            // Update placeholder text for chief complaints
            rtbNotes.Text = "Enter patient's chief complaints here...\n" +
                            "Examples:\n" +
                            "- Difficulty hearing in noisy environments\n" +
                            "- Ringing in ears";

            // Clear placeholder text on focus
            rtbNotes.GotFocus += (s, e) => {
                if (rtbNotes.Text.StartsWith("Enter patient's chief complaints")) {
                    rtbNotes.Text = "";
                }
            };

            // Add controls to notes group
            grpNotes.Controls.Add(rtbNotes);

            // Add all controls to panel
            panel.Controls.Add(grpNotes);
            panel.Controls.Add(lblChiefComplaintsTitle);

            return panel;
        }

        private void ApplyButtonStyle(Button button, Color baseColor) {
            button.ForeColor = Color.White;
            button.BackColor = baseColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = ControlPaint.Dark(baseColor);
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            button.MouseEnter += (s, e) => {
                button.BackColor = ControlPaint.Dark(baseColor);
            };
            button.MouseLeave += (s, e) => {
                button.BackColor = baseColor;
            };
        }

        private void InitializeTestDataGrid() {
            dgvTestData.Columns.Clear();
            dgvTestData.Columns.Add("Ear", "Ear");
            dgvTestData.Columns.Add("Frequency", "Frequency (Hz)");
            dgvTestData.Columns.Add("Threshold", "Threshold (dB)");
            dgvTestData.Columns.Add("Remove", "");

            // Set column properties
            dgvTestData.Columns["Ear"].Width = 80;
            dgvTestData.Columns["Frequency"].Width = 100;
            dgvTestData.Columns["Threshold"].Width = 100;
            dgvTestData.Columns["Remove"].Width = 60;

            // Add "Remove" button to each row
            DataGridViewButtonColumn removeButtonColumn = new DataGridViewButtonColumn();
            removeButtonColumn.HeaderText = "";
            removeButtonColumn.Text = "✖";
            removeButtonColumn.UseColumnTextForButtonValue = true;
            removeButtonColumn.Width = 40;
            dgvTestData.Columns.RemoveAt(3); // Remove placeholder column
            dgvTestData.Columns.Add(removeButtonColumn);

            // Handle button click
            dgvTestData.CellContentClick += (sender, e) => {
                if (e.RowIndex >= 0 && e.ColumnIndex == 3) // Remove button column
                {
                    dgvTestData.Rows.RemoveAt(e.RowIndex);

                    // Enable test type selection again if all data points are removed
                    if (dgvTestData.Rows.Count == 0) {
                        cmbTestType.Enabled = true;
                    }
                }
            };
        }
        #endregion

        #region Event Handlers
        private void DtpAppointmentDate_ValueChanged(object sender, EventArgs e) {
            LoadAppointmentsForDate(dtpAppointmentDate.Value.Date);
        }

        private void BtnRefreshAppointments_Click(object sender, EventArgs e) {
            LoadAppointmentsForDate(dtpAppointmentDate.Value.Date);
        }

        private void CmbAppointments_SelectedIndexChanged(object sender, EventArgs e) {
            if (cmbAppointments.SelectedItem != null && cmbAppointments.Enabled) {
                var selectedItem = cmbAppointments.SelectedItem;

                // Check if it's our special "no appointments" item
                if (selectedItem.GetType().GetProperty("IsEmpty") != null) {
                    return;
                }

                dynamic appointmentItem = selectedItem;
                var appointment = appointmentItem.Appointment;

                // Set the selected appointment and patient
                selectedAppointmentId = appointment.AppointmentID;
                selectedPatientId = appointment.PatientID;

                // Find and display the patient info
                var patient = _repository.GetPatientById(appointment.PatientID);
                if (patient != null) {
                    DisplayPatientInfo(patient);
                    LoadPatientTestHistory(patient.PatientID);

                    // Update appointment info
                    lblAppointmentInfo.Text = $"Appointment: {appointment.Date.ToShortDateString()} at {appointment.Date.ToShortTimeString()} - {appointment.PurposeOfVisit}";
                }
            }
        }

        private void CmbPatients_SelectedIndexChanged(object sender, EventArgs e) {
            if (cmbPatients.SelectedItem != null && cmbPatients.SelectedIndex >= 0) {
                // Clear appointment selection to avoid conflicts
                cmbAppointments.SelectedIndex = -1;
                selectedAppointmentId = null;

                dynamic selectedItem = cmbPatients.SelectedItem;
                Models.Patient patient = selectedItem.Patient;

                selectedPatientId = patient.PatientID;
                DisplayPatientInfo(patient);
                LoadPatientTestHistory(patient.PatientID);

                // Find most recent confirmed appointment for this patient
                var appointments = _repository.GetAppointmentsByPatientId(patient.PatientID)
                    .Where(a => a.Status == "Confirmed")
                    .OrderByDescending(a => a.Date)
                    .ToList();

                var appointment = appointments.FirstOrDefault();
                if (appointment != null) {
                    selectedAppointmentId = appointment.AppointmentID;
                    lblAppointmentInfo.Text = $"Last Confirmed Appointment: {appointment.Date.ToShortDateString()} - {appointment.PurposeOfVisit}";
                } else {
                    lblAppointmentInfo.Text = "No confirmed appointments found.";
                }
            }
        }

        private void BtnAddDataPoint_Click(object sender, EventArgs e) {
            // Validate patient selection
            if (selectedPatientId == null) {
                UIService.ShowWarning("Please select a patient first.");
                return;
            }

            string ear = cmbEar.SelectedItem.ToString();
            int frequency = (int)nudFrequency.Value;
            int threshold = (int)nudThreshold.Value;

            // Add to the data grid
            dgvTestData.Rows.Add(ear, frequency, threshold);

            // Lock test type once data is added
            cmbTestType.Enabled = false;
        }

        private void BtnClearData_Click(object sender, EventArgs e) {
            // Clear the data grid
            dgvTestData.Rows.Clear();

            // Unlock test type when all data is cleared
            cmbTestType.Enabled = true;
        }

        private void BtnSaveTest_Click(object sender, EventArgs e) {
            // Validate data
            if (selectedPatientId == null) {
                UIService.ShowWarning("Please select a patient or appointment first.");
                return;
            }

            if (dgvTestData.Rows.Count == 0) {
                UIService.ShowWarning("Please add at least one data point to save the test.");
                return;
            }

            if (rtbNotes.Text == "" || rtbNotes.Text.StartsWith("Enter patient's chief complaints")) {
                UIService.ShowWarning("Please enter the patient's chief complaints.");
                return;
            }

            try {
                // Get the current audiologist
                var currentUser = AuthService.CurrentUser;
                var audiologist = _repository.GetAudiologistByUserId(currentUser.UserID);

                if (audiologist == null) {
                    UIService.ShowError("Could not identify the current audiologist.");
                    return;
                }

                // Create audiogram data list from the grid
                var audiogramDataList = new List<AudiogramData>();

                foreach (DataGridViewRow row in dgvTestData.Rows) {
                    string ear = row.Cells["Ear"].Value.ToString();
                    int frequency = Convert.ToInt32(row.Cells["Frequency"].Value);
                    int threshold = Convert.ToInt32(row.Cells["Threshold"].Value);

                    audiogramDataList.Add(new AudiogramData {
                        Ear = ear,
                        Frequency = frequency,
                        Threshold = threshold
                    });
                }

                // Use repository method to create hearing test
                int testId = _repository.CreateHearingTest(
                    selectedPatientId.Value,
                    selectedAppointmentId ?? 0,
                    audiologist.AudiologistID,
                    cmbTestType.SelectedItem.ToString(),
                    rtbNotes.Text,
                    audiogramDataList
                );

                UIService.ShowSuccess("Hearing test saved successfully.");

                // Refresh the patient's test history
                LoadPatientTestHistory(selectedPatientId.Value);

                // Reset form data
                BtnClearData_Click(sender, e);
                rtbNotes.Text = "Enter patient's chief complaints here...";

                // Refresh the appointments list
                LoadAppointmentsForDate(dtpAppointmentDate.Value.Date);
            } catch (Exception ex) {
                UIService.ShowError($"Error saving hearing test: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods
        private void LoadAppointmentsForDate(DateTime date) {
            // Clear and reset the appointments dropdown
            cmbAppointments.DataSource = null;
            cmbAppointments.Items.Clear();

            // Get current audiologist
            var currentUser = AuthService.CurrentUser;
            var audiologist = _repository.GetAudiologistByUserId(currentUser.UserID);

            if (audiologist == null) {
                UIService.ShowError("Could not identify the current audiologist.");
                return;
            }

            // Get appointments using repository function
            var appointmentItems = _repository.GetAudiologistAppointmentsForDate(audiologist.AudiologistID, date);

            if (appointmentItems.Count == 0) {
                // Add a dummy item to indicate no appointments
                var noAppointmentsItem = new {
                    DisplayName = "No confirmed appointments for this date",
                    IsEmpty = true
                };
                cmbAppointments.Items.Add(noAppointmentsItem);
                cmbAppointments.DisplayMember = "DisplayName";
                cmbAppointments.Enabled = false;
                cmbAppointments.SelectedIndex = 0;
                return;
            }

            // Process appointment items
            var formattedAppointments = appointmentItems.Select(item => {
                dynamic appointment = item;
                return new {
                    Appointment = new {
                        AppointmentID = appointment.AppointmentId,
                        PatientID = appointment.PatientId,
                        Date = appointment.AppointmentTime,
                        PurposeOfVisit = appointment.Purpose
                    },
                    DisplayName = $"{((DateTime)appointment.AppointmentTime).ToShortTimeString()} - {appointment.PatientName} - {appointment.Purpose}"
                };
            }).ToList();

            // Set up combobox
            cmbAppointments.DisplayMember = "DisplayName";
            cmbAppointments.ValueMember = "Appointment";
            cmbAppointments.DataSource = formattedAppointments;
            cmbAppointments.Enabled = true;

            // If we have appointments, select the first one by default
            if (cmbAppointments.Items.Count > 0) {
                cmbAppointments.SelectedIndex = 0;
            }
        }

        private void SelectAppointmentForPatientOrAppointmentId(int? patientId, int? appointmentId) {
            // Implementation to select the appropriate appointment in the dropdown
            // based on patient ID or appointment ID
            if (cmbAppointments.Items.Count == 0 || !cmbAppointments.Enabled)
                return;

            int targetIndex = -1;

            for (int i = 0; i < cmbAppointments.Items.Count; i++) {
                dynamic item = cmbAppointments.Items[i];

                // Skip if it's our special "no appointments" item
                if (item.GetType().GetProperty("IsEmpty") != null)
                    continue;

                var appointment = item.Appointment;

                if (appointmentId.HasValue && appointment.AppointmentID == appointmentId.Value) {
                    targetIndex = i;
                    break;
                } else if (patientId.HasValue && appointment.PatientID == patientId.Value) {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex >= 0) {
                cmbAppointments.SelectedIndex = targetIndex;
            }
        }

        private void DisplayPatientInfo(Models.Patient patient) {
            if (patient == null || patient.User == null) {
                lblPatientInfo.Text = "No patient selected";
                return;
            }

            // Calculate age
            int age = DateTime.Now.Year - patient.DateOfBirth.Year;
            if (DateTime.Now.DayOfYear < patient.DateOfBirth.DayOfYear)
                age--;

            // Format patient info - simplified to just name and age
            lblPatientInfo.Text = $"Patient: {patient.User.FirstName} {patient.User.LastName}, Age: {age}";
        }

        private void LoadPatientTestHistory(int patientId) {
            // Clear existing data
            dgvTestHistory.Rows.Clear();

            // Use repository to get test history
            var testHistory = _repository.GetPatientHearingTestHistory(patientId);
            bool hasHistory = false;

            foreach (var test in testHistory) {
                dynamic testData = test;
                dgvTestHistory.Rows.Add(
                    ((DateTime)testData.TestDate).ToShortDateString(),
                    testData.TestType,
                    testData.Diagnosis
                );

                hasHistory = true;
            }

            // Show/hide the "no history" message
            var containingPanel = dgvTestHistory.Parent;
            foreach (Control ctrl in containingPanel.Controls) {
                if (ctrl is Label label && label.Tag?.ToString() == "NoHistoryMessage") {
                    label.Visible = !hasHistory;
                    break;
                }
            }

            // Set visibility of the test history grid
            dgvTestHistory.Visible = hasHistory;
        }
        #endregion
    }
}