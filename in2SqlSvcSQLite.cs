using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlEngine
{
    class in2SqlSvcSQLite
    { 
        public struct SQLiteObjectsAndProperties
        {
            public List<String> objColumns;
            public List<String> objIndexes;
            public List<String> objDependencies;
            public String ObjName; 
        }

        public static int vIdtbl = 0;

        public struct SQLiteObjects
        {
            public String Name;
            public String DatabaseFileName;
            public int idTbl;
            public List<String> Tables;
            public List<String> Views;
        }

        public struct SQLiteDataBases
        {
            public string FolderName, Path;
            public List<SQLiteObjects> DataBases;            

        }

        public static List<SQLiteDataBases> vDataBaseList = DataBaseList();

        public static List<SQLiteObjectsAndProperties> vSQLiteObjProp = new List<SQLiteObjectsAndProperties>();
 
        public static string getDBFileName(string vSQLiteFolder, string vSQLiteName)
        {
            try
            {
                var vCurrFolder = vDataBaseList.Find(item => item.FolderName == vSQLiteFolder); 
                return vCurrFolder.Path + '\\' + vSQLiteName;
            }
            catch (Exception e)
            {
                In2SqlSvcTool.ExpHandler(e, "getDBFileName");
                return null;
            }
        }

  

        public static List<SQLiteDataBases> DataBaseList()
        {
            try
            {
                List<SQLiteDataBases> listDataBase = new List<SQLiteDataBases>();
                listDataBase.AddRange(getSQLiteCatalogs());
                return listDataBase;
            }
            catch (Exception e)
            {
                In2SqlSvcTool.ExpHandler(e, "CloudList");
                return null;
            }
        }

        public static IEnumerable<SQLiteObjects> getFileList(string vCurrFolderName)
        {
            SQLiteDataBases vCurrFolderN = vDataBaseList.Find(item => item.FolderName == vCurrFolderName);

            return getDBFilesFromFolder(vCurrFolderN.Path);

        }

        public static IEnumerable<SQLiteObjects> getDBFilesFromFolder(string vFolderPath)
        {
            DirectoryInfo d = new DirectoryInfo(@vFolderPath);
            FileInfo[] Files = d.GetFiles("*.db");          
            foreach (FileInfo file in Files)
            { 
                SQLiteObjects vObj = new SQLiteObjects();
                vObj.Name = file.Name;
                vObj.DatabaseFileName = vFolderPath + "\\" + file.Name;
                yield return vObj;
            }
        } 

        public static IEnumerable<String> SQLiteReadDataValue(string vSQLiteDBFile, string queryString = "")
        {
            if (File.Exists(vSQLiteDBFile))
                using (SQLiteConnection connect = new SQLiteConnection(@"Data Source=" + vSQLiteDBFile))
            {
                connect.Open();
                using (SQLiteCommand fmd = connect.CreateCommand())
                {
                    fmd.CommandText = queryString;
                    SQLiteDataReader r = fmd.ExecuteReader();
                    while (r.Read())
                    {
                        yield return r["value"].ToString();
                    }
                    connect.Close();
                }
            }
            else
            {
                MessageBox.Show("DB file " + vSQLiteDBFile + " was not found ", " SQLiteReadDataValue ",
                                                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static IEnumerable<String> getSQLiteViewList(string vSQLiteFileName)
        {
            var vViews = SQLiteReadDataValue(vSQLiteFileName, in2SqlLibrary.getSqlViews( "SQLITE"));
            foreach (var vCurrView in vViews)
            { 
                yield return vCurrView.ToString();
            }
        }

        public static IEnumerable<String> getSQLiteTableList(string vSQLiteFileName)
        {
            var vTables = SQLiteReadDataValue(vSQLiteFileName, in2SqlLibrary.getSqlTables("SQLITE"));
            foreach (var vCurrTable in vTables)
            { 
                yield return vCurrTable.ToString();
            }
        }

        public static IEnumerable<SQLiteObjectsAndProperties> getObjectProperties(string vDBName, string vObjName, string vDBFileName)
        {
           
            string vSql = in2SqlLibrary.getSQLTableColumn("SQLITE");

            vSql = vSql.Replace("%TNAME%", vObjName);             

            var vObjects = SQLiteReadDataValue(vDBFileName, vSql);

            SQLiteObjectsAndProperties vObject = new SQLiteObjectsAndProperties();
            vObject.ObjName = vDBName + '.' + vObjName;
            vObject.objColumns = new List<string>();

            foreach (var vCurrObject in vObjects)
            {
                vObject.objColumns.Add(vCurrObject);
            }

            vSql = in2SqlLibrary.getSQLIndexes("SQLITE");
           
                vSql = vSql.Replace("%TNAME%", vObjName);            

            vObjects = SQLiteReadDataValue(vDBName, vSql);
            vObject.objIndexes = new List<string>();

            foreach (var vCurrObject in vObjects)
            {
                vObject.objIndexes.Add(vCurrObject);
            } 

            yield return vObject;
        }

        public static IEnumerable<SQLiteDataBases> getSQLiteCatalogs()
        {
            RegistryKey vCurrRegKey = Registry.CurrentUser.OpenSubKey(@"Software\in2sql");
        
            if (vCurrRegKey != null)
            {
                SQLiteDataBases vFolderProp ;

                foreach (string name in vCurrRegKey.GetValueNames())
                {
                    if (name.Contains("SQLite"))
                    {                        
                        string[] vNameDetails = name.Split('.');

                        if (vNameDetails.Count() < 2)
                        {
                            MessageBox.Show("Error in reading registry getCsvList ");
                            yield return new SQLiteDataBases();
                            break;
                        }

                        vFolderProp = new SQLiteDataBases();

                        vFolderProp.FolderName = vNameDetails[1];                         

                        if (name.Contains("Path"))
                        {  vFolderProp.Path = in2SqlRegistry.getLocalRegValue(vCurrRegKey, name);
                            yield return vFolderProp;                           
                        }
                    }                 
                }
            }
        }



        public static void dumpSQLiteToCsv(string vDBCatalogName, string vDBName,  string vSqlCommand, string vCsvFile, int vShoMsgBOx = 1 )
        {
            try { 

                string vSQLiteDBFile = getDBFileName(vDBCatalogName, vDBName);
                int i = 0;
                if (File.Exists(vSQLiteDBFile))
                using (SQLiteConnection connect = new SQLiteConnection(@"Data Source=" + vSQLiteDBFile))
                {
                    connect.Open();

                    In2SqlSvcTool.addSqlLog(vSQLiteDBFile, vSqlCommand);
                    using (SQLiteCommand fmd = connect.CreateCommand())
                    {
                        fmd.CommandText = vSqlCommand;
                        SQLiteDataReader r = fmd.ExecuteReader();

                        object[] output = new object[r.FieldCount];
                        using (var textWriter = new StreamWriter(@vCsvFile))
                        {
                            var writer = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
                            writer.Configuration.Delimiter = ",";
                            writer.Configuration.ShouldQuote = (field, context) => true;

                            for (int j = 0; j < r.FieldCount; j++)
                            {
                                output[j] = r.GetName(j);
                                writer.WriteField(r.GetName(j));
                            }

                            writer.NextRecord();

                            while (r.Read())
                            {
                                r.GetValues(output);
                                writer.WriteField(output);
                                writer.NextRecord();
                                i++;
                            }
                            connect.Close();
                        }
                    }
                    if (vShoMsgBOx==1 )
                        MessageBox.Show("Export completed. \n\r File name is " + vCsvFile + " \n\r Row count:" + i, "csv export",
                                                                             MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                else
                {
                    MessageBox.Show("DB file " + vSQLiteDBFile + " was not found " , " dumpSQLiteToCsv ",
                                                                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
               
            }
            catch (Exception e)
            {
                if (e.HResult != -2147024809)
                    In2SqlSvcTool.ExpHandler(e, "dumpOdbctoCsv");
            }
        }

    }
}
