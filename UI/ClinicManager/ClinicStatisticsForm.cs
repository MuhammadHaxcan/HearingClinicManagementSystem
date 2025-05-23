using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Constants;

namespace HearingClinicManagementSystem.UI.ClinicManager {
    public class ClinicStatisticsForm : BaseForm {
        #region Fields
        // Main layout controls
        private TabControl tabReports;
        private Button btnRefresh;
        private Label lblLastRefreshed;

        // Appointment Overview Controls
        private DataGridView dgvTopAudiologists;
        private DataGridView dgvRecentAppointments;
        private TableLayoutPanel pnlSummaryCards;

        // Financial Analysis Controls
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnApplyDateFilter;
        private ComboBox cmbAudiologist;
        private DataGridView dgvFinancialSummary;
        private DataGridView dgvPaymentMethods;
        private DataGridView dgvPurposeDistribution;
        private DataGridView dgvMonthlyRevenue;

        // Summary Stats Labels
        private Label lblTotalAppointments;
        private Label lblTotalRevenue;
        private Label lblAverageAppointmentValue;
        private Label lblMostActiveDoctor;
        private Label lblCompletionRate;
        private Label lblCancellationRate;

        // Date range for filtering
        private DateTime startDate;
        private DateTime endDate;

        private HearingClinicRepository repository;
        private List<dynamic> audiologistsList; // Store audiologists for reference
        #endregion

        public ClinicStatisticsForm() {
            repository = HearingClinicRepository.Instance;
            startDate = DateTime.Now.AddMonths(-3); // Default to last 3 months
            endDate = DateTime.Now;

            InitializeComponents();
            LoadAllData();
        }

