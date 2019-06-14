using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Rabies_Model_Core;
using System.IO;
using System.Linq;

namespace RabiesRuntime
{
    /// <summary>
    /// Class to read a strategy from an XML settings file
    ///  /// YM : and from Excel Template 
    /// </summary>
    public abstract class cStrategyReader
    {
        #region Protected Variables

        protected DataSet mvarTrialSettings;
        protected string mvarStrategyName;
        
        #endregion

        #region "Constructor"

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StrategyName">The name of the strategy</param>
        /// <param name="TrialSettings">The trial settings dataset from which to read the strategy</param>
        public cStrategyReader(string StrategyName, DataSet TrialSettings)
        {
            // replace spaces with underscore in the strategy name
            mvarStrategyName = StrategyName.Replace(" ", "_");
            mvarTrialSettings = TrialSettings;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read vaccination strategies from the Excel template settings file
        /// </summary>
        /// <param name="ModelBackground"></param>
        public void GetStrategiesExcelTemplate(cBackground ModelBackground)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cStrategyReader.cs: GetStrategiesExcelTemplate() ...does nothing");

            int i = 0;
            int j = 0;
            int Level = 0;
            int ActualLevel = 0;
            int Year = 0;
            int Week = 0;
            
            // get required data tables and datarow
            /*DataTable dtStrategyTimes = mvarTrialSettings.Tables[mvarStrategyName + "_StrategyTimes"];
            DataTable dtStrategies = mvarTrialSettings.Tables[mvarStrategyName + "_Strategies"];
            DataTable dtCellIDs = mvarTrialSettings.Tables[mvarStrategyName + "_CellIDs"];
            List<string> StrategyCellIDs = new List<string>();
            string StrategyFieldName = null;
            DataRow dr = null;*/
            // get the strategy cell ids - these are stored in the same order as the strategy values
            
            // EER: check each cell for data
            /*foreach (DataRow drCell in dtCellIDs.Rows)
            {                
                StrategyCellIDs.Add(drCell["CellID"].ToString());
            }*/
            
            // loop through the number of defined strategies (the count of records in the StrategyTimes table)
            /*int numbdefs = dtStrategyTimes.Rows.Count;
            System.Diagnostics.Debug.WriteLine("    number of defined strategies = " + numbdefs);
            for (i = 0; i <= dtStrategyTimes.Rows.Count - 1; i++)
            {
                System.Diagnostics.Debug.WriteLine(" here01");
                // get the year and week for this strategy
                dr = dtStrategyTimes.Rows[i];
                Year = Convert.ToInt32(dr["Year"]);
                Week = Convert.ToInt32(dr["Week"]);
                // field name in Strategies table for this strategy
                StrategyFieldName = "S" + i;
                // create a list of cells affected by this strategy
                cCellList StrategyCells = new cCellList(null);
                ActualLevel = 0;
                for (j = 0; j <= StrategyCellIDs.Count - 1; j++)
                {
                    System.Diagnostics.Debug.WriteLine(" here02");
                    dr = dtStrategies.Rows[j];
                    // get the level for each cell
                    Level = Convert.ToInt32(dr[StrategyFieldName]);
                    if (Level > ActualLevel) ActualLevel = Level;
                    // add a cell to the list if the level value for that cell is greater than 0
                    if (Level > 0) StrategyCells.Add(ModelBackground.Cells[StrategyCellIDs[j]]);
                }
                // now create a strategy object
                cStrategy NewStrategy = CreateStrategy(ModelBackground, StrategyCells, ActualLevel, Year, Week);
                // add this strategy to the list of strategies
                ModelBackground.Strategies.Add(NewStrategy);
            }*/
        }

