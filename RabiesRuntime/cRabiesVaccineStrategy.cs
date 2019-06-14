using System;
using System.Collections.Generic;
using System.Text;
using Rabies_Model_Core;

namespace RabiesRuntime
{
    /// <summary>
    /// Class representing a rabies vaccine strategy
    /// </summary>
    public class cRabiesVaccineStrategy : cVaccineStrategy
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="BG">The background</param>
        /// <param name="Cells">The cells affected by the strategy</param>
        /// <param name="Level">The level of the strategy</param>
        /// <param name="Year">The year the strategy is applied</param>
        /// <param name="Week">The week the strategy is applied</param>
        /// <param name="EffectivePeriod">The effecive period of the vaccine</param>
        public cRabiesVaccineStrategy(cBackground BG, cCellList Cells, int Level, int Year, int Week, int EffectivePeriod)
            :  base(BG, Cells, Level, Year, Week, "Rabies", EffectivePeriod)
        {
        }
    }
}
