using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A year of time in the rabies model.  A year has 52 weeks numbered from 1 to 52.
	///		The main function of this class is to increment through these weeks one after
	///		the other.  A year is also assigned a winter type.  This value has no effect on
	///		the year class itself, but its value can be referenced by the animals for the
	///		purposes of determining mortality, birth rates, etc.
	///	</summary>
    public class cYear
    {
    	// ************* constructor *******************************************
		/// <summary>
		///		Initialize a cYear object.
		/// </summary>
		/// <param name="ID">The numeric ID of the year.</param>
		/// <param name="WinterType">
		///		The type of winter for this year.  Options are Very Severe, Severe, Normal,
		///		Mild, and Very Mild.
		///	</param>
		public cYear(int ID, enumWinterType WinterType)
		{
			// year starts at week 1
			mvarCurrentWeek = 1;
			// set WinterType.  Note this is the only place this value can be set.
			mvarWinterType = WinterType;
			// set the ID
			mvarID = ID;
		}

		// ************* properties *********************************************
		/// <summary>
		///		The numeric ID of the year (read-only).
		/// </summary>
		public int ID 
		{
			get 
			{
				return mvarID;
			}
		}
      
		/// <summary>
		///		The current week of the year.  It has a value of 1 when a cYear 
		///		object is first created.  The current week is incremented by calling
		///		the 'IncrementWeek' method (read-only).
		/// </summary>
		public int CurrentWeek
		{
			get 
			{
				return mvarCurrentWeek;
			}
		}

		/// <summary>
		///    The type of winter for this year.  Options are Very Severe, Severe,
		///    Normal, Mild, Very Mild.  The value of this property may only be set
		///    when the class is initialized (read-only).
		/// </summary>
		public enumWinterType WinterType
		{
			get
			{
				return mvarWinterType;
			}
		}

		// ************* methods ************************************************
		/// <summary>
		///		Increment the current week value for this year.  The current week is
		///		set to 1 when a cYear is first constructed.  Attempting to increment
		///		the current week value beyond 52 will raise an
		///		ex_cYear_YearIncrementException	exception.
		/// </summary>
		/// <returns>The current week (1 to 52).</returns>
		public int IncrementWeek()
		{
			// if the current week is less than 52, then increment it
			if (mvarCurrentWeek < 52) {
				return mvarCurrentWeek++;
			}
			// raise an exception!! The week cannot have a value greater then 52
			else {
				throw new InvalidOperationException("Cannot increment week beyond 52.");
			}
		}

        /// <summary>
        /// Get the string representing this object
        /// </summary>
        /// <returns>The string representing this object</returns>
        public override string ToString()
        {
            return string.Format("{0} (Winter Type: {1})", this.ID, this.WinterType.ToString());
        }

		// ************* internal methods *********************************************
		/// <summary>
		///		Set the current week.  This is an internal function and can only be called 
		///		by objects within the Rabies_Model_Core namespace.
		/// </summary>
		/// <param name="Value">
		///		The value that you wish to set the week to.  An ArgumentOutOfRangeException
		///		exception is raised if the value is not in the range of 1 to 52.
		/// </param>
		internal void SetCurrentWeek(int Value) 
		{
			// validate the week value
			if (Value < 1 || Value > 52) 
				throw new ArgumentOutOfRangeException("Value",
										"Current week must have a value from 1 to 52.");
			// set the value
			mvarCurrentWeek = Value;
		}

		// ************* private members **********************************************
		// Local storage of the current week.
		private int mvarCurrentWeek;
		// Local storage of the winter type.
		private enumWinterType mvarWinterType;
		// Local storage of the ID
		private int mvarID;
    }
}