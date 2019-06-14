using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A class describing a vaccine against a disease.  The vaccine will offer complete
	///		protection from the disease during the period of it effectiveness.
	/// </summary>
	public class cVaccine
	{
		// ******************* Constructor *************************************************
		/// <summary>
		///		Initialize a vaccine object.
		/// </summary>
		/// <param name="DiseaseName">
		///		The name of the disease that this vaccine protects against.  An ArgumentException
		///		exception is raised if DiseaseName has zero length.
		///	</param>
		/// <param name="Year">
		///		The year the vaccine is given.  An ArgumentOutOfRangeException exception is
		///		raised if Year is less than zero."
		/// </param>
		/// <param name="Week">
		///		The week the vaccine is given.  An ArgumentOutOfRangeException exception is
		///		raised is Week is not in the range of 1-52.
		///	</param>
		/// <param name="EffectivePeriod">
		///		The period of time, in weeks, that the vaccine is effective.  An
		///		ArgumentOutOfRangeException exception is raised if EffectivePeriod is less than
		///		one.
		/// </param>
		public cVaccine(string DiseaseName, int Year, int Week, int EffectivePeriod)
		{
			// make sure values are valid
			if (DiseaseName.Length == 0) 
				throw new ArgumentException("Vaccine must protect against a named disease.",
											"DiseaseName");
			if (Year < 0) ThrowYearException();
			if (Week < 1 || Week > 52) ThrowWeekException();
			if (EffectivePeriod < 1) ThrowEffectivePeriodException();
			// set values
			mvarDiseaseName = DiseaseName;
			mvarYear = Year;
			mvarWeek = Week;
			mvarEffectivePeriod = EffectivePeriod;
		}

		// **************** Properties *****************************************************
		/// <summary>
		///		The name of the disease that this vaccine protects against (read-only).
		/// </summary>
		public string DiseaseName 
		{
			get 
			{
				return mvarDiseaseName;
			}
		}

		/// <summary>
		///		The year the vaccine was given.  An ArgumentOutOfRangeException exception is
		///		raised if Year is less than zero.
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
		///		The week the vaccine was given (read-only).
		/// </summary>
		public int Week
		{
			get 
			{
				return mvarWeek;
			}
		}

		/// <summary>
		///		The period, in weeks, that the vaccine is effective (read-only).
		/// </summary>
		public int EffectivePeriod 
		{
			get 
			{
				return mvarEffectivePeriod;
			}
		}

		/// <summary>
		///		Creates a deep copy of this vaccine object.
		/// </summary>
		/// <returns>The deep copy as a cVaccine.</returns>
		public virtual cVaccine Clone()
		{
			return new cVaccine(mvarDiseaseName, mvarYear, mvarWeek, mvarEffectivePeriod);
		}

		/// <summary>
		///		Determines whether or not the vaccine is still effective
		/// </summary>
		/// <param name="Year">The current year</param>
		/// <param name="Week">The current week</param>
		/// <returns>True if the vaccine is effective</returns>
		public bool IsEffective(int Year, int Week)
		{
			if (Year < 0) ThrowYearException();
			if (Week < 1 || Week > 52) ThrowWeekException();
			int TimeDiff = 52 * (Year - mvarYear) + Week - mvarWeek;
			return (TimeDiff <= mvarEffectivePeriod);
		}

		// **************** Private members ************************************************
		private string mvarDiseaseName;
		private int mvarYear;
		private int mvarWeek;
		private int mvarEffectivePeriod;
		// throw an exception indicating that a year value is out of range
		private void ThrowYearException()
		{
			throw new ArgumentOutOfRangeException("Year", "Year must be >= 0.");
		}
		// throw an exception indicating that a year value is out of range
		private void ThrowWeekException()
		{
			throw new ArgumentOutOfRangeException("Week", "Week must be between 1 and 52.");
		}
		// throw an exception indicating that the effective period is out of range
		private void ThrowEffectivePeriodException()
		{
			throw new ArgumentOutOfRangeException("EffectivePeriod", 
					"Effective period must be at least one week.");
		}

	}
}
