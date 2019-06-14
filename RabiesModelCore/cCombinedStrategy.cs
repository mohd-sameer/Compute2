using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A strategy that vaccinates and applies fertility control to animals in a set of 
	///		cells.  In each affected cell, the Level property detirmines the percent chance
	///		that each animal in the cell will have the combined strategy applied.  The disease
	///		the vaccine protects against and the vaccine's effective period are specified in
	///		the constructor for the vaccine strategy.  The effective period for the fertility
	///		control is a 
	/// </summary>
	public class cCombinedStrategy : cStrategy
	{
		/// <summary>
		///		Initialize a cCombinedStrategy.
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
		/// <param name="VaccineEffectivePeriod">
		///		The effective period (in weeks) of the vaccine being used.  An 
		///		ArgumentOutOfRangeException exception is raise if VaccineEffectivePeriod
		///		is less than zero.
		///	</param>
		///	<param name="FertilityEffectivePeriod">
		///		The effective period(in weeks) of the fertility control.  An ArgumentOutOfRangeException
		///		exception is raised if FertilityEffectivePeriod is less than zero.
		///	</param>
		public cCombinedStrategy(cBackground BG, cCellList Cells, double Level, 
			                     int Year, int Week, string DiseaseName, int VaccineEffectivePeriod,
			                     int FertilityEffectivePeriod) 
			: base(BG, Cells, Level, Year, Week)
		{
			// DiseaseName should not be zero length
			if (DiseaseName.Length == 0)
				throw new ArgumentException("DiseaseName cannot be zero length.", "DiseaseName");
			// Effective Periods must be greater than zero
			if (VaccineEffectivePeriod < 1)
				throw new ArgumentOutOfRangeException("VaccineEffectivePeriod",
					"VaccineEffectivePeriod must be greater than zero.");
			if (FertilityEffectivePeriod < 1)
				throw new ArgumentOutOfRangeException("FertilityEffectivePeriod",
					"FertilityEffectivePeriod must be greater than zero.");
			// set values
			mvarDiseaseName = DiseaseName;
			mvarVaccineEffectivePeriod = VaccineEffectivePeriod;
			mvarFertilityEffectivePeriod = FertilityEffectivePeriod;
		}

		/// <summary>
		///		Apply this combined strategy.
		/// </summary>
		public override void ApplyStrategy() 
		{
			// loop through all cells in the background cell list
			foreach (cCell Cell in mvarBackground.Cells) 
			{
				// is this cell in the strategy list?
				if (this.Contains(Cell.ID)) 
				{
					int AppliedLevel = Convert.ToInt32(Values[Cell.ID]);
					// loop through all animals in the cell, applying the vaccine if appropriate
					foreach (cAnimal Animal in Cell.Animals) 
					{
						// set range of Random numbers
						//mvarBackground.RandomNum.MinValue = 0;
						//mvarBackground.RandomNum.MaxValue = 100;
						// apply vaccine if required
						if (mvarBackground.RandomNum.IntValue(1, 100) <= AppliedLevel) 
						{
							// and vaccinate the animal with it
							Animal.Vaccinate(mvarDiseaseName, mvarVaccineEffectivePeriod);
							// make the animal infertile
							Animal.CannotGiveBirthValue = mvarFertilityEffectivePeriod;
						}
					}
				}
			}
		}

        // get the name for the type of strategy
        protected override string GetStrategyTypeName()
        {
            return "Combined Strategy";
        }


		// **************************** private members **********************************
		protected string mvarDiseaseName;
		protected int mvarVaccineEffectivePeriod;
		protected int mvarFertilityEffectivePeriod;
	}
}
