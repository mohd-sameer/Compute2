using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using Rabies_Model_Core;
using IndexedHashTable;

namespace RabiesRuntime
{
    /// <summary>
    /// Class for outputting cell populations
    /// </summary>
    public class cCellPopulationOutput : cFileOutput
    {
        #region Constructors

        // *************************************** Constructors *********************************************
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Cells">The list of cells to output</param>
        /// <param name="FileName">The name of the file to output</param>
        public cCellPopulationOutput(cCellList Cells, string FileName, bool TabDelimited, bool NoOutputExceptions, cAppReport AppReport)
            : base(FileName, TabDelimited, NoOutputExceptions, AppReport)
        {
            mvarCells = Cells;            
            mvarOutputStrings = new Dictionary<string, StringBuilder>();            
            WriteHeader();            
        }

        #endregion

        #region Public Methods

        // *************************************** Public Methods *******************************************
        /// <summary>
        /// Write a single value to the file
        /// </summary>
        /// <param name="Year">The current year</param>
        /// <param name="Week">The current week</param>
        public void WriteValue(int Year, int Week)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cCellPopulationOutput.cs: WriteValue()");
            //System.Diagnostics.Debug.WriteLine("cCellPopulationOutput.cs: WriteValue(): Year = " + Year + "; Week = " + Week);

            if (this.TabDelimited)
            {
                // write year - week header
                mvarOutputStrings["Title"].AppendFormat("Y:{0} W:{1}\t", Year.ToString("000"), Week.ToString("00"));
                // now write data for each cell
                foreach (cCell c in mvarCells)
                {
                    mvarOutputStrings[c.ID].AppendFormat("{0}\t", c.Animals.Count);
                }
            }
            else
            {
                // write year - week header
                mvarOutputStrings["Title"].AppendFormat("Y:{0} W:{1} ", Year.ToString("000"), Week.ToString("00"));
                // now write data for each cell
                foreach (cCell c in mvarCells)
                {
                    mvarOutputStrings[c.ID].AppendFormat("{0,-11}", c.Animals.Count);
                }

            }
        }

        /// <summary>
        /// Close the file
        /// </summary>
        public override void Close()
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cCellPopulationOutput.cs: Close()");

            // actually write the data to the file
            foreach (StringBuilder sb in mvarOutputStrings.Values)
            {
                this.WriteString(sb.ToString());
            }
            // close the file
            base.Close();
        }

        #endregion

        #region Protected and Private Members

        // ***************************************** Protected Methods **************************************
        // write the header info in the file (i.e. Cell IDs)
        protected void WriteHeader()
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cCellPopulationOutput.cs: WriteHeader()");

            // add a title string
            if (this.TabDelimited)
            {
                mvarOutputStrings.Add("Title", new StringBuilder("Year ->\t"));
                // now add a string for each cell
                foreach (cCell c in mvarCells)
                {
                    mvarOutputStrings.Add(c.ID, new StringBuilder(string.Format("{0}\t", c.ID)));
                }
            }
            else
            {
                mvarOutputStrings.Add("Title", new StringBuilder(string.Format("{0,-11}", "Year ->")));
                // now add a string for each cell
                foreach (cCell c in mvarCells)
                {
                    mvarOutputStrings.Add(c.ID, new StringBuilder(string.Format("{0,-11}", c.ID)));
                }
            }
        }

        // ************************************** Private Methods *******************************************
        // a reference to the cells being output
        private cCellList mvarCells;
        private Dictionary<string, StringBuilder> mvarOutputStrings;

        #endregion
    }
}
