using System;
using System.Data;
using System.IO;

namespace RabiesRuntime
{
    // A class for loading in settings data from an CSV template

    public class cCSVSettingsTemplate
    {
        public cCSVSettingsTemplate(string PathFileName)
        {
        }

        //Function that loads Excel spreadsheet into a DataSet object
        public static DataSet Parse(string fileName)
        {
            DataSet data = new DataSet();

            AddCSVToDataSet(data, "AnimalBehaviour", "csv/AnimalBehaviour.csv");
            AddCSVToDataSet(data, "AnimalBiology", "csv/AnimalBiology.csv");
            AddCSVToDataSet(data, "DiseaseControl", "csv/DiseaseControl.csv");
            AddCSVToDataSet(data, "Epidemiology", "csv/Epidemiology.csv");
            AddCSVToDataSet(data, "RunSettings", "csv/RunSettings.csv");
            AddCSVToDataSet(data, "WinterSeverity", "csv/WinterSeverity.csv");
            AddCSVToDataSet(data, "ValidationRules", "csv/ValidationRules.csv");

            return data;
        }

        // Function that reads a CSV file and add it to the dataset
        private static void AddCSVToDataSet(DataSet dataSet, string name, string fileName)
        {
            DataTable dataTable;
            DataColumn column;
            DataRow row;

            try
            {
                dataTable = new DataTable(name);
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        throw new Exception(string.Format("Empty file: {0}", fileName));
                    string[] fields = SplitAndClean(line);
                    foreach (string field in fields)
                    {
                        column = new DataColumn(field);
                        dataTable.Columns.Add(column);
                    }

                    while ((line = sr.ReadLine()) != null)
                    {
                        fields = SplitAndClean(line);
                        row = dataTable.NewRow();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            int fieldAsInt;
                            double fieldAsDouble;
                            bool fieldAsBoolean;
                            if(int.TryParse(fields[i], out fieldAsInt))
                            {
                                row[i] = fieldAsInt;
                            }
                            else if (double.TryParse(fields[i], out fieldAsDouble))
                            {
                                row[i] = fieldAsDouble;
                            }
                            else if (bool.TryParse(fields[i], out fieldAsBoolean))
                            {
                                row[i] = fieldAsBoolean;
                            }
                            else
                            {
                                row[i] = fields[i];
                            }
                        }
                        dataTable.Rows.Add(row);
                    }
                }
                dataSet.Tables.Add(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw ex;
            }
        }

        //Function that converts a CSV line into an array of strings without quotation marks
        //This can be MUCH more robust. For example, having a comma inside a string will break things
        private static string[] SplitAndClean(string line)
        {
            string[] fields = line.Split(',');
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].StartsWith("\"") && fields[i].EndsWith("\""))
                {
                    fields[i] = fields[i].Substring(1, fields[i].Length - 2);
                }
            }

            return fields;
        }
    }
}
