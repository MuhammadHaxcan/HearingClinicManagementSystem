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
    public class InventoryReportingForm : BaseForm
    {
        #region Fields
        // Main layout controls
        private TabControl tabReports;
        private Button btnRefresh;
        private Label lblLastRefreshed;
        
        // Sales Overview Controls
        private DataGridView dgvTopSellingProducts;
        private DataGridView dgvLowStockProducts;
        private TableLayoutPanel pnlSummaryCards;
        
        // Transaction Analysis Controls
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnApplyDateFilter;
        private ComboBox cmbTransactionType;
        private DataGridView dgvTransactions;
        private DataGridView dgvTransactionSummary;

        // Summary Stats Labels
        private Label lblTotalOrders;
        private Label lblTotalSales;
        private Label lblMostPopularProduct;
        private Label lblAverageOrderValue;
        private Label lblLowStockCount;
        private Label lblTotalProducts;
        
        // Date range for filtering
        private DateTime startDate;
        private DateTime endDate;
        #endregion

        public InventoryReportingForm()
        {
            startDate = DateTime.Now.AddMonths(-3); // Default to last 3 months
            endDate = DateTime.Now;
            
            InitializeComponents();
            LoadAllData();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = "Inventory Reports & Analytics";
            this.Size = new Size(1200, 800);

            // Create title
            var lblTitle = CreateTitleLabel("Inventory Reporting Dashboard");
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
            TabPage tabSalesOverview = new TabPage("Sales Overview");
            TabPage tabTransactionAnalysis = new TabPage("Transaction Analysis");

            // Create sales overview tab
            tabSalesOverview.Controls.Add(CreateSalesOverviewPanel());

            // Create transaction analysis tab
            tabTransactionAnalysis.Controls.Add(CreateTransactionAnalysisPanel());

            // Add tabs to tab control
            tabReports.Controls.Add(tabSalesOverview);
            tabReports.Controls.Add(tabTransactionAnalysis);

            // Add all controls to form
            Controls.Add(tabReports);
            Controls.Add(topPanel);
            Controls.Add(lblTitle);
        }

        private Panel CreateSalesOverviewPanel()
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
            Panel pnlTotalOrders = CreateSummaryPanel("Total Orders", out lblTotalOrders);
            Panel pnlTotalSales = CreateSummaryPanel("Total Sales", out lblTotalSales);
            Panel pnlMostPopular = CreateSummaryPanel("Most Popular Product", out lblMostPopularProduct);
            Panel pnlAvgOrderValue = CreateSummaryPanel("Average Order Value", out lblAverageOrderValue);
            Panel pnlLowStock = CreateSummaryPanel("Low Stock Items", out lblLowStockCount);
            Panel pnlTotalProducts = CreateSummaryPanel("Total Products", out lblTotalProducts);

            // Add panels to the layout
            pnlSummaryCards.Controls.Add(pnlTotalOrders, 0, 0);
            pnlSummaryCards.Controls.Add(pnlTotalSales, 1, 0);
            pnlSummaryCards.Controls.Add(pnlMostPopular, 2, 0);
            pnlSummaryCards.Controls.Add(pnlAvgOrderValue, 0, 1);
            pnlSummaryCards.Controls.Add(pnlLowStock, 1, 1);
            pnlSummaryCards.Controls.Add(pnlTotalProducts, 2, 1);

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
            Panel pnlTopSellingProducts = CreatePanelWithTitle("Top Selling Products", out Panel topSellingContainer);
            Panel pnlLowStockProducts = CreatePanelWithTitle("Low Stock Products", out Panel lowStockContainer);

            // Create top selling products grid
            dgvTopSellingProducts = new DataGridView
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
            ConfigureDataGridView(dgvTopSellingProducts);
            topSellingContainer.Controls.Add(dgvTopSellingProducts);

            // Create low stock products grid
            dgvLowStockProducts = new DataGridView
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
            ConfigureDataGridView(dgvLowStockProducts);
            lowStockContainer.Controls.Add(dgvLowStockProducts);

            // Add grids to layout
            contentLayout.Controls.Add(pnlTopSellingProducts, 0, 0);
            contentLayout.Controls.Add(pnlLowStockProducts, 0, 1);

            // Add everything to the main panel
            panel.Controls.Add(contentLayout);
            panel.Controls.Add(pnlSummaryCards);

            return panel;
        }

        private Panel CreateTransactionAnalysisPanel()
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

            // Transaction type filter
            Label lblTransactionType = new Label
            {
                Text = "Transaction Type:",
                Location = new Point(350, 15),
                AutoSize = true
            };

            cmbTransactionType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(460, 13),
                Size = new Size(120, 20)
            };
            cmbTransactionType.Items.AddRange(new object[] { "All", "Sale", "Restock", "Adjustment" });
            cmbTransactionType.SelectedIndex = 0;

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
            filterPanel.Controls.Add(lblTransactionType);
            filterPanel.Controls.Add(cmbTransactionType);
            filterPanel.Controls.Add(btnApplyDateFilter);

            // Create main content layout
            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };

            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));

            // Create panels for grids
            Panel pnlTransactionSummary = CreatePanelWithTitle("Transaction Summary by Type", out Panel summaryContainer);
            Panel pnlTransactionHistory = CreatePanelWithTitle("Transaction History", out Panel historyContainer);

            // Create transaction summary grid
            dgvTransactionSummary = new DataGridView
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
            ConfigureDataGridView(dgvTransactionSummary);
            summaryContainer.Controls.Add(dgvTransactionSummary);

            // Create transactions grid
            dgvTransactions = new DataGridView
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
            ConfigureDataGridView(dgvTransactions);
            historyContainer.Controls.Add(dgvTransactions);

            // Add grids to layout
            contentLayout.Controls.Add(pnlTransactionSummary, 0, 0);
            contentLayout.Controls.Add(pnlTransactionHistory, 0, 1);

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
                LoadSalesOverviewData();
                LoadTransactionAnalysisData();
                
                lblLastRefreshed.Text = "Last refreshed: " + DateTime.Now.ToString("g");
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error loading report data: {ex.Message}");
            }
        }

        private void LoadSalesOverviewData()
        {
            LoadSummaryStats();
            LoadTopSellingProductsGrid();
            LoadLowStockProductsGrid();
        }

        private void LoadTransactionAnalysisData()
        {
            LoadTransactionSummary();
            LoadTransactionsGrid();
        }

        private void LoadSummaryStats()
        {
            // Get all confirmed orders
            var confirmedOrders = StaticDataProvider.Orders.Where(o => o.Status == "Confirmed").ToList();
            
            // Total Orders
            int totalOrders = confirmedOrders.Count;
            lblTotalOrders.Text = totalOrders.ToString();
            
            // Total Sales Amount
            decimal totalSales = confirmedOrders.Sum(o => o.TotalAmount);
            lblTotalSales.Text = totalSales.ToString("C2");
            
            // Average Order Value
            decimal avgOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;
            lblAverageOrderValue.Text = avgOrderValue.ToString("C2");
            
            // Most Popular Product
            var productSales = new Dictionary<int, int>();
            foreach (var order in confirmedOrders)
            {
                var orderItems = StaticDataProvider.OrderItems.Where(oi => oi.OrderID == order.OrderID);
                foreach (var item in orderItems)
                {
                    if (productSales.ContainsKey(item.ProductID))
                        productSales[item.ProductID] += item.Quantity;
                    else
                        productSales[item.ProductID] = item.Quantity;
                }
            }
            
            if (productSales.Any())
            {
                var topProduct = productSales.OrderByDescending(kv => kv.Value).First();
                var product = StaticDataProvider.Products.FirstOrDefault(p => p.ProductID == topProduct.Key);
                string productName = product != null ? 
                    $"{product.Manufacturer} {product.Model}" : 
                    $"Product #{topProduct.Key}";
                    
                lblMostPopularProduct.Text = $"{productName} ({topProduct.Value} units)";
            }
            else
            {
                lblMostPopularProduct.Text = "No data available";
            }
            
            // Low Stock Count
            int lowStockCount = StaticDataProvider.Products.Count(p => p.QuantityInStock <= 10);
            lblLowStockCount.Text = $"{lowStockCount} items";
            
            // Total Products
            int totalProducts = StaticDataProvider.Products.Count;
            lblTotalProducts.Text = totalProducts.ToString();
        }

        private void LoadTopSellingProductsGrid()
        {
            // Configure grid columns
            dgvTopSellingProducts.Columns.Clear();
            dgvTopSellingProducts.Columns.Add("Rank", "Rank");
            dgvTopSellingProducts.Columns.Add("ProductID", "ID");
            dgvTopSellingProducts.Columns.Add("ProductName", "Product");
            dgvTopSellingProducts.Columns.Add("Manufacturer", "Manufacturer");
            dgvTopSellingProducts.Columns.Add("UnitsSold", "Units Sold");
            dgvTopSellingProducts.Columns.Add("TotalSales", "Total Sales");
            dgvTopSellingProducts.Columns.Add("PercentOfSales", "% of Sales");
            
            // Set column properties
            dgvTopSellingProducts.Columns["Rank"].Width = 40;
            dgvTopSellingProducts.Columns["ProductID"].Width = 40;
            dgvTopSellingProducts.Columns["ProductName"].FillWeight = 80;
            dgvTopSellingProducts.Columns["Manufacturer"].FillWeight = 80;
            dgvTopSellingProducts.Columns["UnitsSold"].Width = 80;
            dgvTopSellingProducts.Columns["TotalSales"].Width = 100;
            dgvTopSellingProducts.Columns["PercentOfSales"].Width = 80;
            
            // Get confirmed orders
            var confirmedOrders = StaticDataProvider.Orders.Where(o => o.Status == "Confirmed").ToList();
            
            // Calculate total units sold and sales by product
            var productStats = new Dictionary<int, (int Units, decimal Sales)>();
            foreach (var order in confirmedOrders)
            {
                var orderItems = StaticDataProvider.OrderItems.Where(oi => oi.OrderID == order.OrderID);
                foreach (var item in orderItems)
                {
                    decimal itemTotal = item.Quantity * item.UnitPrice;
                    if (productStats.ContainsKey(item.ProductID))
                        productStats[item.ProductID] = (
                            productStats[item.ProductID].Units + item.Quantity,
                            productStats[item.ProductID].Sales + itemTotal
                        );
                    else
                        productStats[item.ProductID] = (item.Quantity, itemTotal);
                }
            }
            
            // Calculate total sales for percentage calculation
            decimal totalSales = productStats.Sum(p => p.Value.Sales);
            
            // Sort products by sales and take top 10
            var topProducts = productStats
                .OrderByDescending(p => p.Value.Sales)
                .Take(15) // Show more products
                .ToList();
                
            // Add data rows
            for (int i = 0; i < topProducts.Count; i++)
            {
                var product = StaticDataProvider.Products.FirstOrDefault(p => p.ProductID == topProducts[i].Key);
                if (product == null) continue;
                
                string productName = product.Model;
                string manufacturer = product.Manufacturer;
                
                decimal percentOfSales = totalSales > 0 ? 
                    (topProducts[i].Value.Sales / totalSales) * 100 : 0;
                
                dgvTopSellingProducts.Rows.Add(
                    (i + 1).ToString(),
                    topProducts[i].Key.ToString(),
                    productName,
                    manufacturer,
                    topProducts[i].Value.Units.ToString("N0"),
                    topProducts[i].Value.Sales.ToString("C2"),
                    percentOfSales.ToString("F1") + "%"
                );
                
                // Highlight top 3
                if (i < 3)
                {
                    dgvTopSellingProducts.Rows[i].DefaultCellStyle.Font = new Font(dgvTopSellingProducts.Font, FontStyle.Bold);
                    if (i == 0)
                        dgvTopSellingProducts.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                }
            }
        }

        private void LoadLowStockProductsGrid()
        {
            // Configure grid columns
            dgvLowStockProducts.Columns.Clear();
            dgvLowStockProducts.Columns.Add("ProductID", "ID");
            dgvLowStockProducts.Columns.Add("ProductName", "Product");
            dgvLowStockProducts.Columns.Add("Manufacturer", "Manufacturer");
            dgvLowStockProducts.Columns.Add("CurrentStock", "Current Stock");
            dgvLowStockProducts.Columns.Add("Status", "Status");
            dgvLowStockProducts.Columns.Add("Price", "Price");
            dgvLowStockProducts.Columns.Add("LastRestocked", "Last Restocked");
            
            // Get low stock products (less than or equal to 10 items)
            var lowStockProducts = StaticDataProvider.Products
                .Where(p => p.QuantityInStock <= 10)
                .OrderBy(p => p.QuantityInStock)
                .ToList();
            
            // Get last restock transaction for each product
            var lastRestocks = new Dictionary<int, DateTime?>();
            foreach (var product in lowStockProducts)
            {
                var lastRestock = StaticDataProvider.InventoryTransactions
                    .Where(t => t.ProductID == product.ProductID && t.TransactionType == "Restock")
                    .OrderByDescending(t => t.TransactionDate)
                    .FirstOrDefault();
                
                lastRestocks[product.ProductID] = lastRestock?.TransactionDate;
            }
            
            // Add data rows
            foreach (var product in lowStockProducts)
            {
                string status = GetStockStatusText(product.QuantityInStock);
                string lastRestocked = lastRestocks[product.ProductID].HasValue ? 
                    lastRestocks[product.ProductID].Value.ToShortDateString() : "Never";
                    
                var row = dgvLowStockProducts.Rows.Add(
                    product.ProductID.ToString(),
                    product.Model,
                    product.Manufacturer,
                    product.QuantityInStock.ToString(),
                    status,
                    product.Price.ToString("C2"),
                    lastRestocked
                );
                
                // Color code based on stock level
                if (product.QuantityInStock <= 0)
                {
                    dgvLowStockProducts.Rows[row].Cells["Status"].Style.ForeColor = Color.White;
                    dgvLowStockProducts.Rows[row].Cells["Status"].Style.BackColor = Color.FromArgb(220, 53, 69);
                    dgvLowStockProducts.Rows[row].Cells["Status"].Style.Font = new Font(dgvLowStockProducts.Font, FontStyle.Bold);
                }
                else if (product.QuantityInStock <= 5)
                {
                    dgvLowStockProducts.Rows[row].Cells["Status"].Style.ForeColor = Color.White;
                    dgvLowStockProducts.Rows[row].Cells["Status"].Style.BackColor = Color.FromArgb(255, 193, 7);
                    dgvLowStockProducts.Rows[row].Cells["Status"].Style.Font = new Font(dgvLowStockProducts.Font, FontStyle.Bold);
                }
            }
        }

        private void LoadTransactionSummary()
        {
            // Configure grid columns
            dgvTransactionSummary.Columns.Clear();
            dgvTransactionSummary.Columns.Add("TransactionType", "Transaction Type");
            dgvTransactionSummary.Columns.Add("Count", "Count");
            dgvTransactionSummary.Columns.Add("TotalQuantity", "Total Quantity");
            dgvTransactionSummary.Columns.Add("PercentOfTotal", "% of Total");
            dgvTransactionSummary.Columns.Add("ProductsAffected", "Products Affected");
            dgvTransactionSummary.Columns.Add("AvgQuantityPerTransaction", "Avg Qty/Transaction");
            
            // Get transactions within date range
            string selectedType = cmbTransactionType.SelectedItem.ToString();
            var transactions = StaticDataProvider.InventoryTransactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Where(t => selectedType == "All" || t.TransactionType == selectedType)
                .ToList();
            
            // Group by transaction type
            var transactionStats = transactions
                .GroupBy(t => t.TransactionType)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    TotalQuantity = g.Sum(t => t.Quantity),
                    ProductsAffected = g.Select(t => t.ProductID).Distinct().Count(),
                    AvgQuantityPerTransaction = g.Average(t => t.Quantity)
                })
                .OrderByDescending(x => x.Count)
                .ToList();
            
            // Calculate total for percentage
            int totalTransactions = transactions.Count;
            
            // Add data rows for each type
            foreach (var stat in transactionStats)
            {
                double percentOfTotal = totalTransactions > 0 ?
                    (double)stat.Count / totalTransactions * 100 : 0;
                
                var row = dgvTransactionSummary.Rows.Add(
                    stat.Type,
                    stat.Count.ToString("N0"),
                    stat.TotalQuantity.ToString("N0"),
                    percentOfTotal.ToString("F1") + "%",
                    stat.ProductsAffected.ToString("N0"),
                    stat.AvgQuantityPerTransaction.ToString("F1")
                );
                
                // Color code based on transaction type
                if (stat.Type == "Sale")
                {
                    dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(220, 53, 69);
                    dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactionSummary.Font, FontStyle.Bold);
                }
                else if (stat.Type == "Restock")
                {
                    dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(40, 167, 69);
                    dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactionSummary.Font, FontStyle.Bold);
                }
            }
            
            // Add total row
            if (transactionStats.Count > 0)
            {
                int totalCount = transactionStats.Sum(s => s.Count);
                int totalQuantity = transactionStats.Sum(s => s.TotalQuantity);
                int distinctProducts = transactions.Select(t => t.ProductID).Distinct().Count();
                double avgQuantity = transactions.Count > 0 ? transactions.Average(t => t.Quantity) : 0;
                
                var totalRow = dgvTransactionSummary.Rows.Add(
                    "TOTAL",
                    totalCount.ToString("N0"),
                    totalQuantity.ToString("N0"),
                    "100.0%",
                    distinctProducts.ToString("N0"),
                    avgQuantity.ToString("F1")
                );
                
                // Style total row
                dgvTransactionSummary.Rows[totalRow].DefaultCellStyle.Font = new Font(dgvTransactionSummary.Font, FontStyle.Bold);
                dgvTransactionSummary.Rows[totalRow].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            }
        }

        private void LoadTransactionsGrid()
        {
            // Configure grid columns
            dgvTransactions.Columns.Clear();
            dgvTransactions.Columns.Add("TransactionID", "ID");
            dgvTransactions.Columns.Add("TransactionDate", "Date");
            dgvTransactions.Columns.Add("TransactionType", "Type");
            dgvTransactions.Columns.Add("ProductID", "Product ID");
            dgvTransactions.Columns.Add("ProductName", "Product");
            dgvTransactions.Columns.Add("Quantity", "Quantity");
            dgvTransactions.Columns.Add("ProcessedBy", "Processed By");
            
            // Get transactions within date range
            string selectedType = cmbTransactionType.SelectedItem.ToString();
            var filteredTransactions = StaticDataProvider.InventoryTransactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Where(t => selectedType == "All" || t.TransactionType == selectedType)
                .OrderByDescending(t => t.TransactionDate)
                .Take(100) // Limit to 100 most recent transactions
                .ToList();
            
            // Add data rows
            foreach (var transaction in filteredTransactions)
            {
                // Get product info
                var product = StaticDataProvider.Products.FirstOrDefault(p => p.ProductID == transaction.ProductID);
                string productName = product != null ? 
                    $"{product.Manufacturer} {product.Model}" : 
                    $"Product #{transaction.ProductID}";
                
                // Get user who processed the transaction
                var user = StaticDataProvider.Users.FirstOrDefault(u => u.UserID == transaction.ProcessedBy);
                string processedBy = user != null ? 
                    $"{user.FirstName} {user.LastName}" : 
                    $"User #{transaction.ProcessedBy}";
                
                var row = dgvTransactions.Rows.Add(
                    transaction.TransactionID.ToString(),
                    transaction.TransactionDate.ToString("g"),
                    transaction.TransactionType,
                    transaction.ProductID.ToString(),
                    productName,
                    transaction.Quantity.ToString("N0"),
                    processedBy
                );
                
                // Color-code by transaction type
                if (transaction.TransactionType == "Sale")
                {
                    dgvTransactions.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(220, 53, 69);
                    dgvTransactions.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
                }
                else if (transaction.TransactionType == "Restock")
                {
                    dgvTransactions.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(40, 167, 69);
                    dgvTransactions.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
                }
            }
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
            
            // Reload transaction data
            LoadTransactionAnalysisData();
        }
        #endregion

        #region Helper Methods
        private string GetStockStatusText(int quantity)
        {
            if (quantity <= 0)
                return "Out of Stock";
            else if (quantity <= 5)
                return "Critical Low";
            else if (quantity <= 10)
                return "Low Stock";
            else
                return "In Stock";
        }
        #endregion
    }
}