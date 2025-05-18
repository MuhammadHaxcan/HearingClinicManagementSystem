using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Constants;

namespace HearingClinicManagementSystem.UI.Audiologist
{
    public class AudiogramViewForm : BaseForm
    {
        #region Fields
        private DateTimePicker dtpAppointmentDate;
        private ComboBox cmbAppointments;
        private Button btnRefreshAppointments;
        private Panel appointmentSelectionPanel;
        private Panel patientInfoPanel;
        private Label lblPatientInfo;
        private RichTextBox txtClinicalNotes;
        private Button btnSaveNotes;
        private Panel audiogramPanel;
        private Panel diagnosisPanel;
        private Label lblRightEarDiagnosis;
        private Label lblLeftEarDiagnosis;
        private int? selectedAppointmentId;
        private int? selectedPatientId;
        private int? selectedMedicalRecordId;
        private int? selectedTestId;
        private List<AudiogramData> audiogramData;

        private Panel leftAudiogramPanel;
        private Panel rightAudiogramPanel;
        private ComboBox cmbHearingTests; // For selecting different hearing tests
        private Label lblHearingTestSelection; // Label for the hearing test selection
        #endregion

        public AudiogramViewForm(int? appointmentId = null, int? patientId = null)
        {
            selectedAppointmentId = appointmentId;
            selectedPatientId = patientId;
            audiogramData = new List<AudiogramData>();

            InitializeComponents();
            LoadAppointmentsForDate(DateTime.Today);

            if (patientId.HasValue || appointmentId.HasValue)
            {
                SelectAppointmentForPatientOrAppointmentId(patientId, appointmentId);
            }
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Audiogram Viewer";
            this.Size = new Size(1200, 800);

            // Create title
            var lblTitle = CreateTitleLabel("Patient Audiogram");
            lblTitle.Dock = DockStyle.Top;

            // Main layout panel
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(5),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };

            // Configure column and row styles
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));  // Row for patient/appointment selection
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));    // Row for audiogram
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));    // Row for clinical notes

            // Create the panels
            Panel selectionPanel = CreateSelectionPanel();
            Panel leftAudiogramPanel = CreateAudiogramPanel(true);
            Panel rightAudiogramPanel = CreateAudiogramPanel(false);
            Panel clinicalNotesPanel = CreateClinicalNotesPanel();
            Panel diagnosisPanel = CreateDiagnosisPanel();

            // Add panels to the main layout
            mainPanel.Controls.Add(selectionPanel, 0, 0);
            mainPanel.SetColumnSpan(selectionPanel, 2);

            mainPanel.Controls.Add(leftAudiogramPanel, 0, 1);
            mainPanel.Controls.Add(rightAudiogramPanel, 1, 1);

            mainPanel.Controls.Add(clinicalNotesPanel, 0, 2);
            mainPanel.Controls.Add(diagnosisPanel, 1, 2);

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
            Label lblSectionTitle = CreateLabel("Patient & Appointment Selection", 0, 0);
            lblSectionTitle.Font = new Font(lblSectionTitle.Font, FontStyle.Bold);
            lblSectionTitle.Dock = DockStyle.Top;
            lblSectionTitle.Height = 25;

            // Create appointment selection panel with zero padding to fix the white box issue
            appointmentSelectionPanel = new Panel
            {
                Location = new Point(10, 35),
                Width = panel.Width - 20,
                Height = 100, // Height to accommodate the hearing test selection
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(0) // Zero padding to avoid white boxes
            };

            // Date picker for appointment selection - use direct Label creation
            Label lblDate = new Label
            {
                Text = "Select Date:",
                Location = new Point(10, 20),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Create DateTimePicker with explicit position and size
            dtpAppointmentDate = new DateTimePicker();
            dtpAppointmentDate.Location = new Point(100, 18);
            dtpAppointmentDate.Size = new Size(150, 23);
            dtpAppointmentDate.Format = DateTimePickerFormat.Short;
            dtpAppointmentDate.Value = DateTime.Today;
            dtpAppointmentDate.ValueChanged += DtpAppointmentDate_ValueChanged;

            // Create Refresh button with explicit position and size
            btnRefreshAppointments = new Button();
            btnRefreshAppointments.Text = "Refresh";
            btnRefreshAppointments.Size = new Size(90, 25);
            btnRefreshAppointments.Location = new Point(260, 17);
            btnRefreshAppointments.Click += BtnRefreshAppointments_Click;
            ApplyButtonStyle(btnRefreshAppointments, Color.FromArgb(108, 117, 125));

            // Appointments dropdown - use direct Label creation
            Label lblSelectAppointment = new Label
            {
                Text = "Select Appointment with Medical Records:",
                Location = new Point(370, 20),
                Size = new Size(220, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Create ComboBox with explicit position and size
            cmbAppointments = new ComboBox();
            cmbAppointments.Location = new Point(600, 18);
            cmbAppointments.Size = new Size(350, 23);
            cmbAppointments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbAppointments.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAppointments.SelectedIndexChanged += CmbAppointments_SelectedIndexChanged;

            // Add hearing test selection dropdown with explicit position and size
            lblHearingTestSelection = new Label
            {
                Text = "Select Hearing Test:",
                Location = new Point(370, 60),
                Size = new Size(150, 23),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false // Initially hidden
            };

            cmbHearingTests = new ComboBox();
            cmbHearingTests.Location = new Point(600, 58);
            cmbHearingTests.Size = new Size(350, 23);
            cmbHearingTests.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbHearingTests.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHearingTests.Visible = false; // Initially hidden
            cmbHearingTests.SelectedIndexChanged += CmbHearingTests_SelectedIndexChanged;

            // Add controls to appointment selection panel
            appointmentSelectionPanel.Controls.Add(lblDate);
            appointmentSelectionPanel.Controls.Add(dtpAppointmentDate);
            appointmentSelectionPanel.Controls.Add(btnRefreshAppointments);
            appointmentSelectionPanel.Controls.Add(lblSelectAppointment);
            appointmentSelectionPanel.Controls.Add(cmbAppointments);
            appointmentSelectionPanel.Controls.Add(lblHearingTestSelection);
            appointmentSelectionPanel.Controls.Add(cmbHearingTests);

            // Patient info panel with zero padding
            patientInfoPanel = new Panel
            {
                Location = new Point(10, 145), // Adjusted position
                Width = panel.Width - 20,
                Height = 30,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(0) // Zero padding to avoid white boxes
            };

            // Create patient info label with explicit position
            lblPatientInfo = new Label
            {
                Text = "No patient selected",
                Location = new Point(5, 5),
                Size = new Size(patientInfoPanel.Width - 10, 20),
                Font = new Font(DefaultFont.FontFamily, 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            patientInfoPanel.Controls.Add(lblPatientInfo);

            // Add panels to main selection panel
            panel.Controls.Add(patientInfoPanel);
            panel.Controls.Add(appointmentSelectionPanel);
            panel.Controls.Add(lblSectionTitle);

            return panel;
        }

        private Panel CreateAudiogramPanel(bool isLeftEar)
        {
            string title = isLeftEar ? "Left Ear Audiogram" : "Right Ear Audiogram";

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Audiogram title with explicit position
            Label lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create graph panel with explicit position and zero padding
            Panel graphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(0) // Zero padding to avoid white boxes
            };

            // Store the panel for later use when drawing the audiogram
            if (isLeftEar)
                this.leftAudiogramPanel = graphPanel;
            else
                this.rightAudiogramPanel = graphPanel;

            // We'll draw the audiogram in the Paint event of the panel
            graphPanel.Paint += (sender, e) =>
            {
                DrawAudiogramGrid(e.Graphics, graphPanel.Width, graphPanel.Height);

                if (audiogramData != null && audiogramData.Count > 0)
                {
                    DrawAudiogramData(e.Graphics, graphPanel.Width, graphPanel.Height, isLeftEar);
                }
            };

            panel.Controls.Add(graphPanel);
            panel.Controls.Add(lblTitle);

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

            // Diagnosis title with explicit position
            Label lblTitle = new Label
            {
                Text = "Diagnosis",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create diagnosis panel with zero padding
            diagnosisPanel = new Panel
            {
                Location = new Point(10, 35),
                Width = panel.Width - 20,
                Height = panel.Height - 45,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.FromArgb(252, 252, 252),
                Padding = new Padding(0) // Zero padding to avoid white boxes
            };

            // Create labels for diagnosis with explicit positions
            lblRightEarDiagnosis = new Label
            {
                Location = new Point(10, 20),
                Size = new Size(diagnosisPanel.Width - 20, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 123, 255),
                Text = "Right Ear: No data available"
            };

            lblLeftEarDiagnosis = new Label
            {
                Location = new Point(10, 60),
                Size = new Size(diagnosisPanel.Width - 20, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(220, 53, 69),
                Text = "Left Ear: No data available"
            };

            // Add explanation text with explicit position
            Label lblExplanation = new Label
            {
                Location = new Point(10, 110),
                Size = new Size(diagnosisPanel.Width - 20, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.FromArgb(73, 80, 87),
                Text = "Hearing Loss Classification:\n\n" +
                       "• < 20 dB: Normal hearing\n" +
                       "• 20-40 dB: Mild hearing loss\n" +
                       "• 40-60 dB: Moderate hearing loss\n" +
                       "• 60-80 dB: Severe hearing loss\n" +
                       "• > 80 dB: Profound hearing loss"
            };

            diagnosisPanel.Controls.Add(lblRightEarDiagnosis);
            diagnosisPanel.Controls.Add(lblLeftEarDiagnosis);
            diagnosisPanel.Controls.Add(lblExplanation);

            panel.Controls.Add(diagnosisPanel);
            panel.Controls.Add(lblTitle);

            return panel;
        }

        private Panel CreateClinicalNotesPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Test Notes title with explicit position
            Label lblTitle = new Label
            {
                Text = "Test Notes",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Create RichTextBox with explicit position and size
            txtClinicalNotes = new RichTextBox
            {
                Location = new Point(10, 35),
                Size = new Size(panel.Width - 20, panel.Height - 85),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };

            // Create Save Button with explicit position and size
            btnSaveNotes = new Button
            {
                Text = "Save Test Notes",
                Location = new Point(panel.Width - 170, panel.Height - 45),
                Size = new Size(150, 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnSaveNotes.Click += BtnSaveNotes_Click;
            ApplyButtonStyle(btnSaveNotes, Color.FromArgb(40, 167, 69));

            panel.Controls.Add(btnSaveNotes);
            panel.Controls.Add(txtClinicalNotes);
            panel.Controls.Add(lblTitle);

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

        #region Drawing Methods
        private void DrawAudiogramGrid(Graphics g, int width, int height)
        {
            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Drawing area margins
            int marginLeft = 60;
            int marginRight = 20;
            int marginTop = 30;
            int marginBottom = 50;

            int graphWidth = width - marginLeft - marginRight;
            int graphHeight = height - marginTop - marginBottom;

            // Define frequencies for X axis (Hz)
            int[] frequencies = new int[] { 125, 250, 500, 1000, 2000, 4000, 8000 };

            // Define hearing thresholds for Y axis (dB)
            int[] thresholds = new int[] { -10, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120 };

            // Calculate X positions for each frequency
            Dictionary<int, int> freqPositions = new Dictionary<int, int>();
            for (int i = 0; i < frequencies.Length; i++)
            {
                // Log scale for frequencies
                double relativePos = Math.Log10(frequencies[i] / 125.0) / Math.Log10(8000.0 / 125.0);
                int xPos = marginLeft + (int)(relativePos * graphWidth);
                freqPositions[frequencies[i]] = xPos;
            }

            // Calculate Y positions for each threshold
            Dictionary<int, int> thresholdPositions = new Dictionary<int, int>();
            for (int i = 0; i < thresholds.Length; i++)
            {
                int yPos = marginTop + (int)((thresholds[i] - thresholds[0]) * graphHeight / (thresholds[thresholds.Length - 1] - thresholds[0]));
                thresholdPositions[thresholds[i]] = yPos;
            }

            // Draw grid lines
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                // Vertical lines (frequency)
                foreach (int freq in frequencies)
                {
                    int x = freqPositions[freq];
                    g.DrawLine(gridPen, x, marginTop, x, height - marginBottom);
                }

                // Horizontal lines (threshold)
                foreach (int threshold in thresholds)
                {
                    int y = thresholdPositions[threshold];
                    g.DrawLine(gridPen, marginLeft, y, width - marginRight, y);
                }
            }

            // Draw axis labels
            using (Font labelFont = new Font("Arial", 8))
            using (Brush textBrush = new SolidBrush(Color.Black))
            {
                // X axis (frequency) labels
                foreach (int freq in frequencies)
                {
                    int x = freqPositions[freq];
                    string label = freq >= 1000 ? $"{freq / 1000}k" : freq.ToString();
                    SizeF textSize = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, textBrush, x - textSize.Width / 2, height - marginBottom + 5);
                }

                // Y axis (threshold) labels
                foreach (int threshold in thresholds)
                {
                    if (threshold % 20 == 0 || threshold == -10 || threshold == 110) // Only show some labels to avoid crowding
                    {
                        int y = thresholdPositions[threshold];
                        string label = threshold.ToString();
                        SizeF textSize = g.MeasureString(label, labelFont);
                        g.DrawString(label, labelFont, textBrush, marginLeft - textSize.Width - 5, y - textSize.Height / 2);
                    }
                }

                // X axis title
                string xTitle = "Frequency (Hz)";
                SizeF xTitleSize = g.MeasureString(xTitle, labelFont);
                g.DrawString(xTitle, labelFont, textBrush, marginLeft + graphWidth / 2 - xTitleSize.Width / 2, height - 20);

                // Y axis title
                string yTitle = "Threshold (dB)";
                SizeF yTitleSize = g.MeasureString(yTitle, labelFont);

                // Save the current transformation
                Matrix oldMatrix = g.Transform;

                // Create a rotation transformation
                g.TranslateTransform(20, marginTop + graphHeight / 2);
                g.RotateTransform(-90);
                g.DrawString(yTitle, labelFont, textBrush, -yTitleSize.Width / 2, -yTitleSize.Height / 2);

                // Restore the previous transformation
                g.Transform = oldMatrix;
            }
        }

        private void DrawAudiogramData(Graphics g, int width, int height, bool isLeftEar)
        {
            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Drawing area margins (should match grid drawing)
            int marginLeft = 60;
            int marginRight = 20;
            int marginTop = 30;
            int marginBottom = 50;

            int graphWidth = width - marginLeft - marginRight;
            int graphHeight = height - marginTop - marginBottom;

            // Define frequency range for visualization
            double minLogFreq = Math.Log10(125);
            double maxLogFreq = Math.Log10(8000);
            double logFreqRange = maxLogFreq - minLogFreq;

            // Define threshold range
            int minThreshold = -10;
            int maxThreshold = 120;
            int thresholdRange = maxThreshold - minThreshold;

            // Filter data for the selected ear
            var earData = audiogramData.Where(a =>
                (isLeftEar && a.Ear.ToLower() == "left") ||
                (!isLeftEar && a.Ear.ToLower() == "right"))
                .OrderBy(a => a.Frequency)
                .ToList();

            if (earData.Count == 0)
                return;

            // Prepare symbols and colors
            Brush symbolFill = new SolidBrush(isLeftEar ? Color.Red : Color.Blue);
            Pen symbolOutline = new Pen(Color.Black, 1);
            int symbolSize = 10;

            // First plot points
            foreach (var dataPoint in earData)
            {
                // Calculate position based on frequency and threshold
                double logFreq = Math.Log10(dataPoint.Frequency);
                double normalizedLogFreq = (logFreq - minLogFreq) / logFreqRange;
                int x = marginLeft + (int)(normalizedLogFreq * graphWidth);

                double normalizedThreshold = (double)(dataPoint.Threshold - minThreshold) / thresholdRange;
                int y = marginTop + (int)(normalizedThreshold * graphHeight);

                // Draw the appropriate symbol based on ear
                if (isLeftEar)
                    DrawSymbol(g, x, y, symbolSize, SymbolType.XSymbol, symbolFill, symbolOutline);
                else
                    DrawSymbol(g, x, y, symbolSize, SymbolType.Circle, symbolFill, symbolOutline);
            }

            // Now connect the points with lines
            if (earData.Count > 1)
            {
                Pen linePen = new Pen(isLeftEar ? Color.Red : Color.Blue, 2);
                var points = new List<Point>();

                foreach (var dataPoint in earData)
                {
                    double logFreq = Math.Log10(dataPoint.Frequency);
                    double normalizedLogFreq = (logFreq - minLogFreq) / logFreqRange;
                    int x = marginLeft + (int)(normalizedLogFreq * graphWidth);

                    double normalizedThreshold = (double)(dataPoint.Threshold - minThreshold) / thresholdRange;
                    int y = marginTop + (int)(normalizedThreshold * graphHeight);

                    points.Add(new Point(x, y));
                }

                g.DrawLines(linePen, points.ToArray());
            }
        }

        private enum SymbolType { Circle, XSymbol }

        private void DrawSymbol(Graphics g, int x, int y, int size, SymbolType type, Brush fill, Pen outline)
        {
            Rectangle rect = new Rectangle(x - size / 2, y - size / 2, size, size);

            switch (type)
            {
                case SymbolType.Circle:
                    g.FillEllipse(fill, rect);
                    g.DrawEllipse(outline, rect);
                    break;
                case SymbolType.XSymbol:
                    g.DrawLine(outline, x - size / 2, y - size / 2, x + size / 2, y + size / 2);
                    g.DrawLine(outline, x - size / 2, y + size / 2, x + size / 2, y - size / 2);
                    break;
            }
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
            var recordInfo = appointmentItem.RecordInfo;

            selectedAppointmentId = recordInfo.AppointmentId;
            selectedPatientId = recordInfo.PatientId;
            selectedMedicalRecordId = recordInfo.MedicalRecordId;

            // Load patient info
            LoadPatientInfo(selectedPatientId.Value);

            // Load hearing tests for this medical record
            LoadHearingTestsForMedicalRecord(selectedMedicalRecordId.Value);

            // Load diagnosis info
            LoadDiagnosisInfo(selectedMedicalRecordId.Value);
            
            // Load clinical notes
            LoadClinicalNotes(selectedMedicalRecordId.Value);
        }

        private void CmbHearingTests_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbHearingTests.SelectedItem == null)
                return;

            dynamic selectedTest = cmbHearingTests.SelectedItem;
            selectedTestId = selectedTest.TestId;

            // Load the audiogram data for the selected test
            LoadAudiogramData(selectedTestId.Value);
            
            // Load test notes for the selected hearing test
            LoadTestNotesForHearingTest(selectedTestId.Value);
            
            // Refresh both audiogram panels
            RefreshAudiogramPanels();
        }

        private void BtnSaveNotes_Click(object sender, EventArgs e)
        {
            if (!selectedTestId.HasValue)
            {
                //UIService.ShowWarning("Please select a hearing test first.");
                return;
            }

            try
            {
                // Find the hearing test
                var hearingTest = StaticDataProvider.HearingTests
                    .FirstOrDefault(t => t.TestID == selectedTestId.Value);

                if (hearingTest != null)
                {
                    // Update the test notes directly
                    hearingTest.TestNotes = txtClinicalNotes.Text;

                    UIService.ShowSuccess("Test notes saved successfully.");
                }
                else
                {
                    UIService.ShowError("Could not find the selected hearing test.");
                }
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error saving test notes: {ex.Message}");
            }
        }

        // Helper method to select the appropriate appointment
        private void SelectAppointmentForPatientOrAppointmentId(int? patientId, int? appointmentId)
        {
            if (cmbAppointments.Items.Count == 0 || !cmbAppointments.Enabled)
                return;

            for (int i = 0; i < cmbAppointments.Items.Count; i++)
            {
                dynamic item = cmbAppointments.Items[i];
                // Skip the "no appointments" item
                if (item.GetType().GetProperty("IsEmpty") != null)
                    continue;

                var recordInfo = item.RecordInfo;

                // Match by appointment ID
                if (appointmentId.HasValue && recordInfo.AppointmentId == appointmentId.Value)
                {
                    cmbAppointments.SelectedIndex = i;
                    return;
                }

                // Match by patient ID (if no appointment ID match was found)
                if (patientId.HasValue && recordInfo.PatientId == patientId.Value)
                {
                    cmbAppointments.SelectedIndex = i;
                    return;
                }
            }
        }
        #endregion

        #region Helper Methods
        private void LoadAppointmentsForDate(DateTime date)
        {
            // Clear and reset the appointments dropdown
            cmbAppointments.DataSource = null;
            cmbAppointments.Items.Clear();

            // Hide hearing test selection dropdown
            cmbHearingTests.Visible = false;
            lblHearingTestSelection.Visible = false;

            // Get current audiologist
            var currentUser = AuthService.CurrentUser;
            var audiologist = StaticDataProvider.Audiologists.FirstOrDefault(a => a.UserID == currentUser.UserID);

            if (audiologist == null)
            {
                UIService.ShowError("Could not identify the current audiologist.");
                return;
            }

            // Find appointments with associated medical records
            var appointmentsWithRecords = (from appointment in StaticDataProvider.Appointments
                                          join record in StaticDataProvider.MedicalRecords
                                          on appointment.AppointmentID equals record.AppointmentID
                                          where appointment.AudiologistID == audiologist.AudiologistID
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
                    DisplayName = "No appointments with medical records for this date",
                    IsEmpty = true
                };
                cmbAppointments.Items.Add(noAppointmentsItem);
                cmbAppointments.DisplayMember = "DisplayName";
                cmbAppointments.Enabled = false;
                cmbAppointments.SelectedIndex = 0;
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
                    RecordInfo = new
                    {
                        AppointmentId = item.Appointment.AppointmentID,
                        PatientId = item.Appointment.PatientID,
                        MedicalRecordId = item.Record.RecordID
                    },
                    DisplayName = $"{item.Appointment.Date.ToShortTimeString()} - {patientName} - Record #{item.Record.RecordID}"
                };
            }).ToList();

            // Set up combobox
            cmbAppointments.DisplayMember = "DisplayName";
            cmbAppointments.ValueMember = "RecordInfo";
            cmbAppointments.DataSource = appointmentItems;
            cmbAppointments.Enabled = true;

            // If we have appointments, select the first one by default
            if (cmbAppointments.Items.Count > 0)
            {
                cmbAppointments.SelectedIndex = 0;
            }
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


                lblPatientInfo.Text = $"Patient: {patient.User.FirstName} {patient.User.LastName} | Age: {age} ";
            }
            else
            {
                lblPatientInfo.Text = "Patient information not available";
            }
        }

        private void LoadAudiogramData(int testId)
        {
            // Clear any previous data
            audiogramData.Clear();

            // Load audiogram data for the selected test
            var data = StaticDataProvider.AudiogramData
                .Where(a => a.TestID == testId)
                .ToList();

            if (data.Any())
            {
                audiogramData.AddRange(data);
            }
        }

        private void LoadDiagnosisInfo(int medicalRecordId)
        {
            var record = StaticDataProvider.MedicalRecords
                .FirstOrDefault(r => r.RecordID == medicalRecordId);

            if (record != null && !string.IsNullOrEmpty(record.Diagnosis))
            {
                // Try to parse the diagnosis format: "Right ear: X. Left ear: Y."
                string diagnosis = record.Diagnosis;

                int rightEarIndex = diagnosis.IndexOf("Right ear:");
                int leftEarIndex = diagnosis.IndexOf("Left ear:");

                if (rightEarIndex >= 0 && leftEarIndex > rightEarIndex)
                {
                    string rightEarDiagnosis = diagnosis.Substring(rightEarIndex, leftEarIndex - rightEarIndex).Trim();
                    string leftEarDiagnosis = diagnosis.Substring(leftEarIndex).Trim();

                    lblRightEarDiagnosis.Text = rightEarDiagnosis;
                    lblLeftEarDiagnosis.Text = leftEarDiagnosis;
                }
                else
                {
                    lblRightEarDiagnosis.Text = "Right Ear: Diagnosis not available in expected format";
                    lblLeftEarDiagnosis.Text = "Left Ear: Diagnosis not available in expected format";
                }
            }
            else
            {
                lblRightEarDiagnosis.Text = "Right Ear: No diagnosis available";
                lblLeftEarDiagnosis.Text = "Left Ear: No diagnosis available";
            }
        }

        private void LoadClinicalNotes(int medicalRecordId)
        {
            // This method should only be called when first selecting an appointment
            // The actual notes will be loaded by LoadTestNotesForHearingTest when a hearing test is selected
            
            // Clear the text by default until a test is selected
            txtClinicalNotes.Text = "";
            
            // The notes will be loaded automatically when the hearing test is selected
            // via CmbHearingTests_SelectedIndexChanged
        }

        private void LoadTestNotesForHearingTest(int testId)
        {
            // Find the hearing test
            var hearingTest = StaticDataProvider.HearingTests.FirstOrDefault(t => t.TestID == testId);
            
            if (hearingTest != null)
            {
                // Load the test notes
                txtClinicalNotes.Text = hearingTest.TestNotes ?? "";
            }
            else
            {
                txtClinicalNotes.Text = "";
            }
        }

        private void LoadHearingTestsForMedicalRecord(int medicalRecordId)
        {
            // Clear the hearing tests dropdown
            cmbHearingTests.DataSource = null;
            cmbHearingTests.Items.Clear();

            // Get all hearing tests for this medical record
            var hearingTests = StaticDataProvider.HearingTests
                .Where(t => t.RecordID == medicalRecordId)
                .OrderByDescending(t => t.TestDate)
                .ToList();

            if (hearingTests.Count == 0)
            {
                cmbHearingTests.Visible = false;
                lblHearingTestSelection.Visible = false;
                
                // Clear the audiogram data since there's no test
                audiogramData.Clear();
                RefreshAudiogramPanels();
                return;
            }

            // Create test items for the dropdown
            var testItems = hearingTests.Select(test => new
            {
                TestId = test.TestID,
                DisplayName = $"{test.TestDate.ToShortDateString()} - {test.TestType}"
            }).ToList();

            // Setup the hearing tests dropdown
            cmbHearingTests.DisplayMember = "DisplayName";
            cmbHearingTests.ValueMember = "TestId";
            cmbHearingTests.DataSource = testItems;
            
            // Show the hearing tests dropdown
            cmbHearingTests.Visible = true;
            lblHearingTestSelection.Visible = true;

            // Select the first test by default
            if (cmbHearingTests.Items.Count > 0)
            {
                cmbHearingTests.SelectedIndex = 0;
                // The SelectedIndexChanged event will handle loading the data
            }
        }

        private void RefreshAudiogramPanels()
        {
            if (leftAudiogramPanel != null)
                leftAudiogramPanel.Invalidate();
            
            if (rightAudiogramPanel != null)
                rightAudiogramPanel.Invalidate();
        }
        #endregion
    }
}