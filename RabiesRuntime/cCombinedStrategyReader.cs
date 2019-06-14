using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Rabies_Model_Core;

namespace RabiesRuntime
{

    /// <summary>
    /// Class for reading a combined strategy
    /// </summary>
    public class cCombinedStrategyReader : cStrategyReader
    {
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TrialSettings">The settings dataset to read strategy settings from</param>
        public cCombinedStrategyReader(DataSet TrialSettings) : base("Combined", TrialSettings)
        {
        }
        
        #endregion
        
        #region Protected Methods

        /// <summary>
        /// Create the strategy being read
        /// </summary>
        /// <param name="ModelBackground">The background</param>
        /// <param name="StrategyCells">The cells the strategy applies to</param>
        /// <param name="ActualLevel">The level for the strategy</param>
        /// <param name="Year">The year of the strategy</param>
        /// <param name="Week">The week of the strategy</param>
        /// <returns>A newly created combned stategy</returns>
        protected override cStrategy CreateStrategy(cBackground ModelBackground, cCellList StrategyCells, int ActualLevel, int Year, int Week)
        {
            // extract the current fertility control effective period from the dataset
            int VaccineEffectivePeriod = 0;
            int FertilityEffectivePeriod = 0;
            // extract the current vaccine effective period from the dataset
            var drDisease = mvarTrialSettings.Tables["DiseaseInformation"].Rows[0];
            VaccineEffectivePeriod = Convert.ToInt32(drDisease["VaccineEffectivePeriod"]);
            try {
                var drFertility = mvarTrialSettings.Tables["FertilityControlInformation"].Rows[0];
                FertilityEffectivePeriod = Convert.ToInt32(drFertility["EffectivePeriod"]);
            }
            catch {
                // not found - must be an older dataset
                FertilityEffectivePeriod = 52;
            }
            // return a fertility strategy
            return new cRabiesCombinedStrategy(ModelBackground, StrategyCells, ActualLevel, Year, Week, VaccineEffectivePeriod, FertilityEffectivePeriod);
        }

        #endregion
    
    }

}
