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
        
        // Repository
        private HearingClinicRepository repository;
        #endregion

        public InventoryReportingForm()
        {
            repository = HearingClinicRepository.Instance;
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
                // Make sure the date range is properly initialized before loading data
                if (startDate > endDate)
                {
                    startDate = DateTime.Now.AddMonths(-3);
                    endDate = DateTime.Now;
                    
                    // Update UI date controls
                    dtpStartDate.Value = startDate;
                    dtpEndDate.Value = endDate;
                }
                
                LoadSalesOverviewData();
                LoadTransactionAnalysisData();
                
                lblLastRefreshed.Text = "Last refreshed: " + DateTime.Now.ToString("g");
            }
            catch (Exception ex)
            {
                UIService.ShowError($"Error loading report data: {ex.Message}\n\nPlease check the repository connection.");
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
            try
            {
                // Get inventory summary statistics from repository
                var stats = repository.GetInventorySummaryStats();
                
                if (stats == null)
                {
                    // Set default values if no stats are returned
                    lblTotalOrders.Text = "0";
                    lblTotalSales.Text = "$0.00";
                    lblAverageOrderValue.Text = "$0.00";
                    lblMostPopularProduct.Text = "No data available";
                    lblLowStockCount.Text = "0 items";
                    lblTotalProducts.Text = "0";
                    return;
                }
                
                // Safe retrieval of properties with defaults
                int totalOrders = 0;
                decimal totalSales = 0m;
                decimal avgOrderValue = 0m;
                string mostPopularProduct = "No data available";
                int mostPopularCount = 0;
                int lowStockCount = 0;
                int totalProducts = 0;
                
                try { totalOrders = stats.TotalOrders; } catch { }
                try { totalSales = stats.TotalSales; } catch { }
                try { avgOrderValue = stats.AverageOrderValue; } catch { }
                try { mostPopularProduct = stats.MostPopularProduct?.ToString() ?? "No data available"; } catch { }
                try { mostPopularCount = stats.MostPopularProductCount; } catch { }
                try { lowStockCount = stats.LowStockCount; } catch { }
                try { totalProducts = stats.TotalProducts; } catch { }
                
                // Update UI with the statistics
                lblTotalOrders.Text = totalOrders.ToString();
                lblTotalSales.Text = totalSales.ToString("C2");
                lblAverageOrderValue.Text = avgOrderValue.ToString("C2");
                
                if (mostPopularCount > 0)
                {
                    lblMostPopularProduct.Text = $"{mostPopularProduct} ({mostPopularCount} units)";
                }
                else
                {
                    lblMostPopularProduct.Text = "No data available";
                }
                
                lblLowStockCount.Text = $"{lowStockCount} items";
                lblTotalProducts.Text = totalProducts.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading summary statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Set default values on error
                lblTotalOrders.Text = "Error";
                lblTotalSales.Text = "Error";
                lblAverageOrderValue.Text = "Error";
                lblMostPopularProduct.Text = "Error loading data";
                lblLowStockCount.Text = "Error";
                lblTotalProducts.Text = "Error";
            }
        }

        private void LoadTopSellingProductsGrid()
        {
            try
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
                
                // Get top selling products from repository
                var topProducts = repository.GetTopSellingProducts(15);
                
                if (topProducts == null || topProducts.Count == 0)
                {
                    dgvTopSellingProducts.Rows.Add("-", "-", "No sales data available", "-", "0", "$0.00", "0%");
                    return;
                }
                
                // Add data rows
                for (int i = 0; i < topProducts.Count; i++)
                {
                    var product = topProducts[i];
                    
                    // Safely extract properties
                    int rank = i + 1;
                    int productId = 0;
                    string productName = "Unknown";
                    string manufacturer = "Unknown";
                    int unitsSold = 0;
                    decimal totalSales = 0m;
                    decimal percentOfSales = 0m;
                    
                    try { rank = product.Rank; } catch { }
                    try { productId = product.ProductID; } catch { }
                    try { productName = product.ProductName?.ToString() ?? "Unknown"; } catch { }
                    try { manufacturer = product.Manufacturer?.ToString() ?? "Unknown"; } catch { }
                    try { unitsSold = product.UnitsSold; } catch { }
                    try { totalSales = product.TotalSales; } catch { }
                    try { percentOfSales = product.PercentOfSales; } catch { }
                    
                    dgvTopSellingProducts.Rows.Add(
                        rank.ToString(),
                        productId.ToString(),
                        productName,
                        manufacturer,
                        unitsSold.ToString("N0"),
                        totalSales.ToString("C2"),
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading top selling products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLowStockProductsGrid()
        {
            try
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
                
                // Get low stock products from repository
                var lowStockProducts = repository.GetLowStockProducts(10);
                
                if (lowStockProducts == null || lowStockProducts.Count == 0)
                {
                    // Add a message row if there are no low stock products
                    dgvLowStockProducts.Rows.Add("-", "No low stock products found", "-", "-", "-", "-", "-");
                    return;
                }
                
                // Add data rows
                foreach (var product in lowStockProducts)
                {
                    string lastRestockedText = "Never";
                    
                    // Safely handle the LastRestocked property which might be null
                    try
                    {
                        if (product.GetType().GetProperty("LastRestocked") != null)
                        {
                            var lastRestocked = product.LastRestocked;
                            if (lastRestocked != null)
                            {
                                lastRestockedText = Convert.ToDateTime(lastRestocked).ToShortDateString();
                            }
                        }
                    }
                    catch
                    {
                        // If there's any issue accessing LastRestocked, keep the default "Never"
                    }
                    
                    // Safely get other properties with defaults if missing
                    int productId = 0;
                    string productName = "Unknown";
                    string manufacturer = "Unknown";
                    int currentStock = 0;
                    string status = "Unknown";
                    decimal price = 0m;
                    
                    try { productId = product.ProductID; } catch { }
                    try { productName = product.ProductName?.ToString() ?? "Unknown"; } catch { }
                    try { manufacturer = product.Manufacturer?.ToString() ?? "Unknown"; } catch { }
                    try { currentStock = product.CurrentStock; } catch { }
                    try { status = product.Status?.ToString() ?? "Unknown"; } catch { }
                    try { price = product.Price; } catch { }
                    
                    // Create the row with safely extracted values
                    var row = dgvLowStockProducts.Rows.Add(
                        productId.ToString(),
                        productName,
                        manufacturer,
                        currentStock.ToString(),
                        status,
                        price.ToString("C2"),
                        lastRestockedText
                    );
                    
                    // Color code based on stock level - check if CurrentStock property exists
                    try
                    {
                        if (currentStock <= 0)
                        {
                            dgvLowStockProducts.Rows[row].Cells["Status"].Style.ForeColor = Color.White;
                            dgvLowStockProducts.Rows[row].Cells["Status"].Style.BackColor = Color.FromArgb(220, 53, 69);
                            dgvLowStockProducts.Rows[row].Cells["Status"].Style.Font = new Font(dgvLowStockProducts.Font, FontStyle.Bold);
                        }
                        else if (currentStock <= 5)
                        {
                            dgvLowStockProducts.Rows[row].Cells["Status"].Style.ForeColor = Color.White;
                            dgvLowStockProducts.Rows[row].Cells["Status"].Style.BackColor = Color.FromArgb(255, 193, 7);
                            dgvLowStockProducts.Rows[row].Cells["Status"].Style.Font = new Font(dgvLowStockProducts.Font, FontStyle.Bold);
                        }
                    }
                    catch
                    {
                        // If there's an issue with styling, just continue
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading low stock products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTransactionSummary()
        {
            try
            {
                // Configure grid columns
                dgvTransactionSummary.Columns.Clear();
                dgvTransactionSummary.Columns.Add("TransactionType", "Transaction Type");
                dgvTransactionSummary.Columns.Add("Count", "Count");
                dgvTransactionSummary.Columns.Add("TotalQuantity", "Total Quantity");
                dgvTransactionSummary.Columns.Add("PercentOfTotal", "% of Total");
                dgvTransactionSummary.Columns.Add("ProductsAffected", "Products Affected");
                dgvTransactionSummary.Columns.Add("AvgQuantityPerTransaction", "Avg Qty/Transaction");
                
                // Get selected transaction type
                string selectedType = cmbTransactionType.SelectedItem?.ToString() ?? "All";
                
                // Get transaction summary from repository
                var transactionStats = repository.GetTransactionSummary(startDate, endDate, selectedType);
                
                if (transactionStats == null || transactionStats.Count == 0)
                {
                    dgvTransactionSummary.Rows.Add("No Data", "0", "0", "0%", "0", "0");
                    return;
                }
                
                // Add data rows
                foreach (var stat in transactionStats)
                {
                    string transactionType = "Unknown";
                    int count = 0;
                    int totalQuantity = 0;
                    double percentOfTotal = 0;
                    int productsAffected = 0;
                    double avgQuantity = 0;
                    
                    try { transactionType = stat.TransactionType?.ToString() ?? "Unknown"; } catch { }
                    try { count = stat.Count; } catch { }
                    try { totalQuantity = stat.TotalQuantity; } catch { }
                    try { percentOfTotal = stat.PercentOfTotal; } catch { }
                    try { productsAffected = stat.ProductsAffected; } catch { }
                    try { avgQuantity = stat.AvgQuantityPerTransaction; } catch { }
                    
                    var row = dgvTransactionSummary.Rows.Add(
                        transactionType,
                        count.ToString("N0"),
                        totalQuantity.ToString("N0"),
                        percentOfTotal.ToString("F1") + "%",
                        productsAffected.ToString("N0"),
                        avgQuantity.ToString("F1")
                    );
                    
                    // Highlight the total row
                    if (transactionType == "TOTAL")
                    {
                        dgvTransactionSummary.Rows[row].DefaultCellStyle.Font = new Font(dgvTransactionSummary.Font, FontStyle.Bold);
                        dgvTransactionSummary.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    }
                    // Color code based on transaction type
                    else if (transactionType == "Sale")
                    {
                        dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(220, 53, 69);
                        dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactionSummary.Font, FontStyle.Bold);
                    }
                    else if (transactionType == "Restock")
                    {
                        dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(40, 167, 69);
                        dgvTransactionSummary.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactionSummary.Font, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transaction summary: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTransactionsGrid()
        {
            try
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
                
                // Get selected transaction type
                string selectedType = cmbTransactionType.SelectedItem?.ToString() ?? "All";
                
                // Get transactions from repository
                var transactions = repository.GetTransactionHistory(startDate, endDate, selectedType, 100);
                
                if (transactions == null || transactions.Count == 0)
                {
                    dgvTransactions.Rows.Add("-", "-", "No transactions found", "-", "-", "0", "-");
                    return;
                }
                
                // Add data rows
                foreach (var transaction in transactions)
                {
                    int transactionId = 0;
                    DateTime transactionDate = DateTime.Now;
                    string transactionType = "Unknown";
                    int productId = 0;
                    string productName = "Unknown";
                    int quantity = 0;
                    string processedBy = "Unknown";
                    
                    try { transactionId = transaction.TransactionID; } catch { }
                    try { transactionDate = transaction.TransactionDate; } catch { }
                    try { transactionType = transaction.TransactionType?.ToString() ?? "Unknown"; } catch { }
                    try { productId = transaction.ProductID; } catch { }
                    try { productName = transaction.ProductName?.ToString() ?? "Unknown"; } catch { }
                    try { quantity = transaction.Quantity; } catch { }
                    try { processedBy = transaction.ProcessedBy?.ToString() ?? "Unknown"; } catch { }
                    
                    var row = dgvTransactions.Rows.Add(
                        transactionId.ToString(),
                        transactionDate.ToString("g"),
                        transactionType,
                        productId.ToString(),
                        productName,
                        quantity.ToString("N0"),
                        processedBy
                    );
                    
                    // Color-code by transaction type
                    if (transactionType == "Sale")
                    {
                        dgvTransactions.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(220, 53, 69);
                        dgvTransactions.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
                    }
                    else if (transactionType == "Restock")
                    {
                        dgvTransactions.Rows[row].Cells["TransactionType"].Style.ForeColor = Color.FromArgb(40, 167, 69);
                        dgvTransactions.Rows[row].Cells["TransactionType"].Style.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transaction history: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}