        /// <summary>
        /// YM:
        /// Read control strategies from the CSV settings file
        /// </summary>
        /// <param name="ModelBackground"></param>The background
        /// <param name="StrategiesCtrlPath"></param> the path to the strategies control file
        public void GetStrategiesCsvTemplate(string StrategiesCtrlPath, cBackground ModelBackground)
        {
            int i = 0;
            int j = 0;
            int Level = 0;
            int ActualLevel = 0;
            int Year = 0;
            int Week = 0;
            char Delimiter = ';';
            // Get csv data 
            DataTable dtStrategiesCtrl = new DataTable();
            DataRow Rowdt;
            using (StreamReader StrategiesCtrlData = new StreamReader(StrategiesCtrlPath))
            {
                string StrategiesCtrlLine = StrategiesCtrlData.ReadLine();
                string[] Header = StrategiesCtrlLine.Split(Delimiter);
                foreach (string dc in Header)
                {
                    dtStrategiesCtrl.Columns.Add(new DataColumn(dc));
                }

                while (!StrategiesCtrlData.EndOfStream)
                {
                    StrategiesCtrlLine = StrategiesCtrlData.ReadLine();
                    string[] Rows = StrategiesCtrlLine.Split(Delimiter);
                    if (Rows.Length == dtStrategiesCtrl.Columns.Count)
                    {
                        Rowdt = dtStrategiesCtrl.NewRow();
                        Rowdt.ItemArray = Rows;
                        dtStrategiesCtrl.Rows.Add(Rowdt);
                    }
                }
                StrategiesCtrlData.Close();
            }

            // Use the RowFilter method to find all rows matching the strategy type.
            dtStrategiesCtrl.DefaultView.RowFilter = "Type='" + mvarStrategyName + "'";
            DataTable dtStrategiesCtrlFilterType = (dtStrategiesCtrl.DefaultView).ToTable();
            dtStrategiesCtrl.Clear();

            List<string> StrategiesNameList = new List<string>();
            foreach (DataRow row in dtStrategiesCtrlFilterType.Rows)
            {
                StrategiesNameList.Add(row["StrategyName"].ToString());
            }
            int StrategiesNameCount = StrategiesNameList.Distinct().Count();
            IEnumerable<string> StrategiesNameListDistinct = StrategiesNameList.Distinct();
            
            // loop through the number of defined strategies
            foreach (string StrategyNameStr in StrategiesNameListDistinct)
            {
                // filter strategies by name
                dtStrategiesCtrlFilterType.DefaultView.RowFilter = "StrategyName='" + StrategyNameStr + "'";
                DataTable dtStrategiesCtrlFilterStrgName = (dtStrategiesCtrlFilterType.DefaultView).ToTable();

                // get required data tables and datarow
                List<string> StrategyCellIDs = new List<string>();
                DataRow dr;
                // get the strategy cell ids
                for (i = 0; i <= dtStrategiesCtrlFilterStrgName.Rows.Count - 1; i++)
                {
                    StrategyCellIDs.Add(dtStrategiesCtrlFilterStrgName.Rows[i]["HexCellID"].ToString());
                }

                // create a list of cells affected by this strategy
                cCellList StrategyCells = new cCellList(null);
                StrategyCells.Name = "StrategiesCells";
                cStrategy NewStrategy = null;
                for (j = 0; j <= StrategyCellIDs.Count - 1; j++)
                {
                    //System.Diagnostics.Debug.WriteLine(" here02");
                    dr = dtStrategiesCtrlFilterStrgName.Rows[j];
                    //get the year and the week for this strategy 
                    Year = Convert.ToInt32(dr["Year"]);
                    Week = Convert.ToInt32(dr["Week"]);
                    // get the level for each cell
                    Level = Convert.ToInt32(dr["Level"]);
                    if (Level > ActualLevel) ActualLevel = Level;
                    // add a cell to the list if the level value for that cell is greater than 0
                    if (Level > 0)
                    {
                        ActualLevel = Level;
                        StrategyCells.Add(ModelBackground.Cells[StrategyCellIDs[j]]);
                    }
                }
                // now create a strategy object
                NewStrategy = CreateStrategy(ModelBackground, StrategyCells, ActualLevel, Year, Week);
                // add this strategy to the list of strategies
                ModelBackground.Strategies.Add(NewStrategy);
            }
            dtStrategiesCtrlFilterType.Clear();
        }

        /// <summary>
        /// Read vaccination strategies from the Trial Settings dataset
        /// </summary>
        /// <param name="ModelBackground"></param>
        public void GetStrategies(cBackground ModelBackground)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("Runs with no strategies???? cStrategyReader.cs: GetStrategies()");

            int i = 0;
            int j = 0;
            int Level = 0;
            int ActualLevel = 0;
            int Year = 0;
            int Week = 0;
            // get required data tables and datarow
            DataTable dtStrategyTimes = mvarTrialSettings.Tables[mvarStrategyName + "_StrategyTimes"];
            DataTable dtStrategies = mvarTrialSettings.Tables[mvarStrategyName + "_Strategies"];
            DataTable dtCellIDs = mvarTrialSettings.Tables[mvarStrategyName + "_CellIDs"];
            List<string> StrategyCellIDs = new List<string>();
            string StrategyFieldName = null;
            DataRow dr = null;
            // get the strategy cell ids - these are stored in the same order as the strategy values
            foreach (DataRow drCell in dtCellIDs.Rows)
            {
                //System.Diagnostics.Debug.WriteLine(" No data in drCell");
                StrategyCellIDs.Add(drCell["CellID"].ToString());
            }
            // loop through the number of defined strategies (the count of records in the StrategyTimes table)
            int numbdefs = dtStrategyTimes.Rows.Count;
            //System.Diagnostics.Debug.WriteLine("    number of defined strategies = " + numbdefs);
            for (i = 0; i <= dtStrategyTimes.Rows.Count - 1; i++)
            {
                //System.Diagnostics.Debug.WriteLine(" here01");
                // get the year and week for this strategy
                dr = dtStrategyTimes.Rows[i];
                Year = Convert.ToInt32(dr["Year"]);
                Week = Convert.ToInt32(dr["Week"]);
                // field name in Strategies table for this strategy
                StrategyFieldName = "S" + i;
                // create a list of cells affected by this strategy
                cCellList StrategyCells = new cCellList(null);
                ActualLevel = 0;
                for (j = 0; j <= StrategyCellIDs.Count - 1; j++)
                {
                    //System.Diagnostics.Debug.WriteLine(" here02");
                    dr = dtStrategies.Rows[j];
                    // get the level for each cell
                    Level = Convert.ToInt32(dr[StrategyFieldName]);
                    if (Level > ActualLevel) ActualLevel = Level;
                    // add a cell to the list if the level value for that cell is greater than 0
                    if (Level > 0) StrategyCells.Add(ModelBackground.Cells[StrategyCellIDs[j]]);
                }
                // now create a strategy object
                cStrategy NewStrategy = CreateStrategy(ModelBackground, StrategyCells, ActualLevel, Year, Week);
                // add this strategy to the list of strategies
                ModelBackground.Strategies.Add(NewStrategy);
            }
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Abstract methos for creating an actual strategy.  Overridden by sub-classes to create specific strategy types
        /// </summary>
        /// <param name="ModelBackground">The background</param>
        /// <param name="StrategyCells">The cells the strategy applies to</param>
        /// <param name="ActualLevel">The level for the strategy</param>
        /// <param name="Year">The year of the strategy</param>
        /// <param name="Week">The week of the strategy</param>
        /// <returns>A newly created combned stategy</returns>
        protected abstract cStrategy CreateStrategy(cBackground ModelBackground, cCellList StrategyCells, int ActualLevel, int Year, int Week);

        #endregion

    }
}
