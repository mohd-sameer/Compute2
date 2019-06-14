using System;
using System.Collections;
using System.Collections.Generic;
using Random;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A list of years indexed by integer.  This class allows the years to be stepped
	///		through one week at a time; it is used to keep track of time in the	Rabies Model.
	/// </summary>
	public class cYearList : IEnumerable
    {
        #region Protected Members

        /// <summary>
        /// Reference to a random number generator for generating years
        /// </summary>
        protected cUniformRandom _yearRnd = null;

        /// <summary>
        /// Tell the object to generate years as soon as a random number is set
        /// </summary>
        protected bool GenerateYearsOnRandomSet = false;
        /// <summary>
        /// Bias for above generated winter types
        /// </summary>
        protected enumWinterType GenerateYearsWinterType = enumWinterType.Normal;
        /// <summary>
        /// Number of years for above generated winter types
        /// </summary>
        protected int GenerateYearsNYears = 0;

        #endregion

        // ********************** Constructors *******************************************
		/// <summary>
		///		Initialize a cYearList by specifying the number of years.  Winter types
		///		are generated randomly with a "normal" bias.
		/// </summary>
		/// <param name="NYears">
		///     The number of years to place in the list.  An ArgumentException is raised
		///     if NYears is less than or equal to zero.
		/// </param>
        /// <param name="YearRnd">
        ///     A random number generator for creating random years
        /// </param>
		public cYearList(int NYears, cUniformRandom YearRnd)
            : this(NYears, YearRnd, enumWinterType.Normal)
		{
		}

		/// <summary>
		///		Initialize a cYearList by specifying the number of years.  Winter types
		///		are generated randomly with a bias toward the type specified by the 
		///		'WinterBias' parameter.  If YearRnd is null, the winters are generated as
        ///		soon as the YearRnd property is set.
		/// </summary>
		/// <param name="NYears">
		///		The number of years to place in the list.  An ArgumentException is raised
		///		if NYears is less than or equal to zero.
		/// </param>
		/// <param name="WinterBias">
		///		The bias of the randomly generated winter types.  Options include Very Severe,
		///		Severe, Normal, Mild, and Very Mild.
		/// </param>
        public cYearList(int NYears, cUniformRandom YearRnd, enumWinterType WinterBias)
		{           
            // make sure nYears > 0
            if (NYears <= 0) ThrowNYearsException();
            // store random number generator
            _yearRnd = YearRnd;
            // create the list
			Values = new List<cYear>();
			// create the object
            if (_yearRnd != null)
            {
                GenerateYearsRandom(WinterBias, NYears);
            }
            else
            {
                GenerateYearsOnRandomSet = true;
                GenerateYearsWinterType = WinterBias;
                GenerateYearsNYears = NYears;
            }
			// initialize to first year
			mvarCurrentYearNum = 0;
		}

		/// <summary>
		///		Initialize a cYearList by passing a list of winter types.  One year is
		///		added to the list for each value in the cWinterTypeList.  An ArgumentException
		///		exception is raised if the passed list of winter types is empty.
		/// </summary>
		/// <param name="WTList">The list of winter types.</param>
        public cYearList(cWinterTypeList WTList, cUniformRandom YearRnd) 
		{
            if (WTList == null) throw new ArgumentNullException("WTList");
			// create the list
			Values = new List<cYear>();
			// add the years.  Exception will be raised in this method if WTList is empty
			AddYearsFromList(WTList);
			// initialize to first year
			mvarCurrentYearNum = 0;
		}

		/// <summary>
		///		Create an empty cYear list.  This constructor is Internal and can only be
		///		called by objects that are within the Rabies_Model_Core namespace.
		/// </summary>
        internal cYearList(cUniformRandom YearRnd)
		{
            if (YearRnd == null) throw new ArgumentNullException("YearRnd");
            // create the list
			Values = new List<cYear>();
			// initialize to "first" year
			mvarCurrentYearNum = 0;
		}

		// ************************ Properties ******************************************

        /// <summary>
        ///     Get or set the random number generator used to create winter types
        /// </summary>
        public cUniformRandom YearRnd
        {
            get
            {
                return _yearRnd;
            }
            set
            {
                _yearRnd = value;
                // fill list if it is empty
                if (GenerateYearsOnRandomSet && _yearRnd != null) GenerateYearsRandom(GenerateYearsWinterType, GenerateYearsNYears);
            }
        }

		/// <summary>
		///    The current year in the list.  The current year is set to the first year
		///    in the list when a cYearList object is initialized.  The current year is
		///    incremented by the IncrementTime function (read-only).
		/// </summary>
		public cYear CurrentYear 
		{
			get 
			{
				return Values[mvarCurrentYearNum];
			}
		}

		/// <summary>
		///		The position in the list of the current year (read-only).
		/// </summary>
		public int CurrentYearNum
		{
			get
			{
				return mvarCurrentYearNum;
			}
		}

		/// <summary>
		///		A Boolean property indicating whether the current year is the last year in
		///		the list.  This property is set to true if the current year is the last 
		///		year (read-only).
		/// </summary>
		public bool IsLastYear 
		{
			get 
			{
				return (mvarCurrentYearNum == Values.Count - 1);
			}
		}

		/// <summary>
		///		Retrieve a reference to the year at the passed index value (read-only).    An 
		///		ArgumentOutOfRangeException exception is raised if the index is less than zero
		///		or greater than the number of items in the list minus one.
		/// </summary>
		public cYear this[int index] 
		{
			get 
			{
				return Values[index];
			}
		}

		/// <summary>
		///		The number of years in the list (read-only).
		/// </summary>
		public int Count 
		{
			get 
			{
				return Values.Count;
			}
		}

		// ******************* methods **************************************************
		/// <summary>
		///		Increments time one week at a time.  If the week value of the current year
		///		is 52, this method will switch to the next year in the year list.  Calling
		///		this method after the week value of the last year in the list has reached
		///		52 results in an ex_cYearList_TimeIncrementException exception being raised.
		/// </summary>
		public void IncrementTime()
		{
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cYearList.cs: IncrementTime()");

            // has the current year reached week 52
            cYear TheYear = Values[mvarCurrentYearNum];

            //System.Diagnostics.Debug.WriteLine("cYearList.cs: IncrementTime(): TheYear = " + TheYear);

            if (TheYear.CurrentWeek == 52)
			{
				// YES it is 52
				// is the current year the last year
				if (mvarCurrentYearNum == Values.Count) 
				{
					// YES it is the last year
					// throw an exception, we cannot count further
					throw new InvalidOperationException(
						"Cannot increment another week.  You are at the last week of the last year.");
				}
				else 
				{
					// NO it is not the last year
					// move to the next year in the list
					mvarCurrentYearNum++;
				}
			}
			else 
			{
				// NO it is not 52
				// increment the week
				TheYear.IncrementWeek();
			}
		}

		/// <summary>
		///		Add a year to the year list.
		/// </summary>
		/// <param name="WinterType">The winter type of the added year.</param>
		/// <returns>The newly created year.</returns>
		public cYear Add(enumWinterType WinterType) 
		{
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cYearList.cs: cYear: Add()");

            // create the year
            cYear NewYear = new cYear(Values.Count, WinterType);
			// add the year to the list
			Values.Add(NewYear);
			// return the value
			return NewYear;
		}

		/// <summary>
		///		Add 'NYears' additional years to this list.  The winter types of the new
		///		years will be randomly generated with a 'Normal' winter bias.
		/// </summary>
		/// <param name="NYears">
		///		The number of years to add.  An ArgumentException exception is raised if NYears
		///		is less than or equal to zero.</param>
		public void AddYears(int NYears)
		{
            //System.Diagnostics.Debug.WriteLine("");
           // System.Diagnostics.Debug.WriteLine("cYearList.cs: AddYears()");

            // make sure nYears > 0
            if (NYears <= 0) ThrowNYearsException();
			GenerateYearsRandom(enumWinterType.Normal, NYears);
		}

		/// <summary>
		///		Add 'NYears' additional years to this list.  The winter types of the new
		///		years will be randomly generated but biased towards the value of 
		///		'WinterBias'.
		/// </summary>
		/// <param name="NYears">
		///		The number of years to add.  An ArgumentException exception is raised if NYears
		///		is less than or equal to zero.</param>
		/// <param name="WinterBias">
		///		The bias of the randomly generated winter types.  Options include Very Severe,
		///		Severe, Normal, Mild, and Very Mild.
		///	</param>
		public void AddYears(int NYears, enumWinterType WinterBias)
		{
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cYearList.cs: AddYears() with WinterBias");

            // make sure nYears > 0
            if (NYears <= 0) ThrowNYearsException();
			GenerateYearsRandom(WinterBias, NYears);
		}

		/// <summary>
		///		Add additional years to this list from a passed list of winter types.  A
		///		new year is added for each item in the winter type list.
		/// </summary>
		/// <param name="WTList">
		///		The list of winter types.  An ArgumentException exception is raised if this
		///		list is empty.
		///	</param>
		public void AddYears(cWinterTypeList WTList)
		{
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("cYearList.cs: AddYears() from Winter list");

            AddYearsFromList(WTList);
		}

		/// <summary>
		///		Return a list of the winter types assigned to the years in this list.
		/// </summary>
		/// <returns>The list of winter types in a cWinterTypeList object.</returns>
		public cWinterTypeList GetWinterTypes()
		{
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("cYearList.cs: cWinterTypeList: GetWinterTypes()");

            // create the winter type list
            cWinterTypeList WTList = new cWinterTypeList();
			// add the winter types to this list
			foreach(cYear Year in this) 
			{
				WTList.Add(Year.WinterType);
			}
			return WTList;
		}

		/// <summary>
		///		Get an enumerator that can iterate through the years.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		// *************************** internal members ********************************
		/// <summary>
		///		Add a year to the year list, setting the current weeek of that year at the
		///		same time.  This is an internal function that can only be called by objects
		///		within the Rabies_Model_Core namespace.
		/// </summary>
		/// <param name="WinterType">The winter type of the new year.</param>
		/// <param name="CurrentWeek">
		///		The current week of the new year.  An ArgumentOutOfRangeException exception is
		///		raised if the value of CurrentWeek is not between 1 and 52.
		///	</param>
		/// <returns>A reference to the new year.</returns>
		internal cYear Add(enumWinterType WinterType, int CurrentWeek) 
		{
			// create the year
			cYear NewYear = new cYear(Values.Count, WinterType);
			NewYear.SetCurrentWeek(CurrentWeek);
			// add the year to the list
			Values.Add(NewYear);
			// return the newly create year
			return NewYear;
		}

		/// <summary>
		///		Clear all years from the list.  An ArgumentOutOfRangeException exception is
		///		raised if the value of CurrentWeek is not between 1 and 52.
		/// </summary>
		internal void Clear() 
		{
			Values.Clear();
		}
		
		/// <summary>
		///		Set the current year and week.  This is an internal function that can only be
		///		called by objects within the Rabies_Model_Core namespace.
		/// </summary>
		/// <param name="CurrentYear">
		///		The new current year.  An ArgumentOutOfRange Exception is raised if this value 
		///		is less than 0 or greater than the number of years minus one.
		/// </param>
		/// <param name="CurrentWeek">
		///		The current week for the new current year.  An ArgumentOutOfRangeException
		///		exception is raised if the value of CurrentWeek is not between 1 and 52.
		/// </param>
		internal void SetYearAndWeek(int CurrentYear, int CurrentWeek)
		{
			// make sure the current year exists in the list
			if (CurrentYear > Values.Count - 1)
				throw new ArgumentOutOfRangeException("CurrentYear",
					"The value of CurrentYear cannot exceed the number of years in the list");
			// set the current year
			mvarCurrentYearNum = CurrentYear;
			// set the current week
			cYear TheYear = Values[mvarCurrentYearNum];
			TheYear.SetCurrentWeek(CurrentWeek);
		}

		// **************** private members *******************************************
		// The list of values
		private List<cYear> Values;
		// The position in the list of the current year.
		private int mvarCurrentYearNum; 

		// Add years to the list with randomly generated winter types.
		private void GenerateYearsRandom(enumWinterType WinterBias, int NYears)
		{
            // make sure we can do this
            if (_yearRnd == null) throw new InvalidOperationException("Error generating winter list. The random number generator is not set");

			double RanValue;
			// array of probabilities for each winter bias
			int[,] Probabilities = new int[5, 5] {	
													{ 40, 30, 20, 10, 0 },
													{ 22, 40, 22, 12, 4 },
													{ 10, 20, 40, 20, 10 },
													{ 4, 12, 22, 40, 22 },
													{ 0, 10, 20, 30, 40 } };
			int ProbSum, TempWType;
			enumWinterType WType;
			cYear Year;
            // create an array of about 1000 random numbers.  This way, no matter how many years are run, the results
            // will be the same
            int[] RandomValues = new int[1000];
            for (int i = 0; i < 1000; i++) RandomValues[i] = _yearRnd.IntValue(0, 100);
			// create NYears cYear objects
            int randomCounter = 0;
			for (int i = 0; i < NYears; i++) 
			{
				// set WType to lowest value
				TempWType = 0;
				// get a random number
                RanValue = RandomValues[randomCounter];
                randomCounter++;
                if (randomCounter == 1000) randomCounter = 0;
				// loop, adding together appropriate probablities for selected winter bias
				// until the sum is greater than the random value.  As you do this, keep 
				// incrementing the winter type (TempWType).
				ProbSum = 0;
				while (ProbSum < RanValue) 
				{
					ProbSum += Probabilities[(int)WinterBias - 1, TempWType];
					TempWType++;
				}
				WType = (enumWinterType)TempWType;
				// create a year with this winter type
				Year = new cYear(Values.Count, WType);
				// add this year to the list
				Values.Add(Year);
			}
            GenerateYearsOnRandomSet = false;
		}

		//	Add years to the list.  An ArgumentException exception is raised if the passed list
		//	of winter types is empty. 
		private void AddYearsFromList(cWinterTypeList WTList)
		{
			cYear Year;
			// raise an exception if WTList is empty
			if (WTList.Count == 0) 
				throw new ArgumentException("The list of winter types cannot be empty.",
											"WTList");
			// create the Years
			for (int i = 0; i < WTList.Count; i++) 
			{
				// create year based on winter type in the list
				Year = new cYear(Values.Count, WTList[i]);
				// add this year to the list
				Values.Add(Year);
			}
            GenerateYearsOnRandomSet = false;
		}

		// Throw an exception if the user attempts to build or enhance a cYearList with 0 or
		// negative year count.
		private void ThrowNYearsException()
		{
			throw new ArgumentException("NYears must be > 0", "NYears");
		}

	}
}