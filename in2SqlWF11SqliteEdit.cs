using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static SqlEngine.In2SqlSvcCsv;


namespace SqlEngine
{
    public partial class in2SqlWF11SqliteEdit : Form
    {
        private string vCurrConnection;

        public in2SqlWF11SqliteEdit()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            int pX = Cursor.Position.X - 300;
            int pY = Cursor.Position.Y - 30;

            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(pX, pY);

            refreshList();

        }

        private void refreshList()
        {
            In2SqlSvcCsv.vFolderList = In2SqlSvcCsv.FolderList();
            CMBoxConnection.Items.Clear();
            foreach (var vCurrFolder in In2SqlSvcCsv.vFolderList)
            {
                CMBoxConnection.Items.Add(vCurrFolder.FolderName);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            In2SqlSvcCsv.vFolderList = In2SqlSvcCsv.FolderList();
            this.Close();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (vCurrConnection.Length > 2)
            {
                in2SqlRegistry.delLocalValue("Csv." + vCurrConnection);
                vCurrConnection = "";

                CsvName.Text = "";
                CsvPath.Text = "";
                refreshList();


            }
        }

        private void wf10Create_Click(object sender, EventArgs e)
        {
            in2SqlRegistry.setLocalValue("Csv." + CsvName.Text, "Path", CsvPath.Text);
            refreshList();

        }

        private void wf10Browse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    CsvPath.Text = fbd.SelectedPath;

                }
            }
        }

        private void CMBoxConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            vCurrConnection = CMBoxConnection.SelectedItem.ToString();
            

            CsvName.Text = vCurrConnection;

            FolderProperties vCurrFolderN = vFolderList.Find(item => item.FolderName == vCurrConnection);

            CsvPath.Text = vCurrFolderN.Path;
        }
    }
}
