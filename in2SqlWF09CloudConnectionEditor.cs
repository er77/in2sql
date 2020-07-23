﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlEngine
{
    public partial class in2SqlWF09CloudConnectionEditor : Form
    {
        string vConnType = "CH";

        public in2SqlWF09CloudConnectionEditor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            int pX = Cursor.Position.X - 200;
            int pY = Cursor.Position.Y - 50;

            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(pX, pY);

            WF09BTOk.Enabled = false;
            rbClickHouse.Enabled = true;


        }

        private void WF09BTOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WF09BTCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WF09BTTest_Click(object sender, EventArgs e)
        {
            string vSqlURL;

            vSqlURL = in2sqlSvcCloud.prepareCloudQuery(tbURL.Text, tbSQL.Text, tbLogin.Text, tbPassword.Text);

            vSqlURL = In2SqlSvcTool.HttpGet(vSqlURL);
            
            if (vSqlURL.Length < 2)
            {
                MessageBox.Show("Test Failed");
                return;
            }

             MessageBox.Show("Test Passed ");
            WF09BTOk.Enabled = true;

            vSqlURL = "Cloud" + vConnType + '.' + tbName.Text;

            in2SqlRegistry.setLocalValue(vSqlURL, "Url" , tbURL.Text) ;
            in2SqlRegistry.setLocalValue(vSqlURL, "Login", tbLogin.Text);
            in2SqlRegistry.setLocalValue(vSqlURL, "Password", tbPassword.Text);      

        }

        private void rbClickHouse_CheckedChanged(object sender, EventArgs e)
        {
            vConnType = "CH";
        }
    }
}