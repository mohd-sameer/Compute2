using System;
using Rabies_Model_Core;
using Random;

namespace Fox
{
	/// <summary>
	///		Fox specific implemetation of the Rabies Model datasource.  This class 
	///		simplifies the process of opening a complete project.	
	/// </summary>
	public class cFoxModelDataSource : cModelDataSource
	{
		/// <summary>
		///		Create a read-only rabies model datasource combining a passed cells datasource 
		///		and a passed animals datasource.
		/// </summary>
        /// <param name="Rnd">The random number generator used by the generated background</param>
		/// <param name="TheCells">The cells datasource.</param>
		/// <param name="TheAnimals">The animals datasource.</param>
		public cFoxModelDataSource(cUniformRandom Rnd, cCellsDataSource TheCells, cAnimalsDataSource TheAnimals)
			: base(Rnd, TheCells, TheAnimals) {}

		/// <summary>
		///		Create a read-only rabies model datasource from a passed cells datasource,
		///		a passed wintertype list and a single animal.
		/// </summary>
        /// <param name="Rnd">The random number generator used by the generated background</param>
        /// <param name="TheCells">The cells datasource.</param>
		/// <param name="TheWinters">The list of winter types.</param>
        ///	<param name="MaleMarkers">
        ///	    The markers for the seed male animal
        /// </param>
        /// <param name="FemaleMarkers">
        ///     The markers for the seed female animal
        /// </param>
		public cFoxModelDataSource(cUniformRandom Rnd, cCellsDataSource TheCells, cWinterTypeList TheWinters, 
                                       string MaleMarkers, string FemaleMarkers)
			: base(Rnd, TheCells, TheWinters, MaleMarkers, FemaleMarkers) {}

		/// <summary>
		///		Create a read-only rabies model datasource from a passed cells datasource with
		///		NYears years with the passed winter bias.
		/// </summary>
        /// <param name="Rnd">The random number generator used by the generated background</param>
        /// <param name="TheCells">The cells datasource.</param>
		/// <param name="NYears">The number of years to create.</param>
		/// <param name="WinterBias">The winter type bias for those years.</param>
        ///	<param name="MaleMarkers">
        ///	    The markers for the seed male animal
        /// </param>
        /// <param name="FemaleMarkers">
        ///     The markers for the seed female animal
        /// </param>
		public cFoxModelDataSource(cUniformRandom Rnd, cCellsDataSource TheCells, int NYears, enumWinterType WinterBias,
                                       string MaleMarkers, string FemaleMarkers)
			: base(Rnd, TheCells, NYears, WinterBias, MaleMarkers, FemaleMarkers) {}

		// ****************** protected members *******************************************
		/// <summary>
		///		Create a new animal in the passed background.
		/// </summary>
		/// <param name="ID">The ID of the new animal.</param>
		/// <param name="CellID">
		///		The ID of the cell containing the new animal.
		/// </param>
		/// <param name="Background">
		///		The background object that the animal will live in.
		///	</param>
		///	<param name="Gender">The gender of the new animal.</param>
		/// <returns>A reference to the newly created animal.</returns>
		protected override cAnimal GetNewAnimal(string ID, string CellID, 
												cBackground Background, enumGender Gender)
		{
			return new cFox(ID, CellID, Background, Gender);
		}

        /// <summary>
        ///     Construct a new background object of the approriate type
        /// </summary>
        /// <param name="Rnd">The random number generator for the background</param>
        /// <param name="BackgroundName">The name of the background</param>
        /// <param name="KeepAllAnimals">A boolean value that indicates whether or not the background should retain a list of all animals</param>
        /// <param name="AnimalYears">A list of years</param>
        /// <param name="Diseases">A list of diseases in the background</param>
        /// <returns>A new background object ofthe approriate type</returns>
        protected override cBackground GetNewBackground(cUniformRandom Rnd, string BackgroundName, bool KeepAllAnimals,
                                                        cYearList AnimalYears, cDiseaseList Diseases)
        {
            return new cFoxBackground(Rnd, BackgroundName, KeepAllAnimals, AnimalYears, Diseases);
        }

        /// <summary>
        ///     Construct a new background object of the approriate type
        /// </summary>
        /// <param name="Rnd">The random number generator for the background</param>
        /// <param name="BackgroundName">The name of the background</param>
        /// <param name="KeepAllAnimals">A boolean value that indicates whether or not the background should retain a list of all animals</param>
        /// <param name="NYears">The number of years</param>
        /// <param name="WinterBias">The winter type bias for the years</param>
        /// <returns>A new background object ofthe approriate type</returns>
        protected override cBackground GetNewBackground(cUniformRandom Rnd, string BackgroundName, bool KeepAllAnimals,
                                                        int NYears, enumWinterType WinterBias)
        {
            return new cFoxBackground(Rnd, BackgroundName, KeepAllAnimals, NYears, WinterBias);
        }

        /// <summary>
        ///     Construct a new background object of the approriate type
        /// </summary>
        /// <param name="Rnd">The random number generator for the background</param>
        /// <param name="BackgroundName">The name of the background</param>
        /// <param name="KeepAllAnimals">A boolean value that indicates whether or not the background should retain a list of all animals</param>
        /// <param name="Winters">A list of winter types</param>
        /// <returns>A new background object ofthe approriate type</returns>
        protected override cBackground GetNewBackground(cUniformRandom Rnd, string BackgroundName, bool KeepAllAnimals,
                                                        cWinterTypeList Winters)
        {
            return new cFoxBackground(Rnd, BackgroundName, KeepAllAnimals, Winters);
        }

	}
}
