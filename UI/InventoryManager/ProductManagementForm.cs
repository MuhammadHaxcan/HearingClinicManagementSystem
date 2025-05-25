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
using HearingClinicManagementSystem.UI.Constants;

namespace HearingClinicManagementSystem.UI.InventoryManager {
    public class ProductManagementForm : BaseForm {
        #region Fields
        private DataGridView dgvProducts;
        private Panel pnlProductDetails;
        private Panel pnlInventoryTransaction;
        private TableLayoutPanel mainLayout;
        private Button btnAddProduct;
        private Button btnSaveProduct;
        private Button btnCancelEdit;
        private Button btnRefresh;
        private Button btnAddStock;
        private Button btnRemoveStock;

        // Product details fields
        private TextBox txtProductId;
        private TextBox txtManufacturer;
        private TextBox txtModel;
        private TextBox txtFeatures;
        private NumericUpDown nudPrice;
        private NumericUpDown nudQuantity;

        // Inventory transaction fields
        private NumericUpDown nudTransactionQuantity;
        private ComboBox cmbTransactionType;
        private TextBox txtTransactionReason;

        private bool isEditMode = false;
        private bool isNewProduct = false;
        private int? selectedProductId = null;
        private HearingClinicRepository repository;
        #endregion

        public ProductManagementForm() {
            repository = HearingClinicRepository.Instance;
            InitializeComponents();
            LoadProductInventory();
        }

        #region UI Setup
        private void InitializeComponents() {
            this.Text = "Product Management";
            this.Size = new Size(1200, 800);

            // Create title
            var lblTitle = CreateTitleLabel("Product & Inventory Management");
            lblTitle.Dock = DockStyle.Top;

            // Main layout with 2 columns
            mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(10),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };

