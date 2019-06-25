using System;
using System.Collections.Generic;
using Random;
using Rabies_Model_Core;
using System.IO;
using System.Text;
using Fox;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace RabiesRuntime
{

    /// <summary>
    /// Class that represents a single model trial
    /// </summary>
    public class cModelTrial
    {
        #region Events

        /// <summary>
        /// Delegate for the Trial Progress event
        /// </summary>
        /// <param name="RunName">The name of the run</param>
        /// <param name="TrialNumber">The trial number of the run</param>
        /// <param name="ThreadNumber">The thread number attached to the trial</param>
        /// <param name="ProgressPercent">The percent progress for the run</param>
        public delegate void  TrialProgessEvent(string RunName, int TrialNumber, int ThreadNumber, int ProgressPercent);

        /// <summary>
        /// Event raised by a trial at the end of each year to indicate its progress
        /// </summary>
        public event TrialProgessEvent TrialProgress;

        /// <summary>
        /// Delegate for the Trial Aborted event
        /// </summary>
        public delegate void TrialCompletedAbortedEvent();

        /// <summary>
        /// Event raised when a trial is completed
        /// </summary>
        public event TrialCompletedAbortedEvent TrialCompleted;

        /// <summary>
        /// Event raised when a trial is aborted
        /// </summary>
        public event TrialCompletedAbortedEvent TrialAborted;

        /// <summary>
        /// Delegate for the trial stopped with error event
        /// </summary>
        /// <param name="ex">The exception that caused the trial to stop</param>
        public delegate void TrialStoppedWithErrorDelegate(Exception ex);

        /// <summary>
        /// Event raised if the trial stops because of an error
        /// </summary>
        public event TrialStoppedWithErrorDelegate TrialStoppedWithError;

        #endregion

        #region Protected Members

        /// <summary>
        /// Background object for this trial
        /// </summary>
        protected cBackground ModelBackground;
        /// <summary>
        /// Trial Settings
        /// </summary>
        protected DataSet mvarTrialSettings;
		/// <summary>
		/// Boolean value for turning off output exceptions 
		/// </summary>
		protected bool mvarNoOutputExceptions;
        /// <summary>
        /// Trial Number
        /// </summary>
        private int mvarTrialNumber;
        /// <summary>
        /// Random number generator for trial
        /// </summary>
        private cUniformRandom mvarRnd;
        /// <summary>
        /// The application reporting and logging class
        /// </summary>
        private cAppReport mvarAppReport;
        /// <summary>
        /// Boolean value indicating that we should use unix paths
        /// </summary>
        private bool mvarIsUnix;
        /// <summary>
        /// A predefined output folder
        /// </summary>
        private string mvarOutputFolder = null;
        /// <summary>
        /// A predefined output folder
        /// </summary>
        private string mvarInputFolder = null;
        /// <summary>
        /// The length of the trial in years
        /// </summary>
        private int mvarRunLength = 0;
        /// <summary>
        /// The current year (out of the number years to run that was just run)
        /// </summary>
        protected int mvarCurrentRunYear = 0;
        /// <summary>
        /// The time the trial started
        /// </summary>
        protected DateTime startTime;

        // output to files
        protected cBasicInfoOutput BasicInfoOutput;
        protected cRabiesReportOutput RabiesReportOutput;
        protected cCellPopulationOutput CellPopulationOutput;
        protected cBasicInfoData BasicInfoData;
        protected cRabiesReportData RabiesReportData;
        protected bool PeriodicDatabaseOutput = false;
        protected int PeriodicDatabaseOutputInterval = 0;
        protected bool PeriodicDatabaseOutputMDB = false;
        protected int PeriodicDatabaseOutputCount;
        protected string PeriodicDatabaseOutputFolder;

        // the initial infections
        private cInitialInfectionList InitialInfections;

        // array for writing cell populations
        private int[] WriteCellPop = new int[53];
        // EER: for the ARM excel template this variable is boolean
        private bool[] WriteCellPopExcelTemplate = new bool[53];

        // run information
        private string mvarRunPerson;
        private string mvarRunName;
        private bool ScrambleAnimals;

        // file name of template database
        private string mvarTemplateDatabase;

        // winter cull parameters
        private int WinterCullWeek;
        private double[] WinterCullByAgeMale = new double[9];
        private double[] WinterCullByAgeFemale = new double[9];
        private int[] CullWinterEffect = new int[6];

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TrialSettings">The data set containing the settings for the trial to be run</param>
        /// <param name="TrialNumber">The number of the trial to be run</param>
        /// <param name="RunPerson">The name of the person performing the current run of the rabies model</param>
        /// <param name="RunName">The name assigned to the current run of the rabies model.  This will be used to label output</param>
        /// <param name="AppReport">The application and reporting class for the calling application</param>
        /// <param name="TemplateDatabase">The name of the template database used to create new datasources</param>
        /// <param name="Rnd">The random number generator to be used for this trial</param>
        /// <param name="IsUnix">A boolean value indicating that Unix file paths should be used</param>
        public cModelTrial(DataSet TrialSettings, string RunPerson, string RunName, int TrialNumber, cAppReport AppReport, string TemplateDatabase, cUniformRandom Rnd, bool IsUnix)
			: this(TrialSettings, RunPerson, RunName, TrialNumber, AppReport, TemplateDatabase, Rnd, IsUnix, false)
        {
		}		
		
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TrialSettings">The data set containing the settings for the trial to be run</param>
        /// <param name="TrialNumber">The number of the trial to be run</param>
        /// <param name="RunPerson">The name of the person performing the current run of the rabies model</param>
        /// <param name="RunName">The name assigned to the current run of the rabies model.  This will be used to label output</param>
        /// <param name="AppReport">The application and reporting class for the calling application</param>
        /// <param name="TemplateDatabase">The name of the template database used to create new datasources</param>
        /// <param name="Rnd">The random number generator to be used for this trial</param>
        /// <param name="IsUnix">A boolean value indicating that Unix file paths should be used</param>
        /// <param name="NoOutputExceptions">
        /// 		A boolean value indicating that output errors should be reported to the console rather than as exceptions
        /// </param>
        public cModelTrial(DataSet TrialSettings, string RunPerson, string RunName, int TrialNumber, cAppReport AppReport, string TemplateDatabase, cUniformRandom Rnd, bool IsUnix, bool NoOutputExceptions)
        {
            if (TrialSettings == null) throw new ArgumentNullException("TrialSettings");
            if (Rnd == null) throw new ArgumentNullException("Rnd");
            if (AppReport == null) throw new ArgumentNullException("AppReport");
            if (string.IsNullOrEmpty(RunName)) throw new ArgumentNullException("RunName");
            mvarTrialSettings = TrialSettings;
            mvarTrialNumber = TrialNumber;
            mvarAppReport = AppReport;
			mvarRunPerson = RunPerson;
            mvarRunName = RunName;
            mvarIsUnix = IsUnix;
			mvarNoOutputExceptions = NoOutputExceptions;
            // classes to hold report data
            BasicInfoData = new cBasicInfoData();
            BasicInfoData.Trial = mvarTrialNumber;
            RabiesReportData = new cRabiesReportData();
            RabiesReportData.Trial = mvarTrialNumber;

            // path to executing assembly
            string MyPath = Assembly.GetExecutingAssembly().Location;

            // get the location of the template database
            if (!string.IsNullOrEmpty(TemplateDatabase))
            {
                int i = 0;
                if (mvarIsUnix)
                {
                    i = MyPath.LastIndexOf("/");
                    mvarTemplateDatabase = string.Format("{0}/{1}", MyPath.Substring(0, i), TemplateDatabase);
                }
                else
                {
                    i = MyPath.LastIndexOf(@"\");
                    mvarTemplateDatabase = string.Format(@"{0}\{1}", MyPath.Substring(0, i), TemplateDatabase);
                }
            }
            // default settings
            this.TabDelimitedOutput = false;            
            this.NoAnimalDatabases = false;
            this.PrintWeeks = false;
            this.PrintYears = false;
            this.ConfirmDatasources = true;
            mvarRnd = Rnd;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get or set the current output folder.  If set, this value will override the output folder in the 
        /// settings file
        /// </summary>
        public string OutputFolder
        {
            get { return (mvarOutputFolder == null ? "" : mvarOutputFolder); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    mvarOutputFolder = null;
                }
                else if (value.Trim().Length == 0)
                {
                    mvarOutputFolder = null;
                }
                else
                {
                    mvarOutputFolder = value;
                }
            }
        }

        /// <summary>
        /// Get or set the current input folder.  If set, this value will override the input folders for the various datasources in the 
        /// settings file
        /// </summary>
        public string InputFolder
        {
            get { return (mvarInputFolder == null ? "" : mvarInputFolder); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    mvarInputFolder = null;
                }
                else if (value.Trim().Length == 0)
                {
                    mvarInputFolder = null;
                }
                else
                {
                    mvarInputFolder = value;
                }
            }
        }

        /// <summary>
        /// Get the random number generator used by the model trial
        /// </summary>
        public cUniformRandom RandomGenerator
        {
            get
            {
                return mvarRnd;
            }
        }

        /// <summary>
        /// Get the name of the run for this trial
        /// </summary>
        public string RunName
        {
            get
            {
                return mvarRunName;
            }
        }

        /// <summary>
        /// Get the trial number for this trial
        /// </summary>
        public int TrialNumber
        {
            get
            {
                return mvarTrialNumber;
            }
        }

        /// <summary>
        /// Get or set the thread number attached to this trial
        /// </summary>
        public int ThreadNumber { get; set; }

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
        /// Get or set a boolean value that indicates that no animal databases will be output by the trials
        /// </summary>
        public bool NoAnimalDatabases { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not a trial should display the current year and
        /// week to the console.
        /// </summary>
        public bool PrintWeeks { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not a trial should display the current year to
        /// the console
        /// </summary>
        public bool PrintYears { get; set; }

        /// <summary>
        /// Get or set a boolean value that indicates whether animal datasources should be "confirmed" (that is, that
        /// the list animals loaded from the datasources is identical to the list of animals that created the datasource)
        /// </summary>
        public bool ConfirmDatasources { get; set; }

        #endregion

        #region Public Methods

        // EER: run a single trial of a run using settings from the Excel template
        public virtual void RunModelTrialExcelTemplate()
        {         
                      
            string ActualOutputFolder = "";

            try
            {
                startTime = DateTime.Now;
                int i = 0;

                // Define different data tables containing the run definitions from the Excel template
                DataTable dtAnimalBehav = mvarTrialSettings.Tables[0];
                DataTable dtAnimalBiol = mvarTrialSettings.Tables[1];
                DataTable dtDiseaseCtrl = mvarTrialSettings.Tables[2];
                DataTable dtEpi = mvarTrialSettings.Tables[3];
                DataTable dtRunInfo = mvarTrialSettings.Tables[4];
                DataTable dtWinter = mvarTrialSettings.Tables[5];
                                
                // indicate start of run in the Log file
                mvarAppReport.IndicateRunStart(string.Format("{0} - Trial {1}", mvarRunName, mvarTrialNumber.ToString("0000")));

                // get the actual output folder
                ActualOutputFolder = dtRunInfo.Rows[2][1].ToString();
                //System.Diagnostics.Debug.WriteLine("    ActualOutputFolder = " + ActualOutputFolder);
                if (mvarIsUnix)
                {
                    if (!ActualOutputFolder.EndsWith("/")) ActualOutputFolder = string.Format("{0}/", ActualOutputFolder);
                    //System.Diagnostics.Debug.WriteLine("    Unix");
                }
                else
                {
                    if (!ActualOutputFolder.EndsWith(@"\")) ActualOutputFolder = string.Format(@"{0}\", ActualOutputFolder);
                    //System.Diagnostics.Debug.WriteLine("    Windows");
                }

                // create folder for database output if user is doing periodic database output
                PeriodicDatabaseOutput = Convert.ToBoolean(dtRunInfo.Rows[25][1]);                
                PeriodicDatabaseOutputInterval = Convert.ToInt32(dtRunInfo.Rows[26][1]);               
                PeriodicDatabaseOutputMDB = Convert.ToBoolean(dtRunInfo.Rows[27][1]);
                PeriodicDatabaseOutputFolder = string.Format(@"{0}{1}\AnimalDatasources\Trial{2}", ActualOutputFolder, mvarRunName, mvarTrialNumber.ToString("0000"));
                if (mvarIsUnix) PeriodicDatabaseOutputFolder = PeriodicDatabaseOutputFolder.Replace(@"\", "/");                
                ScrambleAnimals = Convert.ToBoolean(dtRunInfo.Rows[13][1]);
                /*
                System.Diagnostics.Debug.WriteLine("    PeriodicDatabaseOutput = " + PeriodicDatabaseOutput);
                System.Diagnostics.Debug.WriteLine("    PeriodicDatabaseOutputInterval = " + PeriodicDatabaseOutputInterval);
                System.Diagnostics.Debug.WriteLine("    PeriodicDatabaseOutputMDB = " + PeriodicDatabaseOutputMDB);
                System.Diagnostics.Debug.WriteLine("    PeriodicDatabaseOutputFolder = " + PeriodicDatabaseOutputFolder);
                System.Diagnostics.Debug.WriteLine("    ScrambleAnimals = " + ScrambleAnimals);*/

                // create the model datasource                
                cCellsDataSource CellDatasource = OpenCellDatasource(dtRunInfo.Rows[3][1].ToString());
                mvarRunLength = Convert.ToInt32(dtRunInfo.Rows[8][1]);
                mvarCurrentRunYear = 1;
                cModelDataSource ModelDatasource = null;
                /*
                string landscape = dtRunInfo.Rows[3][1].ToString();
                System.Diagnostics.Debug.WriteLine("    landscape = " + landscape);
                System.Diagnostics.Debug.WriteLine("    mvarRunLength = " + mvarRunLength);*/

                /*bool startsingle = Convert.ToBoolean(dtRunInfo.Rows[4][1]);
                System.Diagnostics.Debug.WriteLine("    start with single animal = " + startsingle);
                string pop = dtRunInfo.Rows[7][1].ToString();
                System.Diagnostics.Debug.WriteLine("    existing pop = " + pop);
                //Find value UseSpecifiedYearTypes
                bool valueFind = Convert.ToBoolean(dtWinter.Rows[1][1]);
                System.Diagnostics.Debug.WriteLine("    valueFind = " + valueFind);*/
                
                // does the user wish to seed the cells with a single starting animal
                if (Convert.ToBoolean(dtRunInfo.Rows[4][1]))
                {                   
                    //Get input data for Adam and Eve genetic markers, if there are any
                    string mMarkers = "M";
                    string fMarkers = "M";
                    if (!DBNull.Value.Equals(dtRunInfo.Rows[5][1]))
                    {
                        //not null
                        mMarkers = dtRunInfo.Rows[5][1].ToString();
                        fMarkers = dtRunInfo.Rows[6][1].ToString();                        
                    }
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): mMarkers = " + mMarkers + "; fMarkers = " + fMarkers);
                    
                    // has user supplied a list of winter types?
                    // EER User defined winter types = TRUE
                    //bool userWinterTypes = Convert.ToBoolean(dtWinter.Rows[1][1]);
                    if (Convert.ToBoolean(dtWinter.Rows[1][1]))
                    {
                        //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): User defined winter types = TRUE ");
                        // create winter list based on values input into spreadsheet
                        // EER: NEED TO UPDATE CreateWinterList from cModelTrial to get data from Excel template
                        cWinterTypeList wtList = CreateWinterListExcelTemplate(mvarRunLength, mvarTrialSettings);
                        /*int yr = 0;
                        foreach (int winter in wtList) // Loop through List with foreach.
                        {                            
                            System.Diagnostics.Debug.WriteLine("    winter year " + yr + " = " + winter);
                            yr = yr + 1;
                        }*/
                                                
                        // now create the model datasource
                        ModelDatasource = new cFoxModelDataSource(mvarRnd, CellDatasource, wtList, mMarkers, fMarkers);                        
                    }
                    else
                    {
                        
                        // create a random list of winters with a specified bias
                        string winterBias = dtWinter.Rows[2][1].ToString();
                        int winterType = 0;
                        switch (winterBias)
                        {
                            case "VerySevere":
                                winterType = 1;
                                break;
                            case "Severe":
                                winterType = 2;
                                break;
                            case "Normal":
                                winterType = 3;
                                break;
                            case "Mild":
                                winterType = 4;
                                break;
                            case "VeryMild":
                                winterType = 5;
                                break;
                        }
                        //System.Diagnostics.Debug.WriteLine("    winter bias is " + winterBias);
                        //System.Diagnostics.Debug.WriteLine("    winterType = " + winterType);                        
                        ModelDatasource = new cFoxModelDataSource(mvarRnd, CellDatasource, mvarRunLength, 
                                                                    (enumWinterType)winterType, mMarkers, fMarkers);                        
                    }
                }
                else
                {
                    // EER: use an existing animal population datasource
                    
                    // connect to an animals datasource. Additional years will be added later if required
                    ModelDatasource = new cFoxModelDataSource(mvarRnd, CellDatasource, OpenAnimalDatasource(dtRunInfo.Rows[7][1].ToString()));
                }

                // create the rabies disease object                
                int ChanceOfDeath = Convert.ToInt32(dtEpi.Rows[1][1]);
                //System.Diagnostics.Debug.WriteLine("    ChanceOfDeath = " + ChanceOfDeath);                                             
                bool RecoveredBecomeImmune = Convert.ToBoolean(dtEpi.Rows[3][1]);
                //System.Diagnostics.Debug.WriteLine("    RecoveredBecomeImmune = " + RecoveredBecomeImmune);                
                bool RecoveredNotInfectious = Convert.ToBoolean(dtEpi.Rows[4][1]);
                //System.Diagnostics.Debug.WriteLine("    RecoveredNotInfectious = " + RecoveredNotInfectious);
                Double SpreadProbability = Convert.ToDouble(dtEpi.Rows[0][1]);
                //System.Diagnostics.Debug.WriteLine("    SpreadProbability = " + SpreadProbability);
                                
                //EER: the last arguement is a flag to indicate that ARM is running (thus uses input from the Excel template)
                cBatchRabies Rabies = new cBatchRabies(SpreadProbability, mvarRnd, ChanceOfDeath, RecoveredBecomeImmune, RecoveredNotInfectious, mvarTrialSettings, true);                

                // create a disease list containing this disease
                cDiseaseList DiseaseList = new cDiseaseList(mvarRnd);                
                DiseaseList.Add(Rabies);
                                
                // now use the data source to get a background and load the data into it
                ModelBackground = ModelDatasource.ReadDatasource(string.Format("{0}-{1}", mvarRunName, mvarTrialNumber), false, DiseaseList);
                
                // set the abort on rabies die out flag
                bool abortFlag = Convert.ToBoolean(dtRunInfo.Rows[14][1]);                
                ModelBackground.AbortOnDiseaseDisappearance = abortFlag;                

                // set reflective spread flag
                bool reflectSpreadFlag = Convert.ToBoolean(dtEpi.Rows[2][1]);                
                ModelBackground.UseReflectiveDiseaseSpread = reflectSpreadFlag;                

                // set prevent incest flag
                bool preventIncestFlag = Convert.ToBoolean(dtAnimalBiol.Rows[5][1]);                
                ModelBackground.PreventIncest = preventIncestFlag;                

                // if the animals were loaded from a datasource, add on the required number of years
                bool growPopFlag = Convert.ToBoolean(dtRunInfo.Rows[4][1]);
                bool userWinterTypes = Convert.ToBoolean(dtWinter.Rows[1][1]);                
                if (!growPopFlag)
                {                    
                    int YearsToAdd = mvarRunLength - ModelBackground.Years.Count + ModelBackground.Years.CurrentYearNum;                    
                    if (ModelBackground.Years.IsLastYear & ModelBackground.Years.CurrentYear.CurrentWeek == 52)
                    {
                        YearsToAdd += 1;
                    }
                    if (YearsToAdd > 0)
                    {
                        // has the user asked for random winters or created a list of winters
                        // EER: If the user provided a list of winters (i.e. user defined winter types = TRUE)                        
                        if (userWinterTypes == true)
                        {
                            cWinterTypeList wtList = default(cWinterTypeList);                            
                            // create winter list based on values input into spreadsheet
                            wtList = CreateWinterListExcelTemplate(YearsToAdd, mvarTrialSettings);                            
                            // add these winter types
                            ModelBackground.Years.AddYears(wtList);                            
                        }
                        else
                        {
                            // add years with the desired winter bias
                            string winterBias = dtWinter.Rows[2][1].ToString();                            
                            int winterType = 0;
                            switch (winterBias)
                            {
                                case "VerySevere":
                                    winterType = 1;
                                    break;
                                case "Severe":
                                    winterType = 2;
                                    break;
                                case "Normal":
                                    winterType = 3;
                                    break;
                                case "Mild":
                                    winterType = 4;
                                    break;
                                case "VeryMild":
                                    winterType = 5;
                                    break;
                            }
                            ModelBackground.Years.AddYears(YearsToAdd, (enumWinterType)winterType);                                                    
                        }
                        
                    }
                    // increment the week by one because value store in the database was for week
                    // last run
                    ModelBackground.Years.IncrementTime();
                }
                                
                // EER: Define the output folder location
                StringBuilder OutFolder = new StringBuilder(ActualOutputFolder);
                
                // EER: Define the run name, for tagging output files placed inthe output folder location
                OutFolder.Append(mvarRunName);

                // create regular and rabies ouput files if it is requested
                // EER: define the format of the text files (space or tab-delimited)
                this.TabDelimitedOutput = Convert.ToBoolean(dtRunInfo.Rows[30][1]);                

                // EER: this is the name for the text file output of population data for the total simulation time
                // EER: the Trim function is used to remove any white spaces before or after the name, entered by the user                
                string RegularOutput = dtRunInfo.Rows[28][1].ToString().Trim();
                // EER: this is the name for the text file output of rabies data for the total simulation time
                string RabiesOutput = dtRunInfo.Rows[29][1].ToString().Trim();                
                // EER: Create the output folder and regular and rabies files within to store the upcoming data
                if (RegularOutput.Length > 0)
                {
                    CreateBasicInfoOutput(OutFolder.ToString(), string.Format("{0}{1}", RegularOutput, mvarTrialNumber.ToString("0000")));                    
                }
                if (RabiesOutput.Length > 0)
                {
                    CreateRabiesReportOutput(OutFolder.ToString(), string.Format("{0}{1}", RabiesOutput, mvarTrialNumber.ToString("0000")));                    
                }
                
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): start cell by cell report");
                // create a cell by cell report if it is requested                
                bool CellPopCreated = false;
                // loop through each week, looking for at least one where output is requested
                for (i = 0; i < 52; i++) {
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): i = " + i);
                    // write value into array
                    WriteCellPopExcelTemplate[i] = Convert.ToBoolean(dtRunInfo.Rows[32 + i][1]);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): WriteCellPopExcelTemplate[" + i + "] = " + WriteCellPopExcelTemplate[i]);
                    // look for even a single week where output occurs
                    if (!CellPopCreated & WriteCellPopExcelTemplate[i] != false)
                    {
                        //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): if (!CellPopCreated & WriteCellPopExcelTemplate[i] != false)");
                        // found one - therefore create the file
                        // build the folder name
                        // create the file
                        CreateCellPopulationOutput(OutFolder.ToString(), string.Format("CellPopulation{0}", mvarTrialNumber.ToString("0000")), ModelBackground.Cells);
                        // indicate it has been created
                        CellPopCreated = true;
                    }
                }

                // get the cell, year, week and level of the initial infections
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): InitialInfections");
                InitialInfections = new cInitialInfectionList();
                cInitialInfection InitInfection = null;                
                // EER: Create a list to store intitial infection cell id's
                //      A list is more efficient than a dynamic array
                List<string> listInfCell = new List<string>();                
                bool addArrayItem;
                addArrayItem = true;
                i = 0;
                do {
                    //Get infection cell ID
                    string infCell = dtEpi.Rows[8 + i][0].ToString();                    
                    i++;
                    if (infCell.Length == 0)
                    {
                        addArrayItem = false;
                    }
                    else {
                        listInfCell.Add(infCell);
                        //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): infCell = " + infCell);
                    }
                } while (addArrayItem == true);                
                // EER: Now that we know how many cells are seeding the infection,
                //      loop that many times and add the cell, year, week, and level to the InitialInfections list
                for (i = 0; i < listInfCell.Count; i++) {
                    InitInfection = new cInitialInfection();
                    InitInfection.InfectionCell = listInfCell[i];
                    //System.Diagnostics.Debug.WriteLine("    InitInfection.InfectionCell " + i + " = " + InitInfection.InfectionCell);
                    InitInfection.Year = Convert.ToInt32(dtEpi.Rows[8 + i][1]);
                    //System.Diagnostics.Debug.WriteLine("     InitInfection.Year " + i + " = " + InitInfection.Year);
                    InitInfection.Week = Convert.ToInt32(dtEpi.Rows[8 + i][2]);
                    //System.Diagnostics.Debug.WriteLine("     InitInfection.Week " + i + " = " + InitInfection.Week);
                    InitInfection.InfectionLevel = Convert.ToInt32(dtEpi.Rows[8 + i][3]);
                    //System.Diagnostics.Debug.WriteLine("     InitInfection.InfectionLevel " + i + " = " + InitInfection.InfectionLevel);
                    if (InitInfection.InfectionCell.Trim().Length > 0 & InitInfection.Week > 0 & InitInfection.Year > 0)
                    {
                        InitialInfections.Add(InitInfection);
                    }                    
                }
                
                // get the static animal data
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): get the static animal data");
                GetStaticAnimalDataExcelTemplate(ModelBackground as cFoxBackground);

                // YM: get the mortality rate 
                //ReadMortalityRate(); 

                // get any strategies the user may have set up
                // EER: this proccess currently does nothing until a new method for inputing strategies is created
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): get any strategies");
                GetStrategiesExcelTemplate();
                
                // create and write the initial summary file
                // EER: this summary file is less extensive than the original created by ORM
                //      it can be expanded, when time allows
                CreateSummaryFileARM(ActualOutputFolder);

                // attach event handlers
                // EER: += is syntax for for incrementing the left side by the right side
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): attach event handlers");
                ModelBackground.WeeklyUpdateBeforeRemovingDead += new WeeklyUpdateBeforeRemovingDeadEventHandler(ModelBackground_WeeklyUpdateBeforeRemovingDead);
                ModelBackground.WeeklyUpdateAfterRemovingDead += new WeeklyUpdateAfterRemovingDeadEventHandler(ModelBackground_WeeklyUpdateAfterRemovingDead);
                
                // now run the model                
                ModelBackground.RunModel();
                
                // detach event handlers
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: RunModelTrialExcelTemplate(): detach event handlers");
                ModelBackground.WeeklyUpdateBeforeRemovingDead -= new WeeklyUpdateBeforeRemovingDeadEventHandler(ModelBackground_WeeklyUpdateBeforeRemovingDead);
                ModelBackground.WeeklyUpdateAfterRemovingDead -= new WeeklyUpdateAfterRemovingDeadEventHandler(ModelBackground_WeeklyUpdateAfterRemovingDead);
                               
                
                // does the user wish to write the resulting animal database
                //if (!this.NoAnimalDatabases && Convert.ToBoolean(drOutput["WriteAnimalDatabase"]))
                bool outputBool = Convert.ToBoolean(dtRunInfo.Rows[20][1]);                                
                if (!this.NoAnimalDatabases && Convert.ToBoolean(dtRunInfo.Rows[20][1]));
                {
                    
                    // write result into a database
                    WriteResultsExcelTemplate();
                }
                
                // close any opened output files
                if ((BasicInfoOutput != null)) BasicInfoOutput.Close();
                if ((RabiesReportOutput != null)) RabiesReportOutput.Close();
                if ((CellPopulationOutput != null)) CellPopulationOutput.Close();
                
                // let various things know that the trial was completed
                CompleteSummaryFile(ActualOutputFolder, "Completed");
                mvarAppReport.IndicateRunCompletion(string.Format("{0} - Trial {1}", mvarRunName, mvarTrialNumber.ToString("0000")), startTime);
                
                if (this.TrialCompleted != null) TrialCompleted();
                // clean up and force a garbage collection
                ModelBackground = null;
                GC.Collect();                
            }
            catch (ThreadAbortException)
            {
                // this one is OK and caused by higher up issues.  Do nothing here other than quickly finish up
                if ((BasicInfoOutput != null)) BasicInfoOutput.Close();
                if ((RabiesReportOutput != null)) RabiesReportOutput.Close();
                if ((CellPopulationOutput != null)) CellPopulationOutput.Close();
                CompleteSummaryFile(ActualOutputFolder, "Aborted");
                mvarAppReport.IndicateRunAbort(string.Format("{0} - Trial {1}", mvarRunName, mvarTrialNumber.ToString("0000")));
                // raise event to indicate that the trial as aborted
                if (this.TrialAborted != null) TrialAborted();                
            }
            catch (Exception ex)
            {
                // close any opened output files
                if ((BasicInfoOutput != null)) BasicInfoOutput.Close();
                if ((RabiesReportOutput != null)) RabiesReportOutput.Close();
                if ((CellPopulationOutput != null)) CellPopulationOutput.Close();
                CompleteSummaryFile(ActualOutputFolder, "Stopped by Error");
                mvarAppReport.IndicateError(string.Format("Error occured in {0} - Trial {1}. {2}", mvarRunName, mvarTrialNumber.ToString("0000"), ex.Message), ex);
                if (this.TrialStoppedWithError != null) TrialStoppedWithError(ex);                
            }            
        }

        // run a single trial of a run
        public virtual void RunModelTrial()
        {
            string ActualOutputFolder = "";
            try
            {
				startTime = DateTime.Now;
                int i = 0;
                // define different data tables and rows
                DataRow drRunInfo = mvarTrialSettings.Tables["RunInfo"].Rows[0];
                DataRow drOutput = mvarTrialSettings.Tables["OutputSheet"].Rows[0];
                DataRow drYears = mvarTrialSettings.Tables["Years"].Rows[0];
                DataRow drDisease = mvarTrialSettings.Tables["DiseaseInformation"].Rows[0];
                DataRow drAnimals = mvarTrialSettings.Tables["AnimalInformation"].Rows[0];
                DataRow drMarkers = null;
                if (mvarTrialSettings.Tables.Contains("InitialMarkers"))
                {
                    drMarkers = mvarTrialSettings.Tables["InitialMarkers"].Rows[0];
                }
                // indicate start of run
                mvarAppReport.IndicateRunStart(string.Format("{0} - Trial {1}", mvarRunName, mvarTrialNumber.ToString("0000")));
                // get the actual output folder
				if (this.OutputFolder != "!-current-!")
				{
	                if (this.OutputFolder.Length != 0)
	                {
	                     ActualOutputFolder = this.OutputFolder;
	                }
	                else
	                {
	                    ActualOutputFolder = drRunInfo["OutputFolder"].ToString();
	                }
	                if (mvarIsUnix)
	                {
	                    if (!ActualOutputFolder.EndsWith("/")) ActualOutputFolder = string.Format("{0}/", ActualOutputFolder);
	                }
	                else
	                {
	                    if (!ActualOutputFolder.EndsWith(@"\")) ActualOutputFolder = string.Format(@"{0}\", ActualOutputFolder);
	                }
				}
                // create folder for database output if user is doing periodic database output
                PeriodicDatabaseOutput = Convert.ToBoolean(drOutput["DatabaseOutput"]);
                PeriodicDatabaseOutputInterval = Convert.ToInt32(drOutput["DatabaseOutputInterval"]);
                PeriodicDatabaseOutputMDB = Convert.ToBoolean(drOutput["DatabaseOutputMDB"]);
                PeriodicDatabaseOutputFolder = string.Format(@"{0}{1}\AnimalDatasources\Trial{2}", ActualOutputFolder, mvarRunName, mvarTrialNumber.ToString("0000"));
                if (mvarIsUnix) PeriodicDatabaseOutputFolder = PeriodicDatabaseOutputFolder.Replace(@"\", "/");
                ScrambleAnimals = Convert.ToBoolean(drRunInfo["ScrambleAnimals"]);
                // create the model datasource
                cCellsDataSource CellDatasource = OpenCellDatasource(drRunInfo["CellDatasource"].ToString());
                mvarRunLength = Convert.ToInt32(drRunInfo["RunLength"]);
                mvarCurrentRunYear = 1;
                cModelDataSource ModelDatasource = null;
                // does the user wish to seed the cells with a single starting animal
                if (Convert.ToBoolean(drRunInfo["StartWithSingleAnimal"]))
                {
                    string mMarkers = "M";
                    string fMarkers = "M";
                    if (drMarkers != null)
                    {
                        mMarkers = drMarkers["Male"].ToString();
                        fMarkers = drMarkers["Female"].ToString();
                    }

                    //YES!!
                    // has user supplied a list of winter types
                    if (Convert.ToBoolean(drYears["UseSpecifiedYearTypes"]))
                    {
                        //YES!
                        // create winter list based on values input into spreadsheet
                        cWinterTypeList wtList = CreateWinterList(mvarRunLength, mvarTrialSettings);
                        // now create the model datasource
                        ModelDatasource = new cFoxModelDataSource(mvarRnd, CellDatasource, wtList, mMarkers, fMarkers);
                    }
                    else
                    {
                        //NO!
                        // create a random list of winters with a specified bias
                        ModelDatasource = new cFoxModelDataSource(mvarRnd, CellDatasource, mvarRunLength, 
                                                                      (enumWinterType)drYears["RandomWinterBias"], mMarkers, fMarkers);
                    }
                }
                else
                {
                    // NO!!
                    // connect to an animals datasource. Additional years will be added later if required
                    ModelDatasource = new cFoxModelDataSource(mvarRnd, CellDatasource, OpenAnimalDatasource(drRunInfo["AnimalDatasource"].ToString()));
                }
                // create the rabies disease object
                int ChanceOfDeath = mvarTrialSettings.Tables["DiseaseInformation"].Columns.Contains("ChanceOfDeath") ? Convert.ToInt32(drDisease["ChanceOfDeath"]) : 100;
                bool RecoveredBecomeImmune = mvarTrialSettings.Tables["DiseaseInformation"].Columns.Contains("RecoveredBecomeImmune") ? Convert.ToBoolean(drDisease["RecoveredBecomeImmune"]) : true;
                bool RecoveredNotInfectious = mvarTrialSettings.Tables["DiseaseInformation"].Columns.Contains("RecoveredNotInfectious") ? Convert.ToBoolean(drDisease["RecoveredNotInfectious"]) : false;
                cBatchRabies Rabies = new cBatchRabies(Convert.ToDouble(drDisease["SpreadProbability"]), mvarRnd, ChanceOfDeath, RecoveredBecomeImmune, RecoveredNotInfectious, mvarTrialSettings);
                
                // create a disease list containing this disease
                cDiseaseList DiseaseList = new cDiseaseList(mvarRnd);
                DiseaseList.Add(Rabies);
                // now use the data source to get a background and load the data into it
                ModelBackground = ModelDatasource.ReadDatasource(string.Format("{0}-{1}", mvarRunName, mvarTrialNumber), false, DiseaseList);              
                // set the abort on rabies die out flag
                ModelBackground.AbortOnDiseaseDisappearance = Convert.ToBoolean(drRunInfo["AbortOnRabiesDieOut"]);
                // set reflective spread flag
                ModelBackground.UseReflectiveDiseaseSpread = Convert.ToBoolean(drDisease["UseReflectiveSpread"]);
                // set prevent incest flag
                ModelBackground.PreventIncest = Convert.ToBoolean(drAnimals["PreventIncest"]);
                // if the animals were loaded from a datasource, add on the required number of years
                if (!Convert.ToBoolean(drRunInfo["StartWithSingleAnimal"]))
                {
                    int YearsToAdd = mvarRunLength - ModelBackground.Years.Count + ModelBackground.Years.CurrentYearNum;
                    if (ModelBackground.Years.IsLastYear & ModelBackground.Years.CurrentYear.CurrentWeek == 52)
                    {
                        YearsToAdd += 1;
                    }
                    if (YearsToAdd > 0)
                    {
                        // has the user asked for random winters or created a list of winters
                        if (Convert.ToBoolean(drYears["UseSpecifiedYearTypes"]))
                        {
                            cWinterTypeList wtList = default(cWinterTypeList);
                            // create winter list based on values input into spreadsheet
                            wtList = CreateWinterList(YearsToAdd, mvarTrialSettings);
                            // add these winter types
                            ModelBackground.Years.AddYears(wtList);
                        }
                        else
                        {
                            // add years with the desired winter bias
                            ModelBackground.Years.AddYears(YearsToAdd, (enumWinterType)drYears["RandomWinterBias"]);
                        }
                    }
                    // increment the week by one because value store in the database was for week
                    // last run
                    ModelBackground.Years.IncrementTime();
                }

                StringBuilder OutFolder = new StringBuilder(ActualOutputFolder);
                OutFolder.Append(mvarRunName);
                // create regular and rabies ouput files if it is requested
                string RegularOutput = (string.IsNullOrEmpty(this.SummaryFileName) ? drOutput["RegularOutputSheetName"].ToString().Trim() : this.SummaryFileName.Trim());
                string RabiesOutput = (string.IsNullOrEmpty(this.RabiesFileName) ? drOutput["RabiesOutputSheetName"].ToString().Trim() : this.RabiesFileName.Trim());
                if (RegularOutput.Length > 0)
                {
                    CreateBasicInfoOutput(OutFolder.ToString(), string.Format("{0}{1}", RegularOutput, mvarTrialNumber.ToString("0000")));
                }
                if (RabiesOutput.Length > 0)
                {
                    CreateRabiesReportOutput(OutFolder.ToString(), string.Format("{0}{1}", RabiesOutput, mvarTrialNumber.ToString("0000")));
                }

                // create a cell by cell report if it is requested
                DataTable dtOutputCellbyCell = mvarTrialSettings.Tables["OutputSheetCellByCellWeeks"];
                bool CellPopCreated = false;
                // loop through each week, looking for at least one where output is requested
                for (i = 0; i <= dtOutputCellbyCell.Rows.Count - 1; i++)
                {
                    // get row
                    DataRow dr = dtOutputCellbyCell.Rows[i];
                    // write value into array
                    WriteCellPop[i] = Convert.ToInt32(dr["WriteData"]);
                    // look for even a single week where output occurs
                    if (!CellPopCreated & WriteCellPop[i] != 0)
                    {
                        // found one - therefore create the file
                        // build the folder name
                        // create the file
                        CreateCellPopulationOutput(OutFolder.ToString(), string.Format("CellPopulation{0}", mvarTrialNumber.ToString("0000")), ModelBackground.Cells);
                        // indicate it has been created
                        CellPopCreated = true;
                    }
                }
                // get the year, week and location of the initial infections
                InitialInfections = new cInitialInfectionList();
                cInitialInfection InitInfection = null;
                DataTable dtInitialInfections = mvarTrialSettings.Tables["DiseaseInformationInitialInfections"];
                // loop through all potential initial infections
                foreach (DataRow dr in dtInitialInfections.Rows)
                {
                    InitInfection = new cInitialInfection();
                    InitInfection.InfectionCell = Convert.ToString(dr["InfectionCell"]);
                    InitInfection.Year = Convert.ToInt32(dr["InfectionYear"]);
                    InitInfection.Week = Convert.ToInt32(dr["InfectionWeek"]);
                    InitInfection.InfectionLevel = Convert.ToInt32(dr["InfectionLevel"]);
                    // if this is a valid initial infection, add it to the list of initial infections
                    if (InitInfection.InfectionCell.Trim().Length > 0 & InitInfection.Week > 0 & InitInfection.Year > 0)
                    {
                        InitialInfections.Add(InitInfection);
                    }
                }
                // get the static animal data
                GetStaticAnimalData(ModelBackground as cFoxBackground);
                // get any strategies the user may have set up
                GetStrategies();
                // create and write the initial summary file
                CreateSummaryFile(ActualOutputFolder);
                // attach event handlers
                ModelBackground.WeeklyUpdateBeforeRemovingDead += new WeeklyUpdateBeforeRemovingDeadEventHandler(ModelBackground_WeeklyUpdateBeforeRemovingDead);
                ModelBackground.WeeklyUpdateAfterRemovingDead += new WeeklyUpdateAfterRemovingDeadEventHandler(ModelBackground_WeeklyUpdateAfterRemovingDead);
                // now run the model
                ModelBackground.RunModel();
                // detach event handlers
                ModelBackground.WeeklyUpdateBeforeRemovingDead -= new WeeklyUpdateBeforeRemovingDeadEventHandler(ModelBackground_WeeklyUpdateBeforeRemovingDead);
                ModelBackground.WeeklyUpdateAfterRemovingDead -= new WeeklyUpdateAfterRemovingDeadEventHandler(ModelBackground_WeeklyUpdateAfterRemovingDead);

                // does the user wish to write the resulting animal database
                if (!this.NoAnimalDatabases && Convert.ToBoolean(drOutput["WriteAnimalDatabase"]))
                {
                    // write result into a database
                    WriteResults();
                }
                // close any opened output files
                if ((BasicInfoOutput != null)) BasicInfoOutput.Close();
                if ((RabiesReportOutput != null)) RabiesReportOutput.Close();
                if ((CellPopulationOutput != null)) CellPopulationOutput.Close();
                // let various things know that the trial was completed
                CompleteSummaryFile(ActualOutputFolder, "Completed");
                mvarAppReport.IndicateRunCompletion(string.Format("{0} - Trial {1}", mvarRunName, mvarTrialNumber.ToString("0000")), startTime);
                if (this.TrialCompleted != null) TrialCompleted();
                // clean up and force a garbage collection
                ModelBackground = null;
                GC.Collect();
            }
            catch (ThreadAbortException)
            {
                // this one is OK and caused by higher up issues.  Do nothing here other than quickly finish up
                if ((BasicInfoOutput != null)) BasicInfoOutput.Close();
                if ((RabiesReportOutput != null)) RabiesReportOutput.Close();
                if ((CellPopulationOutput != null)) CellPopulationOutput.Close();
                CompleteSummaryFile(ActualOutputFolder, "Aborted");
                mvarAppReport.IndicateRunAbort(string.Format("{0} - Trial {1}", mvarRunName, mvarTrialNumber.ToString("0000")));
                // raise event to indicate that the trial as aborted
                if (this.TrialAborted != null) TrialAborted();
            }
            catch (Exception ex)
            {
                // close any opened output files
                if ((BasicInfoOutput != null)) BasicInfoOutput.Close();
                if ((RabiesReportOutput != null)) RabiesReportOutput.Close();
                if ((CellPopulationOutput != null)) CellPopulationOutput.Close();
                CompleteSummaryFile(ActualOutputFolder, "Stopped by Error");
                mvarAppReport.IndicateError(string.Format("Error occured in {0} - Trial {1}. {2}", mvarRunName, mvarTrialNumber.ToString("0000"), ex.Message), ex);
                if (this.TrialStoppedWithError != null) TrialStoppedWithError(ex);
            }
        }

        #endregion

        #region Model Background Event Handlers

        /// <summary>
        /// Handler for weekly update before dead animals are removed from the list
        /// </summary>
        private void ModelBackground_WeeklyUpdateBeforeRemovingDead(object sender, cWeeklyUpdateEventArgs e)
        {

            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: ModelBackground_WeeklyUpdateBeforeRemovingDead()");

            int Count = 0;
            long TotalAge = 0;

            // apply the winter cull at the appropriate week
            if (e.Week == WinterCullWeek) ApplyWinterCull();

            // calculate average age if we are outputing to basic info
            if ((BasicInfoOutput != null))
            {
                if (BasicInfoOutput.FileName.Trim().Length > 0)
                {
                    foreach (cAnimal Animal in ModelBackground.Animals)
                    {
                        if (!Animal.IsAlive) Count += 1;
                        TotalAge += Animal.Age;
                    }
                    BasicInfoData.AverageAge = Convert.ToDouble((TotalAge / (double)ModelBackground.Animals.Count / 52.0).ToString("0.00"));
                    BasicInfoData.NumDeaths = Count;
                }
            }
            // write basic info required for display
            BasicInfoData.Year = e.Year;
            BasicInfoData.Week = e.Week;

            // call virtual method
            this.WeeklyUpdateBeforeDead(e);
        }

        /// <summary>
        /// Handler for weekly update after dead animals are removed from the list
        /// </summary>
        private void ModelBackground_WeeklyUpdateAfterRemovingDead(object sender, Rabies_Model_Core.cWeeklyUpdateEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: ModelBackground_WeeklyUpdateAfterRemovingDead()");

            int NOcc = 0;
            int[] Count = new int[10];
            int i = 0;
            int j = 0;
            int NInfectious = 0;
            int NIncubating = 0;
            int NVaccinated = 0;
            int NInfectedRecovered = 0;
            //Dim CanExit As Boolean
            bool WritingToBasicInfoFile = (BasicInfoOutput != null);
            bool WritingToRabiesReportFile = (RabiesReportOutput != null);

            if (WritingToBasicInfoFile) WritingToBasicInfoFile = (BasicInfoOutput.FileName.Trim().Length > 0);
            if (WritingToRabiesReportFile) WritingToRabiesReportFile = (RabiesReportOutput.FileName.Trim().Length > 0);

            // is there still a population
            if (ModelBackground.Animals.Count == 0) throw new ApplicationException("Animal population is 0.");

            // loop through all cells to get occupied count and animals per cell count
            // if we are not writing basic info and the main for is not visible, don't bother with this
            if (WritingToBasicInfoFile || WritingToRabiesReportFile)
            {                
                // animals per cell calculation
                if (WritingToBasicInfoFile)
                {
                    foreach (cCell Cell in ModelBackground.Cells)
                    {
                        if (Cell.Animals.Count > 0)
                        {
                            NOcc += 1;
                        }
                    }
                    if (NOcc > 0)
                    {
                        BasicInfoData.AnimalsPerCell = ModelBackground.Animals.Count / (double)NOcc;
                    }
                    else
                    {
                        BasicInfoData.AnimalsPerCell = 0;
                    }
                }

                // count of animals by age - preparation for
                for (i = 0; i <= 8; i++)
                {
                    Count[i] = 0;
                }
                // loop through all animals to get count by age, number incubating etc.
                foreach (cAnimal Animal in ModelBackground.Animals)
                {
                    // put animals age in correct slot - if we are writing to a file
                    if (WritingToBasicInfoFile)
                    {
                        i = Convert.ToInt32(Math.Floor((double)Animal.Age / 52.0));
                        if (i > 7)
                        {
                            Count[8] += 1;
                        }
                        else
                        {
                            Count[i] += 1;
                        }
                    }
                    // is the animal incubating a disease or is it infectious
                    if (Animal.HasDisease)
                    {
                        cInfectionList Infections = Animal.GetInfections();
                        foreach (cInfection Infection in Infections)
                        {
                            if (Infection.IsIncubating(e.Year, e.Week))
                            {
                                NIncubating += 1;
                            }
                            else if (Infection.IsInfectious(e.Year, e.Week))
                            {
                                NInfectious += 1;
                                // if this should be reported, report it now
                                if ((RabiesReportOutput != null))
                                {
                                    if (RabiesReportOutput.FileName.Trim().Length > 0)
                                    {
                                        AddToReportFile(Animal, RabiesReportOutput);
                                    }
                                }
                            }
                        }
                    }
                    cInfectionList AnimalInfections = Animal.GetInfections();

                    foreach (cInfection Infection in AnimalInfections)
                    {
                        if (Infection.NaturalImmunity == "Infected_recovered")
                        {
                            NInfectedRecovered += 1;
                        }
                    }
                    BasicInfoData.NIncubating = NIncubating;
                    BasicInfoData.NInfectious = NInfectious;
                    BasicInfoData.NInfectedRecovered = NInfectedRecovered;

                    // does the animal have an active vaccine - only bother if we are writing to a file
                    if (WritingToBasicInfoFile)
                    {
                        cVaccineList Vaccines = Animal.GetVaccines();
                        foreach (cVaccine Vaccine in Vaccines)
                        {
                            if (Vaccine.IsEffective(e.Year, e.Week))
                            {
                                NVaccinated += 1;
                                break;                             }
                        }
                    }
                }
                // update basic info file
                i = ModelBackground.Animals.Count;
                if (WritingToBasicInfoFile)
                {
                    BasicInfoData.TotPop = i;
                    BasicInfoData.NCells = NOcc;
                    if (NOcc > 0)
                    {
                        BasicInfoData.AnimalsPerCell = i / (double)NOcc;
                    }
                    else
                    {
                        BasicInfoData.AnimalsPerCell = 0;
                    }
                    for (j = 0; j <= 8; j++)
                    {
                        BasicInfoData.Age[j] = (double)(100.0 * Count[j] / (double)i);
                    }
                    // update spreadsheet to show disease results
                    BasicInfoData.NIncubating = NIncubating;
                    BasicInfoData.NInfectious = NInfectious;
                    //YM:
                    //update to show the number of vaccinated animal
                    BasicInfoData.NVaccinated = NVaccinated;
                    //update output to shaow the number of immunited animal
                    BasicInfoData.NInfectedRecovered = NInfectedRecovered;
                    //YM: 
                    // test, write on the ouput YoungofYear
                    BasicInfoData.AgeAdult = ModelBackground.Animals.GetCountByAgeClass(enumAgeClass.Adult);
                    BasicInfoData.AgeJuv = ModelBackground.Animals.GetCountByAgeClass(enumAgeClass.Juvenile);
                    BasicInfoData.AgeYoungofYear= ModelBackground.Animals.GetCountByAgeClass(enumAgeClass.YoungOfTheYear);

                    // update the basic info output file
                    BasicInfoOutput.WriteValue(BasicInfoData);
                }
            }

            // output cell by cell results if this is an output week            
            if ((CellPopulationOutput != null))
            {
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: ModelBackground_WeeklyUpdateAfterRemovingDead(): WriteCellPopExcelTemplate[e.Week - 1] = " + WriteCellPopExcelTemplate[e.Week - 1]);
                if (WriteCellPopExcelTemplate[e.Week - 1] != false)
                {
                    CellPopulationOutput.WriteValue(e.Year, e.Week);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: ModelBackground_WeeklyUpdateAfterRemovingDead(): e.Week = " + e.Week);
                }
            }

            // now, is this the week the rabies infection is introduced
            for (i = 0; i <= InitialInfections.Count - 1; i++)
            {                
                if (InitialInfections[i].Year == e.Year & InitialInfections[i].Week == e.Week)
                {
                    // it is time!!! Introduce the infection
                    if (!ModelBackground.Cells.ContainsKey(InitialInfections[i].InfectionCell))
                    {                        
                        // specified cell not found - throw an exception
                        throw new ApplicationException(string.Format("A cell with ID {0} is not found in the current list of cells.  Cannot introduce a new infection", InitialInfections[i].InfectionCell));
                    }
                    // randomly select animals from the source cell.  Make sure they are juvenile
                    // or adult animal
                    cAnimalList PotentialInfectives = ModelBackground.Cells[InitialInfections[i].InfectionCell].Animals;
                    cAnimalList Infectives = new cAnimalList(null);                    
                    foreach (cAnimal a in PotentialInfectives)
                    {
                        if (a.AgeClass != enumAgeClass.YoungOfTheYear) Infectives.Add(a);
                    }
                    // now infect the requisite number (if possible)
                    // if there are no animals in this cell, that is it!!
                    if (Infectives.Count > 0)
                    {                        
                        // calculate the number of animals to infect
                        int NInfect = 1 + (int)(InitialInfections[i].InfectionLevel / 100.0 * Infectives.Count);
                        // make sure this number does not exceed the number of animals
                        if (NInfect > Infectives.Count) NInfect = Infectives.Count;
                        // infect the first 'NInfect' animals in the Infectives list
                        for (j = 0; j <= NInfect - 1; j++)
                        {
                            Infectives[j].Infect(ModelBackground.Diseases["Rabies"], null, true);
                        }
                    }
                }
            }

            // if we have been requested to write animal datasources periodically, check for that now
            if (e.Week == 52 && !this.NoAnimalDatabases)
            {                
                bool OutputDatasource = false;
                // check for periodic output
                if (PeriodicDatabaseOutput)
                {
                    PeriodicDatabaseOutputCount += 1;
                    if (PeriodicDatabaseOutputCount == PeriodicDatabaseOutputInterval)
                    {
                        OutputDatasource = true;
                    }
                }
                // output the datasource if we must
                if (OutputDatasource)
                {                    
                    this.BeforeOutputIntermediateDatasource();
                    // create folder if it does not exist
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: ModelBackground_WeeklyUpdateAfterRemovingDead(): Here06b: PeriodicDatabaseOutputFolder = " + PeriodicDatabaseOutputFolder);
                    if (!Directory.Exists(PeriodicDatabaseOutputFolder))
                    {
                        Directory.CreateDirectory(PeriodicDatabaseOutputFolder);
                    }
                    // output the database
                    StringBuilder DBName = new StringBuilder("AnimalDatasource");
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: ModelBackground_WeeklyUpdateAfterRemovingDead(): Here06c: DBName = " + DBName);
                    DBName.AppendFormat("-Year{0}-Trial{1}.", e.Year, this.TrialNumber.ToString("0000"));
                    if (PeriodicDatabaseOutputMDB && !mvarIsUnix)
                    {
                        DBName.Append("mdb");
                    }
                    else
                    {
                        DBName.Append("xml");
                    }
                    WriteAnimalDatasourceExcelTemplate(PeriodicDatabaseOutputFolder, DBName.ToString());
                    // reset counter to 0
                    PeriodicDatabaseOutputCount = 0;
                    this.AfterOutputIntermediateDatasource();
                }
            }


            // if this year divides by 20, internally rebuild the main animal list.
            // this should keep things ticking along
            if (e.Year > 0)
            {
                if ((e.Year % 20) == 0 & e.Week == 1) ModelBackground.Animals.Rebuild();                
            }
            
            // finally, if the user has requested that the order of the animals be scrambled each
            // week, do it now
            if (ScrambleAnimals) ModelBackground.Animals.ScrambleOrder();
           
            // raise event to indicate progress
            if ((e.Week % 13 == 0))
            {
                if (this.TrialProgress != null)
                {
                    double CurrentLength = (double)(mvarCurrentRunYear - 1) + (double)((e.Week / 13) * 0.25);
                    int Progress = (int)Math.Round((CurrentLength / (double)mvarRunLength) * 100);
                    TrialProgress(mvarRunName, mvarTrialNumber, this.ThreadNumber, Progress);                    
                }
            }
            if (e.Week == 52) mvarCurrentRunYear++;

            // call virtual method
            this.WeeklyUpdateAfterDead(e, NVaccinated);

            // display to console if required
            if (this.PrintYears && e.Week == 1)
            {
                int RunTime = (int)(DateTime.Now - startTime).TotalSeconds;
                Console.WriteLine(string.Format("Trial: {0} - Year: {1} (Execution Time {2}min {3}sec)", this.TrialNumber.ToString("0000"), e.Year.ToString("000"), RunTime / 60, RunTime % 60));
            }
            if (this.PrintWeeks)
            {
                Console.WriteLine(string.Format("Trial: {0} - Year: {1}  Week: {2}", this.TrialNumber.ToString("0000"), e.Year.ToString("000"), e.Week.ToString("00")));
            }            
        }

        // open the cell datasource specified
        //private cCellsDataSource OpenCellDatasource(string DatasourceName)
        // EER: changed this function to be public so it could be used in the ARM windows form interface
        public cCellsDataSource OpenCellDatasource(string DatasourceName)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: cCellsDataSource: OpenCellDatasource()");

            // datasource is assumed to be XML based unless the file name ends with .mdb
            string dbName = "";
            string dbPath = "";
            cCellsDataSource CellDatasource = null;
            // extract file and path
            ExtractPathAndFile(DatasourceName, ref dbPath, ref dbName);
            // create appropriate datasource type
            if (DatasourceName.ToLower().EndsWith(".mdb"))
            {
                CellDatasource = new cMDBCellDataSource(mvarIsUnix);
            }
            else
            {
                CellDatasource = new cXMLCellDataSource(mvarIsUnix);
            }
            // open the datasource
            CellDatasource.DataSourceName = dbName;
            CellDatasource.DataSourcePath = dbPath;
            CellDatasource.Connect();
            // return the opened cell datasource
            return CellDatasource;
        }

        // open the animal datasource specified
        private cAnimalsDataSource OpenAnimalDatasource(string DatasourceName)
        {           

            // datasource is assumed to be XML based unless the file name ends with .mdb
            string dbName = "";
            string dbPath = "";
            cAnimalsDataSource AnimalDatasource = null;
            // extract file and path
            ExtractPathAndFile(DatasourceName, ref dbPath, ref dbName);
            // create appropriate datasource type
            if (DatasourceName.ToLower().EndsWith(".mdb"))
            {
                AnimalDatasource = new cMDBDataSource("", mvarIsUnix);
            }
            else
            {
                AnimalDatasource = new cXMLFoxDataSource(mvarIsUnix);
            }
            
            AnimalDatasource.DataSourcePath = dbPath;            
            AnimalDatasource.DataSourceName = dbName;            
            AnimalDatasource.Connect(true);            
            // return the opened animals datasource            
            return AnimalDatasource;
        }

        // extract a path and file name from a combined path and file name
        private void ExtractPathAndFile(string FullName, ref string Path, ref string FileName)
        {
            int i = 0;
            // is the path a local path or an internet path
            if (FullName.ToLower().StartsWith("http"))
            {
                // internet path  - return entire name as the FileName
                Path = "";
                FileName = FullName;
            }
            else
            {
                // look for the last occurence of the slash character.  If it is not there, then the
                // file name is the full name and the path is zero length
                // NOTE: we don't look for unix path separator here because the FullName is always assigned by a Windows tool
                i = FullName.LastIndexOf(@"\");
                if (i == -1)
                {
                    FileName = FullName;
                    Path = "";
                }
                else
                {
                    Path = FullName.Substring(0, i);
                    FileName = FullName.Substring(i + 1);
					
                }
                // do we need to override the path
                if (this.InputFolder.Length > 0)
                {
					if (this.InputFolder == "!-current-!")
					{
						Path = "";
					}
					else
					{
                    		Path = this.InputFolder;
                    		if (mvarIsUnix)
                    		{
                        		if (!Path.EndsWith("/")) Path = string.Format("{0}/", Path);
                    		}
                    		else
                    		{
                        		if (!Path.EndsWith(@"\")) Path = string.Format(@"{0}\", Path);
                    		}
					}
                }
            }
        }

        // create a winter list based on values input into the Excel template settings file
        private cWinterTypeList CreateWinterListExcelTemplate(int NYears, DataSet TrialSettings)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: cWinterTypeList: CreateWinterListExcelTemplate()");

            //Put winter severity data from Excel template into a DataTable object
            DataTable dtWinter = mvarTrialSettings.Tables[5];
            /*string winter1 = dtWinter.Rows[14][1].ToString();
            string winter2 = dtWinter.Rows[15][1].ToString();
            string winter3 = dtWinter.Rows[16][1].ToString();
            System.Diagnostics.Debug.WriteLine("    winter1 = " + winter1);
            System.Diagnostics.Debug.WriteLine("    winter2 = " + winter2);
            System.Diagnostics.Debug.WriteLine("    winter3 = " + winter3);*/
            
            // how many years is the model running?
            int i = 0;
            //System.Diagnostics.Debug.WriteLine(" length of simulation = " + NYears);
            cWinterTypeList wtList = new cWinterTypeList();            
            
            // now go through the list one by one and build a list of winters
            for (i = 0; i <= NYears - 1; i++)
            {
                // loop back to beginning of list if i is greater than 200
                // EER winter types begin on row 14 of Excel template                                               
                string winter = Convert.ToString(dtWinter.Rows[14 + i % 200][1]);
                //System.Diagnostics.Debug.WriteLine("    winter at i = " + i + " is " + winter);                
                switch (winter)
                {
                    case "VerySevere":
                        wtList.Add(enumWinterType.VerySevere);
                        break;
                    case "Severe":
                        wtList.Add(enumWinterType.Severe);
                        break;
                    case "Mild":
                        wtList.Add(enumWinterType.Mild);
                        break;
                    case "VeryMild":
                        wtList.Add(enumWinterType.VeryMild);
                        break;
                    case "Normal":
                        wtList.Add(enumWinterType.Normal);
                        break;
                }
            }
            // return the created list
            return wtList;
        }

        // create a winter list based on values input into the XML settings file
        private cWinterTypeList CreateWinterList(int NYears, DataSet TrialSettings)
        {
            DataTable dtWinterTypes = mvarTrialSettings.Tables["YearsSpecifiedWinterTypes"];
            // how many years is the model running?
            int i = 0;
            cWinterTypeList wtList = new cWinterTypeList();
            // now go through the list one by one and build a list of winters
            for (i = 0; i <= NYears - 1; i++)
            {
                // loop back to beginning of list if i is greater than 200
                DataRow dr = dtWinterTypes.Rows[i % 200];
                switch (Convert.ToString(dr["WinterType"]))
                {
                    case "VerySevere":
                        wtList.Add(enumWinterType.VerySevere);
                        break;
                    case "Severe":
                        wtList.Add(enumWinterType.Severe);
                        break;
                    case "Mild":
                        wtList.Add(enumWinterType.Mild);
                        break;
                    case "VeryMild":
                        wtList.Add(enumWinterType.VeryMild);
                        break;
                    case "Normal":
                        wtList.Add(enumWinterType.Normal);
                        break;
                }
            }
            // return the created list
            return wtList;
        }

        // create a new basic output text file
        private void CreateBasicInfoOutput(string OutputFolder, string Name)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: CreateBasicInfoOutput()");

            // check for the output folder
            if (OutputFolder != "!-current-!")
			{
	            if (!Directory.Exists(OutputFolder))
	            {
	                Directory.CreateDirectory(OutputFolder);
	            }
			}
            // create output object
            if (mvarIsUnix)
            {
                if (!(OutputFolder.EndsWith("/"))) OutputFolder = string.Format("{0}/", OutputFolder);
            }
            else
            {
                if (!(OutputFolder.EndsWith(@"\"))) OutputFolder = string.Format(@"{0}\", OutputFolder);
            }
            BasicInfoOutput = new cBasicInfoOutput(string.Format("{0}{1}.txt", OutputFolder, Name), this.TabDelimitedOutput, mvarNoOutputExceptions, mvarAppReport);
        }

        // create a new rabies report text file
        private void CreateRabiesReportOutput(string OutputFolder, string Name)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: CreateRabiesReportOutput()");

            // check for the output folder
            if (OutputFolder != "!-current-!")
			{
	            if (!Directory.Exists(OutputFolder))
	            {
	                Directory.CreateDirectory(OutputFolder);
	            }
			}
            // create output object
            if (mvarIsUnix)
            {
                if (!(OutputFolder.EndsWith("/"))) OutputFolder = string.Format("{0}/", OutputFolder);
            }
            else
            {
                if (!(OutputFolder.EndsWith(@"\"))) OutputFolder = string.Format(@"{0}\", OutputFolder);
            }
            RabiesReportOutput = new cRabiesReportOutput(string.Format("{0}{1}.txt", OutputFolder, Name), this.TabDelimitedOutput,  mvarNoOutputExceptions, mvarAppReport);
        }

        // create a new cell population text file
        private void CreateCellPopulationOutput(string OutputFolder, string Name, cCellList Cells)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: CreateCellPopulationOutput()");

            // check for the output folder
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);                
            }
            // create output object
            if (mvarIsUnix)
            {
                if (!(OutputFolder.EndsWith("/"))) OutputFolder = string.Format("{0}/", OutputFolder);                
            }
            else
            {
                if (!(OutputFolder.EndsWith(@"\"))) OutputFolder = string.Format(@"{0}\", OutputFolder);                
            }
                        
            string testt = string.Format("{0}{1}.txt", OutputFolder, Name);
                       
            CellPopulationOutput = new cCellPopulationOutput(Cells, string.Format("{0}{1}.txt", OutputFolder, Name), this.TabDelimitedOutput,  mvarNoOutputExceptions, mvarAppReport);            
        }

        // EER: get the static animal information from the Excel template settings file
        private void GetStaticAnimalDataExcelTemplate(cFoxBackground BG)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: GetStaticAnimalDataExcelTemplate()");
            
            // Load animal biology and behaviour data
            BG.LoadStaticAnimalDataExcelTemplate(mvarTrialSettings);

            // Load winter cull data
            DataTable dtWinter = mvarTrialSettings.Tables[5];
            int WinterCullWeek = Convert.ToInt32(dtWinter.Rows[1][4]);
            //System.Diagnostics.Debug.WriteLine("    WinterCullWeek = " + WinterCullWeek);

            // Load winter cull level            
            for (int i = 0; i <= 7; i++)
            {                
                WinterCullByAgeMale[i] = Convert.ToInt32(dtWinter.Rows[i+10][4]);
                WinterCullByAgeFemale[i] = Convert.ToInt32(dtWinter.Rows[i+10][5]);
                //System.Diagnostics.Debug.WriteLine("    i = " + i + "; WinterCullByAgeMale = " + WinterCullByAgeMale[i] + "; WinterCullByAgeFemale = " + WinterCullByAgeFemale[i]);
            }

            // Load winter cull level (by winter type)            
            for (int i = 0; i <= 4; i++)
            {                
                CullWinterEffect[i] = Convert.ToInt32(dtWinter.Rows[i + 3][4]);
                //System.Diagnostics.Debug.WriteLine("    i = " + i + "; CullWinterEffect = " + CullWinterEffect[i]);
            }
        }

        // get the static animal information from the XML settings file
        private void GetStaticAnimalData(cFoxBackground BG)
        {          
            // Fox Data
            BG.LoadStaticAnimalData(mvarTrialSettings);

            // winter cull data
            DataRow drAnimals = mvarTrialSettings.Tables["AnimalInformation"].Rows[0];
            WinterCullWeek = Convert.ToInt32(drAnimals["WinterCullWeek"]);
            // winter cull level
            DataTable dt = mvarTrialSettings.Tables["AnimalInformationWinterCull"];
            for (int i = 0; i <= 7; i++)
            {
                DataRow dr = dt.Rows[i];
                WinterCullByAgeMale[i] = Convert.ToInt32(dr["Male"]);
                WinterCullByAgeFemale[i] = Convert.ToInt32(dr["Female"]);
            }
            // winter cull level (by winter type)
            dt = mvarTrialSettings.Tables["AnimalInformationWinterCullLevel"];
            for (int i = 0; i <= 4; i++)
            {
                DataRow dr = dt.Rows[i];
                CullWinterEffect[i] = Convert.ToInt32(dr["Level"]);
            }
        }

        // EER: read disease control strategies from ??
        private void GetStrategiesExcelTemplate()
        {
            // EER: this function will need to be created depending on how the strategy data are loaded
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: GetStrategiesExcelTemplate()");

            //YM:
            //cStrategyReader StrategyReader = null;

            //// vaccine strategies            
            //StrategyReader = new cVaccineStrategyReader(mvarTrialSettings);
            //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);

            //// cull strategies            
            //StrategyReader = new cCullStrategyReader(mvarTrialSettings);
            //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);

            //// fertility control strategies            
            //StrategyReader = new cFertilityStrategyReader(mvarTrialSettings);
            //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);

            //// combined fertility and vaccine strategies            
            //StrategyReader = new cCombinedStrategyReader(mvarTrialSettings);
            //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);

            //YM
            // Load control strategies data
            DataTable dtDiseaseCtrl = mvarTrialSettings.Tables[2];
            int VaccineEffectivePeriod = Convert.ToInt32(dtDiseaseCtrl.Rows[0][1]);
            int FertilityCtrlEffectivePeriod = Convert.ToInt32(dtDiseaseCtrl.Rows[1][1]);

            // Get the path to control strategies csv file 
            string StrategiesCtrlDataSourcePath = dtDiseaseCtrl.Rows[2][1].ToString();
            if (mvarIsUnix) StrategiesCtrlDataSourcePath = StrategiesCtrlDataSourcePath.Replace(@"\", "/");
            cStrategyReader StrategyReader = null;
            if (StrategiesCtrlDataSourcePath.Length != 0)
            {
                if (StrategiesCtrlDataSourcePath.EndsWith("csv"))
                {
                    // vaccine strategies            
                    StrategyReader = new cVaccineStrategyReader(mvarTrialSettings);
                    // YM:
                    //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);
                    StrategyReader.GetStrategiesCsvTemplate(StrategiesCtrlDataSourcePath, ModelBackground);
                    // cull strategies            
                    StrategyReader = new cCullStrategyReader(mvarTrialSettings);
                    //YM :
                    //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);
                    StrategyReader.GetStrategiesCsvTemplate(StrategiesCtrlDataSourcePath, ModelBackground);
                    // fertility control strategies            
                    StrategyReader = new cFertilityStrategyReader(mvarTrialSettings);
                    //YM:
                    //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);
                    StrategyReader.GetStrategiesCsvTemplate(StrategiesCtrlDataSourcePath, ModelBackground);
                    // combined fertility and vaccine strategies            
                    StrategyReader = new cCombinedStrategyReader(mvarTrialSettings);
                    //YM:
                    //StrategyReader.GetStrategiesExcelTemplate(ModelBackground);
                    StrategyReader.GetStrategiesCsvTemplate(StrategiesCtrlDataSourcePath, ModelBackground);
                }
                else
                {
                    throw new ArgumentException("The file indicated by the path, must be a csv", "strategiesctrlpath");
                }
            }
        }

        // read strategies from the Trial Settings dataset
        private void GetStrategies()
        {
            cStrategyReader StrategyReader = null;
            // vacccine strategies
            StrategyReader = new cVaccineStrategyReader(mvarTrialSettings);
            StrategyReader.GetStrategies(ModelBackground);
            // cull strategies
            StrategyReader = new cCullStrategyReader(mvarTrialSettings);
            StrategyReader.GetStrategies(ModelBackground);
            // fertility control strategies
            StrategyReader = new cFertilityStrategyReader(mvarTrialSettings);
            StrategyReader.GetStrategies(ModelBackground);
            // combined control strategies
            StrategyReader = new cCombinedStrategyReader(mvarTrialSettings);
            StrategyReader.GetStrategies(ModelBackground);
        }

        // write the results of a run to a database or xml file
        // EER: this function takes parameters from the ARM Excel template settings file
        private void WriteResultsExcelTemplate()
        {            
            // get a reference to the table with the required info
            //DataRow drRunInfo = mvarTrialSettings.Tables["RunInfo"].Rows[0];
            DataTable dtRunInfo = mvarTrialSettings.Tables[4];
            // get the desired name for the database
            //string DBName = Convert.ToString(drRunInfo["RunName"]);
            string DBName = Convert.ToString(dtRunInfo.Rows[0][1]);
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteResultsExcelTemplate(): DBName = " + DBName);
            if (DBName.ToLower().EndsWith(".xml")) DBName = DBName.Substring(0, DBName.Length - 4);
            // Append the trial number to the name
            DBName = string.Format("{0}{1}", DBName, mvarTrialNumber.ToString("0000"));
            // add extension to name
            if (this.PeriodicDatabaseOutputMDB && !mvarIsUnix)
            {
                DBName = string.Format("{0}.mdb", DBName);
            }
            else
            {
                DBName = string.Format("{0}.xml", DBName);
            }
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteResultsExcelTemplate(): DBName with extension = " + DBName);
            // get the actual output folder
            string ActualOutputFolder = "";
            if (this.OutputFolder != "!-current-!")
            {
                if (this.OutputFolder.Length != 0)
                {
                    ActualOutputFolder = this.OutputFolder;
                }
                else
                {
                    //ActualOutputFolder = drRunInfo["OutputFolder"].ToString();
                    ActualOutputFolder = dtRunInfo.Rows[2][1].ToString();
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteResultsExcelTemplate(): ActualOutputFolder = " + ActualOutputFolder);
                }
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteResultsExcelTemplate(): mvarIsUnix = " + mvarIsUnix);
                if (mvarIsUnix)
                {
                    if (!ActualOutputFolder.EndsWith("/")) ActualOutputFolder = string.Format("{0}/", ActualOutputFolder);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteResultsExcelTemplate(): Unix ActualOutputFolder = " + ActualOutputFolder);
                }
                else
                {
                    if (!ActualOutputFolder.EndsWith(@"\")) ActualOutputFolder = string.Format(@"{0}\", ActualOutputFolder);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteResultsExcelTemplate(): Windows ActualOutputFolder = " + ActualOutputFolder);
                }
            }            
            //string DBPath = string.Format("{0}{1}", ActualOutputFolder, drRunInfo["RunName"]);
            string DBPath = string.Format("{0}{1}", ActualOutputFolder, dtRunInfo.Rows[0][1]);
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteResultsExcelTemplate(): DBPath = " + DBPath);         
            // now write the datasource
            WriteAnimalDatasourceExcelTemplate(DBPath, DBName);            
        }

        // write the results of a run to a database or xml file
        private void WriteResults()
        {            
            // get a reference to the table with the required info
            DataRow drRunInfo = mvarTrialSettings.Tables["RunInfo"].Rows[0];
            // get the desired name for the database
            string DBName = Convert.ToString(drRunInfo["RunName"]);
            if (DBName.ToLower().EndsWith(".xml")) DBName = DBName.Substring(0, DBName.Length - 4);
            // Append the trial number to the name
            DBName = string.Format("{0}{1}", DBName, mvarTrialNumber.ToString("0000"));
            // add extension to name
            if (this.PeriodicDatabaseOutputMDB && !mvarIsUnix)
            {
                DBName = string.Format("{0}.mdb", DBName);
            }
            else
            {
                DBName = string.Format("{0}.xml", DBName);
            }
            // get the actual output folder
            string ActualOutputFolder = "";
			if (this.OutputFolder != "!-current-!")
			{
	        		if (this.OutputFolder.Length != 0)
	            {
	                ActualOutputFolder = this.OutputFolder;
	            }
	            else
	            {
	                ActualOutputFolder = drRunInfo["OutputFolder"].ToString();
	            }
	            if (mvarIsUnix)
	            {
	                if (!ActualOutputFolder.EndsWith("/")) ActualOutputFolder = string.Format("{0}/", ActualOutputFolder);
	            }
	            else
	            {
	                if (!ActualOutputFolder.EndsWith(@"\")) ActualOutputFolder = string.Format(@"{0}\", ActualOutputFolder);
	            }
			}
            string DBPath = string.Format("{0}{1}", ActualOutputFolder, drRunInfo["RunName"]);
            // now write the datasource
            WriteAnimalDatasourceExcelTemplate(DBPath, DBName);
        }

        // write an animal datasource with passed path and filename
        // EER: write output to database from using the Excel Template settings file
        // EER: the code is only updated to output in XML format
        // EER: If MS Access mdf format is required, update code in cMDBDataSource.cs to input from ARM Excel template settings file
        private void WriteAnimalDatasourceExcelTemplate(string DatasourcePath, string DatasourceName)
        {            
            
            // get a reference to the table with the required info                   
            DataTable dtEpi = mvarTrialSettings.Tables[3];
            DataTable dtRunInfo = mvarTrialSettings.Tables[4];                       

            if (DatasourcePath.Length > 0)
            {
                if (!(DatasourcePath.EndsWith(@"\") || DatasourcePath.EndsWith("/")))
                {
                    if (mvarIsUnix)
                    {
                        DatasourcePath = string.Format("{0}/", DatasourcePath);
                    }
                    else
                    {
                        DatasourcePath = string.Format(@"{0}\", DatasourcePath);
                    }
                }
            }
            string DBName = string.Format("{0}{1}", DatasourcePath, DatasourceName);
            
            // check for the ".mdb" extension in the file name.  If it is there, open an
            // mdb datasource, otherwise open an XML datasource
            cFoxDataSource ResultDatabase = null;
            if (DatasourceName.ToLower().EndsWith(".mdb"))
            {
                // if file already exists, erase it
                try
                {
                    if (File.Exists(DBName)) File.Delete(DBName);
                }
                catch { }
                ResultDatabase = new cMDBDataSource(mvarTemplateDatabase, mvarIsUnix);
            }
            else
            {
                // if an XML file with this name exists, delete it now
                if (File.Exists(DBName)) File.Delete(DBName);
                // write the new XML file
                // EER: UseShortFieldNames
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate(): UseShortFieldNames = " + dtRunInfo.Rows[21][1]);
                if (Convert.ToBoolean(dtRunInfo.Rows[21][1]))
                {                    
                    ResultDatabase = new cXMLFoxDataSource(enumFieldNameSize.ShortNames, mvarIsUnix);                    
                }
                else
                {                    
                    ResultDatabase = new cXMLFoxDataSource(enumFieldNameSize.LongNames, mvarIsUnix);
                }
            }
            
            ResultDatabase.DataSourceName = DatasourceName;            
            ResultDatabase.DataSourcePath = DatasourcePath;            
            try
            {                
                ResultDatabase.Connect(false);                
                ResultDatabase.WriteYearAndAnimalData(ModelBackground.Years, ModelBackground.Animals, ModelBackground);                
                // get metadata from spreadsheet and write it to the datasource
                cAnimalsMetadata MD = new cAnimalsMetadata();                
                // EER: Username                
                MD.CreatorName = (string.IsNullOrEmpty(mvarRunPerson) ? dtRunInfo.Rows[1][16].ToString() : mvarRunPerson);                
                // EER: UserOrganization                
                MD.CreatorAffiliation = dtRunInfo.Rows[17][1].ToString();                
                // EER:UserContactInfo 
                MD.CreatorContactInfo = dtRunInfo.Rows[18][1].ToString();               
                MD.Animals = "Fox";
                // EER: RunName                
                MD.RunName = (string.IsNullOrEmpty(mvarRunName) ? dtRunInfo.Rows[0][1].ToString() : mvarRunName);                
                // EER: RunComments
                MD.RunComments = dtRunInfo.Rows[1][1].ToString();                
                // EER: If true to StartWithSingleAnimal
                if (Convert.ToBoolean(dtRunInfo.Rows[4][1]))
                {
                    MD.StartingDatasource = "Started with single seed animal";                    
                }
                else
                {
                    // EER: else, use existing animal datasource population
                    MD.StartingDatasource = dtRunInfo.Rows[0][7].ToString();                    
                }
                MD.RandomGenerator = "RngStream";
                MD.RandomSeed = ModelBackground.RandomNum.InitialState;
                // EER: landscape
                MD.CellDatasourceLocation = Convert.ToString(dtRunInfo.Rows[3][1]);                
                MD.CellDatasourceName = ModelBackground.Cells.Name;
                MD.TotalPopulation = ModelBackground.Animals.Count;
                // set the next year
                MD.NextYear = ModelBackground.Years.CurrentYearNum;
                MD.NextWeek = ModelBackground.Years.CurrentYear.CurrentWeek + 1;
                if (MD.NextWeek > 52)
                {
                    MD.NextYear += 1;
                    MD.NextWeek = 1;
                }
                MD.DiseasePresent = false;
                // check for the presence of disease
                foreach (cAnimal A in ModelBackground.Animals)
                {
                    if (A.HasDisease)
                    {
                        MD.DiseasePresent = true;
                        break;
                    }
                }
                // write the metadata
                ResultDatabase.WriteMetadata(MD);
                // write data to datasource
                ResultDatabase.WriteFile();
                ResultDatabase.Disconnect();
                bool Temp = mvarAppReport.ShowDialogs;
                mvarAppReport.ShowDialogs = false;
                mvarAppReport.SendMessage(string.Format("Created animal datasource {0}", DBName));
                mvarAppReport.ShowDialogs = Temp;

                // now confirm the saved file by doing a reload and compare
                //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate(): this.ConfirmDatasources = " + this.ConfirmDatasources);
                if (this.ConfirmDatasources)
                {
                    cCellsDataSource ConfirmCells = OpenCellDatasource(dtRunInfo.Rows[3][1].ToString());
                    cFoxDataSource ConfirmDS = null;
                    if (DatasourceName.EndsWith(".mdb"))
                    {
                        ConfirmDS = new cMDBDataSource("", false);
                    }
                    else
                    {
                        ConfirmDS = new cXMLFoxDataSource(mvarIsUnix);
                    }
                    ConfirmDS.DataSourcePath = DatasourcePath;
                    ConfirmDS.DataSourceName = DatasourceName;
                    ConfirmDS.Connect(true);
                    cFoxModelDataSource ModelDatasource = new cFoxModelDataSource(mvarRnd, ConfirmCells, ConfirmDS);
                    // create the rabies disease object
                    int ChanceOfDeath = Convert.ToInt32(dtEpi.Rows[1][1]);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate():  ChanceOfDeath = " + ChanceOfDeath);                    
                    bool RecoveredBecomeImmune = Convert.ToBoolean(dtEpi.Rows[3][1]);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate():  RecoveredBecomeImmune = " + RecoveredBecomeImmune);
                    bool RecoveredNotInfectious = Convert.ToBoolean(dtEpi.Rows[4][1]);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate():  RecoveredNotInfectious = " + RecoveredNotInfectious);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate():  SpreadProbability = " + dtEpi.Rows[0][1]);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate():  mvarRnd = " + mvarRnd);
                    cBatchRabies Rabies = new cBatchRabies(Convert.ToDouble(dtEpi.Rows[0][1]), mvarRnd, ChanceOfDeath, RecoveredBecomeImmune, RecoveredNotInfectious, mvarTrialSettings,true);
                    
                    // create a disease list containing this disease
                    cDiseaseList ConfirmDiseaseList = new cDiseaseList(mvarRnd);
                    ConfirmDiseaseList.Add(Rabies);                    

                    // now use the data source to get a background and load the data into it
                    cBackground ConfirmBG = ModelDatasource.ReadDatasource(string.Format("{0}-{1}", mvarRunName, mvarTrialNumber), false, ConfirmDiseaseList);                    

                    // OK now compare the animals in this list with those the were just saved.  Indicate a problem if they do not match
                    bool Failed = false;
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate(): ConfirmBG.Animals.Count = " + ConfirmBG.Animals.Count);
                    //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: WriteAnimalDatasourceExcelTemplate(): ModelBackground.Animals.Count = " + ModelBackground.Animals.Count);

                    if (ConfirmBG.Animals.Count != ModelBackground.Animals.Count)
                    {
                        mvarAppReport.WriteLogEntry("Animal datasource confirmation failed.  Number of animals in the written datasource are different from the the number of animals currently defined");
                        Failed = true;
                    }
                    else
                    {
                        for (int i = 0; i < ModelBackground.Animals.Count; i++)
                        {
                            if (!ModelBackground.Animals[i].Equals(ConfirmBG.Animals[i]))
                            {
                                mvarAppReport.WriteLogEntry(string.Format("Animal datasource confirmation failed. Equality comparison for animal with ID \"{0}\" failed (at list position {1} - comparison ID \"{2}\")", ModelBackground.Animals[i].ID, i, ConfirmBG.Animals[i].ID));
                                Failed = true;
                            }
                        }
                    }
                    if (Failed)
                    {
                        mvarAppReport.SendMessage("Animal datasource confirmation failed.  See application log for more details");
                    }
                }
            }
            catch (Exception ex)
            {
                mvarAppReport.IndicateError(string.Format("Cannot write animal data - {0}", ex.Message), ex);
            }            
        }

        // write an animal datasource with passed path and filename
        private void WriteAnimalDatasource(string DatasourcePath, string DatasourceName)
        {
            // get a reference to the table with the required info
            DataRow drOutput = mvarTrialSettings.Tables["OutputSheet"].Rows[0];
            DataRow drRunInfo = mvarTrialSettings.Tables["RunInfo"].Rows[0];
            DataRow drDisease = mvarTrialSettings.Tables["DiseaseInformation"].Rows[0];
			if (DatasourcePath.Length > 0)
			{
	            if (!(DatasourcePath.EndsWith(@"\") || DatasourcePath.EndsWith("/")))
	            {
	                if (mvarIsUnix)
	                {
	                    DatasourcePath = string.Format("{0}/", DatasourcePath);
	                }
	                else
	                {
	                    DatasourcePath = string.Format(@"{0}\", DatasourcePath);
	                }
	            }
			}
            string DBName = string.Format("{0}{1}", DatasourcePath, DatasourceName);
            // check for the ".mdb" extension in the file name.  If it is there, open an
            // mdb datasource, otherwise open an XML datasource
            cFoxDataSource ResultDatabase = null;
            if (DatasourceName.ToLower().EndsWith(".mdb"))
            {
                // if file already exists, erase it
                try
                {
                    if (File.Exists(DBName)) File.Delete(DBName);
                }
                catch { }
                ResultDatabase = new cMDBDataSource(mvarTemplateDatabase, mvarIsUnix);
            }
            else
            {
                // if an XML file with this name exists, delete it now
                if (File.Exists(DBName)) File.Delete(DBName);
                // write the new XML file
                if (Convert.ToBoolean(drOutput["UseShortFieldNames"]))
                {
                    ResultDatabase = new cXMLFoxDataSource(enumFieldNameSize.ShortNames, mvarIsUnix);
                }
                else
                {
                    ResultDatabase = new cXMLFoxDataSource(enumFieldNameSize.LongNames, mvarIsUnix);
                }
            }
            ResultDatabase.DataSourceName = DatasourceName;
            ResultDatabase.DataSourcePath = DatasourcePath;
            try
            {
                ResultDatabase.Connect(false);
                ResultDatabase.WriteYearAndAnimalData(ModelBackground.Years, ModelBackground.Animals, ModelBackground);
                // get metadata from spreadsheet and write it to the datasource
                cAnimalsMetadata MD = new cAnimalsMetadata();
                DataRow dr = mvarTrialSettings.Tables["UserInfo"].Rows[0];
                MD.CreatorName = (string.IsNullOrEmpty(mvarRunPerson) ? dr["Username"].ToString() : mvarRunPerson);
                MD.CreatorAffiliation = dr["UserOrganization"].ToString();
                MD.CreatorContactInfo = dr["UserContactInfo"].ToString();
                MD.Animals = "Fox";
                MD.RunName = (string.IsNullOrEmpty(mvarRunName) ? drRunInfo["RunName"].ToString() : mvarRunName);
                MD.RunComments = drRunInfo["RunComments"].ToString();
                if (Convert.ToBoolean(drRunInfo["StartWithSingleAnimal"]))
                {
                    MD.StartingDatasource = "Started with single seed animal";
                }
                else
                {
                    MD.StartingDatasource = drRunInfo["AnimalDatasource"].ToString();
                }
                MD.RandomGenerator = "RngStream";
                MD.RandomSeed = ModelBackground.RandomNum.InitialState;
                MD.CellDatasourceLocation = Convert.ToString(drRunInfo["CellDatasource"]);
                MD.CellDatasourceName = ModelBackground.Cells.Name;
                MD.TotalPopulation = ModelBackground.Animals.Count;
                // set the next year
                MD.NextYear = ModelBackground.Years.CurrentYearNum;
                MD.NextWeek = ModelBackground.Years.CurrentYear.CurrentWeek + 1;
                if (MD.NextWeek > 52)
                {
                    MD.NextYear += 1;
                    MD.NextWeek = 1;
                }
                MD.DiseasePresent = false;
                // check for the presence of disease
                foreach (cAnimal A in ModelBackground.Animals)
                {
                    if (A.HasDisease)
                    {
                        MD.DiseasePresent = true;
                        break;                     }
                }
                // write the metadata
                ResultDatabase.WriteMetadata(MD);
                // write data to datasource
                ResultDatabase.WriteFile();
                ResultDatabase.Disconnect();
                bool Temp = mvarAppReport.ShowDialogs;
                mvarAppReport.ShowDialogs = false;
                mvarAppReport.SendMessage(string.Format("Created animal datasource {0}", DBName));
                mvarAppReport.ShowDialogs = Temp;

                // now confirm the saved file by doing a reload and compare
                if (this.ConfirmDatasources)
                {
                    cCellsDataSource ConfirmCells = OpenCellDatasource(drRunInfo["CellDatasource"].ToString());
                    cFoxDataSource ConfirmDS = null;
                    if (DatasourceName.EndsWith(".mdb"))
                    {
                        ConfirmDS = new cMDBDataSource("", false);
                    }
                    else
                    {
                        ConfirmDS = new cXMLFoxDataSource(mvarIsUnix);
                    }
                    ConfirmDS.DataSourcePath = DatasourcePath;
                    ConfirmDS.DataSourceName = DatasourceName;
                    ConfirmDS.Connect(true);
                    cFoxModelDataSource ModelDatasource = new cFoxModelDataSource(mvarRnd, ConfirmCells, ConfirmDS);
                    // create the rabies disease object
                    int ChanceOfDeath = mvarTrialSettings.Tables["DiseaseInformation"].Columns.Contains("ChanceOfDeath") ? Convert.ToInt32(drDisease["ChanceOfDeath"]) : 100;
                    bool RecoveredBecomeImmune = mvarTrialSettings.Tables["DiseaseInformation"].Columns.Contains("RecoveredBecomeImmune") ? Convert.ToBoolean(drDisease["RecoveredBecomeImmune"]) : true;
                    bool RecoveredNotInfectious = mvarTrialSettings.Tables["DiseaseInformation"].Columns.Contains("RecoveredNotInfectious") ? Convert.ToBoolean(drDisease["RecoveredNotInfectious"]) : false;
                    cBatchRabies Rabies = new cBatchRabies(Convert.ToDouble(drDisease["SpreadProbability"]), mvarRnd, ChanceOfDeath, RecoveredBecomeImmune, RecoveredNotInfectious, mvarTrialSettings);
                    // create a disease list containing this disease
                    cDiseaseList ConfirmDiseaseList = new cDiseaseList(mvarRnd);
                    ConfirmDiseaseList.Add(Rabies);

                    // now use the data source to get a background and load the data into it
                    cBackground ConfirmBG = ModelDatasource.ReadDatasource(string.Format("{0}-{1}", mvarRunName, mvarTrialNumber), false, ConfirmDiseaseList);

                    // OK now compare the animals in this list with those the were just saved.  Indicate a problem if they do not match
                    bool Failed = false;
                    if (ConfirmBG.Animals.Count != ModelBackground.Animals.Count)
                    {
                        mvarAppReport.WriteLogEntry("Animal datasource confirmation failed.  Number of animals in the written datasource are different from the the number of animals currently defined");
                        Failed = true;
                    }
                    else
                    {
                        for (int i = 0; i < ModelBackground.Animals.Count; i++)
                        {
                            if (!ModelBackground.Animals[i].Equals(ConfirmBG.Animals[i]))
                            {
                                mvarAppReport.WriteLogEntry(string.Format("Animal datasource confirmation failed. Equality comparison for animal with ID \"{0}\" failed (at list position {1} - comparison ID \"{2}\")", ModelBackground.Animals[i].ID, i, ConfirmBG.Animals[i].ID));
                                Failed = true;
                            }
                        }
                    }
                    if (Failed)
                    {
                        mvarAppReport.SendMessage("Animal datasource confirmation failed.  See application log for more details");
                    }
                }
            }
            catch (Exception ex)
            {
                mvarAppReport.IndicateError(string.Format("Cannot write animal data - {0}", ex.Message), ex);
            }
        }

        // apply the winter cull
        private void ApplyWinterCull()
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: ApplyWinterCull()");

            int CullLevel = 0;
            // get the level of the cull
            switch (ModelBackground.Years.CurrentYear.WinterType)
            {
                case enumWinterType.VerySevere:
                    CullLevel = CullWinterEffect[0];
                    break;
                case enumWinterType.Severe:
                    CullLevel = CullWinterEffect[1];
                    break;
                case enumWinterType.Normal:
                    CullLevel = CullWinterEffect[2];
                    break;
                case enumWinterType.Mild:
                    CullLevel = CullWinterEffect[3];
                    break;
                case enumWinterType.VeryMild:
                    CullLevel = CullWinterEffect[4];
                    break;
            }
            // if the cull level is greater than 0, apply it!!
            if (CullLevel > 0)
            {
                // get the total number of animals
                int TotalAnimals = ModelBackground.Animals.Count;
                int TotalAnimalsToDie = (int)(TotalAnimals * CullLevel / 100.0);
                int AgeInYears = 0;
                cAnimalList AgeList = null;
                double ChanceToDie = 0;
                int NumberToDie = 0;
                enumGender CurrentGender = enumGender.male;
                // loop through ages 0 through 7 and genders male and female.  Make a collection of animals
                // in each age group
                for(;;)
                {
                    for (AgeInYears = 0; AgeInYears <= 7; AgeInYears++)
                    {
                        // loop through all animals looking for all of the correct age
                        AgeList = new cAnimalList(null);
                        foreach (cAnimal Animal in ModelBackground.Animals)
                        {
                            if (Animal.Age / 52 == AgeInYears & Animal.Gender == CurrentGender)
                            {
                                AgeList.Add(Animal);
                            }
                        }
                        // chance for each animal in this list to die is the number that
                        // should die divided by the total number in the list
                        if (CurrentGender == enumGender.male)
                        {
                            NumberToDie = (int)(WinterCullByAgeMale[AgeInYears] / 100.0 * TotalAnimalsToDie);
                        }
                        else
                        {
                            NumberToDie = (int)(WinterCullByAgeFemale[AgeInYears] / 100.0 * TotalAnimalsToDie);
                        }
                        ChanceToDie = NumberToDie / (double)(AgeList.Count * 100);
                        // loop through all animals.  They roll the dice and die if they come
                        // up with a number less than the cull level
                        foreach (cAnimal Animal in AgeList)
                        {
                            if (Animal.IsAlive)
                            {
                                if (ModelBackground.RandomNum.RealValue(0, 100) < ChanceToDie)
                                {
                                    Animal.Die();
                                }
                            }
                        }
                    }
                    // switch to females.  If we have already done so, exit the loop
                    if (CurrentGender == enumGender.male)
                    {
                        CurrentGender = enumGender.female;
                    }
                    else
                    {
                        break; 
                    }
                }
            }
        }

        // output information about an animal into the Report File
        public void AddToReportFile(cAnimal Animal, cRabiesReportOutput ReportFile)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: AddToReportFile()");

            cCellTimeList Cells = null;
            string CellHistory = null;
            cInfectionList InfList = null;
            RabiesReportData.AnimalID = Convert.ToInt32(Animal.ID);
            RabiesReportData.CellID = Animal.CurrentCell.ID;
            RabiesReportData.Year = Animal.Background.Years.CurrentYearNum;
            RabiesReportData.Week = Animal.Background.Years.CurrentYear.CurrentWeek;
            InfList = Animal.GetInfections();
            RabiesReportData.IncubationPeriod = InfList[0].IncubationPeriod;
            RabiesReportData.InfectiousPeriod = InfList[0].InfectiousPeriod;
            RabiesReportData.WasFatal = InfList[0].IsFatal;
            RabiesReportData.Parent_ID = Convert.ToInt32(Animal.ParentID);
            if (InfList[0].InfectingAnimalID.Length > 0)
            {
                RabiesReportData.Infecting_ID = Convert.ToInt32(InfList[0].InfectingAnimalID);
            }
            else
            {
                RabiesReportData.Infecting_ID = -1;
            }
            RabiesReportData.Age = Animal.Age;
            RabiesReportData.Gender = Animal.Gender;
            CellHistory = "";
            Cells = Animal.GetCellsAndTime();
            foreach (cCellTime Cell in Cells)
            {
                CellHistory += Cell.CellID + ", ";
            }
            CellHistory = CellHistory.Substring(0, CellHistory.Length - 2);
            RabiesReportData.MovementHistory = CellHistory;
            ReportFile.WriteValue(RabiesReportData);
        }

        /// <summary>
        /// Write a summary file just before the trial begins
        /// </summary>
        /// <param name="ActualOutputFolder">The folder into which the summary file is written</param>
        protected virtual void CreateSummaryFileARM(string ActualOutputFolder)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: CreateSummaryFileARM()");

            string fName = "";
            if (mvarIsUnix)
            {
                fName = string.Format("{0}{1}/Summary{2}.txt", ActualOutputFolder, this.RunName, this.TrialNumber.ToString("0000"));
            }
            else
            {
                fName = string.Format(@"{0}{1}\Summary{2}.txt", ActualOutputFolder, this.RunName, this.TrialNumber.ToString("0000"));
            }
           
            // create the output file
            cFileOutput SummaryOutput = new cFileOutput(fName, false, mvarNoOutputExceptions, mvarAppReport);

            // get various table for run info
            DataTable dtAnimalBehav = mvarTrialSettings.Tables[0];
            DataTable dtAnimalBiol = mvarTrialSettings.Tables[1];
            DataTable dtDiseaseCtrl = mvarTrialSettings.Tables[2];
            DataTable dtEpi = mvarTrialSettings.Tables[3];
            DataTable dtRunInfo = mvarTrialSettings.Tables[4];
            DataTable dtWinter = mvarTrialSettings.Tables[5];

            // get actual datasource paths
            string dbPath = "";
            string dbFile = "";
            string AnimalDB = "";
            this.ExtractPathAndFile(dtRunInfo.Rows[2][1].ToString(), ref dbPath, ref dbFile);
            string CellDB = string.Format("{0}{1}", dbPath, dbFile);
            //drRunInfo["CellDatasource"].ToString();
            if (!Convert.ToBoolean(dtRunInfo.Rows[4][1]))
            {                
                AnimalDB = dtRunInfo.Rows[7][1].ToString();                                
            }
            if (Convert.ToBoolean(dtRunInfo.Rows[4][1])) {
                AnimalDB = "NA; population was grown from Adam & Eve";
            }

            //create summary
            SummaryOutput.WriteString(string.Format("Run name: {0}", this.RunName));
            SummaryOutput.WriteString(string.Format("Trial number: {0}", this.TrialNumber.ToString("0000")));
            SummaryOutput.WriteString(string.Format("Start date and time: {0}", this.startTime.ToString("yyyy-MMM-dd hh:mm:ss")));
            SummaryOutput.WriteString(string.Format("Initial random seed value for all streams: {0}", this.ModelBackground.RandomNum.StreamSeed));
            SummaryOutput.WriteString(string.Format("Stream ID for generator used for this trial: {0}", this.ModelBackground.RandomNum.StreamID));
            SummaryOutput.WriteString(string.Format("Intended run length: {0} years", dtRunInfo.Rows[8][1].ToString()));
            SummaryOutput.WriteString(string.Format("Cell datasource: {0}", CellDB));
            SummaryOutput.WriteString(string.Format("Animal datasource: {0}", AnimalDB));

            SummaryOutput.Close();
        }

        /// <summary>
        /// Write a summary file just before the trial begins
        /// </summary>
        /// <param name="ActualOutputFolder">The folder into which the summary file is written</param>
        protected virtual void CreateSummaryFile(string ActualOutputFolder)
        {
            string fName = "";
            if (mvarIsUnix)
            {
                fName = string.Format("{0}{1}/Summary{2}.txt", ActualOutputFolder, this.RunName, this.TrialNumber.ToString("0000"));
            }
            else
            {
                fName = string.Format(@"{0}{1}\Summary{2}.txt", ActualOutputFolder, this.RunName, this.TrialNumber.ToString("0000"));
            }


            // create the output file
            cFileOutput SummaryOutput = new cFileOutput(fName, false, mvarNoOutputExceptions, mvarAppReport);
   
			// get various table for run info
            DataRow drRunInfo = mvarTrialSettings.Tables["RunInfo"].Rows[0];
            DataRow drDisease = mvarTrialSettings.Tables["DiseaseInformation"].Rows[0];
            DataRow drFertility = mvarTrialSettings.Tables["FertilityControlInformation"].Rows[0];
            DataRow drAnimals = mvarTrialSettings.Tables["AnimalInformation"].Rows[0];
            DataRow drMarkers = null;
            if (mvarTrialSettings.Tables.Contains("InitialMarkers"))
            {
                drMarkers = mvarTrialSettings.Tables["InitialMarkers"].Rows[0];
            }

			// get actual datasource paths
			string dbPath = "";
			string dbFile = "";
			string AnimalDB = "";
			this.ExtractPathAndFile(	drRunInfo["CellDatasource"].ToString(), ref dbPath, ref dbFile);
			string CellDB = string.Format("{0}{1}", dbPath, dbFile);
			drRunInfo["CellDatasource"].ToString();
            if (!Convert.ToBoolean(drRunInfo["StartWithSingleAnimal"]))
            {
				this.ExtractPathAndFile(drRunInfo["AnimalDatasource"].ToString(), ref dbPath, ref dbFile);
				AnimalDB = string.Format("{0}{1}", dbPath, dbFile);
			}
			
            // create summary
            SummaryOutput.WriteString(string.Format("Run name: {0}", this.RunName));
            SummaryOutput.WriteString(string.Format("Trial number: {0}", this.TrialNumber.ToString("0000")));
            SummaryOutput.WriteString(string.Format("Start date and time: {0}", this.startTime.ToString("yyyy-MMM-dd hh:mm:ss")));
            SummaryOutput.WriteString(string.Format("Initial random seed value for all streams: {0}", this.ModelBackground.RandomNum.StreamSeed));
            SummaryOutput.WriteString(string.Format("Stream ID for generator used for this trial: {0}", this.ModelBackground.RandomNum.StreamID));
            SummaryOutput.WriteString(string.Format("Intended run length: {0} years", drRunInfo["RunLength"].ToString()));
            SummaryOutput.WriteString(string.Format("Cell datasource: {0}", CellDB));

            cFoxBackground rb = this.ModelBackground as cFoxBackground;
            if (rb != null)
            {
                SummaryOutput.WriteString("");
                SummaryOutput.WriteString("");
                SummaryOutput.WriteString("************ Animal Infomation ******************");
                if (Convert.ToBoolean(drRunInfo["StartWithSingleAnimal"]))
                {
                    SummaryOutput.WriteString(string.Format("Run started with initial pair of animals"));
                    if (drMarkers != null)
                    {
                        SummaryOutput.WriteString(string.Format("Initial male marker: {0}", drMarkers["Male"].ToString())); 
                        SummaryOutput.WriteString(string.Format("Initial female marker: {0}", drMarkers["Female"].ToString())); 
                    }
                    else
                    {
                        SummaryOutput.WriteString("Initial male marker: M"); 
                        SummaryOutput.WriteString("Initial female marker: M"); 
                    }
                }
                else
                {
                    SummaryOutput.WriteString(string.Format("Starting animal datasource: {0}", AnimalDB));
                    SummaryOutput.WriteString(string.Format("Starting year: {0}", this.ModelBackground.Years.CurrentYearNum));
                    SummaryOutput.WriteString(string.Format("Starting week: {0}", this.ModelBackground.Years.CurrentYear.CurrentWeek));
                }

                SummaryOutput.WriteString(string.Format("Independence age: {0} weeks", drAnimals["AgeOfIndependence"].ToString()));
                SummaryOutput.WriteString(string.Format("Adult age: {0} weeks", drAnimals["AdultAge"].ToString()));
                SummaryOutput.WriteString(string.Format("Mating week: {0}", drAnimals["MatingWeek"].ToString()));
                SummaryOutput.WriteString(string.Format("Reproduction week: {0}", drAnimals["ReproductionWeek"].ToString()));
                SummaryOutput.WriteString(string.Format("Odds of juvenile giving birth: {0}%", drAnimals["JuvenileBirthOdds"].ToString()));
                SummaryOutput.WriteString(string.Format("Odds of adult giving birth: {0}%", drAnimals["AdultBirthOdds"].ToString()));
                SummaryOutput.WriteString(string.Format("Average litter size: {0} weeks", drAnimals["LitterAverageSize"].ToString()));
                SummaryOutput.WriteString(string.Format("Litter size variance: {0} weeks", drAnimals["LitterSizeVariance"].ToString()));
                SummaryOutput.WriteString(string.Format("Gender ratio: {0}% male", drAnimals["GenderRatio"].ToString()));
                SummaryOutput.WriteString(string.Format("Prevent incest?: {0}", (rb.PreventIncest ? "Yes" : "No")));

                DataTable dtMort = mvarTrialSettings.Tables["AnimalInformationMortality"];
                SummaryOutput.WriteString("");
                SummaryOutput.WriteString("Yearly mortalities:");
                SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2}", "Age", "Male", "Female"));
                for (int i = 0; i < dtMort.Rows.Count; i++)
                {
                    SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2}", i, Convert.ToDouble(dtMort.Rows[i]["Male"]).ToString("0.00"), Convert.ToDouble(dtMort.Rows[i]["Female"]).ToString("0.00")));
                }
                SummaryOutput.WriteString(string.Format("Mortality adjustment: {0}", drAnimals["MortalityAdjustment"].ToString()));

                SummaryOutput.WriteString("");
                SummaryOutput.WriteString("Home ranges:");
                SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2}", "Week", "Male", "Female"));
                for (int i = 0; i < rb.FoxMaleHomeRange.Length; i++)
                {
                    SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2}", i, rb.FoxMaleHomeRange[i].ToString("0.00"), rb.FoxFemaleHomeRange[i].ToString("0.00")));
                }

                SummaryOutput.WriteString("");
                SummaryOutput.WriteString("Movement weeks:");
                SummaryOutput.WriteString(string.Format("{0,-18}{1,-18}{2,-18}{3}", "Y. of Y. Male", "Y. of Y. Female", "Juv./Adult Male", "Juv./Adult Female"));
                for (int i = 0; i < rb.FoxYoungMaleWeeklyMovement.Length; i++)
                {
                    SummaryOutput.WriteString(string.Format("{0,-18}{1,-18}{2,-18}{3}", (rb.FoxYoungMaleWeeklyMovement[i] ? "Yes" : "No"), (rb.FoxYoungFemaleWeeklyMovement[i] ? "Yes" : "No "), (rb.FoxJuvAdultMaleWeeklyMovement[i] ? "Yes" : "No "), (rb.FoxJuvAdultFemaleWeeklyMovement[i] ? "Yes" : "No ")));
                }
                SummaryOutput.WriteString("");
                SummaryOutput.WriteString("Movement distance probabilities:");
                SummaryOutput.WriteString(string.Format("{0,-13}{1,-18}{2,-18}{3,-18}{4}", "# of Cells", "Y. of Y. Male", "Y. of Y. Female", "Juv./Adult Male", "Juv./Adult Female"));
                for (int i = 0; i < rb.FoxYoungMaleMovementDistribution.Length; i++)
                {
                    SummaryOutput.WriteString(string.Format("{0,-13}{1,-18}{2,-18}{3,-18}{4}", i, string.Format("{0}%", rb.FoxYoungMaleMovementDistribution[i].ToString("0.00")), string.Format("{0}%", rb.FoxYoungFemaleMovementDistribution[i].ToString("0.00")), string.Format("{0}%", rb.FoxJuvAdultMaleMovementDistribution[i].ToString("0.00")), string.Format("{0}%", rb.FoxJuvAdultFemaleMovementDistribution[i].ToString("0.00"))));
                }

                SummaryOutput.WriteString("");
                SummaryOutput.WriteString("Winter cull");
                SummaryOutput.WriteString("Distribution of cull by age and gender:");
                SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2}", "Age", "Male", "Female"));
                for (int i = 0; i < 8; i++)
                {
                    SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2}", i, string.Format("{0}%", WinterCullByAgeMale[i].ToString("0.0")), string.Format("{0}%", WinterCullByAgeFemale[i].ToString("0.0"))));
                }
                SummaryOutput.WriteString("% culled by winter type:");
                SummaryOutput.WriteString(string.Format("{0,-12}{1,-12}{2,-12}{3,-12}{4}", "Very Severe", "Severe", "Normal", "Mild", "Very Mild"));
                SummaryOutput.WriteString(string.Format("{0,-12}{1,-12}{2,-12}{3,-12}{4}", string.Format("{0}%", this.CullWinterEffect[0].ToString("0.0")), string.Format("{0}%", this.CullWinterEffect[1].ToString("0.0")), string.Format("{0}%", this.CullWinterEffect[2].ToString("0.0")), string.Format("{0}%", this.CullWinterEffect[3].ToString("0.0")), string.Format("{0}%", this.CullWinterEffect[4].ToString("0.0"))));
                SummaryOutput.WriteString(string.Format("Winter cull week: {0}", this.WinterCullWeek));

                if (ModelBackground.Diseases.Count > 0)
                {
                    DataTable dtIncubation = mvarTrialSettings.Tables["DiseaseInformationIncubationPeriodNormalized"];
                    DataTable dtInfectious = mvarTrialSettings.Tables["DiseaseInformationInfectiousPeriodNormalized"];

                    SummaryOutput.WriteString("");
                    SummaryOutput.WriteString("");
                    SummaryOutput.WriteString("************ Disease ******************");
                    foreach (cDisease D in ModelBackground.Diseases)
                    {
                        SummaryOutput.WriteString(string.Format("Disease name: {0}", D.Name));
                        SummaryOutput.WriteString(string.Format("Spread probability: {0}", D.ContactRate.ToString("0.0")));
                        SummaryOutput.WriteString(string.Format("Reflective Spread: {0}", (ModelBackground.UseReflectiveDiseaseSpread ? "Yes" : "No")));
                        SummaryOutput.WriteString(string.Format("Chance of Death: {0}%", D.ChanceOfDeath));
                        SummaryOutput.WriteString(string.Format("Recovered animals become immune: {0}", D.BecomesImmune ? "Yes" : "No"));
                        SummaryOutput.WriteString(string.Format("Recovered animals do not become infectious: {0}", D.RecoveredNotInfectious ? "Yes" : "No"));
                        SummaryOutput.WriteString("");
                        SummaryOutput.WriteString("Incubation and infectious period probabilities:");
                        SummaryOutput.WriteString(string.Format("{0,-10}{1,-12}{2}", "Length", "Incubation", "Infectious"));
                        for (int i = 0; i < 50; i++)
                        {
                            DataRow dr = dtIncubation.Rows[i];
                            double Incubation = Convert.ToDouble(dr["Value"]);
                            dr = dtInfectious.Rows[i];
                            double Infection = Convert.ToDouble(dr["Value"]);
                            SummaryOutput.WriteString(string.Format("{0,-10}{1,-12}{2}", string.Format("{0} Weeks", i + 1), string.Format("{0}%", Incubation.ToString("0.00")), string.Format("{0}%", Infection.ToString("0.00"))));
                        }
                    }
                    if (this.InitialInfections.Count > 0)
                    {
                        SummaryOutput.WriteString("");
                        SummaryOutput.WriteString("Initial infections:");
                        SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2,-12}{3}", "Year", "Week", "% Infected", "Cell"));
                        for (int i = 0; i < this.InitialInfections.Count; i++)
                        {
                            cInitialInfection ii = this.InitialInfections[i];
                            SummaryOutput.WriteString(string.Format("{0,-10}{1,-10}{2,-12}{3}", ii.Year, ii.Week, string.Format("{0}%", ii.InfectionLevel), ii.InfectionCell));
                        }
                    }



                    SummaryOutput.WriteString("");
                    SummaryOutput.WriteString("");
                    SummaryOutput.WriteString("************ Strategies ******************");
                    SummaryOutput.WriteString(string.Format("Vaccine effective period: {0} weeks", drDisease["VaccineEffectivePeriod"].ToString()));
                    SummaryOutput.WriteString(string.Format("Fertility control effective period: {0} weeks", drFertility["EffectivePeriod"].ToString()));
                    if (ModelBackground.Strategies.Count > 0)
                    {
                        SummaryOutput.WriteString("");
                        SummaryOutput.WriteString("Defined strategies:");
                        for (int i = 0; i < ModelBackground.Strategies.Count; i++)
                        {
                            SummaryOutput.WriteString(ModelBackground.Strategies[i].ToString());
                        }
                    }
                }
                SummaryOutput.Close();
            }
        }

        /// <summary>
        /// Write a summary file just before the trial begins
        /// </summary>
        /// <param name="ActualOutputFolder">The folder into which the summary file is written</param>
        /// <param name="TrialOutCome">The outcome: "Completed", "Aborted", "Stopped by Error"</param>
        protected virtual void CompleteSummaryFile(string ActualOutputFolder, string TrialOutCome)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cModelTrial.cs: CompleteSummaryFile()");

            DateTime CompletionTime = DateTime.Now;
            string fName = "";
            if (mvarIsUnix)
            {
                fName = string.Format("{0}{1}/Summary{2}.txt", ActualOutputFolder, this.RunName, this.TrialNumber.ToString("0000"));
            }
            else
            {
                fName = string.Format(@"{0}{1}\Summary{2}.txt", ActualOutputFolder, this.RunName, this.TrialNumber.ToString("0000"));
            }
            // create the output file
            cFileOutput SummaryOutput = new cFileOutput(fName, false, true, mvarNoOutputExceptions, mvarAppReport);
            SummaryOutput.WriteString("");
            SummaryOutput.WriteString("");
            SummaryOutput.WriteString("**************** Trial Outcome ***************");
            SummaryOutput.WriteString(string.Format("Outcome: {0} at {1}", TrialOutCome, CompletionTime.ToString("yyyy-MMM-dd hh:mm:ss")));  
            SummaryOutput.WriteString(string.Format("Duration of trial: {0} seconds", (CompletionTime - this.startTime).TotalSeconds.ToString("0.00")));
            SummaryOutput.Close();
        }

        //private void ReadMortalityRate()
        //{
        //    this.ModelBackground.FemaleMortality = (this.ModelBackground as cFoxBackground).FoxFemaleMortality;
        //    this.ModelBackground.MaleMortality = (this.ModelBackground as cFoxBackground).FoxMaleMortality;
            
        //}
        
        #endregion

        #region Overridable Members

        /// <summary>
        /// Method called by the event handler that performs weekly update before the dead animals are removed.
        /// May be overriden by sub-classes
        /// </summary>
        /// <param name="e">The event arguments for the underlying event</param>
        protected virtual void WeeklyUpdateBeforeDead(cWeeklyUpdateEventArgs e)
        {
            // does nothing in the base class
        }

        /// <summary>
        /// Method called by the event handler that performs weekly update after the dead animals are removed.
        /// May be overriden by sub-classes
        /// </summary>
        /// <param name="e">The event arguments for the underlying event</param>
        /// <param name="NVaccinated">The number of animals currently vaccinated</param>
        protected virtual void WeeklyUpdateAfterDead(cWeeklyUpdateEventArgs e, int NVaccinated)
        {
            // does nothing in the base class
        }

        /// <summary>
        /// Method called before an intermediate datasource is created
        /// </summary>
        protected virtual void BeforeOutputIntermediateDatasource()
        {
            // does nothing in the base class
        }

        /// <summary>
        /// Method called after an intermediate datasource is created
        /// </summary>
        protected virtual void AfterOutputIntermediateDatasource()
        {
            // does nothing in the base class
        }


        #endregion
    }
}
