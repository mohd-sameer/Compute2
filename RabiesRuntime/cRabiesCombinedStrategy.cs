using System;
using System.Collections.Generic;
using System.Text;
using Rabies_Model_Core;

namespace RabiesRuntime
{
    /// <summary>
    /// A rabies specific combined stratgey
    /// </summary>
    public class cRabiesCombinedStrategy : cCombinedStrategy
    {
        /// <summary>
        /// Constructor.  Create a strategy for applying a combined vaccination and fertility control strategy
        /// </summary>
        /// <param name="BG">The background</param>
        /// <param name="Cells">The list of cells affected by the strategy</param>
        /// <param name="Level">The level of the strategy</param>
        /// <param name="Year">The year the strategy is applied</param>
        /// <param name="Week">The week the strategy is applied</param>
        /// <param name="VaccineEffectivePeriod">The effective period of the vaccine</param>
        /// <param name="FertilityEffectivePeriod">The effective period of the fertility control</param>
        public cRabiesCombinedStrategy(cBackground BG, cCellList Cells, int Level, int Year, int Week, 
                                       int VaccineEffectivePeriod, int FertilityEffectivePeriod)
            : base(BG, Cells, Level, Year, Week, "Rabies", VaccineEffectivePeriod, FertilityEffectivePeriod)
        {
        }
    }
}
