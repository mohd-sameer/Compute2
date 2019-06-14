using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Rabies_Model_Core;

namespace RabiesRuntime
{
    #region cRabiesReportData

    /// <summary>
    /// Class representing the data for a rabies report 
    /// </summary>
    public class cRabiesReportData
    {
        /// <summary>
        /// Get or set the trial number
        /// </summary>
        public int Trial { get; set; }
        /// <summary>
        /// Get or set the ID of the animal
        /// </summary>
        public int AnimalID { get; set; }
        /// <summary>
        /// Get or set the ID of the cell
        /// </summary>
        public string CellID { get; set; }
        /// <summary>
        /// Get or st the year
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Get or set the week
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// Get or set the Incubation period of the diesease
        /// </summary>
        public int IncubationPeriod { get; set; }
        /// <summary>
        /// Get or set the Infectious period of the disease
        /// </summary>
        public int InfectiousPeriod { get; set; }
        /// <summary>
        /// Get or set a boolean value that indicates that the case of rabies was fatal
        /// </summary>
        public bool WasFatal { get; set; }
        /// <summary>
        /// Get or set the ID of the parent of the animal
        /// </summary>
        public int Parent_ID { get; set; }
        /// <summary>
        /// Get or set the ID if the animal that infected this animal
        /// </summary>
        public int Infecting_ID { get; set; }
        /// <summary>
        /// Get or set the age of the animal
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// Get or set the gender of the animal
        /// </summary>
        public enumGender Gender { get; set; }
        /// <summary>
        /// Get or set a string describing the movement history of the animal
        /// </summary>
        public string MovementHistory 
        {
            get
            {
                return (_movementHistory == null ? "" : _movementHistory);
            }
            set
            {
                _movementHistory = value;
            }
        }
        private string _movementHistory;
    }

    #endregion

    #region cRabiesReportOutput

    /// <summary>
    /// Class for creating a rabies report
    /// </summary>
    public class cRabiesReportOutput : cFileOutput
    {
        #region Constructor

        // *************************************** Constructors *********************************************
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName">The filename for the report</param>
        public cRabiesReportOutput(string FileName, bool TabDelimited,  bool NoOutputExceptions, cAppReport AppReport)
            : base(FileName, TabDelimited, NoOutputExceptions, AppReport)
        {
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public cRabiesReportOutput() : base()
        {
        }

        #endregion

        #region Public Methods

        // *************************************** Public Methods *******************************************

        /// <summary>
        /// Write a single value to the file
        /// </summary>
        /// <param name="Value">The value to write</param>
        public void WriteValue(cRabiesReportData Value)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cRabiesReportOutput.cs: WriteValue()");

            if (this.TabDelimited)
            {
                this.WriteString(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}", Value.Trial.ToString("0000"), Value.AnimalID,
                                               Value.CellID, Value.Year, Value.Week, Value.IncubationPeriod, Value.InfectiousPeriod, Value.Parent_ID, Value.Infecting_ID,
                                               Value.Age, Value.Gender.ToString(), Value.MovementHistory, Value.WasFatal ? "Yes" : "No"));
            }
            else
            {
                this.WriteString(string.Format("{0,-6}{1,-10}{2,-10}{3,-4}{4,-4}{5,-5}{6,-5}{7,-10}{8,-10}{9,-4}{10,-4}{11,-50}{12,-6}", Value.Trial.ToString("0000"), Value.AnimalID,
                                               Value.CellID, Value.Year, Value.Week, Value.IncubationPeriod, Value.InfectiousPeriod, Value.Parent_ID, Value.Infecting_ID,
                                               Value.Age, (Value.Gender == enumGender.male ? "m" : "f"), (Value.MovementHistory.Length > 50 ? string.Format("{0}...", Value.MovementHistory.Substring(0,47)) : Value.MovementHistory),
                                               Value.WasFatal ? "Yes" : "No"));
            }
        }

        #endregion

        #region Protected Methods

        // ***************************************** Protected Methods **************************************

        /// <summary>
        /// Open the file
        /// </summary>
        /// <param name="Append">If true, content is appended to the file if it exists</param>
        protected override void OpenFile(bool Append)
        {
            base.OpenFile(Append);
            DoHeader();
        }

        /// <summary>
        /// Open the file
        /// </summary>
        protected override void OpenFile()
        {
            // open the file as normal
            base.OpenFile();
            DoHeader();
        }

        protected void DoHeader()
        {
            // write the header if the file opened properly
            if (this.IsOpen)
            {
                if (this.TabDelimited)
                {
                    this.WriteString("Trial\tID\tCellID\tYear\tWeek\tIncubation_Period\tInfectious_Period\tParent_ID\tInfectingAnimal_ID\tAge\tGender\tMovement_History\tWas Fatal");
                }
                else
                {
                    this.WriteString(string.Format("{0,-6}{1,-10}{2,-10}{3,-4}{4,-4}{5,-5}{6,-5}{7,-10}{8,-10}{9,-4}{10,-4}{11,-50}{12,-6}",
                                                   "Trial", "ID", "CellID", "Yr", "Wk", "IncP", "InfP", "P_ID", "IA_ID", "Age", "Sex", "Movement", "Fatal"));
                }
            }
        }
        #endregion
    }

    #endregion
}
