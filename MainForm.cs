using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using HearingClinicManagementSystem.Services;
using HearingClinicManagementSystem.UI;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.UI.Forms;
using HearingClinicManagementSystem.UI.Patient;
using HearingClinicManagementSystem.UI.Receptionist;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ManageAppointmentForm = HearingClinicManagementSystem.UI.Patient.ManageAppointmentForm;

namespace HearingClinicManagementSystem
{
    public class MainForm : Form
    {
        #region Fields
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Form currentForm;
        private Dictionary<string, Button> sidebarButtons = new Dictionary<string, Button>();
        private bool dashboardOpened = false; // Flag to track if dashboard has been opened
        #endregion

        public MainForm()
        {
            InitializeLayout();
            InitializeSidebar();

            // Initialize static data
            StaticDataProvider.Initialize();

            // Show the dashboard by default
            OpenForm(new DashboardForm());
        }

        #region UI Setup
        private void InitializeLayout()
        {
            // Set form properties
            this.Text = AppStrings.Titles.AppTitle;
            this.Size = new Size(UIConstants.Forms.DefaultFormWidth, UIConstants.Forms.DefaultFormHeight);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create panels
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = UIConstants.Size.SidebarWidth,
                BackColor = Color.FromArgb(51, 51, 76), // Dark blue-gray for professional look
                Padding = new Padding(0, 0, 0, 20) // Add bottom padding
            };

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(UIConstants.Padding.Medium),
                BackColor = Color.White
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);
        }

        private void InitializeSidebar()
        {
            // Add logo or app name at the top of sidebar
            Label lblAppName = new Label
            {
                Text = AppStrings.Titles.AppTitle,
                Dock = DockStyle.Top,
                Height = UIConstants.Layout.TopBarHeight,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 39, 58), // Darker than sidebar
                ForeColor = Color.White
            };
            sidebarPanel.Controls.Add(lblAppName);

            // Add separator below the header
            Panel separator = new Panel
            {
                Height = 2,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(73, 73, 108) // Slightly lighter than sidebar
            };
            sidebarPanel.Controls.Add(separator);

            // Always add Dashboard button
            AddSidebarButton("Dashboard", () => OpenForm(new DashboardForm()));

            // Set up the rest of the sidebar based on current login state
            UpdateSidebarForCurrentUser();

            // Subscribe to login success event
            UIService.UserLoggedIn += UIService_UserLoggedIn;
            UIService.UserLoggedOut += UIService_UserLoggedOut;
        }

        private void UpdateSidebarForCurrentUser()
        {
            // Clear existing menu items except Dashboard
            foreach (var key in new List<string>(sidebarButtons.Keys))
            {
                if (key != "Dashboard")
                {
                    var btn = sidebarButtons[key];
                    sidebarPanel.Controls.Remove(btn);
                    btn.Dispose();
                    sidebarButtons.Remove(key);
                }
            }

            // No need to add Dashboard button again, it's already there

            if (!AuthService.IsLoggedIn)
                return;

            // Add menu items based on user role
            switch (AuthService.CurrentUser.Role)
            {
                case "Patient":
                    AddPatientMenuItems();
                    break;
                case "Receptionist":
                    AddReceptionistMenuItems();
                    break;
                case "Audiologist":
                    AddAudiologistMenuItems();
                    break;
                case "InventoryManager":
                    AddInventoryManagerMenuItems();
                    break;
                case "ClinicManager":
                    AddClinicManagerMenuItems();
                    break;
            }
        }

        private Button AddSidebarButton(string text, Action clickAction)
        {
            // If the button already exists, remove it and create a new one
            // This is a more reliable approach than trying to detach event handlers
            if (sidebarButtons.TryGetValue(text, out Button existingButton))
            {
                sidebarPanel.Controls.Remove(existingButton);
                existingButton.Dispose();
                sidebarButtons.Remove(text);
            }

            // Create a new button
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = UIConstants.Layout.SidebarButtonHeight,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(51, 51, 76), // Match sidebar color
                ForeColor = Color.Gainsboro,
                Image = null, // Can add icons here if needed
                ImageAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Regular)
            };

            // Set button appearance
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 25, 38);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 64, 92);

            // Add click event handler
            if (clickAction != null)
            {
                btn.Click += (s, e) => clickAction();
            }

            // Store the button in our dictionary
            sidebarButtons[text] = btn;

            // Add to sidebar
            sidebarPanel.Controls.Add(btn);

            return btn;
        }

        private void AddPatientMenuItems()
        {
            AddSidebarButton("Manage Appointments", () => OpenForm(new ManageAppointmentForm()));
            AddSidebarButton("Purchase Hearing Aid", () => OpenForm(new PurchaseHearingAidForm()));
            AddSidebarButton("View Medical History", () => OpenForm(new ViewMedicalHistoryForm()));
            AddSidebarButton("Update Personal Info", () => OpenForm(new UpdatePersonalInfoForm()));
        }

        private void AddReceptionistMenuItems()
        {
            AddSidebarButton("Manage Appointments", () => OpenForm(new UI.Receptionist.ManageAppointmentForm()));
            AddSidebarButton("Create Appointment", () => OpenForm(new UI.Receptionist.CreateAppointmentForm()));
            AddSidebarButton("Process Payments.", () => OpenForm(new UI.Receptionist.PaymentCollectionForm()));
            // Future receptionist menu items will be added here
        }

        private void AddAudiologistMenuItems()
        {
            // Future audiologist menu items will be added here
            AddSidebarButton("Hearing Test", () => OpenForm(new UI.Audiologist.HearingTestForm()));
        }

        private void AddInventoryManagerMenuItems()
        {
            // Future inventory manager menu items will be added here
            AddSidebarButton("Manage Inventory", null);
        }

        private void AddClinicManagerMenuItems()
        {
            // Future clinic manager menu items will be added here
            AddSidebarButton("View Reports", null);
        }
        #endregion

        #region Event Handlers
        private void UIService_UserLoggedIn(object sender, EventArgs e)
        {
            UpdateSidebarForCurrentUser();

            // No need to open dashboard again as the login was handled on existing dashboard
            // Just make sure the current form is refreshed to reflect login state
            if (currentForm is DashboardForm)
            {
                // Update the current form to reflect login changes
                DisplayCurrentForm();
            }
        }

        private void UIService_UserLoggedOut(object sender, EventArgs e)
        {
            UpdateSidebarForCurrentUser();

            // Open dashboard after logout if not already on dashboard
            if (!(currentForm is DashboardForm))
            {
                OpenForm(new DashboardForm());
            }
            else
            {
                // Update the current form to reflect logout changes
                DisplayCurrentForm();
            }
        }
        #endregion

        #region Helper Methods
        private void OpenForm(Form form)
        {
            // If we're already on a dashboard and trying to open another one, skip
            if (currentForm is DashboardForm && form is DashboardForm)
            {
                return;
            }

            // Remove current form
            if (currentForm != null)
            {
                currentForm.Close();
                currentForm.Dispose();
                currentForm = null;
            }

            // Set up and display new form
            currentForm = form;
            DisplayCurrentForm();
        }

        private void DisplayCurrentForm()
        {
            if (currentForm == null) return;

            currentForm.TopLevel = false;
            currentForm.FormBorderStyle = FormBorderStyle.None;
            currentForm.Dock = DockStyle.Fill;

            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(currentForm);
            currentForm.Show();
        }
        #endregion
    }
}