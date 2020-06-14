using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.Odbc;
using System.Data.Common;


namespace SqlEngine
{
    public partial class in2SqlWF03PanelRigtSqlM : UserControl
    {
        TreeNode miSelectNode;

        public static in2SqlWF03PanelRigtSqlM CurrSqlPanel;

        

        static void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // doneInitSQlObjects = true;
        }

         public void ShowOdbcTree() 
        {
            try
            { 
                this.splitContainer1.Panel2Collapsed = true;
                this.splitContainer1.Panel2.Hide(); 
                this.ODBCtabControl.Dock  = System.Windows.Forms.DockStyle.Fill;                
            }
            catch { }
        }

        public void showSqlEdit()
        { 
            try
            {
                this.splitContainer1.Panel2Collapsed = false;
                this.splitContainer1.Panel2.Show();                
                this.splitContainer1.SplitterDistance = 205;               
             //   this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
                this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            }
            catch { }
        } 

        private   void initParam ()
        {
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Panel2.Hide();
            this.splitContainer1.AutoSize = true;
            this.ODBCtabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeODBC.Dock = System.Windows.Forms.DockStyle.Fill;
           // this.splitContainer2.SplitterDistance = 400; // = new Size(300, 300);

            /// grid prop 
         //   this.SqlDataResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
          //  this.SqlDataResult.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

         //   this.SqlDataResult.AllowUserToOrderColumns = true;
         //   this.SqlDataResult.AllowUserToResizeColumns = true;
          //  this.SqlDataResult.AutoResizeColumns();

            this.treeODBC.NodeMouseClick +=
                new TreeNodeMouseClickEventHandler(this.treeODBC_NodeMouseClick);


            this.treeODBC.KeyPress +=
                new KeyPressEventHandler(this.in2SqlRightPane_KeyPress);

        
            this.treeODBC.AllowDrop = true;
            //     this.SqlDocument.AllowDrop = true;


 
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Add("N1");
            tabControl1.TabPages[tabControl1.TabCount - 1].Text = "S0" + tabControl1.TabCount;

            in2SqlWF05SqlEngine sqlTab = new in2SqlWF05SqlEngine();
            sqlTab.TopLevel = false;
            sqlTab.FormBorderStyle = FormBorderStyle.None;
            sqlTab.Parent = tabControl1.TabPages[tabControl1.TabCount - 1];
            sqlTab.Dock = DockStyle.Fill;
            sqlTab.Show();
        }

        in2SqlWF06SqlBuilder sqlBuild = null;

        private void SqlBuild ()
        {
            sqlBuild = new in2SqlWF06SqlBuilder();
            sqlBuild.TopLevel = false;
            sqlBuild.FormBorderStyle = FormBorderStyle.None;
            sqlBuild.Parent = ODBCtabControl.TabPages[1];
            sqlBuild.Dock = DockStyle.Fill;
            sqlBuild.Show();
               
        }

        in2SqlWF07OdbcProperties OdbcEdit =  null;

        private void OdbcEditPanel ()
        {
            OdbcEdit = new in2SqlWF07OdbcProperties();
           // OdbcEdit.TopLevel = false;
           // OdbcEdit.FormBorderStyle = FormBorderStyle.None;
            OdbcEdit.Parent = ODBCtabControl.TabPages[2];
            OdbcEdit.Dock = DockStyle.Fill;
            OdbcEdit.Show();

        }

        

