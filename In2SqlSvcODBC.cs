using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlEngine
{

    class In2SqlSvcODBC
    {

        static int vIdtbl;

        public struct SqlObjects
        {
            public String Name;
            public int idTbl;
        }



        public struct ObjectsAndProperties
        {
            public List<String> objColumns;
            public List<String> objIndexes;
            public List<String> objDependencies;
            public String ObjName;
        }


        public struct OdbcProperties
        {
            public string OdbcName, Database, Description, Driver, LastUser, Server, ConnErrMsg, ConnErrType, DBType, Login, Password, DSNStr;
            public int ConnStatus;
            public List<SqlObjects> Tables;
            public List<SqlObjects> Views;
            public List<SqlObjects> SQLProgramms;
            public List<SqlObjects> SQLFunctions;
        }



        public static List<OdbcProperties> vODBCList = ODBCList();

        public static List<ObjectsAndProperties> vObjProp = new List<ObjectsAndProperties>();

        public static void ChangeOdbcValue(String vOdbcName, OdbcProperties vNewOdbcValue)
        {
            try
            {
                int vOdbcIndex = In2SqlSvcODBC.vODBCList.FindIndex(item => item.OdbcName == vOdbcName);
                vODBCList[vOdbcIndex] = vNewOdbcValue;
            }
            catch (Exception e)
            {
                In2SqlSvcTool.ExpHandler(e, "ChangeOdbcValue");
            }

        }

        // var vCurrODBC = In2SqlSvcODBC.vODBCList.Find(item => item.OdbcName == vCurrvODBCList.OdbcName);

        public static string getODBCProperties(string vODBCName, string vProperties)
        {
            try
            {
                var vCurrODBC = In2SqlSvcODBC.vODBCList.Find(item => item.OdbcName == vODBCName);
                string vCurrProp = "";
                if (vProperties.Contains("Database")) vCurrProp = vCurrODBC.Database;
                else if (vProperties.Contains("Description")) vCurrProp = vCurrODBC.Description;
                else if (vProperties.Contains("Driver")) vCurrProp = vCurrODBC.Driver;
                else if (vProperties.Contains("LastUser")) vCurrProp = vCurrODBC.LastUser;
                else if (vProperties.Contains("ConnErrMsg")) vCurrProp = vCurrODBC.ConnErrMsg;
                else if (vProperties.Contains("DBType")) vCurrProp = vCurrODBC.DBType;
                else if (vProperties.Contains("Login")) vCurrProp = vCurrODBC.Login;
                else if (vProperties.Contains("Password")) vCurrProp = vCurrODBC.Password;
                else if (vProperties.Contains("DSNStr")) vCurrProp = vCurrODBC.DSNStr;
                return vCurrProp;
            }
            catch (Exception e)
            {
                In2SqlSvcTool.ExpHandler(e, "getODBCProperties");
                return null;
            }
        }

        public static List<OdbcProperties> ODBCList()
        {
            try
            {
                vIdtbl = 0;
                List<OdbcProperties> listOdbcProperties = new List<OdbcProperties>();
                listOdbcProperties.AddRange(getODBCList(Registry.CurrentUser));
                listOdbcProperties.AddRange(getODBCList(Registry.LocalMachine));
                return listOdbcProperties;
            }
            catch (Exception e)
            {
                In2SqlSvcTool.ExpHandler(e, "ODBCList");
                return null;
            }
        }



        public static IEnumerable<OdbcProperties> getODBCList(RegistryKey rootKey)
        {
            RegistryKey regKey = rootKey.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
            if (regKey != null)
            {
                foreach (string name in regKey.GetValueNames())
                {

                    OdbcProperties vOdbcProperties = new OdbcProperties();
                    vOdbcProperties.OdbcName = regKey.GetValue(name, "").ToString();
                    if (vOdbcProperties.OdbcName.Contains("Microsoft ") == false)
                    {
                        try
                        {
                            vOdbcProperties.OdbcName = name;
                            RegistryKey vCurrRegKey = rootKey.OpenSubKey(@"Software\ODBC\ODBC.INI\" + name);
                            vOdbcProperties.Database = in2SqlRegistry.getLocalRegValue(vCurrRegKey, "Database");
                            vOdbcProperties.Description = in2SqlRegistry.getLocalRegValue(vCurrRegKey, "Description");
                            vOdbcProperties.Driver = in2SqlRegistry.getLocalRegValue(vCurrRegKey, "Driver");
                            vOdbcProperties.LastUser = in2SqlRegistry.getLocalRegValue(vCurrRegKey, "LastUser");
                            vOdbcProperties.Server = in2SqlRegistry.getLocalRegValue(vCurrRegKey, "Server");
                            vOdbcProperties.ConnStatus = 0;
                            vCurrRegKey = Registry.CurrentUser.OpenSubKey(@"Software\in2sql");
                            vOdbcProperties.Login = in2SqlRegistry.getLocalRegValue(vCurrRegKey, name + '.' + "Login");
                            vOdbcProperties.Password = in2SqlRegistry.getLocalRegValue(vCurrRegKey, name + '.' + "Password");
                        }
                        catch (Exception e)
                        {
                            In2SqlSvcTool.ExpHandler(e, "ODBCList");
                        }
                        yield return vOdbcProperties;
                    }
                }
            }
        }





        public static IEnumerable<String> SqlReadDataValue(string vOdbcName, string queryString = "")
        {
            var vCurrODBC = In2SqlSvcODBC.vODBCList.Find(item => item.OdbcName == vOdbcName);

            using
                  (OdbcConnection conn = new System.Data.Odbc.OdbcConnection())
            {
                using (OdbcCommand cmnd = new OdbcCommand(queryString, conn))
                {
                    try
                    {
                        vCurrODBC.DSNStr = "DSN=" + vOdbcName;
                        if (vCurrODBC.Login != null)
                        {
                            vCurrODBC.DSNStr = vCurrODBC.DSNStr + ";Uid=" + vCurrODBC.Login + ";Pwd=" + vCurrODBC.Password + ";";
                        }

                        conn.ConnectionString = vCurrODBC.DSNStr;
                        conn.ConnectionTimeout = 5;
                        conn.Open();
                    }
                    catch (Exception e)
                    {
                        In2SqlSvcTool.ExpHandler(e, "In2SqlSvcODBC.ReadData", queryString);
                        conn.Close();
                        conn.Dispose();
                        yield break;
                    }
                    OdbcDataReader rd = cmnd.ExecuteReader();
                    while (rd.Read())
                    {
                        yield return rd["value"].ToString();//.Split(',').ToList();  ;
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public static void checkOdbcStatus(string vOdbcName)
        {

            var vCurrODBC = In2SqlSvcODBC.vODBCList.Find(item => item.OdbcName == vOdbcName);
            try
            {
                vCurrODBC.ConnStatus = 0;
                vCurrODBC.ConnErrMsg = "";
                vCurrODBC.ConnErrType = "";
                vCurrODBC.DSNStr = "DSN=" + vOdbcName;
                if (vCurrODBC.Login != null)
                {
                    vCurrODBC.DSNStr = vCurrODBC.DSNStr + ";Uid=" + vCurrODBC.Login + ";Pwd=" + vCurrODBC.Password + ";";
                }

                using (OdbcConnection conn = new System.Data.Odbc.OdbcConnection())
                {
                    conn.ConnectionString = vCurrODBC.DSNStr;
                    conn.ConnectionTimeout = 2;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        vCurrODBC.ConnStatus = 1;
                        vCurrODBC.ConnErrMsg = "";
                        vCurrODBC.DBType = in2SqlLibrary.getDBType(conn.DataSource, conn.Driver);
                    }
                }
            }
            catch (Exception e)
            {
                vCurrODBC.ConnStatus = -3;
                vCurrODBC.ConnErrMsg = e.Message.ToString();
                vCurrODBC.ConnErrType = in2SqlLibrary.getErrConType(e.Message.ToString());
            }
            ChangeOdbcValue(vOdbcName, vCurrODBC);
        }

        public static IEnumerable<SqlObjects> getViewList(string vOdbcName)
        {
            var vViews = SqlReadDataValue(vOdbcName, in2SqlLibrary.getSqlViews(getODBCProperties(vOdbcName, "DBType")));
            foreach (var vCurrView in vViews)
            {
                SqlObjects vView = new SqlObjects();
                vView.Name = vCurrView.ToString();
                vView.idTbl = vIdtbl;
                vIdtbl = vIdtbl + 1;
                yield return vView;
            }
        }

        public static IEnumerable<SqlObjects> getTableList(string vOdbcName)
        {
            var vTables = SqlReadDataValue(vOdbcName, in2SqlLibrary.getSqlTables(getODBCProperties(vOdbcName, "DBType")));
            foreach (var vCurrTable in vTables)
            {
                SqlObjects vTable = new SqlObjects();
                vTable.Name = vCurrTable.ToString();
                vTable.idTbl = vIdtbl;
                vIdtbl = vIdtbl + 1;
                yield return vTable;
            }
        }


        public static IEnumerable<SqlObjects> getSQLProgrammsList(string vOdbcName)
        {
            var vTables = SqlReadDataValue(vOdbcName, in2SqlLibrary.getSQLProgramms(getODBCProperties(vOdbcName, "DBType")));
            foreach (var vCurrTable in vTables)
            {
                SqlObjects vTable = new SqlObjects();
                vTable.Name = vCurrTable.ToString();
                vTable.idTbl = vIdtbl;
                vIdtbl = vIdtbl + 1;
                yield return vTable;
            }
        }

        public static IEnumerable<SqlObjects> getSQLFunctionsList(string vOdbcName)
        {
            var vTables = SqlReadDataValue(vOdbcName, in2SqlLibrary.getSQLFunctions(getODBCProperties(vOdbcName, "DBType")));
            foreach (var vCurrTable in vTables)
            {
                SqlObjects vTable = new SqlObjects();
                vTable.Name = vCurrTable.ToString();
                vTable.idTbl = vIdtbl;
                vIdtbl = vIdtbl + 1;
                yield return vTable;
            }
        }

        public static IEnumerable<ObjectsAndProperties> getObjectProperties(string vOdbcName, string vObjName)
        {
            string vDBType = getODBCProperties(vOdbcName, "DBType");
            string vSql = in2SqlLibrary.getSQLTableColumn(vDBType);

            var vTb1 = vObjName.Split('.');

            if (vDBType.Contains("ORACLE"))
            {                
                vSql = vSql.Replace("%TNAME%", vTb1[1]);
            }
              else if (vDBType.Contains("CH"))
            {
                vSql = vSql.Replace("%TNAME%", vTb1[1]);
                vSql = vSql.Replace("%TOWNER%", vTb1[0]);
            }
              else
            {
                vSql = vSql.Replace("%TNAME%", vObjName);
            }

            var vObjects = SqlReadDataValue(vOdbcName, vSql);
            ObjectsAndProperties vObject = new ObjectsAndProperties();
            vObject.ObjName = vOdbcName + '.' + vObjName;
            vObject.objColumns = new List<string>();

            foreach (var vCurrObject in vObjects)
            {
                vObject.objColumns.Add(vCurrObject);
            }

            vSql = in2SqlLibrary.getSQLIndexes(vDBType);

            if (vDBType.Contains("ORACLE"))
            {
                vSql = vSql.Replace("%TNAME%", vTb1[1]);
            }
              else if (vDBType.Contains("CH"))
            {
                vSql = vSql.Replace("%TNAME%", vTb1[1]);
                vSql = vSql.Replace("%TOWNER%", vTb1[0]);
            }
            {
                vSql = vSql.Replace("%TNAME%", vObjName);
            }

            vObjects = SqlReadDataValue(vOdbcName, vSql);
            vObject.objIndexes = new List<string>();

            foreach (var vCurrObject in vObjects)
            {
                vObject.objIndexes.Add(vCurrObject);
            }


            vSql = in2SqlLibrary.getSQLDependencies(vDBType);
            if (vDBType.Contains("ORACLE"))
            {
                vSql = vSql.Replace("%TOWNER%", vTb1[1]);
                vSql = vSql.Replace("%TNAME%", vTb1[1]);

                vObjects = SqlReadDataValue(vOdbcName, vSql);
                vObject.objDependencies = new List<string>();

                foreach (var vCurrObject in vObjects)
                {
                    vObject.objDependencies.Add(vCurrObject);
                }

            }            
            

            yield return vObject;
        }





    }
}
