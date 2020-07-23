using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlEngine
{
    class in2sqlSvcCloud
    {

        public struct CloudObjects
        {
            public String Name;
            public int idTbl;
        }

        public static int vIdtbl = 0;

        public struct ObjectsAndProperties
        {
            public List<String> objColumns;
            public List<String> objIndexes;
            public List<String> objDependencies;
            public String ObjName;
        }

        public struct CloudProperties
        {
            public string CloudName, CloudType, Url, Login, Password;
            public int ConnStatus;
            public List<CloudObjects> Tables;
            public List<CloudObjects> Views;
            public List<CloudObjects> SQLProgramms;
            public List<CloudObjects> SQLFunctions;
        }

        public static List<CloudProperties> vCloudList = CloudList();

        public static List<ObjectsAndProperties> vCloudObjProp = new List<ObjectsAndProperties>();

        public static List<CloudProperties> CloudList()
        {
            try
            {                
                List<CloudProperties> listClooudProperties = new List<CloudProperties>();               
                listClooudProperties.AddRange(getCloudList());
                return listClooudProperties;
            }
            catch (Exception e)
            {
                In2SqlSvcTool.ExpHandler(e, "CloudList");
                return null;
            }
        }

        public static string prepareCloudQuery ( string vUrl, string vSql,string vLogin,string vPassword )
        {
            string vResult = vUrl;

            vResult = vResult.Replace("%SQL%", vSql);
            vResult = vResult.Replace("%LOGIN%", vLogin);
            vResult = vResult.Replace("%PASSWORD%", vPassword);

            return vResult;
        }

        public static int checkCloudState (string  vCurrCloudName)
        {

            CloudProperties vCurrCloud = vCloudList.Find(item => item.CloudName == vCurrCloudName);

            string vSqlURL ; 

            vSqlURL = prepareCloudQuery(vCurrCloud.Url, in2SqlLibrary.getCloudSqlCheck(vCurrCloud.CloudType), vCurrCloud.Login, vCurrCloud.Password);
            vSqlURL = In2SqlSvcTool.HttpGet(vSqlURL);

            if (vSqlURL.Length < 2)
            {                 
                return -1 ;
            }

            return 1;
        }


        /*    public static IEnumerable<SqlObjects> getViewList(string vOdbcName)
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
        */
            public static IEnumerable<CloudObjects> getCloudTableList(string vCurrCloudName)
            {
            CloudProperties vCurrCloud = vCloudList.Find(item => item.CloudName == vCurrCloudName);
            string vSqlURL;

            vSqlURL = prepareCloudQuery(vCurrCloud.Url, in2SqlLibrary.getCloudSqlTable(vCurrCloud.CloudType), vCurrCloud.Login, vCurrCloud.Password);

             return getCloudObjectList(vSqlURL);
               
            }

        public static IEnumerable<CloudObjects> getCloudViewList(string vCurrCloudName)
        {
            CloudProperties vCurrCloud = vCloudList.Find(item => item.CloudName == vCurrCloudName);
            string vSqlURL;

            vSqlURL = prepareCloudQuery(vCurrCloud.Url, in2SqlLibrary.getCloudSqlView(vCurrCloud.CloudType), vCurrCloud.Login, vCurrCloud.Password);

            return getCloudObjectList(vSqlURL);

        }

        private static IEnumerable<CloudObjects> getCloudObjectList(string vSqlURL)
        {
             
            List<String> vObjects = new List<String>();
            vObjects.AddRange(In2SqlSvcTool.HttpGetArray(vSqlURL));

            foreach (var vCurrObj in vObjects)
            {
                CloudObjects vObj = new CloudObjects();
                vObj.Name = vCurrObj.ToString();
                vObj.idTbl = vIdtbl;
                vIdtbl = vIdtbl + 1;
                yield return vObj;
            }
        }


        public static IEnumerable<ObjectsAndProperties> getObjectProperties(string vCurrCloudName, string vObjName)
        {
            CloudProperties vCurrCloud = vCloudList.Find(item => item.CloudName == vCurrCloudName);
            string vSqlURL;

            vSqlURL = prepareCloudQuery(vCurrCloud.Url, in2SqlLibrary.getCloudColumns(vCurrCloud.CloudType), vCurrCloud.Login, vCurrCloud.Password);
            var vTb1 = vObjName.Split('.');

            vSqlURL = vSqlURL.Replace("%TNAME%", vTb1[1]);
            vSqlURL = vSqlURL.Replace("%TOWNER%", vTb1[0]);           
             
            ObjectsAndProperties vObject = new ObjectsAndProperties();
            vObject.ObjName = vCurrCloudName + '.' + vObjName;
            vObject.objColumns = new List<string>(); 

            List<String> vObjects = new List<String>();
            vObjects.AddRange(In2SqlSvcTool.HttpGetArray(vSqlURL));

            foreach (var vCurrObj in vObjects)
            {
                vObject.objColumns.Add(vCurrObj);
            }

            yield return vObject;

        }
         

        public static IEnumerable<CloudProperties> getCloudList()
        {
            RegistryKey   vCurrRegKey = Registry.CurrentUser.OpenSubKey(@"Software\in2sql");
            string vCurrName = "";
            string vPrevName = "";
            if (vCurrRegKey != null)
            {
                CloudProperties vCloudProperties= new CloudProperties();  

                foreach (string name in vCurrRegKey.GetValueNames())
                {
                    if (name.Contains("Cloud"))
                    {
                        string[] vNameDetails = name.Split('.');

                        if (vNameDetails.Count() < 2)
                        {
                            MessageBox.Show("Error in reading registry getCloudList ");
                            yield return new CloudProperties();
                            break;
                        }
                        vCurrName = vNameDetails[1];

                        if (!vCurrName.Equals(vPrevName))
                        {
                            if (vPrevName.Length > 2)
                                yield return vCloudProperties;

                            vCloudProperties = new CloudProperties();
                        }

                        vPrevName = vCurrName;

                        vCloudProperties.CloudName = vCurrName;
                        vCloudProperties.CloudType = vNameDetails[0];

                        string vCurrRegValue = in2SqlRegistry.getLocalRegValue(vCurrRegKey, name);

                        if (name.Contains("Url"))
                            vCloudProperties.Url = vCurrRegValue;

                        if (name.Contains("Password"))
                            vCloudProperties.Password = vCurrRegValue;

                        if (name.Contains("Login"))
                            vCloudProperties.Login = vCurrRegValue;

                        if (vCloudProperties.Url != null )  
                            if (vCloudProperties.Password != null) 
                                if (vCloudProperties.Login.Length  > 0 )
                                {
                                  vPrevName = "";
                                    yield return vCloudProperties;
                                 }
                    }
                    else { 
                        if (vPrevName.Length > 2 )
                        {
                            vPrevName = "";                           
                        }
                    }
               
                }
            }
        }
    }
}
