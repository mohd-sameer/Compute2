using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;//required to connect with Excel file
using System.Linq;
using System.Text;

namespace RabiesRuntime
{
    // A class for loading in settings data from an Excel template

    public class cExcelSettingsTemplate
    {
        /// <summary>
        /// Constructor.  Must pass the name of the application
        /// </summary>
        /// <param name="PathFileName">Pathway and name of settings file</param>    
        public cExcelSettingsTemplate(string PathFileName)
        {
        }

        //Function that loads Excel spreadsheet into a DataSet object
        public static DataSet Parse(string fileName)
        {            
            //System.Diagnostics.Debug.WriteLine("cExcelSettingsTemplate.cs: Parse()");
            //string connectionString = string.Format("provider=Microsoft.Jet.OLEDB.4.0; data source={0};Extended Properties=Excel 8.0;", fileName); //Excel files below 2007
            //string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 12.0;", fileName); //Excel files above 2007

            // EER: To load in mixed data types, that are read in the code as mixed data types
            //  I found this fix: 1) add in the single quotes containing the extended properties,
            //  and 2) include IMEX=1
            string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties='Excel 12.0;IMEX=1'", fileName); //Excel files above 2007
            
            DataSet data = new DataSet();

            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                //System.Diagnostics.Debug.WriteLine("connectionString = " + connectionString);
                using (OleDbConnection con = new OleDbConnection(connectionString))                
                {
                    var dataTable = new DataTable();
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    //System.Diagnostics.Debug.WriteLine("    template worksheet = " + sheetName);
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    data.Tables.Add(dataTable);
                    con.Close();//EER: Close connection to run more efficently
                }
            }

            return data;
        }

        //Function that returns the names of worksheets in an Excel spreadsheet in alphabetical order       
        static string[] GetExcelSheetNames(string connectionString)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cExcelSettingsTemplate.cs: GetExcelSheetNames()");

            OleDbConnection con = null;
            DataTable dt = null;
            con = new OleDbConnection(connectionString);
            con.Open();
            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            String[] excelSheetNames = new String[dt.Rows.Count];
            
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                excelSheetNames[i] = row["TABLE_NAME"].ToString();
                //System.Diagnostics.Debug.WriteLine("worksheet " + i + " is " + excelSheetNames[i]);
                i++;
            }
            con.Close();
            return excelSheetNames;
        }
    }
}
