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

            string vSql  = vCurrTable.QueryTable.CommandText;
             
            vSql = intSqlVBAEngine.RemoveBetween(vSql, '`', '`');
            vSql = vSql.Replace("/**/", "");

            SqlEditor.Text = vSql;
             this.Text = "Sql Edit: " + vCurrTable.Name;
          //  this.TableName.Text =  vCurrObject.Name;
            SqlEditor_TextChanged( null, null);
            

        }

        int colored = 1;

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
                 colored = 0;
                SqlEditor.Paste();
                colored = 1;
            }

            else if (sender.ToString().Contains("Execute"))
            {
                vCurrTable.QueryTable.CommandText = intSqlVBAEngine.setSqlLimit(intSqlVBAEngine.getOdbcNameFromObject(vCurrTable), SqlEditor.Text);
                vCurrTable.Refresh();
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
            if (colored == 1)
            {
                // getting keywords/functions
                string keywords = in2SqlLibrary.getMsSqlReserved();

                MatchCollection keywordMatches = Regex.Matches(SqlEditor.Text.ToUpper(), keywords);

                // getting types/classes from the text 
                string types = @"\b(Console)\b";
                MatchCollection typeMatches = Regex.Matches(SqlEditor.Text, types);

                // getting comments (inline or multiline)
                string comments = @"(\/\/.+?$|\/\*.+?\*\/)";
                MatchCollection commentMatches = Regex.Matches(SqlEditor.Text, comments, RegexOptions.Multiline);

                // getting strings
                string strings = "\".+?\"";
                MatchCollection stringMatches = Regex.Matches(SqlEditor.Text, strings);

                // saving the original caret position + forecolor
                int originalIndex = SqlEditor.SelectionStart;
                int originalLength = SqlEditor.SelectionLength;
                Color originalColor = Color.Black;

                // MANDATORY - focuses a label before highlighting (avoids blinking)
                this.Focus();

                // removes any previous highlighting (so modified words won't remain highlighted)
                SqlEditor.SelectionStart = 0;
                SqlEditor.SelectionLength = SqlEditor.Text.Length;
                SqlEditor.SelectionColor = originalColor;

                // scanning...
                foreach (Match m in keywordMatches)
                {
                    SqlEditor.SelectionStart = m.Index;
                    SqlEditor.SelectionLength = m.Length;
                    SqlEditor.SelectionColor = Color.Blue;
                }

                foreach (Match m in typeMatches)
                {
                    SqlEditor.SelectionStart = m.Index;
                    SqlEditor.SelectionLength = m.Length;
                    SqlEditor.SelectionColor = Color.DarkCyan;
                }

                foreach (Match m in stringMatches)
                {
                    SqlEditor.SelectionStart = m.Index;
                    SqlEditor.SelectionLength = m.Length;
                    SqlEditor.SelectionColor = Color.Brown;
                }

                foreach (Match m in commentMatches)
                {
                    SqlEditor.SelectionStart = m.Index;
                    SqlEditor.SelectionLength = m.Length;
                    SqlEditor.SelectionColor = Color.Green;
                }

                // restoring the original colors, for further writing
                SqlEditor.SelectionStart = originalIndex;
                SqlEditor.SelectionLength = originalLength;
                SqlEditor.SelectionColor = originalColor;

                // giving back the focus
                SqlEditor.Focus();
            }
        }
    }
}
