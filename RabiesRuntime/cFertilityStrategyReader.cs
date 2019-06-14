using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Rabies_Model_Core;

namespace RabiesRuntime
{
    /// <summary>
    /// Class for reading fertility strategies
    /// </summary>
    public class cFertilityStrategyReader : cStrategyReader
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TrialSettings">The dataset containing the trial settings to read</param>
        public cFertilityStrategyReader(DataSet TrialSettings) : base("Fertility Control", TrialSettings)
        {
        }
        
        #endregion

        #region Protected Methods
    
        /// <summary>
        /// Create a new stategy based on passed settings
        /// </summary>
        /// <param name="ModelBackground">The background</param>
        /// <param name="StrategyCells">The cells affected by the strategy</param>
        /// <param name="ActualLevel">The level of the strategy</param>
        /// <param name="Year">The year the strategy is applied</param>
        /// <param name="Week">The week the strategy is applied</param>
        /// <returns>A newly created cull strategy</returns>
        protected override cStrategy CreateStrategy(cBackground ModelBackground, cCellList StrategyCells, int ActualLevel,
                                                    int Year, int Week)
        {
            // extract the current fertility control effective period from the dataset
            int EffectivePeriod;
            try
            {
                if (mvarTrialSettings.Tables.Contains("FertilityControlInformation"))
                {
                    //YM:
                    //DataRow drFertility = mvarTrialSettings.Tables["FertilityControlInformation"].Rows[0];
                    //EffectivePeriod = Convert.ToInt32(drFertility["EffectivePeriod"]);
                    DataRow drFertility = mvarTrialSettings.Tables[2].Rows[1];
                    EffectivePeriod = Convert.ToInt32(drFertility[1]);
                }
                else
                {
                    // not found - must be an older dataset
                    EffectivePeriod = 52;
                }

            }
            catch
            {
                // not found - must be an older dataset
                EffectivePeriod = 52;
            }

            // return a cull strategy
            return new cFertilityStrategy(ModelBackground, StrategyCells, ActualLevel, Year, Week, EffectivePeriod);
        }

        #endregion
    }
}
