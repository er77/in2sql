using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SqlEngine.in2sqlSvcCloud;
using static SqlEngine.In2SqlSvcCsv;

namespace SqlEngine
{
    class in2SqlRightPaneTreeCloud
    {
      
            private static void initCloudObjects(ref in2sqlSvcCloud.CloudProperties vCurrCloudObj)
            {
                try
                {
                    if (vCurrCloudObj.Tables == null)
                    {
                        vCurrCloudObj.Tables = new List<in2sqlSvcCloud.CloudObjects>();
                    }

                    if (vCurrCloudObj.Views == null)
                    {
                        vCurrCloudObj.Views = new List<in2sqlSvcCloud.CloudObjects>();
                    }

                    if (vCurrCloudObj.SQLProgramms == null)
                    {
                        vCurrCloudObj.SQLProgramms = new List<in2sqlSvcCloud.CloudObjects>();
                    }

                    if (vCurrCloudObj.SQLFunctions == null)
                    {
                        vCurrCloudObj.SQLFunctions = new List<in2sqlSvcCloud.CloudObjects>();
                    }

                    if (vCurrCloudObj.Tables.Count == 0)
                    {
                        vCurrCloudObj.Tables.AddRange(in2sqlSvcCloud.getCloudTableList(vCurrCloudObj.CloudName));
                    }

                    if (vCurrCloudObj.Views.Count == 0)
                    {
                        vCurrCloudObj.Views.AddRange(in2sqlSvcCloud.getCloudViewList(vCurrCloudObj.CloudName));
                    }                     

                }
                catch (Exception er)
                {
                    In2SqlSvcTool.ExpHandler(er, "initCloudObjects");
                }
            }


        private static void initCsvObjects(ref In2SqlSvcCsv.FolderProperties vCurrCloudObj)
        {
            try
            {
                if (vCurrCloudObj.Files == null)
                {
                    vCurrCloudObj.Files = new List<In2SqlSvcCsv.CloudObjects>();
                } 

                if (vCurrCloudObj.Files.Count == 0)
                {
                    vCurrCloudObj.Files.AddRange(In2SqlSvcCsv.getFileList(vCurrCloudObj.FolderName));
                }                 
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "initCloudObjects");
            }
        }

