using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A strategy that vaccinates animals in a set of cells.  In each affected cell,
	///		the Level property detirmines the percent chance that each animal in the cell
	///		will be vaccinated.  The disease the vaccine protects against and the vaccine's
	///		effective period are specified in the constructor for the vaccine strategy.
	/// </summary>
	public class cVaccineStrategy : cStrategy
	{
		/// <summary>
		///		Initialize a cVaccine strategy.
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
		/// <param name="DiseaseName">
		///		The name of the disease to protect against.  An ArgumentException exception is
		///		raised if DiseaseName is zero length.
		///	</param>
		/// <param name="EffectivePeriod">
		///		The effective period (in weeks) of the vaccine being used.  An 
		///		ArgumentOutOfRangeException exception is raise if EffectivePeriod is less than
		///		zero.
		///	</param>
		public cVaccineStrategy(cBackground BG, cCellList Cells, double Level, 
								int Year, int Week, string DiseaseName, int EffectivePeriod) 
			: base(BG, Cells, Level, Year, Week)
		{
			// DiseaseName should not be zero length
			if (DiseaseName.Length == 0)
				throw new ArgumentException("DiseaseName cannot be zero length.", "DiseaseName");
			// Effective Period must be greater than zero
			if (EffectivePeriod < 1)
				throw new ArgumentOutOfRangeException("EffectivePeriod",
					"EffectivePeriod must be greater than zero.");
			// set values
			mvarDiseaseName = DiseaseName;
			mvarEffectivePeriod = EffectivePeriod;
		}

		/// <summary>
		///		Apply this vaccination strategy.
		/// </summary>
		public override void ApplyStrategy() 
		{
			// set range of Random numbers
			//mvarBackground.RandomNum.MinValue = 0;
			//mvarBackground.RandomNum.MaxValue = 100;
			// loop through all cells in the background cell list
			foreach (cCell Cell in mvarBackground.Cells) {
				// is this cell in the strategy list?
				if (Values.ContainsKey(Cell.ID)) {
					int AppliedLevel = Convert.ToInt32(Values[Cell.ID]);
					// loop through all animals in the cell, applying the vaccine if appropriate
					foreach (cAnimal Animal in Cell.Animals) {
						if (mvarBackground.RandomNum.IntValue(1, 100) <= AppliedLevel) {
							// and vaccinate the animal with it
							Animal.Vaccinate(mvarDiseaseName, mvarEffectivePeriod);
						}
					}
				}
			}
			
		}

        // get the name for the type of strategy
        protected override string GetStrategyTypeName()
        {
            return "Vaccine Strategy";
        }


		// **************************** private members **********************************
		protected string mvarDiseaseName;
		protected int mvarEffectivePeriod;
	}
}