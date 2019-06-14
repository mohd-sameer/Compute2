using System;
using Rabies_Model_Core;

namespace Fox
{
	/// <summary>
	///		A class representing a datasource containing Fox objects.
	/// </summary>
	public abstract class cFoxDataSource : cAnimalsDataSource
	{
		/// <summary>
		///		Initialize a Fox datasource.
		/// </summary>
        /// <param name="IsUnix">A boolean value indocating that this is a Unix based datasource</param>
        public cFoxDataSource(bool IsUnix) : base(IsUnix) { }

		/// <summary>
		///		Create a new specific animal type
		/// </summary>
		/// <param name="Animal">The animal attributes used to define the animal.</param>
		/// <param name="BG">The background in which the animal will live.</param>
		/// <returns>The new animal as a cAnimal type.</returns>
		protected override cAnimal GetNewAnimal(cAnimalAttributes NewAnimal, cBackground BG)
		{
			// make sure parameters are not null
			if (NewAnimal == null) ThrowAnimalsException();
			if (BG == null) ThrowBackgroundException();
			// return a new fox
			return new cFox(NewAnimal, BG);
		}

		// *********************** private members ******************************************
		// exception raised if Animals parameter is null
		private void ThrowAnimalsException()
		{
			throw new ArgumentNullException("Animals", "Animals cannot be null.");
		}
		// throw an exception indicating a null background parameter
		private void ThrowBackgroundException() 
		{
			throw new ArgumentNullException("BG", "BG must not be null.");
		}
	}
}