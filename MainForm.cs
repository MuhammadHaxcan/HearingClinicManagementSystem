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
using HearingClinicManagementSystem.UI.Forms;
using HearingClinicManagementSystem.UI.Patient;
using HearingClinicManagementSystem.UI.Constants;
using HearingClinicManagementSystem.Services;

namespace HearingClinicManagementSystem
{
    // MainForm.cs
    public class MainForm : Form
    {
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Form currentForm;

        // Sidebar navigation buttons
        private Button btnDashboard;
        private Button btnManageAppointment;
        private Button btnPurchaseHearingAid;
        private Button btnViewMedicalHistory;
        private Button btnUpdatePersonalInfo;

        public MainForm()
        {
            InitializeLayout();
            SetupSidebar();

            // Initialize static data
            StaticDataProvider.Initialize();

            // Show the dashboard by default
            ShowForm(new DashboardForm());
        }

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
                BackColor = Color.LightGray
            };

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(UIConstants.Padding.Medium)
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);
        }

        private void SetupSidebar()
        {
            // Add logo or app name at the top of sidebar
            Label lblAppName = new Label
            {
                Text = AppStrings.Titles.AppTitle,
                Dock = DockStyle.Top,
                Height = UIConstants.Layout.TopBarHeight,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold),
                BackColor = Color.DarkGray,
                ForeColor = Color.White
            };
            sidebarPanel.Controls.Add(lblAppName);

            // Create navigation buttons
            btnDashboard = CreateSidebarButton(AppStrings.Titles.Dashboard, 0);
            btnManageAppointment = CreateSidebarButton(AppStrings.Titles.ManageAppointment, 1);
            btnPurchaseHearingAid = CreateSidebarButton(AppStrings.Titles.PurchaseHearingAid, 2);
            btnViewMedicalHistory = CreateSidebarButton(AppStrings.Titles.MedicalHistory, 3);
            btnUpdatePersonalInfo = CreateSidebarButton(AppStrings.Titles.UpdatePersonalInfo, 4);

            // Initially hide patient-specific buttons
            btnManageAppointment.Visible = AuthService.IsLoggedIn;
            btnPurchaseHearingAid.Visible = AuthService.IsLoggedIn;
            btnViewMedicalHistory.Visible = AuthService.IsLoggedIn;
            btnUpdatePersonalInfo.Visible = AuthService.IsLoggedIn;

            // Add event handlers for each button
            btnDashboard.Click += (s, e) => ShowForm(new DashboardForm());
            btnManageAppointment.Click += (s, e) => ShowForm(new  ManageAppointmentForm());
            btnPurchaseHearingAid.Click += (s, e) => ShowForm(new PurchaseHearingAidForm());
            btnViewMedicalHistory.Click += (s, e) => ShowForm(new ViewMedicalHistoryForm());
            btnUpdatePersonalInfo.Click += (s, e) => ShowForm(new UpdatePersonalInfoForm());

            // Add the buttons to the sidebar
            sidebarPanel.Controls.Add(btnUpdatePersonalInfo);
            sidebarPanel.Controls.Add(btnViewMedicalHistory);
            sidebarPanel.Controls.Add(btnPurchaseHearingAid);
            sidebarPanel.Controls.Add(btnManageAppointment);
            sidebarPanel.Controls.Add(btnDashboard);
        }

        private Button CreateSidebarButton(string text, int position)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = UIConstants.Layout.SidebarButtonHeight,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
            };

            // Set button appearance
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular);

            // Create hover effect
            btn.MouseEnter += (s, e) => { btn.BackColor = Color.Silver; };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.LightGray; };

            return btn;
        }

        private void ShowForm(Form form)
        {
            // Remove current form
            if (currentForm != null)
            {
                currentForm.Close();
                currentForm.Dispose();
            }

            // Set up and display new form
            currentForm = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(form);
            form.Show();

            // Enable sidebar buttons after login (if LoginSuccess event triggered)
            if (form is DashboardForm dashboard)
            {
                dashboard.LoginSuccess += Dashboard_LoginSuccess;
            }
        }

        private void Dashboard_LoginSuccess(object sender, EventArgs e)
        {
            // Show patient-related buttons after successful login
            btnManageAppointment.Visible = true;
            btnPurchaseHearingAid.Visible = true;
            btnViewMedicalHistory.Visible = true;
            btnUpdatePersonalInfo.Visible = true;
        }
    }
}
