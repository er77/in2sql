using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SqlEngine.in2sqlSvcCloud;
using Excel = Microsoft.Office.Interop.Excel;

namespace SqlEngine
{
    class In2SqlVBAEngineCloud
    {



        public static string setSqlLimit(string vCloudType, string vCurrSql)
        {
            string vSql = intSqlVBAEngine.RemoveSqlLimit(vCurrSql);
              
            if (vCloudType.Contains("CloudCH"))
            {
                if (vSql.ToUpper().Contains("LIMIT") == false)
                    vSql = vSql + Environment.NewLine + "/*`*/ LIMIT " + In2SqlRibbon.vRowCount + " /*`*/ ";
            }
             
            return vSql;
        }


        public static void createExTable(string vCurrCloudName, string vTableName, string vCurrSql = null)
        {
            var vCurrWorkSheet = SqlEngine.currExcelApp.ActiveSheet;
            var vCurrWorkBook = SqlEngine.currExcelApp.ActiveWorkbook;
            var vActivCell = SqlEngine.currExcelApp.ActiveCell;             

            if (vCurrSql == null )
                vCurrSql = "SELECT * FROM " + vTableName; 

            string vConnURL = in2sqlSvcCloud.prepareCloudQuery(vCurrCloudName, vCurrSql );             

            if (vActivCell != null & vConnURL.Length > 1 & vTableName.Length > 1)
            {
                if (vActivCell.Value == null)
                    if (vActivCell.ListObject == null)
                    {
                       
                         string vTempFile = "TEXT;" + In2SqlSvcTool.writeHttpToFile(vConnURL);
                            var xlQueryTable = vCurrWorkSheet.QueryTables.Add(
                                                               Connection: vTempFile
                                                             , Destination: vActivCell
                                                            );

                            xlQueryTable.Name = vCurrCloudName + "|" + vTableName;
                            xlQueryTable.FieldNames = true;
                            xlQueryTable.RowNumbers = false;
                            xlQueryTable.FillAdjacentFormulas = false;
                            xlQueryTable.PreserveFormatting = true; 
                            xlQueryTable.Connection = vTempFile;
                            xlQueryTable.RefreshOnFileOpen = false;
                            xlQueryTable.RefreshStyle = Excel.XlCellInsertionMode.xlInsertDeleteCells;
                            xlQueryTable.SavePassword = false;
                            xlQueryTable.SaveData = true;
                            xlQueryTable.AdjustColumnWidth = true;
                            xlQueryTable.RefreshPeriod = 0;
                            xlQueryTable.TextFilePromptOnRefresh = false;
                            xlQueryTable.TextFileStartRow = 1; 
                            xlQueryTable.TextFileConsecutiveDelimiter = false;
                            xlQueryTable.TextFileTabDelimiter = true;
                            xlQueryTable.TextFileCommaDelimiter = true;
                            xlQueryTable.TextFileSemicolonDelimiter = true;
                            xlQueryTable.TextFileOtherDelimiter = "|"; 
                            xlQueryTable.TextFileSpaceDelimiter = false;
                            xlQueryTable.SourceDataFile = vCurrCloudName + "|" + vCurrSql;
                            xlQueryTable.Refresh(true);                         

                           vTempFile = vTempFile.Replace("TEXT;", "");
                           In2SqlSvcTool.deleteFile(vTempFile);  
                            var qtAddress = xlQueryTable.ResultRange.Address;

                            xlQueryTable.Delete();
                        //, Selection, , xlYes
                        var xlTable  =   vCurrWorkSheet.ListObjects.Add(
                                  SourceType: Excel.XlListObjectSourceType.xlSrcRange
                               , Source: vCurrWorkSheet.Range( qtAddress)
                               , XlListObjectHasHeaders: Excel.XlYesNoGuess.xlYes);
                        xlTable.Name = vCurrCloudName + "|" + vTableName;
                        xlTable.Comment = "CLOUD|" +  vCurrCloudName + "|" + vCurrSql;

                        xlTable.TableStyle = "TableStyleLight13";

                        return;
                       
                    }
            }
            MessageBox.Show(" Please select empty area  in Excel data grid");
        }
    }
}
