using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SqlEngine.in2sqlSvcCloud;
using Excel = Microsoft.Office.Interop.Excel;

namespace SqlEngine
{
    class In2SqlVBAEngineCloud
    {

        public static void createExTable(string vCurrCloudName, string vTableName, string vCurrSql = null)
        {
            var vCurrWorkSheet = SqlEngine.currExcelApp.ActiveSheet;
            var vCurrWorkBook = SqlEngine.currExcelApp.ActiveWorkbook;
            var vActivCell = SqlEngine.currExcelApp.ActiveCell;

            CloudProperties vCurrCloud = in2sqlSvcCloud.vCloudList.Find(item => item.CloudName == vCurrCloudName);
            //public string CloudName, CloudType, Url, Login, Password;

            // SqlEngine.currExcelApp.SheetChange += CurrExcelApp_SheetChange;

            if (vCurrSql == null )
                vCurrSql = "SELECT * FROM " + vTableName;
            vCurrSql = vCurrSql + " FORMAT TabSeparatedWithNames";
            string vConnURL = "URL;" +  in2sqlSvcCloud.prepareCloudQuery(vCurrCloud.Url, vCurrSql, vCurrCloud.Login, vCurrCloud.Password);
             

            if (vActivCell != null & vConnURL.Length > 1 & vTableName.Length > 1)
            {
                if (vActivCell.Value == null)
                    if (vActivCell.ListObject == null)
                    {
                        /*
                             var connections = vCurrWorkBook.Connections.Add(
                                                           Name: "In2Sql|" + vODBC + "|" + vTableName
                                                         , Description: vSql
                                                         , ConnectionString: vDSN
                                                         , CommandText: vSql
                                                         , lCmdtype: Excel.XlCmdType.xlCmdSql);
                        Microsoft.Office.Interop.Excel.ListObject table = vCurrWorkSheet.ListObjects.Add(
                                                 SourceType: Excel.XlListObjectSourceType.xlSrcQuery
                                               , Source: connections
                                               , Destination: vCurrWorkSheet.Cells(vActivCell.Row, vActivCell.Column));
                         */
                        var xlQueryTable = vCurrWorkSheet.QueryTables.Add (
                                                         //  Name: "Cloud|" + vCurrCloudName + "|" + vTableName
                                                         // Description: vCurrCloudName + "|" + vCurrSql
                                                          Connection: vConnURL
                                                          , Destination:vActivCell 
                                                        /* , FieldNames = true
                                                         , RefreshOnFileOpen = false
                                                         , BackgroundQuery = false
                                                         , RefreshStyle = Excel.XlCellInsertionMode.xlOverwriteCells
                                                         , SavePassword = true
                                                         , SaveData = true
                                                         , AdjustColumnWidth = true
                                                         , RefreshPeriod = 0 
                                                         , WebTables = "2"
                                                         , WebPreFormattedTextToColumns = true
                                                        */
                                                         );
                        xlQueryTable.Name= "Cloud|" + vCurrCloudName + "|" + vTableName;
                        xlQueryTable.FieldNames = true;
                        xlQueryTable.RefreshOnFileOpen = false;
                        xlQueryTable.BackgroundQuery = true;
                        xlQueryTable.RefreshStyle = Excel.XlCellInsertionMode.xlOverwriteCells;
                        xlQueryTable.SavePassword = true;
                        xlQueryTable.SaveData = true;
                        xlQueryTable.AdjustColumnWidth = true;
                        xlQueryTable.RefreshPeriod = 0;
                       // xlQueryTable.Refresh  = true;
                        xlQueryTable.WebTables = "2";
                        xlQueryTable.WebPreFormattedTextToColumns = true;

                       /* Microsoft.Office.Interop.Excel.ListObject xlQueryTable = vCurrWorkSheet.ListObjects.Add(
                                            SourceType: Excel.XlListObjectSourceType.xlSrcQuery
                                          , Source: xlConn 
                                          ,  Destination: vCurrWorkSheet.Cells(vActivCell.Row, vActivCell.Column) );
                       */
                        xlQueryTable.Refresh(true);
                        return;
                    }
            }
            System.Windows.Forms.MessageBox.Show(" Please select empty area  in Excel data grid");
        }
    }
}
