using System;
using System.Collections.Generic;
using System.Text;
using Rabies_Model_Core;
using System.Data;

namespace RabiesRuntime
{
    /// <summary>
    /// Class for reading cull strategies
    /// </summary>
    public class cCullStrategyReader : cStrategyReader
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TrialSettings">The dataset containing the trial settings to read</param>
        public cCullStrategyReader(DataSet TrialSettings) : base("Cull", TrialSettings)
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
            // return a cull strategy
            return new cCullStrategy(ModelBackground, StrategyCells, ActualLevel, Year, Week);
        }
        #endregion
    }
}
