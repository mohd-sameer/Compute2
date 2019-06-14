using System;
using System.Collections;
using System.Collections.Generic;

namespace Rabies_Model_Core
{
	/// <summary>
	///		Defines a strategy for a group of cells.  For each cell, a percentage value
	///		from 0-100 is defined.  This value indicates the probability or level of some
	///		specific action occuring for that cell or for each animal in that cell.  You must 
	///		override this class to define the function that applies a specific strategy.
	/// </summary>
	public abstract class cStrategy : IComparable
	{
		// ************************ Constructors ****************************************
		/// <summary>
		///		Construct a cStrategy object by applying a strategy at the same level
		///		across the entire set of cells.
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
		public cStrategy(cBackground BG, cCellList Cells, double Level, int Year, int Week)
		{
			InitClass(BG, Cells, Level, Year, Week);
		}

		/// <summary>
		///		Construct a cStrategy object.  Each cell in the strategy is given a 0%
		///		level value by default.
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
		/// <param name="Year">
		///		The year in which this strategy will be applied.  An ArgumentOutOfRangeException
		///		exception is raised if Year is less then zero.
		///	</param>
		/// <param name="Week">
		///		The week in which this strategy will be applied.  An ArgumentOutOfRangeException
		///		exception is raised if Week is not in the range of 1-52.
		///	</param>
		public cStrategy(cBackground BG, cCellList Cells, int Year, int Week)
		{
			InitClass(BG, Cells, 0, Year, Week);
		}

		// initialize the class
		private void InitClass(cBackground BG, cCellList Cells, double Level, int Year, int Week)
		{
			// make sure Background is not a null reference
			if (BG == null)
				throw new ArgumentNullException("BG", "BG must not be null.");
			// make sure Cells is not null
			if (Cells == null) 
				throw new ArgumentNullException("Cells", "Cells must not be null.");
			// make sure Cells contains at least one cell
			if (Cells.Count == 0)
				throw new ArgumentException("Cells must not be an empty list", "Cells");
			// make sure all passed cells are in the background
			foreach (cCell Cell in Cells) {
				if (!BG.Cells.ContainsKey(Cell.ID))
					throw new ArgumentException("All passed cells must be part of the passed background.", "Cells");
			}
			// make sure Level is correct
			if (Level < 0 || Level > 100) ThrowLevelException();
			// check the year and the week
			if (Year < 0)
				throw new ArgumentOutOfRangeException("Year", "Year must be greater than zero.");
			if (Week < 1 || Week > 52)
				throw new ArgumentOutOfRangeException("Week", "Week must be in the range of 1-52.");
			// create the internal hashtable, applying the passed Value to each cell
            Values = new Dictionary<string, double>(Cells.Count);
			// loop through all passed cells, creating an entry for each
			foreach (cCell Cell in Cells) {
				Values.Add(Cell.ID, Level);
			}
			// store the reference to the background
			mvarBackground = BG;
			// set the year and week that the strategy is applied
			mvarYear = Year;
			mvarWeek = Week;
		}

		// ************************ Properties ******************************************
		/// <summary>
		///		The year that this strategy is applied (read-only).
		/// </summary>
		public int Year 
		{
			get 
			{
				return mvarYear;
			}
		}

		/// <summary>
		///		The week this strategy is applied (read-only).
		/// </summary>
		public int Week
		{
			get 
			{
				return mvarWeek;
			}
		}

		/// <summary>
		///		The background against which this strategy is applied (read-only).
		/// </summary>
		public cBackground Background
		{
			get 
			{
				return mvarBackground;
			}
		}

		/// <summary>
		///		Read or set the strategy value for the cell with the passed ID value.  An
		///		ArgumentOutOfRangeException exception is raised if an attempt is made to
		///		set a level to less than 0 or greater than 100.
		/// </summary>
		public double this[string CellID] 
		{
			get 
			{
				return Values[CellID];
			}
			set
			{
				// throw an exception if the passed value is out of range
				if (value < 0 || value > 100) ThrowLevelException();
				// set the value
				Values[CellID] = value;
			}
		}

		// ************************ Methods *********************************************
		/// <summary>
		///		Compare this instance of this class to another instance.  The second
		///		instance is less than this one if it is applied earlier.
		/// </summary>
		/// <param name="obj">The second instance of this class</param>
		/// <returns>
		///		-1 if this instance is applied earlier, 0 if both instances are applied at
		///		the same time, 1 if this instance is applied later.
		/// </returns>
		public int CompareTo(Object obj)
		{
			cStrategy Other = (cStrategy) obj;
			if		(mvarYear < Other.Year)		return -1;
			else if (mvarYear == Other.Year) {
				if		(mvarWeek < Other.Week) return -1;
				else if (mvarWeek > Other.Week) return 1;
				else							return 0;
			}
			else								return 1;
		}

		/// <summary>
		///		Detirmine whether or not the Strategy contains the passed cell ID.
		/// </summary>
		/// <param name="ID">The ID to test.</param>
		/// <returns>True if the strategy does contain this cell ID.</returns>
		public bool Contains(string ID) 
        {
			return Values.ContainsKey(ID);
		}

		/// <summary>
		///		Apply this strategy.  This is an abstract method that must be overriden
		///		in a derived class.
		/// </summary>
		public abstract void ApplyStrategy();

        // ******************** To string function **************************************

        /// <summary>
        /// Get the string representing this object
        /// </summary>
        /// <returns>The string representing this object</returns>
        public override string ToString()
        {
            foreach (double LevelVal in Values.Values) 
            {
                return string.Format("{0} at {1}% (Year: {2} Week: {3})", this.GetStrategyTypeName(), LevelVal, this.Year, this.Week);
            }
            // if we get here then we had no cells to begin with
            return string.Format("{0} at (Unknown)% (Year: {1} Week: {2})", this.GetStrategyTypeName(), this.Year, this.Week);
        }

		// ************************ Protected Members **************************************
		// the hashtable holding individual strategy values for each cell
		protected Dictionary<string, double> Values;
		// the background object against which this strategy is applied
		protected cBackground mvarBackground;

        // get the name for the type of strategy
        protected abstract string GetStrategyTypeName();

		// ************************* Private members ************************************
		// the week and year that this strategy is applied
		private int mvarYear, mvarWeek;
		// throw an exception indicating an invalid Level value
		private void ThrowLevelException()
		{
			throw new ArgumentOutOfRangeException("Level", "The level must be in the range of 0-100%");
		}
	}
}
