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
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Common.HearingClinicManagementSystem.UI.Common;

namespace HearingClinicManagementSystem.UI.Patient
{
    public class PurchaseHearingAidForm : BaseForm
    {
        #region Fields
        private DataGridView dgvProducts;
        private DataGridView dgvCart;
        private DataGridView dgvOrders;
        private NumericUpDown nudQuantity;
        private TextBox txtDeliveryAddress;
        private Button btnAddToCart;
        private Button btnRemoveFromCart;
        private Button btnCheckout;
        private Button btnCancelOrder;
        private Label lblTotalPrice;
        private ListBox _lstProductFeatures; 
        #endregion


        public PurchaseHearingAidForm()
        {
            InitializeComponents();
            LoadProducts();
            LoadCart();
            LoadOrders();
        }

        #region UI Setup
        private void InitializeComponents()
        {
            this.Text = AppStrings.Titles.PurchaseHearingAid;

            // Form title
            var lblTitle = CreateTitleLabel(AppStrings.Titles.PurchaseHearingAid);
            lblTitle.Dock = DockStyle.Top;

            // Main layout - Two rows
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10),
                RowStyles = {
                    new RowStyle(SizeType.Percent, 60F),
                    new RowStyle(SizeType.Percent, 40F)
                }
            };

            // Upper section - Products and Cart
            TableLayoutPanel upperPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 60F),
                    new ColumnStyle(SizeType.Percent, 40F)
                }
            };

            InitializeProductsPanel(upperPanel);
            InitializeCartPanel(upperPanel);
            InitializeOrderHistoryPanel(mainPanel);

            mainPanel.Controls.Add(upperPanel, 0, 0);

            Controls.Add(mainPanel);
            Controls.Add(lblTitle);
        }
        private void InitializeProductsPanel(TableLayoutPanel parent)
        {
            // === PRODUCT LIST (Left) ===
            Panel pnlProducts = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Products Header
            var lblProducts = CreateLabel("Available Hearing Aids", 0, 0);
            lblProducts.Dock = DockStyle.Top;
            lblProducts.Font = new Font(lblProducts.Font, FontStyle.Bold);
            lblProducts.Height = 25;
            lblProducts.TextAlign = ContentAlignment.MiddleLeft;

            // Products DataGrid
            dgvProducts = CreateDataGrid(0, false, true);
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.MultiSelect = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.Columns.Add("ProductID", "ID");
            dgvProducts.Columns.Add("Model", "Model");
            dgvProducts.Columns.Add("Manufacturer", "Manufacturer");
            dgvProducts.Columns.Add("Price", "Price");
            dgvProducts.Columns.Add("Stock", "In Stock");
            dgvProducts.Columns["ProductID"].Visible = false;
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;

            // Create a visual separator
            Panel pnlSeparator = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 2,
                BackColor = Color.FromArgb(0, 120, 215) // Use the same blue as the checkout button
            };

            // Product Features Panel - with improved visual design
            Panel pnlProductFeatures = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(0)
            };

            // Create a better looking header for features
            Panel pnlFeaturesHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 28,
                BackColor = Color.FromArgb(240, 240, 240) // Light gray background
            };

            var lblFeaturesHeader = new Label
            {
                Text = "Product Features",
                Font = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204), // Darker blue for better contrast
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 0, 0, 0)
            };

            pnlFeaturesHeader.Controls.Add(lblFeaturesHeader);

            // Features list with better styling
            _lstProductFeatures = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                IntegralHeight = false,
                BackColor = Color.White,
                Font = new Font(this.Font.FontFamily, this.Font.Size),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 18
            };

            // Add custom drawing for bullet points
            _lstProductFeatures.DrawItem += (sender, e) =>
            {
                if (e.Index < 0) return;

                e.DrawBackground();
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                string text = _lstProductFeatures.Items[e.Index].ToString();
                Brush textBrush = new SolidBrush(e.ForeColor);

                // Draw the text with proper padding
                e.Graphics.DrawString(text, e.Font, textBrush, e.Bounds.Left + 2, e.Bounds.Top + 1);

                e.DrawFocusRectangle();
            };

            // Add components to the features panel
            pnlProductFeatures.Controls.Add(_lstProductFeatures);
            pnlProductFeatures.Controls.Add(pnlFeaturesHeader);

            // Add to cart panel
            Panel pnlAddToCart = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(245, 245, 245) // Light gray background
            };

            var lblQuantity = CreateLabel("Quantity:", 10, 10);
            lblQuantity.AutoSize = true;

            nudQuantity = CreateNumericUpDown(lblQuantity.Right + 5, 8, 60);
            nudQuantity.Minimum = 1;
            nudQuantity.Maximum = 10;
            nudQuantity.Value = 1;

            btnAddToCart = CreateButton("Add to Cart", nudQuantity.Right + 10, 8, BtnAddToCart_Click, 100, 24);
            // Use the helper method for consistent button styling
            ApplyButtonStyle(btnAddToCart);

            pnlAddToCart.Controls.AddRange(new Control[] { lblQuantity, nudQuantity, btnAddToCart });

            // Add all panels to main product panel in correct order
            pnlProducts.Controls.AddRange(new Control[] {
        lblProducts,
        dgvProducts,
        pnlAddToCart,
        pnlSeparator,
        pnlProductFeatures
    });

            parent.Controls.Add(pnlProducts, 0, 0);
        }

        private void InitializeCartPanel(TableLayoutPanel parent)
        {
            // === CART (Right) ===
            Panel pnlCart = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Cart Header
            var lblCart = CreateLabel("Your Shopping Cart", 0, 0);
            lblCart.Dock = DockStyle.Top;
            lblCart.Font = new Font(lblCart.Font, FontStyle.Bold);
            lblCart.Height = 25;
            lblCart.TextAlign = ContentAlignment.MiddleLeft;

            // Cart DataGrid
            dgvCart = CreateDataGrid(0, false, true);
            dgvCart.Dock = DockStyle.Fill;
            dgvCart.MultiSelect = false;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.Columns.Add("ProductID", "ID");
            dgvCart.Columns.Add("Model", "Model");
            dgvCart.Columns.Add("Quantity", "Quantity");
            dgvCart.Columns.Add("UnitPrice", "Unit Price");
            dgvCart.Columns.Add("TotalPrice", "Total Price");
            dgvCart.Columns["ProductID"].Visible = false;

            // Cart Controls Panel - Use FlowLayoutPanel for better alignment
            TableLayoutPanel pnlCartControls = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 120, // Increased height to accommodate all controls
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(5)
            };

            // Set column and row styles
            pnlCartControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            pnlCartControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            pnlCartControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Remove button row
            pnlCartControls.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));  // Address row
            pnlCartControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Checkout row

            // Row 1 - Remove button - Apply consistent styling
            btnRemoveFromCart = CreateButton("Remove from Cart", 0, 0, BtnRemoveFromCart_Click);
            btnRemoveFromCart.Dock = DockStyle.Fill;
            btnRemoveFromCart.Margin = new Padding(3);
            // Apply consistent button styling
            ApplyButtonStyle(btnRemoveFromCart);
            pnlCartControls.Controls.Add(btnRemoveFromCart, 0, 0);

            // Row 2 - Delivery address
            var lblAddress = CreateLabel("Delivery Address:", 0, 0);
            lblAddress.Dock = DockStyle.Fill;
            lblAddress.TextAlign = ContentAlignment.TopLeft; // Align to top
            lblAddress.Margin = new Padding(3, 7, 3, 3); // Add top margin

            txtDeliveryAddress = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                Text = AuthService.CurrentPatient?.Address ?? "",
                Margin = new Padding(3)
            };

            pnlCartControls.Controls.Add(lblAddress, 0, 1);
            pnlCartControls.Controls.Add(txtDeliveryAddress, 1, 1);

            // Row 3 - Total and checkout
            lblTotalPrice = CreateLabel("Total: $0.00", 0, 0, 120);
            lblTotalPrice.Dock = DockStyle.Fill;
            lblTotalPrice.Font = new Font(lblTotalPrice.Font, FontStyle.Bold);
            lblTotalPrice.TextAlign = ContentAlignment.MiddleLeft;
            lblTotalPrice.Margin = new Padding(3);

            Panel pnlCheckout = new Panel { Dock = DockStyle.Fill };
            btnCheckout = CreateButton("Checkout", 0, 0, BtnCheckout_Click, 100, 30);
            btnCheckout.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnCheckout.Location = new Point(pnlCheckout.Width - btnCheckout.Width - 5, 0);
            // Apply consistent button styling
            ApplyButtonStyle(btnCheckout);

            pnlCheckout.Controls.Add(btnCheckout);
            pnlCartControls.Controls.Add(lblTotalPrice, 0, 2);
            pnlCartControls.Controls.Add(pnlCheckout, 1, 2);

            pnlCart.Controls.AddRange(new Control[] { lblCart, dgvCart, pnlCartControls });

            parent.Controls.Add(pnlCart, 1, 0);
        }

        private void InitializeOrderHistoryPanel(TableLayoutPanel parent)
        {
            // === ORDER HISTORY (Bottom) ===
            Panel pnlOrderHistory = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Orders Header
            var lblOrders = CreateLabel("Your Order History", 0, 0);
            lblOrders.Dock = DockStyle.Top;
            lblOrders.Font = new Font(lblOrders.Font, FontStyle.Bold);
            lblOrders.Height = 25;
            lblOrders.TextAlign = ContentAlignment.MiddleLeft;

            // Orders DataGrid
            dgvOrders = CreateDataGrid(0, false, true);
            dgvOrders.Dock = DockStyle.Fill;
            dgvOrders.MultiSelect = false;
            dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrders.Columns.Add("OrderID", "Order ID");
            dgvOrders.Columns.Add("Date", "Date");
            dgvOrders.Columns.Add("Model", "Product");
            dgvOrders.Columns.Add("Manufacturer", "Manufacturer");
            dgvOrders.Columns.Add("Quantity", "Qty");
            dgvOrders.Columns.Add("Total", "Total");
            dgvOrders.Columns.Add("Status", "Status");
            dgvOrders.Columns["OrderID"].Visible = false;

            // Order Controls Panel
            Panel pnlOrderControls = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(245, 245, 245) // Match the same light gray as other control panels
            };

            btnCancelOrder = CreateButton("Cancel Order", 5, 8, BtnCancelOrder_Click, 120, 24);
            // Apply consistent button styling
            ApplyButtonStyle(btnCancelOrder);

            pnlOrderControls.Controls.Add(btnCancelOrder);

            pnlOrderHistory.Controls.AddRange(new Control[] { lblOrders, dgvOrders, pnlOrderControls });

            parent.Controls.Add(pnlOrderHistory, 0, 1);
        }

        #endregion

        #region Event Handlers
        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                int index = dgvProducts.SelectedRows[0].Index;
                if (index >= 0 && index < StaticDataProvider.Products.Count)
                {
                    var product = StaticDataProvider.Products[index];
                    DisplayProductDetails(product);
                }
            }
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                UIService.ShowError("Please select a product to add to cart");
                return;
            }

            int selectedIndex = dgvProducts.SelectedRows[0].Index;
            var selectedProduct = StaticDataProvider.Products[selectedIndex];

            // Check if selected quantity is available in inventory
            int requestedQuantity = (int)nudQuantity.Value;
            if (requestedQuantity > selectedProduct.QuantityInStock)
            {
                UIService.ShowError($"Sorry, only {selectedProduct.QuantityInStock} units available in stock.");
                return;
            }

            var cartOrder = GetOrCreateCartOrder();

            // Check if item already in cart, update quantity if it is
            var existingItem = StaticDataProvider.OrderItems.FirstOrDefault(oi =>
                oi.OrderID == cartOrder.OrderID && oi.ProductID == selectedProduct.ProductID);

            if (existingItem != null)
            {
                int newQuantity = existingItem.Quantity + requestedQuantity;
                if (newQuantity > selectedProduct.QuantityInStock)
                {
                    UIService.ShowError($"Cannot add more. Total would exceed available stock ({selectedProduct.QuantityInStock} units).");
                    return;
                }

                existingItem.Quantity = newQuantity;
            }
            else
            {
                // Add item to cart
                var newItem = new OrderItem
                {
                    OrderItemID = StaticDataProvider.OrderItems.Count > 0 ?
                        StaticDataProvider.OrderItems.Max(oi => oi.OrderItemID) + 1 : 1,
                    OrderID = cartOrder.OrderID,
                    ProductID = selectedProduct.ProductID,
                    Quantity = requestedQuantity,
                    UnitPrice = selectedProduct.Price
                };
                StaticDataProvider.OrderItems.Add(newItem);
            }

            // Update cart display
            LoadCart();
            UIService.ShowSuccess($"{selectedProduct.Model} added to cart!");
        }

        private void BtnRemoveFromCart_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count == 0)
            {
                UIService.ShowError("Please select an item to remove from your cart");
                return;
            }

            int productId = (int)dgvCart.SelectedRows[0].Cells["ProductID"].Value;
            var cartOrder = StaticDataProvider.Orders.FirstOrDefault(o =>
                o.PatientID == AuthService.CurrentPatient.PatientID && o.Status == "Cart");

            if (cartOrder != null)
            {
                var itemToRemove = StaticDataProvider.OrderItems.FirstOrDefault(oi =>
                    oi.OrderID == cartOrder.OrderID && oi.ProductID == productId);

                if (itemToRemove != null)
                {
                    StaticDataProvider.OrderItems.Remove(itemToRemove);

                    // If cart is empty, remove the cart order too
                    if (!StaticDataProvider.OrderItems.Any(oi => oi.OrderID == cartOrder.OrderID))
                    {
                        StaticDataProvider.Orders.Remove(cartOrder);
                    }

                    LoadCart();
                    UIService.ShowSuccess("Item removed from cart");
                }
            }
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            var cartOrder = StaticDataProvider.Orders.FirstOrDefault(o =>
                o.PatientID == AuthService.CurrentPatient.PatientID && o.Status == "Cart");

            if (cartOrder == null || !StaticDataProvider.OrderItems.Any(oi => oi.OrderID == cartOrder.OrderID))
            {
                UIService.ShowError("Your cart is empty");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDeliveryAddress.Text))
            {
                UIService.ShowError("Please enter a delivery address");
                return;
            }

            if (!VerifyInventory(cartOrder, out string outOfStockItems))
            {
                UIService.ShowError($"Some items are not available in requested quantities:\n\n{outOfStockItems}");
                return;
            }

            // Calculate total
            decimal total = StaticDataProvider.OrderItems
                .Where(oi => oi.OrderID == cartOrder.OrderID)
                .Sum(oi => oi.Quantity * oi.UnitPrice);


            // Update order
            cartOrder.DeliveryAddress = txtDeliveryAddress.Text;
            cartOrder.Status = "Pending";
            cartOrder.OrderDate = DateTime.Now;
            cartOrder.TotalAmount = total;


            // Refresh UI
            LoadProducts();
            LoadCart();
            LoadOrders();
            UIService.ShowSuccess("Order placed successfully! It will be processed soon.");
        }

        private void BtnCancelOrder_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                UIService.ShowError("Please select an order to cancel");
                return;
            }

            int orderId = (int)dgvOrders.SelectedRows[0].Cells["OrderID"].Value;
            string status = dgvOrders.SelectedRows[0].Cells["Status"].Value.ToString();

            if (status != "Pending")
            {
                UIService.ShowError("Only pending orders can be cancelled");
                return;
            }

            var order = StaticDataProvider.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order != null)
            {
                // Update order status
                order.Status = "Cancelled";

                // Refresh UI
                LoadProducts();
                LoadOrders();
                UIService.ShowSuccess("Order cancelled successfully");
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

            // Improve text appearance
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Italic);
            button.TextAlign = ContentAlignment.MiddleCenter;

            // Add hover and press effects
            button.MouseEnter += (s, e) => {
                button.BackColor = Color.FromArgb(40, 96, 144); // Darker blue on hover
            };

            button.MouseLeave += (s, e) => {
                button.BackColor = Color.FromArgb(51, 122, 183); // Return to original blue
            };

            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 71, 109); // Even darker when pressed

            // Create slight shadow effect using border
            button.FlatAppearance.BorderColor = Color.FromArgb(40, 96, 144);
        }

        private void DisplayProductDetails(Product product)
        {
            // First, clear the existing items
            _lstProductFeatures.Items.Clear();

            if (product == null)
            {
                _lstProductFeatures.Items.Add("Select a product to view its features");
                return;
            }

            // Parse and add features as separate items in the list
            if (!string.IsNullOrWhiteSpace(product.Features))
            {
                string[] features = product.Features.Split(',');
                foreach (var feature in features)
                {
                    if (!string.IsNullOrWhiteSpace(feature))
                    {
                        _lstProductFeatures.Items.Add($"• {feature.Trim()}");
                    }
                }
            }
            else
            {
                _lstProductFeatures.Items.Add("No features listed for this product");
            }
        }


        private void LoadProducts()
        {
            dgvProducts.Rows.Clear();
            foreach (var product in StaticDataProvider.Products)
            {
                dgvProducts.Rows.Add(
                    product.ProductID,
                    product.Model,
                    product.Manufacturer,
                    product.Price.ToString("C"),
                    product.QuantityInStock
                );
            }
        }

        private void LoadCart()
        {
            dgvCart.Rows.Clear();
            decimal total = 0;

            var cartOrder = StaticDataProvider.Orders.FirstOrDefault(o =>
                o.PatientID == AuthService.CurrentPatient.PatientID && o.Status == "Cart");

            if (cartOrder != null)
            {
                foreach (var item in StaticDataProvider.OrderItems.Where(oi => oi.OrderID == cartOrder.OrderID))
                {
                    var product = StaticDataProvider.Products.First(p => p.ProductID == item.ProductID);
                    decimal itemTotal = item.Quantity * item.UnitPrice;
                    total += itemTotal;

                    dgvCart.Rows.Add(
                        product.ProductID,
                        product.Model,
                        item.Quantity,
                        item.UnitPrice.ToString("C"),
                        itemTotal.ToString("C")
                    );
                }
            }

            lblTotalPrice.Text = $"Total: {total:C}";
        }

        private void LoadOrders()
        {
            dgvOrders.Rows.Clear();
            var patientOrders = StaticDataProvider.Orders
                .Where(o => o.PatientID == AuthService.CurrentPatient.PatientID && o.Status != "Cart")
                .OrderByDescending(o => o.OrderDate);

            foreach (var order in patientOrders)
            {
                foreach (var item in StaticDataProvider.OrderItems.Where(oi => oi.OrderID == order.OrderID))
                {
                    var product = StaticDataProvider.Products.First(p => p.ProductID == item.ProductID);
                    dgvOrders.Rows.Add(
                        order.OrderID,
                        order.OrderDate.ToString("MM/dd/yyyy"),
                        product.Model,
                        product.Manufacturer,
                        item.Quantity,
                        (item.Quantity * item.UnitPrice).ToString("C"),
                        order.Status
                    );
                }
            }
        }

        private Order GetOrCreateCartOrder()
        {
            var cartOrder = StaticDataProvider.Orders.FirstOrDefault(o =>
                o.PatientID == AuthService.CurrentPatient.PatientID && o.Status == "Cart");

            // Create cart if doesn't exist
            if (cartOrder == null)
            {
                cartOrder = new Order
                {
                    OrderID = StaticDataProvider.Orders.Count > 0 ?
                        StaticDataProvider.Orders.Max(o => o.OrderID) + 1 : 1,
                    PatientID = AuthService.CurrentPatient.PatientID,
                    OrderDate = DateTime.Now,
                    DeliveryAddress = AuthService.CurrentPatient.Address,
                    Status = "Cart"
                };
                StaticDataProvider.Orders.Add(cartOrder);
            }

            return cartOrder;
        }

        private bool VerifyInventory(Order order, out string outOfStockItems)
        {
            bool allInStock = true;
            outOfStockItems = "";

            foreach (var item in StaticDataProvider.OrderItems.Where(oi => oi.OrderID == order.OrderID))
            {
                var product = StaticDataProvider.Products.First(p => p.ProductID == item.ProductID);
                if (item.Quantity > product.QuantityInStock)
                {
                    allInStock = false;
                    outOfStockItems += $"{product.Model} (requested: {item.Quantity}, available: {product.QuantityInStock})\n";
                }
            }

            return allInStock;
        }

        #endregion
    }
}