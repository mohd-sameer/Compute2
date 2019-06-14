using System;
using System.Collections.Specialized;
using System.Data;
using Rabies_Model_Core;
using System.IO;
using System.Net;

namespace Fox
{
	/// <summary>
	///		Enumeration describing field name types in the XML file.  Long names are readable
	///		but result in a large XML file.  Short names are single letter, difficult to
	///		read in the file, but giving much smaller XML files
	/// </summary>
	public enum enumFieldNameSize 
	{
		/// <summary>
		///		Long descriptive field names.
		/// </summary>
		LongNames = 0,
		/// <summary>
		///		Short, single letter field names
		/// </summary>
		ShortNames = 1
	};

	/// <summary>
	///		Summary description for cXMLFoxDataSource.
	/// </summary>
	public class cXMLFoxDataSource : cFoxDataSource
	{
		/// <summary>
		///		Initialize an XML based fox data source.  Fieldnames are long by default.
		///		If an existing XML datasource is opened, field name size will be set to the
		///		field name size of that XML datasource.
		/// </summary>
        /// <param name="IsUnix">A boolean value indicating that this is a Unix based datasource</param>
		public cXMLFoxDataSource(bool IsUnix) : base(IsUnix)
		{            
            mvarFieldSize = enumFieldNameSize.LongNames;
		}

		/// <summary>
		///		Initialize an XML based fox data source, specifying the field name size.
		///		Opening an existing XML datasource will override this value.
		/// </summary>
		/// <param name="FieldSize">
		///		The desired field name size.  This can be LongNames or ShortNames.
		/// </param>
        /// <param name="IsUnix">A boolean value indocating that this is a Unix based datasource</param>
        public cXMLFoxDataSource(enumFieldNameSize FieldNameSize, bool IsUnix)
            : base(IsUnix)
		{            
            mvarFieldSize = FieldNameSize;
		}

		// ************************ Properties *********************************************
		/// <summary>
		///		The field name size currently in use (read-only).
		/// </summary>
		public enumFieldNameSize FieldNameSize 
		{
			get 
			{
				return mvarFieldSize;
			}
		}

