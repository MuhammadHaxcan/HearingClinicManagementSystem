using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;

namespace HearingClinicManagementSystem.UI.Receptionist
{
    public class PaymentCollectionForm : BaseForm
    {
        #region Fields
        private DataGridView dgvCompletedAppointments;
        private DataGridView dgvInvoices;
        private Panel pnlInvoiceOptions;
        private Panel pnlInvoiceDetails;
        private Label lblTotalFee;
        private Label lblPaidAmount;
        private Label lblRemainingAmount;
        private NumericUpDown nudPaymentAmount;
        private ComboBox cmbPaymentMethod;
        private Button btnCreateInvoice;
        private Button btnRefresh;
        private TableLayoutPanel mainLayout;
        private int selectedAppointmentId = -1;

        private TabControl tabPaymentCollection;
        private DataGridView dgvConfirmedOrders;
        private DataGridView dgvOrderInvoices;
        private Panel pnlOrderPaymentOptions;
        private Panel pnlOrderInvoiceDetails;
        private Label lblOrderTotal;
        private Label lblOrderPaid;
        private Label lblOrderRemaining;
        private NumericUpDown nudOrderPaymentAmount;
        private ComboBox cmbOrderPaymentMethod;
        private Button btnCreateOrderInvoice;
        private Button btnRefreshOrders;
        private TableLayoutPanel orderLayout;
        private int selectedOrderId = -1;

        private HearingClinicRepository repository;
        #endregion

        public PaymentCollectionForm()
        {
            repository = HearingClinicRepository.Instance;
            InitializeComponents();
            LoadCompletedAppointments();
            LoadConfirmedOrders();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Payment Collection";
            this.Size = new Size(1200, 700);

            var lblTitle = CreateTitleLabel("Payment Collection");
            lblTitle.Dock = DockStyle.Top;

            // Create tab control
            tabPaymentCollection = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Regular)
            };

            // Create appointment payments tab
            TabPage tabAppointments = new TabPage("Appointment Payments");
            InitializeAppointmentTab(tabAppointments);
            tabPaymentCollection.TabPages.Add(tabAppointments);

            // Create product orders tab
            TabPage tabOrders = new TabPage("Order Payments");
            InitializeOrderTab(tabOrders);
            tabPaymentCollection.TabPages.Add(tabOrders);

