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
using HearingClinicManagementSystem.UI.Constants;

namespace HearingClinicManagementSystem.UI.Common
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    namespace HearingClinicManagementSystem.UI.Common
    {
        /// <summary>
        /// Base form that provides common functionality and styling for all forms in the application
        /// </summary>
        public class BaseForm : Form
        {
            public BaseForm()
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Dock = DockStyle.Fill;
                this.Padding = new Padding(UIConstants.Padding.Medium);
                this.BackColor = Color.White;
            }

            protected Label CreateLabel(string text, int x, int y, int width = -1, ContentAlignment alignment = ContentAlignment.MiddleLeft)
            {
                width = width > 0 ? width : UIConstants.Size.StandardTextBoxWidth;

                return new Label
                {
                    Text = text,
                    Location = new Point(x, y),
                    Width = width,
                    TextAlign = alignment,
                    AutoSize = false
                };
            }

            protected TextBox CreateTextBox(int x, int y, string text = "", bool isPassword = false, int width = -1)
            {
                width = width > 0 ? width : UIConstants.Size.StandardTextBoxWidth;

                var textBox = new TextBox
                {
                    Location = new Point(x, y),
                    Width = width,
                    Text = text
                };

                if (isPassword)
                {
                    textBox.UseSystemPasswordChar = true;
                }

                return textBox;
            }

            protected Button CreateButton(string text, int x, int y, EventHandler clickHandler = null, int width = -1, int height = -1)
            {
                width = width > 0 ? width : UIConstants.Size.StandardButtonWidth;
                height = height > 0 ? height : UIConstants.Size.StandardButtonHeight;

                var button = new Button
                {
                    Text = text,
                    Location = new Point(x, y),
                    Width = width,
                    Height = height,
                    FlatStyle = FlatStyle.Standard,
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White
                };

                if (clickHandler != null)
                {
                    button.Click += clickHandler;
                }

                return button;
            }

            protected Label CreateTitleLabel(string text)
            {
                return new Label
                {
                    Text = text,
                    Dock = DockStyle.Top,
                    Height = UIConstants.Layout.TopBarHeight,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold),
                    ForeColor = Color.DarkSlateBlue
                };
            }

            protected DataGridView CreateDataGrid(int height, bool allowUserToAddRows = false, bool readOnly = true)
            {
                return new DataGridView
                {
                    Dock = DockStyle.Top,
                    Height = height,
                    AllowUserToAddRows = allowUserToAddRows,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ReadOnly = readOnly,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    RowHeadersVisible = false
                };
            }

            protected ComboBox CreateComboBox(int x, int y, int width = -1)
            {
                width = width > 0 ? width : UIConstants.Size.StandardTextBoxWidth;

                return new ComboBox
                {
                    Location = new Point(x, y),
                    Width = width,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
            }

            protected DateTimePicker CreateDatePicker(int x, int y, int width = -1)
            {
                width = width > 0 ? width : UIConstants.Size.StandardTextBoxWidth;

                return new DateTimePicker
                {
                    Location = new Point(x, y),
                    Width = width,
                    Format = DateTimePickerFormat.Short
                };
            }

            protected NumericUpDown CreateNumericUpDown(int x, int y, int width = -1)
            {
                width = width > 0 ? width : 60;

                return new NumericUpDown
                {
                    Location = new Point(x, y),
                    Width = width
                };
            }
        }
    }
}
