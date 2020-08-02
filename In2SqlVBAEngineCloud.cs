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
        //https://stackoverflow.com/questions/23835828/add-csv-connection-to-excel-with-c-sharp
        /*
         * string strFileName = textpath + "\\filename.csv";
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + System.IO.Path.GetDirectoryName(strFileName) +"; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"");
            conn.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM " + System.IO.Path.GetFileName(strFileName), conn);
            DataSet ds = new DataSet("Temp");
            adapter.Fill(ds);
            DataTable tb = ds.Tables[0];

        --

            ActiveWorkbook.Connections.Add(Name:="MnM3" _
                             , Description:="eWE" _
                             , ConnectionString:=Array(Array( _
                                "ODBC;DBQ=C:\USERS\ADMINISTRATOR\DOWNLOADS;DefaultDir=C:\USERS\ADMINISTRATOR\DOWNLOADS;Driver={Microsoft Access Text Driver (*.txt, *"), Array(".csv)};DriverId=27;Extensions=txt,csv,tab,asc;FIL=text;MaxBufferSize=2048;MaxScanRows=25;PageTimeout=5;SafeTransactions=0;Threa"), Array("ds=3;UID=admin;UserCommitSync=Yes;")) _
                             , CommandText:=Array("SELECT  * " & Chr(13) & "" & Chr(10) & "FROM r2.csv r2") _
                             , lCmdtype:=xlCmdSql).Name = "MnM3"

         */

        public static void createExTable(string vCurrCloudName, string vTableName, string vCurrSql = null)
        {
            var vCurrWorkSheet = SqlEngine.currExcelApp.ActiveSheet;
            var vCurrWorkBook = SqlEngine.currExcelApp.ActiveWorkbook;
            var vActivCell = SqlEngine.currExcelApp.ActiveCell;

            CloudProperties vCurrCloud = in2sqlSvcCloud.vCloudList.Find(item => item.CloudName == vCurrCloudName);
 

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
  
                        return;
                    }
            }
            MessageBox.Show(" Please select empty area  in Excel data grid");
        }
    }
}
