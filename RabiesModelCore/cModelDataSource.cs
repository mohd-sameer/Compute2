using System;
using Random;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A class that represents a complete rabies model project datasource.  It's
	///		function is to provide the means for loading a complete rabies model from
	///		various datasources, leaving a cBackground object ready to run.  This object has
	///		two components, a Cell Datasource and an Animal Datasource.  These two
	///		components must be created and connected before they are used by this class.
	///		This object can also create an initial starting population by seeding a set
	///		of cells with a single female animal.  Note, this class MUST be overridden to
	///		provide the function for creating the initial single animal.
	/// </summary>
	public abstract class cModelDataSource
	{
		/// <summary>
		///		Create a read-only rabies model datasource combining a passed cells datasource 
		///		and a passed animals datasource.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the generated background</param>
        /// <param name="Cells">
		///		The cells datasource.  An ArgumentNullException exception is raised if Cells
		///		is null.
		///	</param>
		/// <param name="Animals">
		///		The animals datasource.  An ArgumentNullException exception is raised if Animals
		///		is null.
		///	</param>
		public cModelDataSource(cUniformRandom Rnd, cCellsDataSource Cells, cAnimalsDataSource Animals)
		{
			// set the values
			mvarCells = Cells;
			mvarAnimals = Animals;
            mvarRnd = Rnd;
			// make sure that they are valid
			CheckCells();
			CheckAnimals();
            CheckRandom();
		}

		/// <summary>
		///		Create a read-only rabies model datasource from a passed cells datasource,
		///		a passed wintertype list and a single animal.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the generated background</param>
        /// <param name="Cells">
		///		The cells datasource.  An ArgumentNullException exception is raised if Cells
		///		is null.
		///	</param>
		/// <param name="Winters">
		///		The list of winter types.  An ArgumentNullException exception is raised if
		///		Winters is null.  An ArgumentException exception is raised if Winter is an
		///		empty list.
		///	</param>
        ///	<param name="MaleMarkers">
        ///	    The markers for the seed male animal
        /// </param>
        /// <param name="FemaleMarkers">
        ///     The markers for the seed female animal
        /// </param>
		public cModelDataSource(cUniformRandom Rnd, cCellsDataSource Cells, cWinterTypeList Winters, 
                                string MaleMarkers, string FemaleMarkers)
		{
			// set the values
			mvarCells = Cells;
			mvarWinters = Winters;
            mvarRnd = Rnd;
			// make sure that they a valid
			CheckCells();
			CheckWinterTypes();
            CheckRandom();
            // get markers
            mvarMaleMarkers = MaleMarkers;
            mvarFemaleMarkers = FemaleMarkers;
        }

		/// <summary>
		///		Create a read-only rabies model datasource from a passed cells datasource with
		///		NYears years with the passed winter bias.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the generated background</param>
        /// <param name="Cells">
		///		The cells datasource.  An ArgumentNullException exception is raised if Cells
		///		is null.
		///	</param>
		/// <param name="NYears">
		///		The number of years to create.  An ArgumentOutOfRangeException exception is
		///		raised if NYears is less than one.
		///	</param>
		/// <param name="WinterBias">The winter type bias for those years.</param>
        ///	<param name="MaleMarkers">
        ///	    The markers for the seed male animal
        /// </param>
        /// <param name="FemaleMarkers">
        ///     The markers for the seed female animal
        /// </param>
        public cModelDataSource(cUniformRandom Rnd, cCellsDataSource Cells, int NYears, enumWinterType WinterBias,
                                string MaleMarkers, string FemaleMarkers)
		{
			// set the values
			mvarCells = Cells;
			mvarYears = NYears;
			mvarWinterBias = WinterBias;
            mvarRnd = Rnd;
			// make sure that they are valid
			CheckCells();
			CheckNYears();
            CheckRandom();
            // get markers
            mvarMaleMarkers = MaleMarkers;
            mvarFemaleMarkers = FemaleMarkers;
        }

		// ************************ Methods *************************************************
		/// <summary>
		///		Read the data from the datasource.
		/// </summary>
		/// <param name="BackgroundName">
		///		The name to assign to the returned background object.  If BackgroundName has
		///		zero length, an ArgumentException is raised.
		/// </param>
		/// <param name="KeepAllAnimals">
		///		A boolean value indicating whether or not the created background should keep
		///		a reference to animals that die.
		/// </param>
		/// <returns>
		///		A cBackground object containing the settings from this datasource.
		/// </returns>
		public cBackground ReadDatasource(string BackgroundName, bool KeepAllAnimals) 
		{
            //System.Diagnostics.Debug.WriteLine("cModelDataSource.cs: cBackground: ReadDataSource: without disease arguments");
            return ReadDatasource(BackgroundName, KeepAllAnimals, null);
		}

		/// <summary>
		///		Read the data from the datasource.
		/// </summary>
		/// <param name="BackgroundName">
		///		The name to assign to the returned background object.  If BackgroundName has
		///		zero length, an ArgumentException is raised.
		/// </param>
		/// <param name="KeepAllAnimals">
		///		A boolean value indicating whether or not the created background should keep
		///		a reference to animals that die.
		/// </param>
		/// <param name="Diseases">
		///		The list of Diseases that can occur in this background.  If diseases is null, a
		///		new empty disease list is created.
		/// </param>
		/// <returns>
		///		A cBackground object containing the settings from this datasource.
		/// </returns>
		public cBackground ReadDatasource(string BackgroundName, bool KeepAllAnimals, cDiseaseList Diseases)
		{            
            //System.Diagnostics.Debug.WriteLine("cModelDataSource.cs: cBackground: ReadDataSource: with disease arguments");

            cBackground NewBackground;
			// make sure BackgroundName is not zero length
			if (BackgroundName.Length == 0)
				throw new ArgumentException("BackgroundName must not be zero length.", "BackgroundName");
			// if the diseases list is null, create an empty list instead
			if (Diseases == null) Diseases = new cDiseaseList(mvarRnd);
			// we start by assigning years.  How we do this will depend on how this object
			// was originally created
            if (mvarAnimals != null)
            {
                // read the years from the animals datasource
                cYearList AnimalYears = new cYearList(1, mvarRnd);
                //mvarAnimals.GetYears(AnimalYears);
                // using these years, create a winter type list
                // create a background with this list of years
                //System.Diagnostics.Debug.WriteLine("cModelDataSource.cs: cBackground: mvarAnimals NULL = " + mvarYears);
                NewBackground = GetNewBackground(mvarRnd, BackgroundName, KeepAllAnimals, AnimalYears, Diseases);
            }
            else if (mvarYears > 0)
            {
                // create mvarYears years with a given winter bias
                //System.Diagnostics.Debug.WriteLine("cModelDataSource.cs: cBackground: mvarYears > 0 = " + mvarYears);
                NewBackground = GetNewBackground(mvarRnd, BackgroundName, KeepAllAnimals, mvarYears, mvarWinterBias);
                NewBackground.Diseases = Diseases;
            }
            else
            {
                // create years from a given winter type list
                //System.Diagnostics.Debug.WriteLine("cModelDataSource.cs: cBackground: mvarAnimals ? " + mvarYears);
                NewBackground = GetNewBackground(mvarRnd, BackgroundName, KeepAllAnimals, mvarWinters);
                NewBackground.Diseases = Diseases;
            }
			// now get the cell data.  This is the same no matter how the datasource 
			// was created
			// get the supercells first
			mvarCells.GetSuperCellData(NewBackground.SuperCells);
            // now get the cell data
            //System.Diagnostics.Debug.WriteLine("cModelDataSource.cs: cBackground: GetCellData");
            mvarCells.GetCellData(NewBackground.Cells, NewBackground.SuperCells);

			// now either read the animals data from the animals datasource OR create a single
			// new animal
			if (mvarAnimals != null) 
            {
                // read the animals
                // mvarAnimals.GetAnimalData(NewBackground.Animals, NewBackground);                
                mvarAnimals.LoadYearAndAnimalData(NewBackground.Years, NewBackground.Animals, NewBackground);
				// now read the time stamp and set the current year and week
			}
			else 
            {
				// seed a single male and female animal somewhere in the background
                int SeedCell = NewBackground.RandomNum.IntValue(0, NewBackground.Cells.Count - 1);
				// add the new female and male animals
                cAnimal NewFemale = GetNewAnimal("0", NewBackground.Cells[SeedCell].ID, NewBackground, enumGender.female);
                NewFemale.Marker = (string.IsNullOrEmpty(mvarFemaleMarkers) ? "M" : mvarFemaleMarkers);
				NewBackground.Animals.Add(NewFemale);

                cAnimal NewMale = GetNewAnimal("1", NewBackground.Cells[SeedCell].ID, NewBackground, enumGender.male);
                NewMale.Marker = (string.IsNullOrEmpty(mvarMaleMarkers) ? "M" : mvarMaleMarkers);
				NewBackground.Animals.Add(NewMale);
			}
            // return the newly create Background object
            //System.Diagnostics.Debug.WriteLine("cModelDataSource.cs: cBackground: ReadDatasource END");
            return NewBackground;
		}

		// ************************ Protected Members ***************************************
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
		protected abstract cAnimal GetNewAnimal(string ID, string CellID, cBackground Background, enumGender Gender);

        /// <summary>
        ///     Construct a new background object of the approriate type
        /// </summary>
        /// <param name="Rnd">The random number generator for the background</param>
        /// <param name="BackgroundName">The name of the background</param>
        /// <param name="KeepAllAnimals">A boolean value that indicates whether or not the background should retain a list of all animals</param>
        /// <param name="AnimalYears">A list of years</param>
        /// <param name="Diseases">A list of diseases in the background</param>
        /// <returns>A new background object ofthe approriate type</returns>
        protected abstract cBackground GetNewBackground(cUniformRandom Rnd, string BackgroundName, bool KeepAllAnimals,
                                                        cYearList AnimalYears, cDiseaseList Diseases);

        /// <summary>
        ///     Construct a new background object of the approriate type
        /// </summary>
        /// <param name="Rnd">The random number generator for the background</param>
        /// <param name="BackgroundName">The name of the background</param>
        /// <param name="KeepAllAnimals">A boolean value that indicates whether or not the background should retain a list of all animals</param>
        /// <param name="NYears">The number of years</param>
        /// <param name="WinterBias">The winter type bias for the years</param>
        /// <returns>A new background object ofthe approriate type</returns>
        protected abstract cBackground GetNewBackground(cUniformRandom Rnd, string BackgroundName, bool KeepAllAnimals,
                                                        int NYears, enumWinterType WinterBias);

        /// <summary>
        ///     Construct a new background object of the approriate type
        /// </summary>
        /// <param name="Rnd">The random number generator for the background</param>
        /// <param name="BackgroundName">The name of the background</param>
        /// <param name="KeepAllAnimals">A boolean value that indicates whether or not the background should retain a list of all animals</param>
        /// <param name="Winters">A list of winter types</param>
        /// <returns>A new background object ofthe approriate type</returns>
        protected abstract cBackground GetNewBackground(cUniformRandom Rnd, string BackgroundName, bool KeepAllAnimals, 
                                                        cWinterTypeList Winters);

		// ************************ Private Members *****************************************
		// the random number generator
        cUniformRandom mvarRnd;
        // the cells data source
		cCellsDataSource mvarCells;
		// the animals datasource
		cAnimalsDataSource mvarAnimals;
		// a list of winter types
		cWinterTypeList mvarWinters;
        // the markers for the seed male animal
        string mvarMaleMarkers;
        // the markers for the seed female animal
        string mvarFemaleMarkers;

		// the number of years desired
		int mvarYears = 0;
		// the winter bias
		enumWinterType mvarWinterBias = enumWinterType.Normal;

		// check the cells datasource object.  Throw an exception if it is null
		private void CheckCells()
		{
			if (mvarCells == null) 
				throw new ArgumentNullException("Cells", "Cells cannot be null.");
		}

		// check the animals datasource object.  Throw an exception if it is null
		private void CheckAnimals()
		{
			if (mvarAnimals == null)
				throw new ArgumentNullException("Animals", "Animals cannot be null.");
		}

        // check the random number generator object.  Throw an exception if it is null
        private void CheckRandom()
        {
            if (mvarRnd == null)
                throw new ArgumentNullException("Rnd", "Rnd cannot be null.");
        }

		// check the wintertype list.  Throw an exception if it is null or if it is zero length.
		private void CheckWinterTypes()
		{
			// the wintertype list cannot be null
			if (mvarWinters == null) 
				throw new ArgumentNullException("Winters", "Winters cannot be null.");
			// TheWinters must contain at least one value
			if (mvarWinters.Count == 0) 
				throw new ArgumentException("The winter type list must not be empty.", "Winters");
		}

		// check mvarYears.  Make sure it is > 0
		private void CheckNYears()
		{
			if (mvarYears <= 0) 
				throw new ArgumentOutOfRangeException("Years", 
						"The number of years desired must be greater than zero.");
		}
	}
}