            // Add tab control to form
            Controls.Add(tabPaymentCollection);
            Controls.Add(lblTitle);
        }

        private void InitializeAppointmentTab(TabPage tabPage)
        {
            // Main layout with 2 rows and 2 columns
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Set column and row styles
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

            // Create and add all panels
            InitializeAppointmentsPanel();
            InitializeInvoiceOptionsPanel();
            InitializeInvoicesPanel();

            tabPage.Controls.Add(mainLayout);
        }

        private void InitializeOrderTab(TabPage tabPage)
        {
            // Main layout with 2 rows and 2 columns
            orderLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Set column and row styles
            orderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            orderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            orderLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            orderLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

            // Create and add all panels
            InitializeOrdersPanel();
            InitializeOrderPaymentOptionsPanel();
            InitializeOrderInvoicesPanel();

            tabPage.Controls.Add(orderLayout);
        }

        private void InitializeAppointmentsPanel()
        {
            // Create panel for completed appointments
            Panel pnlCompletedAppointments = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Create header with label and refresh button
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.White
            };

            var lblAppointments = CreateLabel("Completed Appointments", 5, 10);
            lblAppointments.Font = new Font(lblAppointments.Font.FontFamily, 12, FontStyle.Bold);
            lblAppointments.AutoSize = true;

            btnRefresh = CreateButton("Refresh", headerPanel.Width - 120, 5, BtnRefresh_Click, 100, 30);
            ApplyButtonStyle(btnRefresh, Color.FromArgb(0, 123, 255));
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            headerPanel.Controls.Add(lblAppointments);
            headerPanel.Controls.Add(btnRefresh);

            // Create appointments grid
            dgvCompletedAppointments = CreateDataGrid(0, false, true);
            dgvCompletedAppointments.Dock = DockStyle.Fill;
            dgvCompletedAppointments.MultiSelect = false;
            dgvCompletedAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCompletedAppointments.BorderStyle = BorderStyle.None;
            dgvCompletedAppointments.BackgroundColor = Color.White;
            dgvCompletedAppointments.CellFormatting += DgvCompletedAppointments_CellFormatting;
            dgvCompletedAppointments.SelectionChanged += DgvCompletedAppointments_SelectionChanged;
            dgvCompletedAppointments.RowHeadersVisible = false;
            dgvCompletedAppointments.AllowUserToAddRows = false;
            dgvCompletedAppointments.AllowUserToDeleteRows = false;
            dgvCompletedAppointments.AllowUserToResizeRows = false;
            dgvCompletedAppointments.ReadOnly = true;

            // Set up grid columns
            dgvCompletedAppointments.Columns.Add("AppointmentID", "ID");
            dgvCompletedAppointments.Columns.Add("Date", "Date");
            dgvCompletedAppointments.Columns.Add("Patient", "Patient");
            dgvCompletedAppointments.Columns.Add("Audiologist", "Audiologist");
            dgvCompletedAppointments.Columns.Add("Fee", "Fee");
            dgvCompletedAppointments.Columns.Add("PaidAmount", "Paid");
            dgvCompletedAppointments.Columns.Add("RemainingAmount", "Remaining");
            dgvCompletedAppointments.Columns.Add("Status", "Status");
            dgvCompletedAppointments.Columns["AppointmentID"].Visible = false;

            // Set column widths as percentages for better optimization
            dgvCompletedAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCompletedAppointments.Columns["Date"].FillWeight = 8;
            dgvCompletedAppointments.Columns["Patient"].FillWeight = 20;
            dgvCompletedAppointments.Columns["Audiologist"].FillWeight = 20;
            dgvCompletedAppointments.Columns["Fee"].FillWeight = 10;
            dgvCompletedAppointments.Columns["PaidAmount"].FillWeight = 10;
            dgvCompletedAppointments.Columns["RemainingAmount"].FillWeight = 13;
            dgvCompletedAppointments.Columns["Status"].FillWeight = 19;

            // Style the grid header
            dgvCompletedAppointments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvCompletedAppointments.ColumnHeadersDefaultCellStyle.Font = new Font(dgvCompletedAppointments.Font, FontStyle.Bold);
            dgvCompletedAppointments.ColumnHeadersHeight = 35;
            dgvCompletedAppointments.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Add controls to panel
            pnlCompletedAppointments.Controls.Add(dgvCompletedAppointments);
            pnlCompletedAppointments.Controls.Add(headerPanel);

            // Add panel to main layout
            mainLayout.Controls.Add(pnlCompletedAppointments, 0, 0);
        }

        private void InitializeInvoiceOptionsPanel()
        {
            // Panel for invoice options
            pnlInvoiceOptions = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Header label
            var lblOptionsTitle = CreateLabel("Invoice Options", 0, 5);
            lblOptionsTitle.Font = new Font(lblOptionsTitle.Font.FontFamily, 12, FontStyle.Bold);
            lblOptionsTitle.Dock = DockStyle.Top;
            lblOptionsTitle.Height = 30;

            // Create card-style panel for options
            Panel optionsCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Create financial summary section
            Panel summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create title for summary section
            Label lblSummaryTitle = CreateLabel("Payment Summary", 5, 5);
            lblSummaryTitle.Font = new Font(lblSummaryTitle.Font.FontFamily, 10, FontStyle.Bold);
            lblSummaryTitle.AutoSize = true;
            lblSummaryTitle.ForeColor = Color.FromArgb(60, 60, 60);

            // Create layout for financial details
            TableLayoutPanel summaryLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            // Set column styles
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // Total fee
            Label lblTotalFeeTitle = CreateLabel("Total Fee:", 0, 0);
            lblTotalFeeTitle.Dock = DockStyle.Fill;
            lblTotalFeeTitle.TextAlign = ContentAlignment.BottomCenter;
            lblTotalFeeTitle.Font = new Font(lblTotalFeeTitle.Font, FontStyle.Regular);

            lblTotalFee = CreateLabel("$0.00", 0, 0);
            lblTotalFee.Dock = DockStyle.Fill;
            lblTotalFee.TextAlign = ContentAlignment.TopCenter;
            lblTotalFee.Font = new Font(lblTotalFee.Font.FontFamily, 12, FontStyle.Bold);

            // Paid amount
            Label lblPaidAmountTitle = CreateLabel("Amount Paid:", 0, 0);
            lblPaidAmountTitle.Dock = DockStyle.Fill;
            lblPaidAmountTitle.TextAlign = ContentAlignment.BottomCenter;
            lblPaidAmountTitle.Font = new Font(lblPaidAmountTitle.Font, FontStyle.Regular);

            lblPaidAmount = CreateLabel("$0.00", 0, 0);
            lblPaidAmount.Dock = DockStyle.Fill;
            lblPaidAmount.TextAlign = ContentAlignment.TopCenter;
            lblPaidAmount.Font = new Font(lblPaidAmount.Font.FontFamily, 12, FontStyle.Bold);
            lblPaidAmount.ForeColor = Color.Green;

            // Remaining amount
            Label lblRemainingAmountTitle = CreateLabel("Amount Remaining:", 0, 0);
            lblRemainingAmountTitle.Dock = DockStyle.Fill;
            lblRemainingAmountTitle.TextAlign = ContentAlignment.BottomCenter;
            lblRemainingAmountTitle.Font = new Font(lblRemainingAmountTitle.Font, FontStyle.Regular);

            lblRemainingAmount = CreateLabel("$0.00", 0, 0);
            lblRemainingAmount.Dock = DockStyle.Fill;
            lblRemainingAmount.TextAlign = ContentAlignment.TopCenter;
            lblRemainingAmount.Font = new Font(lblRemainingAmount.Font.FontFamily, 12, FontStyle.Bold);
            lblRemainingAmount.ForeColor = Color.Red;

            // Add labels to layout
            summaryLayout.Controls.Add(lblTotalFeeTitle, 0, 0);
            summaryLayout.Controls.Add(lblTotalFee, 0, 1);
            summaryLayout.Controls.Add(lblPaidAmountTitle, 1, 0);
            summaryLayout.Controls.Add(lblPaidAmount, 1, 1);
            summaryLayout.Controls.Add(lblRemainingAmountTitle, 2, 0);
            summaryLayout.Controls.Add(lblRemainingAmount, 2, 1);

            // Add controls to summary panel
            summaryPanel.Controls.Add(summaryLayout);
            summaryPanel.Controls.Add(lblSummaryTitle);

            // Create new invoice section
            Panel newInvoicePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 15, 0, 0)
            };

            TableLayoutPanel paymentInputsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(0, 10, 0, 10)
            };

            paymentInputsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            paymentInputsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Payment amount
            Label lblAmount = CreateLabel("Amount ($):", 0, 0);
            lblAmount.Dock = DockStyle.Fill;
            lblAmount.TextAlign = ContentAlignment.MiddleLeft;

            nudPaymentAmount = CreateNumericUpDown(0, 0, 0);
            nudPaymentAmount.Dock = DockStyle.Fill;
            nudPaymentAmount.Minimum = 0;
            nudPaymentAmount.Maximum = 10000;
            nudPaymentAmount.DecimalPlaces = 2;
            nudPaymentAmount.Increment = 10M;
            nudPaymentAmount.Value = 0;
            nudPaymentAmount.Enabled = false;
            nudPaymentAmount.ValueChanged += NudPaymentAmount_ValueChanged;

            // Payment method
            Label lblMethod = CreateLabel("Payment Method:", 0, 0);
            lblMethod.Dock = DockStyle.Fill;
            lblMethod.TextAlign = ContentAlignment.MiddleLeft;

            cmbPaymentMethod = CreateComboBox(0, 0, 0);
            cmbPaymentMethod.Dock = DockStyle.Fill;
            cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMethod.Items.AddRange(new object[] { "Cash", "Credit Card", "Debit Card", "Insurance" });
            cmbPaymentMethod.SelectedIndex = 0;
            cmbPaymentMethod.Enabled = false;

            // Add inputs to layout
            paymentInputsLayout.Controls.Add(lblAmount, 0, 0);
            paymentInputsLayout.Controls.Add(nudPaymentAmount, 1, 0);
            paymentInputsLayout.Controls.Add(lblMethod, 0, 1);
            paymentInputsLayout.Controls.Add(cmbPaymentMethod, 1, 1);

            // Create invoice button
            btnCreateInvoice = CreateButton("Create Invoice", 0, 0, BtnCreateInvoice_Click, 150, 40);
            btnCreateInvoice.Anchor = AnchorStyles.Right;
            ApplyButtonStyle(btnCreateInvoice, Color.FromArgb(40, 167, 69));
            btnCreateInvoice.Margin = new Padding(3, 15, 3, 3);
            btnCreateInvoice.Enabled = false;

            // Add button to the last row and make it span both columns
            paymentInputsLayout.Controls.Add(btnCreateInvoice, 1, 2);
            paymentInputsLayout.SetColumnSpan(btnCreateInvoice, 1);

            // Add controls to new invoice panel
            newInvoicePanel.Controls.Add(paymentInputsLayout);

            // Add all sections to options card
            optionsCard.Controls.Add(newInvoicePanel);
            optionsCard.Controls.Add(summaryPanel);

            // Add card to panel
            pnlInvoiceOptions.Controls.Add(optionsCard);
            pnlInvoiceOptions.Controls.Add(lblOptionsTitle);

            // Add panel to main layout
            mainLayout.Controls.Add(pnlInvoiceOptions, 1, 0);
        }

        private void InitializeInvoicesPanel()
        {
            // Panel for invoices
            pnlInvoiceDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Header label
            var lblInvoicesTitle = CreateLabel("Appointment Invoices", 0, 5);
            lblInvoicesTitle.Font = new Font(lblInvoicesTitle.Font.FontFamily, 12, FontStyle.Bold);
            lblInvoicesTitle.Dock = DockStyle.Top;
            lblInvoicesTitle.Height = 30;

            // Create invoices grid
            dgvInvoices = CreateDataGrid(0, false, true);
            dgvInvoices.Dock = DockStyle.Fill;
            dgvInvoices.MultiSelect = false;
            dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoices.BorderStyle = BorderStyle.None;
            dgvInvoices.BackgroundColor = Color.White;
            dgvInvoices.RowHeadersVisible = false;
            dgvInvoices.AllowUserToAddRows = false;
            dgvInvoices.AllowUserToDeleteRows = false;
            dgvInvoices.AllowUserToResizeRows = false;
            dgvInvoices.ReadOnly = true;

            // Set up grid columns
            dgvInvoices.Columns.Add("InvoiceID", "Invoice #");
            dgvInvoices.Columns.Add("Date", "Date");
            dgvInvoices.Columns.Add("Amount", "Amount");
            dgvInvoices.Columns.Add("Method", "Payment Method");
            dgvInvoices.Columns.Add("Status", "Status");
            dgvInvoices.Columns.Add("CreatedBy", "Created By");

            // Set column widths as percentages for better optimization
            dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInvoices.Columns["InvoiceID"].FillWeight = 10;
            dgvInvoices.Columns["Date"].FillWeight = 15;
            dgvInvoices.Columns["Amount"].FillWeight = 15;
            dgvInvoices.Columns["Method"].FillWeight = 20;
            dgvInvoices.Columns["Status"].FillWeight = 10;
            dgvInvoices.Columns["CreatedBy"].FillWeight = 30;

            // Style the grid header
            dgvInvoices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvInvoices.ColumnHeadersDefaultCellStyle.Font = new Font(dgvInvoices.Font, FontStyle.Bold);
            dgvInvoices.ColumnHeadersHeight = 35;
            dgvInvoices.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvInvoices.CellFormatting += DgvInvoices_CellFormatting;

            // Add no invoices message label (will be shown when no invoices exist)
            Label lblNoInvoices = new Label
            {
                Text = "No invoices found for this appointment",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Visible = false,
                Tag = "NoInvoicesMessage" // Use tag to identify this label
            };

            // Add controls to panel
            pnlInvoiceDetails.Controls.Add(dgvInvoices);
            pnlInvoiceDetails.Controls.Add(lblNoInvoices);
            pnlInvoiceDetails.Controls.Add(lblInvoicesTitle);

            // Add panel to main layout - spans 2 columns in second row
            mainLayout.Controls.Add(pnlInvoiceDetails, 0, 1);
            mainLayout.SetColumnSpan(pnlInvoiceDetails, 2);
        }

        private void InitializeOrdersPanel()
        {
            // Create panel for confirmed orders
            Panel pnlConfirmedOrders = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Create header with label and refresh button
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.White
            };

            var lblOrders = CreateLabel("Confirmed Orders", 5, 10);
            lblOrders.Font = new Font(lblOrders.Font.FontFamily, 12, FontStyle.Bold);
            lblOrders.AutoSize = true;

            btnRefreshOrders = CreateButton("Refresh", headerPanel.Width - 120, 5, BtnRefreshOrders_Click, 100, 30);
            ApplyButtonStyle(btnRefreshOrders, Color.FromArgb(0, 123, 255));
            btnRefreshOrders.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            headerPanel.Controls.Add(lblOrders);
            headerPanel.Controls.Add(btnRefreshOrders);

            // Create orders grid
            dgvConfirmedOrders = CreateDataGrid(0, false, true);
            dgvConfirmedOrders.Dock = DockStyle.Fill;
            dgvConfirmedOrders.MultiSelect = false;
            dgvConfirmedOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvConfirmedOrders.BorderStyle = BorderStyle.None;
            dgvConfirmedOrders.BackgroundColor = Color.White;
            dgvConfirmedOrders.CellFormatting += DgvConfirmedOrders_CellFormatting;
            dgvConfirmedOrders.SelectionChanged += DgvConfirmedOrders_SelectionChanged;
            dgvConfirmedOrders.RowHeadersVisible = false;
            dgvConfirmedOrders.AllowUserToAddRows = false;
            dgvConfirmedOrders.AllowUserToDeleteRows = false;
            dgvConfirmedOrders.AllowUserToResizeRows = false;
            dgvConfirmedOrders.ReadOnly = true;

            // Set up grid columns
            dgvConfirmedOrders.Columns.Add("OrderID", "Order #");
            dgvConfirmedOrders.Columns.Add("OrderDate", "Date");
            dgvConfirmedOrders.Columns.Add("Patient", "Patient");
            dgvConfirmedOrders.Columns.Add("TotalAmount", "Total");
            dgvConfirmedOrders.Columns.Add("PaidAmount", "Paid");
            dgvConfirmedOrders.Columns.Add("RemainingAmount", "Remaining");
            dgvConfirmedOrders.Columns.Add("Status", "Status");
            dgvConfirmedOrders.Columns.Add("Items", "Products"); // Renamed column from "Items" to "Products" for clarity

            // Set column widths as percentages for better optimization
            dgvConfirmedOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvConfirmedOrders.Columns["OrderID"].FillWeight = 8;
            dgvConfirmedOrders.Columns["OrderDate"].FillWeight = 10; // Reduced to give more space for products
            dgvConfirmedOrders.Columns["Patient"].FillWeight = 15;  // Reduced to give more space for products
            dgvConfirmedOrders.Columns["TotalAmount"].FillWeight = 12;
            dgvConfirmedOrders.Columns["PaidAmount"].FillWeight = 10;
            dgvConfirmedOrders.Columns["RemainingAmount"].FillWeight = 10;
            dgvConfirmedOrders.Columns["Status"].FillWeight = 10;
            dgvConfirmedOrders.Columns["Items"].FillWeight = 25;   // Increased to show product names

            // Style the grid header
            dgvConfirmedOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvConfirmedOrders.ColumnHeadersDefaultCellStyle.Font = new Font(dgvConfirmedOrders.Font, FontStyle.Bold);
            dgvConfirmedOrders.ColumnHeadersHeight = 35;
            dgvConfirmedOrders.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Add text wrapping for the Products column
            dgvConfirmedOrders.Columns["Items"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvConfirmedOrders.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Add controls to panel
            pnlConfirmedOrders.Controls.Add(dgvConfirmedOrders);
            pnlConfirmedOrders.Controls.Add(headerPanel);

            // Add panel to order layout
            orderLayout.Controls.Add(pnlConfirmedOrders, 0, 0);
        }

        private void InitializeOrderPaymentOptionsPanel()
        {
            // Panel for order payment options
            pnlOrderPaymentOptions = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Header label
            var lblOptionsTitle = CreateLabel("Payment Options", 0, 5);
            lblOptionsTitle.Font = new Font(lblOptionsTitle.Font.FontFamily, 12, FontStyle.Bold);
            lblOptionsTitle.Dock = DockStyle.Top;
            lblOptionsTitle.Height = 30;

            // Create card-style panel for options
            Panel optionsCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Create financial summary section
            Panel summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create title for summary section
            Label lblSummaryTitle = CreateLabel("Order Summary", 5, 5);
            lblSummaryTitle.Font = new Font(lblSummaryTitle.Font.FontFamily, 10, FontStyle.Bold);
            lblSummaryTitle.AutoSize = true;
            lblSummaryTitle.ForeColor = Color.FromArgb(60, 60, 60);

            // Create layout for financial details
            TableLayoutPanel summaryLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            // Set column styles
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // Total amount
            Label lblOrderTotalTitle = CreateLabel("Order Total:", 0, 0);
            lblOrderTotalTitle.Dock = DockStyle.Fill;
            lblOrderTotalTitle.TextAlign = ContentAlignment.BottomCenter;
            lblOrderTotalTitle.Font = new Font(lblOrderTotalTitle.Font, FontStyle.Regular);

            lblOrderTotal = CreateLabel("$0.00", 0, 0);
            lblOrderTotal.Dock = DockStyle.Fill;
            lblOrderTotal.TextAlign = ContentAlignment.TopCenter;
            lblOrderTotal.Font = new Font(lblOrderTotal.Font.FontFamily, 12, FontStyle.Bold);

            // Paid amount
            Label lblOrderPaidTitle = CreateLabel("Amount Paid:", 0, 0);
            lblOrderPaidTitle.Dock = DockStyle.Fill;
            lblOrderPaidTitle.TextAlign = ContentAlignment.BottomCenter;
            lblOrderPaidTitle.Font = new Font(lblOrderPaidTitle.Font, FontStyle.Regular);

            lblOrderPaid = CreateLabel("$0.00", 0, 0);
            lblOrderPaid.Dock = DockStyle.Fill;
            lblOrderPaid.TextAlign = ContentAlignment.TopCenter;
            lblOrderPaid.Font = new Font(lblOrderPaid.Font.FontFamily, 12, FontStyle.Bold);
            lblOrderPaid.ForeColor = Color.Green;

            // Remaining amount
            Label lblOrderRemainingTitle = CreateLabel("Amount Remaining:", 0, 0);
            lblOrderRemainingTitle.Dock = DockStyle.Fill;
            lblOrderRemainingTitle.TextAlign = ContentAlignment.BottomCenter;
            lblOrderRemainingTitle.Font = new Font(lblOrderRemainingTitle.Font, FontStyle.Regular);

            lblOrderRemaining = CreateLabel("$0.00", 0, 0);
            lblOrderRemaining.Dock = DockStyle.Fill;
            lblOrderRemaining.TextAlign = ContentAlignment.TopCenter;
            lblOrderRemaining.Font = new Font(lblOrderRemaining.Font.FontFamily, 12, FontStyle.Bold);
            lblOrderRemaining.ForeColor = Color.Red;

            // Add labels to layout
            summaryLayout.Controls.Add(lblOrderTotalTitle, 0, 0);
            summaryLayout.Controls.Add(lblOrderTotal, 0, 1);
            summaryLayout.Controls.Add(lblOrderPaidTitle, 1, 0);
            summaryLayout.Controls.Add(lblOrderPaid, 1, 1);
            summaryLayout.Controls.Add(lblOrderRemainingTitle, 2, 0);
            summaryLayout.Controls.Add(lblOrderRemaining, 2, 1);

            // Add controls to summary panel
            summaryPanel.Controls.Add(summaryLayout);
            summaryPanel.Controls.Add(lblSummaryTitle);

            // Create new payment section
            Panel newPaymentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 15, 0, 0)
            };

            TableLayoutPanel paymentInputsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(0, 10, 0, 10)
            };

            paymentInputsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            paymentInputsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Payment amount
            Label lblAmount = CreateLabel("Amount ($):", 0, 0);
            lblAmount.Dock = DockStyle.Fill;
            lblAmount.TextAlign = ContentAlignment.MiddleLeft;

            nudOrderPaymentAmount = CreateNumericUpDown(0, 0, 0);
            nudOrderPaymentAmount.Dock = DockStyle.Fill;
            nudOrderPaymentAmount.Minimum = 0;
            nudOrderPaymentAmount.Maximum = 10000;
            nudOrderPaymentAmount.DecimalPlaces = 2;
            nudOrderPaymentAmount.Increment = 10M;
            nudOrderPaymentAmount.Value = 0;
            nudOrderPaymentAmount.Enabled = false;
            nudOrderPaymentAmount.ValueChanged += NudOrderPaymentAmount_ValueChanged;

            // Payment method
            Label lblMethod = CreateLabel("Payment Method:", 0, 0);
            lblMethod.Dock = DockStyle.Fill;
            lblMethod.TextAlign = ContentAlignment.MiddleLeft;

            cmbOrderPaymentMethod = CreateComboBox(0, 0, 0);
            cmbOrderPaymentMethod.Dock = DockStyle.Fill;
            cmbOrderPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrderPaymentMethod.Items.AddRange(new object[] { "Cash", "Credit Card", "Debit Card" });
            cmbOrderPaymentMethod.SelectedIndex = 0;
            cmbOrderPaymentMethod.Enabled = false;

            // Add inputs to layout
            paymentInputsLayout.Controls.Add(lblAmount, 0, 0);
            paymentInputsLayout.Controls.Add(nudOrderPaymentAmount, 1, 0);
            paymentInputsLayout.Controls.Add(lblMethod, 0, 1);
            paymentInputsLayout.Controls.Add(cmbOrderPaymentMethod, 1, 1);

            // Create payment button
            btnCreateOrderInvoice = CreateButton("Process Payment", 0, 0, BtnCreateOrderInvoice_Click, 150, 40);
            btnCreateOrderInvoice.Anchor = AnchorStyles.Right;
            ApplyButtonStyle(btnCreateOrderInvoice, Color.FromArgb(40, 167, 69));
            btnCreateOrderInvoice.Margin = new Padding(3, 15, 3, 3);
            btnCreateOrderInvoice.Enabled = false;

            // Add button to the last row
            paymentInputsLayout.Controls.Add(btnCreateOrderInvoice, 1, 2);

            // Add controls to new payment panel
            newPaymentPanel.Controls.Add(paymentInputsLayout);

            // Add all sections to options card
            optionsCard.Controls.Add(newPaymentPanel);
            optionsCard.Controls.Add(summaryPanel);

            // Add card to panel
            pnlOrderPaymentOptions.Controls.Add(optionsCard);
            pnlOrderPaymentOptions.Controls.Add(lblOptionsTitle);

            // Add panel to order layout
            orderLayout.Controls.Add(pnlOrderPaymentOptions, 1, 0);
        }

        private void InitializeOrderInvoicesPanel()
        {
            // Panel for order invoices
            pnlOrderInvoiceDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Header label
            var lblInvoicesTitle = CreateLabel("Order Payment History", 0, 5);
            lblInvoicesTitle.Font = new Font(lblInvoicesTitle.Font.FontFamily, 12, FontStyle.Bold);
            lblInvoicesTitle.Dock = DockStyle.Top;
            lblInvoicesTitle.Height = 30;

            // Create order invoices grid
            dgvOrderInvoices = CreateDataGrid(0, false, true);
            dgvOrderInvoices.Dock = DockStyle.Fill;
            dgvOrderInvoices.MultiSelect = false;
            dgvOrderInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrderInvoices.BorderStyle = BorderStyle.None;
            dgvOrderInvoices.BackgroundColor = Color.White;
            dgvOrderInvoices.RowHeadersVisible = false;
            dgvOrderInvoices.AllowUserToAddRows = false;
            dgvOrderInvoices.AllowUserToDeleteRows = false;
            dgvOrderInvoices.AllowUserToResizeRows = false;
            dgvOrderInvoices.ReadOnly = true;

            // Set up grid columns
            dgvOrderInvoices.Columns.Add("InvoiceID", "Invoice #");
            dgvOrderInvoices.Columns.Add("Date", "Date");
            dgvOrderInvoices.Columns.Add("Amount", "Amount");
            dgvOrderInvoices.Columns.Add("Method", "Payment Method");
            dgvOrderInvoices.Columns.Add("Status", "Status");
            dgvOrderInvoices.Columns.Add("CreatedBy", "Created By");

            // Set column widths as percentages for better optimization
            dgvOrderInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrderInvoices.Columns["InvoiceID"].FillWeight = 10;
            dgvOrderInvoices.Columns["Date"].FillWeight = 15;
            dgvOrderInvoices.Columns["Amount"].FillWeight = 15;
            dgvOrderInvoices.Columns["Method"].FillWeight = 20;
            dgvOrderInvoices.Columns["Status"].FillWeight = 10;
            dgvOrderInvoices.Columns["CreatedBy"].FillWeight = 30;

            // Style the grid header
            dgvOrderInvoices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvOrderInvoices.ColumnHeadersDefaultCellStyle.Font = new Font(dgvOrderInvoices.Font, FontStyle.Bold);
            dgvOrderInvoices.ColumnHeadersHeight = 35;
            dgvOrderInvoices.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvOrderInvoices.CellFormatting += DgvOrderInvoices_CellFormatting;

            // Add no invoices message label
            Label lblNoOrderInvoices = new Label
            {
                Text = "No payment history found for this order",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Visible = false,
                Tag = "NoOrderInvoicesMessage"
            };

            // Add controls to panel
            pnlOrderInvoiceDetails.Controls.Add(dgvOrderInvoices);
            pnlOrderInvoiceDetails.Controls.Add(lblNoOrderInvoices);
            pnlOrderInvoiceDetails.Controls.Add(lblInvoicesTitle);

            // Add panel to order layout - spans 2 columns in second row
            orderLayout.Controls.Add(pnlOrderInvoiceDetails, 0, 1);
            orderLayout.SetColumnSpan(pnlOrderInvoiceDetails, 2);
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

        #region Event Handlers
        private void DgvCompletedAppointments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCompletedAppointments.Rows[e.RowIndex];
                string status = row.Cells["Status"].Value?.ToString();

                // Format status cell
                if (e.ColumnIndex == dgvCompletedAppointments.Columns["Status"].Index)
                {
                    if (status == "Paid")
                    {
                        e.CellStyle.ForeColor = Color.Green;
                        e.CellStyle.Font = new Font(dgvCompletedAppointments.Font, FontStyle.Bold);
                    }
                    else if (status == "Partially Paid")
                    {
                        e.CellStyle.ForeColor = Color.Orange;
                        e.CellStyle.Font = new Font(dgvCompletedAppointments.Font, FontStyle.Bold);
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Red;
                    }
                }

                // Format currency columns
                if (e.ColumnIndex == dgvCompletedAppointments.Columns["Fee"].Index ||
                    e.ColumnIndex == dgvCompletedAppointments.Columns["PaidAmount"].Index ||
                    e.ColumnIndex == dgvCompletedAppointments.Columns["RemainingAmount"].Index)
                {
                    if (e.Value != null && decimal.TryParse(e.Value.ToString().Replace("$", "").Replace(",", ""), out decimal amount))
                    {
                        e.Value = amount.ToString("C", CultureInfo.CurrentCulture);
                        e.FormattingApplied = true;

                        if (e.ColumnIndex == dgvCompletedAppointments.Columns["PaidAmount"].Index && amount > 0)
                        {
                            e.CellStyle.ForeColor = Color.Green;
                        }
                        else if (e.ColumnIndex == dgvCompletedAppointments.Columns["RemainingAmount"].Index && amount > 0)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                    }
                }

                // Alternate row coloring for better readability
                if (e.RowIndex % 2 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void DgvInvoices_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvInvoices.Rows[e.RowIndex];

                // Format status cell
                if (e.ColumnIndex == dgvInvoices.Columns["Status"].Index)
                {
                    string status = row.Cells["Status"].Value?.ToString();
                    if (status == "Paid")
                    {
                        e.CellStyle.ForeColor = Color.Green;
                        e.CellStyle.Font = new Font(dgvInvoices.Font, FontStyle.Bold);
                    }
                    else if (status == "Pending")
                    {
                        e.CellStyle.ForeColor = Color.Orange;
                    }
                }

                // Format amount column as currency
                if (e.ColumnIndex == dgvInvoices.Columns["Amount"].Index && e.Value != null)
                {
                    if (decimal.TryParse(e.Value.ToString().Replace("$", "").Replace(",", ""),
                            out decimal amount))
                    {
                        e.Value = amount.ToString("C", CultureInfo.CurrentCulture);
                        e.FormattingApplied = true;
                    }
                }

                // Alternate row coloring for better readability
                if (e.RowIndex % 2 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void DgvConfirmedOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvConfirmedOrders.Rows[e.RowIndex];
                string status = row.Cells["Status"].Value?.ToString();

                // Format status cell
                if (e.ColumnIndex == dgvConfirmedOrders.Columns["Status"].Index)
                {
                    if (status == "Paid" || status == "Completed")
                    {
                        e.CellStyle.ForeColor = Color.Green;
                        e.CellStyle.Font = new Font(dgvConfirmedOrders.Font, FontStyle.Bold);
                    }
                    else if (status == "Partially Paid")
                    {
                        e.CellStyle.ForeColor = Color.Orange;
                        e.CellStyle.Font = new Font(dgvConfirmedOrders.Font, FontStyle.Bold);
                    }
                    else // Confirmed status
                    {
                        e.CellStyle.ForeColor = Color.Blue;
                    }
                }

                // Format currency columns
                if (e.ColumnIndex == dgvConfirmedOrders.Columns["TotalAmount"].Index ||
                    e.ColumnIndex == dgvConfirmedOrders.Columns["PaidAmount"].Index ||
                    e.ColumnIndex == dgvConfirmedOrders.Columns["RemainingAmount"].Index)
                {
                    if (e.Value != null && decimal.TryParse(e.Value.ToString().Replace("$", "").Replace(",", ""), out decimal amount))
                    {
                        e.Value = amount.ToString("C", CultureInfo.CurrentCulture);
                        e.FormattingApplied = true;

                        if (e.ColumnIndex == dgvConfirmedOrders.Columns["PaidAmount"].Index && amount > 0)
                        {
                            e.CellStyle.ForeColor = Color.Green;
                        }
                        else if (e.ColumnIndex == dgvConfirmedOrders.Columns["RemainingAmount"].Index && amount > 0)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                    }
                }

                // Special formatting for the Products column
                if (e.ColumnIndex == dgvConfirmedOrders.Columns["Items"].Index)
                {
                    e.CellStyle.Font = new Font(dgvConfirmedOrders.Font.FontFamily, dgvConfirmedOrders.Font.Size - 0.5f);
                    e.CellStyle.Padding = new Padding(2, 2, 2, 2);
                }

                // Alternate row coloring for better readability
                if (e.RowIndex % 2 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void DgvOrderInvoices_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvOrderInvoices.Rows[e.RowIndex];

                // Format status cell
                if (e.ColumnIndex == dgvOrderInvoices.Columns["Status"].Index)
                {
                    string status = row.Cells["Status"].Value?.ToString();
                    if (status == "Paid")
                    {
                        e.CellStyle.ForeColor = Color.Green;
                        e.CellStyle.Font = new Font(dgvOrderInvoices.Font, FontStyle.Bold);
                    }
                    else if (status == "Pending")
                    {
                        e.CellStyle.ForeColor = Color.Orange;
                    }
                }

                // Format amount column as currency
                if (e.ColumnIndex == dgvOrderInvoices.Columns["Amount"].Index && e.Value != null)
                {
                    if (decimal.TryParse(e.Value.ToString().Replace("$", "").Replace(",", ""),
                            out decimal amount))
                    {
                        e.Value = amount.ToString("C", CultureInfo.CurrentCulture);
                        e.FormattingApplied = true;
                    }
                }

                // Alternate row coloring for better readability
                if (e.RowIndex % 2 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void DgvCompletedAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCompletedAppointments.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvCompletedAppointments.SelectedRows[0];
                selectedAppointmentId = (int)selectedRow.Cells["AppointmentID"].Value;

                // Parse values, handling currency formatting
                string patientName = selectedRow.Cells["Patient"].Value.ToString();
                decimal totalFee = ParseCurrencyValue(selectedRow.Cells["Fee"].Value.ToString());
                decimal paidAmount = ParseCurrencyValue(selectedRow.Cells["PaidAmount"].Value.ToString());
                decimal remainingAmount = ParseCurrencyValue(selectedRow.Cells["RemainingAmount"].Value.ToString());
                string status = selectedRow.Cells["Status"].Value.ToString();

                // Update UI - simplified to just show appointment number and financial summary
                lblTotalFee.Text = totalFee.ToString("C", CultureInfo.CurrentCulture);
                lblPaidAmount.Text = paidAmount.ToString("C", CultureInfo.CurrentCulture);
                lblRemainingAmount.Text = remainingAmount.ToString("C", CultureInfo.CurrentCulture);

                // Enable or disable payment controls based on remaining amount
                bool canAddPayment = remainingAmount > 0;
                nudPaymentAmount.Enabled = canAddPayment;
                cmbPaymentMethod.Enabled = canAddPayment;
                btnCreateInvoice.Enabled = false; // Will be enabled when a valid amount is entered

                if (canAddPayment)
                {
                    // Set maximum payment amount to remaining balance
                    nudPaymentAmount.Maximum = remainingAmount;

                    // Suggest full remaining amount as default
                    nudPaymentAmount.Value = remainingAmount;
                }
                else
                {
                    // Reset payment amount for fully paid appointments
                    nudPaymentAmount.Value = 0;
                    nudPaymentAmount.Maximum = 0;
                }

                // Load invoices for this appointment
                LoadInvoicesForAppointment(selectedAppointmentId);
            }
            else
            {
                // Clear the details
                ClearDetails();
            }
        }

        private void DgvConfirmedOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvConfirmedOrders.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvConfirmedOrders.SelectedRows[0];
                selectedOrderId = Convert.ToInt32(selectedRow.Cells["OrderID"].Value);

                // Parse values, handling currency formatting
                string patientName = selectedRow.Cells["Patient"].Value.ToString();
                decimal totalAmount = ParseCurrencyValue(selectedRow.Cells["TotalAmount"].Value.ToString());
                decimal paidAmount = ParseCurrencyValue(selectedRow.Cells["PaidAmount"].Value.ToString());
                decimal remainingAmount = ParseCurrencyValue(selectedRow.Cells["RemainingAmount"].Value.ToString());
                string status = selectedRow.Cells["Status"].Value.ToString();

                // Update UI
                lblOrderTotal.Text = totalAmount.ToString("C", CultureInfo.CurrentCulture);
                lblOrderPaid.Text = paidAmount.ToString("C", CultureInfo.CurrentCulture);
                lblOrderRemaining.Text = remainingAmount.ToString("C", CultureInfo.CurrentCulture);

                // Enable or disable payment controls based on remaining amount
                bool canAddPayment = remainingAmount > 0 && status != "Completed";
                nudOrderPaymentAmount.Enabled = canAddPayment;
                cmbOrderPaymentMethod.Enabled = canAddPayment;
                btnCreateOrderInvoice.Enabled = false; // Will be enabled when a valid amount is entered

                if (canAddPayment)
                {
                    // Set maximum payment amount to remaining balance
                    nudOrderPaymentAmount.Maximum = remainingAmount;

                    // Suggest full remaining amount as default
                    nudOrderPaymentAmount.Value = remainingAmount;
                }
                else
                {
                    // Reset payment amount for fully paid orders
                    nudOrderPaymentAmount.Value = 0;
                    nudOrderPaymentAmount.Maximum = 0;
                }

                // Load invoices for this order
                LoadInvoicesForOrder(selectedOrderId);
            }
            else
            {
                // Clear the details
                ClearOrderDetails();
            }
        }

        private void NudPaymentAmount_ValueChanged(object sender, EventArgs e)
        {
            // Enable the create invoice button only if there's a valid payment amount
            bool isValidAmount = nudPaymentAmount.Value > 0 && nudPaymentAmount.Value <= nudPaymentAmount.Maximum;
            btnCreateInvoice.Enabled = isValidAmount;

            // Visual feedback
            if (isValidAmount)
            {
                nudPaymentAmount.BackColor = Color.FromArgb(240, 255, 240); // Light green
            }
            else
            {
                nudPaymentAmount.BackColor = Color.FromArgb(255, 240, 240); // Light red
            }
        }

        private void NudOrderPaymentAmount_ValueChanged(object sender, EventArgs e)
        {
            // Enable the create invoice button only if there's a valid payment amount
            bool isValidAmount = nudOrderPaymentAmount.Value > 0 && nudOrderPaymentAmount.Value <= nudOrderPaymentAmount.Maximum;
            btnCreateOrderInvoice.Enabled = isValidAmount;

            // Visual feedback
            if (isValidAmount)
            {
                nudOrderPaymentAmount.BackColor = Color.FromArgb(240, 255, 240); // Light green
            }
            else
            {
                nudOrderPaymentAmount.BackColor = Color.FromArgb(255, 240, 240); // Light red
            }
        }

        private void BtnCreateInvoice_Click(object sender, EventArgs e)
        {
            if (selectedAppointmentId <= 0 || nudPaymentAmount.Value <= 0)
            {
                UIService.ShowError("Please select an appointment and enter a valid payment amount");
                return;
            }

            try
            {
                // Use repository to create invoice for appointment
                int newInvoiceId = repository.CreateAppointmentInvoice(
                    selectedAppointmentId,
                    nudPaymentAmount.Value,
                    cmbPaymentMethod.SelectedItem.ToString(),
                    AuthService.CurrentUser.UserID);

                // Show success message
                UIService.ShowSuccess($"Payment of {nudPaymentAmount.Value:C} received successfully.\nInvoice #{newInvoiceId} created.");

                // Reset the payment amount
                nudPaymentAmount.Value = 0;

                // Refresh the UI
                LoadCompletedAppointments();

                // Re-select the current appointment
                SelectAppointmentById(selectedAppointmentId);
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error creating invoice: {ex.Message}");
            }
        }

        private void BtnCreateOrderInvoice_Click(object sender, EventArgs e)
        {
            if (selectedOrderId <= 0 || nudOrderPaymentAmount.Value <= 0)
            {
                UIService.ShowError("Please select an order and enter a valid payment amount");
                return;
            }

            try
            {
                // Use repository to create invoice for order
                int newInvoiceId = repository.CreateOrderInvoice(
                    selectedOrderId,
                    nudOrderPaymentAmount.Value,
                    cmbOrderPaymentMethod.SelectedItem.ToString(),
                    AuthService.CurrentUser.UserID);

                // Show success message
                UIService.ShowSuccess($"Payment of {nudOrderPaymentAmount.Value:C} processed successfully.\nOrder invoice #{newInvoiceId} created.");

                // Reset the payment amount
                nudOrderPaymentAmount.Value = 0;

                // Refresh the UI
                LoadConfirmedOrders();

                // Re-select the current order
                SelectOrderById(selectedOrderId);
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error processing payment: {ex.Message}");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            int previouslySelectedAppointmentId = selectedAppointmentId;
            LoadCompletedAppointments();

            // Re-select the previously selected appointment if it still exists
            if (previouslySelectedAppointmentId > 0)
            {
                SelectAppointmentById(previouslySelectedAppointmentId);
            }
        }

        private void BtnRefreshOrders_Click(object sender, EventArgs e)
        {
            int previouslySelectedOrderId = selectedOrderId;
            LoadConfirmedOrders();

            // Re-select the previously selected order if it still exists
            if (previouslySelectedOrderId > 0)
            {
                SelectOrderById(previouslySelectedOrderId);
            }
        }
        #endregion

        #region Helper Methods
        private void LoadCompletedAppointments()
        {
            dgvCompletedAppointments.Rows.Clear();

            // Get completed appointments from repository
            var completedAppointments = repository.GetCompletedAppointmentsWithPaymentDetails();

            foreach (var appointment in completedAppointments)
            {
                // Calculate total paid amount from invoices
                decimal totalPaid = repository.CalculateTotalPaidAmountForAppointment(appointment.AppointmentID);
                decimal remainingAmount = appointment.Fee - totalPaid;

                // Determine payment status
                string paymentStatus;
                if (remainingAmount <= 0)
                    paymentStatus = "Paid";
                else if (totalPaid > 0)
                    paymentStatus = "Partially Paid";
                else
                    paymentStatus = "Unpaid";

                // Add row to grid
                if (appointment.Patient?.User != null && appointment.Audiologist?.User != null)
                {
                    dgvCompletedAppointments.Rows.Add(
                        appointment.AppointmentID,
                        appointment.Date.ToShortDateString(),
                        $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}",
                        $"Dr. {appointment.Audiologist.User.FirstName} {appointment.Audiologist.User.LastName}",
                        appointment.Fee,
                        totalPaid,
                        remainingAmount,
                        paymentStatus
                    );
                }
            }
        }

        private void LoadConfirmedOrders()
        {
            dgvConfirmedOrders.Rows.Clear();

            // Get confirmed orders from repository
            var confirmedOrders = repository.GetConfirmedOrdersWithPaymentDetails();

            foreach (var order in confirmedOrders)
            {
                // Calculate total paid amount from invoices
                decimal totalPaid = repository.CalculateTotalPaidAmountForOrder(order.OrderID);
                decimal remainingAmount = order.TotalAmount - totalPaid;

                // Determine payment status
                string paymentStatus = order.Status;
                if (remainingAmount <= 0 && paymentStatus != "Completed")
                    paymentStatus = "Paid";

                // Get order items with product details to show product names
                var orderItems = repository.GetOrderItemsWithProductDetails(order.OrderID);
                
                // Create a formatted list of product names
                string productsList = "";
                if (orderItems != null && orderItems.Any())
                {
                    var productNames = new List<string>();
                    foreach (var item in orderItems)
                    {
                        if (item.Product != null)
                        {
                            // Format as "Product (Qty)" if quantity > 1, otherwise just "Product"
                            string productEntry = item.Product.Model;
                            if (item.Quantity > 1)
                                productEntry += $" ({item.Quantity})";
                            
                            productNames.Add(productEntry);
                        }
                    }
                    
                    // Join product names with commas
                    productsList = string.Join(", ", productNames);
                }
                else
                {
                    productsList = "(No items)";
                }

                // Add row to grid
                if (order.Patient?.User != null)
                {
                    dgvConfirmedOrders.Rows.Add(
                        order.OrderID,
                        order.OrderDate.ToShortDateString(),
                        $"{order.Patient.User.FirstName} {order.Patient.User.LastName}",
                        order.TotalAmount,
                        totalPaid,
                        remainingAmount,
                        paymentStatus,
                        productsList  // Show product names instead of count
                    );
                }
            }
        }

        private decimal CalculateTotalPaidAmount(int appointmentId)
        {
            // Use repository to calculate total paid amount
            return repository.CalculateTotalPaidAmountForAppointment(appointmentId);
        }

        private decimal CalculateTotalPaidAmountForOrder(int orderId)
        {
            // Use repository to calculate total paid amount
            return repository.CalculateTotalPaidAmountForOrder(orderId);
        }

        private void LoadInvoicesForAppointment(int appointmentId)
        {
            dgvInvoices.Rows.Clear();

            // Get invoices from repository
            var appointmentInvoices = repository.GetInvoicesForAppointment(appointmentId);

            bool hasInvoices = appointmentInvoices.Any();

            // Show/hide the "no invoices" message
            foreach (Control ctrl in pnlInvoiceDetails.Controls)
            {
                if (ctrl is Label label && label.Tag?.ToString() == "NoInvoicesMessage")
                {
                    label.Visible = !hasInvoices;
                    break;
                }
            }

            if (!hasInvoices) return;

            foreach (var invoice in appointmentInvoices)
            {
                // Get user who created the invoice (from payment)
                string createdBy = "Unknown";

                // Use Payment (singular) instead of Payments (plural)
                if (invoice.Payment != null && invoice.CreatedByUser != null)
                {
                    createdBy = $"{invoice.CreatedByUser.FirstName} {invoice.CreatedByUser.LastName}";
                }

                dgvInvoices.Rows.Add(
                    invoice.Invoice.InvoiceID,
                    invoice.Invoice.InvoiceDate.ToShortDateString(),
                    invoice.Invoice.TotalAmount,
                    invoice.Invoice.PaymentMethod ?? "-",
                    invoice.Invoice.Status,
                    createdBy
                );
            }
        }

        private void LoadInvoicesForOrder(int orderId)
        {
            dgvOrderInvoices.Rows.Clear();

            // Get invoices from repository
            var orderInvoices = repository.GetInvoicesForOrder(orderId);

            bool hasInvoices = orderInvoices.Any();

            // Show/hide the "no invoices" message
            foreach (Control ctrl in pnlOrderInvoiceDetails.Controls)
            {
                if (ctrl is Label label && label.Tag?.ToString() == "NoOrderInvoicesMessage")
                {
                    label.Visible = !hasInvoices;
                    break;
                }
            }

            if (!hasInvoices) return;

            foreach (var invoice in orderInvoices)
            {
                // Get user who created the invoice (from payment)
                string createdBy = "Unknown";

                // Use Payment (singular) instead of Payments (plural)
                if (invoice.Payment != null && invoice.CreatedByUser != null)
                {
                    createdBy = $"{invoice.CreatedByUser.FirstName} {invoice.CreatedByUser.LastName}";
                }

                dgvOrderInvoices.Rows.Add(
                    invoice.Invoice.InvoiceID,
                    invoice.Invoice.InvoiceDate.ToShortDateString(),
                    invoice.Invoice.TotalAmount,
                    invoice.Invoice.PaymentMethod ?? "-",
                    invoice.Invoice.Status,
                    createdBy
                );
            }
        }

        private void SelectAppointmentById(int appointmentId)
        {
            foreach (DataGridViewRow row in dgvCompletedAppointments.Rows)
            {
                if ((int)row.Cells["AppointmentID"].Value == appointmentId)
                {
                    dgvCompletedAppointments.ClearSelection();
                    row.Selected = true;
                    dgvCompletedAppointments.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private void SelectOrderById(int orderId)
        {
            foreach (DataGridViewRow row in dgvConfirmedOrders.Rows)
            {
                if (Convert.ToInt32(row.Cells["OrderID"].Value) == orderId)
                {
                    dgvConfirmedOrders.ClearSelection();
                    row.Selected = true;
                    dgvConfirmedOrders.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private decimal ParseCurrencyValue(string value)
        {
            // Remove currency symbol and commas, then parse
            string cleanValue = value.Replace("$", "").Replace(",", "");
            decimal.TryParse(cleanValue, out decimal result);
            return result;
        }

        private void ClearDetails()
        {
            selectedAppointmentId = -1;
            lblTotalFee.Text = "$0.00";
            lblPaidAmount.Text = "$0.00";
            lblRemainingAmount.Text = "$0.00";

            // Disable payment controls
            nudPaymentAmount.Enabled = false;
            cmbPaymentMethod.Enabled = false;
            btnCreateInvoice.Enabled = false;

            // Clear the invoices grid
            dgvInvoices.Rows.Clear();

            // Show the "no invoices" message
            foreach (Control ctrl in pnlInvoiceDetails.Controls)
            {
                if (ctrl is Label label && label.Tag?.ToString() == "NoInvoicesMessage")
                {
                    label.Visible = true;
                    break;
                }
            }
        }

        private void ClearOrderDetails()
        {
            selectedOrderId = -1;
            lblOrderTotal.Text = "$0.00";
            lblOrderPaid.Text = "$0.00";
            lblOrderRemaining.Text = "$0.00";

            // Disable payment controls
            nudOrderPaymentAmount.Enabled = false;
            cmbOrderPaymentMethod.Enabled = false;
            btnCreateOrderInvoice.Enabled = false;

            // Clear the invoices grid
            dgvOrderInvoices.Rows.Clear();

            // Show the "no invoices" message
            foreach (Control ctrl in pnlOrderInvoiceDetails.Controls)
            {
                if (ctrl is Label label && label.Tag?.ToString() == "NoOrderInvoicesMessage")
                {
                    label.Visible = true;
                    break;
                }
            }
        }
        #endregion
    }
}