		/// <summary>
		///		Write the XML file to the disk.  An InvalidOperationException exception is
		///		thrown if the datasource is open for reading only or if the datasource is not
		///		connected.
		/// </summary>
		public override void WriteFile() 
		{
			if (!mvarConnected) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			// store the appropriate field name size
			DataTable dt = mvarDataSet.Tables["NameSize"];
			// start by eliminating any existing data
			if (dt.Rows.Count > 0) dt.Rows[0].Delete();
			// write the new data
			DataRow dr = dt.NewRow();
			dr["NameSize"] = (int) mvarFieldSize;
			dt.Rows.Add(dr);
			// write the file
            string MyPath = this.DataSourcePath;
			if (MyPath.Length > 0)
			{
	            if (this.IsUnix)
	            {
	                if (!MyPath.EndsWith("/")) MyPath = string.Format("{0}/", MyPath);
	            }
	            else
	            {
	                if (!MyPath.EndsWith(@"\")) MyPath = string.Format(@"{0}\", MyPath);
	            }
			}
			string FullName = string.Format("{0}{1}", MyPath, this.DataSourceName);
			mvarDataSet.WriteXml(FullName, System.Data.XmlWriteMode.WriteSchema);            
		}

		/// <summary>
		///		Connect to the xml datasource.  If the datasource is opened for read only, the
		///		file path and name specified by the DataSourcePath and DataSourceName
		///		properties must point to an existing file or a FileNotFoundException exception
		///		will result.  If datasource name points to an internet URL (i.e. is begins with
		///		'http://'), the datasource will be a read-only datasource.
		/// </summary>
		/// <param name="ReadOnly">
		///		A flag indicating that the datasource should be opened as read only.  If the 
		///		datasource is internet based, the value of this parameter is ignored; the
		///		datasource will always be read-only.
		///	</param>
		public override void Connect(bool ReadOnly) 
		{            
            if (this.DataSourceName.ToLower().StartsWith("http://")) 
			{
                // create the dataset from an internet based source                
                ReadOnly = true;
				CreateDataset();
			}
			else 
			{
				// create full path and name
                string MyPath = this.DataSourcePath;
				if (MyPath.Length > 0)
				{
	                if (this.IsUnix)
	                {
	                    if (!MyPath.EndsWith("/")) MyPath = string.Format("{0}/", MyPath);
	                }
	                else
	                {
	                    if (!MyPath.EndsWith(@"\")) MyPath = string.Format(@"{0}\", MyPath);
	                }
				}
				string FullName = string.Format("{0}{1}", MyPath, this.DataSourceName);
                
                // does the file exist?
                FileInfo DBSourceInfo = new FileInfo(FullName);
                
                // if it does not, create it if the datasource was opened for read-write.  If the
                // datasource was created for read-only, raise an exception.
                if (!DBSourceInfo.Exists) 
				{
					if (ReadOnly) 
					{
                        
                        throw new FileNotFoundException(string.Format("Fox datasource \"{0}\" not found!!", FullName),
														FullName);                        
                    }
					else 
					{

                        // create an empty data source                        
                        CreateEmptyDataSource();                        
                    }                    
                }
					// else the datasource is found!! Open it
				else 
				{

                    // now open the data source                    
                    CreateDataset();                    
                }
			}
			// set initial reading positions
			ReadPosition = 0;
            CellPosition = 0;
            OffspringPosition = 0;
            VaccinePosition = 0;
            DiseasePosition = 0;
			YearReadPosition = 0;
			mvarReadOnly = ReadOnly;
			mvarConnected = true;            
        }

		/// <summary>
		///		Disconnect from the fox datasource.
		/// </summary>
		public override void Disconnect() 
		{
			if (mvarConnected) 
			{
				// close the recordsets
				mvarDataSet.Relations.Clear();
				try { mvarDataSet.Clear(); }
				catch {}
				mvarConnected = false;
			}
		}

		// *********************** protected members ****************************************
		/// <summary>
		///		Checks to see if the name of the cell data source matches the cell data
		///		source name stored in the animals data source.  An InvalidOperationException
		///		exception is raised if the datasource is not currently connected.
		/// </summary>
		/// <param name="Cells">
		///		The list of cells whose name shall be checked.  An ArgumentNullException
		///		exception is raised if Cells is null.
		///	</param>
		/// <returns>True if the names match.</returns>
		protected override bool CheckCellsName(cCellList Cells) 
		{
			if (!mvarConnected) ThrowConnectionException();
			if (Cells == null) ThrowCellsException();
			// get the CellsName table
			DataTable dt = mvarDataSet.Tables[TName["CellsName"]];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[0];
			// check the cells name
			bool CheckValue = (Convert.ToString(dr[FName["CellsName"]]) == Cells.Name);
			return CheckValue;
		}
		
		/// <summary>
		///		Writes the name of the cells data source that these animals are associated 
		///		with.  An InvalidOperationException exception is raised if the datasource is
		///		not currently connected.
		/// </summary>
		/// <param name="Cells">
		///		The name of the cells that the animals occupy.  An ArgumentNullException
		///		exception is raised if Cells is null.
		///	</param>
		protected override void WriteCellsName(cCellList Cells)
		{
			if (!mvarConnected) ThrowConnectionException();
			if (Cells == null) ThrowCellsException();
			// write the cells
			DataTable dt = mvarDataSet.Tables[TName["CellsName"]];
			// start by eliminating any existing data
			if (dt.Rows.Count > 0) dt.Rows[0].Delete();
			// write the new data
			DataRow dr = dt.NewRow();
			dr[FName["CellsName"]] = Cells.Name;
			dt.Rows.Add(dr);
		}

		/// <summary>
		///		Read summary information about the run used to create the datasource. An
		///		InvalidOperationException exception is raised if the datasource has not been
		///		connected.
		/// </summary>
		/// <param name="Metadata">
		///		The metadata object into which the metadata is written.  An
		///		ArgumentNullException exception is raised if Metadata is null.
		///	</param>
		public override void GetMetadata(cAnimalsMetadata Metadata)
		{
			if (!mvarConnected) ThrowConnectionException();
			if (Metadata == null) ThrowMetadataException();
			// get the RunInfo Table
			DataTable dt = mvarDataSet.Tables[TName["RunInfo"]];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[0];
			// fill the Metadata structure
			Metadata.RunName = Convert.ToString(dr[FName["RunName"]]);
			Metadata.RunComments = Convert.ToString(dr[FName["RunComments"]]);
			Metadata.CreationDate = Convert.ToString(dr[FName["RunDate"]]);
			Metadata.CreatorName = Convert.ToString(dr[FName["RunPerson"]]);
			Metadata.CreatorAffiliation = Convert.ToString(dr[FName["CreatorAffiliation"]]);
			Metadata.CreatorContactInfo = Convert.ToString(dr[FName["CreatorContactInfo"]]);
			Metadata.Animals = Convert.ToString(dr[FName["Animals"]]);
			Metadata.StartingDatasource = Convert.ToString(dr[FName["StartingDatasource"]]);
			Metadata.RandomGenerator = Convert.ToString(dr[FName["RandomGenerator"]]);
			Metadata.RandomSeed = dr[FName["RandomSeed"]].ToString();
			Metadata.CellDatasourceName = Convert.ToString(dr[FName["CellDatasourceName"]]);
			Metadata.CellDatasourceLocation = Convert.ToString(dr[FName["CellDatasourceLocation"]]);
			Metadata.TotalPopulation = Convert.ToInt32(dr[FName["TotalPopulation"]]);
			Metadata.NextYear = Convert.ToInt32(dr[FName["NextYear"]]);
			Metadata.NextWeek = Convert.ToInt32(dr[FName["NextWeek"]]);
			Metadata.DiseasePresent = Convert.ToBoolean(dr[FName["DiseasePresent"]]);
		}

		/// <summary>
		///		Write summary information about the run used to create a datasource. As well
		///		as the passed parameters, the date and time of the run are also written into
		///		the datasource. This method will overwrite any metadata that is already in the 
		///		datasource.  An InvalidOperationException exception is raised if the datasource
		///		is open for reading only or if the datasource has not been connected.
		/// </summary>
		/// <param name="Metadata">
		///		The metadata to be written into the datasource.  An ArgumentNullException
		///		exception is raised if Metadata is null.
		///	</param>
		public override void WriteMetadata(cAnimalsMetadata Metadata)
		{
			if (!mvarConnected) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Metadata == null) ThrowMetadataException();
			// write the metadata
			DataTable dt = mvarDataSet.Tables[TName["RunInfo"]];
			// start by eliminating any existing data
			if (dt.Rows.Count > 0) dt.Rows[0].Delete();
			// write the new metadata
			DataRow dr = dt.NewRow();
			dr[FName["RunName"]] = Metadata.RunName;
			dr[FName["RunComments"]] = Metadata.RunComments;
			dr[FName["RunDate"]] = DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
			dr[FName["RunPerson"]] = Metadata.CreatorName;
			dr[FName["CreatorAffiliation"]] = Metadata.CreatorAffiliation;
			dr[FName["CreatorContactInfo"]] = Metadata.CreatorContactInfo;
			dr[FName["Animals"]] = Metadata.Animals;
			dr[FName["StartingDatasource"]] = Metadata.StartingDatasource;
			dr[FName["RandomGenerator"]] = Metadata.RandomGenerator;
			dr[FName["RandomSeed"]] = Metadata.RandomSeed;
			dr[FName["CellDatasourceName"]] = Metadata.CellDatasourceName;
			dr[FName["CellDatasourceLocation"]] = Metadata.CellDatasourceLocation;
			dr[FName["TotalPopulation"]] = Metadata.TotalPopulation;
			dr[FName["NextYear"]] = Metadata.NextYear;
			dr[FName["NextWeek"]] = Metadata.NextWeek;
			dr[FName["DiseasePresent"]] = Metadata.DiseasePresent;
			// write the current version number if such a field exists
			if (dt.Columns.Contains(FName["Version"])) dr[FName["Version"]] = 2;
			dt.Rows.Add(dr);
		}

		/// <summary>
		///		Reads the version number of the data source from the database
		/// </summary>
		protected override int ReadVersionNumber()
		{
			int Value;
			if (!mvarConnected) ThrowConnectionException();
			// get the RunInfo Table
			DataTable dt = mvarDataSet.Tables[TName["RunInfo"]];
			// is there a datarow
			if (dt.Rows.Count > 0) 
			{
				// does the data row have a version field.  If it does, get the version
				// if it does not, default to version 1
				if (dt.Columns.Contains(FName["Version"])) 
				{
					// get the datarow at the current read position
					DataRow dr = dt.Rows[0];
					Value = Convert.ToInt32(dr[FName["Version"]]);
				}
				else 
				{
					Value = 1;
				}
			}
			else 
			{
				// no datarow found.  This must be a brand new datasource - it must be version 2
				Value = 2;
			}
			return Value;
		}

		/// <summary>
		///		Write a single animal attributes record to the datasource.  The record is
		///		either added to the datasource or updated in the datasource if it already
		///		exists.  An InvalidOperationException exception is raised if the datasource
		///		is open for reading only or if the datasource has not been connected.
		/// </summary>
		/// <param name="Attributes">
		///		The attributes to add or update.  An ArgumentNullException exception is raised
		///		if Attributes is null.
		///	</param>
		///	<param name="DeleteFirst">
		///		A flag indicating whether or not an existing record with the same ID should
		///		be deleted before the current data is written.
		///	</param>
		protected override void WriteAnimalRecord(cAnimalAttributes Attributes, bool DeleteFirst)
		{
            
            int i;
			if (!mvarConnected) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Attributes == null) ThrowAnimalAttributesException();
			// reference the animal table
			DataTable dt = mvarDataSet.Tables[TName["Animals"]];
			// are we creating a new row or updating an existing row
			DataRow dr;
			if (DeleteFirst) 
			{
				// delete old data first if requested
				DataRow[] DataRows = dt.Select("ID = " + Attributes.ID);
				if (DataRows.GetLength(0) > 0) DataRows[0].Delete();
			}
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalRecord() HERE 01");
            dr = dt.NewRow();
			// write the data
			dr.BeginEdit();
			dr[FName["ID"]] = Attributes.ID;
			dr[FName["Age"]]= Attributes.Age;
			if (Attributes.ParentID.Length > 0) 
				dr[FName["Parent_ID"]] = Attributes.ParentID;
			else
				dr[FName["Parent_ID"]] = -1;
			dr[FName["Gender"]] = Convert.ToInt32(Attributes.Gender);
			dr[FName["IsAlive"]] = Attributes.IsAlive;
			dr[FName["YearBorn"]] = Attributes.Cells[0].Year;
			dr[FName["WeekBorn"]] = Attributes.Cells[0].Week;
			dr[FName["YearDied"]] = Attributes.YearDied;
			dr[FName["WeekDied"]] = Attributes.WeekDied;
			dr[FName["IsIndependent"]] = Attributes.IsIndependent;
			dr[FName["CannotGiveBirth"]] = Attributes.CannotGiveBirth;                                 
            dr[FName["ListIndex"]] = Attributes.ListIndex;
			dt.Rows.Add(dr);
			// update the cells table
			dt = mvarDataSet.Tables[TName["Cells"]];
			// delete old data first if requested
			if (DeleteFirst) 
			{
				DataRow[] DataRows = dt.Select("Animal_ID = " + Attributes.ID);
				for (i = 0; i < DataRows.GetLength(0); i++) DataRows[i].Delete();
			}
			foreach (cCellTime CT in Attributes.Cells) 
			{
				dr = dt.NewRow();
				dr[FName["Animal_ID"]] = Attributes.ID;
				dr[FName["Cell_ID"]] = CT.CellID;
				dr[FName["Year"]] = CT.Year;
				dr[FName["Week"]] = CT.Week;
				dt.Rows.Add(dr);
			}
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalRecord() HERE 02");
            // update the offspring table
            dt = mvarDataSet.Tables[TName["Offspring"]];
			// delete old data first if requested
			if (DeleteFirst) 
			{
				DataRow[] DataRows = dt.Select("Animal_ID = " + Attributes.ID);
				for (i = 0; i < DataRows.GetLength(0); i++) DataRows[i].Delete();
			}
			foreach (string OffspringID in Attributes.Offspring) 
			{
				dr = dt.NewRow();
				dr[FName["Animal_ID"]] = Attributes.ID;
				dr[FName["Offspring_ID"]] = OffspringID;
				dt.Rows.Add(dr);
			}
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalRecord() HERE 03");
            // update the disease table
            dt = mvarDataSet.Tables[TName["Disease"]];
			// delete old data first if requested
			if (DeleteFirst) 
			{
				DataRow[] DataRows = dt.Select("Animal_ID = " + Attributes.ID);
				for (i = 0; i < DataRows.GetLength(0); i++) DataRows[i].Delete();
			}
			foreach (cInfection Inf in Attributes.Infections) 
			{
				dr = dt.NewRow();
				dr[FName["Animal_ID"]] = Attributes.ID;
				dr[FName["Disease_Name"]] = Inf.Disease.Name;
				dr[FName["Infection_Year"]] = Inf.Year;
				dr[FName["Infection_Week"]] = Inf.Week;
				dr[FName["IncubationPeriod"]] = Inf.IncubationPeriod;
				dr[FName["InfectiousPeriod"]] = Inf.InfectiousPeriod;
                dr[FName["InfectingAnimalID"]] = string.IsNullOrEmpty(Inf.InfectingAnimalID) ? "-1" : Inf.InfectingAnimalID;
                dr[FName["Natural_Immunity"]] = Inf.NaturalImmunity;                                  
                dt.Rows.Add(dr);
			}
            //System.Diagnostics.Debug.WriteLine("cAnimalDataSource.cs: WriteAnimalRecord() HERE 04");
            // update the vaccines table
            dt = mvarDataSet.Tables[TName["Vaccines"]];
			// delete old data first if requested
			if (DeleteFirst) 
			{
				DataRow[] DataRows = dt.Select("Animal_ID = " + Attributes.ID);
				for (i = 0; i < DataRows.GetLength(0); i++) DataRows[i].Delete();
			}
			foreach (cVaccine Vaccine in Attributes.Vaccines) 
			{
				dr = dt.NewRow();
				dr[FName["Animal_ID"]] = Attributes.ID;
				dr[FName["Disease_Name"]] = Vaccine.DiseaseName;
				dr[FName["Vaccinate_Year"]] = Vaccine.Year;
				dr[FName["Vaccinate_Week"]] = Vaccine.Week;
				dr[FName["Effective_Period"]] = Vaccine.EffectivePeriod;
				dt.Rows.Add(dr);
			}
		}

		/// <summary>
		///		Get the next record in the fox data source.  An InvalidOperationException
		///		exception is raised if the datasource has not been connected.
		/// </summary>
		/// <param name="Attributes">
		///		The attributes to add or update.  An ArgumentNullException exception is raised
		///		if Attributes is null.
		///	</param>
		/// <param name="Background">
		///		The background object in which this animal will live.  An ArgumentNullException
		///		exception is raised if Background is null.
		/// </param>
		/// <returns>False if the last record has already been read.</returns>
		protected override bool GetNextAnimalRecord(cAnimalAttributes Attributes, cBackground Background) 
		{
			if (!mvarConnected) ThrowConnectionException();
			if (Attributes == null) ThrowAnimalAttributesException();
			if (Background == null) ThrowBackgroundException();
			// return false if we have reached the end of the record set
            DataTable dt = mvarDataSet.Tables[TName["Animals"]];
			if (ReadPosition == dt.Rows.Count) return false;
			// copy the data to the Attributes object
            DataRow dr = dt.DefaultView[ReadPosition].Row;
            // get the basic animal data
            Attributes.ID = dr[FName["ID"]].ToString();
            Attributes.ParentID = dr[FName["Parent_ID"]].ToString();
            Attributes.Gender = ((byte)dr[FName["Gender"]] == 0 ? enumGender.female : enumGender.male);
            Attributes.IsAlive = (bool)dr[FName["IsAlive"]];
            Attributes.IsIndependent = (bool)dr[FName["IsIndependent"]];            
            Attributes.YearDied = (int)dr[FName["YearDied"]];
            Attributes.WeekDied = (int)dr[FName["WeekDied"]];
            Attributes.Age = (int)dr[FName["Age"]];
            if (dt.Columns.Contains(FName["ListIndex"])) Attributes.ListIndex = (int)dr[FName["ListIndex"]];
            try 
			{
                if (dt.Columns.Contains(FName["CannotGiveBirth"]))
                {
                    Attributes.CannotGiveBirth = (dr[FName["CannotGiveBirth"]] != DBNull.Value ? (int)dr[FName["CannotGiveBirth"]] : 0);
                }
                else
                {
                    Attributes.CannotGiveBirth = 0;
                }
			}
			catch 
			{
                // keep the catch, just in case
				Attributes.CannotGiveBirth = 0;
			}

            // get the cells
            for (; CellPosition < mvarDataSet.Tables[TName["Cells"]].Rows.Count; CellPosition++)
            {
                DataRow drChild = mvarDataSet.Tables[TName["Cells"]].DefaultView[CellPosition].Row;
                if (drChild[FName["Animal_ID"]].ToString() != Attributes.ID) break;
                cCellTime CT = new cCellTime();
                CT.CellID = drChild[FName["Cell_ID"]].ToString();
                CT.Year = (int)drChild[FName["Year"]];
                CT.Week = (int)drChild[FName["Week"]];
                Attributes.Cells.Add(CT);
            }
            // get the offspring
            for (; OffspringPosition < mvarDataSet.Tables[TName["OffSpring"]].Rows.Count; OffspringPosition++)
            {
                DataRow drChild = mvarDataSet.Tables[TName["OffSpring"]].DefaultView[OffspringPosition].Row;
                if (drChild[FName["Animal_ID"]].ToString() != Attributes.ID) break;
                Attributes.Offspring.Add(drChild[FName["Offspring_ID"]].ToString());
            }
            // get the diseases
            for (; DiseasePosition < mvarDataSet.Tables[TName["Disease"]].Rows.Count; DiseasePosition++)
            {
                DataRow drChild = mvarDataSet.Tables[TName["Disease"]].DefaultView[DiseasePosition].Row;
                if (drChild[FName["Animal_ID"]].ToString() != Attributes.ID) break;
                string DiseaseName = drChild[FName["Disease_Name"]].ToString();
                int Year = (int)drChild[FName["Infection_Year"]];
                int Week = (int)drChild[FName["Infection_Week"]];
                int IncubationPeriod = (int)drChild[FName["IncubationPeriod"]];
                int InfectiousPeriod = (int)drChild[FName["InfectiousPeriod"]];
                string InfectingAnimalID = drChild[FName["InfectingAnimalID"]].ToString();
                string NaturalImmunity = drChild[FName["Natural_Immunity"]].ToString();                
                Attributes.Infections.Add(new cInfection(Background, DiseaseName, Year, Week, IncubationPeriod, 
                                                         InfectiousPeriod, InfectingAnimalID));
            }
            // get the vaccines
            for (; VaccinePosition < mvarDataSet.Tables[TName["Vaccines"]].Rows.Count; VaccinePosition++)
            {
                DataRow drChild = mvarDataSet.Tables[TName["Vaccines"]].DefaultView[VaccinePosition].Row;
                if (drChild[FName["Animal_ID"]].ToString() != Attributes.ID) break;
                string DiseaseName = drChild[FName["Disease_Name"]].ToString();
                int Year = (int)drChild[FName["Vaccinate_Year"]];
                int Week = (int)drChild[FName["Vaccinate_Week"]];
                int EffectivePeriod = (int)drChild[FName["Effective_Period"]];
                Attributes.Vaccines.Add(new cVaccine(DiseaseName, Year, Week, EffectivePeriod));
            }
			// return true, indicating that a value was read
			return true;
		}

		/// <summary>
		///		Read markers for a single animal record from the database.
		/// </summary>
		/// <param name="Data">
		///		The animal attributes object that will contain the marker data
		///	</param>
		protected override void ReadMarkers(cAnimalAttributes Data)
		{
			if (Data == null) ThrowAnimalAttributesException();
			DataTable dt = mvarDataSet.Tables[TName["Markers"]];
			// throw exception if we have reached the end of the record set
			if (ReadPosition < dt.Rows.Count) 
			{
				// find the row in the Markers tables containing the appropriate animal ID
                DataRow dr = dt.DefaultView[ReadPosition].Row;
                if (dr[FName["Animal_ID"]].ToString() == Data.ID)
                {
                    // get the marker data
                    Data.AutoMarker = dr[FName["AutoMarker"]].ToString();
                    Data.Marker = dr[FName["Marker"]].ToString();
                    Data.PartnerMarker = dr[FName["PartnerMarker"]].ToString();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Error reading markers.  Was looking for animal ID {0}, but found ID {1} instead", Data.ID, dr[FName["Animal_ID"]].ToString()));
                }
			}
            else
            {
                throw new InvalidOperationException("Error reading markers.  Not enough markers are available.  There should be one set for each animal");
            }
		}

		/// <summary>
		///		Move to the next record in the database
		/// </summary>
		protected override void MoveToNextRecord()
		{
			// increment the current read position
			ReadPosition++;
		}

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
		protected override void WriteMarkers(cAnimalAttributes Data, bool DeleteFirst)
		{
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Data == null) ThrowAnimalAttributesException();
			DataTable dt = mvarDataSet.Tables[TName["Markers"]];
			// are we creating a new row or updating an existing row
			DataRow dr;
			if (DeleteFirst) 
			{
				DataRow[] DataRows = dt.Select("Animal_ID = " + Data.ID);
				if (DataRows.GetLength(0) > 0) DataRows[0].Delete();
			}
			dr = dt.NewRow();
			// write the data
			dr.BeginEdit();
			dr[FName["Animal_ID"]] = Data.ID;
			dr[FName["AutoMarker"]] = Data.AutoMarker;
			dr[FName["Marker"]] = Data.Marker;
			dr[FName["PartnerMarker"]] = Data.PartnerMarker;
			dt.Rows.Add(dr);
		}

		/// <summary>
		///		Delete all records from this fox datasource.  An InvalidOperationException
		///		exception is raised if the datasource is open for reading only or if the
		///		datasource has not been connected.
		/// </summary>
		protected override void Clear()
		{
			if (!mvarConnected) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			// create a new set of empty data tables!!
			CreateEmptyDataSource();
		}

		/// <summary>
		///		Reset the datasource so that it points to the first fox in the datasource.
		///		An InvalidOperationException exception is raised if the datasource has not been
		///		connected.
		/// </summary>
		protected override void Reset() 
		{
			if (!mvarConnected) ThrowConnectionException();
			ReadPosition = 0;
            CellPosition = 0;
            OffspringPosition = 0;
            VaccinePosition = 0;
            DiseasePosition = 0;
			YearReadPosition = 0;
		}

		/// <summary>
		///		Write a time record into the datasource.  An InvalidOperationException exception
		///		is raised if the datasource is open for reading only or if the datasource has
		///		not been connected.
		/// </summary>
		/// <param name="Year">
		///		The year to write.  An ArgumentOutOfRangeException exception is raised if Year
		///		is less than zero.
		///	</param>
		/// <param name="Week">
		///		The week to write.  An ArgumentOutOfRangeException exception is raised if Week
		///		is less than zero.
		///	</param>
		protected override void WriteTimeRecord(int Year, int Week) 
		{
			if (!mvarConnected) ThrowConnectionException();
			if (Year < 0)
				throw new ArgumentOutOfRangeException("Year", "Year cannot be less than zero.");
			if (Week < 1 || Week > 52)
				throw new ArgumentOutOfRangeException("Week", "Week must be in the range of 1-52.");
			if (this.ReadOnly) ThrowReadOnlyException();
			// write time record
			DataTable dt = mvarDataSet.Tables[TName["TimeStamp"]];
			// remove old record
			if (dt.Rows.Count > 0) dt.Rows[0].Delete();
			// add new record
			DataRow dr = dt.NewRow();
			dr[FName["Year"]] = Convert.ToInt32(Year);
			dr[FName["Week"]] = Convert.ToInt32(Week);
			dt.Rows.Add(dr);
		}

		/// <summary>
		///		Read the time record from the animal database.  An InvalidOperationException
		///		exception is raised if the datasource has not been connected.
		/// </summary>
		/// <param name="Year">The year value in the database.</param>
		/// <param name="Week">The week value in the database.</param>
		protected override void ReadTimeRecord(ref int Year, ref int Week) 
		{
			if (!mvarConnected) ThrowConnectionException();
			DataTable dt = mvarDataSet.Tables[TName["TimeStamp"]];
			DataRow dr = dt.Rows[0];
			// get the values
			Year = Convert.ToInt32(dr[FName["Year"]]);
			Week = Convert.ToInt32(dr[FName["Week"]]);
		}

		/// <summary>
		///		Read year data from the datasource.  An InvalidOperationException exception is
		///		raised if the datasource has not been connected.
		/// </summary>
		/// <param name="WinterType">The winter type for the year.</param>
		/// <param name="CurrentWeek">The current week value for the year.</param>
		/// <returns>True if a year was available from the datasource.</returns>
		protected override bool GetYearData(ref enumWinterType WinterType, ref int CurrentWeek) 
		{            
            if (!mvarConnected) ThrowConnectionException();
			// return false if we have reached the end of the record set
			if (YearReadPosition == mvarDataSet.Tables[TName["Years"]].Rows.Count) return false;
			// copy the data to the Attributes object
			DataTable dt = mvarDataSet.Tables[TName["Years"]];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[YearReadPosition];
			// get the year data
			WinterType = (enumWinterType) Convert.ToInt32(dr[FName["WinterType"]]);
			CurrentWeek = Convert.ToInt32(dr[FName["CurrentWeek"]]);
			// get next year
			YearReadPosition++;
			return true;
		}

		/// <summary>
		///		Write year data into the datasource.  An InvalidOperationException exception is
		///		raised if the datasource is open for reading only or if the datasource has not
		///		been connected.
		/// </summary>
		/// <param name="Year">
		///		The year to write into the datasource.  An ArgumentNullException exception is
		///		raised if Year is null.
		///	</param>
		protected override void WriteYearData(cYear Year) 
		{
			if (!mvarConnected) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Year == null) ThrowYearException();
			// write the data
			DataTable dt = mvarDataSet.Tables[TName["Years"]];
			// add the year record
			DataRow dr = dt.NewRow();
			dr[FName["ID"]] = Year.ID;
			dr[FName["WinterType"]] = Convert.ToInt32(Year.WinterType);
			dr[FName["CurrentWeek"]] = Year.CurrentWeek;
			dt.Rows.Add(dr);
		}

