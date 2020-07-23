using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlEngine
{
    public partial class In2SqlWF04EditQuery : Form
    {
        private Microsoft.Office.Interop.Excel.ListObject vCurrTable;
       

        public In2SqlWF04EditQuery()
        {
            

            vCurrTable = SqlEngine.currExcelApp.ActiveCell.ListObject;
             
            if ((vCurrTable == null )  )
            {
                MessageBox.Show("Please select table with SQL query");                 
                return;
            }

            InitializeComponent();
            SqlEditor.Language = FastColoredTextBoxNS.Language.SQL;

            string vSql  = vCurrTable.QueryTable.CommandText;
             
            vSql = intSqlVBAEngine.RemoveBetween(vSql, '`', '`');
            vSql = vSql.Replace("/**/", "");

            SqlEditor.Text = vSql;
             this.Text = "Sql Edit: " + vCurrTable.Name;
          //  this.TableName.Text =  vCurrObject.Name;
            SqlEditor_TextChanged( null, null);

            
        }

      

        private void SqlEditTol_Click(object sender, EventArgs e)
        {
            if (sender.ToString().Contains("New"))
                SqlEditor.Clear();

            else if (sender.ToString().Contains("Open"))
            {
                string vSql;

                vSql = vCurrTable.QueryTable.CommandText;

                vSql = intSqlVBAEngine.RemoveBetween(vSql, '`', '`');
                vSql = vSql.Replace("/**/", "");

                SqlEditor.Text = vSql;
            }

            else if (sender.ToString().Contains("Save"))
                vCurrTable.QueryTable.CommandText = intSqlVBAEngine.setSqlLimit(intSqlVBAEngine.getOdbcNameFromCell(), SqlEditor.Text);

            else if (sender.ToString().Contains("Cut"))
                SqlEditor.Cut();

            else if (sender.ToString().Contains("Copy"))
                SqlEditor.Copy();

            else if (sender.ToString().Contains("Paste"))
            {
                  
                SqlEditor.Paste();
                
            }

            else if (sender.ToString().Contains("Execute"))
            {
                SqlEditor.ReadOnly = true;
                SQLEditToolStrip.Focus();
                vCurrTable.QueryTable.CommandText = intSqlVBAEngine.setSqlLimit(intSqlVBAEngine.getOdbcNameFromObject(vCurrTable), SqlEditor.Text);
                intSqlVBAEngine.objRefreshHistory(vCurrTable);
                SqlEditor.ReadOnly = false;
               // SqlEditor.Focus();
                // vCurrTable.Refresh();
            }

        }

        private void SqlECloseIm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TableName_Click(object sender, EventArgs e)
        {
         //   var vActivCell = SqlEngine.currExcelApp.ActiveCell;
        //    vCurrTable.QueryTable.Name = this.TableName.Text ;
        }

        private void SQLEditToolStrip_VisibleChanged(object sender, EventArgs e)
        {
           // vCurrObject.Name = this.TableName.Text;

        }        

        private void SqlEditor_TextChanged(object sender, EventArgs e)
        {
 
           
        }

        private void SqlEditor_Load(object sender, EventArgs e)
        {
            SqlEditor.Language = FastColoredTextBoxNS.Language.SQL;
        }
    }
}