        public static void getCsvFilesList (TreeNodeMouseClickEventArgs e)
        {
            e.Node.Nodes.Clear();
            string vCurrFolderName = e.Node.Text;
             
            FolderProperties vCurrFolder = In2SqlSvcCsv.vFolderList.Find(item => item.FolderName == vCurrFolderName);

            try
            {
               
                
               /* e.Node.SelectedImageIndex = 22;
                e.Node.Tag = "CSV#";
                TreeNode vNodeTableFolder = new TreeNode("Tables".ToString(), 3, 3);
                vNodeTableFolder.Tag = vCurrFolder.FolderName + "_csv";
                e.Node.Nodes.Add(vNodeTableFolder);
                */
                initCsvObjects(ref vCurrFolder);

                foreach (var vCurrFile in vCurrFolder.Files)
                {
                    TreeNode vNodeTable = new TreeNode(vCurrFile.Name, 22, 22);
                    vNodeTable.Tag = vCurrFolder.FolderName + "|" + vCurrFile.Name + "|$FILE_CSV$";
                    e.Node.Nodes.Add(vNodeTable);
                    TreeNode vNodeColumnTbl = new TreeNode(" ".ToString(), 99, 99);
                    vNodeColumnTbl.Tag = vCurrFolder.FolderName + "." + vCurrFile.Name;
                    vNodeTable.Nodes.Add(vNodeColumnTbl);
                }
 
                return;
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "getCsvFilesList 1 ");
            }
        }

        public static void getCloudTablesAndViews(TreeNodeMouseClickEventArgs e)
            {
                e.Node.Nodes.Clear();
                string vCurrCloudName = e.Node.Text; 

                CloudProperties vCurrCloud = in2sqlSvcCloud.vFolderList.Find(item => item.CloudName == vCurrCloudName);

                try
                {
                    if ((in2sqlSvcCloud.checkCloudState(vCurrCloudName) < 0))
                    {
                        return;
                    }

                    e.Node.ImageIndex = 2;
                    e.Node.SelectedImageIndex = 2;
                    e.Node.Tag = vCurrCloud.CloudType + '#';
                    TreeNode vNodeTableFolder = new TreeNode("Tables".ToString(), 3, 3);
                    vNodeTableFolder.Tag = vCurrCloud.CloudName + "_tf";
                    e.Node.Nodes.Add(vNodeTableFolder);

                    initCloudObjects(ref vCurrCloud);

                    foreach (var vCurrTable in vCurrCloud.Tables)
                    {
                        TreeNode vNodeTable = new TreeNode(vCurrTable.Name, 4, 4);
                        vNodeTable.Tag = vCurrCloud.CloudName + "|" + vCurrTable.Name + "|$TABLE_CLD$";
                        vNodeTableFolder.Nodes.Add(vNodeTable);
                        TreeNode vNodeColumnTbl = new TreeNode(" ".ToString(), 99, 99);
                        vNodeColumnTbl.Tag = vCurrCloud.CloudName + "." + vCurrTable.Name;
                        vNodeTable.Nodes.Add(vNodeColumnTbl);
                    }

                    TreeNode vNodeViewFolder = new TreeNode("Views".ToString(), 5, 5);
                    vNodeViewFolder.Tag = vCurrCloud.CloudName + "_vf";
                    e.Node.Nodes.Add(vNodeViewFolder);

                    foreach (var vCurrView in vCurrCloud.Views)
                    {
                        TreeNode vNodeView = new TreeNode(vCurrView.Name, 6, 6);
                        vNodeView.Tag = vCurrCloud.CloudName + "." + vNodeView.Name + "|$VIEW_CLD$";
                        vNodeViewFolder.Nodes.Add(vNodeView);
                        TreeNode vNodeColumnVw = new TreeNode(" ".ToString(), 99, 99);
                        vNodeColumnVw.Tag = vCurrCloud.CloudName + "." + vNodeView.Name;
                        vNodeView.Nodes.Add(vNodeColumnVw);
                    }
                    return;
                }
                catch (Exception er)
                {
                    In2SqlSvcTool.ExpHandler(er, "treeODBC_NodeMouseClick 1 ");
                }
            }

        public static void getCsvHeaders (TreeNodeMouseClickEventArgs e)
        {
            try
            {
                String vNodeTag = e.Node.Parent.Text + '.' + e.Node.Text;

                FilesAndProperties vCurrObjProp = In2SqlSvcCsv.vFileObjProp.Find(item => item.ObjName == vNodeTag); 

                if (vCurrObjProp.objColumns == null)
                {
                    In2SqlSvcCsv.vFileObjProp.AddRange(In2SqlSvcCsv.getCsvFileColumn(e.Node.Parent.Text, e.Node.Text));
                    vCurrObjProp = In2SqlSvcCsv.vFileObjProp.Find(item => item.ObjName == vNodeTag);
                }

                if (vCurrObjProp.objColumns != null)
                    if (vCurrObjProp.objColumns.Count > 0)
                    {
                        e.Node.Nodes.Clear();

                        foreach (var vCurrColumn in vCurrObjProp.objColumns)
                        {
                            TreeNode vNodeColumn = new TreeNode(vCurrColumn.ToString(), 14, 14);
                            vNodeColumn.Tag = vNodeTag + '.' + vCurrColumn + "_clm";
                            e.Node.Nodes.Add(vNodeColumn);
                        }
                    }
            }
            catch (Exception er)
            {
                In2SqlSvcTool.ExpHandler(er, "getColumnsandIndexes ");
            }

        }


        public static void getColumnsAndIndexes(TreeNodeMouseClickEventArgs e)
            {
                try
                {
                    String vNodeTag = e.Node.Parent.Parent.Text + '.' + e.Node.Text;
                    var vCurrObjProp = in2sqlSvcCloud.vCloudObjProp.Find(item => item.ObjName == vNodeTag);

                    if (vCurrObjProp.objColumns == null)
                    {
                    in2sqlSvcCloud.vCloudObjProp.AddRange(in2sqlSvcCloud.getObjectProperties(e.Node.Parent.Parent.Text , e.Node.Text  ));
                        vCurrObjProp = in2sqlSvcCloud.vCloudObjProp.Find(item => item.ObjName == vNodeTag);
                    }

                    if (vCurrObjProp.objColumns != null)
                        if (vCurrObjProp.objColumns.Count > 0)
                        {
                            e.Node.Nodes.Clear();

                            foreach (var vCurrColumn in vCurrObjProp.objColumns)
                            {
                                TreeNode vNodeColumn = new TreeNode(vCurrColumn.ToString(), 14, 14);
                                vNodeColumn.Tag = vNodeTag + '.' + vCurrColumn + "_clm";
                                e.Node.Nodes.Add(vNodeColumn);
                            }
                            if (e.Node.Tag.ToString().Contains("$TABLE$"))
                            {
                                e.Node.Tag = vNodeTag + ".TABLE";
                                TreeNode vNodeIndexFolder = new TreeNode("Indexes".ToString(), 12, 12);
                                vNodeIndexFolder.Tag = vNodeTag + "_idx";
                                e.Node.Nodes.Add(vNodeIndexFolder);
                                foreach (var vCurrIndx in vCurrObjProp.objIndexes)
                                {
                                    TreeNode vNodeIndx = new TreeNode(vCurrIndx.ToString(), 13, 13);
                                    vNodeIndx.Tag = vNodeTag + '.' + vCurrIndx + "_idx";
                                    vNodeIndexFolder.Nodes.Add(vNodeIndx);
                                }
                            }
                            else
                            {
                                e.Node.Tag = vNodeTag + ".VIEW";
                            }
                        }
                }
                catch (Exception er)
                {
                    In2SqlSvcTool.ExpHandler(er, "getColumnsandIndexes ");
                }

            }
   

}
}