		/// <summary>
		///		Remove all year data from the data source.  An InvalidOperationException
		///		exception is raised if the datasource is open for reading only or if the
		///		datasource has not been connected.
		/// </summary>
		protected override void ClearYears()
		{
			if (!mvarConnected) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			DataTable dt = mvarDataSet.Tables[TName["Years"]];
			dt.Rows.Clear();
		}

		// ********************** private members **********************************
		// the main dataset used for reading data
		private DataSet mvarDataSet;
		private int ReadPosition = 0;
        private int CellPosition = 0;
        private int OffspringPosition = 0;
        private int VaccinePosition = 0;
        private int DiseasePosition = 0;
		private int YearReadPosition = 0;

		// create an empty datasource
		private void CreateEmptyDataSource()
		{           

            // make sure field names are of the required size
            SetTableAndFieldNames(mvarFieldSize);
			// create the data set
			mvarDataSet = new DataSet("Animals_Data");
			// Indicator of Long or Short names
			DataTable dt = mvarDataSet.Tables.Add("NameSize");
			dt.Columns.Add("NameSize", System.Type.GetType("System.Int32"));
			// animals
			dt = mvarDataSet.Tables.Add(TName["Animals"]);
			dt.Columns.Add(FName["ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Age"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Parent_ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Gender"], System.Type.GetType("System.Byte"));
			dt.Columns.Add(FName["IsAlive"], System.Type.GetType("System.Boolean"));
			dt.Columns.Add(FName["YearBorn"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["WeekBorn"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["YearDied"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["WeekDied"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["IsIndependent"], System.Type.GetType("System.Boolean"));            
            dt.Columns.Add(FName["CannotGiveBirth"], System.Type.GetType("System.Int32"));
            dt.Columns.Add(FName["ListIndex"], System.Type.GetType("System.Int32"));
			// Markers
			dt = mvarDataSet.Tables.Add(TName["Markers"]);
			dt.Columns.Add(FName["Animal_ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["AutoMarker"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["Marker"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["PartnerMarker"], System.Type.GetType("System.String"));
			// cells
			dt = mvarDataSet.Tables.Add(TName["Cells"]);
			dt.Columns.Add(FName["Animal_ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Cell_ID"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["Year"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Week"], System.Type.GetType("System.Int32"));
			// CellsName
			dt = mvarDataSet.Tables.Add(TName["CellsName"]);
			dt.Columns.Add(FName["CellsName"], System.Type.GetType("System.String"));
			// Disease
			dt = mvarDataSet.Tables.Add(TName["Disease"]);
			dt.Columns.Add(FName["Animal_ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Disease_Name"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["Infection_Year"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Infection_Week"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["InfectiousPeriod"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["IncubationPeriod"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["InfectingAnimalID"], System.Type.GetType("System.Int32"));
            dt.Columns.Add(FName["Natural_Immunity"], System.Type.GetType("System.String")); //EER
			// Offspring
			dt = mvarDataSet.Tables.Add(TName["Offspring"]);
			dt.Columns.Add(FName["Animal_ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Offspring_ID"], System.Type.GetType("System.Int32"));
			// RunInfo
			dt = mvarDataSet.Tables.Add(TName["RunInfo"]);
			dt.Columns.Add(FName["RunName"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["RunDate"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["RunPerson"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["RunComments"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["CreatorAffiliation"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["CreatorContactInfo"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["Animals"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["StartingDatasource"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["RandomGenerator"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["RandomSeed"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["CellDatasourceName"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["CellDatasourceLocation"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["TotalPopulation"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["NextYear"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["NextWeek"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["DiseasePresent"], System.Type.GetType("System.Boolean"));
			dt.Columns.Add(FName["Version"], System.Type.GetType("System.String"));
			// TimeStamp
			dt = mvarDataSet.Tables.Add(TName["TimeStamp"]);
			dt.Columns.Add(FName["Year"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Week"], System.Type.GetType("System.Int32"));
			// Vaccines
			dt = mvarDataSet.Tables.Add(TName["Vaccines"]);
			dt.Columns.Add(FName["Animal_ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Disease_Name"], System.Type.GetType("System.String"));
			dt.Columns.Add(FName["Vaccinate_Year"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Vaccinate_Week"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["Effective_Period"], System.Type.GetType("System.Int32"));
			// Years
			dt = mvarDataSet.Tables.Add(TName["Years"]);
			dt.Columns.Add(FName["ID"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["WinterType"], System.Type.GetType("System.Int32"));
			dt.Columns.Add(FName["CurrentWeek"], System.Type.GetType("System.Int32"));

        }
		
		// Create the datatables for reading the data
		private void CreateDataset() 
		{            
            // create the data set
            mvarDataSet = new DataSet("Animals_Data");
			
            // read the XML file from disk or from the internet
			if (this.DataSourceName.ToLower().StartsWith("http://")) 
			{                
                // create a stream based on an internet connection
                WebRequest req = WebRequest.Create(this.DataSourceName);
				WebResponse result = req.GetResponse();
				Stream ReceiveStream = result.GetResponseStream();
				// read data from that stream
				mvarDataSet.ReadXml(ReceiveStream, XmlReadMode.ReadSchema);
				ReceiveStream.Close();
			}
			else 
			{                
                // read data from a file stream
                string MyPath = this.DataSourcePath;                
                if (MyPath.Length > 0)
				{
	                if (this.IsUnix)
	                {
	                    if (!MyPath.EndsWith("/")) MyPath = string.Format("{0}/", MyPath);
	                }
	                else
	                {
	                    if (!MyPath.EndsWith(@"\")) MyPath = string.Format(@"{0}\", MyPath);
	                }
				}                
                mvarDataSet.ReadXml(string.Format("{0}{1}", MyPath, this.DataSourceName), XmlReadMode.ReadSchema);                
            }
			// now get the field name size and set it appropriately
			try 
			{
				DataTable dt = mvarDataSet.Tables["NameSize"];
				DataRow dr = dt.Rows[0];
				if (Convert.ToInt32(dr["NameSize"]) == 1) 
					mvarFieldSize = enumFieldNameSize.ShortNames;
				else
					mvarFieldSize = enumFieldNameSize.LongNames;
			}
			catch 
			{
				mvarFieldSize = enumFieldNameSize.LongNames;
			}            
            SetTableAndFieldNames(mvarFieldSize);
            
            // make sure DataViews are sorted
            this.CreateDataViewSorts();            
        }

        // create the sorting of the default dataviews for tables within the dataset.  This will be called on the
        // first attempt to read from the DataSource.
        private void CreateDataViewSorts()
        {            
            // set up the sorting on the default data views for each table
            mvarDataSet.Tables[TName["Animals"]].DefaultView.Sort = FName["ID"];            
            mvarDataSet.Tables[TName["Markers"]].DefaultView.Sort = FName["Animal_ID"];            
            mvarDataSet.Tables[TName["Offspring"]].DefaultView.Sort = FName["Animal_ID"];            
            mvarDataSet.Tables[TName["Cells"]].DefaultView.Sort = string.Format("{0},{1},{2}", FName["Animal_ID"], FName["Year"], FName["Week"]);            
            mvarDataSet.Tables[TName["Disease"]].DefaultView.Sort = string.Format("{0},{1},{2}", FName["Animal_ID"], FName["Infection_Year"], FName["Infection_Week"]);            
            mvarDataSet.Tables[TName["Vaccines"]].DefaultView.Sort = string.Format("{0},{1},{2}", FName["Animal_ID"], FName["Vaccinate_Year"], FName["Vaccinate_Week"]);            
        }

		// 2-D array mapping long table names to short table names
		private string[,] TableNames = new string[10, 2] {
															{"Animals", "A"},
															{"Cells", "B"},
															{"CellsName", "C"},
															{"Disease", "D"},
															{"Offspring", "E"},
															{"RunInfo", "F"},
															{"TimeStamp", "G"},
															{"Vaccines", "H"},
															{"Years", "I"},
															{"Markers", "J"}
														 };


        // 2-D array mapping long field names to short field names
        private string[,] FieldNames = new string[50, 2] {
                                                            {"ID", "A"},
                                                            {"Age", "B"},
                                                            {"Parent_ID", "C"},
                                                            {"Gender", "D"},
                                                            {"IsAlive", "E"},
                                                            {"YearBorn", "F"},
                                                            {"WeekBorn", "G"},
                                                            {"YearDied", "H"},
                                                            {"WeekDied", "I"},
                                                            {"IsIndependent", "J"},
                                                            {"Animal_ID", "K"},
                                                            {"Cell_ID", "L"},
                                                            {"Infection_Year", "M"},
                                                            {"Infection_Week", "N"},
                                                            {"CellsName", "O"},
                                                            {"Disease_Name", "P"},
                                                            {"InfectiousPeriod", "Q"},
                                                            {"IncubationPeriod", "R"},
                                                            {"Offspring_ID", "S"},
                                                            {"Effective_Period", "T"},
                                                            {"WinterType", "U"},
                                                            {"CurrentWeek", "V"},
                                                            {"RunName", "W"},
                                                            {"RunDate", "X"},
                                                            {"RunPerson", "Y"},
                                                            {"RunComments", "Z"},
                                                            {"CreatorAffiliation", "AA"},
                                                            {"CreatorContactInfo", "BB"},
                                                            {"Animals", "CC"},
                                                            {"StartingDatasource", "DD"},
                                                            {"RandomGenerator", "EE"},
                                                            {"RandomSeed", "FF"},
                                                            {"CellDatasourceName", "GG"},
                                                            {"CellDatasourceLocation", "HH"},
                                                            {"InfectingAnimalID", "II"},
                                                            {"TotalPopulation", "JJ"},
                                                            {"NextYear", "KK"},
                                                            {"NextWeek", "LL"},
                                                            {"DiseasePresent", "MM"},
                                                            {"AutoMarker", "NN"},
                                                            {"Marker", "OO"},
                                                            {"Version", "PP"},
                                                            {"CannotGiveBirth", "QQ"},
                                                            {"PartnerMarker", "RR"},
                                                            {"ListIndex", "SS"},
                                                            {"Natural_Immunity", "TT"},
                                                            {"Vaccinate_Year", "UU"},
                                                            {"Vaccinate_Week", "VV"},
                                                            {"Year", "WW"},
                                                            {"Week", "XX"}
        };
                                                       
		// the field size used by items in this datasource
		private enumFieldNameSize mvarFieldSize;
		// a list of field names keyed by the long field name
		private StringDictionary FName;
		// a list of table names keyed by the long table name
		private StringDictionary TName;
		// set table and field names to long or short
		private void SetTableAndFieldNames(enumFieldNameSize FSize) 
		{
			// convert field size to an integer
			int FS = (int) FSize;
			// just in case
			if (FS != 1 && FS != 0) FS = 0;
			// set table names
			TName = new StringDictionary();
			for (int i = 0; i < 10; i++) {
				TName.Add(TableNames[i, 0], TableNames[i, FS]);
			}
			// set field names
			FName = new StringDictionary();//EER: increased to 46 to accommodate {"Natural_Immunity", "TT"} and then 48 for {"Vaccinate_Year", "UU"} and {"Vaccinate_Week", "VV"} and 50 for {"Year", "WW"} and {"Week", "XX"}
            for (int i = 0; i < 50; i++) {
				FName.Add(FieldNames[i, 0], FieldNames[i, FS]);                
            }
		}
		// throw an exception indicating that the datasource is not connected
		private void ThrowConnectionException() 
		{
			throw new InvalidOperationException("The datasource is not connected.");
		}
		// raise an exception indicating that the datasource is open for read-only
		private void ThrowReadOnlyException()
		{
			throw new InvalidOperationException("Datasource is open for reading only.");
		}
		// throw an exception indication that the Cells parameter is null
		private void ThrowCellsException()
		{
			throw new ArgumentNullException("Cells", "Cells must not be null.");
		}
		// raise an exception indicating a null Metadata parameter.
		private void ThrowMetadataException()
		{
			throw new ArgumentNullException("Metadata", "Metadata must not be null.");
		}
		// raise an exception indication a null AnimalAttribute parameter
		private void ThrowAnimalAttributesException()
		{
			throw new ArgumentNullException("Attributes", "Attributes must not be null.");
		}
		// raise an exception indicating a null background object
		private void ThrowBackgroundException()
		{
			throw new ArgumentNullException("Background", "Background must not be null.");
		}
		// raise an exception indicating a null Year object
		private void ThrowYearException()
		{
			throw new ArgumentNullException("Year", "Year must not be null.");
		}
	}
}
