using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;
using HearingClinicManagementSystem.UI.Constants;

namespace HearingClinicManagementSystem.UI.ClinicManager
{
    public class OrderManagementForm : BaseForm
    {
        #region Fields
        private DataGridView dgvPendingOrders;
        private DataGridView dgvOrderDetails;
        private Panel pnlOrderDetails;
        private Panel pnlConfirmation;
        private Button btnRefresh;
        private Button btnConfirmOrder;
        private Button btnRejectOrder;
        private Label lblOrderStatus;
        private Label lblOrderDate;
        private Label lblCustomerInfo;
        private Label lblTotalAmount;

        private int? selectedOrderId = null;
        private Order selectedOrder = null;
        #endregion

        public OrderManagementForm()
        {
            InitializeComponents();
            LoadPendingOrders();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Order Management";
            this.Size = new Size(1200, 800);

            // Create title
            var lblTitle = CreateTitleLabel("Incoming Orders Management");
            lblTitle.Dock = DockStyle.Top;

            // Main layout with 2 columns
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(10),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };

            // Configure column and row styles
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));  // Orders list (60%)
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));  // Order details (40%)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));        // Upper section
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));        // Lower section

            // Create the panels
            var ordersPanel = CreateOrdersPanel();             // Pending orders grid
            pnlOrderDetails = CreateOrderDetailsPanel();       // Order details panel
            pnlConfirmation = CreateConfirmationPanel();       // Order confirmation panel
            var orderItemsPanel = CreateOrderItemsPanel();     // Order items grid

            // Add panels to the main layout
            mainLayout.Controls.Add(ordersPanel, 0, 0);
            mainLayout.Controls.Add(pnlOrderDetails, 1, 0);
            mainLayout.Controls.Add(orderItemsPanel, 0, 1);
            mainLayout.Controls.Add(pnlConfirmation, 1, 1);

            // Add all controls to the form
            Controls.Add(mainLayout);
            Controls.Add(lblTitle);
        }

        private Panel CreateOrdersPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Create panel header with title and refresh button
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 10, 5)
            };

            Label lblHeader = new Label
            {
                Text = "Incoming Orders",
                Font = new Font(DefaultFont.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 15),
                AutoSize = true
            };

            btnRefresh = new Button
            {
                Text = "Refresh",
                Size = new Size(100, 30),
                Location = new Point(headerPanel.Width - 120, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefresh.Click += BtnRefresh_Click;
            ApplyButtonStyle(btnRefresh, Color.FromArgb(108, 117, 125));

            headerPanel.Controls.Add(lblHeader);
            headerPanel.Controls.Add(btnRefresh);

            // Create orders grid
            dgvPendingOrders = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Set up columns
            dgvPendingOrders.Columns.Add("OrderID", "Order #");
            dgvPendingOrders.Columns.Add("OrderDate", "Date");
            dgvPendingOrders.Columns.Add("CustomerName", "Customer");
            dgvPendingOrders.Columns.Add("TotalAmount", "Total");
            dgvPendingOrders.Columns.Add("Status", "Status");
            dgvPendingOrders.Columns.Add("ItemCount", "Items");

            // Set column widths as percentages
            dgvPendingOrders.Columns["OrderID"].FillWeight = 10;
            dgvPendingOrders.Columns["OrderDate"].FillWeight = 15;
            dgvPendingOrders.Columns["CustomerName"].FillWeight = 30;
            dgvPendingOrders.Columns["TotalAmount"].FillWeight = 15;
            dgvPendingOrders.Columns["Status"].FillWeight = 15;
            dgvPendingOrders.Columns["ItemCount"].FillWeight = 10;

            // Style the grid header
            dgvPendingOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvPendingOrders.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPendingOrders.Font, FontStyle.Bold);
            dgvPendingOrders.ColumnHeadersHeight = 35;
            dgvPendingOrders.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Attach selection changed event
            dgvPendingOrders.SelectionChanged += DgvPendingOrders_SelectionChanged;
            dgvPendingOrders.CellFormatting += DgvPendingOrders_CellFormatting;

            // Add header and grid to panel
            panel.Controls.Add(dgvPendingOrders);
            panel.Controls.Add(headerPanel);

            return panel;
        }

        private Panel CreateOrderDetailsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Create panel header
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 10, 5)
            };

            Label lblHeader = new Label
            {
                Text = "Order Details",
                Font = new Font(DefaultFont.FontFamily, 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);

            // Create details layout
            Panel detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            // Order status
            lblOrderStatus = new Label
            {
                Location = new Point(15, 15),
                Size = new Size(detailsPanel.Width - 30, 25),
                Font = new Font(DefaultFont.FontFamily, 10, FontStyle.Bold),
                Text = "Status: N/A",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Order date
            lblOrderDate = new Label
            {
                Location = new Point(15, 45),
                Size = new Size(detailsPanel.Width - 30, 25),
                Text = "Order Date: N/A",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Customer info
            lblCustomerInfo = new Label
            {
                Location = new Point(15, 75),
                Size = new Size(detailsPanel.Width - 30, 25),
                Text = "Customer: N/A",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Total amount
            lblTotalAmount = new Label
            {
                Location = new Point(15, 105),
                Size = new Size(detailsPanel.Width - 30, 25),
                Font = new Font(DefaultFont.FontFamily, 10, FontStyle.Bold),
                Text = "Total: $0.00",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Add order details to panel
            detailsPanel.Controls.Add(lblOrderStatus);
            detailsPanel.Controls.Add(lblOrderDate);
            detailsPanel.Controls.Add(lblCustomerInfo);
            detailsPanel.Controls.Add(lblTotalAmount);

            panel.Controls.Add(detailsPanel);
            panel.Controls.Add(headerPanel);

            return panel;
        }

        private Panel CreateConfirmationPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Create panel header
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 10, 5)
            };

            Label lblHeader = new Label
            {
                Text = "Order Processing",
                Font = new Font(DefaultFont.FontFamily, 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);

            // Create action panel
            Panel actionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            // Update status label
            Label lblUpdateStatus = new Label
            {
                Location = new Point(15, 15),
                Size = new Size(100, 25),
                Text = "Update Status:",
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Buttons for confirm/reject
            btnConfirmOrder = new Button
            {
                Text = "Confirm Order",
                Size = new Size(150, 35),
                Location = new Point(actionPanel.Width - 320, actionPanel.Height - 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Enabled = false
            };
            btnConfirmOrder.Click += BtnConfirmOrder_Click;
            ApplyButtonStyle(btnConfirmOrder, Color.FromArgb(40, 167, 69));

            btnRejectOrder = new Button
            {
                Text = "Reject Order",
                Size = new Size(150, 35),
                Location = new Point(actionPanel.Width - 160, actionPanel.Height - 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Enabled = false
            };
            btnRejectOrder.Click += BtnRejectOrder_Click;
            ApplyButtonStyle(btnRejectOrder, Color.FromArgb(220, 53, 69));

            // Add controls to action panel
            actionPanel.Controls.Add(lblUpdateStatus);
            actionPanel.Controls.Add(btnConfirmOrder);
            actionPanel.Controls.Add(btnRejectOrder);

            panel.Controls.Add(actionPanel);
            panel.Controls.Add(headerPanel);

            return panel;
        }

        private Panel CreateOrderItemsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Create panel header
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 10, 5)
            };

            Label lblHeader = new Label
            {
                Text = "Order Items",
                Font = new Font(DefaultFont.FontFamily, 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);

            // Create order items grid
            dgvOrderDetails = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Set up columns
            dgvOrderDetails.Columns.Add("ProductID", "Product ID");
            dgvOrderDetails.Columns.Add("ProductName", "Product");
            dgvOrderDetails.Columns.Add("Quantity", "Qty");
            dgvOrderDetails.Columns.Add("UnitPrice", "Unit Price");
            dgvOrderDetails.Columns.Add("Subtotal", "Subtotal");
            dgvOrderDetails.Columns.Add("InStock", "In Stock");

            // Set column widths as percentages
            dgvOrderDetails.Columns["ProductID"].FillWeight = 10;
            dgvOrderDetails.Columns["ProductName"].FillWeight = 35;
            dgvOrderDetails.Columns["Quantity"].FillWeight = 10;
            dgvOrderDetails.Columns["UnitPrice"].FillWeight = 15;
            dgvOrderDetails.Columns["Subtotal"].FillWeight = 15;
            dgvOrderDetails.Columns["InStock"].FillWeight = 15;

            // Style the grid header
            dgvOrderDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvOrderDetails.ColumnHeadersDefaultCellStyle.Font = new Font(dgvOrderDetails.Font, FontStyle.Bold);
            dgvOrderDetails.ColumnHeadersHeight = 35;
            dgvOrderDetails.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgvOrderDetails.CellFormatting += DgvOrderDetails_CellFormatting;

            // Add header and grid to panel
            panel.Controls.Add(dgvOrderDetails);
            panel.Controls.Add(headerPanel);

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

            button.MouseEnter += (s, e) => {
                button.BackColor = ControlPaint.Dark(baseColor);
            };
            button.MouseLeave += (s, e) => {
                button.BackColor = baseColor;
            };
        }
        #endregion

        #region Event Handlers
        private void DgvPendingOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPendingOrders.SelectedRows.Count > 0)
            {
                var selectedRow = dgvPendingOrders.SelectedRows[0];
                selectedOrderId = (int)selectedRow.Cells["OrderID"].Value;

                // Load order details
                selectedOrder = StaticDataProvider.Orders.FirstOrDefault(o => o.OrderID == selectedOrderId);
                if (selectedOrder != null)
                {
                    LoadOrderDetails(selectedOrder);
                    LoadOrderItems(selectedOrder);

                    // Enable/disable buttons based on order status
                    bool isPending = selectedOrder.Status == "Pending";
                    btnConfirmOrder.Enabled = isPending;
                    btnRejectOrder.Enabled = isPending;
                }
            }
            else
            {
                ClearOrderDetails();
                selectedOrderId = null;
                selectedOrder = null;
                btnConfirmOrder.Enabled = false;
                btnRejectOrder.Enabled = false;
            }
        }

        private void DgvPendingOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPendingOrders.Rows[e.RowIndex];

                // Format total amount as currency
                if (e.ColumnIndex == dgvPendingOrders.Columns["TotalAmount"].Index && e.Value != null)
                {
                    if (decimal.TryParse(e.Value.ToString(), out decimal total))
                    {
                        e.Value = string.Format("${0:N2}", total);
                        e.FormattingApplied = true;
                    }
                }

                // Format status with color
                if (e.ColumnIndex == dgvPendingOrders.Columns["Status"].Index && e.Value != null)
                {
                    string status = e.Value.ToString();
                    switch (status)
                    {
                        case "Pending":
                            e.CellStyle.ForeColor = Color.DarkOrange;
                            break;
                        case "Confirmed":
                            e.CellStyle.ForeColor = Color.Blue;
                            break;
                        case "Processing":
                            e.CellStyle.ForeColor = Color.Purple;
                            break;
                        case "Completed":
                            e.CellStyle.ForeColor = Color.Green;
                            break;
                        case "Cancelled":
                            e.CellStyle.ForeColor = Color.Red;
                            break;
                    }
                    e.CellStyle.Font = new Font(dgvPendingOrders.Font, FontStyle.Bold);
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

        private void DgvOrderDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Format price columns as currency
                if ((e.ColumnIndex == dgvOrderDetails.Columns["UnitPrice"].Index ||
                     e.ColumnIndex == dgvOrderDetails.Columns["Subtotal"].Index) &&
                    e.Value != null)
                {
                    if (decimal.TryParse(e.Value.ToString(), out decimal amount))
                    {
                        e.Value = string.Format("${0:N2}", amount);
                        e.FormattingApplied = true;
                    }
                }

                // Highlight if stock is low
                if (e.ColumnIndex == dgvOrderDetails.Columns["InStock"].Index && e.Value != null)
                {
                    if (int.TryParse(e.Value.ToString(), out int stock))
                    {
                        if (stock <= 5)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                            e.CellStyle.Font = new Font(dgvOrderDetails.Font, FontStyle.Bold);
                        }
                    }
                }

                // Alternate row coloring
                DataGridViewRow row = dgvOrderDetails.Rows[e.RowIndex];
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

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadPendingOrders();
        }

        private void BtnConfirmOrder_Click(object sender, EventArgs e)
        {
            if (!selectedOrderId.HasValue || selectedOrder == null)
            {
                UIService.ShowWarning("Please select an order first.");
                return;
            }

            if (selectedOrder.Status != "Pending")
            {
                UIService.ShowWarning("Only pending orders can be confirmed.");
                return;
            }

            // Check if all items are in stock
            bool allItemsInStock = true;
            var orderItems = StaticDataProvider.OrderItems.Where(oi => oi.OrderID == selectedOrder.OrderID).ToList();

            foreach (var item in orderItems)
            {
                var product = StaticDataProvider.Products.FirstOrDefault(p => p.ProductID == item.ProductID);
                if (product != null && product.QuantityInStock < item.Quantity)
                {
                    allItemsInStock = false;
                    UIService.ShowWarning($"Not enough stock for {product.Manufacturer} {product.Model}. " +
                                          $"Required: {item.Quantity}, Available: {product.QuantityInStock}");
                    return;
                }
            }

            if (allItemsInStock)
            {
                // Confirm the dialog with the user
                string message = "Confirm this order? This will:\n" +
                                 "- Update the order status\n" +
                                 "- Adjust inventory for all ordered products\n" +
                                 "- Create inventory transactions\n\n" +
                                 "This action cannot be undone.";

                if (UIService.ShowQuestion(message) == DialogResult.Yes)
                {
                    try
                    {
                        // Update order status
                        selectedOrder.Status = "Confirmed";

                        // Process inventory changes
                        foreach (var item in orderItems)
                        {
                            var product = StaticDataProvider.Products.FirstOrDefault(p => p.ProductID == item.ProductID);
                            if (product != null)
                            {
                                // Reduce inventory
                                product.QuantityInStock -= item.Quantity;

                                // Record transaction
                                RecordInventoryTransaction(product.ProductID, item.Quantity, $"Sale for Order #{selectedOrder.OrderID}", "Sale");
                            }
                        }

                        // Update ProcessedBy field
                        selectedOrder.ProcessedBy = AuthService.CurrentUser.UserID;

                        // Success message
                        UIService.ShowSuccess("Order confirmed successfully. Inventory has been updated.");

                        // Reload data
                        LoadPendingOrders();

                        // Clear selection
                        ClearOrderDetails();
                        selectedOrderId = null;
                        selectedOrder = null;
                    }
                    catch (Exception ex)
                    {
                        UIService.ShowError($"Error confirming order: {ex.Message}");
                    }
                }
            }
        }

        private void BtnRejectOrder_Click(object sender, EventArgs e)
        {
            if (!selectedOrderId.HasValue || selectedOrder == null)
            {
                UIService.ShowWarning("Please select an order first.");
                return;
            }

            if (selectedOrder.Status != "Pending")
            {
                UIService.ShowWarning("Only pending orders can be rejected.");
                return;
            }

            // Confirm with the user
            string message = "Are you sure you want to reject this order? This will mark it as cancelled.";
            if (UIService.ShowQuestion(message) == DialogResult.Yes)
            {
                try
                {
                    // Update order status
                    selectedOrder.Status = "Cancelled";

                    // Update ProcessedBy field
                    selectedOrder.ProcessedBy = AuthService.CurrentUser.UserID;

                    // Success message
                    UIService.ShowSuccess("Order has been cancelled.");

                    // Reload data
                    LoadPendingOrders();

                    // Clear selection
                    ClearOrderDetails();
                    selectedOrderId = null;
                    selectedOrder = null;
                }
                catch (Exception ex)
                {
                    UIService.ShowError($"Error cancelling order: {ex.Message}");
                }
            }
        }
        #endregion

        #region Helper Methods
        private void LoadPendingOrders()
        {
            dgvPendingOrders.Rows.Clear();

            // Get orders that need processing (focus on pending, but show all for tracking)
            var orders = StaticDataProvider.Orders
                .OrderByDescending(o => o.Status == "Pending") // Put pending orders first
                .ThenByDescending(o => o.OrderDate)
                .ToList();

            foreach (var order in orders)
            {
                // Find customer name
                string customerName = "Unknown";
                var patient = StaticDataProvider.Patients.FirstOrDefault(p => p.PatientID == order.PatientID);
                if (patient != null && patient.User != null)
                {
                    customerName = $"{patient.User.FirstName} {patient.User.LastName}";
                }

                // Count items
                int itemCount = StaticDataProvider.OrderItems
                    .Count(oi => oi.OrderID == order.OrderID);

                // Calculate total amount
                decimal totalAmount = order.TotalAmount;

                dgvPendingOrders.Rows.Add(
                    order.OrderID,
                    order.OrderDate.ToShortDateString(),
                    customerName,
                    totalAmount,
                    order.Status,
                    itemCount
                );

                // Highlight pending orders
                if (order.Status == "Pending")
                {
                    dgvPendingOrders.Rows[dgvPendingOrders.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(255, 252, 230);
                }
            }

            // Reset selection
            if (selectedOrderId.HasValue)
            {
                foreach (DataGridViewRow row in dgvPendingOrders.Rows)
                {
                    if ((int)row.Cells["OrderID"].Value == selectedOrderId.Value)
                    {
                        row.Selected = true;
                        dgvPendingOrders.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }
            else if (dgvPendingOrders.Rows.Count > 0)
            {
                dgvPendingOrders.Rows[0].Selected = true;
            }
        }

        private void LoadOrderDetails(Order order)
        {
            if (order == null)
            {
                ClearOrderDetails();
                return;
            }

            // Find customer name
            string customerName = "Unknown";
            var patient = StaticDataProvider.Patients.FirstOrDefault(p => p.PatientID == order.PatientID);
            if (patient != null && patient.User != null)
            {
                customerName = $"{patient.User.FirstName} {patient.User.LastName}";
            }

            // Calculate total amount - use directly from order model
            decimal totalAmount = order.TotalAmount;

            // Update the display fields
            lblOrderStatus.Text = $"Status: {order.Status}";
            lblOrderStatus.ForeColor = GetStatusColor(order.Status);

            lblOrderDate.Text = $"Order Date: {order.OrderDate.ToShortDateString()}";
            lblCustomerInfo.Text = $"Customer: {customerName}";

            if (order.DeliveryAddress != null)
            {
                lblCustomerInfo.Text += $" | Address: {order.DeliveryAddress}";
            }

            lblTotalAmount.Text = $"Total: ${totalAmount:N2}";

        }

        private void LoadOrderItems(Order order)
        {
            dgvOrderDetails.Rows.Clear();

            if (order == null) return;

            var orderItems = StaticDataProvider.OrderItems
                .Where(oi => oi.OrderID == order.OrderID)
                .ToList();

            foreach (var item in orderItems)
            {
                var product = StaticDataProvider.Products.FirstOrDefault(p => p.ProductID == item.ProductID);
                string productName = product != null ? $"{product.Manufacturer} {product.Model}" : "Unknown Product";
                int inStock = product != null ? product.QuantityInStock : 0;
                decimal subtotal = item.Quantity * item.UnitPrice;

                dgvOrderDetails.Rows.Add(
                    item.ProductID,
                    productName,
                    item.Quantity,
                    item.UnitPrice,
                    subtotal,
                    inStock
                );

                // Highlight if stock is not enough
                if (product != null && product.QuantityInStock < item.Quantity)
                {
                    dgvOrderDetails.Rows[dgvOrderDetails.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(255, 232, 232);
                }
            }
        }

        private void ClearOrderDetails()
        {
            lblOrderStatus.Text = "Status: N/A";
            lblOrderDate.Text = "Order Date: N/A";
            lblCustomerInfo.Text = "Customer: N/A";
            lblTotalAmount.Text = "Total: $0.00";

            dgvOrderDetails.Rows.Clear();

            btnConfirmOrder.Enabled = false;
            btnRejectOrder.Enabled = false;
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "Pending":
                    return Color.DarkOrange;
                case "Confirmed":
                    return Color.Blue;
                case "Processing":
                    return Color.Purple;
                case "Completed":
                    return Color.Green;
                case "Cancelled":
                    return Color.Red;
                default:
                    return Color.Black;
            }
        }

        private void RecordInventoryTransaction(int productId, int quantity, string reason, string transactionType)
        {
            try
            {
                // Create new inventory transaction
                int newTransactionId = StaticDataProvider.InventoryTransactions.Count > 0 ?
                    StaticDataProvider.InventoryTransactions.Max(t => t.TransactionID) + 1 : 1;

                // Create the transaction using the right model
                var transaction = new InventoryTransaction
                {
                    TransactionID = newTransactionId,
                    ProductID = productId,
                    TransactionType = transactionType, // "Sale", "Restock", "Adjustment"
                    Quantity = quantity,
                    TransactionDate = DateTime.Now,
                    ProcessedBy = AuthService.CurrentUser.UserID
                };

                // Add to data store
                StaticDataProvider.InventoryTransactions.Add(transaction);
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error recording inventory transaction: {ex.Message}");
            }
        }
        #endregion
    }
}