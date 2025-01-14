﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fox_Model_Library;
using IndexedHashTable;
using Fox;
using RabiesRuntime;
using Rabies_Model_Core;
using Random;
using System.Threading;

namespace CommandLineARM
{
    class MainClass
    {
        // Object for sending output to log files and console
        static cBatchAppReport AppReport;

        // Location for AMR output: ARM.log
        static string outputFolder;

        // Pathway and file of settings file
        static string settingsFile;

        static Boolean IsUnix;
        string[] pathArg = new string[] { "one", "two", "three" };
        string[] testArg = new string[] { "C:\\coop\\project\\test_RITA220917_001.xlsx", "two1", "three1" };


        public object Background { get; private set; }
        public object mvarBackground { get; private set; }
        public object cAnimalList { get; private set; }

        public static void Main(string[] args)
        {
            MainClass main = new MainClass();
            Console.WriteLine("Starting");
            main.LoadSettingsFile(args);
            System.Threading.Thread.Sleep(4000);
            main.Run_Program();
        }

        //***************************************************************************************************************************************
        //mothode : Load Files ** for ORM console **
        public void LoadSettingsFile(string[] args)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("Form1.cs: btn_LoadSettingsFile_Click()");
            pathArg[0] = ".";
            Debug.WriteLine("settingsFile =++++++++++++++++++++++++++++++++++++++++++++++++++++++++ " + pathArg[0]);
            DataSet returnSettingsData = null;
            string filePath = string.Empty;
            string fileExt = string.Empty;
            settingsFile = null;
            //filePath = pathArg[0]; //get the path and name of the file 
            settingsFile = pathArg[0];
            /*Debug.WriteLine("settingsFile = " + settingsFile);
            Debug.WriteLine("filePath = " + filePath);*/
            //fileExt = pathArg[0]; //get the file extension  
            if (pathArg[0] != "")
            {
                try
                {
                    //Code to put Excel worksheets in datatables and then 1 dataset alphabetically                      

                    //Make excel settings file into dataset object                                            
                    returnSettingsData = cCSVSettingsTemplate.Parse(pathArg[0]);

                    //Retrieve setttings values in the dataset: Table(worksheet), row and column
                    String parameterSetName = returnSettingsData.Tables[4].Rows[0][1].ToString();
//                    textBoxParameterSet.Text = parameterSetName;

                    outputFolder = returnSettingsData.Tables[4].Rows[2][1].ToString();
//                    textBoxOutputFolder.Text = outputFolder;
                    Debug.WriteLine(" outputFolder =++++++++++++++++++++++++++++++++++++++++++++++++++++++++ " + outputFolder);


                    String landscapeFile = returnSettingsData.Tables[4].Rows[3][1].ToString();
                    Char[] delimiters = { '\\', '/' };
                    String[] landscapeFileItems = landscapeFile.Split(delimiters);
                    String landscapeFileItemsLast = landscapeFileItems.Last();
//                    textBoxLandscapeFile.Text = landscapeFileItemsLast;

                    String growPop = returnSettingsData.Tables[4].Rows[4][1].ToString();
                    //Debug.WriteLine("growPop = " + growPop);

                    if (growPop == "TRUE")
                    {
//                        textBoxPopulationFile.Text = "No file provided. A population is being grown.";
                    }
                    else
                    {

                        String populationFile = returnSettingsData.Tables[4].Rows[7][1].ToString();
                        //Debug.WriteLine("populationFile = " + populationFile);
                        //Char delimiter = '\\';
                        String[] populationFileItems = populationFile.Split(delimiters);
                        String populationFileItemsLast = populationFileItems.Last();
                        //Debug.WriteLine("populationFileItemsLast = " + populationFileItemsLast);
//                        textBoxPopulationFile.Text = populationFileItemsLast;
                    }
                    String numbIterations = returnSettingsData.Tables[4].Rows[12][1].ToString();
//                    textBoxNumbIterations.Text = numbIterations;

                    String numbYears = returnSettingsData.Tables[4].Rows[8][1].ToString();
//                    textBoxNumbYears.Text = numbYears;

                    //Connect to landscape XML file to get supercell and K information                       
                    cXMLCellDataSource landscapeXMLFile = new cXMLCellDataSource(true);
                    int totalPathLength = landscapeFile.Length;
                    int landNameLength = landscapeFileItemsLast.Length;
                    string landPath = landscapeFile.Substring(0, totalPathLength - landNameLength);
                    //Debug.WriteLine("Form1.cs: btn_LoadSettingsFile_Click(): landPath = " + landPath);
                    //Debug.WriteLine("Form1.cs: btn_LoadSettingsFile_Click(): landscapeFileItemsLast = " + landscapeFileItemsLast);                        
                    landscapeXMLFile.DataSourcePath = landPath;
                    landscapeXMLFile.DataSourceName = landscapeFileItemsLast;
                    landscapeXMLFile.Connect();

                    //Getting set up to get supercell values
                    cSuperCellList listSuperCells = new cSuperCellList();
                    landscapeXMLFile.GetSuperCellData(listSuperCells);
                    int numbSupCells = listSuperCells.Count;
                    //Debug.WriteLine("Form1.cs: btn_LoadSettingsFile_Click(): numbSupCells = " + numbSupCells);                      
                    string superCellFormEntry;
                    superCellFormEntry = null;

                    //Getting set up to get K values
                    cUniformRandom rand = new cUniformRandom();
                    cCellList listCells = new cCellList(rand);
                    landscapeXMLFile.GetCellData(listCells, listSuperCells);
                    cCellData CellData = new cCellData();
                    int numbCells = listCells.Count;
                    //Debug.WriteLine("Form1.cs: btn_LoadSettingsFile_Click(): numbCells = " + numbCells);
                    double mvarK;



                    // Get the supercell and K combinations
                    var superK = new ListWithDuplicates();
                    for (int i = 0; i < numbCells; i++)
                    {
                        //landscapeKValues[i] = listCells.ToString()
                        //values = listCells.ToString();
                        mvarK = listCells[i].K;
                        superCellFormEntry = listCells[i].SuperCell.ID;
                        //Debug.WriteLine("Form1.cs: btn_LoadSettingsFile_Click(): i = " + i + " mvarK = " + mvarK + "; supercell = " + superCellFormEntry);
                        superK.Add(superCellFormEntry, mvarK);
                    }



                    // Disconnect from the landscape XML file
                    landscapeXMLFile.Disconnect();

                    //With the data loaded in, activate the run button
//                    btnRun.Enabled = true;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message.ToString());
                }
            }
            else
            {
                Console.Error.WriteLine("Please choose .xls or .xlsx file only.");
            }
        }

