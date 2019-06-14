using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A class that stores an association between a cell ID and a year and week value.
	/// </summary>
	public class cCellTime
	{
		// ************************** Constructor ***************************************
		/// <summary>
		///		Initialize a cCellTimeobject.  Cell ID is set to empty string, Year is set
		///		to 0.  Week is set to 1.
		/// </summary>
		public cCellTime()
		{
			mvarCellID = "";
			mvarYear = 0;
			mvarWeek = 1;
		}

		/// <summary>
		///		Initilize a cCellTime object.
		/// </summary>
		/// <param name="CellID">The ID of the cell.</param>
		/// <param name="Year">
		///		The year to associate with this cell.  An ArgumentOutOfRangeException
		///		exception is raised if Year is less than zero.
		///	</param>
		/// <param name="Week">
		///		The week to associate with this cell.  An ArgumentOutOfRangeException
		///		exception is raised if Week is not in the range 1-52.
		///	</param>
		public cCellTime(string CellID, int Year, int Week)
		{
			mvarCellID = CellID;
			// make sure year is not less than 0
			if (Year < 0) ThrowYearException();
			// make sure week is between 1 and 52
			if (Week < 1 || Week > 52) ThrowWeekException();
			mvarYear = Year;
			mvarWeek = Week;
		}

		// *********************** properties ******************************************
		/// <summary>
		///		The ID of the associated cell.
		/// </summary>
		public string CellID
		{
			get 
			{
				return mvarCellID;
			}
			set 
			{
				mvarCellID = value;
			}
		}

		/// <summary>
		///		The year associated with the cell.    An ArgumentOutOfRangeException exception
		///		is raised if an attempt is made to set Year is less than zero.
		/// </summary>
		public int Year
		{
			get 
			{
				return mvarYear;
			}
			set 
			{
				if (value < 0) ThrowYearException();
				mvarYear = value;
			}
		}

		/// <summary>
		///		The week associated with this cell.    An ArgumentOutOfRangeException exception
		///		is raised if an attempt is made to set Week outside of the range 1-52.
		/// </summary>
		public int Week
		{
			get 
			{
				return mvarWeek;
			}
			set 
			{
				// make sure week is between 1 and 52
				if (value < 1 || value > 52) ThrowWeekException();
				mvarWeek = value;
			}
		}

        // ******************** To string function **************************************

        /// <summary>
        /// Get the string representing this object
        /// </summary>
        /// <returns>The string representing this object</returns>
        public override string ToString()
        {
            return string.Format("{0} (Year: {1} Week: {2})", this.CellID, this.Year, this.Week);
        }


		// *********************** private members *************************************
		private string mvarCellID;
		private int mvarWeek, mvarYear;
		// throw an exception indicating the Year is out of range
		private void ThrowYearException()
		{
			throw new ArgumentOutOfRangeException("Year", "Year must be greater than 0.");
		}
		// throw an exception indicating the Week is out of range
		private void ThrowWeekException()
		{
			throw new ArgumentOutOfRangeException("Week", "Year must be greater than 0.");
		}

	}
}