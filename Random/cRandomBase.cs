using System;
using RngStreams;

namespace Random
{
	/// <summary>
	///		Base class for random number generators.  Uses the RngStream random
    ///		number generator as its base generator
	/// </summary>
	public abstract class cRandomBase
	{
        /// <summary>
        /// The range stream random number generator associated with this random class
        /// </summary>
        protected RngStream Rnd;

        #region Static Methods

        /// <summary>
        /// Get the seed value for all random number generators created.
        /// </summary>
        public static long Seed
        {
            get
            {
                return RngStream.OriginalSeed;
            }
        }

        /// <summary>
        /// Set the seed for all random number generators created.  This method should be called
        /// before any random number instances are created.
        /// </summary>
        /// <param name="SeedVal">The seed value</param>
        /// <returns>True if the seed is successfully set, false otherwise</returns>
        public static bool SetSeed(long SeedVal)
        {
            return RngStream.setPackageSeed(SeedVal);
        }

        /// <summary>
        /// Set the seed for all random number generators created based on the current system time.  This
        /// method should be called before any random number instances are created.
        /// </summary>
        /// <returns>True if the seed is successfully set, false otherwise</returns>
        public static bool SetSeed()
        {
            return RngStream.setPackageSeed();
        }

        #endregion

        #region Constructors

        // ************************* Constructors *************************************
		/// <summary>
		///	Default constructor.
		/// </summary>
		public cRandomBase()
		{
            Rnd = new RngStream();
        }

        #endregion

        #region Public Properties

        // *********************** Properties ******************************************
		/// <summary>
		///	A random number.  This property must be implemented in a subclass.
		///	(read-only property)
		/// </summary>
		protected double Value 
		{
            get
            {
                return GenerateValue();
            }
        }

        /// <summary>
        /// Get or set a boolean value indicating that high precision numbers (53 bit as opposed to 32 bit)
        /// should be generated
        /// </summary>
        public bool HighPrecision
        {
            get
            {
                return Rnd.HighPrecision;
            }
            set
            {
                Rnd.HighPrecision = value;
            }
        }

        /// <summary>
        /// Get a string describing the inital state of the random number generator
        /// </summary>
        public string InitialState
        {
            get
            {
                return Rnd.InitialState;
            }
        }

        /// <summary>
        /// Get the value of the original seed value for the random stream generator that was used to generate
        /// this random generator
        /// </summary>
        public long StreamSeed
        {
            get
            {
                return Rnd.StreamSeed;
            }
        }

        /// <summary>
        /// Get the unique interger ID for this stream
        /// </summary>
        public int StreamID
        {
            get
            {
                return Rnd.StreamID;
            }
        }

        #endregion

        // *********************** Methods ***********************************************
		/// <summary>
		///		Reset the random number generator.  After reset, the random generator will
		///		repeat the sequence of psuedo-random numbers 
		/// </summary>
        public void reset()
        {
            internal_reset();
        }

        #region Protected Methods

        // ********************** Protected Memebers *************************************
		/// <summary>
		///		Get a random number from 0 to 1 by the currently selected random number
		///		generation method.
		/// </summary>
		/// <returns>A random number from 0 to 1</returns>
		public double GetValue()
		{
            return Rnd.randU01();
		}

        /// <summary>
        /// Reset the random number generator
        /// </summary>
        protected virtual void internal_reset()
        {
            Rnd.resetStartStream();
        }

        /// <summary>
        /// Generate an appropriate random number for the defined class (must be overriden) 
        /// </summary>
        /// <returns>The generated random number</returns>
        protected abstract double GenerateValue();

        #endregion

    }
}