        private void Run_Program()
        {

            //Debug.WriteLine("");
            //Debug.WriteLine("Form1.cs: btnRun_Click()");

            // Location of executable for running the program
            Assembly MyAssembly = Assembly.GetExecutingAssembly();
            string MyPath = MyAssembly.Location;
            //Debug.WriteLine(" MyPath = " + MyPath);

            // We might be dealing with unix or windows paths
            if (MyPath.Contains("/"))
            {
                int index = MyPath.LastIndexOf("/");
                if (index > 0) MyPath = MyPath.Substring(0, index);
                IsUnix = true;
                //Debug.WriteLine(" Unix");                
            }
            else
            {
                int index = MyPath.LastIndexOf(@"\");
                if (index > 0) MyPath = MyPath.Substring(0, index);
                IsUnix = false;
                //Debug.WriteLine(" Windows;" + " IsUnix = " + IsUnix);
            }

            // report object for application output to log file 
            // ARM.log file written to outputFolder as defined by settings file
            AppReport = new cBatchAppReport("ARM", outputFolder, true, IsUnix);

            AppReport.ShowDialogs = false;
            AppReport.WriteLog = true;

            // let log know that application was started
            AppReport.WriteToLogOnly("Application started...");

            // Print introductory screen
            //Debug.WriteLine(string.Format("   Rabies Model v.{0} - Start Time {1}", MyAssembly.GetName().Version.ToString(), DateTime.Now.ToString("MMM-dd-yyyy HH:mm:ss")));
            AppReport.WriteToLogOnly(string.Format("Rabies Model v.{0} - Start Time {1}", MyAssembly.GetName().Version.ToString(), DateTime.Now.ToString("MMM-dd-yyyy HH:mm:ss")));
            object[] attributes = MyAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
            {
                //Debug.WriteLine(" " + (attributes[0] as AssemblyCopyrightAttribute).Copyright);
                AppReport.WriteToLogOnly((attributes[0] as AssemblyCopyrightAttribute).Copyright);
            }

            // put settings files to run in a BatchRun list
            // For the form, we will only be running 1 settings file at-a-time and not batching them
            cBatchRun br = new cBatchRun();
            br.Add(settingsFile);

            // create the batch runner 
            // currently with 1 thread running
            cBatchRunner runner = new cBatchRunner(br, 1, AppReport, IsUnix, true);

            // run the model            
            runner.RunTrialsExcelInput();

            // wait for runner to complete task
            // this "for (;;)" syntax creates an infinite loop until the task is completed
            for (; ; )
            {
                if (!runner.IsRunning) break;
                System.Threading.Thread.Sleep(1000);
            }

            // Notify user of end of simulation(s)
            //Connect to landscape XML file to get supercell and K information                       
            //cXMLCellDataSource landscapeXMLFile = new cXMLCellDataSource(false);
            //cSuperCellList listSuperCells = new cSuperCellList();
            //landscapeXMLFile.GetSuperCellData(listSuperCells);
            //int numbSupCells = listSuperCells.Count;
            //Connect to ? file to get population size

            // set the background
            // mvarBackground = Background;
            //cAnimalList aList = new cAnimalList();   
            //cAnimalList aList = cAnimalList();
            //int popSize = aList.Count;
            //MessageBox.Show("   Finished running. Total population size = " + popSize);
            //MessageBox.Show("   Finished running");



            AppReport.WriteToLogOnly("Rabies Model Run Completed Normally");
            string conect_path = "";
            conect_path = @outputFolder + "\\connect.txt";
            //Debug.WriteLine(" conect_path =++++++++++++++++++++++++++++++++++++++++++++++++++++++++ " + conect_path);

            if (!File.Exists(conect_path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(conect_path))
                {
                    sw.WriteLine("Completed");

                }
            }


            Console.WriteLine("       Simulation was completed successfully ... ");


        }

    }
}
