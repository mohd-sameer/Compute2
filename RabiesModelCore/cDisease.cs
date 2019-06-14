using System;
using Random;

namespace Rabies_Model_Core
{
	/// <summary>
	///		Defines a disease that infects the animals in the model.
	/// </summary>
	public class cDisease
	{   /// <summary>
        ///		Initialize a Disease.
        /// </summary>
        /// <param name="Name">
        ///		The name of the disease.  An ArgumentException exception is raised if Name has
        ///		zero length.
        /// </param>
        /// <param name="IncubationPeriod">
        ///		The average incubation period of the disease.  An ArgumentOutOfRangeException
        ///		exception is raised if IncubationPeriod is less than zero.
        /// </param>
        /// <param name="IncubationVariance">
        ///		The variance in the incubation period.  An ArgumentOutOfRangeException
        ///		exception is raised if IncubationVariance is less than zero.
        ///	</param>
        /// <param name="InfectiousPeriod">
        ///		The average infectious period of the disease.  An ArgumentOutOfRangeException
        ///		exception is raised if InfectiousPeriod is less than one.
        /// </param>
        /// <param name="InfectiousVariance">
        ///		The variance in the infectious period.  An ArgumentOutOfRangeException
        ///		exception is raised if InfectiousVariance is less than zero.
        ///	</param>
        /// <param name="ContactRate">
        ///		The contact rate for the infected animal.  That is, the weekly chance of spreading
        ///		the disease to each animal in the infected animals activity radius.  The value is
        ///		expressed as a percent.  An ArgumentOutOfRangeException exception is raised if
        ///		CellContaceRate is not in the range of 0 to 100.
        ///	</param>
        /// <param name="ChanceOfDeath">
        ///		The probability that the animal will die once the disease has run its
        ///		course.  An	ArgumentOutOfRangeException	exception is raised if ChanceOfDeath
        ///		is not in the range of 0 to 100.
        /// </param>
        /// <param name="BecomesImmune">
        ///		If set to true, any animal that recovers from this disease will become immune for
        ///		life.
        /// </param>
        /// <param name="RecoveredNotInfectious">
        ///     If set to true, recovered animals will not become infectious
        /// </param>
        /// <param name="Rnd">The random number generator used by this disease
        /// </param>
        
        public cDisease(string Name, int IncubationPeriod, double IncubationVariance,
						int InfectiousPeriod, double InfectiousVariance,
						double ContactRate, double ChanceOfDeath, bool BecomesImmune,
                        bool RecoveredNotInfectious, cUniformRandom Rnd)
		{
			// name must not be zero length
			if (Name.Length == 0) 
				throw new ArgumentException("Name must not be 0 length.", "Name");
			// Incubation period must not be negative
			if (IncubationPeriod < 0)
				throw new ArgumentOutOfRangeException("IncubationPeriod", 
					"Incubation period must be >= 0");
			if (InfectiousPeriod < 1) 
				throw new ArgumentOutOfRangeException("InfectiousPeriod",
					"The infectious period must have a value of at leat one"); 
			// contact rates must be between 0 and 1.
			if (ContactRate < 0 || ContactRate > 100)
				throw new ArgumentOutOfRangeException("ContactRate", 
					"The contact rate must be between 0 and 100.");
			// chance of death must be between 0 and 100
			if (ChanceOfDeath < 0 || ChanceOfDeath > 100)
				throw new ArgumentOutOfRangeException("ChanceOfDeath", 
					"Chance of death must be between 0 and 100.");
			// set the values
			mvarName = Name;
			mvarIncubationPeriod = new cGaussianRandom((double) IncubationPeriod, IncubationVariance, Rnd);
			mvarInfectiousPeriod = new cGaussianRandom((double) InfectiousPeriod, InfectiousVariance, Rnd);
			mvarContactRate = ContactRate;
			mvarChanceOfDeath = ChanceOfDeath / 100;
			mvarRandom = Rnd;
			mvarBecomesImmune = BecomesImmune;
            mvarRecoveredNotInfectious = RecoveredNotInfectious;
            
        }

		// *********************** Properties *******************************************
		/// <summary>
		///		The average incubation period in weeks (read-only).
		/// </summary>
		public int IncubationPeriod 
		{
			get 
			{
				return (int) mvarIncubationPeriod.Average;
			}
		}

		/// <summary>
		///		The variance in the incubation period (read-only).
		/// </summary>
		public double IncubationVariance
		{
			get 
			{
				return mvarIncubationPeriod.Variance;
			}
		}

		/// <summary>
		///		The average incubation period in weeks (read-only).
		/// </summary>
		public int InfectiousPeriod 
		{
			get 
			{
				return (int) mvarInfectiousPeriod.Average;
			}
		}

