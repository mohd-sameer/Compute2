using System;

namespace Random
{
	/// <summary>
	///		Generates a series of pseudo-random deviates with a gaussian distribution of
	///		specified average and variance.  By default, the avarage is 1, the variance
	///		is 0.1 .
	/// </summary>
	public class cGaussianRandom
    {
        #region Constructors

        /// <summary>
        /// The random number generator used
        /// </summary>
        protected cRandomBase _rndGenerator;

        // ************** Constructors **********************************************
		/// <summary>
		///		Default constructor.  Average = 1, Variance = 0.1 .
		/// </summary>
        /// <param name="RndGenerator">The random number generator to use</param>
        public cGaussianRandom(cRandomBase RndGenerator)
            : this(1.0, 0.1, RndGenerator)
		{
		}
		
		/// <summary>
		///		Set Average and Variance Constructor.  Throws an ArgumentOutOfRangeException
		///		exception if the variance is set to less than 0.
		/// </summary>
		/// <param name="Average">The average value of the returned random deviates.</param>
		/// <param name="Variance">The variance of the returned random deviates.</param>
        public cGaussianRandom(double Average, double Variance, cRandomBase RndGenerator)
		{
            if (RndGenerator == null) throw new ArgumentNullException("RndGenerator");
			// throw an exception if the variance is less than or equal to 0
			if (Variance <= 0) ThrowVarianceException();
            // store the generator
            _rndGenerator = RndGenerator;
			// set the average and variance
			_average = Average;
			_variance = Variance;
			DeviateWaiting = false;
        }

        #endregion

        #region Properties

        // *********************** properties ****************************************
		/// <summary>
		///		The average of the generated random deviates.
		/// </summary>
		public double Average 
		{
			get 
			{
				return _average;
			}
			set
			{
				// set the value
				_average = value;
				// reset the generator
				DeviateWaiting = false;
			}
		}

		/// <summary>
		///		The variance of the generated random deviates.  An ArgumentOutOfRangeException
		///		will be thrown if an attempt is made to set the value of the variance to less
		///		than zero.
		/// </summary>
		public double Variance 
		{
			get 
			{
				return _variance;
			}
			set 
			{
				// throw an exception if value is <= 0
				if (value <= 0) ThrowVarianceException();
				// set the value
				_variance = value;
				// reset the generator
				DeviateWaiting = false;
			}
		}

        /// <summary>
        ///	A random number. 
        ///	(read-only property)
        /// </summary>
        public double Value
        {
            get
            {
                return GenerateValue();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///		Reset the random number generator.  After reset, the random generator will
        ///		repeat the sequence of psuedo-random numbers 
        /// </summary>
        public void reset()
        {
            _rndGenerator.reset();
            DeviateWaiting = false;
        }

        #endregion


        #region Protected Methods

        /// <summary>
        ///     Generate an appropriate random number for the defined class 
        /// </summary>
        /// <returns>The generated random number</returns>
        protected double GenerateValue()
        {
            // deviates are calculated two at a time
            if (DeviateWaiting)
            {
                // a deviate is calculated and waiting!!
                DeviateWaiting = false;
                return WaitingDeviate;
            }
            else
            {
                // we must calculate some deviates
                double Fac, R, V1, V2;
                do
                {
                    // pick two uniform deviates in the square extending from
                    // -Variance to +Variance
                    V1 = 2 * _rndGenerator.GetValue() - 1;
                    V2 = 2 * _rndGenerator.GetValue() - 1;
                    // are they in the circle.  Keep looping until they are
                    R = V1 * V1 + V2 * V2;
                } while (R >= 1 || R == 0);
                // now use the Box-Muller transformation to get two normal
                // deviates.  Return one now and keep the other for later
                Fac = Math.Sqrt(-2 * Math.Log(R) / R);
                WaitingDeviate = Fac * V1 * _variance + _average;
                DeviateWaiting = true;
                return Fac * V2 * _variance + _average;
            }
        }

        #endregion

        #region Private Methods and Values

        // *********************** private members ***********************************
		// internal storage for the desired average and variance values
		private double _average, _variance;
		// internal storage of deviate values
		private bool DeviateWaiting;
		private double WaitingDeviate;

		// throw an exception indicating that the variance is being set to less than zero
		private void ThrowVarianceException()
		{
			throw new ArgumentOutOfRangeException("Variance",
												"The variance must be zero or greater.");
        }

        #endregion
    }
}
