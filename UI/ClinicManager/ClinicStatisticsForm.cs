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

namespace HearingClinicManagementSystem.UI.ClinicManager
{
    public class ClinicStatisticsForm : BaseForm
    {
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
        #endregion

        public ClinicStatisticsForm()
        {
            startDate = DateTime.Now.AddMonths(-3); // Default to last 3 months
            endDate = DateTime.Now;

            InitializeComponents();
            LoadAllData();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Clinic Statistics & Analytics";
            this.Size = new Size(1200, 800);

            // Create title
            var lblTitle = CreateTitleLabel("Clinic Statistics Dashboard");
            lblTitle.Dock = DockStyle.Top;

            // Create main layout panel with refresh button
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            btnRefresh = new Button
            {
                Text = "Refresh Data",
                Size = new Size(120, 30),
                Location = new Point(topPanel.Width - 140, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefresh.Click += BtnRefresh_Click;
            ApplyButtonStyle(btnRefresh, Color.FromArgb(0, 123, 255));

            lblLastRefreshed = new Label
            {
                Text = "Last refreshed: " + DateTime.Now.ToString("g"),
                AutoSize = true,
                Location = new Point(10, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(lblLastRefreshed);

            // Create tab control for different report views
            tabReports = new TabControl
            {
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

        private Panel CreateAppointmentOverviewPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Create summary cards panel
            pnlSummaryCards = new TableLayoutPanel
            {
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
            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
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
            dgvTopAudiologists = new DataGridView
            {
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
            dgvRecentAppointments = new DataGridView
            {
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

        private Panel CreateFinancialAnalysisPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Create filter panel
            Panel filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Date range filter
            Label lblDateRange = new Label
            {
                Text = "Date Range:",
                Location = new Point(10, 15),
                AutoSize = true
            };

            dtpStartDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(90, 13),
                Size = new Size(100, 20),
                Value = startDate
            };

            Label lblTo = new Label
            {
                Text = "to",
                Location = new Point(195, 15),
                AutoSize = true
            };

            dtpEndDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(215, 13),
                Size = new Size(100, 20),
                Value = endDate
            };

            // Audiologist filter
            Label lblAudiologist = new Label
            {
                Text = "Audiologist:",
                Location = new Point(350, 15),
                AutoSize = true
            };

            cmbAudiologist = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(440, 13),
                Size = new Size(150, 20)
            };
            cmbAudiologist.Items.Add("All Audiologists");

            foreach (var audiologist in StaticDataProvider.Audiologists)
            {
                var user = StaticDataProvider.Users.FirstOrDefault(u => u.UserID == audiologist.UserID);
                if (user != null)
                {
                    cmbAudiologist.Items.Add($"Dr. {user.FirstName} {user.LastName}");
                }
            }
            cmbAudiologist.SelectedIndex = 0;

            btnApplyDateFilter = new Button
            {
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
            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
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
            dgvFinancialSummary = new DataGridView
            {
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
            dgvPaymentMethods = new DataGridView
            {
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
            dgvMonthlyRevenue = new DataGridView
            {
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
            dgvPurposeDistribution = new DataGridView
            {
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

        private Panel CreateSummaryPanel(string title, out Label valueLabel)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create title label
            Label lblTitle = new Label
            {
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
            valueLabel = new Label
            {
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

        private Panel CreatePanelWithTitle(string title, out Panel contentContainer)
        {
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font(DefaultFont.FontFamily, 10, FontStyle.Bold),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 0, 0)
            };

            contentContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            mainPanel.Controls.Add(contentContainer);
            mainPanel.Controls.Add(lblTitle);

            return mainPanel;
        }

        private void ConfigureDataGridView(DataGridView dataGridView)
        {
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
                if (e.RowIndex % 2 == 0)
                {
                    e.CellStyle.BackColor = Color.White;
                }
                else
                {
                    e.CellStyle.BackColor = Color.FromArgb(250, 250, 250);
                }
            };
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

            button.MouseEnter += (s, e) => {
                button.BackColor = ControlPaint.Dark(baseColor);
            };
            button.MouseLeave += (s, e) => {
                button.BackColor = baseColor;
            };
        }
        #endregion

        #region Data Loading
        private void LoadAllData()
        {
            try
            {
                LoadAppointmentOverviewData();
                LoadFinancialAnalysisData();

                lblLastRefreshed.Text = "Last refreshed: " + DateTime.Now.ToString("g");
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error loading report data: {ex.Message}");
            }
        }

        private void LoadAppointmentOverviewData()
        {
            LoadSummaryStats();
            LoadTopAudiologistsGrid();
            LoadRecentAppointmentsGrid();
        }

        private void LoadFinancialAnalysisData()
        {
            LoadFinancialSummaryGrid();
            LoadPaymentMethodsGrid();
            LoadMonthlyRevenueGrid();
            LoadPurposeDistributionGrid();
        }

        private void LoadSummaryStats()
        {
            // Get all appointments within the date range
            var allAppointments = StaticDataProvider.Appointments
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .ToList();

            // Total Appointments
            int totalAppointments = allAppointments.Count;
            lblTotalAppointments.Text = totalAppointments.ToString();

            // Get completed appointments
            var completedAppointments = allAppointments
                .Where(a => a.Status == "Completed")
                .ToList();

            // Total Revenue (from completed appointments)
            decimal totalRevenue = completedAppointments.Sum(a => a.Fee);
            lblTotalRevenue.Text = totalRevenue.ToString("C2");

            // Average Appointment Value
            decimal avgAppointmentValue = completedAppointments.Count > 0 ?
                totalRevenue / completedAppointments.Count : 0;
            lblAverageAppointmentValue.Text = avgAppointmentValue.ToString("C2");

            // Most Active Doctor
            var appointmentsByDoctor = allAppointments
                .GroupBy(a => a.AudiologistID)
                .Select(g => new { AudiologistID = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (appointmentsByDoctor.Any())
            {
                var topDoctor = appointmentsByDoctor.First();
                var audiologist = StaticDataProvider.Audiologists.FirstOrDefault(a => a.AudiologistID == topDoctor.AudiologistID);
                if (audiologist != null)
                {
                    var user = StaticDataProvider.Users.FirstOrDefault(u => u.UserID == audiologist.UserID);
                    if (user != null)
                    {
                        lblMostActiveDoctor.Text = $"Dr. {user.FirstName} {user.LastName} ({topDoctor.Count})";
                    }
                }
            }
            else
            {
                lblMostActiveDoctor.Text = "No data available";
            }

            // Completion Rate
            double completionRate = totalAppointments > 0 ?
                (double)completedAppointments.Count / totalAppointments * 100 : 0;
            lblCompletionRate.Text = completionRate.ToString("F1") + "%";

            // Cancellation Rate
            int cancelledAppointments = allAppointments.Count(a => a.Status == "Cancelled");
            double cancellationRate = totalAppointments > 0 ?
                (double)cancelledAppointments / totalAppointments * 100 : 0;
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
        }

        private void LoadTopAudiologistsGrid()
        {
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

            // Get all appointments within date range
            var appointments = StaticDataProvider.Appointments
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .ToList();

            // Group appointments by audiologist
            var audiologistStats = appointments
                .GroupBy(a => a.AudiologistID)
                .Select(g => new {
                    AudiologistID = g.Key,
                    TotalAppointments = g.Count(),
                    CompletedAppointments = g.Count(a => a.Status == "Completed"),
                    CancelledAppointments = g.Count(a => a.Status == "Cancelled"),
                    TotalRevenue = g.Where(a => a.Status == "Completed").Sum(a => a.Fee)
                })
                .OrderByDescending(a => a.TotalRevenue)
                .ToList();

            // Add data rows
            for (int i = 0; i < audiologistStats.Count; i++)
            {
                var audiologist = StaticDataProvider.Audiologists.FirstOrDefault(a => a.AudiologistID == audiologistStats[i].AudiologistID);
                if (audiologist == null) continue;

                var user = StaticDataProvider.Users.FirstOrDefault(u => u.UserID == audiologist.UserID);
                if (user == null) continue;

                string name = $"Dr. {user.FirstName} {user.LastName}";

                decimal avgRevenue = audiologistStats[i].CompletedAppointments > 0 ?
                    audiologistStats[i].TotalRevenue / audiologistStats[i].CompletedAppointments : 0;

                dgvTopAudiologists.Rows.Add(
                    (i + 1).ToString(),
                    audiologistStats[i].AudiologistID.ToString(),
                    name,
                    audiologistStats[i].TotalAppointments.ToString("N0"),
                    audiologistStats[i].CompletedAppointments.ToString("N0"),
                    audiologistStats[i].CancelledAppointments.ToString("N0"),
                    audiologistStats[i].TotalRevenue.ToString("C2"),
                    avgRevenue.ToString("C2")
                );

                // Highlight top 3
                if (i < 3)
                {
                    dgvTopAudiologists.Rows[i].DefaultCellStyle.Font = new Font(dgvTopAudiologists.Font, FontStyle.Bold);
                    if (i == 0)
                        dgvTopAudiologists.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                }
            }
        }

        private void LoadRecentAppointmentsGrid()
        {
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

            // Get most recent appointments (completed or upcoming)
            var recentAppointments = StaticDataProvider.Appointments
                .OrderByDescending(a => a.Date)
                .Take(15)
                .ToList();

            // Add data rows
            foreach (var appointment in recentAppointments)
            {
                // Get patient info
                var patient = StaticDataProvider.Patients.FirstOrDefault(p => p.PatientID == appointment.PatientID);
                var patientName = "Unknown";
                if (patient != null)
                {
                    var patientUser = StaticDataProvider.Users.FirstOrDefault(u => u.UserID == patient.UserID);
                    if (patientUser != null)
                        patientName = $"{patientUser.FirstName} {patientUser.LastName}";
                }

                // Get audiologist info
                var audiologist = StaticDataProvider.Audiologists.FirstOrDefault(a => a.AudiologistID == appointment.AudiologistID);
                var audiologistName = "Unknown";
                if (audiologist != null)
                {
                    var audiologistUser = StaticDataProvider.Users.FirstOrDefault(u => u.UserID == audiologist.UserID);
                    if (audiologistUser != null)
                        audiologistName = $"Dr. {audiologistUser.FirstName} {audiologistUser.LastName}";
                }

                var row = dgvRecentAppointments.Rows.Add(
                    appointment.AppointmentID.ToString(),
                    appointment.Date.ToString("g"),
                    patientName,
                    audiologistName,
                    appointment.PurposeOfVisit,
                    appointment.Fee.ToString("C2"),
                    appointment.Status
                );

                // Color code based on status
                if (appointment.Status == "Completed")
                {
                    dgvRecentAppointments.Rows[row].Cells["Status"].Style.ForeColor = Color.Green;
                    dgvRecentAppointments.Rows[row].Cells["Status"].Style.Font = new Font(dgvRecentAppointments.Font, FontStyle.Bold);
                }
                else if (appointment.Status == "Scheduled")
                {
                    dgvRecentAppointments.Rows[row].Cells["Status"].Style.ForeColor = Color.Blue;
                }
                else if (appointment.Status == "Cancelled")
                {
                    dgvRecentAppointments.Rows[row].Cells["Status"].Style.ForeColor = Color.Red;
                }
            }
        }

        private void LoadFinancialSummaryGrid()
        {
            // Configure grid columns
            dgvFinancialSummary.Columns.Clear();
            dgvFinancialSummary.Columns.Add("Purpose", "Visit Purpose");
            dgvFinancialSummary.Columns.Add("Count", "# of Appointments");
            dgvFinancialSummary.Columns.Add("TotalRevenue", "Total Revenue");
            dgvFinancialSummary.Columns.Add("AvgRevenue", "Avg. Revenue");
            dgvFinancialSummary.Columns.Add("PercentOfTotal", "% of Total");

            // Get selected audiologist
            int? selectedAudiologistId = null;
            if (cmbAudiologist.SelectedIndex > 0)
            {
                string selectedName = cmbAudiologist.SelectedItem.ToString();
                selectedName = selectedName.Replace("Dr. ", "");
                var selectedUser = StaticDataProvider.Users.FirstOrDefault(u =>
                    $"{u.FirstName} {u.LastName}" == selectedName);

                if (selectedUser != null)
                {
                    var selectedAudiologist = StaticDataProvider.Audiologists.FirstOrDefault(a =>
                        a.UserID == selectedUser.UserID);

                    if (selectedAudiologist != null)
                        selectedAudiologistId = selectedAudiologist.AudiologistID;
                }
            }

            // Get appointments within date range and filter by audiologist if selected
            var appointments = StaticDataProvider.Appointments
                .Where(a => a.Status == "Completed")
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .Where(a => !selectedAudiologistId.HasValue || a.AudiologistID == selectedAudiologistId.Value)
                .ToList();

            // Group appointments by purpose
            var financialSummary = appointments
                .GroupBy(a => a.PurposeOfVisit)
                .Select(g => new {
                    Purpose = g.Key,
                    Count = g.Count(),
                    TotalRevenue = g.Sum(a => a.Fee),
                    AvgRevenue = g.Average(a => a.Fee)
                })
                .OrderByDescending(s => s.TotalRevenue)
                .ToList();

            // Calculate totals for percentage
            decimal totalRevenue = appointments.Sum(a => a.Fee);

            // Add data rows
            foreach (var summary in financialSummary)
            {
                decimal percentOfTotal = totalRevenue > 0 ? (summary.TotalRevenue / totalRevenue) * 100 : 0;

                dgvFinancialSummary.Rows.Add(
                    summary.Purpose,
                    summary.Count.ToString("N0"),
                    summary.TotalRevenue.ToString("C2"),
                    summary.AvgRevenue.ToString("C2"),
                    percentOfTotal.ToString("F1") + "%"
                );
            }

            // Add total row
            if (financialSummary.Count > 0)
            {
                int totalCount = financialSummary.Sum(s => s.Count);
                decimal avgRevenue = totalRevenue / totalCount;

                var totalRow = dgvFinancialSummary.Rows.Add(
                    "TOTAL",
                    totalCount.ToString("N0"),
                    totalRevenue.ToString("C2"),
                    avgRevenue.ToString("C2"),
                    "100%"
                );

                dgvFinancialSummary.Rows[totalRow].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                dgvFinancialSummary.Rows[totalRow].DefaultCellStyle.Font = new Font(dgvFinancialSummary.Font, FontStyle.Bold);
            }
        }

        private void LoadPaymentMethodsGrid()
        {
            // Configure grid columns
            dgvPaymentMethods.Columns.Clear();
            dgvPaymentMethods.Columns.Add("PaymentMethod", "Payment Method");
            dgvPaymentMethods.Columns.Add("Count", "Count");
            dgvPaymentMethods.Columns.Add("TotalAmount", "Total Amount");
            dgvPaymentMethods.Columns.Add("PercentOfTotal", "% of Total");

            // Get selected audiologist
            int? selectedAudiologistId = null;
            if (cmbAudiologist.SelectedIndex > 0)
            {
                string selectedName = cmbAudiologist.SelectedItem.ToString();
                selectedName = selectedName.Replace("Dr. ", "");
                var selectedUser = StaticDataProvider.Users.FirstOrDefault(u =>
                    $"{u.FirstName} {u.LastName}" == selectedName);

                if (selectedUser != null)
                {
                    var selectedAudiologist = StaticDataProvider.Audiologists.FirstOrDefault(a =>
                        a.UserID == selectedUser.UserID);

                    if (selectedAudiologist != null)
                        selectedAudiologistId = selectedAudiologist.AudiologistID;
                }
            }

            // Get appointments within date range and filter by audiologist if selected
            var appointments = StaticDataProvider.Appointments
                .Where(a => a.Status == "Completed")
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .Where(a => !selectedAudiologistId.HasValue || a.AudiologistID == selectedAudiologistId.Value)
                .ToList();

            // Get invoices for these appointments
            var appointmentIds = appointments.Select(a => a.AppointmentID).ToList();
            var invoices = StaticDataProvider.Invoices
                .Where(i => i.AppointmentID.HasValue && appointmentIds.Contains(i.AppointmentID.Value))
                .ToList();

            // Group invoices by payment method
            var paymentSummary = invoices
                .GroupBy(i => i.PaymentMethod ?? "Unknown")
                .Select(g => new {
                    Method = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(i => i.TotalAmount)
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToList();

            // Calculate total for percentage
            decimal totalAmount = invoices.Sum(i => i.TotalAmount);

            // Add data rows
            foreach (var summary in paymentSummary)
            {
                decimal percentOfTotal = totalAmount > 0 ? (summary.TotalAmount / totalAmount) * 100 : 0;

                dgvPaymentMethods.Rows.Add(
                    summary.Method,
                    summary.Count.ToString("N0"),
                    summary.TotalAmount.ToString("C2"),
                    percentOfTotal.ToString("F1") + "%"
                );
            }

            // Add total row
            if (paymentSummary.Count > 0)
            {
                int totalCount = paymentSummary.Sum(s => s.Count);

                var totalRow = dgvPaymentMethods.Rows.Add(
                    "TOTAL",
                    totalCount.ToString("N0"),
                    totalAmount.ToString("C2"),
                    "100%"
                );

                dgvPaymentMethods.Rows[totalRow].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                dgvPaymentMethods.Rows[totalRow].DefaultCellStyle.Font = new Font(dgvPaymentMethods.Font, FontStyle.Bold);
            }
        }

        private void LoadMonthlyRevenueGrid()
        {
            // Configure grid columns
            dgvMonthlyRevenue.Columns.Clear();
            dgvMonthlyRevenue.Columns.Add("Month", "Month");
            dgvMonthlyRevenue.Columns.Add("Year", "Year");
            dgvMonthlyRevenue.Columns.Add("AppointmentCount", "Appointments");
            dgvMonthlyRevenue.Columns.Add("CompletedCount", "Completed");
            dgvMonthlyRevenue.Columns.Add("Revenue", "Total Revenue");
            dgvMonthlyRevenue.Columns.Add("AvgRevenue", "Avg. Revenue");
            dgvMonthlyRevenue.Columns.Add("CompletionRate", "Completion Rate");

            // Get start of first day of month, 12 months ago
            DateTime startMonth = new DateTime(DateTime.Now.AddMonths(-11).Year, DateTime.Now.AddMonths(-11).Month, 1);

            // Get selected audiologist
            int? selectedAudiologistId = null;
            if (cmbAudiologist.SelectedIndex > 0)
            {
                string selectedName = cmbAudiologist.SelectedItem.ToString();
                selectedName = selectedName.Replace("Dr. ", "");
                var selectedUser = StaticDataProvider.Users.FirstOrDefault(u =>
                    $"{u.FirstName} {u.LastName}" == selectedName);

                if (selectedUser != null)
                {
                    var selectedAudiologist = StaticDataProvider.Audiologists.FirstOrDefault(a =>
                        a.UserID == selectedUser.UserID);

                    if (selectedAudiologist != null)
                        selectedAudiologistId = selectedAudiologist.AudiologistID;
                }
            }

            // Get appointments within the range
            var appointments = StaticDataProvider.Appointments
                .Where(a => a.Date >= startMonth && a.Date <= DateTime.Now)
                .Where(a => !selectedAudiologistId.HasValue || a.AudiologistID == selectedAudiologistId.Value)
                .ToList();

            // Group appointments by month
            var monthlyStats = appointments
                .GroupBy(a => new { Year = a.Date.Year, Month = a.Date.Month })
                .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
                .Select(g => new {
                    Month = GetMonthName(g.Key.Month),
                    Year = g.Key.Year,
                    TotalCount = g.Count(),
                    CompletedCount = g.Count(a => a.Status == "Completed"),
                    Revenue = g.Where(a => a.Status == "Completed").Sum(a => a.Fee),
                })
                .ToList();

            // Add data rows
            foreach (var stat in monthlyStats)
            {
                decimal avgRevenue = stat.CompletedCount > 0 ? stat.Revenue / stat.CompletedCount : 0;
                double completionRate = stat.TotalCount > 0 ? (double)stat.CompletedCount / stat.TotalCount * 100 : 0;

                dgvMonthlyRevenue.Rows.Add(
                    stat.Month,
                    stat.Year.ToString(),
                    stat.TotalCount.ToString("N0"),
                    stat.CompletedCount.ToString("N0"),
                    stat.Revenue.ToString("C2"),
                    avgRevenue.ToString("C2"),
                    completionRate.ToString("F1") + "%"
                );
            }

            // Add a total row if we have data
            if (monthlyStats.Count > 0)
            {
                int totalCount = monthlyStats.Sum(s => s.TotalCount);
                int completedCount = monthlyStats.Sum(s => s.CompletedCount);
                decimal totalRevenue = monthlyStats.Sum(s => s.Revenue);
                decimal avgRevenue = completedCount > 0 ? totalRevenue / completedCount : 0;
                double completionRate = totalCount > 0 ? (double)completedCount / totalCount * 100 : 0;

                var totalRow = dgvMonthlyRevenue.Rows.Add(
                    "TOTAL",
                    "",
                    totalCount.ToString("N0"),
                    completedCount.ToString("N0"),
                    totalRevenue.ToString("C2"),
                    avgRevenue.ToString("C2"),
                    completionRate.ToString("F1") + "%"
                );

                dgvMonthlyRevenue.Rows[totalRow].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                dgvMonthlyRevenue.Rows[totalRow].DefaultCellStyle.Font = new Font(dgvMonthlyRevenue.Font, FontStyle.Bold);
            }

            // Apply cell formatting
            dgvMonthlyRevenue.CellFormatting += (sender, e) => {
                if (e.ColumnIndex == dgvMonthlyRevenue.Columns["Revenue"].Index && e.Value != null)
                {
                    if (decimal.TryParse(e.Value.ToString().Replace("$", "").Replace(",", ""),
                            out decimal amount))
                    {
                        if (amount > 5000)
                        {
                            e.CellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                            e.CellStyle.Font = new Font(dgvMonthlyRevenue.Font, FontStyle.Bold);
                        }
                    }
                }
                else if (e.ColumnIndex == dgvMonthlyRevenue.Columns["CompletionRate"].Index && e.Value != null)
                {
                    if (double.TryParse(e.Value.ToString().Replace("%", ""), out double rate))
                    {
                        if (rate >= 90)
                        {
                            e.CellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                        }
                        else if (rate < 70)
                        {
                            e.CellStyle.ForeColor = Color.FromArgb(220, 53, 69);
                        }
                    }
                }
            };
        }

        private void LoadPurposeDistributionGrid()
        {
            // Configure grid columns
            dgvPurposeDistribution.Columns.Clear();
            dgvPurposeDistribution.Columns.Add("Purpose", "Visit Purpose");
            dgvPurposeDistribution.Columns.Add("Count", "Count");
            dgvPurposeDistribution.Columns.Add("CompletedCount", "Completed");
            dgvPurposeDistribution.Columns.Add("Percentage", "% of Total");
            dgvPurposeDistribution.Columns.Add("AvgFee", "Avg. Fee");
            dgvPurposeDistribution.Columns.Add("TotalRevenue", "Total Revenue");

            // Get selected audiologist
            int? selectedAudiologistId = null;
            if (cmbAudiologist.SelectedIndex > 0)
            {
                string selectedName = cmbAudiologist.SelectedItem.ToString();
                selectedName = selectedName.Replace("Dr. ", "");
                var selectedUser = StaticDataProvider.Users.FirstOrDefault(u =>
                    $"{u.FirstName} {u.LastName}" == selectedName);

                if (selectedUser != null)
                {
                    var selectedAudiologist = StaticDataProvider.Audiologists.FirstOrDefault(a =>
                        a.UserID == selectedUser.UserID);

                    if (selectedAudiologist != null)
                        selectedAudiologistId = selectedAudiologist.AudiologistID;
                }
            }

            // Get appointments within date range and filter by audiologist if selected
            var appointments = StaticDataProvider.Appointments
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .Where(a => !selectedAudiologistId.HasValue || a.AudiologistID == selectedAudiologistId.Value)
                .ToList();

            // Group appointments by purpose of visit
            var appointmentsByPurpose = appointments
                .GroupBy(a => a.PurposeOfVisit)
                .Select(g => new {
                    Purpose = g.Key,
                    Count = g.Count(),
                    CompletedCount = g.Count(a => a.Status == "Completed"),
                    TotalRevenue = g.Where(a => a.Status == "Completed").Sum(a => a.Fee)
                })
                .OrderByDescending(a => a.Count)
                .ToList();

            // Calculate total for percentage
            int totalAppointments = appointments.Count;

            // Add data rows
            foreach (var summary in appointmentsByPurpose)
            {
                double percentage = totalAppointments > 0 ?
                    (double)summary.Count / totalAppointments * 100 : 0;

                decimal avgFee = summary.CompletedCount > 0 ?
                    summary.TotalRevenue / summary.CompletedCount : 0;

                dgvPurposeDistribution.Rows.Add(
                    summary.Purpose,
                    summary.Count.ToString("N0"),
                    summary.CompletedCount.ToString("N0"),
                    percentage.ToString("F1") + "%",
                    avgFee.ToString("C2"),
                    summary.TotalRevenue.ToString("C2")
                );
            }

            // Add total row
            if (appointmentsByPurpose.Count > 0)
            {
                int totalCount = appointmentsByPurpose.Sum(s => s.Count);
                int totalCompletedCount = appointmentsByPurpose.Sum(s => s.CompletedCount);
                decimal totalRevenue = appointmentsByPurpose.Sum(s => s.TotalRevenue);
                decimal avgFee = totalCompletedCount > 0 ? totalRevenue / totalCompletedCount : 0;

                var totalRow = dgvPurposeDistribution.Rows.Add(
                    "TOTAL",
                    totalCount.ToString("N0"),
                    totalCompletedCount.ToString("N0"),
                    "100%",
                    avgFee.ToString("C2"),
                    totalRevenue.ToString("C2")
                );

                dgvPurposeDistribution.Rows[totalRow].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                dgvPurposeDistribution.Rows[totalRow].DefaultCellStyle.Font = new Font(dgvPurposeDistribution.Font, FontStyle.Bold);
            }
        }

        private string GetMonthName(int month)
        {
            return new DateTime(2020, month, 1).ToString("MMMM");
        }
        #endregion

        #region Event Handlers
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllData();
        }

        private void BtnApplyDateFilter_Click(object sender, EventArgs e)
        {
            // Validate date range
            if (dtpEndDate.Value < dtpStartDate.Value)
            {
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