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
using System.Windows.Forms.Design;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;

namespace HearingClinicManagementSystem.UI.Patient
{
    public class ViewMedicalHistoryForm : BaseForm
    {
        #region Fields
        private DataGridView dgvMedicalHistory;
        private DataGridView dgvTestDetails;
        private DataGridView dgvPrescriptions;
        private RichTextBox rtbDiagnosis;
        private RichTextBox rtbRecommendations;
        private HearingClinicRepository repository;
        #endregion

        public ViewMedicalHistoryForm()
        {
            repository = HearingClinicRepository.Instance;
            InitializeComponents();
            LoadMedicalHistory();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = AppStrings.Titles.MedicalHistory;

            // Form title
            var lblTitle = CreateTitleLabel(AppStrings.Titles.MedicalHistory);
            lblTitle.Dock = DockStyle.Top;

            // Main layout - Two rows
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10),
                RowStyles = {
                    new RowStyle(SizeType.Percent, 50F),
                    new RowStyle(SizeType.Percent, 50F)
                }
            };

            // Upper section - Medical History Records
            InitializeMedicalHistoryPanel(mainPanel);

            // Lower section - Details Panel (with diagnosis and tests)
            TableLayoutPanel detailsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 40F), // Reduced width for diagnosis
                    new ColumnStyle(SizeType.Percent, 60F)  // Increased width for test results
                }
            };

            InitializeDetailsPanel(detailsPanel);
            InitializeTestsPanel(detailsPanel);

            mainPanel.Controls.Add(detailsPanel, 0, 1);

            Controls.Add(mainPanel);
            Controls.Add(lblTitle);
        }

        private void InitializeMedicalHistoryPanel(TableLayoutPanel parent)
        {
            // === MEDICAL HISTORY LIST (Top) ===
            Panel pnlMedicalHistory = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Medical History Header
            var lblHistory = CreateLabel("Your Medical History Records", 0, 0);
            lblHistory.Dock = DockStyle.Top;
            lblHistory.Font = new Font(lblHistory.Font, FontStyle.Bold);
            lblHistory.Height = 25;
            lblHistory.TextAlign = ContentAlignment.MiddleLeft;

            // Medical History DataGrid
            dgvMedicalHistory = CreateDataGrid(0, false, true);
            dgvMedicalHistory.Dock = DockStyle.Fill;
            dgvMedicalHistory.MultiSelect = false;
            dgvMedicalHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMedicalHistory.Columns.Add("RecordID", "Record ID");
            dgvMedicalHistory.Columns.Add("Date", "Date");
            dgvMedicalHistory.Columns.Add("TestType", "Test Type");
            dgvMedicalHistory.Columns.Add("DoctorName", "Doctor Name");
            dgvMedicalHistory.Columns.Add("Diagnosis", "Brief Diagnosis");
            dgvMedicalHistory.Columns["RecordID"].Visible = false;
            dgvMedicalHistory.SelectionChanged += DgvMedicalHistory_SelectionChanged;

            pnlMedicalHistory.Controls.AddRange(new Control[] {
                lblHistory,
                dgvMedicalHistory
            });

            parent.Controls.Add(pnlMedicalHistory, 0, 0);
        }

        private void InitializeDetailsPanel(TableLayoutPanel parent)
        {
            // === DETAILS PANEL (Bottom-Left) ===
            Panel pnlDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Changed panel header to "Diagnosis & Treatment Plan"
            var lblDiagnosis = CreateLabel("Diagnosis & Treatment Plan", 0, 0);
            lblDiagnosis.Dock = DockStyle.Top;
            lblDiagnosis.Font = new Font(lblDiagnosis.Font, FontStyle.Bold);
            lblDiagnosis.Height = 25;
            lblDiagnosis.TextAlign = ContentAlignment.MiddleLeft;

            // Create panel for diagnosis and recommendations
            TableLayoutPanel diagnosisPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                RowStyles = {
                    new RowStyle(SizeType.Percent, 60F), // Diagnosis area
                    new RowStyle(SizeType.Percent, 40F)  // Recommendations area
                }
            };

            // Create diagnosis text box
            Panel pnlDiagnosisSection = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            var lblDiagnosisHeader = CreateLabel("Diagnosis:", 5, 5);
            lblDiagnosisHeader.Font = new Font(lblDiagnosisHeader.Font.FontFamily, lblDiagnosisHeader.Font.Size, FontStyle.Bold);
            lblDiagnosisHeader.Dock = DockStyle.Top;
            lblDiagnosisHeader.Height = 20;

            rtbDiagnosis = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = true
            };

            pnlDiagnosisSection.Controls.Add(rtbDiagnosis);
            pnlDiagnosisSection.Controls.Add(lblDiagnosisHeader);

            // Create recommendations section using RichTextBox for word wrapping
            Panel pnlRecommendations = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            var lblRecommendations = CreateLabel("Treatment Plan:", 5, 5);
            lblRecommendations.Font = new Font(lblRecommendations.Font.FontFamily, lblRecommendations.Font.Size, FontStyle.Bold);
            lblRecommendations.Dock = DockStyle.Top;
            lblRecommendations.Height = 20;

            // Replace ListBox with RichTextBox for word wrapping
            rtbRecommendations = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = true
            };

            pnlRecommendations.Controls.Add(rtbRecommendations);
            pnlRecommendations.Controls.Add(lblRecommendations);

            diagnosisPanel.Controls.Add(pnlDiagnosisSection, 0, 0);
            diagnosisPanel.Controls.Add(pnlRecommendations, 0, 1);

            pnlDetails.Controls.Add(diagnosisPanel);
            pnlDetails.Controls.Add(lblDiagnosis);

            parent.Controls.Add(pnlDetails, 0, 0);
        }

        private void InitializeTestsPanel(TableLayoutPanel parent)
        {
            // === TEST DETAILS PANEL (Bottom-Right) ===
            Panel pnlTestDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Renamed header to be more accurate
            var lblTestDetails = CreateLabel("Hearing Tests & Prescriptions", 0, 0);
            lblTestDetails.Dock = DockStyle.Top;
            lblTestDetails.Font = new Font(lblTestDetails.Font, FontStyle.Bold);
            lblTestDetails.Height = 25;
            lblTestDetails.TextAlign = ContentAlignment.MiddleLeft;

            TableLayoutPanel testsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                RowStyles = {
                    new RowStyle(SizeType.Percent, 50F), // Test results
                    new RowStyle(SizeType.Percent, 50F)  // Prescriptions
                }
            };

            // Test Results
            Panel pnlTestResults = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            var lblTestResultsHeader = CreateLabel("Tests Performed:", 5, 5);
            lblTestResultsHeader.Font = new Font(lblTestResultsHeader.Font.FontFamily, lblTestResultsHeader.Font.Size, FontStyle.Bold);
            lblTestResultsHeader.Dock = DockStyle.Top;
            lblTestResultsHeader.Height = 20;

            dgvTestDetails = CreateDataGrid(0, false, true);
            dgvTestDetails.Dock = DockStyle.Fill;
            dgvTestDetails.MultiSelect = false;

            // Removed Results column, keeping only TestType and Notes
            dgvTestDetails.Columns.Add("TestType", "Test Type");
            dgvTestDetails.Columns.Add("Notes", "Notes");

            pnlTestResults.Controls.Add(dgvTestDetails);
            pnlTestResults.Controls.Add(lblTestResultsHeader);

            // Prescriptions
            Panel pnlPrescriptions = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            var lblPrescriptionsHeader = CreateLabel("Prescribed Products:", 5, 5);
            lblPrescriptionsHeader.Font = new Font(lblPrescriptionsHeader.Font.FontFamily, lblPrescriptionsHeader.Font.Size, FontStyle.Bold);
            lblPrescriptionsHeader.Dock = DockStyle.Top;
            lblPrescriptionsHeader.Height = 20;

            dgvPrescriptions = CreateDataGrid(0, false, true);
            dgvPrescriptions.Dock = DockStyle.Fill;
            dgvPrescriptions.MultiSelect = false;

            dgvPrescriptions.Columns.Add("Product", "Product");
            dgvPrescriptions.Columns.Add("Description", "Features/Description");

            pnlPrescriptions.Controls.Add(dgvPrescriptions);
            pnlPrescriptions.Controls.Add(lblPrescriptionsHeader);

            testsPanel.Controls.Add(pnlTestResults, 0, 0);
            testsPanel.Controls.Add(pnlPrescriptions, 0, 1);

            pnlTestDetails.Controls.Add(testsPanel);
            pnlTestDetails.Controls.Add(lblTestDetails);

            parent.Controls.Add(pnlTestDetails, 1, 0);
        }
        #endregion

        #region Event Handlers
        private void DgvMedicalHistory_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMedicalHistory.SelectedRows.Count > 0)
            {
                int recordId = (int)dgvMedicalHistory.SelectedRows[0].Cells["RecordID"].Value;
                LoadRecordDetails(recordId);
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

            // Add a subtle border
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(40, 96, 144);

            // Add white inner border by setting padding
            button.Padding = new Padding(2);
            button.FlatAppearance.BorderColor = Color.White;

            // Improve text appearance
            button.Font = new Font(button.Font.FontFamily, button.Font.Size - 2, FontStyle.Regular);
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

        private void LoadMedicalHistory()
        {
            dgvMedicalHistory.Rows.Clear();

            // Make sure we have a current patient
            if (AuthService.CurrentPatient == null)
            {
                MessageBox.Show("No patient is currently logged in", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Get patient records from repository
                var patientRecords = repository.GetMedicalRecordsByPatientId(AuthService.CurrentPatient.PatientID);

                if (patientRecords.Count == 0)
                {
                    MessageBox.Show("No medical records found for this patient", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (var record in patientRecords)
                {
                    // Get tests for this record
                    var tests = repository.GetHearingTestsByRecordId(record.RecordID);
                    string testTypes = string.Join(", ", tests.Select(t => t.TestType));

                    // Get doctor name
                    string doctorName = "Unknown";
                    if (record.Appointment?.Audiologist?.User != null)
                    {
                        var audiologist = record.Appointment.Audiologist;
                        doctorName = $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}";
                    }

                    // Get brief diagnosis
                    string briefDiagnosis = record.Diagnosis?.Length > 50
                        ? record.Diagnosis.Substring(0, 50) + "..."
                        : record.Diagnosis ?? "No diagnosis provided";

                    dgvMedicalHistory.Rows.Add(
                        record.RecordID,
                        record.RecordDate.ToShortDateString(),
                        testTypes,
                        doctorName,
                        briefDiagnosis
                    );
                }

                // Select first row if available
                if (dgvMedicalHistory.Rows.Count > 0)
                {
                    dgvMedicalHistory.Rows[0].Selected = true;
                }
                else
                {
                    ClearDetailPanels();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading medical history: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearDetailPanels();
            }
        }

        private void LoadRecordDetails(int recordId)
        {
            try
            {
                var record = repository.GetMedicalRecordById(recordId);
                if (record == null)
                {
                    ClearDetailPanels();
                    return;
                }

                // Load Diagnosis
                rtbDiagnosis.Text = record.Diagnosis ?? "No diagnosis provided.";

                // Load Treatment Plan in RichTextBox with word wrap
                rtbRecommendations.Clear();
                if (!string.IsNullOrWhiteSpace(record.TreatmentPlan))
                {
                    rtbRecommendations.Text = record.TreatmentPlan;
                }
                else
                {
                    rtbRecommendations.Text = "No treatment plan provided.";
                }

                // Load Test Results
                LoadTestResults(recordId);

                // Load Prescriptions
                LoadPrescriptions(record.AppointmentID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading record details: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearDetailPanels();
            }
        }

        private void LoadTestResults(int recordId)
        {
            dgvTestDetails.Rows.Clear();

            try
            {
                var tests = repository.GetHearingTestsByRecordId(recordId);
                
                if (tests.Any())
                {
                    foreach (var test in tests)
                    {
                        // Only adding TestType and Notes columns
                        dgvTestDetails.Rows.Add(
                            test.TestType,
                            test.TestNotes ?? "No notes"
                        );
                    }
                }
                else
                {
                    dgvTestDetails.Rows.Add("No tests", "No test data available");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading test results: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvTestDetails.Rows.Add("Error", "Failed to load test data");
            }
        }

        private void LoadPrescriptions(int appointmentId)
        {
            dgvPrescriptions.Rows.Clear();

            try
            {
                var prescriptions = repository.GetPrescriptionsByAppointmentId(appointmentId);
                
                if (prescriptions.Any())
                {
                    foreach (var prescription in prescriptions)
                    {
                        var product = prescription.Product;
                        string productName = product != null ? $"{product.Manufacturer} {product.Model}" : "Unknown Product";

                        // Extract product features as description
                        string description = product?.Features ?? "No description";

                        dgvPrescriptions.Rows.Add(
                            productName,
                            description
                        );
                    }
                }
                else
                {
                    dgvPrescriptions.Rows.Add("No prescriptions", "No prescribed products for this visit");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading prescriptions: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvPrescriptions.Rows.Add("Error", "Failed to load prescription data");
            }
        }

        private void ClearDetailPanels()
        {
            rtbDiagnosis.Text = "No record selected";
            rtbRecommendations.Text = "No record selected";
            dgvTestDetails.Rows.Clear();
            dgvPrescriptions.Rows.Clear();
        }
        #endregion
    }
}
