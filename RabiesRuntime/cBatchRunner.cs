using System;
using System.Collections.Generic;
using System.Data.OleDb;//required to connect with Excel file
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using Random;
using System.Windows.Forms;
using System.Threading;
using Fox_Model_Library;

namespace RabiesRuntime
{
    /// <summary>
    /// Class that will run through a list of run definitions and run a specified number
    /// of trials for each run definition. 
    /// </summary>
    public class cBatchRunner
    {
        #region Events

        /// <summary>
        /// Delegate used by the various trial events raised by this class
        /// </summary>
        /// <param name="RunName">The name of the run that raised the event</param>
        /// <param name="TrialNumber">The trial number that raised the event</param>
        /// <param name="ThreadNumber">The number (1-NThreads) of the thread that raised the event</param>
        /// <param name="PercentComplete">The completion percentage of the trial</param>
        public delegate void TrialEvent(string RunName, int TrialNumber, int ThreadNumber, int PercentComplete);

        /// <summary>
        /// Event raised when a trial is started
        /// </summary>
        public event TrialEvent TrialStarted;

        /// <summary>
        /// Event raised when a trial is completed
        /// </summary>
        public event TrialEvent TrialCompleted;

        /// <summary>
        /// Event raised at the end of each year by currently running trials
        /// </summary>
        public event TrialEvent TrialRunning;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="RunDefinitions">The list of run definitions</param>
        /// <param name="NThreads">The number of run threads to be run at once</param>
        /// <param name="RunPerson">The name of the person doing the runs</param>
        /// <param name="AppReport">The application report object to send messages to</param>
        /// <param name="IsUnix">A boolean value indicating that Unix file paths are used</param>
        public cBatchRunner(cBatchRun RunDefinitions, int NThreads, cAppReport AppReport, bool IsUnix)
			: this(RunDefinitions, NThreads, AppReport, IsUnix, false)
		{
		}
		
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="RunDefinitions">The list of run definitions</param>
        /// <param name="NThreads">The number of run threads to be run at once</param>
        /// <param name="RunPerson">The name of the person doing the runs</param>
        /// <param name="AppReport">The application report object to send messages to</param>
        /// <param name="IsUnix">A boolean value indicating that Unix file paths are used</param>
        /// <param name="NoOutputExceptions">
        /// 		A boolean value indicating that output errors should be reported to the console rather than as exceptions
        /// </param>
        public cBatchRunner(cBatchRun RunDefinitions, int NThreads, cAppReport AppReport, bool IsUnix, bool NoOutputExceptions)
        {
            mvarRunDefinitions = RunDefinitions;
            mvarNThreads = NThreads;
            mvarIsUnix = IsUnix;
            mvarAppReport = AppReport;
			mvarNoOutputExceptions = NoOutputExceptions;
            this.NTrials = null;
            this.AbortOnRabiesDieOut = null;
            this.RunLength = null;
            this.UseRandomSeed = null;
            this.NThreads = 2;
            this.TabDelimitedOutput = true;
            this.FirstTrialNumber = 1;
            this.NoAnimalDatabases = false;
            this.ConfirmDatasources = true;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get or set the input folder for the animal and cell datasources.  If set, this value overrides
        /// the values in the definition files
        /// </summary>
        public string InputFolder { get; set; }

        /// <summary>
        /// Getor set the output folder for the results of the trials.  If set, this value overrides the values
        /// in the definition files
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// Get or set the name of the trial run.  This will be applied ONLY if there is a single set or trials to
        /// run.  If set, this value overrides the value in the definition file
        /// </summary>
        public string RunName { get; set; }

        /// <summary>
        /// Get or set the number of trials to run for each run definition.  If this value is set then
        /// it overrides the number of trials defined in the definition files
        /// </summary>
        public int? NTrials { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not the trials should abort if the disease
        /// dies out.  Setting this value will override settings in individual definition files
        /// </summary>
        public bool? AbortOnRabiesDieOut { get; set; }

        /// <summary>
        /// Get or set the length of each trial in years.  If this value is set it ovverides the run length
        /// values defined in the definition files
        /// </summary>
        public int? RunLength { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not to generate random number seeds from the
        /// clock.  If this value is set, it overrides the UseRandomSeed values in the definition files.  If
        /// the UseRandomSeed property of this class is set to true, that property will override this one.
        /// </summary>
        public bool? SeedFromClock { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not to use a specific random number seed. If
        /// this value is set it overrides the UseRandomSeed values in the definition files
        /// </summary>
        public bool? UseRandomSeed { get; set; }

        /// <summary>
        /// Get or set a random seed value.  If set it overrides the specific random seed values in the definition
        /// files.  If this value is set, tt is used if the UseRandomSeed property of the class is set to true -OR- 
        /// if the UseRandomSeed property of this class is not set and the a definition file has the UseRandomSeed
        /// value set to true
        /// </summary>
        public long? RandomSeed { get; set; }

        /// <summary>
        /// Get or set the number of simultaneous threads that can run
        /// </summary>
        public int NThreads { get; set; }

        /// <summary>
        /// Get a boolean value that indicates the trials are currently running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return mvarIsRunning;
            }
        }
        private bool mvarIsRunning = false;

        /// <summary>
        /// Get or set the name of the person performing the run
        /// </summary>
        public string RunPerson { get; set; }

        /// <summary>
        /// Get or set the affiliation of the person performing the run
        /// </summary>
        public string RunAffiliation { get; set; }

        /// <summary>
        /// Get or set contact information for the person performing the run
        /// </summary>
        public string RunContactInfo { get; set; }

        /// <summary>
        /// Get or set the name assigned to the rabies output file.  Overrides the setting
        /// in run settings if set
        /// </summary>
        public string RabiesFileName { get; set; }

        /// <summary>
        /// Get or set the name assigned to the summary output file.  Overrides the setting
        /// in run settings if set
        /// </summary>
        public string SummaryFileName { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates that the output files will be tab-delimited
        /// </summary>
        public bool TabDelimitedOutput { get; set; }

        /// <summary>
        /// Get or set the first trial number created by this batch runner.  This will be applied to
        /// each settings file run
        /// </summary>
        public int FirstTrialNumber { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates that no animal databases will be output by the trials
        /// </summary>
        public bool NoAnimalDatabases { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not the trials should display the current year and
        /// week to the console.  Ignored if NThreads > 1
        /// </summary>
        public bool PrintWeeks { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not the trials should display the current year to
        /// the console.
        /// </summary>
        public bool PrintYears { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether animal datasources should be "confirmed" (that is, that
        /// the list animals loaded from the datasources is identical to the list of animals that created the datasource)
        /// </summary>
        public bool ConfirmDatasources { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Abort a set of running trials
        /// </summary>
        public void Abort()
        {
            if (MainThread != null)
            {
                if (MainThread.ThreadState != ThreadState.Stopped && MainThread.ThreadState != ThreadState.Aborted) MainThread.Abort();
            }
        }

        /// <summary>
        /// Actual run all of the requested trials from all of the specified settings files
        /// </summary>
        public void RunTrials()
        {
            // do nothing if the main thread is currently running
            if (MainThread != null)
            {
                if (MainThread.ThreadState == ThreadState.Running) return;
            }
            // run the trials on their own thread so that we don't hand the UI thread
            MainThread = new Thread(DoRunTrials);
            MainThread.Start();
            mvarIsRunning = true;
        }

        
        /// <summary>
        /// EER: Actual run all of the requested trials from a settings file defined with the Excel template (i.e. ARM)
        /// </summary>
        public void RunTrialsExcelInput()
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cBatchRunner.cs: RunTrialsExcelInput()");
            
            // do nothing if the main thread is currently running
            if (MainThread != null)
            {
                if (MainThread.ThreadState == ThreadState.Running) return;
            }
            // run the trials on their own thread so that we don't hand the UI thread
            MainThread = new Thread(DoRunTrialsExcelInput);            
            MainThread.Start();
            mvarIsRunning = true;            
        }
        

        #endregion

        #region Protected Members

        /// <summary>
        /// The list of run definition files
        /// </summary>
        protected cBatchRun mvarRunDefinitions;
        /// <summary>
        /// The number of simultaneous threads
        /// </summary>
        protected int mvarNThreads;
        /// <summary>
        /// IsUnix flag
        /// </summary>
        protected bool mvarIsUnix;
        /// <summary>
        /// The name of the person doing the runs
        /// </summary>
        protected string mvarRunPerson;
		/// <summary>
		///Boolean value indicationg that output errors be reported to the console rather than as exceptions 
		/// </summary>
		protected bool mvarNoOutputExceptions;
        /// <summary>
        /// The application report object
        /// </summary>
        private cAppReport mvarAppReport;
        /// <summary>
        /// The list of run definition datasets
        /// </summary>
        private List<DataSet> RunDefinitions;
        /// <summary>
        /// The list of model trials
        /// </summary>
        private List<cModelTrial> ModelTrials;
        /// <summary>
        /// The list of trial threads to run
        /// </summary>
        private List<Thread> TrialThreads;
        /// <summary>
        /// The main thread running the trials
        /// </summary>
        private Thread MainThread;

        
        /// <summary>
        /// EER: Actual run all of the requested trials from a settings files defined by the Excel template (i.e. ARM)
        /// </summary>
        protected void DoRunTrialsExcelInput()
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cBatchRunner.cs: DoRunTrialsExcelInput()");
            
            // arrays to hold currently running threads and trials
            Thread[] RunningThreads = new Thread[NThreads];
            cModelTrial[] RunningTrials = new cModelTrial[NThreads];
            try
            {
                //EER
                int cnt = mvarRunDefinitions.Count;
                //System.Diagnostics.Debug.WriteLine("    mvarRunDefinitions.Count = " + cnt);
                
                // Define dataset of run definitions                
                DataSet runDS = cExcelSettingsTemplate.Parse(mvarRunDefinitions[0]);                

                //Build the threads for running trials
                ModelTrials = new List<cModelTrial>();
                TrialThreads = new List<Thread>();
                
                int ntrials = Convert.ToInt32(runDS.Tables[4].Rows[12][1]);                
                bool userandomseed = Convert.ToBoolean(runDS.Tables[4].Rows[9][1]);
                long randomseed = Convert.ToInt64(runDS.Tables[4].Rows[10][1]);
                string RunName = runDS.Tables[4].Rows[0][1].ToString();
                mvarRunPerson = runDS.Tables[4].Rows[16][1].ToString();
                //System.Diagnostics.Debug.WriteLine("    ntrials = " + ntrials);
                //System.Diagnostics.Debug.WriteLine("    userandomseed = " + userandomseed);
                //System.Diagnostics.Debug.WriteLine("    randomseed = " + randomseed);
                //System.Diagnostics.Debug.WriteLine("    RunName = " + RunName);
                //System.Diagnostics.Debug.WriteLine("    mvarRunPerson = " + mvarRunPerson);                              

                //EER Set the randomseed
                //If the user wants to use a random seed, than define the seed per input settings (i.e. randomseed)
                // otherwise, randomly set the seed
                if (userandomseed)
                {
                    cUniformRandom.SetSeed(randomseed);
                    long seed = cUniformRandom.Seed;
                    //System.Diagnostics.Debug.WriteLine("    user defined seed = " + seed);
                }
                else
                {
                    cUniformRandom.SetSeed();
                    long seed = cUniformRandom.Seed;
                    //System.Diagnostics.Debug.WriteLine("    random seed = " + seed);
                }                

                //EER The first argument in cModelTrial is for the running settings
                for (int j = 0; j < ntrials; j++)
                {                    
                    // create the actual trial
                    // EER: arguments for cModelTrial
                    //  runDS = excel template; will need to define how to use data in cModelTrial
                    //  mvarRunPerson, RunName are defined in DoRunTrialsInput()
                    //  this.FirstTrialNumber, mvarAppReport, mvarIsUnix, mvarNoOutputExceptions are defined from creating the cBatchRunner object in Form1
                    //  "" there is not template database defined
                    cModelTrial mt = new cModelTrial(runDS, mvarRunPerson, RunName, j + this.FirstTrialNumber, mvarAppReport, "",
                                                     new cUniformRandom(), mvarIsUnix, mvarNoOutputExceptions);
                                       
                    // set file names etc.
                    //if (!string.IsNullOrEmpty(this.SummaryFileName)) mt.SummaryFileName = this.SummaryFileName;
                    //if (!string.IsNullOrEmpty(this.RabiesFileName)) mt.RabiesFileName = this.RabiesFileName;
                    //mt.TabDelimitedOutput = this.TabDelimitedOutput;
                    //mt.NoAnimalDatabases = this.NoAnimalDatabases;
                    // set input and output paths if these are set for the batch runner
                    //if (!string.IsNullOrEmpty(this.InputFolder)) mt.InputFolder = this.InputFolder;
                    //if (!string.IsNullOrEmpty(this.OutputFolder)) mt.OutputFolder = this.OutputFolder;
                    //mt.PrintYears = this.PrintYears;
                    //mt.PrintWeeks = (this.NThreads == 1 ? this.PrintWeeks : false);
                    //mt.ConfirmDatasources = this.ConfirmDatasources;

                    // EER
                    //I probably don't need the above data
                    //I can create the mt
                    //Then a new RunModelTrialExcelTemplate to populate the model data with the settins and run

                    // create thread for trial
                    ModelTrials.Add(mt);
                    TrialThreads.Add(new Thread(mt.RunModelTrialExcelTemplate));
                }

                // now we have the threads defined, lets actually run individual threads
                // EER This is the section of code that actually uses RunModelTrialExcelTemplate from cModelTrial
                int ThreadCounter = 0;
                while (ThreadCounter < TrialThreads.Count)
                {
                    for (int i = 0; i < NThreads; i++)
                    {
                        // has this thread stopped
                        if (RunningThreads[i] != null)
                        {
                            ThreadState tState = RunningThreads[i].ThreadState;
                            if (tState == ThreadState.Stopped || tState == ThreadState.Aborted)
                            {
                                // the thread has stopped
                                // raise an event to indicate this
                                if (TrialCompleted != null) TrialCompleted(RunningTrials[i].RunName, RunningTrials[i].TrialNumber, i, 100);
                                // set the thread to null
                                RunningTrials[i].TrialProgress -= new cModelTrial.TrialProgessEvent(cBatchRunner_TrialProgress);
                                RunningThreads[i] = null;
                            }
                        }
                        if (RunningThreads[i] == null && ThreadCounter < TrialThreads.Count)
                        {
                            // start a thread in this position
                            RunningThreads[i] = TrialThreads[ThreadCounter];
                            RunningTrials[i] = ModelTrials[ThreadCounter];
                            // attach event handler to the running trial
                            RunningTrials[i].ThreadNumber = i;
                            RunningTrials[i].TrialProgress += new cModelTrial.TrialProgessEvent(cBatchRunner_TrialProgress);
                            // raise an event to indicate the start
                            if (TrialStarted != null) TrialStarted(RunningTrials[i].RunName, RunningTrials[i].TrialNumber, i, 0);
                            // start the thread
                            RunningThreads[i].Start();
                            // increment the ThreadCounter
                            ThreadCounter++;
                        }
                    }
                    // now stop for 2 seconds
                    Thread.Sleep(2000);
                }               

                // now just let the various still running threads finish up
                bool ThreadsRunning = true;
                while (ThreadsRunning)
                {
                    ThreadsRunning = false;
                    for (int i = 0; i < NThreads; i++)
                    {
                        if (RunningThreads[i] != null)
                        {
                            ThreadState tState = RunningThreads[i].ThreadState;
                            if (tState != ThreadState.Stopped && tState != ThreadState.Aborted)
                            {
                                ThreadsRunning = true;
                            }
                            else
                            {
                                // raise event for the finish of this thread
                                if (TrialCompleted != null) TrialCompleted(RunningTrials[i].RunName, RunningTrials[i].TrialNumber, i, 100);
                                RunningTrials[i].TrialProgress -= new cModelTrial.TrialProgessEvent(cBatchRunner_TrialProgress);
                            }
                        }
                    }
                    Thread.Sleep(2000);
                }
                
                // finally reset to non-running mode
                mvarIsRunning = false;
            }
            catch (ThreadAbortException)
            {
                // stop all running threads
                for (int i = 0; i < this.NThreads; i++)
                {
                    if (RunningThreads[i] != null)
                    {
                        if (RunningThreads[i].ThreadState == ThreadState.Running)
                        {
                            RunningThreads[i].Abort();
                        }
                    }
                }
                mvarIsRunning = false;
            }
            catch (Exception)
            {
                mvarIsRunning = false;
                throw;
            }
        }


        /// <summary>
        /// Actual run all of the requested trials from all of the specified settings files
        /// </summary>
        protected void DoRunTrials()
        {            
            // arrays to hold currently running threads and trials
            Thread[] RunningThreads = new Thread[NThreads];
            cModelTrial[] RunningTrials = new cModelTrial[NThreads];
            try
            {
                // build list of run definitions
                RunDefinitions = new List<DataSet>();
                for (int i = 0; i < mvarRunDefinitions.Count; i++)
                {
                    //EER Create list of settings data from the XML settings file
                    DataSet runDS = ReadSettingsFromXML(mvarRunDefinitions[i]);
                    
                    /*
                    //EER
                    //Use this to print out each row item in the datables
                    StringBuilder sb = new StringBuilder();
                    //int nTables = runDS.Tables.Count;
                    string tableName = "Combined_Strategies";
                    DataTable table = runDS.Tables[tableName];                    
                    {
                        
                        //Column Data
                        System.Diagnostics.Debug.WriteLine("");
                        int nCol = runDS.Tables[tableName].Columns.Count;
                        System.Diagnostics.Debug.WriteLine("Number of columns in " + tableName + " = " + nCol);
                        for (int m = 0; m < nCol; m++)
                        {
                            DataColumn dscol = runDS.Tables[tableName].Columns[m];
                            int maxL = dscol.MaxLength;
                            string name = dscol.ColumnName;
                            System.Diagnostics.Debug.WriteLine("name = " + name + "; maxLength = " + maxL);
                        }

                        //Row data
                        int nRows = runDS.Tables[tableName].Rows.Count;
                        System.Diagnostics.Debug.WriteLine("");
                        System.Diagnostics.Debug.WriteLine("Number of rows in " + tableName + " = " + nRows);                        
                        for (int m = 0; m < nRows; m++)
                        {                            
                            DataRow dsrow = runDS.Tables[tableName].Rows[m];
                            object[] arr = dsrow.ItemArray;
                            //int arrL = arr.Length;
                            //System.Diagnostics.Debug.WriteLine("arrL = " + arrL);
                            for (int j = 0; j < arr.Length; j++)
                            {                                
                                string rowitem = Convert.ToString(arr[j]);
                                System.Diagnostics.Debug.WriteLine("Row item " + j + " = " + rowitem);
                            }                            
                        }
                        System.Diagnostics.Debug.WriteLine("");
                    }
                    //EER
                    */                  

                    if (runDS != null)
                    {                        
                        DataRow drRunInfo = runDS.Tables["RunInfo"].Rows[0];

                        //EER
                        /*DataRow dsrow = runDS.Tables["RunInfo"].Rows[0];
                        object[] arr = dsrow.ItemArray;
                        for (int j = 0; j < arr.Length; j++)
                        {
                            string rowitem = Convert.ToString(arr[j]);
                            System.Diagnostics.Debug.WriteLine("    Row item " + j + " = " + rowitem);
                        }*/

                        DataRow drUserInfo = runDS.Tables["UserInfo"].Rows[0];
                        // adjust values in dataset if user wishes to override
                        drRunInfo.BeginEdit();
                        if (!string.IsNullOrEmpty(this.RunName))
                        {
                            if (mvarRunDefinitions.Count == 1) drRunInfo["RunName"] = this.RunName;
                        }
                        if (this.NTrials != null) drRunInfo["NTrials"] = this.NTrials;
                        if (this.AbortOnRabiesDieOut != null) drRunInfo["AbortOnRabiesDieOut"] = this.AbortOnRabiesDieOut;
                        if (this.RunLength != null) drRunInfo["RunLength"] = this.RunLength;
                        if (this.UseRandomSeed != null) drRunInfo["UseRandomSeed"] = this.UseRandomSeed;
                        if (this.RandomSeed != null) drRunInfo["RandomSeed"] = this.RandomSeed;
                        drRunInfo.AcceptChanges();
                        drUserInfo.BeginEdit();
                        if (!string.IsNullOrEmpty(this.RunPerson)) drUserInfo["UserName"] = this.RunPerson;
                        if (!string.IsNullOrEmpty(this.RunAffiliation)) drUserInfo["UserOrganization"] = this.RunAffiliation;
                        if (!string.IsNullOrEmpty(this.RunContactInfo)) drUserInfo["UserContactInfo"] = this.RunContactInfo;
                        drUserInfo.AcceptChanges();
                        // add dataset to list of RunDefinitions
                        RunDefinitions.Add(runDS);
                    }
                }
                // make sure output folder exists
                if (!string.IsNullOrEmpty(this.OutputFolder))
                {
                    if (this.OutputFolder != "!-current-!" && !Directory.Exists(this.OutputFolder)) Directory.CreateDirectory(this.OutputFolder);
                }
                // OK, we are now ready to begin building the threads for each trial we intend to run
                ModelTrials = new List<cModelTrial>();
                TrialThreads = new List<Thread>();
                for (int i = 0; i < RunDefinitions.Count; i++)
                {
                    DataRow drRunInfo = RunDefinitions[i].Tables["RunInfo"].Rows[0];
                    
                    //EER
                    //Check what infor is in RunDefinitions[i].Tables["RunInfo"].Rows[0]
                    /* 
                    StringBuilder sb = new StringBuilder();
                    object[] arr = drRunInfo.ItemArray;
                    for (int j = 0; j < arr.Length; j++)
                    {
                        sb.Append(Convert.ToString(arr[j]));
                        sb.Append("|");
                    }
                    System.Diagnostics.Debug.WriteLine("i = " + i);
                    System.Diagnostics.Debug.WriteLine("RunInfo = " + sb.ToString());
                    System.Diagnostics.Debug.WriteLine("");
                    */
                    //EER

                    //EER
                    /*
                    //How many tables in RunDefinitions[0]?
                    int numbTables = RunDefinitions[i].Tables.Count;
                    System.Diagnostics.Debug.WriteLine("Number of tables in RunDefinitions = " + numbTables);
                    //What are their names? and what is contained within?
                    // Print each table's name:
                    foreach (DataTable table in RunDefinitions[i].Tables)
                    {
                        System.Diagnostics.Debug.WriteLine(" Table name = " + table.TableName);
                    }
                    */

                    int ntrials = Convert.ToInt32(drRunInfo["NTrials"]);
                    bool userandomseed = Convert.ToBoolean(drRunInfo["UseRandomSeed"]);
                    long randomseed = Convert.ToInt64(drRunInfo["RandomSeed"]);
                    string RunName = drRunInfo["RunName"].ToString();
					if (this.SeedFromClock != null)
					{
						if ((bool)this.SeedFromClock) userandomseed = false; 
					}
                    if (userandomseed)
                    {
                        cUniformRandom.SetSeed(randomseed);
                    }
                    else
                    {
                        cUniformRandom.SetSeed();
                    }

                    //EER
                    /*
                    //Confirm values
                    System.Diagnostics.Debug.WriteLine("ntrials = " + ntrials);
                    System.Diagnostics.Debug.WriteLine("userandomseed = " + userandomseed);
                    System.Diagnostics.Debug.WriteLine("randomseed = " + randomseed);
                    System.Diagnostics.Debug.WriteLine("RunName = " + RunName);
                    //ntrials = 1
                    //userandomseed = False
                    //randomseed = 5344
                    //RunName = testrun_rabies_wCell80km2L50W20K50_weekly_mvt
                    */


                    // EER:cModelTrial takes in all 39 tables from the RunDefinitions[i] 
                    for (int j = 0; j < ntrials; j++)
                    {
                        // create the actual trial
                        cModelTrial mt = new cModelTrial(RunDefinitions[i], mvarRunPerson, RunName, j + this.FirstTrialNumber, mvarAppReport, "",
                                                         new cUniformRandom(), mvarIsUnix, mvarNoOutputExceptions);
                        // set file names etc.
                        if (!string.IsNullOrEmpty(this.SummaryFileName)) mt.SummaryFileName = this.SummaryFileName;
                        if (!string.IsNullOrEmpty(this.RabiesFileName)) mt.RabiesFileName = this.RabiesFileName;
                        mt.TabDelimitedOutput = this.TabDelimitedOutput;
                        mt.NoAnimalDatabases = this.NoAnimalDatabases;
                        // set input and output paths if these are set for the batch runner
                        if (!string.IsNullOrEmpty(this.InputFolder)) mt.InputFolder = this.InputFolder;
                        if (!string.IsNullOrEmpty(this.OutputFolder)) mt.OutputFolder = this.OutputFolder;
                        mt.PrintYears = this.PrintYears;
                        mt.PrintWeeks = (this.NThreads == 1 ? this.PrintWeeks : false);
                        mt.ConfirmDatasources = this.ConfirmDatasources;

                        // EER: Not all of the above updates from form data are active e.g. SummaryFileName
                        string log = OutputFolder;                       
                        
                        // create thread for trial
                        ModelTrials.Add(mt);
                        TrialThreads.Add(new Thread(mt.RunModelTrial));
                    }
                }
                // now we have the threads defined, lets actually run individual threads
                int ThreadCounter = 0;
                while (ThreadCounter < TrialThreads.Count)
                {
                    for (int i = 0; i < NThreads; i++)
                    {
                        // has this thread stopped
                        if (RunningThreads[i] != null)
                        {
                            ThreadState tState = RunningThreads[i].ThreadState;
                            if (tState == ThreadState.Stopped || tState == ThreadState.Aborted)
                            {
                                // the thread has stopped
                                // raise an event to indicate this
                                if (TrialCompleted != null) TrialCompleted(RunningTrials[i].RunName, RunningTrials[i].TrialNumber, i, 100);
                                // set the thread to null
                                RunningTrials[i].TrialProgress -= new cModelTrial.TrialProgessEvent(cBatchRunner_TrialProgress);
                                RunningThreads[i] = null;
                            }
                        }
                        if (RunningThreads[i] == null && ThreadCounter < TrialThreads.Count)
                        {
                            // start a thread in this position
                            RunningThreads[i] = TrialThreads[ThreadCounter];
                            RunningTrials[i] = ModelTrials[ThreadCounter];
                            // attach event handler to the running trial
                            RunningTrials[i].ThreadNumber = i;
                            RunningTrials[i].TrialProgress += new cModelTrial.TrialProgessEvent(cBatchRunner_TrialProgress);
                            // raise an event to indicate the start
                            if (TrialStarted != null) TrialStarted(RunningTrials[i].RunName, RunningTrials[i].TrialNumber, i, 0);
                            // start the thread
                            RunningThreads[i].Start();
                            // increment the ThreadCounter
                            ThreadCounter++;
                        }
                    }
                    // now stop for 2 seconds
                    Thread.Sleep(2000);
                }
                // now just let the various still running threads finish up
                bool ThreadsRunning = true;
                while (ThreadsRunning)
                {
                    ThreadsRunning = false;
                    for (int i = 0; i < NThreads; i++)
                    {
                        if (RunningThreads[i] != null)
                        {
                            ThreadState tState = RunningThreads[i].ThreadState;
                            if (tState != ThreadState.Stopped && tState != ThreadState.Aborted)
                            {
                                ThreadsRunning = true;
                            }
                            else
                            {
                                // raise event for the finish of this thread
                                if (TrialCompleted != null) TrialCompleted(RunningTrials[i].RunName, RunningTrials[i].TrialNumber, i, 100);
                                RunningTrials[i].TrialProgress -= new cModelTrial.TrialProgessEvent(cBatchRunner_TrialProgress);
                            }
                        }
                    }
                    Thread.Sleep(2000);
                }
                // finally reset to non-running mode
                mvarIsRunning = false;
            }
            catch (ThreadAbortException)
            {
                // stop all running threads
                for (int i = 0; i < this.NThreads; i++)
                {
                    if (RunningThreads[i] != null)
                    {
                        if (RunningThreads[i].ThreadState == ThreadState.Running)
                        {
                            RunningThreads[i].Abort();
                        }
                    }
                }
                mvarIsRunning = false;
            }
            catch (Exception)
            {
                mvarIsRunning = false;
                throw;
            }
        }

        /// <summary>
        /// Event handler for prgress event from trials
        /// </summary>
        /// <param name="RunName">The name of the run</param>
        /// <param name="TrialNumber">The trial number</param>
        /// <param name="ThreadNumber">The thread number</param>
        /// <param name="ProgressPercent">The current progress</param>
        void cBatchRunner_TrialProgress(string RunName, int TrialNumber, int ThreadNumber, int ProgressPercent)
        {
            if (this.TrialRunning != null) this.TrialRunning(RunName, TrialNumber, ThreadNumber, ProgressPercent);
        }
        /*
        /// <summary>
        /// Load run definition contents from Excel template and then recreate all underlying objects.
        /// Returns the dataset containing the run information or null if something went wrong
        /// Note: FName may be a local file reference or a URL
        /// </summary>
        /// <param name="FName">The file name and path to open</param>
        /// <returns>A dataset containing the run definition or null if the file could not be opened</returns>
        protected DataSet ReadSettingsFromExcelTemplate(string FName)
        {
            DataSet ds = null;
            try
            {
                // get run settings from an XML file into a dataset
                ds = new DataSet("RunSettings");
                
                // read in data
                
                                   
                // check version number
                string VersionCheck = CompareVersion(ds);
                if (!string.IsNullOrEmpty(VersionCheck))
                {
                    // a message was returned.  Time to abort!!
                    mvarAppReport.IndicateError(VersionCheck, null);
                    return null;
                }
                // all went well, return the loaded dataset
                return ds;
            }
            catch (Exception ex)
            {
                // something went wrong.  Throw error to higher level
                mvarAppReport.IndicateError(string.Format("Failed to load settings file. {0}", ex.Message), ex);
                return null;
            }
        }
        */
        /// <summary>
        /// Load run definition contents from XML and then recreate all underlying objects.
        /// Returns the dataset containing the run information or null if something went wrong
        /// Note: FName may be a local file reference or a URL
        /// </summary>
        /// <param name="FName">The file name and path to open</param>
        /// <returns>A dataset containing the run definition or null if the file could not be opened</returns>
        protected DataSet ReadSettingsFromXML(string FName)
        {
            DataSet ds = null;
            try
            {
                // get run settings from an XML file into a dataset
                ds = new DataSet("RunSettings");
                // read the data from the XML file into a dataset
                if (FName.StartsWith("http://")) // http:// web request
                {
                    // create the stream for reading data from this IP Address
                    WebRequest req = WebRequest.Create(FName);
                    WebResponse result = req.GetResponse();
                    Stream ReceiveStream = result.GetResponseStream();
                    // read data from that stream
                    ds.ReadXml(ReceiveStream, XmlReadMode.ReadSchema);
                    ReceiveStream.Close();
                }
                else
                {
                    // read data from a file stream
                    ds.ReadXml(FName, XmlReadMode.ReadSchema);                  

                }
                // check version number
                string VersionCheck = CompareVersion(ds);
                if (!string.IsNullOrEmpty(VersionCheck))
                {
                    // a message was returned.  Time to abort!!
                    mvarAppReport.IndicateError(VersionCheck, null);
                    return null;
                }
                // all went well, return the loaded dataset
                return ds;

                //EER
                //System.Diagnostics.Debug.WriteLine("We are here");
                int numbTables = ds.Tables.Count;
                //System.Diagnostics.Debug.WriteLine("Number of tables in RunDefinitions = " + numbTables);
                //What are their names? and what is contained within?
                // Print each table's name:
                foreach (DataTable table in ds.Tables)
                {
                    //System.Diagnostics.Debug.WriteLine(" Table name = " + table.TableName);
                }
                //Check what infor is in RunDefinitions[i].Tables["RunInfo"].Rows[0]
                /* 
                StringBuilder sb = new StringBuilder();
                object[] arr = drRunInfo.ItemArray;
                for (int j = 0; j < arr.Length; j++)
                {
                    sb.Append(Convert.ToString(arr[j]));
                    sb.Append("|");
                }
                System.Diagnostics.Debug.WriteLine("i = " + i);
                System.Diagnostics.Debug.WriteLine("RunInfo = " + sb.ToString());
                System.Diagnostics.Debug.WriteLine("");
                */
                //EER
            }
            catch (Exception ex)
            {
                // something went wrong.  Throw error to higher level
                mvarAppReport.IndicateError(string.Format("Failed to load settings file. {0}", ex.Message), ex);
                return null;
            }
        }

        /// <summary>
        /// Compare the version number in loaded Run Information (passed in ds) with this application's version number.
        /// Return an error message if they do not match.  Return empty string if they do
        /// </summary>
        /// <param name="ds">The data set to check</param>
        /// <returns>An error message if there is a mismatch or empty string if they do match</returns>
        protected string CompareVersion(DataSet ds)
        {
            try
            {
                // runinfo info
                DataTable tb = ds.Tables["RunInfo"];
                DataRow dr = tb.Rows[0];
                // begin by checking the version number
                if (dr["Version"].ToString() != cFoxModelVersion.SettingsFileVersion)
                {
                    return string.Format("Version number of the Settings File ({0}) does not match the expected version number of the settings file ({1})", dr["Version"], cFoxModelVersion.SettingsFileVersion);
                }
                return "";
            }
            catch
            {
                return "Unable to detirmine version number of the Settings File.  It will not be loaded.";
            }
        }

        #endregion
    }
}
