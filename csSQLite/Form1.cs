using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace csSQLite
{
    public partial class Form1 : Form
    {
        SqliteHelper db;

        public Form1()
        {
            InitializeComponent();
            Setup24hourPanel();
            db = new SqliteHelper(); 
            db.SqlConnect();
        }

        void Setup24hourPanel()
        {
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Padding = new Padding(1);
            int height = flowLayoutPanel1.Height / 8 - 8;
            int width = flowLayoutPanel1.Width / 3 - 3;

            for (int i = 0; i < 24; i++)
            {
                Button btn = new Button();
                //btn.Name = "btn" + dt.Rows[i][1];
                //btn.Tag = dt.Rows[i][1];
                //btn.Text = dt.Rows[i][2].ToString();
                //btn.
                btn.Font = new Font("Arial", 14f, FontStyle.Bold);
                // btn.UseCompatibleTextRendering = true;
                btn.BackColor = Color.Green;
                btn.Height = height;// 57;
                btn.Width = width; // 116;
                btn.Click += button1_Click;   //  set any method
                //btn.Enter += button1_Enter;   // 
                //btn.Leave += button1_Leave;   //
                btn.Padding = new Padding(1);
                flowLayoutPanel1.Controls.Add(btn);                
            }
        }

        void button1_Click(object sender, EventArgs e)
        {
            
        }



        private void button1_Click()
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'reportingappDataSet.report_table' table. You can move, or remove it, as needed.
            this.report_tableTableAdapter.Fill(this.reportingappDataSet.report_table);

        }
    }
}
