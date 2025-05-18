
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
        //private Label lblSelectedAppointment;
        private Label lblTotalFee;
        private Label lblPaidAmount;
        private Label lblRemainingAmount;
        private NumericUpDown nudPaymentAmount;
        private ComboBox cmbPaymentMethod;
        private Button btnCreateInvoice;
        private Button btnRefresh;
        private TableLayoutPanel mainLayout;
        private int selectedAppointmentId = -1;
        #endregion

        public PaymentCollectionForm()
        {
            InitializeComponents();
            LoadCompletedAppointments();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Payment Collection";
            this.Size = new Size(1200, 700);

            var lblTitle = CreateTitleLabel("Payment Collection");
            lblTitle.Dock = DockStyle.Top;

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

            Controls.Add(mainLayout);
            Controls.Add(lblTitle);
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
            //optionsCard.Controls.Add(lblSelectedAppointment);

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
                //lblSelectedAppointment.Text = $"Appointment #{selectedAppointmentId} - Patient: {patientName}";
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

        private void BtnCreateInvoice_Click(object sender, EventArgs e)
        {
            if (selectedAppointmentId <= 0 || nudPaymentAmount.Value <= 0)
            {
                UIService.ShowError("Please select an appointment and enter a valid payment amount");
                return;
            }

            try
            {
                // Create a new invoice for the appointment
                int newInvoiceId = StaticDataProvider.Invoices.Count > 0 ?
                    StaticDataProvider.Invoices.Max(i => i.InvoiceID) + 1 : 1;

                var newInvoice = new Invoice
                {
                    InvoiceID = newInvoiceId,
                    AppointmentID = selectedAppointmentId,
                    OrderID = null,
                    InvoiceDate = DateTime.Now,
                    TotalAmount = nudPaymentAmount.Value,
                    Status = "Paid", // Mark as paid immediately since this is a direct payment
                    PaymentMethod = cmbPaymentMethod.SelectedItem.ToString()
                };

                // Add the invoice to the data store
                StaticDataProvider.Invoices.Add(newInvoice);

                // Create a payment record for this invoice
                int newPaymentId = StaticDataProvider.Payments.Count > 0 ?
                    StaticDataProvider.Payments.Max(p => p.PaymentID) + 1 : 1;

                var newPayment = new Payment
                {
                    PaymentID = newPaymentId,
                    InvoiceID = newInvoiceId,
                    Amount = nudPaymentAmount.Value,
                    PaymentDate = DateTime.Now,
                    ReceivedBy = AuthService.CurrentUser.UserID,
                    PaymentMethod = cmbPaymentMethod.SelectedItem.ToString()
                };

                // Add the payment to the data store
                StaticDataProvider.Payments.Add(newPayment);

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
        #endregion

        #region Helper Methods
        private void LoadCompletedAppointments()
        {
            dgvCompletedAppointments.Rows.Clear();

            var completedAppointments = StaticDataProvider.Appointments
                .Where(a => a.Status == "Completed")
                .OrderByDescending(a => a.Date)
                .ToList();

            foreach (var appointment in completedAppointments)
            {
                // Get related data
                var patient = StaticDataProvider.Patients.FirstOrDefault(p => p.PatientID == appointment.PatientID);
                var audiologist = StaticDataProvider.Audiologists.FirstOrDefault(a => a.AudiologistID == appointment.AudiologistID);

                // Calculate total paid amount from invoices
                decimal totalPaid = CalculateTotalPaidAmount(appointment.AppointmentID);
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
                if (patient?.User != null && audiologist?.User != null)
                {
                    dgvCompletedAppointments.Rows.Add(
                        appointment.AppointmentID,
                        appointment.Date.ToShortDateString(),
                        $"{patient.User.FirstName} {patient.User.LastName}",
                        $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}",
                        appointment.Fee,
                        totalPaid,
                        remainingAmount,
                        paymentStatus
                    );
                }
            }
        }

        private void ClearDetails()
        {
            selectedAppointmentId = -1;
            //lblSelectedAppointment.Text = "No appointment selected";
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
        private decimal CalculateTotalPaidAmount(int appointmentId)
        {
            // Get all invoices for this appointment
            var invoices = StaticDataProvider.Invoices
                .Where(i => i.AppointmentID == appointmentId && i.Status == "Paid")
                .ToList();

            // Sum up the total amount paid
            return invoices.Sum(i => i.TotalAmount);
        }

        private void LoadInvoicesForAppointment(int appointmentId)
        {
            dgvInvoices.Rows.Clear();

            var appointmentInvoices = StaticDataProvider.Invoices
                .Where(i => i.AppointmentID == appointmentId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

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

                var payment = StaticDataProvider.Payments.FirstOrDefault(p => p.InvoiceID == invoice.InvoiceID);
                if (payment != null)
                {
                    var user = StaticDataProvider.Users.FirstOrDefault(u => u.UserID == payment.ReceivedBy);
                    if (user != null)
                    {
                        createdBy = $"{user.FirstName} {user.LastName}";
                    }
                }

                dgvInvoices.Rows.Add(
                    invoice.InvoiceID,
                    invoice.InvoiceDate.ToShortDateString(),
                    invoice.TotalAmount,
                    invoice.PaymentMethod ?? "-",
                    invoice.Status,
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

        private decimal ParseCurrencyValue(string value)
        {
            // Remove currency symbol and commas, then parse
            string cleanValue = value.Replace("$", "").Replace(",", "");
            decimal.TryParse(cleanValue, out decimal result);
            return result;
        }
        #endregion
    }
} 