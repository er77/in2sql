﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlEngine
{
    public partial class in2SqlWF05SqlEngine : Form
    {
        public in2SqlWF05SqlEngine()
        {
            InitializeComponent();

            ConnectionDropDownMenu();


                LineNumberTextBox.Width = 10;

            // now add each line number to LineNumberTextBox upto last line 

            this.splitContainer2.SplitterDistance = 10;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            
        }

        private void ConnectionDropDownMenu()
        {
            contextMenuSqlConnections.Items.Clear();

            foreach (var vCurrvODBCList in In2SqlSvcODBC.vODBCList)
            {
                ToolStripMenuItem vCurrConnMenu = new ToolStripMenuItem(vCurrvODBCList.OdbcName);
                vCurrConnMenu.Click += Connection_Click;
                contextMenuSqlConnections.Items.Add(vCurrConnMenu);
            }
            SqlConnectionsToolStripDropDown.DropDown = contextMenuSqlConnections;
        }

        private void Connection_Click(object sender, EventArgs e)
        {
            ConnName.Text = sender.ToString();
        }



        public void AddLineNumbers()
        {
            // create & set Point pt to (0,0)    
            Point pt = new Point(0, 0);
            // get First Index & First Line from SqlDocument    
            int First_Index = SqlDocument.GetCharIndexFromPosition(pt);
            int First_Line = SqlDocument.GetLineFromCharIndex(First_Index);
            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively    
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;
            // get Last Index & Last Line from SqlDocument    
            int Last_Index = SqlDocument.GetCharIndexFromPosition(pt);
            int Last_Line = SqlDocument.GetLineFromCharIndex(Last_Index);
            // set Center alignment to LineNumberTextBox    
            LineNumberTextBox.SelectionAlignment = HorizontalAlignment.Center;
            // set LineNumberTextBox text to null & width to getWidth() function value    
            LineNumberTextBox.Text = "";
            if (Last_Line > 100)
                LineNumberTextBox.Width = 24;
            else if (Last_Line > 10)
                LineNumberTextBox.Width = 16;
            else
                LineNumberTextBox.Width = 10;

            // now add each line number to LineNumberTextBox upto last line 

            this.splitContainer2.SplitterDistance = LineNumberTextBox.Width;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            for (int i = First_Line; i <= Last_Line + 2; i++)
            {
                LineNumberTextBox.Text += i + 1 + "\n";
            }
        }

        private void LineNumberTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            SqlDocument.Select();
            LineNumberTextBox.DeselectAll();
        }

        private void SqlDocument_VScroll(object sender, EventArgs e)
        {
            LineNumberTextBox.Text = "";
            AddLineNumbers();
            LineNumberTextBox.Invalidate();
        }

        private void in2SqlWF05SqlEngine_Load(object sender, EventArgs e)
        {
            LineNumberTextBox.Font = SqlDocument.Font;
            SqlDocument.Select();
            AddLineNumbers();
        }


        private void opnSqlDocument()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Text files (.sql)|*.sql",
                Title = "Open Sql Script..."
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
                SqlDocument.Text = sr.ReadToEnd();
            }
            ofd = null;
        }

        private void saveSqlDocument()
        {
            SaveFileDialog svf = new SaveFileDialog
            {
                Filter = "Text files (.sql)|*.sql",
                Title = "Save Sql Script..."
            };
            if (svf.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(svf.FileName);
                sw.Write(SqlDocument.Text);
                sw.Close();
            }
            svf = null;
        }

        BackgroundWorker bw  = new BackgroundWorker();    

        private void EditTollMenu_Click(object sender, EventArgs e)
        {
            In2SqlSvcTool.RunGarbageCollector();

            if (sender.ToString().Contains("New"))
                SqlDocument.Clear();

            else if (sender.ToString().Contains("Open"))
                opnSqlDocument();

            else if (sender.ToString().Contains("Save"))
                saveSqlDocument();

            else if (sender.ToString().Contains("Undo"))
                SqlDocument.Undo();

            else if (sender.ToString().Contains("Redo"))
                SqlDocument.Redo();

            else if (sender.ToString().Contains("Cut"))
                SqlDocument.Cut();

            else if (sender.ToString().Contains("Copy"))
                SqlDocument.Copy();

            else if (sender.ToString().Contains("Paste"))
                SqlDocument.Paste();

            else if (sender.ToString().Contains("sqlRun"))            
                if (bw.IsBusy == false )
                {
                    SqlDocument.ReadOnly = true;
                    string qstr = SqlDocument.SelectedText;

                    if (qstr == "")
                        qstr = SqlDocument.Text;

                    SqlDataResult.SelectAll();
                    SqlDataResult.ClearSelection();

                    /*   bw.DoWork += (obj, ea) => sqlExecuteandDataGrid(qstr);
                       bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
                       bw.RunWorkerAsync();*/
                    sqlExecuteandDataGrid(qstr);
                    SqlDocument.ReadOnly = false;
                }   
            else
                    MessageBox.Show("This Sql Engine is busy. Please create new one", "sql run event",
                                                                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // sqlExecuteandDataGrid(SqlDocument );

            else if (sender.ToString().Contains("SqlConnections"))
                ConnectionDropDownMenu();

            else if (sender.ToString().Contains("Excel"))
            {
                String qstr = SqlDocument.SelectedText;
                if (qstr == "")
                    qstr = SqlDocument.Text;
                intSqlVBAEngine.createExTable(ConnName.Text, In2SqlSvcTool.GetHash(qstr), qstr);
            }

            else
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

     /*   private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Action action = () => SqlDocument.ReadOnly = false;
            SqlDocument.Invoke(action);
        }
        */

        private  void  sqlExecuteandDataGrid(string  SqlCommand)
        {
           // await Task.Delay(1);
           
            if (ConnName.Text.Equals("SQL"))
            {
                MessageBox.Show("Please select Sql connection on the rigth drop-down menu", "sql run event",
                                                                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            { 
                string DsnConn = In2SqlSvcODBC.getODBCProperties(ConnName.Text, "DSNStr");
                if (DsnConn == null | DsnConn == "")
                {
                    MessageBox.Show("Please make the connection by expand list on the left pane ", "sql run event",
                                                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                 
                intSqlVBAEngine.setSqlLimit(ConnName.Text, SqlCommand);

                In2SqlSvcTool.addSqlLog(ConnName.Text, SqlCommand);

                using
                        (OdbcConnection conn = new System.Data.Odbc.OdbcConnection(DsnConn))
                using (OdbcDataAdapter cmnd = new OdbcDataAdapter(SqlCommand, conn))
                {
                    DataTable table = new DataTable();
                    cmnd.Fill(table);

                    /*  Action action1 = () => this.SqlDataResult.DataSource = table;
                      SqlDataResult.Invoke(action1);
                      */
                    this.SqlDataResult.DataSource = table; 

                }
                
            }
            catch (Exception e)
            {
                if ((e.HResult == -2147024809) == false)
                    In2SqlSvcTool.ExpHandler(e, "sqlExecuteandDataGrid");                    
            }
           
             
        }


        private void SqlDocument_TextChanged(object sender, EventArgs e)
        {

            Point pt = SqlDocument.GetPositionFromCharIndex(SqlDocument.SelectionStart);
            if (pt.X == 1)
            {
                AddLineNumbers();
            }

            // getting keywords/functions
            string keywords = in2SqlLibrary.getMsSqlReserved();
            //  string keywords = @"\b(select|from|join|left|right|with|as|union|all|having|group|by)\b";
            MatchCollection keywordMatches = Regex.Matches(SqlDocument.Text.ToUpper(), keywords);

            // getting types/classes from the text 
            string types = @"\b(Console)\b";
            MatchCollection typeMatches = Regex.Matches(SqlDocument.Text, types);

            // getting comments (inline or multiline)
            string comments = @"(\/\/.+?$|\/\*.+?\*\/)";
            MatchCollection commentMatches = Regex.Matches(SqlDocument.Text, comments, RegexOptions.Multiline);

            // getting strings
            string strings = "\".+?\"";
            MatchCollection stringMatches = Regex.Matches(SqlDocument.Text, strings);

            // saving the original caret position + forecolor
            int originalIndex = SqlDocument.SelectionStart;
            int originalLength = SqlDocument.SelectionLength;
            Color originalColor = Color.Black;

            // MANDATORY - focuses a label before highlighting (avoids blinking)
            this.Focus();

            // removes any previous highlighting (so modified words won't remain highlighted)
            SqlDocument.SelectionStart = 0;
            SqlDocument.SelectionLength = SqlDocument.Text.Length;
            SqlDocument.SelectionColor = originalColor;

            // scanning...
            foreach (Match m in keywordMatches)
            {
                SqlDocument.SelectionStart = m.Index;
                SqlDocument.SelectionLength = m.Length;
                SqlDocument.SelectionColor = Color.Blue;
            }

            foreach (Match m in typeMatches)
            {
                SqlDocument.SelectionStart = m.Index;
                SqlDocument.SelectionLength = m.Length;
                SqlDocument.SelectionColor = Color.DarkCyan;
            }

            foreach (Match m in commentMatches)
            {
                SqlDocument.SelectionStart = m.Index;
                SqlDocument.SelectionLength = m.Length;
                SqlDocument.SelectionColor = Color.Green;
            }

            foreach (Match m in stringMatches)
            {
                SqlDocument.SelectionStart = m.Index;
                SqlDocument.SelectionLength = m.Length;
                SqlDocument.SelectionColor = Color.Brown;
            }

            // restoring the original colors, for further writing
            SqlDocument.SelectionStart = originalIndex;
            SqlDocument.SelectionLength = originalLength;
            SqlDocument.SelectionColor = originalColor;

            // giving back the focus
            SqlDocument.Focus();
        }

        private void SqlConnectionsToolStripDropDown_Click(object sender, EventArgs e)
        {

        }
    }
}
