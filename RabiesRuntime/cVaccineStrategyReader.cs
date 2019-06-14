using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Rabies_Model_Core;

namespace RabiesRuntime
{
    /// <summary>
    /// Class for reading vaccination strategies
    /// </summary>
    public class cVaccineStrategyReader : cStrategyReader
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TrialSettings">The dataset containing the trial settings to read</param>
        public cVaccineStrategyReader(DataSet TrialSettings) : base("Vaccination", TrialSettings)
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
            // extract the current vaccine effective period from the dataset
            //YM:
            //DataRow drDisease = mvarTrialSettings.Tables["DiseaseInformation"].Rows[0];
            //int EffectivePeriod = Convert.ToInt32(drDisease["VaccineEffectivePeriod"]);
            DataRow drDisease = mvarTrialSettings.Tables[2].Rows[0];
            int EffectivePeriod = Convert.ToInt32(drDisease[1]);
            // return a vaccine strategy
            return new cRabiesVaccineStrategy(ModelBackground, StrategyCells, ActualLevel, Year, Week, EffectivePeriod);
        }

        #endregion
    }
}
