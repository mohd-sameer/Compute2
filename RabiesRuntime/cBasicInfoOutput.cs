using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Rabies_Model_Core;

namespace RabiesRuntime
{
    #region cBasicInfoData

    /// <summary>
    /// Class defining fields for basic info output
    /// </summary>
    public class cBasicInfoData
    {
        /// <summary>
        /// The current year
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// The current week
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// The total population
        /// </summary>
        public int TotPop { get; set; }
        /// <summary>
        /// The number of deaths
        /// </summary>
        public int NumDeaths { get; set; }
        /// <summary>
        /// The average animal age in weeks
        /// </summary>
        public double AverageAge { get; set; }
        /// <summary>
        /// The number of animals per cell
        /// </summary>
        public double AnimalsPerCell { get; set; }
        /// <summary>
        /// An array containing the number of animals by age
        /// </summary>
        public double[] Age
        { 
            get
            {
                return mvarAge;
            }
        }
        private double[] mvarAge = new double[10];
        /// <summary>
        /// The number of animals incubating a disease
        /// </summary>
        public int NIncubating { get; set; }
        /// <summary>
        /// The number of animals currently infectious
        /// </summary>
        public int NInfectious { get; set; }
        /// <summary>
        /// The number of cells
        /// </summary>
        public int NCells { get; set; }
        /// <summary>
        /// The trial number
        /// </summary>
        public int Trial { get; set; }
        ///<summary>
        /// //YM :
        /// The number of vaccinated animals
        ///</summary>
        public int NVaccinated { get; set; }
        ///<summary>
        /// //YM :
        /// The number of infected recoved animals
        ///</summary>
        public int NInfectedRecovered { get; set; }

        //YM: the number of young of year (babies)
        public int AgeYoungofYear { get; set; }
        public int AgeAdult { get; set; }
        public int AgeJuv { get; set; }
    }

    #endregion

    #region cBasicInfoOutput

    /// <summary>
    /// Class for output of basic model run information
    /// </summary>
    public class cBasicInfoOutput : cFileOutput
    {
        #region Constructors

        // *************************************** Constructors *********************************************

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName">The name of the output file</param>
        public cBasicInfoOutput(string FileName, bool TabDelimited, bool NoOutputExceptions, cAppReport AppReport)
            : base(FileName, TabDelimited, NoOutputExceptions, AppReport)
        {
        }
		

        /// <summary>
        /// Constructor
        /// </summary>
        public cBasicInfoOutput()
            : base()
        {
        }

        #endregion

        #region Public Methods

        // *************************************** Public Methods *******************************************
        // write a single value to the file
        public void WriteValue(cBasicInfoData Value)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cBasicInfoData.cs: WriteValue()");

            StringBuilder s = new StringBuilder();
            // get data
            if (this.TabDelimited)
            {
                s.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t", Value.Trial.ToString("0000"), Value.Year, Value.Week, Value.TotPop, Value.NumDeaths, Value.AverageAge.ToString("0.00"), Value.NCells, Value.AnimalsPerCell.ToString("0.00"));
            }
            else
            {
                s.AppendFormat("{0,-6}{1,-4}{2,-4}{3,-8}{4,-6}{5,-6}{6,-7}{7,-6}", Value.Trial.ToString("0000"), Value.Year, Value.Week, Value.TotPop, Value.NumDeaths, Value.AverageAge.ToString("0.00"), Value.NCells, Value.AnimalsPerCell.ToString("0.00"));
            }
            // get age counts
            for (int i = 0; i <= 8; i++)
            {
                if (this.TabDelimited)
                {
                    s.AppendFormat("{0}\t", Value.Age[i].ToString("0.00"));
                }
                else
                {
                    s.AppendFormat("{0,-6}", Value.Age[i].ToString("0.00"));
                }
            }
            // disease
            if (this.TabDelimited)
            {
                //YM: Add the number of vaccinated animals
                //s.AppendFormat("{0}\t{1}", Value.NIncubating, Value.NInfectious);
                s.AppendFormat("{0}\t{1}\t{2}\t{3}", Value.NIncubating, Value.NInfectious, Value.NVaccinated, Value.NInfectedRecovered);
            }
            else
            {
                //YM: Add the number of vaccinated animals
                // s.AppendFormat("{0,-6}{1,-6}", Value.NIncubating, Value.NInfectious);
                s.AppendFormat("{0,-6}{1,-6}{2,-6}{3,-6}", Value.NIncubating, Value.NInfectious, Value.NVaccinated, Value.NInfectedRecovered);
            }
            // write the line
            this.WriteString(s.ToString());            
        }

        #endregion

        #region Protected and Private Members

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
                    // write header
                    //YM:
                    //this.WriteString("Trial\tYear\tWeek\tTot. Pop\t# Deaths\tAverage Age\t# Cells Occupied\tAnimals/Cell\tAge 0\tAge 1\tAge 2\tAge 3\tAge 4\tAge 5\tAge 6\tAge 7\tAge Other\tIncubating Animals\tInfectious Animals");
                    this.WriteString("Trial\tYear\tWeek\tTot. Pop\t# Deaths\tAverage Age\t# Cells Occupied\tAnimals/Cell\tAge 0\tAge 1\tAge 2\tAge 3\tAge 4\tAge 5\tAge 6\tAge 7\tAge Other\tIncubating Animals\tInfectious Animals\tVaccinated Animals\tInfectedRecovered Animals");
                }
                else
                {
                    // write header
                    //YM
                    //this.WriteString(string.Format("{0,-6}{1,-4}{2,-4}{3,-8}{4,-6}{5,-6}{6,-7}{7,-6}{8,-6}{9,-6}{10,-6}{11,-6}{12,-6}{13,-6}{14,-6}{15,-6}{16,-6}{17, -6}{18,-6}", 
                    //                               "Trial", "Yr", "Wk", "Pop", "NDth", "AAge", "CellsO", "ACell", "Age0", "Age1", "Age2", "Age3", "Age4", "Age5", "Age6", "Age7", "AgeO", "Inc", "Inf"));
                    this.WriteString(string.Format("{0,-6}{1,-4}{2,-4}{3,-8}{4,-6}{5,-6}{6,-7}{7,-6}{8,-6}{9,-6}{10,-6}{11,-6}{12,-6}{13,-6}{14,-6}{15,-6}{16,-6}{17, -6}{18,-6}",
                                                   "Trial", "Yr", "Wk", "Pop", "NDth", "AAge", "CellsO", "ACell", "Age0", "Age1", "Age2", "Age3", "Age4", "Age5", "Age6", "Age7", "AgeO", "Inc", "Inf", "Vacc", "InfRecov"));
                }
            }

            #endregion
        }

    #endregion

    }
}
