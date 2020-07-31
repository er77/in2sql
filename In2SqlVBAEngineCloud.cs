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
            string vConnURL = in2sqlSvcCloud.prepareCloudQuery(vCurrCloud.Url, vCurrSql, vCurrCloud.Login, vCurrCloud.Password);
             

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
                       // xlQueryTable.TextFileParseType = Excel.xlDelimited;
                       // xlQueryTable.TextFileTextQualifier = Excel.xlTextQualifierDoubleQuote;
                        xlQueryTable.TextFileConsecutiveDelimiter = false;
                        xlQueryTable.TextFileTabDelimiter = true;
                        xlQueryTable.TextFileSemicolonDelimiter = true;
                        xlQueryTable.TextFileOtherDelimiter = "|";
                        xlQueryTable.TextFileCommaDelimiter = false;
                        xlQueryTable.TextFileSpaceDelimiter = false;
                        xlQueryTable.SourceDataFile = vCurrCloudName + "|" + vCurrSql;
                        xlQueryTable.Refresh(true);                         

                       vTempFile = vTempFile.Replace("TEXT;", "");
                        File.Delete(vTempFile);
                        var qtAddress = xlQueryTable.ResultRange.Address;


                        xlQueryTable.Delete();
                        /*  Microsoft.Office.Interop.Excel.ListObject table = vCurrWorkSheet.ListObjects.Add(
                                           SourceType: Excel.XlSourceType.xlSourceRange
                                         , Destination: vActivCell //qtAddress
                                         , LinkSource: false
                                         , Type.Missing
                                         , XlListObjectHasHeaders :Excel.XlYesNoGuess.xlYes
                                         );

     Microsoft.Office.Interop.Excel.ListObject table = vCurrWorkSheet.ListObjects.Add(
                                           SourceType: Excel.XlSourceType.xlSourceRange
                                         , Source: vCurrWorkSheet.Range[qtAddress]
                                         , Destination: vCurrWorkSheet.Cells(vActivCell.Row, vActivCell.Column)
                                         , LinkSource: false
                                         , XlListObjectHasHeaders: Excel.XlYesNoGuess.xlYes
                                       );
                                          , Type.Missing
                                         */
                        /*
                        Microsoft.Office.Interop.Excel.ListObject table = vCurrWorkSheet.ListObjects.Add(
                                            Excel.XlSourceType.xlSourceRange
                                         , vCurrWorkSheet.Cells(vActivCell.Row, vActivCell.Column) //qtAddress
                                         , Excel.XlYesNoGuess.xlYes
                                      
                                       );
                               table.Name =  vTableName + "|" + vCurrCloudName;
                               table.Comment = vCurrCloudName + "|" + vCurrSql;
                               table.TableStyle = "TableStyleLight13";
                              
                        */
                        // MessageBox.Show(vActivCell.QueryTable.SourceDataFile);


                        /*
                         var xlQueryTable = vCurrWorkSheet.QueryTables.Add ( 
                                                           Connection: vConnURL
                                                           , Destination:vActivCell 

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
                         xlQueryTable.WebTables = "2";
                         xlQueryTable.WebPreFormattedTextToColumns = true;

                         xlQueryTable.Refresh(true);
                         */
                        return;
                    }
            }
            MessageBox.Show(" Please select empty area  in Excel data grid");
        }
    }
}
