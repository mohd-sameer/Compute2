using System;

namespace Random
{
	/// <summary>
	///		Generates a series of pseudo-random deviates with a uniform distribution
	///		between a specified minumum and maximum value.
	/// </summary>
	public class cUniformRandom : cRandomBase
    {
        #region Constructors

        // ************************* constructors ************************************
		/// <summary>
		///		Default constructor.  MinValue = 0, MaxValue = 1.
		/// </summary>
		public cUniformRandom() : this(0, 1) 
		{
		}

		/// <summary>
		///		Set MinValue and MaxValue constructor.  If MinValue is not less than
		///		MaxValue, an ArgumentException exception is raised.
		/// </summary>
		/// <param name="MinValue">Minimum random value generated.</param>
		/// <param name="MaxValue">Maximum random value generated.</param>
		public cUniformRandom(double MinValue, double MaxValue) : base()
		{
			// raise an exception if MinValue >= MaxValue
			if (MinValue >= MaxValue) ThrowMinMaxException("MinValue");
			// set the minimum and maximum values
			_minValue = MinValue;
			_maxValue = MaxValue;
            _intMinValue = (int)Math.Ceiling(MinValue);
            _intMaxValue = (int)Math.Floor(MaxValue);
            if (_intMaxValue < _intMinValue) _intMaxValue = _intMinValue;
        }

        #endregion

        #region Public Properties

        // *********************** properties ****************************************
		/// <summary>
		///		The minimum value returned by the Value property.  This property is
		///		used in conjunction with the MaxValue property to define the range of
		///		the random numbers returned by the Value property.  Attempting to set
		///		MinValue to a value that is greater than or equal to MaxValue will raise
		///		an ArgumentException exception.  Changing this property does not
		///		reset the random generator.
		/// </summary>
		public double MinValue 
		{
			get 
			{
				return _minValue;
			}
			set 
			{
				if (value >= _maxValue) ThrowMinMaxException("MinValue");
				_minValue = value;
                _intMinValue = (int)Math.Ceiling(value);
                if (_intMaxValue < _intMinValue) _intMaxValue = _intMinValue;

			}
		}

		/// <summary>
		///		The maximum value returned by the Value property.  This property is
		///		used in conjunction with the MinValue property to define the range of
		///		the random numbers returned by the Value property.  Attempting to set
		///		MaxValue to a value that is less than or equal to MinValue will raise
		///		an ArgumentException exception.  Changing this property does not
		///		reset the random generator.
		/// </summary>
		public double MaxValue 
		{
			get 
			{
				return _maxValue;
			}
			set 
			{
				if (value <= _minValue) ThrowMinMaxException("MaxValue");
				_maxValue = value;
                _intMaxValue = (int)Math.Floor(MaxValue);
                if (_intMaxValue < _intMinValue) _intMaxValue = _intMinValue;
			}
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Return an integer value between the current Minimum and Maximum values.
        /// The current minimum value is rounded up, the current maximum value is rounded down
        /// </summary>
        /// <returns>An random integer between the current minumum and maximum values</returns>
        public int IntValue()
        {
            return IntValue(_intMinValue, _intMaxValue);
        }

        /// <summary>
        /// Return and integer value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="MinValue"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public int IntValue(int MinValue, int MaxValue)
        {
            if (MaxValue <= MinValue)
            {
                return MinValue;
            }
            else
            {
                return Rnd.randInt(MinValue, MaxValue);
            }
        }

        /// <summary>
        /// Return a double value between the specified minimum and maximum values
        /// </summary>
        /// <param name="MinValue">The minimum value</param>
        /// <param name="MaxValue">The maximum value</param>
        /// <returns>A double value betweem MinValue and MaxValue</returns>
        public double RealValue(double MinValue, double MaxValue)
        {
            return base.GetValue() * (MaxValue - MinValue) + MinValue;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generate an appropriate random number for the defined class 
        /// </summary>
        /// <returns>The generated random number</returns>
        protected override double GenerateValue()
        {
            if (_minValue == 0 && _maxValue == 1)
            {
                return base.GetValue();
            }
            else
            {
                // get a random number from 0 to 1
                double RanNumber = base.GetValue();
                // put that number into the desired range
                return RanNumber * (_maxValue - _minValue) + _minValue;
            }
        }

        #endregion

        #region Private Methods and Values

        // *********************** private members ***********************************
		// internal storage for the desired maximum and minimum values
		private double _minValue, _maxValue;
        private int _intMinValue, _intMaxValue;

		// throw an exception indicating the minimum value must be less than the 
		// maximum value
		private void ThrowMinMaxException(string Param)
		{
			throw new ArgumentException("The minimum value must be less than the maximum value.",
										Param);
        }

        #endregion
    }
}
