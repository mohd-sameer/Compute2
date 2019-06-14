using System;
using Rabies_Model_Core;

namespace Raccoon
{
	/// <summary>
	///		A class representing a datasource containing Raccoon objects.
	/// </summary>
	public abstract class cRaccoonDataSource : cAnimalsDataSource
	{
		/// <summary>
		///		Initialize a Raccoon datasource.
		/// </summary>
        /// <param name="IsUnix">A boolean value indocating that this is a Unix based datasource</param>
        public cRaccoonDataSource(bool IsUnix) : base(IsUnix) { }

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
			// return a new raccoon
			return new cRaccoon(NewAnimal, BG);
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