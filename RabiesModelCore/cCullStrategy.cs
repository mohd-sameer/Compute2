using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A strategy that culls animals in a set of cells.  In each affected cell,
	///		the Level property detirmines the percent chance that each animal in the cell
	///		will be culled.
	/// </summary>
	public class cCullStrategy : cStrategy
	{
		/// <summary>
		///		Initialize a cCull strategy.
		/// </summary>
		/// <param name="BG">
		///		The background object against which this strategy shall be applied. An 
		///		ArgumentNullException exception is raised if BG is null.
		///	</param>
		/// <param name="Cells">
		///		The list of cells to which this strategy will apply.  An ArgumentNullException
		///		exception is raised if Cells is null.  An ArgumentException exception is raised
		///		if Cells is an empty list or if it contains cell IDs not found in the passed 
		///		Background object (BG).
		///	</param>
		/// <param name="Level">
		///		The level at with this strategy shall be applied.  An ArgumentOutOfRangeException
		///		exception is raised if Level is not in the range of 0-100.
		///	</param>
		/// <param name="Year">
		///		The year in which this strategy will be applied.  An ArgumentOutOfRangeException
		///		exception is raised if Year is less then zero.
		///	</param>
		/// <param name="Week">
		///		The week in which this strategy will be applied.  An ArgumentOutOfRangeException
		///		exception is raised if Week is not in the range of 1-52.
		///	</param>
		public cCullStrategy(cBackground BG, cCellList Cells, double Level, int Year, int Week) 
			: base(BG, Cells, Level, Year, Week) {}

		/// <summary>
		///		Apply this cull strategy.
		/// </summary>
		public override void ApplyStrategy() 
		{
			// loop through all cells in the background cell list
			foreach (cCell Cell in mvarBackground.Cells) {
				// is this cell in the strategy list?
				if (this.Contains(Cell.ID)) {
					int AppliedLevel = Convert.ToInt32(Values[Cell.ID]);
					// loop through all animals in the cell, culling if appropriate
					for (int i = Cell.Animals.Count - 1; i >= 0; i--) {
						// set range of Random numbers
						//mvarBackground.RandomNum.MinValue = 0;
						//mvarBackground.RandomNum.MaxValue = 100;
						if (mvarBackground.RandomNum.IntValue(1, 100) <= AppliedLevel) {
							// cull the animal
							Cell.Animals[i].Die();
						}
					}
				}
			}
		}

        // get the name for the type of strategy
        protected override string GetStrategyTypeName()
        {
            return "Cull Strategy";
        }

	}
}