            // Configure column and row styles
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));  // Products grid (65%)
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));  // Details panel (35%)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));        // Upper section
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));        // Lower section

            // Create the panels
            var productsPanel = CreateProductsPanel();         // Products grid
            pnlProductDetails = CreateProductDetailsPanel();   // Product details/add panel
            pnlInventoryTransaction = CreateInventoryTransactionPanel(); // Inventory transaction panel

            // Add panels to the main layout
            mainLayout.Controls.Add(productsPanel, 0, 0);
            mainLayout.SetRowSpan(productsPanel, 2);         // Products grid spans both rows
            mainLayout.Controls.Add(pnlProductDetails, 1, 0);
            mainLayout.Controls.Add(pnlInventoryTransaction, 1, 1);

            // Add all controls to the form
            Controls.Add(mainLayout);
            Controls.Add(lblTitle);
        }

        private Panel CreateProductsPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Create panel header with title and buttons
            Panel headerPanel = new Panel {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 10, 5)
            };

            Label lblHeader = new Label {
                Text = "Products Inventory",
                Font = new Font(DefaultFont.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 15),
                AutoSize = true
            };

            btnRefresh = new Button {
                Text = "Refresh",
                Size = new Size(100, 30),
                Location = new Point(headerPanel.Width - 120, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefresh.Click += BtnRefresh_Click;
            ApplyButtonStyle(btnRefresh, Color.FromArgb(108, 117, 125));

            btnAddProduct = new Button {
                Text = "Add New Product",
                Size = new Size(140, 30),
                Location = new Point(headerPanel.Width - 270, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAddProduct.Click += BtnAddProduct_Click;
            ApplyButtonStyle(btnAddProduct, Color.FromArgb(40, 167, 69));

            headerPanel.Controls.Add(lblHeader);
            headerPanel.Controls.Add(btnRefresh);
            headerPanel.Controls.Add(btnAddProduct);

            // Create products grid
            dgvProducts = new DataGridView {
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
            dgvProducts.Columns.Add("ProductID", "ID");
            dgvProducts.Columns.Add("Manufacturer", "Manufacturer");
            dgvProducts.Columns.Add("Model", "Model");
            dgvProducts.Columns.Add("Features", "Features");
            dgvProducts.Columns.Add("Price", "Price");
            dgvProducts.Columns.Add("QuantityInStock", "Quantity in Stock");

            // Set column widths as percentages
            dgvProducts.Columns["ProductID"].Width = 50;
            dgvProducts.Columns["Manufacturer"].FillWeight = 20;
            dgvProducts.Columns["Model"].FillWeight = 20;
            dgvProducts.Columns["Features"].FillWeight = 30;
            dgvProducts.Columns["Price"].FillWeight = 15;
            dgvProducts.Columns["QuantityInStock"].FillWeight = 15;

            // Style the grid header
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font(dgvProducts.Font, FontStyle.Bold);
            dgvProducts.ColumnHeadersHeight = 35;
            dgvProducts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Attach selection changed event
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
            dgvProducts.CellFormatting += DgvProducts_CellFormatting;

            // Add header and grid to panel
            panel.Controls.Add(dgvProducts);
            panel.Controls.Add(headerPanel);

            return panel;
        }

        private Panel CreateProductDetailsPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Create panel header
            Panel headerPanel = new Panel {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 10, 5)
            };

            Label lblHeader = new Label {
                Text = "Product Details",
                Font = new Font(DefaultFont.FontFamily, 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);

            // Create details form layout
            TableLayoutPanel detailsLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 10, 15, 10),
                ColumnCount = 2,
                RowCount = 7
            };

            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Add form fields
            int rowIndex = 0;

            // Product ID (readonly)
            detailsLayout.Controls.Add(new Label { Text = "Product ID:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            txtProductId = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.LightGray };
            detailsLayout.Controls.Add(txtProductId, 1, rowIndex++);

            // Manufacturer
            detailsLayout.Controls.Add(new Label { Text = "Manufacturer:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            txtManufacturer = new TextBox { Dock = DockStyle.Fill, MaxLength = 100 };
            detailsLayout.Controls.Add(txtManufacturer, 1, rowIndex++);

            // Model
            detailsLayout.Controls.Add(new Label { Text = "Model:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            txtModel = new TextBox { Dock = DockStyle.Fill, MaxLength = 100 };
            detailsLayout.Controls.Add(txtModel, 1, rowIndex++);

            // Features
            detailsLayout.Controls.Add(new Label { Text = "Features:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            txtFeatures = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 80, MaxLength = 500 };
            detailsLayout.Controls.Add(txtFeatures, 1, rowIndex);
            detailsLayout.SetRowSpan(txtFeatures, 2); // Features field spans 2 rows
            rowIndex += 2;

            // Price
            detailsLayout.Controls.Add(new Label { Text = "Price ($):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            nudPrice = new NumericUpDown {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 9999.99M,
                DecimalPlaces = 2,
                Increment = 10,
                Value = 0
            };
            detailsLayout.Controls.Add(nudPrice, 1, rowIndex++);

            // Quantity
            detailsLayout.Controls.Add(new Label { Text = "Quantity:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            nudQuantity = new NumericUpDown {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 0,
                Value = 0
            };
            detailsLayout.Controls.Add(nudQuantity, 1, rowIndex++);

            // Button panel
            Panel buttonPanel = new Panel {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            btnSaveProduct = new Button {
                Text = "Save",
                Size = new Size(100, 35),
                Location = new Point(buttonPanel.Width - 220, 8),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnSaveProduct.Click += BtnSaveProduct_Click;
            ApplyButtonStyle(btnSaveProduct, Color.FromArgb(0, 123, 255));

            btnCancelEdit = new Button {
                Text = "Cancel",
                Size = new Size(100, 35),
                Location = new Point(buttonPanel.Width - 110, 8),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCancelEdit.Click += BtnCancelEdit_Click;
            ApplyButtonStyle(btnCancelEdit, Color.FromArgb(108, 117, 125));

            buttonPanel.Controls.Add(btnSaveProduct);
            buttonPanel.Controls.Add(btnCancelEdit);

            // Add layouts to panel
            panel.Controls.Add(detailsLayout);
            panel.Controls.Add(buttonPanel);
            panel.Controls.Add(headerPanel);

            // Disable all fields by default
            SetDetailsFieldsReadOnly(true);

            return panel;
        }

        private Panel CreateInventoryTransactionPanel() {
            Panel panel = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Create panel header
            Panel headerPanel = new Panel {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10, 5, 10, 5)
            };

            Label lblHeader = new Label {
                Text = "Inventory Transaction",
                Font = new Font(DefaultFont.FontFamily, 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);

            // Create transaction form layout
            TableLayoutPanel transactionLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 10, 15, 10),
                ColumnCount = 2,
                RowCount = 5
            };

            transactionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            transactionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Add form fields
            int rowIndex = 0;

            // Transaction Type
            transactionLayout.Controls.Add(new Label { Text = "Transaction:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            cmbTransactionType = new ComboBox {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTransactionType.Items.AddRange(new object[] { "Add Stock", "Remove Stock" });
            cmbTransactionType.SelectedIndex = 0;
            transactionLayout.Controls.Add(cmbTransactionType, 1, rowIndex++);

            // Quantity
            transactionLayout.Controls.Add(new Label { Text = "Quantity:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            nudTransactionQuantity = new NumericUpDown {
                Dock = DockStyle.Fill,
                Minimum = 1,
                Maximum = 1000,
                DecimalPlaces = 0,
                Value = 1
            };
            transactionLayout.Controls.Add(nudTransactionQuantity, 1, rowIndex++);

            // Reason/Notes
            transactionLayout.Controls.Add(new Label { Text = "Reason:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, rowIndex);
            txtTransactionReason = new TextBox {
                Dock = DockStyle.Fill,
                Multiline = true,
                Height = 60,
                MaxLength = 500
            };
            transactionLayout.Controls.Add(txtTransactionReason, 1, rowIndex++);

            // Button panel
            Panel buttonPanel = new Panel {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            btnAddStock = new Button {
                Text = "Add Stock",
                Size = new Size(120, 35),
                Location = new Point(buttonPanel.Width - 240, 8),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAddStock.Click += BtnAddStock_Click;
            ApplyButtonStyle(btnAddStock, Color.FromArgb(40, 167, 69));

            btnRemoveStock = new Button {
                Text = "Remove Stock",
                Size = new Size(120, 35),
                Location = new Point(buttonPanel.Width - 110, 8),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRemoveStock.Click += BtnRemoveStock_Click;
            ApplyButtonStyle(btnRemoveStock, Color.FromArgb(220, 53, 69));

            buttonPanel.Controls.Add(btnAddStock);
            buttonPanel.Controls.Add(btnRemoveStock);

            // Add layouts to panel
            panel.Controls.Add(transactionLayout);
            panel.Controls.Add(buttonPanel);
            panel.Controls.Add(headerPanel);

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
        #endregion

        #region Event Handlers
        private void DgvProducts_SelectionChanged(object sender, EventArgs e) {
            if (isEditMode)
                return; // Don't change selection during edit mode

            if (dgvProducts.SelectedRows.Count > 0) {
                var selectedRow = dgvProducts.SelectedRows[0];
                selectedProductId = (int)selectedRow.Cells["ProductID"].Value;

                // Load product details into the form
                txtProductId.Text = selectedRow.Cells["ProductID"].Value.ToString();
                txtManufacturer.Text = selectedRow.Cells["Manufacturer"].Value.ToString();
                txtModel.Text = selectedRow.Cells["Model"].Value.ToString();
                txtFeatures.Text = selectedRow.Cells["Features"].Value.ToString();
                nudPrice.Value = Convert.ToDecimal(selectedRow.Cells["Price"].Value);
                nudQuantity.Value = Convert.ToDecimal(selectedRow.Cells["QuantityInStock"].Value);

                // Enable inventory transaction buttons
                btnAddStock.Enabled = true;
                btnRemoveStock.Enabled = true;
            } else {
                ClearProductDetails();
                selectedProductId = null;
                btnAddStock.Enabled = false;
                btnRemoveStock.Enabled = false;
            }
        }

        private void DgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.RowIndex >= 0) {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                // Format price as currency
                if (e.ColumnIndex == dgvProducts.Columns["Price"].Index && e.Value != null) {
                    if (decimal.TryParse(e.Value.ToString(), out decimal price)) {
                        e.Value = string.Format("${0:N2}", price);
                        e.FormattingApplied = true;
                    }
                }

                // Highlight low stock (less than 5 items)
                if (e.ColumnIndex == dgvProducts.Columns["QuantityInStock"].Index && e.Value != null) {
                    if (int.TryParse(e.Value.ToString(), out int quantity)) {
                        if (quantity <= 5) {
                            e.CellStyle.ForeColor = Color.Red;
                            e.CellStyle.Font = new Font(dgvProducts.Font, FontStyle.Bold);
                        }
                    }
                }

                // Alternate row coloring for better readability
                if (e.RowIndex % 2 == 0) {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                } else {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e) {
            if (isEditMode) {
                if (UIService.ShowQuestion("You have unsaved changes. Do you want to discard them and refresh?") == DialogResult.Yes) {
                    CancelEdit();
                    LoadProductInventory();
                }
            } else {
                LoadProductInventory();
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e) {
            if (isEditMode) {
                UIService.ShowWarning("Please save or cancel the current operation first.");
                return;
            }

            // Set up for new product
            isEditMode = true;
            isNewProduct = true;
            selectedProductId = null;

            // Clear and enable form fields
            ClearProductDetails();
            SetDetailsFieldsReadOnly(false);

            // Hide ID for new product
            txtProductId.Text = "(New Product)";
            txtProductId.BackColor = Color.LightYellow;

            // Enable save/cancel buttons
            btnSaveProduct.Enabled = true;
            btnCancelEdit.Enabled = true;

            // Disable other operations
            btnAddProduct.Enabled = false;
            btnAddStock.Enabled = false;
            btnRemoveStock.Enabled = false;
            dgvProducts.Enabled = false;
        }

        private void BtnSaveProduct_Click(object sender, EventArgs e) {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtManufacturer.Text)) {
                UIService.ShowWarning("Manufacturer name is required.");
                txtManufacturer.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtModel.Text)) {
                UIService.ShowWarning("Model name is required.");
                txtModel.Focus();
                return;
            }

            try {
                if (isNewProduct) {
                    // Create a new product
                    var newProduct = new Product {
                        Manufacturer = txtManufacturer.Text,
                        Model = txtModel.Text,
                        Features = txtFeatures.Text,
                        Price = nudPrice.Value,
                        QuantityInStock = 0
                    };

                    // Add product using repository
                    int newProductId = repository.AddProductForManager(newProduct);
                    selectedProductId = newProductId;

                    // Record inventory transaction if initial quantity > 0
                    if (nudQuantity.Value > 0) {
                        repository.AddStock(newProductId, (int)nudQuantity.Value, "Initial stock", AuthService.CurrentUser.UserID);
                    }

                    UIService.ShowSuccess("New product added successfully.");
                } else if (selectedProductId.HasValue) {
                    // Update existing product
                    var product = repository.GetProductByIdForManager(selectedProductId.Value);
                    if (product != null) {
                        // Check if quantity changed
                        int oldQuantity = product.QuantityInStock;
                        int newQuantity = (int)nudQuantity.Value;
                        int quantityDifference = newQuantity - oldQuantity;

                        // Update properties
                        product.Manufacturer = txtManufacturer.Text;
                        product.Model = txtModel.Text;
                        product.Features = txtFeatures.Text;
                        product.Price = nudPrice.Value;
                        product.QuantityInStock = newQuantity;

                        // Update product using repository
                        repository.UpdateProductForManager(product);

                        // Record inventory transaction if quantity changed
                        if (quantityDifference > 0) {
                            repository.AddStock(product.ProductID, quantityDifference, "Stock adjustment (increase)", AuthService.CurrentUser.UserID);
                        } else if (quantityDifference < 0) {
                            repository.RemoveStock(product.ProductID, Math.Abs(quantityDifference), "Stock adjustment (decrease)", AuthService.CurrentUser.UserID);
                        }

                        UIService.ShowSuccess("Product updated successfully.");
                    }
                }

                // Exit edit mode and reload
                isEditMode = false;
                isNewProduct = false;
                SetDetailsFieldsReadOnly(true);
                btnSaveProduct.Enabled = false;
                btnCancelEdit.Enabled = false;
                btnAddProduct.Enabled = true;
                dgvProducts.Enabled = true;

                // Reload products
                LoadProductInventory();
            } catch (Exception ex) {
                UIService.ShowError($"Error saving product: {ex.Message}");
            }
        }

        private void BtnCancelEdit_Click(object sender, EventArgs e) {
            CancelEdit();
        }

        private void BtnAddStock_Click(object sender, EventArgs e) {
            ProcessInventoryTransaction(true);
        }

        private void BtnRemoveStock_Click(object sender, EventArgs e) {
            ProcessInventoryTransaction(false);
        }
        #endregion

        #region Helper Methods
        private void LoadProductInventory() {
            dgvProducts.Rows.Clear();

            // Get products using repository method
            var products = repository.GetProductsOrderedByManufacturerAndModel();

            foreach (var product in products) {
                dgvProducts.Rows.Add(
                    product.ProductID,
                    product.Manufacturer,
                    product.Model,
                    product.Features,
                    product.Price,
                    product.QuantityInStock
                );
            }

            // Select previously selected product if it still exists
            if (selectedProductId.HasValue) {
                foreach (DataGridViewRow row in dgvProducts.Rows) {
                    if ((int)row.Cells["ProductID"].Value == selectedProductId.Value) {
                        row.Selected = true;
                        dgvProducts.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }
        }

        private void CancelEdit() {
            isEditMode = false;
            isNewProduct = false;

            // Re-enable controls
            dgvProducts.Enabled = true;
            btnAddProduct.Enabled = true;

            // Disable edit mode buttons
            btnSaveProduct.Enabled = false;
            btnCancelEdit.Enabled = false;

            // Set fields to read-only
            SetDetailsFieldsReadOnly(true);

            // Restore selected product details or clear if none selected
            if (selectedProductId.HasValue) {
                var product = repository.GetProductById(selectedProductId.Value);
                if (product != null) {
                    txtProductId.Text = product.ProductID.ToString();
                    txtManufacturer.Text = product.Manufacturer;
                    txtModel.Text = product.Model;
                    txtFeatures.Text = product.Features;
                    nudPrice.Value = product.Price;
                    nudQuantity.Value = product.QuantityInStock;

                    btnAddStock.Enabled = true;
                    btnRemoveStock.Enabled = true;
                }
            } else {
                ClearProductDetails();
                btnAddStock.Enabled = false;
                btnRemoveStock.Enabled = false;
            }
        }

        private void ClearProductDetails() {
            txtProductId.Text = string.Empty;
            txtManufacturer.Text = string.Empty;
            txtModel.Text = string.Empty;
            txtFeatures.Text = string.Empty;
            nudPrice.Value = 0;
            nudQuantity.Value = 0;
            txtProductId.BackColor = Color.LightGray;
        }

        private void SetDetailsFieldsReadOnly(bool readOnly) {
            // Product ID is always read-only
            txtManufacturer.ReadOnly = readOnly;
            txtModel.ReadOnly = readOnly;
            txtFeatures.ReadOnly = readOnly;
            nudPrice.Enabled = !readOnly;
            nudQuantity.Enabled = !readOnly;

            // Set background colors for better UX
            Color bgColor = readOnly ? SystemColors.Control : SystemColors.Window;
            txtManufacturer.BackColor = bgColor;
            txtModel.BackColor = bgColor;
            txtFeatures.BackColor = bgColor;
        }

        private void ProcessInventoryTransaction(bool isAddition) {
            if (!selectedProductId.HasValue) {
                UIService.ShowWarning("Please select a product first.");
                return;
            }

            // Validate quantity
            int quantity = (int)nudTransactionQuantity.Value;
            if (quantity <= 0) {
                UIService.ShowWarning("Please enter a valid quantity greater than zero.");
                nudTransactionQuantity.Focus();
                return;
            }

            // Validate reason
            if (string.IsNullOrWhiteSpace(txtTransactionReason.Text)) {
                UIService.ShowWarning("Please provide a reason for this transaction.");
                txtTransactionReason.Focus();
                return;
            }

            try {
                // Process stock change using repository
                if (isAddition) {
                    repository.AddStock(selectedProductId.Value, quantity, txtTransactionReason.Text, AuthService.CurrentUser.UserID);
                } else {
                    repository.RemoveStock(selectedProductId.Value, quantity, txtTransactionReason.Text, AuthService.CurrentUser.UserID);
                }

                // Show success message
                string action = isAddition ? "added to" : "removed from";
                UIService.ShowSuccess($"{quantity} items {action} inventory successfully.");

                // Update UI
                LoadProductInventory();
                txtTransactionReason.Text = string.Empty;
                nudTransactionQuantity.Value = 1;
            } catch (Exception ex) {
                UIService.ShowError($"Error processing inventory transaction: {ex.Message}");
            }
        }
        #endregion
    }
}