        public in2SqlWF03PanelRigtSqlM()
        {
            try
            {
                InitializeComponent();
                PopulateOdbcTreeView();
                initParam();
                newToolStripButton_Click(null, null);

                SqlBuild();
              //  OdbcEditPanel();
                CurrSqlPanel = this;

            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "in2SqlRightPane");
            }
        }

        private void treeODBC_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = (TreeNode)e.Item;
            if ( node.Level > 0)
            {
                DoDragDrop(node.Text, DragDropEffects.Copy);
            }
        }



        public void PopulateOdbcTreeView(int vIsUI = 0)
        {
            try
            {
                TreeNode rootNode = new TreeNode("ODBC".ToString());
                rootNode.Tag = "root";
                GetODbcRecords(rootNode, vIsUI);
                treeODBC.Nodes.Add(rootNode);

                TreeNode rootTable = new TreeNode("Excel".ToString(), 15, 15);
                rootTable.Tag = "excel";
                //  getExcelRecords(rootTable);
                TreeNode vNodeExcelSheet = new TreeNode(" ".ToString(), 99, 99);
                rootTable.Nodes.Add(vNodeExcelSheet);
                treeODBC.Nodes.Add(rootTable);

            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "PopulateOdbcTreeView");
            }
        }

        private void GetODbcRecords(TreeNode nodeToAddTo, int vIsUI = 0)
        {
            try
            {
                if (vIsUI == 0)
                {
                    foreach (var vCurrvODBCList in In2SqlSvcODBC.vODBCList)
                    {
                        in2SqlRightPaneTreeTables.setODBCTreeLineSimple(nodeToAddTo, vCurrvODBCList.OdbcName);
                    }
                    return;
                }
                if (vIsUI == 1)
                {

                    foreach (var vCurrvODBCList in In2SqlSvcODBC.vODBCList)
                    {
                        In2SqlSvcODBC.checkOdbcStatus(vCurrvODBCList.OdbcName);
                        in2SqlRightPaneTreeTables.setODBCTreeLineComplex(nodeToAddTo, vCurrvODBCList.OdbcName, vCurrvODBCList.OdbcName);
                    }
                    return;
                }
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "GetODbcRecords");
            }
        }

        private void refreshTreeOdbc_all(TreeNodeMouseClickEventArgs e)
        {
            try
            {
                e.Node.Nodes.Clear();
                PopulateOdbcTreeView(1);
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "refreshTreeOdbc_all");
            }
        }

        private ContextMenuStrip createMenu(TreeNodeMouseClickEventArgs e, String[] vMenu, EventHandler myFuncName, ContextMenuStrip vCurrMenu)
        {            
            if (e.Node.ContextMenuStrip == null)
            {
                if (vCurrMenu.Items.Count < 1 )
                {
                    vCurrMenu.Items.Clear();

                    foreach (string rw in vMenu)
                    {
                        ToolStripMenuItem vMenuRun = new ToolStripMenuItem(rw);
                        vMenuRun.Click += myFuncName;
                        vCurrMenu.Items.Add(vMenuRun);
                    }
                }
                e.Node.ContextMenuStrip = vCurrMenu;
            }
            return vCurrMenu;
        }         

        private void rootMenu_Click(object sender, EventArgs e)
        {
            if (sender.ToString().Contains("Edit"))
               In2SqlSvcTool.RunCmdLauncher("odbcad32");

            else if (sender.ToString().Contains("Refresh"))
            {
                miSelectNode.Nodes.Clear();
                GetODbcRecords(miSelectNode, 0);
            }

            else if (sender.ToString().Contains("ReConnect"))
            {
                miSelectNode.ImageIndex = 1;
                miSelectNode.Collapse();
                miSelectNode.SelectedImageIndex = 1;
                miSelectNode.Tag = "ODBC$";
            }

            else if (sender.ToString().Contains("Login"))
            {
                in2SqlWF01Login frm1 = new in2SqlWF01Login(miSelectNode.Text);//.Sho
                frm1.Show();
                miSelectNode.ImageIndex = 1;
                miSelectNode.Collapse();
                miSelectNode.SelectedImageIndex = 1;
                miSelectNode.Tag = "ODBC$";
            }
        } 

        private void RefreshExcel(TreeNode nodeToAddTo)
        {
            // intSqlVBAEngine.createExTable(miSelectNode.Parent.Parent.Text, miSelectNode.Text);
            nodeToAddTo.Nodes.Clear();

            foreach (var sheet in SqlEngine.currExcelApp.ActiveSheet.Parent.Worksheets)
            {
                TreeNode vNodeExcelSheet = new TreeNode(sheet.name, 16, 16);
                nodeToAddTo.Nodes.Add(vNodeExcelSheet);
                foreach (var vObj in sheet.ListObjects)
                {
                    TreeNode vNodeExcelObject = new TreeNode(vObj.name, 17, 17);
                    vNodeExcelObject.Tag = "ExTable " + vObj.name;
                    vNodeExcelSheet.Nodes.Add(vNodeExcelObject);
                }
            }
        }

        private TreeNode fTN = null;

        private void  FindTreeNode(TreeNode treeNode , string NodeName )
        { 

            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (fTN == null)
                    if (tn.Text.Equals(NodeName))
                    {
                        fTN = tn;
                    }
                    else
                        FindTreeNode(tn, NodeName);
                else
                    break;
            } 
        }


        public   TreeNode findeTable( string NodeName , string vODBCName )
        {

            // Find the node specified by the user.
            fTN = null;
            foreach (TreeNode tn in this.treeODBC.Nodes) {
                if (tn.Text.Equals("ODBC"))
                { foreach (TreeNode tn1 in tn.Nodes )
                    {
                        if (tn1.Text.Equals(vODBCName))
                        {
                            FindTreeNode(tn1, NodeName);
                        }
                        if ((fTN == null) == false)
                            break;
                    }
                   
                }
            }

            return fTN; 
         }

         

        private void miRefreshExcel_Click(object sender, EventArgs e)
        {            
            RefreshExcel(miSelectNode); 
        }
 

        private void ExTableMenu_Click(object sender, EventArgs e)
        {
            if (sender.ToString().Contains("Rename"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (sender.ToString().Contains("Edit Sql"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (sender.ToString().Contains("Refresh"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (sender.ToString().Contains("Properties"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (sender.ToString().Contains("Delete"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
        } 
 

        private void RegularObjecteMenu_Click(object sender, EventArgs e)
        {

            if (sender.ToString().Contains("PivotTable"))
                intSqlVBAEngine.createPivotTable(miSelectNode.Parent.Parent.Text, miSelectNode.Text);

            else if (sender.ToString().Contains("Table"))
                intSqlVBAEngine.createExTable(miSelectNode.Parent.Parent.Text, miSelectNode.Text);
 
            else if (sender.ToString().Contains("Chart"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (sender.ToString().Contains("Editor"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (sender.ToString().Contains("Properties"))
                MessageBox.Show(string.Concat("You have Clicked '", sender.ToString(), "' Menu"), "Menu Items Event",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

         private void treeODBC_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                miSelectNode = treeODBC.GetNodeAt(e.X, e.Y);

                if (e.Node.Tag != null)
                {
                    if ((e.Button == MouseButtons.Right) & (e.Node.Tag.ToString().Contains("root")))
                    {
                       
                        contextMenuRootODBC = createMenu( 
                                        e
                                    , new String[] { "Edit", "Refresh" }
                                    , rootMenu_Click
                                    , contextMenuRootODBC);
                        return;

                    }

                    if ((e.Button == MouseButtons.Right) & (e.Node.Tag.ToString().Contains("excel")))
                    {
                        contextMenuExcelRoot = createMenu (
                                     e
                                    , new String[] { "Refresh" }
                                    , miRefreshExcel_Click
                                    , contextMenuExcelRoot);
                        return;
                    }

                    if ((e.Button == MouseButtons.Right) & (e.Node.Tag.ToString().Contains("ExTable")))
                    {
                        contextMenuExTable = createMenu(
                                        e
                                    , new String[] { "Rename", "Edit Sql", "Refresh", "Properties", "Delete" }
                                    , ExTableMenu_Click
                                    , contextMenuExTable);
                        return;
                    }

                    if ((e.Button == MouseButtons.Left) & (e.Node.Tag.ToString().Contains("excel")))
                    {
                        RefreshExcel(e.Node);
                        return;
                    }

                    if ((e.Button == MouseButtons.Left) & (e.Node.Tag.ToString().Contains("ODBC$")))
                    {                        
                        in2SqlRightPaneTreeTables.getTablesAndViews(e);
                        sqlBuild.setLblConnectionName(e.Node.Text);                     
                        return;
                    }

                    if ((e.Button == MouseButtons.Right) & (e.Node.Tag.ToString().Contains("ODBC%")))
                    {
                        contextMenuOdbcError = createMenu(
                                    e
                                    , new String[] { "ReConnect", "Edit", "Login" }
                                    , rootMenu_Click
                                    , contextMenuOdbcError);
                        return;
                    }

                    if ((e.Node.Tag.ToString().Contains("VIEW") | e.Node.Tag.ToString().Contains("TABLE")))
                    {
                        if (e.Button == MouseButtons.Left | e.Node.Tag.ToString().Contains('$') )
                        {
                            in2SqlRightPaneTreeTables.getColumnsandIndexes(e);
                            return;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            this.contextMenuTable = createMenu(
                                    e
                                    , new String[] { "to Table", "to PivotTable", "to Chart", "to Sql Editor", "get Properties" }
                                    , RegularObjecteMenu_Click
                                    , this.contextMenuTable);
                            return;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "treeODBC_NodeMouseClick 2 ");
            }

        }

        private void treeODBC_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                /*  string selectedNodeText = e.Node.Text;
                  DialogResult  result = MessageBox.Show(selectedNodeText + "  1");
                  */
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "treeODBC_AfterSelect");
            }
        }


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "treeODBC_AfterSelect");
            }
        }



        private void in2SqlRightPane_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u0003')  //Control+C
                if (treeODBC.SelectedNode != null)
                {
                    Clipboard.SetText(treeODBC.SelectedNode.Text);
                }
        }


    }
}
