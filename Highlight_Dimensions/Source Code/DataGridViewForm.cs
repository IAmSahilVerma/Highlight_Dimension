namespace Highlight_Dimensions
{
    using NXOpen.Drawings;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using NXOpen;

    public partial class DataGridViewForm : Form
    {
        public DataGridViewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewForm_Load(object sender, EventArgs e)
        {
            // Updating the Windows Form with data grid view
            GuiInitialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public void GuiInitialize()
        {
            StyleDataGridView();

            // Initializing number of columns
            dataGridView1.ColumnCount = 2;
           
            // Setting header text in first row
            dataGridView1.Columns[0].Name = "Sr. No.";
            dataGridView1.Columns[1].Name = "Sheet Name";
            dataGridView1.BackgroundColor = Color.Gray;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            // List contains all key from the GetDrawingSheets() dictionary
            List<int> sheetKeyList = new List<int>();
            List<DrawingSheet> sheetKeyList2 = new List<DrawingSheet>();
            foreach (KeyValuePair<int, DrawingSheet> sheetKey in AsoociativeDimensions.GetDrawingSheets())
            {
                sheetKeyList.Add(sheetKey.Key);
                sheetKeyList2.Add(sheetKey.Value);
            }

            // Looping through GetDrawingSheets() dictionary to populate table grid view rows
            foreach (KeyValuePair<int, DrawingSheet> dictKeyValue in AsoociativeDimensions.GetDrawingSheets())
            {
                object[] gridobjects = new object[2];
                gridobjects[0] = dictKeyValue.Key;
                gridobjects[1] = dictKeyValue.Value.Name;
                dataGridView1.Rows.Add(gridobjects);

            }

            // Adding button control in data grid view
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Name = "Highlight",
                Text = "Highlight",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(btn);
            
            int dataGridViewRowHeight = dataGridView1.RowTemplate.Height;
            this.Height = dataGridViewRowHeight * dataGridView1.Rows.Count + 75;
            this.Width = dataGridView1.Width;
        }

        /// <summary>
        /// Data grid view button handler event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Passes an object specific to the event that is being handled.</param>
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                this.Hide();
                AsoociativeDimensions.HighlightAssociativeDimension(e.RowIndex + 1);
                dataGridView1.ClearSelection();
                this.Show();
                this.Focus();
            }
        }

        /// <summary>
        /// Styling for the Data Grid View in Windows Form
        /// </summary>
        private void StyleDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToResizeColumns = true;
            dataGridView1.BorderStyle = BorderStyle.FixedSingle;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Green;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.BackgroundColor = Color.FromArgb(30, 30, 30);
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;//optional
            dataGridView1.EnableHeadersVisualStyles = false; //absolute necessary
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 10);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridView1.BackgroundColor = Color.Gray;
        }
    }
}
