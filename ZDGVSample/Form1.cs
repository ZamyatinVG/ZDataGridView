using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using ZDGV;

namespace ZDGVSample
{
    public partial class Form1 : Form
    {
        public OleDbConnection connection;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1100, 500);
            DGVWithFilter DG = new DGVWithFilter();
            DG.Bounds = new Rectangle(10, 10, 986, 440);
            DG.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right | AnchorStyles.Bottom)));
            this.Controls.Add(DG);
            try
            {
                connection = new OleDbConnection("Provider=MSDAORA.1;User ID=;Data Source=askona;Extended Properties=;Persist Security Info=False;Password=");
                connection.Open();
            }
            catch (Exception exp)
            {
                MessageBox.Show("Ошибка соединения с базой!\n\n" + exp, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            try
            {
                DataSet ds = new DataSet();
                OleDbDataAdapter da = new OleDbDataAdapter("select * from table(gal_asup.label.GetPrintData('8001000000075666'))", connection);
                da.Fill(ds);
                DG.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            for (int i = 0; i < DG.Rows.Count; i++)
                //switch (Convert.ToInt16(DG.Rows[i].Cells[6].Value) % 4)
                switch (i % 4)
                {
                    case 0:
                        DG.Rows[i].Cells[5].Style.BackColor = Color.Bisque;
                        break;
                    case 1:
                        DG.Rows[i].Cells[5].Style.BackColor = Color.Chocolate;
                        break;
                    case 2:
                        DG.Rows[i].Cells[5].Style.BackColor = Color.DarkKhaki;
                        break;
                }
        }
    }
}