		/// <summary>
		///		The variance in the infectious period (read-only).
		/// </summary>
		public double InfectiousVariance
		{
			get 
			{
				return mvarInfectiousPeriod.Variance;
			}
		}

		/// <summary>
		///		The name of the disease (read-only).
		/// </summary>
		public string Name
		{
			get 
			{
				return mvarName;
			}
		}

		/// <summary>
		///		The overall contact rate of the disease.  That is, the weekly chance
		///		of spreading the disease to other animals within the activity radius
		///		of the infected animal.  The value is expressed as a percent .
		///		(read-only).
		/// </summary>
		public double ContactRate 
		{
			get 
			{
				return mvarContactRate;
			}
		}
		
		/// <summary>
		///		The probability (expressed as a percentage) that the animal will die once
		///		the disease has run its course.
		/// </summary>
		public double ChanceOfDeath 
		{
			get 
			{
				return mvarChanceOfDeath * 100.0;
			}
		}

		/// <summary>
		///		A flag indicating whether or not an animal becomes immune after recovery from
		///		this disease.
		/// </summary>
		public bool BecomesImmune
		{
			get
			{
				return mvarBecomesImmune;
			}
		}

        /// <summary>
        ///     A flag indicating whether or not an animal that recovers will become infectious.
        ///     If this flag is set to true, recovered animals will not become infectious
        /// </summary>
        public bool RecoveredNotInfectious
        {
            get
            {
                return mvarRecoveredNotInfectious;
            }
        }

        
        // *********************** Methods **********************************************
        /// <summary>
        ///		Get an infectious period based on the average infectious period and
        ///		the variance of that period.  This function may be overriden in a 
        ///		derived class to provide a different method of calculating an infectious
        ///		period.
        /// </summary>
        /// <returns>The infectious period in weeks.</returns>
        public virtual int GetInfectiousPeriod()
        {
            //System.Diagnostics.Debug.WriteLine("cDisease.cs: GetInfectiousPeriod()");
            int TheValue;
			// make sure value is not <= 0
			do {
				TheValue = (int) mvarInfectiousPeriod.Value;
			} while (TheValue <= 0);
            //System.Diagnostics.Debug.WriteLine("cDisease.cs: GetInfectiousPeriod() TheValue = " + TheValue);
            return TheValue;
		}

		/// <summary>
		///		Get an incubation period based on the average incubation period and
		///		the variance of that period.  This function may be overriden in a 
		///		derived class to provide a different method of calculating an incubation
		///		period.
		/// </summary>
		/// <returns>The incubation period in weeks.</returns>
		public virtual int GetIncubationPeriod()
		{
            //System.Diagnostics.Debug.WriteLine("cDisease.cs: GetIncubationPeriod()");
            int TheValue;
			// make sure value is not <= 0
			do 
			{
				TheValue = (int) mvarIncubationPeriod.Value;
			} while (TheValue <= 0);
            //System.Diagnostics.Debug.WriteLine("    cDisease.cs: GetIncubationPeriod() TheValue = " + TheValue);
            return TheValue;
		}

		/// <summary>
		///		Randomly determines whether an instance of this disease is fatal based
		///		on the ChanceOfDeath setting.  This function may be overridden in a
		///		derived class if you wish to provide a different method for determining
		///		mortality due to this disease.
		/// </summary>
		/// <returns>True if the animal is killed.</returns>
		public virtual bool GetAnimalDies()
		{
            //System.Diagnostics.Debug.WriteLine("cDiease.cs GetAnimalDies()");
            //System.Diagnostics.Debug.WriteLine("cDiease.cs GetAnimalDies() mvarChanceOfDeath = " + mvarChanceOfDeath);
            if (mvarChanceOfDeath < 1)
            {
                //System.Diagnostics.Debug.WriteLine("cDiease.cs GetAnimalDies() mvarRandom.GetValue()  = " + mvarRandom.GetValue());
                return (mvarRandom.GetValue() < mvarChanceOfDeath);
            }
            
			else
			  return true;
		}

        // ******************** To string function **************************************

        /// <summary>
        /// Get the string representing this object
        /// </summary>
        /// <returns>The string representing this object</returns>
        public override string ToString()
        {
            return this.Name;
        }


		// *********************** Private members **************************************
		private string mvarName;
		private cGaussianRandom mvarIncubationPeriod;
		private cGaussianRandom mvarInfectiousPeriod;
		private double mvarContactRate;
		private double mvarChanceOfDeath;
		protected cUniformRandom mvarRandom;
		private bool mvarBecomesImmune;
        private bool mvarRecoveredNotInfectious;        
    }
}