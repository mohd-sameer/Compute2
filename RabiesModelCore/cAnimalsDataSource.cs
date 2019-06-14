using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A class for retrieving metadata stored in animal datasources.  All animal
	///		datasources should define this data.
	/// </summary>
	public class cAnimalsMetadata
	{
		/// <summary>
		///		A name given to the run that created the datasource.
		/// </summary>
        public string RunName { get; set; }
		/// <summary>
		///		Comments about the run that created the datasource.
		/// </summary>
        public string RunComments { get; set; }
		/// <summary>
		///		The name of the individual who created the datasource.
		/// </summary>
        public string CreatorName { get; set; }
		/// <summary>
		///		The organization the creator of the datasource is affiliated with.
		/// </summary>
        public string CreatorAffiliation { get; set; }
		/// <summary>
		///		Contact information for the creator of the datasource.
		/// </summary>
        public string CreatorContactInfo { get; set; }
		/// <summary>
		///		A list of the types of animals in the datasource.
		/// </summary>
        public string Animals { get; set; }
		/// <summary>
		///		The date and time the datasource was created.
		/// </summary>
        public string CreationDate { get; set; }
		/// <summary>
		///		The path to the datasource used at the start of the run that
		///		generated this datasource.
		/// </summary>
        public string StartingDatasource { get; set; }
		/// <summary>
		///		The type of random number generator used by the run that generated
		///		this datasource.
		/// </summary>
        public string RandomGenerator { get; set; }
		/// <summary>
		///		The random number seed used to seed the random number generator that
		///		was used to generate this datasource.
		/// </summary>
        public string RandomSeed { get; set; }
		/// <summary>
		///		The path to the cell datasource used by the run that generated this
		///		datasource.
		/// </summary>
        public string CellDatasourceLocation { get; set; }
		/// <summary>
		///		The name of the cell datasource used by the run that generated this
		///		datasource.
		/// </summary>
        public string CellDatasourceName { get; set; }
		/// <summary>
		///		The current population of animals in the animal datasource.
		/// </summary>
        public int TotalPopulation { get; set; }
		/// <summary>
		///		The first year that will be run when a trial is resumed from this animal datasource.
		/// </summary>
        public int NextYear { get; set; }
		/// <summary>
		///		The first week that will be run when a trial is resumed from this animal datasource.
		/// </summary>
        public int NextWeek { get; set; }
		/// <summary>
		///		A flag indicating whether or not disease is present in the naimals in this datasource.
		/// </summary>
        public bool DiseasePresent { get; set; }
	}
	
	/// <summary>
	///		An abstract base class for a class implementing a datasource for animals.  This
	///		class cannot directly instantiated.  It must be overridden to provide specific
	///		functionality for each datasource (and animal) type.
	///		The cAnimalsDataSource class implements the IDisposable interface; the dispose method
	///		calls the Disconnect method to free up any resources that may be tied up by the
	///		datasource.  The Dispose method will ignore an errors raised by the Disconnect
	///		method.
	/// </summary>
	public abstract class cAnimalsDataSource : IDisposable
	{
		// ************************** Constructor *****************************************
		/// <summary>
		///		Initialize an animals datasource.
		/// </summary>
        /// <param name="IsUnix">A boolean value indocating that this is a Unix based datasource</param>
        public cAnimalsDataSource(bool IsUnix)
        { 
            mvarConnected = false; 
			mvarIsUnix = IsUnix;
        }

		/// <summary>
		///		Destructor.  Disconnects the datasource if it is still connected.
		/// </summary>
		~cAnimalsDataSource()
		{
			this.Dispose();
		}
		
		// ************************* Properties ********************************************
		/// <summary>
		///		The name of the data source.
		/// </summary>
		public string DataSourceName { get; set; }

		/// <summary>
		///		The path to the data source.
		/// </summary>
		public string DataSourcePath { get; set; }

		/// <summary>
		///		Flag indicating that the data source is read only (read-only).
		/// </summary>
		public bool ReadOnly 
		{
			get 
			{
				return mvarReadOnly;
			}
		}

		/// <summary>
		///		Flag indicating whether or not the datasource is connected (read-only).
		/// </summary>
		public bool Connected
		{
			get 
			{
				return mvarConnected;
			}
		}

        /// <summary>
        ///     Get a boolean value indicating that this is a Unix based datasouce
        /// </summary>
        public bool IsUnix
        {
            get
            {
                return mvarIsUnix;
            }
        }

		// ************************* Methods ***********************************************
		/// <summary>
		///		Open a data source and connect to it.  If the datasource does not currently
		///		exist, it is created (only if the type of datasource supports this behaviour).
		///		You must set the values of the protected members mvarReadOnly and mvarConnected
		///		in your implementations of this method.
		/// </summary>
		/// <param name="ReadOnly">
		///		Flag indicating that the data source is opened for read only.
		/// </param>
		public abstract void Connect(bool ReadOnly);

		/// <summary>
		///		Disconnect from and close the data source.  You must set the value of the
		///		protected member mvarConnected in your implementation of this method.
		/// </summary>
		public abstract void Disconnect();

		/// <summary>
		///		Loads the animal and year data.
		/// </summary>
		/// <param name="Years">
		///		The list of years to be filled.  An ArgumentNullException exception is raised
		///		if Years is null.
		/// </param>
		/// <param name="Animals">
		///		The list of animals to be filled.  An ArgumentNullException exception is raised
		///		if Animals is null.
		///	</param>
		/// <param name="BG">
		///		The background that these animals will occupy.  An ArgumentNullException
		///		exception is raised if BG is null.  An InvalidOperationException exception is
		///		raised if name of the cell list in the datasource does not match the name of
		///		the cell list in the passed background.
		///	</param>
		public void LoadYearAndAnimalData(cYearList Years, cAnimalList Animals, 
											cBackground BG) 
		{
            //System.Diagnostics.Debug.WriteLine("cAnimalsDataSource.cs: LoadYearAndAnimalData");
            // make sure Years is not null
            if (Years == null) ThrowYearsException();
			// make sure Animals is not null
			if (Animals == null) ThrowAnimalsException();
			// make sure BG is not null
			if (BG == null) ThrowBGException();
			// make sure the cell list name of the data source matches the cell list
			// name of the cells belonging to the background.
			if (!this.CheckCellsName(BG.Cells)) {
				throw new InvalidOperationException("The cell list name associated with this animal data source does not match the cell list name associated with the background.");
			}
			// load the years
			this.GetYears(Years);
			// load the animals
			this.GetAnimalData(Animals, BG);
            //System.Diagnostics.Debug.WriteLine("cAnimalsDataSource.cs: LoadYearAndAnimalData END");
        }

		/// <summary>
		///		Write year and animal data to the data source.  All existing data in the
		///		data source is overwritten.
		/// </summary>
		/// <param name="Years">
		///		The list of years to write.  An ArgumentNullException exception is raised
		///		if Years is null.
		/// </param>
		/// <param name="Animals">
		///		The list of animals to write.  An ArgumentNullException exception is raised
		///		if Animals is null.
		///	</param>
		/// <param name="BG">
		///		The background that these animals occupy.  An ArgumentNullException
		///		exception is raised if BG is null.
		///	</param>
		public void WriteYearAndAnimalData(cYearList Years, cAnimalList Animals,
											cBackground BG)
		{            
            // make sure Years is not null
            if (Years == null) ThrowYearsException();            
            // make sure Animals is not null
            if (Animals == null) ThrowAnimalsException();            
            // make sure BG is not null
            if (BG == null) ThrowBGException();            
            // write the animal data
            this.WriteAnimalData(Animals, BG);            
            // write the name of the cell list
            this.WriteCellsName(BG.Cells);            
            // write the years
            this.WriteYears(Years);            
        }

		/// <summary>
		///		Get the version number for the currently opened datasource
		/// </summary>
		public int GetVersion()
		{
			// if version is already set, return that value, otherwise
			// read it from the datasource
			if (mvarVersionNumber == -1) {
				mvarVersionNumber = ReadVersionNumber();
			}
			return mvarVersionNumber;
		}

		/// <summary>
		///		Read animal data from the data source and fill the passed animal list.
		/// </summary>
		/// <param name="Animals">
		///		The animal list to fill.  The animals are added to any that are already in
		///		the list.
		/// </param>
		/// <param name="BG">The background that these animals will occupy.</param>
		public void GetAnimalData(cAnimalList Animals, cBackground BG)
		{
			cAnimal AnimalObj;
			if (Animals == null) ThrowAnimalsException();
			if (BG == null) ThrowBackgroundException();
			// go to the beginning of the list
			this.Reset();
			// get subtraction factor for year of birth
			// this.ReadTimeRecord(ref DataYear, ref DataWeek);
			cAnimalAttributes NewAnimal = new cAnimalAttributes();
			// loop through all animals in the list
			while (this.GetNextAnimalRecord(NewAnimal, BG)) 
			{
				// create the new animal
				AnimalObj = GetNewAnimal(NewAnimal, BG);
                AnimalObj.ListIndex = NewAnimal.ListIndex;
				// read markers for this animal if datasource is of appropriate version
				// number
				if (GetVersion() > 1) {
					ReadMarkers(NewAnimal);
					AnimalObj.AutoMarker = NewAnimal.AutoMarker;
					AnimalObj.Marker = NewAnimal.Marker;
				}
				// add the animal to the list
				Animals.Add(AnimalObj);
				// create a new animal attributes object so that we don't load the 
				// same data into all animals
				NewAnimal = new cAnimalAttributes();
				// advance to the next record in the dataset
				this.MoveToNextRecord();
			}
            // now reorder the list based on current list index
            Animals.ReorderByListIndex();
		}

		/// <summary>
		///		Retrieve metadata from the datasource.
		/// </summary>
		/// <param name="Metadata">
		///		The cAnimalsMetadata object into which the metadata is written.
		///	</param>
		public abstract void GetMetadata(cAnimalsMetadata Metadata);

		/// <summary>
		///		Write metadata to the datasource.  Any existing metadata is overwritten.
		/// </summary>
		/// <param name="Metadata">
		///		The cAnimalsMetadata object containing the metadata to be written.
		/// </param>
		public abstract void WriteMetadata(cAnimalsMetadata Metadata);
        
		/// <summary>
		///		Updates an existing animal data source.  Animals in the passed animal list but
		///		not in the datasource are added to the data source.  Animal in the list and in
		///		the data source are updated.
		/// </summary>
		/// <param name="Animals">
		///		The animal list containing the animals to be added and updated.
		///	</param>
		///	<param name="BG">The background that the animals occupy.</param>
		public void UpdateAnimalData(cAnimalList Animals, cBackground BG)
		{
			if (Animals == null) ThrowAnimalsException();
			if (BG == null) ThrowBGException();
			// throw an exception if data source is read only
			if (this.ReadOnly) ThrowReadOnlyException();
			// create a cAnimalAttributes object
			cAnimalAttributes Attributes = new cAnimalAttributes();
			// write the time stamp
			this.WriteTime(BG.Years.CurrentYearNum, BG.Years.CurrentYear.CurrentWeek,
				BG.HaveRunWeeklyEvents);
			// loop through all foxes, writing each one to the datasource
            for (int i = 0; i < Animals.Count; i++)
            {
				Animals[i].GetAttributes(Attributes);
                Attributes.ListIndex = i;
				this.WriteAnimalRecord(Attributes, true);
				// write markers if the version is correct
				if (GetVersion() > 1) this.WriteMarkers(Attributes, true);
			}
		}  

		/// <summary>
		///		Writes the animals in the passed animal list into the data source.  This 
		///		method overwrites any data that is already in the data source.
		/// </summary>
		/// <param name="Animals">
		///		The list of animals to be written into the datasource.
		/// </param>
		///	<param name="BG">The background that the animals occupy.</param>
		public void WriteAnimalData(cAnimalList Animals, cBackground BG)
		{
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData()");
            if (Animals == null) ThrowAnimalsException();
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 01");
            if (BG == null) ThrowBGException();
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 02");
            // throw an exception if data source is read only
            if (this.ReadOnly) ThrowReadOnlyException();
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 03");
            // erase the existing contents of this datasource
            this.Clear();
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 04");
            // write the time stamp
            this.WriteTime(BG.Years.CurrentYearNum, BG.Years.CurrentYear.CurrentWeek, BG.HaveRunWeeklyEvents);
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 05");
            // create a cAnimalAttributes object
            cAnimalAttributes Attributes = new cAnimalAttributes();
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 06");
            // loop through all foxes, writing each one to the datasource
            for (int i = 0; i < Animals.Count; i++)
            {
                //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() i = " + i);
                //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 06 i = " + i);
                Animals[i].GetAttributes(Attributes);
                //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 06 b");
                Attributes.ListIndex = i;                
                //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 06 c");
                this.WriteAnimalRecord(Attributes, false);
                //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 06 d");
                // write markers if the version is correct
                if (GetVersion() > 1) this.WriteMarkers(Attributes, false);
            }
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalData() HERE 07");
            // lastly, reset the datasource so that subsequent reads are correct
            this.Reset();
		}

		/// <summary>
		///		Write the actual file.  Note: for datasources based on databases, this method
		///		will do nothing.  It is good practice to call this method at the end of any
		///		datasource writing process to ensure that the datasource itself is updated.
		///	</summary>
		public virtual void WriteFile() {}
		
		/// <summary>
		///		Read year data from the datasource.
		/// </summary>
		/// <param name="WinterType">The winter type for the year.</param>
		/// <param name="CurrentWeek">The current week value for the year.</param>
		/// <returns>True if a year was available from the datasource.</returns>
		protected abstract bool GetYearData(ref enumWinterType WinterType, ref int CurrentWeek);

		/// <summary>
		///		Write year data into the datasource.
		/// </summary>
		/// <param name="Years">The year to write into the datasource.</param>
		protected abstract void WriteYearData(cYear Year);

		/// <summary>
		///		Remove all year data from the data source.
		/// </summary>
		protected abstract void ClearYears();

		/// <summary>
		///		Load a list of years from the data source.
		/// </summary>
		/// <param name="Years">
		///		The list of years to fill.  An ArgumentNullException exception is raised if
		///		Years is null.
		/// </param>
		public void GetYears(cYearList Years) 
		{
			// make sure Years is not null
			if (Years == null) ThrowYearsException();
			// get data
			enumWinterType WinterType = enumWinterType.Normal;
			int CurrentWeek = 0;
			int CurrentYear = 0;
			// clear the current years from the list.
			Years.Clear();
			// add years from the datasource, one-by-one
			while (GetYearData(ref WinterType, ref CurrentWeek)) {
				Years.Add(WinterType, CurrentWeek);
			}
			// get the current time stamp
			this.ReadTimeRecord(ref CurrentYear, ref CurrentWeek);
			// set the current year and week for the year list.
			Years.SetYearAndWeek(CurrentYear, CurrentWeek);
		}

		/// <summary>
		///		Write a list of years into the datasource.  Any years already in the data
		///		source are overwritten.
		/// </summary>
		/// <param name="Years">
		///		The list of years to write into the datasource.  An ArgumentNullException
		///		exception is raised if Years is null.
		///	</param>
		public void WriteYears(cYearList Years) 
		{
			// make sure Years is not null
			if (Years == null) ThrowYearsException();
			// remove the current years from the datasource
			this.ClearYears();
			// add the years to the list, one-by-one
			foreach (cYear Year in Years) {
				WriteYearData(Year);
			}
		}

		/// <summary>
		///		Dispose this object (calls Disconnect).
		/// </summary>
		public void Dispose()
		{
			// if we have already disposed, leave now!!
			if (IsDisposed) return;
			// disconnect
			try {
				Disconnect();
			}
			catch {
				return;
			}
			IsDisposed = true;
		}

		// ***************************** Protected Methods *********************************
		/// <summary>
		///		Checks to see if the name of the cell data source matches the cell data
		///		source name stored in the animals data source.
		/// </summary>
		/// <param name="Cells">The list of cells containing the name to be checked.</param>
		/// <returns>True if the name match.</returns>
		protected abstract bool CheckCellsName(cCellList Cells);
		
		/// <summary>
		///		Writes the name of the cells data source that these animals are associated 
		///		with.
		/// </summary>
		/// <param name="Cells">The list of cells with the name to be written.</param>
		protected abstract void WriteCellsName(cCellList Cells);

		/// <summary>
		///		Reads the version number of the data source from the data source
		/// </summary>
		protected abstract int ReadVersionNumber();

		/// <summary>
		///		Create a new specific animal type
		/// </summary>
		/// <param name="Animal">The animal attributes used to define the animal.</param>
		/// <param name="BG">The background in which the animal will live.</param>
		/// <returns>The new animal as a cAnimal type.</returns>
		protected abstract cAnimal GetNewAnimal(cAnimalAttributes NewAnimal, cBackground BG);

		/// <summary>
		///		Work out the time stamp for the current animal records and call the
		///		WriteTimeRecord function to actually write the time stamp to the datasource.
		/// </summary>
		/// <param name="Year">The year to write.</param>
		/// <param name="Week">The week to write.</param>
		/// <param name="HaveRunWeeklyEvents">
		///		A flag indicating whether or not the weekly events have been run
		///		for the passed time.
		/// </param>
		protected void WriteTime(int Year, int Week, bool HaveRunWeeklyEvents) 
		{
			// have the weekly events been run for the current year and week
			if (HaveRunWeeklyEvents) {
				// yes!! This is easy
				WriteTimeRecord(Year, Week);
			}
			else {
				// no!! We must subtract 1 week from the current time
				if (Week == 1) {
					if (Year > 0) {
						Week = 52;
						Year--;
					}
					else {
						Week = 0;
					}
				}
				else {
					Week--;
				}
				WriteTimeRecord(Year, Week);
			}
		}

		/// <summary>
		///		Retrieves a single Animal record from the data source and places it in the
		///		passed Animal data object.
		/// </summary>
		/// <param name="Data">
		///		The Animal data object that will contain the Animal data.
		/// </param>
		/// <returns>
		///		True if the retrieval was successful, False if there are no more records to
		///		retrieve.
		///	</returns>
		protected abstract bool GetNextAnimalRecord(cAnimalAttributes Data,
													cBackground Background);

		/// <summary>
		///		Write a single animal record into the database.
		/// </summary>
		/// <param name="Data">
		///		The animal attributes object containing the data to be written.
		///	</param>
		///	<param name="DeleteFirst">
		///		A flag indicating whether or not an existing record with the same ID should
		///		be deleted before the current data is written.
		///	</param>
		protected abstract void WriteAnimalRecord(cAnimalAttributes Data, bool DeleteFirst);

		/// <summary>
		///		Write markers for a single animal record into the database.
		/// </summary>
		/// <param name="Data">
		///		The animal attributes object containing the data to be written.
		///	</param>
		///	<param name="DeleteFirst">
		///		A flag indicating whether or not an existing record with the same ID should
		///		be deleted before the current data is written.
		///	</param>
		protected abstract void WriteMarkers(cAnimalAttributes Data, bool DeleteFirst);

		/// <summary>
		///		Read markers for a single animal record from the database.
		/// </summary>
		/// <param name="Data">
		///		The animal attributes object that will contain the marker data
		///	</param>
		protected abstract void ReadMarkers(cAnimalAttributes Data);

		/// <summary>
		///		Reset the datasource so that the next call to GetNextAnimalRecord returns the
		///		first Animal record.
		/// </summary>
		protected abstract void Reset();

		/// <summary>
		///		Erase all records from the animal datasource.
		/// </summary>
		protected abstract void Clear();

		/// <summary>
		///		Write the time stamp into the animal datasource.
		/// </summary>
		/// <param name="Year">The year to write.</param>
		/// <param name="Week">The week to write.</param>
		protected abstract void WriteTimeRecord(int Year, int Week);

		/// <summary>
		///		Read the time stamp from the animal datasource.
		/// </summary>
		/// <param name="Year">The year.</param>
		/// <param name="Week">The week.</param>
		protected abstract void ReadTimeRecord(ref int Year, ref int Week);

		/// <summary>
		///		Advance to the next record.
		/// </summary>
		protected abstract void MoveToNextRecord();
		
		/// <summary>
		///		Flag indicating that the datasource is read-only.
		/// </summary>
		protected bool mvarReadOnly;

		/// <summary>
		///		Flag indicating whether or not the datasource is connected.
		/// </summary>
		protected bool mvarConnected;

        /// <summary>
        ///     Flag indicating that this is a Unix based datasource
        /// </summary>
        protected bool mvarIsUnix;

		// ********************* Private Memebers *********************************
		private bool IsDisposed = false;
		// the version number for this data source (defaults to #.#), which indicates
		// that it must be read from the datasource
		private int mvarVersionNumber = -1;

		// exception raised if Years parameter is null
		private void ThrowYearsException()
		{
			throw new ArgumentNullException("Years", "Years cannot be null.");
		}
		// exception raised if Animals parameter is null
		private void ThrowAnimalsException()
		{
			throw new ArgumentNullException("Animals", "Animals cannot be null.");
		}
		// exception raised if BG (Background) parameter is null
		private void ThrowBGException()
		{
			throw new ArgumentNullException("BG", "BG (Background) cannot be null.");
		}
		// throw an exception indicating a null background parameter
		private void ThrowBackgroundException() 
		{
			throw new ArgumentNullException("BG", "BG must not be null.");
		}
		// throw an Exception indicating that the datasource is read-only
		private void ThrowReadOnlyException()
		{
			throw new InvalidOperationException("The datasource is open for reading only.");
		}

	}
}