        #region UI Setup
        private void InitializeComponents() {
            this.Text = "Clinic Statistics & Analytics";
            this.Size = new Size(1200, 800);

            // Create title
            var lblTitle = CreateTitleLabel("Clinic Statistics Dashboard");
            lblTitle.Dock = DockStyle.Top;

            // Create main layout panel with refresh button
            Panel topPanel = new Panel {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            btnRefresh = new Button {
                Text = "Refresh Data",
                Size = new Size(120, 30),
                Location = new Point(topPanel.Width - 140, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefresh.Click += BtnRefresh_Click;
            ApplyButtonStyle(btnRefresh, Color.FromArgb(0, 123, 255));

            lblLastRefreshed = new Label {
                Text = "Last refreshed: " + DateTime.Now.ToString("g"),
                AutoSize = true,
                Location = new Point(10, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(lblLastRefreshed);

            // Create tab control for different report views
            tabReports = new TabControl {
                Dock = DockStyle.Fill
            };

            // Create tabs
            TabPage tabAppointmentOverview = new TabPage("Appointment Overview");
            TabPage tabFinancialAnalysis = new TabPage("Financial Analysis");

            // Create appointment overview tab
            tabAppointmentOverview.Controls.Add(CreateAppointmentOverviewPanel());

            // Create financial analysis tab
            tabFinancialAnalysis.Controls.Add(CreateFinancialAnalysisPanel());

            // Add tabs to tab control
            tabReports.Controls.Add(tabAppointmentOverview);
            tabReports.Controls.Add(tabFinancialAnalysis);

            // Add all controls to form
            Controls.Add(tabReports);
            Controls.Add(topPanel);
            Controls.Add(lblTitle);
        }

        private Panel CreateAppointmentOverviewPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Create summary cards panel
            pnlSummaryCards = new TableLayoutPanel {
                Dock = DockStyle.Top,
                Height = 150,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(0, 0, 0, 10)
            };

            // Configure summary panel columns/rows
            for (int i = 0; i < 3; i++)
                pnlSummaryCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            for (int i = 0; i < 2; i++)
                pnlSummaryCards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Create summary card panels
            Panel pnlTotalAppointments = CreateSummaryPanel("Total Appointments", out lblTotalAppointments);
            Panel pnlTotalRevenue = CreateSummaryPanel("Total Revenue", out lblTotalRevenue);
            Panel pnlMostActiveDoctor = CreateSummaryPanel("Most Active Doctor", out lblMostActiveDoctor);
            Panel pnlAvgAppointmentValue = CreateSummaryPanel("Avg. Appointment Value", out lblAverageAppointmentValue);
            Panel pnlCompletionRate = CreateSummaryPanel("Completion Rate", out lblCompletionRate);
            Panel pnlCancellationRate = CreateSummaryPanel("Cancellation Rate", out lblCancellationRate);

            // Add panels to the layout
            pnlSummaryCards.Controls.Add(pnlTotalAppointments, 0, 0);
            pnlSummaryCards.Controls.Add(pnlTotalRevenue, 1, 0);
            pnlSummaryCards.Controls.Add(pnlMostActiveDoctor, 2, 0);
            pnlSummaryCards.Controls.Add(pnlAvgAppointmentValue, 0, 1);
            pnlSummaryCards.Controls.Add(pnlCompletionRate, 1, 1);
            pnlSummaryCards.Controls.Add(pnlCancellationRate, 2, 1);

            // Create main content layout
            TableLayoutPanel contentLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(0)
            };

            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Create panels for grids
            Panel pnlTopAudiologists = CreatePanelWithTitle("Top Performing Audiologists", out Panel topAudiologistsContainer);
            Panel pnlRecentAppointments = CreatePanelWithTitle("Recent Appointments", out Panel recentAppointmentsContainer);

            // Create top audiologists grid
            dgvTopAudiologists = new DataGridView {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigureDataGridView(dgvTopAudiologists);
            topAudiologistsContainer.Controls.Add(dgvTopAudiologists);

            // Create recent appointments grid
            dgvRecentAppointments = new DataGridView {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigureDataGridView(dgvRecentAppointments);
            recentAppointmentsContainer.Controls.Add(dgvRecentAppointments);

            // Add grids to layout
            contentLayout.Controls.Add(pnlTopAudiologists, 0, 0);
            contentLayout.Controls.Add(pnlRecentAppointments, 0, 1);

            // Add everything to the main panel
            panel.Controls.Add(contentLayout);
            panel.Controls.Add(pnlSummaryCards);

            return panel;
        }

        private Panel CreateFinancialAnalysisPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Create filter panel
            Panel filterPanel = new Panel {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Date range filter
            Label lblDateRange = new Label {
                Text = "Date Range:",
                Location = new Point(10, 15),
                AutoSize = true
            };

            dtpStartDate = new DateTimePicker {
                Format = DateTimePickerFormat.Short,
                Location = new Point(90, 13),
                Size = new Size(100, 20),
                Value = startDate
            };

            Label lblTo = new Label {
                Text = "to",
                Location = new Point(195, 15),
                AutoSize = true
            };

            dtpEndDate = new DateTimePicker {
                Format = DateTimePickerFormat.Short,
                Location = new Point(215, 13),
                Size = new Size(100, 20),
                Value = endDate
            };

            // Audiologist filter
            Label lblAudiologist = new Label {
                Text = "Audiologist:",
                Location = new Point(350, 15),
                AutoSize = true
            };

            cmbAudiologist = new ComboBox {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(440, 13),
                Size = new Size(150, 20)
            };

            btnApplyDateFilter = new Button {
                Text = "Apply Filter",
                Location = new Point(600, 10),
                Size = new Size(100, 30)
            };
            btnApplyDateFilter.Click += BtnApplyDateFilter_Click;
            ApplyButtonStyle(btnApplyDateFilter, Color.FromArgb(0, 123, 255));

            filterPanel.Controls.Add(lblDateRange);
            filterPanel.Controls.Add(dtpStartDate);
            filterPanel.Controls.Add(lblTo);
            filterPanel.Controls.Add(dtpEndDate);
            filterPanel.Controls.Add(lblAudiologist);
            filterPanel.Controls.Add(cmbAudiologist);
            filterPanel.Controls.Add(btnApplyDateFilter);

            // Create main content layout
            TableLayoutPanel contentLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2
            };

            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Create panels for grids
            Panel pnlFinancialSummary = CreatePanelWithTitle("Financial Summary by Visit Purpose", out Panel financialContainer);
            Panel pnlPaymentMethods = CreatePanelWithTitle("Payment Methods Used", out Panel paymentContainer);
            Panel pnlMonthlyRevenue = CreatePanelWithTitle("Monthly Revenue", out Panel revenueContainer);
            Panel pnlPurposeDistribution = CreatePanelWithTitle("Visit Purpose Distribution", out Panel purposeContainer);

            // Create financial summary grid
            dgvFinancialSummary = new DataGridView {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigureDataGridView(dgvFinancialSummary);
            financialContainer.Controls.Add(dgvFinancialSummary);

            // Create payment methods grid
            dgvPaymentMethods = new DataGridView {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigureDataGridView(dgvPaymentMethods);
            paymentContainer.Controls.Add(dgvPaymentMethods);

            // Create monthly revenue grid
            dgvMonthlyRevenue = new DataGridView {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigureDataGridView(dgvMonthlyRevenue);
            revenueContainer.Controls.Add(dgvMonthlyRevenue);

            // Create purpose distribution grid
            dgvPurposeDistribution = new DataGridView {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigureDataGridView(dgvPurposeDistribution);
            purposeContainer.Controls.Add(dgvPurposeDistribution);

            // Add grids to layout
            contentLayout.Controls.Add(pnlMonthlyRevenue, 0, 0);
            contentLayout.Controls.Add(pnlPurposeDistribution, 1, 0);
            contentLayout.Controls.Add(pnlFinancialSummary, 0, 1);
            contentLayout.Controls.Add(pnlPaymentMethods, 1, 1);

            // Add everything to the main panel
            panel.Controls.Add(contentLayout);
            panel.Controls.Add(filterPanel);

            return panel;
        }

        private Panel CreateSummaryPanel(string title, out Label valueLabel) {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create title label
            Label lblTitle = new Label {
                Text = title,
                Font = new Font(DefaultFont.FontFamily, 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(panel.Width - 20, 20),
                BackColor = Color.FromArgb(248, 249, 250),
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(10, 5, 0, 0)
            };

            // Create value label
            valueLabel = new Label {
                Text = "0",
                Location = new Point(0, 35),
                Size = new Size(panel.Width, 30),
                Font = new Font(DefaultFont.FontFamily, 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            panel.Controls.Add(valueLabel);
            panel.Controls.Add(lblTitle);

            return panel;
        }

        private Panel CreatePanelWithTitle(string title, out Panel contentContainer) {
            Panel mainPanel = new Panel {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitle = new Label {
                Text = title,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font(DefaultFont.FontFamily, 10, FontStyle.Bold),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 0, 0)
            };

            contentContainer = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            mainPanel.Controls.Add(contentContainer);
            mainPanel.Controls.Add(lblTitle);

            return mainPanel;
        }

        private void ConfigureDataGridView(DataGridView dataGridView) {
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView.Font, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 30;
            dataGridView.RowTemplate.Height = 25;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView.CellFormatting += (sender, e) => {
                if (e.RowIndex % 2 == 0) {
                    e.CellStyle.BackColor = Color.White;
                } else {
                    e.CellStyle.BackColor = Color.FromArgb(250, 250, 250);
                }
            };
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
        #endregion

        #region Data Loading
        private void LoadAllData() {
            try {
                // Load audiologists for filtering first
                audiologistsList = repository.GetAudiologistsForFilter();
                LoadAudiologistFilter();

                LoadAppointmentOverviewData();
                LoadFinancialAnalysisData();

                lblLastRefreshed.Text = "Last refreshed: " + DateTime.Now.ToString("g");
            } catch (Exception ex) {
                UIService.ShowError($"Error loading report data: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
            }
        }

        private void LoadAudiologistFilter() {
            cmbAudiologist.Items.Clear();

            foreach (var audiologist in audiologistsList) {
                cmbAudiologist.Items.Add(audiologist.DisplayName);
            }

            if (cmbAudiologist.Items.Count > 0) {
                cmbAudiologist.SelectedIndex = 0;
            }
        }

        private void LoadAppointmentOverviewData() {
            LoadSummaryStats();
            LoadTopAudiologistsGrid();
            LoadRecentAppointmentsGrid();
        }

        private void LoadFinancialAnalysisData() {
            LoadFinancialSummaryGrid();
            LoadPaymentMethodsGrid();
            LoadMonthlyRevenueGrid();
            LoadPurposeDistributionGrid();
        }

        private void LoadSummaryStats() {
            try {
                // Get summary statistics from repository
                var stats = repository.GetAppointmentSummaryStats(startDate, endDate);

                if (stats == null) {
                    // Set default values if stats are null
                    lblTotalAppointments.Text = "0";
                    lblTotalRevenue.Text = "$0.00";
                    lblAverageAppointmentValue.Text = "$0.00";
                    lblMostActiveDoctor.Text = "No data available";
                    lblCompletionRate.Text = "0%";
                    lblCancellationRate.Text = "0%";
                    return;
                }

                // Safely extract values using try/catch blocks
                try { lblTotalAppointments.Text = stats.TotalAppointments.ToString(); } catch { lblTotalAppointments.Text = "0"; }

                try { lblTotalRevenue.Text = stats.TotalRevenue.ToString("C2"); } catch { lblTotalRevenue.Text = "$0.00"; }

                try { lblAverageAppointmentValue.Text = stats.AverageAppointmentValue.ToString("C2"); } catch { lblAverageAppointmentValue.Text = "$0.00"; }

                try {
                    if (stats.MostActiveDoctorAppointmentCount > 0) {
                        lblMostActiveDoctor.Text = $"{stats.MostActiveDoctor} ({stats.MostActiveDoctorAppointmentCount})";
                    } else {
                        lblMostActiveDoctor.Text = "No data available";
                    }
                } catch { lblMostActiveDoctor.Text = "Error"; }

                double completionRate = 0, cancellationRate = 0;
                try { completionRate = stats.CompletionRate; } catch { }
                try { cancellationRate = stats.CancellationRate; } catch { }

                lblCompletionRate.Text = completionRate.ToString("F1") + "%";
                lblCancellationRate.Text = cancellationRate.ToString("F1") + "%";

                // Color coding for rates
                if (completionRate >= 80)
                    lblCompletionRate.ForeColor = Color.FromArgb(40, 167, 69);
                else if (completionRate >= 60)
                    lblCompletionRate.ForeColor = Color.FromArgb(255, 193, 7);
                else
                    lblCompletionRate.ForeColor = Color.FromArgb(220, 53, 69);

                if (cancellationRate <= 10)
                    lblCancellationRate.ForeColor = Color.FromArgb(40, 167, 69);
                else if (cancellationRate <= 20)
                    lblCancellationRate.ForeColor = Color.FromArgb(255, 193, 7);
                else
                    lblCancellationRate.ForeColor = Color.FromArgb(220, 53, 69);
            } catch (Exception ex) {
                UIService.ShowError($"Error loading summary statistics: {ex.Message}");

                // Set default values on error
                lblTotalAppointments.Text = "Error";
                lblTotalRevenue.Text = "Error";
                lblAverageAppointmentValue.Text = "Error";
                lblMostActiveDoctor.Text = "Error loading data";
                lblCompletionRate.Text = "Error";
                lblCancellationRate.Text = "Error";
            }
        }

        private void LoadTopAudiologistsGrid() {
            try {
                // Configure grid columns
                dgvTopAudiologists.Columns.Clear();
                dgvTopAudiologists.Columns.Add("Rank", "Rank");
                dgvTopAudiologists.Columns.Add("AudiologistID", "ID");
                dgvTopAudiologists.Columns.Add("Name", "Name");
                dgvTopAudiologists.Columns.Add("TotalAppointments", "Total Appointments");
                dgvTopAudiologists.Columns.Add("CompletedAppointments", "Completed");
                dgvTopAudiologists.Columns.Add("CancelledAppointments", "Cancelled");
                dgvTopAudiologists.Columns.Add("TotalRevenue", "Total Revenue");
                dgvTopAudiologists.Columns.Add("AvgRevenue", "Avg Revenue/Appt");

                // Set column properties
                dgvTopAudiologists.Columns["Rank"].Width = 40;
                dgvTopAudiologists.Columns["AudiologistID"].Width = 40;
                dgvTopAudiologists.Columns["AudiologistID"].Visible = false;
                dgvTopAudiologists.Columns["Name"].FillWeight = 80;
                dgvTopAudiologists.Columns["TotalAppointments"].FillWeight = 40;
                dgvTopAudiologists.Columns["CompletedAppointments"].FillWeight = 40;
                dgvTopAudiologists.Columns["CancelledAppointments"].FillWeight = 40;
                dgvTopAudiologists.Columns["TotalRevenue"].FillWeight = 60;
                dgvTopAudiologists.Columns["AvgRevenue"].FillWeight = 60;

                // Get audiologist statistics from repository
                var audiologistStats = repository.GetTopAudiologistStats(startDate, endDate);

                if (audiologistStats == null || audiologistStats.Count == 0) {
                    dgvTopAudiologists.Rows.Add("N/A", "", "No audiologist data available", "0", "0", "0", "$0.00", "$0.00");
                    return;
                }

                // Add data rows
                for (int i = 0; i < audiologistStats.Count; i++) {
                    var stat = audiologistStats[i];

                    // Safely extract values
                    int audiologistId = 0;
                    string name = "Unknown";
                    int totalAppointments = 0;
                    int completedAppointments = 0;
                    int cancelledAppointments = 0;
                    decimal totalRevenue = 0m;
                    decimal averageRevenue = 0m;

                    try { audiologistId = stat.AudiologistID; } catch { }
                    try { name = stat.Name; } catch { }
                    try { totalAppointments = stat.TotalAppointments; } catch { }
                    try { completedAppointments = stat.CompletedAppointments; } catch { }
                    try { cancelledAppointments = stat.CancelledAppointments; } catch { }
                    try { totalRevenue = stat.TotalRevenue; } catch { }
                    try { averageRevenue = stat.AverageRevenue; } catch { }

                    dgvTopAudiologists.Rows.Add(
                        (i + 1).ToString(),
                        audiologistId.ToString(),
                        name,
                        totalAppointments.ToString("N0"),
                        completedAppointments.ToString("N0"),
                        cancelledAppointments.ToString("N0"),
                        totalRevenue.ToString("C2"),
                        averageRevenue.ToString("C2")
                    );

                    // Highlight top 3
                    if (i < 3) {
                        dgvTopAudiologists.Rows[i].DefaultCellStyle.Font = new Font(dgvTopAudiologists.Font, FontStyle.Bold);
                        if (i == 0)
                            dgvTopAudiologists.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                    }
                }
            } catch (Exception ex) {
                UIService.ShowError($"Error loading audiologist statistics: {ex.Message}");
                dgvTopAudiologists.Rows.Add("Error", "", "Error loading data", "", "", "", "", "");
            }
        }

        private void LoadRecentAppointmentsGrid() {
            try {
                // Configure grid columns
                dgvRecentAppointments.Columns.Clear();
                dgvRecentAppointments.Columns.Add("AppointmentID", "ID");
                dgvRecentAppointments.Columns.Add("Date", "Date & Time");
                dgvRecentAppointments.Columns.Add("Patient", "Patient");
                dgvRecentAppointments.Columns.Add("Audiologist", "Audiologist");
                dgvRecentAppointments.Columns.Add("Purpose", "Purpose");
                dgvRecentAppointments.Columns.Add("Fee", "Fee");
                dgvRecentAppointments.Columns.Add("Status", "Status");

                // Set column properties
                dgvRecentAppointments.Columns["AppointmentID"].Width = 40;
                dgvRecentAppointments.Columns["Date"].FillWeight = 60;
                dgvRecentAppointments.Columns["Patient"].FillWeight = 80;
                dgvRecentAppointments.Columns["Audiologist"].FillWeight = 80;
                dgvRecentAppointments.Columns["Purpose"].FillWeight = 60;
                dgvRecentAppointments.Columns["Fee"].FillWeight = 60;
                dgvRecentAppointments.Columns["Status"].FillWeight = 60;

                // Get recent appointments from repository
                var recentAppointments = repository.GetRecentAppointments(15);

                if (recentAppointments == null || recentAppointments.Count == 0) {
                    dgvRecentAppointments.Rows.Add("N/A", "-", "No appointment data available", "-", "-", "$0.00", "-");
                    return;
                }

                // Add data rows
                foreach (var appointment in recentAppointments) {
                    // Safely extract values
                    int appointmentId = 0;
                    DateTime date = DateTime.Now;
                    string patientName = "Unknown";
                    string audiologistName = "Unknown";
                    string purpose = "";
                    decimal fee = 0m;
                    string status = "";

                    try { appointmentId = appointment.AppointmentID; } catch { }
                    try { date = appointment.Date; } catch { }
                    try { patientName = appointment.PatientName; } catch { }
                    try { audiologistName = appointment.AudiologistName; } catch { }
                    try { purpose = appointment.Purpose; } catch { }
                    try { fee = appointment.Fee; } catch { }
                    try { status = appointment.Status; } catch { }

                    var row = dgvRecentAppointments.Rows.Add(
                        appointmentId.ToString(),
                        date.ToString("g"),
                        patientName,
                        audiologistName,
                        purpose,
                        fee.ToString("C2"),
                        status
                    );

                    // Color code based on status
                    if (status == "Completed") {
                        dgvRecentAppointments.Rows[row].Cells["Status"].Style.ForeColor = Color.Green;
                        dgvRecentAppointments.Rows[row].Cells["Status"].Style.Font = new Font(dgvRecentAppointments.Font, FontStyle.Bold);
                    } else if (status == "Confirmed") {
                        dgvRecentAppointments.Rows[row].Cells["Status"].Style.ForeColor = Color.Blue;
                    } else if (status == "Cancelled") {
                        dgvRecentAppointments.Rows[row].Cells["Status"].Style.ForeColor = Color.Red;
                    }
                }
            } catch (Exception ex) {
                UIService.ShowError($"Error loading recent appointments: {ex.Message}");
                dgvRecentAppointments.Rows.Add("Error", "", "Error loading appointment data", "", "", "", "");
            }
        }

        private void LoadFinancialSummaryGrid() {
            try {
                // Configure grid columns
                dgvFinancialSummary.Columns.Clear();
                dgvFinancialSummary.Columns.Add("Purpose", "Visit Purpose");
                dgvFinancialSummary.Columns.Add("Count", "# of Appointments");
                dgvFinancialSummary.Columns.Add("TotalRevenue", "Total Revenue");
                dgvFinancialSummary.Columns.Add("AvgRevenue", "Avg. Revenue");
                dgvFinancialSummary.Columns.Add("PercentOfTotal", "% of Total");

                // Get selected audiologist
                int? selectedAudiologistId = GetSelectedAudiologistId();

                // Get financial summary from repository
                var financialSummary = repository.GetFinancialSummaryByPurpose(startDate, endDate, selectedAudiologistId);

                if (financialSummary == null || financialSummary.Count == 0) {
                    dgvFinancialSummary.Rows.Add("No data", "0", "$0.00", "$0.00", "0%");
                    return;
                }

                // Add data rows
                foreach (var summary in financialSummary) {
                    string purpose = "Unknown";
                    int count = 0;
                    decimal totalRevenue = 0m;
                    decimal averageRevenue = 0m;
                    decimal percentOfTotal = 0m;

                    try { purpose = summary.Purpose; } catch { }
                    try { count = summary.Count; } catch { }
                    try { totalRevenue = summary.TotalRevenue; } catch { }
                    try { averageRevenue = summary.AverageRevenue; } catch { }
                    try { percentOfTotal = summary.PercentOfTotal; } catch { }

                    int rowIndex = dgvFinancialSummary.Rows.Add(
                        purpose,
                        count.ToString("N0"),
                        totalRevenue.ToString("C2"),
                        averageRevenue.ToString("C2"),
                        percentOfTotal.ToString("F1") + "%"
                    );

                    // Highlight the total row
                    if (purpose == "TOTAL") {
                        dgvFinancialSummary.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                        dgvFinancialSummary.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgvFinancialSummary.Font, FontStyle.Bold);
                    }
                }
            } catch (Exception ex) {
                UIService.ShowError($"Error loading financial summary: {ex.Message}");
                dgvFinancialSummary.Rows.Add("Error", "", "", "", "");
            }
        }

        private void LoadPaymentMethodsGrid() {
            try {
                // Configure grid columns
                dgvPaymentMethods.Columns.Clear();
                dgvPaymentMethods.Columns.Add("PaymentMethod", "Payment Method");
                dgvPaymentMethods.Columns.Add("Count", "Count");
                dgvPaymentMethods.Columns.Add("TotalAmount", "Total Amount");
                dgvPaymentMethods.Columns.Add("PercentOfTotal", "% of Total");

                // Get selected audiologist
                int? selectedAudiologistId = GetSelectedAudiologistId();

                // Get payment method statistics from repository
                var paymentSummary = repository.GetPaymentMethodStats(startDate, endDate, selectedAudiologistId);

                if (paymentSummary == null || paymentSummary.Count == 0) {
                    dgvPaymentMethods.Rows.Add("No data", "0", "$0.00", "0%");
                    return;
                }

                // Add data rows
                foreach (var summary in paymentSummary) {
                    string paymentMethod = "Unknown";
                    int count = 0;
                    decimal totalAmount = 0m;
                    decimal percentOfTotal = 0m;

                    try { paymentMethod = summary.PaymentMethod; } catch { }
                    try { count = summary.Count; } catch { }
                    try { totalAmount = summary.TotalAmount; } catch { }
                    try { percentOfTotal = summary.PercentOfTotal; } catch { }

                    int rowIndex = dgvPaymentMethods.Rows.Add(
                        paymentMethod,
                        count.ToString("N0"),
                        totalAmount.ToString("C2"),
                        percentOfTotal.ToString("F1") + "%"
                    );

                    // Highlight the total row
                    if (paymentMethod == "TOTAL") {
                        dgvPaymentMethods.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                        dgvPaymentMethods.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgvPaymentMethods.Font, FontStyle.Bold);
                    }
                }
            } catch (Exception ex) {
                UIService.ShowError($"Error loading payment methods: {ex.Message}");
                dgvPaymentMethods.Rows.Add("Error", "", "", "");
            }
        }

        private void LoadMonthlyRevenueGrid() {
            try {
                // Configure grid columns
                dgvMonthlyRevenue.Columns.Clear();
                dgvMonthlyRevenue.Columns.Add("Month", "Month");
                dgvMonthlyRevenue.Columns.Add("Year", "Year");
                dgvMonthlyRevenue.Columns.Add("AppointmentCount", "Appointments");
                dgvMonthlyRevenue.Columns.Add("CompletedCount", "Completed");
                dgvMonthlyRevenue.Columns.Add("Revenue", "Total Revenue");
                dgvMonthlyRevenue.Columns.Add("AvgRevenue", "Avg. Revenue");
                dgvMonthlyRevenue.Columns.Add("CompletionRate", "Completion Rate");

                // Get selected audiologist
                int? selectedAudiologistId = GetSelectedAudiologistId();

                // Get monthly revenue statistics from repository
                var monthlyStats = repository.GetMonthlyRevenueStats(12, selectedAudiologistId);

                if (monthlyStats == null || monthlyStats.Count == 0) {
                    dgvMonthlyRevenue.Rows.Add("No data", "", "0", "0", "$0.00", "$0.00", "0%");
                    return;
                }

                // Add data rows
                foreach (var stat in monthlyStats) {
                    string month = "Unknown";
                    string year = "";
                    int appointmentCount = 0;
                    int completedCount = 0;
                    decimal revenue = 0m;
                    decimal averageRevenue = 0m;
                    double completionRate = 0d;

                    try { month = stat.Month; } catch { }
                    try { year = stat.Year.ToString(); } catch { }
                    try { appointmentCount = stat.AppointmentCount; } catch { }
                    try { completedCount = stat.CompletedCount; } catch { }
                    try { revenue = stat.Revenue; } catch { }
                    try { averageRevenue = stat.AverageRevenue; } catch { }
                    try { completionRate = stat.CompletionRate; } catch { }

                    int rowIndex = dgvMonthlyRevenue.Rows.Add(
                        month,
                        month == "TOTAL" ? "" : year,
                        appointmentCount.ToString("N0"),
                        completedCount.ToString("N0"),
                        revenue.ToString("C2"),
                        averageRevenue.ToString("C2"),
                        completionRate.ToString("F1") + "%"
                    );

                    // Highlight the total row
                    if (month == "TOTAL") {
                        dgvMonthlyRevenue.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                        dgvMonthlyRevenue.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgvMonthlyRevenue.Font, FontStyle.Bold);
                    }
                }

                // Apply cell formatting
                dgvMonthlyRevenue.CellFormatting += (sender, e) => {
                    if (e.ColumnIndex == dgvMonthlyRevenue.Columns["Revenue"].Index && e.Value != null) {
                        if (decimal.TryParse(e.Value.ToString().Replace("$", "").Replace(",", ""),
                                out decimal amount)) {
                            if (amount > 5000) {
                                e.CellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                                e.CellStyle.Font = new Font(dgvMonthlyRevenue.Font, FontStyle.Bold);
                            }
                        }
                    } else if (e.ColumnIndex == dgvMonthlyRevenue.Columns["CompletionRate"].Index && e.Value != null) {
                        if (double.TryParse(e.Value.ToString().Replace("%", ""), out double rate)) {
                            if (rate >= 90) {
                                e.CellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                            } else if (rate < 70) {
                                e.CellStyle.ForeColor = Color.FromArgb(220, 53, 69);
                            }
                        }
                    }
                };
            } catch (Exception ex) {
                UIService.ShowError($"Error loading monthly revenue: {ex.Message}");
                dgvMonthlyRevenue.Rows.Add("Error", "", "", "", "", "", "");
            }
        }

        private void LoadPurposeDistributionGrid() {
            try {
                // Configure grid columns
                dgvPurposeDistribution.Columns.Clear();
                dgvPurposeDistribution.Columns.Add("Purpose", "Visit Purpose");
                dgvPurposeDistribution.Columns.Add("Count", "Count");
                dgvPurposeDistribution.Columns.Add("CompletedCount", "Completed");
                dgvPurposeDistribution.Columns.Add("Percentage", "% of Total");
                dgvPurposeDistribution.Columns.Add("AvgFee", "Avg. Fee");
                dgvPurposeDistribution.Columns.Add("TotalRevenue", "Total Revenue");

                // Get selected audiologist
                int? selectedAudiologistId = GetSelectedAudiologistId();

                // Get visit purpose statistics from repository
                var purposeStats = repository.GetVisitPurposeStats(startDate, endDate, selectedAudiologistId);

                if (purposeStats == null || purposeStats.Count == 0) {
                    dgvPurposeDistribution.Rows.Add("No data", "0", "0", "0%", "$0.00", "$0.00");
                    return;
                }

                // Add data rows
                foreach (var stat in purposeStats) {
                    string purpose = "Unknown";
                    int count = 0;
                    int completedCount = 0;
                    double percentage = 0d;
                    decimal avgFee = 0m;
                    decimal totalRevenue = 0m;

                    try { purpose = stat.Purpose; } catch { }
                    try { count = stat.Count; } catch { }
                    try { completedCount = stat.CompletedCount; } catch { }
                    try { percentage = stat.Percentage; } catch { }
                    try { avgFee = stat.AverageFee; } catch { }
                    try { totalRevenue = stat.TotalRevenue; } catch { }

                    int rowIndex = dgvPurposeDistribution.Rows.Add(
                        purpose,
                        count.ToString("N0"),
                        completedCount.ToString("N0"),
                        percentage.ToString("F1") + "%",
                        avgFee.ToString("C2"),
                        totalRevenue.ToString("C2")
                    );

                    // Highlight the total row
                    if (purpose == "TOTAL") {
                        dgvPurposeDistribution.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                        dgvPurposeDistribution.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgvPurposeDistribution.Font, FontStyle.Bold);
                    }
                }
            } catch (Exception ex) {
                UIService.ShowError($"Error loading purpose distribution: {ex.Message}");
                dgvPurposeDistribution.Rows.Add("Error", "", "", "", "", "");
            }
        }

        private int? GetSelectedAudiologistId() {
            if (cmbAudiologist.SelectedIndex <= 0 || audiologistsList == null || audiologistsList.Count == 0)
                return null;

            try {
                return audiologistsList[cmbAudiologist.SelectedIndex].AudiologistID;
            } catch {
                return null;
            }
        }
        #endregion

        #region Event Handlers
        private void BtnRefresh_Click(object sender, EventArgs e) {
            LoadAllData();
        }

        private void BtnApplyDateFilter_Click(object sender, EventArgs e) {
            // Validate date range
            if (dtpEndDate.Value < dtpStartDate.Value) {
                UIService.ShowWarning("End date cannot be earlier than start date.");
                return;
            }

            // Update date range
            startDate = dtpStartDate.Value.Date;
            endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1); // End of selected day

            // Reload financial data
            LoadFinancialAnalysisData();
        }
        #endregion
    }
}