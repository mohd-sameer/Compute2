using System;
using System.Data;
using System.Data.OleDb;
using Rabies_Model_Core;
using System.IO;

namespace Raccoon
{
	/// <summary>
	///		A raccoon data source stored in a Microsoft Access database.
	///		This class implements the IDisposable interface; the dispose method	calls the
	///		Disconnect method to free up any resources that may be tied up by the
	///		datasource.  The Dispose method will ignore any errors raised by the Disconnect
	///		method.
	/// </summary>
	public class cMDBRaccoonDataSource : cRaccoonDataSource
	{
		/// <summary>
		///		Initialize a raccoon Access datasource.  The Connect method must be called to
		///		explicitly connect to the database.
		/// </summary>
		/// <param name="EmptyPath">
		///		The path to the empty database used to create new datasources.
		///	</param>
        /// <param name="IsUnix">A boolean value indicating that this is a Unix based datasource</param>
        public cMDBRaccoonDataSource(string EmptyPath, bool IsUnix) 
			: base(IsUnix)
		{
			DB_CONN_STRING = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=";
			mvarEmptyPath = EmptyPath;
		}

		// ********************************* Properties *************************************
		/// <summary>
		///		The path to the empty database used to create new raccoon datasources
		///		(read-only).
		/// </summary>
		public string EmptyPath {
			get {
				return mvarEmptyPath;
			}
		}

		// ********************************* Methods ****************************************
		/// <summary>
		///		Connect to the mdb (access format) datasource.  The name and path to the
		///		database are specified by the DatasourceName and DatasourcePath properties.  If
		///		the access database file specified by these properties does not exist, it is
		///		created if the datasource is opened for read/write access.  A FileNotFoundException
		///		exception is raised if an attempt is made to open a datasource that does not
		///		exist as read only.  If the path to the empty database (the EmptyPath property)
		///		is not correct, a FileNotFoundException exception will be raised when the new
		///		datasource is created.
		/// </summary>
		/// <param name="ReadOnly">
		///		A flag indicating that the datasource should be opened as read only.
		///	</param>
		public override void Connect(bool ReadOnly) 
		{
			// create full path and name
			string FullName = "";
			if (this.DataSourcePath.Length == 0)
			{
				FullName = this.DataSourceName;
			}
			else
			{
				if (!this.DataSourcePath.EndsWith(@"\"))
				{
					FullName = string.Format(@"{0}\{1}", this.DataSourcePath, this.DataSourceName);
				}
				else
				{
					FullName = string.Format("{0}{1}", this.DataSourcePath, this.DataSourceName);
				}
			}
			// does the file already exist
			// if it does not, create it if the datasource was opened for read-write.  If the
			// datasource was created for read-only, raise an exception.
			if (!File.Exists(FullName)) {
				if (ReadOnly) {
					throw new FileNotFoundException(string.Format("Raccoon datasource \"{0}\" not found!!", FullName),
													FullName);
				}
				else {
					// if this datasource does not exist, raise an exception
					if (!File.Exists(mvarEmptyPath)) {
						throw new FileNotFoundException("Path to template database is invalid!!",
								mvarEmptyPath);
					}
					// copy this empty database file to the location of the datasource
					File.Copy(mvarEmptyPath, FullName, true);
                }
			}
			// make sure database is just a regular file (no read only etc.)
			File.SetAttributes(FullName, System.IO.FileAttributes.Normal);
			// now open the data source
			// connect to the database
			mvarConnection = new OleDbConnection(DB_CONN_STRING + FullName);
			mvarConnection.Open();
			// get the data and set the initial read positions
			CreateDataset();
			ReadPosition = 0;
			YearReadPosition = 0;
			mvarReadOnly = ReadOnly;
			mvarConnected = true;
		}

		// Create the internal in memory datatables for reading the data
		private void CreateDataset() 
		{
			
			int i;
			DataRow dr;
			OleDbDataAdapter da;
			// create the data set
			mvarDataSet = new DataSet("Animals_Data");

			// get animals table or animalmarker query (depending on version of database)
			// look for the markers tables
			DataTable schemaTable = mvarConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
				new object[] {null, null, null, "TABLE"});
			for (i = 0; i < schemaTable.Rows.Count; i++) 
			{
				dr = schemaTable.Rows[i];
				if (Convert.ToString(dr["TABLE_NAME"]) == "Markers") 
				{
					// markers table found - load data from query
					da = new OleDbDataAdapter("SELECT * FROM [AnimalMarkers] ORDER BY ID",
						mvarConnection);
					da.Fill(mvarDataSet, "Animals");
					break;
				}
			}
			if (i == schemaTable.Rows.Count) {
				// markers table not found - just get data from the animals table
				da = new OleDbDataAdapter("SELECT * FROM [Animals] ORDER BY ID",
					mvarConnection);
				da.Fill(mvarDataSet, "Animals");
			}

			// get cells table
			da = new OleDbDataAdapter("SELECT * FROM [Cells] ORDER BY Animal_ID, Year, Week",
										mvarConnection);
			da.Fill(mvarDataSet, "Cells");
			// get disease table
			da = new OleDbDataAdapter("SELECT * FROM [Disease] ORDER BY Animal_ID, Year, Week",
										mvarConnection);
			da.Fill(mvarDataSet, "Disease");
			// get offspring table
			da = new OleDbDataAdapter("SELECT * FROM [Offspring] ORDER BY Animal_ID",
										mvarConnection);
			da.Fill(mvarDataSet, "Offspring");
			// get vaccines table
			da = new OleDbDataAdapter("SELECT * FROM [Vaccines] ORDER BY Animal_ID, Year, Week",
										mvarConnection);
			da.Fill(mvarDataSet, "Vaccines");
			// now create the relations between the tables
			mvarDataSet.Relations.Add("CellsRelation", 
										mvarDataSet.Tables["Animals"].Columns["ID"],
										mvarDataSet.Tables["Cells"].Columns["Animal_ID"]);
			mvarDataSet.Relations.Add("DiseaseRelation", 
										mvarDataSet.Tables["Animals"].Columns["ID"],
										mvarDataSet.Tables["Disease"].Columns["Animal_ID"]);
			mvarDataSet.Relations.Add("OffspringRelation", 
										mvarDataSet.Tables["Animals"].Columns["ID"],
										mvarDataSet.Tables["Offspring"].Columns["Animal_ID"]);
			mvarDataSet.Relations.Add("VaccinesRelation", 
										mvarDataSet.Tables["Animals"].Columns["ID"],
										mvarDataSet.Tables["Vaccines"].Columns["Animal_ID"]);
			// get years table
			da = new OleDbDataAdapter("SELECT * FROM [Years] ORDER BY ID", mvarConnection);
			da.Fill(mvarDataSet, "Years");
		}

		/// <summary>
		///		Disconnect from the raccoon datasource and close the database.
		/// </summary>
		public override void Disconnect()
		{
			if (mvarConnection != null) {
				if (mvarConnection.State > 0) {
					// close the recordsets
					mvarDataSet.Relations.Clear();
					mvarDataSet.Clear();
					// close the database
					mvarConnection.Close();
				}
				mvarConnection = null;
				mvarConnected = false;
			}
		}

		// *********************** protected members ****************************************
		/// <summary>
		///		Checks to see if the name of the cell data source matches the cell data
		///		source name stored in the animals data source.  An InvalidOperationException
		///		exception is raised if the datasource is not connected.
		/// </summary>
		/// <param name="Cells">
		///		The list of cells containg the name to check.  An ArgumentNullException
		///		exception is raised if Cells is null.
		///	</param>
		/// <returns>True if the names match.</returns>
		protected override bool CheckCellsName(cCellList Cells) 
		{
			if (mvarConnection == null) ThrowConnectionException();
			if (Cells == null) ThrowCellsException();
			// create a dataset
			DataSet CellsNameDataSet = new DataSet("CellsNameDataSet");
			// get CellsName table
			OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [CellsName]",
														mvarConnection);
			da.Fill(CellsNameDataSet, "CellsName");
			// now get the new table
			DataTable dt = CellsNameDataSet.Tables[0];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[0];
			// check the cells name
			bool CheckValue = (Convert.ToString(dr["CellsName"]) == Cells.Name);
			CellsNameDataSet.Clear();
			return CheckValue;
		}
		
		/// <summary>
		///		Writes the name of the cells data source that these animals are associated 
		///		with.  An InvalidOperationException	exception is raised if the datasource
		///		is not connected or if the datasource is open as read-only.
		/// </summary>
		/// <param name="Cells">
		///		The list of cells who's name shall be written.  An ArgumentNullException
		///		exception is raised if Cells is null.
		///	</param>
		protected override void WriteCellsName(cCellList Cells)
		{
			if (mvarConnection == null) ThrowConnectionException();
			if (Cells == null) ThrowCellsException();
			if (mvarReadOnly) ThrowReadOnlyException();
			// start by eliminating any existing data
			OleDbCommand cmd = new OleDbCommand("DELETE * FROM [CellsName]", mvarConnection);
			cmd.ExecuteNonQuery();
			// create the query to write the data
			cmd.CommandText = "INSERT INTO [CellsName] VALUES('" + Cells.Name +	"');";
			cmd.ExecuteNonQuery();
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
			if (mvarConnection == null) ThrowConnectionException();
			if (Metadata == null) ThrowMetadataException();
			// create a dataset
			DataSet MetaDataSet = new DataSet("MetaDataSet");
			// get CellsName table
			OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [RunInfo]",
														mvarConnection);
			da.Fill(MetaDataSet, "RunInfo");
			// now get the new table
			DataTable dt = MetaDataSet.Tables[0];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[0];
			// fill the Metadata structure
			Metadata.RunName = Convert.ToString(dr["RunName"]);
			Metadata.RunComments = Convert.ToString(dr["RunComments"]);
			Metadata.CreationDate = Convert.ToString(dr["RunDate"]);
			Metadata.CreatorName = Convert.ToString(dr["RunPerson"]);
			Metadata.CreatorAffiliation = Convert.ToString(dr["CreatorAffiliation"]);
			Metadata.CreatorContactInfo = Convert.ToString(dr["CreatorContactInfo"]);
			Metadata.Animals = Convert.ToString(dr["Animals"]);
			Metadata.StartingDatasource = Convert.ToString(dr["StartingDatasource"]);
			Metadata.RandomGenerator = Convert.ToString(dr["RandomGenerator"]);
			Metadata.RandomSeed = dr["RandomSeed"].ToString();
			Metadata.CellDatasourceLocation = Convert.ToString(dr["CellDatasourceLocation"]);
			Metadata.CellDatasourceName = Convert.ToString(dr["CellDatasourceName"]);
			Metadata.TotalPopulation = Convert.ToInt32(dr["TotalPopulation"]);
			Metadata.NextYear = Convert.ToInt32(dr["NextYear"]);
			Metadata.NextWeek = Convert.ToInt32(dr["NextWeek"]);
			Metadata.DiseasePresent = Convert.ToBoolean(dr["DiseasePresent"]);
			// were done
			MetaDataSet.Clear();
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
			if (mvarConnection == null) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Metadata == null) ThrowMetadataException();
			// create the command object which will be used to run the required queries
			OleDbCommand cmd = new OleDbCommand();
			cmd.Connection = mvarConnection;
			cmd.CommandType = CommandType.StoredProcedure;
			// delete the current settings
			cmd.CommandText = "DeleteInfo";
			cmd.Parameters.Clear();
			cmd.ExecuteNonQuery();
			// add the new settings (via a stored command)
			cmd.CommandText = "InsertInfo";
			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("RName", Metadata.RunName);
			cmd.Parameters.AddWithValue("RPerson", Metadata.CreatorName);
			cmd.Parameters.AddWithValue("RComments", Metadata.RunComments);
			cmd.Parameters.AddWithValue("CAffiliation", Metadata.CreatorAffiliation);
			cmd.Parameters.AddWithValue("CContactInfo", Metadata.CreatorContactInfo);
			cmd.Parameters.AddWithValue("Animals", Metadata.Animals);
			cmd.Parameters.AddWithValue("Datasource", Metadata.StartingDatasource);
			cmd.Parameters.AddWithValue("RandomGenerator", Metadata.RandomGenerator);
			cmd.Parameters.AddWithValue("RandomSeed", Metadata.RandomSeed);
			cmd.Parameters.AddWithValue("CellDatasource", Metadata.CellDatasourceLocation);
			cmd.Parameters.AddWithValue("CellName", Metadata.CellDatasourceName);
			cmd.Parameters.AddWithValue("TotalPopulation", Metadata.TotalPopulation);
			cmd.Parameters.AddWithValue("NextYear", Metadata.NextYear);
			cmd.Parameters.AddWithValue("NextWeek", Metadata.NextWeek);
			cmd.Parameters.AddWithValue("DiseasePresent", Metadata.DiseasePresent);
			cmd.ExecuteNonQuery();
		}

		/// <summary>
		///		Write a single animal attributes record to the database.  The record is
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
		protected override void WriteAnimalRecord(cAnimalAttributes Attributes, 
													bool DeleteFirst)
		{
			if (mvarConnection == null) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Attributes == null) ThrowAnimalAttributesException();
			// create the command object whch will be used to run the required queries
			OleDbCommand cmd = new OleDbCommand();
			cmd.Connection = mvarConnection;
			cmd.CommandType = CommandType.StoredProcedure;
			if (DeleteFirst) {
				// run DELETE queries to remove current version of this record
				RunDeleteQuery("DeleteAnimals", Attributes.ID, cmd);
				RunDeleteQuery("DeleteCells", Attributes.ID, cmd);
				RunDeleteQuery("DeleteDisease", Attributes.ID, cmd);
				RunDeleteQuery("DeleteVaccines", Attributes.ID, cmd);
				RunDeleteQuery("DeleteOffspring", Attributes.ID, cmd);
			}
			// write the data
			// run the stored procedure to insert the main record
			cmd.CommandText = "InsertAnimal";
			cmd.Parameters.Clear();
			// set parameters
			cmd.Parameters.AddWithValue("New_ID", Attributes.ID);
			cmd.Parameters.AddWithValue("New_Age", Attributes.Age);
			if (Attributes.ParentID.Length > 0) 
				cmd.Parameters.AddWithValue("New_ParentID", Attributes.ParentID);
			else
				cmd.Parameters.AddWithValue("New_ParentID", -1);
			cmd.Parameters.AddWithValue("New_Gender", Convert.ToInt32(Attributes.Gender));
			cmd.Parameters.AddWithValue("New_IsAlive", Attributes.IsAlive);
			cmd.Parameters.AddWithValue("New_YearBorn", Attributes.Cells[0].Year);
			cmd.Parameters.AddWithValue("New_WeekBorn", Attributes.Cells[0].Week);
			cmd.Parameters.AddWithValue("New_YearDied", Attributes.YearDied);
			cmd.Parameters.AddWithValue("New_WeekDied", Attributes.WeekDied);
			cmd.Parameters.AddWithValue("New_IsIndependent", Attributes.IsIndependent);
			cmd.Parameters.AddWithValue("New_CannotGiveBirth", Attributes.CannotGiveBirth);
			cmd.ExecuteNonQuery();
			// update the cells table
			cmd.CommandText = "InsertCell";
			foreach (cCellTime CT in Attributes.Cells) {
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("New_Animal_ID", Attributes.ID);
				cmd.Parameters.AddWithValue("New_Cell_ID", CT.CellID);
				cmd.Parameters.AddWithValue("New_Year", CT.Year);
				cmd.Parameters.AddWithValue("New_Week", CT.Week);
				cmd.ExecuteNonQuery();
			}
			// update the offspring table
			cmd.CommandText = "InsertOffspring";
			foreach (string OffspringID in Attributes.Offspring) {
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("New_Animal_ID", Attributes.ID);
				cmd.Parameters.AddWithValue("New_Offspring_ID", OffspringID);
				cmd.ExecuteNonQuery();
			}
			// update the disease table
			cmd.CommandText = "InsertDisease";
			foreach (cInfection Inf in Attributes.Infections) 
			{
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("New_Animal_ID", Attributes.ID);
				cmd.Parameters.AddWithValue("New_Disease_Name", Inf.Disease.Name);
				cmd.Parameters.AddWithValue("New_Year", Inf.Year);
				cmd.Parameters.AddWithValue("New_Week", Inf.Week);
				cmd.Parameters.AddWithValue("New_IncubationPeriod", Inf.IncubationPeriod);
				cmd.Parameters.AddWithValue("New_InfectiousPeriod", Inf.InfectiousPeriod);
				cmd.Parameters.AddWithValue("New_InfectingAnimalID", Inf.InfectingAnimalID);
				cmd.ExecuteNonQuery();
			}
			// update the vaccines table
			cmd.CommandText = "InsertVaccine";
			foreach (cVaccine Vaccine in Attributes.Vaccines) {
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("New_Animal_ID", Attributes.ID);
				cmd.Parameters.AddWithValue("New_Disease_Name", Vaccine.DiseaseName);
				cmd.Parameters.AddWithValue("New_Year", Vaccine.Year);
				cmd.Parameters.AddWithValue("New_Week", Vaccine.Week);
				cmd.Parameters.AddWithValue("New_EffectivePeriod", Vaccine.EffectivePeriod);
				cmd.ExecuteNonQuery();
			}
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
			if (mvarConnection == null) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Data == null) ThrowAnimalAttributesException();
			// create the command object whch will be used to run the required queries
			OleDbCommand cmd = new OleDbCommand();
			cmd.Connection = mvarConnection;
			cmd.CommandType = CommandType.StoredProcedure;
			if (DeleteFirst) {
				// run a DELETE query to remove current version of this record
				RunDeleteQuery("DeleteMarkers", Data.ID, cmd);
			}
			// write the data
			// run the stored procedure to insert the main record
			cmd.CommandText = "InsertMarkers";
			cmd.Parameters.Clear();
			// set parameters
			cmd.Parameters.AddWithValue("New_AnimalID", Data.ID);
			cmd.Parameters.AddWithValue("New_AutoMarker", Data.AutoMarker);
			cmd.Parameters.AddWithValue("New_Marker", Data.Marker);
			cmd.Parameters.AddWithValue("New_PartnerMarker", (Data.PartnerMarker == null ? "" : Data.PartnerMarker));
			// execute the query
			cmd.ExecuteNonQuery();
		}

		// run a delete query
		private void RunDeleteQuery(string QueryName, string IDValue, OleDbCommand cmd)
		{
			cmd.CommandText = QueryName;
			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("DelID", IDValue);
			cmd.ExecuteNonQuery();
		}

		/// <summary>
		///		Get the next record in the raccoon data source.  An InvalidOperationException
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
		protected override bool GetNextAnimalRecord(cAnimalAttributes Attributes, 
													cBackground Background) 
		{
			if (mvarConnection == null) ThrowConnectionException();
			if (Attributes == null) ThrowAnimalAttributesException();
			if (Background == null) ThrowBackgroundException();
			// return false if we have reached the end of the record set
			if (ReadPosition == mvarDataSet.Tables["Animals"].Rows.Count) return false;
			// copy the data to the Attributes object
			DataTable dt = mvarDataSet.Tables["Animals"];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[ReadPosition];
			// get the basic animal data
			Attributes.ID = Convert.ToString(dr["ID"]);
			Attributes.ParentID = Convert.ToString(dr["Parent_ID"]);
			Attributes.Gender = (enumGender) Convert.ToInt32(dr["Gender"]);
			Attributes.IsAlive = Convert.ToBoolean(dr["IsAlive"]);
			Attributes.IsIndependent = Convert.ToBoolean(dr["IsIndependent"]);
			Attributes.YearDied = Convert.ToInt32(dr["YearDied"]);
			Attributes.WeekDied = Convert.ToInt32(dr["WeekDied"]);
			Attributes.Age = Convert.ToInt32(dr["Age"]);
			try {
				Attributes.CannotGiveBirth = Convert.ToInt32(dr["CannotGiveBirth"]);
			}
			catch {
				Attributes.CannotGiveBirth = 0;
			}
			// now get cells, disease, offspring and vaccine info
			DataRow[] drChildren;
			// get the cells info
			drChildren = dr.GetChildRows(mvarDataSet.Relations["CellsRelation"]);
			foreach (DataRow drChild in drChildren) 
			{
				cCellTime CT = new cCellTime();
				CT.CellID = Convert.ToString(drChild["Cell_ID"]);
				CT.Year = Convert.ToInt32(drChild["Year"]);
				CT.Week = Convert.ToInt32(drChild["Week"]);
				Attributes.Cells.Add(CT);
			}
			// get the offspring info
			drChildren = dr.GetChildRows(mvarDataSet.Relations["OffspringRelation"]);
			foreach (DataRow drChild in drChildren) 
			{
				Attributes.Offspring.Add(Convert.ToString(drChild["Offspring_ID"]));
			}
			// get the disease info
			drChildren = dr.GetChildRows(mvarDataSet.Relations["DiseaseRelation"]);
			foreach (DataRow drChild in drChildren) 
			{
				string DiseaseName = Convert.ToString(drChild["Disease_Name"]);
				int Year = Convert.ToInt32(drChild["Year"]);
				int Week = Convert.ToInt32(drChild["Week"]);
				int IncubationPeriod = Convert.ToInt32(drChild["IncubationPeriod"]);
				int InfectiousPeriod = Convert.ToInt32(drChild["InfectiousPeriod"]);
				string InfectingAnimalID = Convert.ToString(drChild["InfectingAnimalID"]);
				Attributes.Infections.Add(new cInfection(Background, DiseaseName, Year, 
					Week, IncubationPeriod, InfectiousPeriod, InfectingAnimalID));
			}
			// get vaccine info
			drChildren = dr.GetChildRows(mvarDataSet.Relations["VaccinesRelation"]);
			foreach (DataRow drChild in drChildren) 
			{
				string DiseaseName = Convert.ToString(drChild["Disease_Name"]);
				int Year = Convert.ToInt32(drChild["Year"]);
				int Week = Convert.ToInt32(drChild["Week"]);
				int EffectivePeriod = Convert.ToInt32(drChild["Effective_Period"]);
				Attributes.Vaccines.Add(new cVaccine(DiseaseName, Year, Week, 
					EffectivePeriod));
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
			if (mvarConnection == null) ThrowConnectionException();
			if (Data == null) ThrowAnimalAttributesException();
			// return false if we have reached the end of the record set
			if (ReadPosition < mvarDataSet.Tables["Animals"].Rows.Count) {
				// copy the data to the Attributes object
				DataTable dt = mvarDataSet.Tables["Animals"];
				// get the datarow at the current read position
				DataRow dr = dt.Rows[ReadPosition];
				// get the basic animal data
				Data.AutoMarker = Convert.ToString(dr["AutoMarker"]);
				Data.Marker = Convert.ToString(dr["Marker"]);
				try {
					Data.PartnerMarker = Convert.ToString(dr["PartnerMarker"]);
				}
				catch {
					Data.PartnerMarker = "";
				}
			}
		}

		/// <summary>
		///		Delete all records from this raccoon datasource.  An InvalidOperationException
		///		exception is raised if the datasource is open for reading only or if the
		///		datasource has not been connected.
		/// </summary>
		protected override void Clear()
		{
			if (mvarConnection == null) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			// run DELETE queries to empty the recordsets
			OleDbCommand cmd = new OleDbCommand("DELETE * FROM [Disease]", mvarConnection);
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DELETE * FROM [Cells]";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DELETE * FROM [Offspring]";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DELETE * FROM [Vaccines]";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DELETE * FROM [Animals]";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DELETE * FROM [TimeStamp]";
			cmd.ExecuteNonQuery();
			if (this.GetVersion() > 1) {
				cmd.CommandText = "DELETE * FROM [Markers]";
				cmd.ExecuteNonQuery();
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
		///		Reset the datasource so that it points to the first raccoon in the datasource.
		///		An InvalidOperationException exception is raised if the datasource has not been
		///		connected.
		/// </summary>
		protected override void Reset() 
		{
			if (mvarConnection == null) ThrowConnectionException();
			CreateDataset();
			ReadPosition = 0;
			YearReadPosition = 0;
		}

		/// <summary>
		///		Write a time record into the data base.  An InvalidOperationException exception
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
			if (mvarConnection == null) ThrowConnectionException();
			if (Year < 0)
				throw new ArgumentOutOfRangeException("Year", "Year cannot be less than zero.");
			if (Week < 1 || Week > 52)
				throw new ArgumentOutOfRangeException("Week", "Week must be in the range of 1-52.");
			// write time record
			if (this.ReadOnly) ThrowReadOnlyException();
			// remove all records from the time record
			OleDbCommand cmd = new OleDbCommand("DELETE * FROM [TimeStamp]", mvarConnection);
			cmd.ExecuteNonQuery();
			// build the new update query and excute it
			cmd.CommandText = "INSERT INTO [TimeStamp] VALUES(" + Year + 
								", " + Week + ");";
			cmd.ExecuteNonQuery();
		}

		/// <summary>
		///		Read the time record from the animal database.  An InvalidOperationException
		///		exception is raised if the datasource has not been connected.
		/// </summary>
		/// <param name="Year">The year value in the database.</param>
		/// <param name="Week">The week value in the database.</param>
		protected override void ReadTimeRecord(ref int Year, ref int Week) 
		{
			if (mvarConnection == null) ThrowConnectionException();
			// connect to the table
			OleDbCommand cmd = new OleDbCommand("SELECT * FROM [TimeStamp]", mvarConnection);
			OleDbDataReader mvarTimeReader = cmd.ExecuteReader();
			mvarTimeReader.Read();
			// get the values
			Year = Convert.ToInt32(mvarTimeReader["Year"]);
			Week = Convert.ToInt32(mvarTimeReader["Week"]);
			mvarTimeReader.Close();
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
			if (mvarConnection == null) ThrowConnectionException();
			// return false if we have reached the end of the record set
			if (YearReadPosition == mvarDataSet.Tables["Years"].Rows.Count) return false;
			// copy the data to the Attributes object
			DataTable dt = mvarDataSet.Tables["Years"];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[YearReadPosition];
			// get the year data
			WinterType = (enumWinterType) Convert.ToInt32(dr["WinterType"]);
			CurrentWeek = Convert.ToInt32(dr["CurrentWeek"]);
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
			if (mvarConnection == null) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			if (Year == null) ThrowYearException();
			// write data
			OleDbCommand cmd = new OleDbCommand("InsertYear", mvarConnection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("New_ID", Year.ID);
			cmd.Parameters.AddWithValue("New_WinterType", Convert.ToInt32(Year.WinterType));
			cmd.Parameters.AddWithValue("New_CurrentWeek", Year.CurrentWeek);
			cmd.ExecuteNonQuery();
		}

		/// <summary>
		///		Remove all year data from the data source.  An InvalidOperationException
		///		exception is raised if the datasource is open for reading only or if the
		///		datasource has not been connected.
		/// </summary>
		protected override void ClearYears()
		{
			if (mvarConnection == null) ThrowConnectionException();
			if (this.ReadOnly) ThrowReadOnlyException();
			OleDbCommand cmd = new OleDbCommand("DELETE * FROM [Years]", mvarConnection);
			cmd.ExecuteNonQuery();
		}

		/// <summary>
		///		Reads the version number of the data source from the database
		/// </summary>
		protected override int ReadVersionNumber()
		{
			int Value;
			if (mvarConnection == null) ThrowConnectionException();
			// create a dataset
			DataSet VersionNumberDataSet = new DataSet("VersionNumberDataSet");
			// attempt to query the version number.  If this fails, assume the version is 1
		    OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [RunInfo]", mvarConnection);
			da.Fill(VersionNumberDataSet, "RunInfo");
			// now get the new table
			DataTable dt = VersionNumberDataSet.Tables[0];
			// is therer a version field
			if (dt.Columns.Contains("Version")) {
				// is there a data row currently in existence
				if (dt.Rows.Count > 0) {
					// get the first datarow
					DataRow dr = dt.Rows[0];
					// get the version number
					Value = Convert.ToInt32(dr["Version"]);
				}
				else {
					// no datarow exists.  This must be a brand new data source - version must be 2
					Value = 2;
				}
				// we're done
				VersionNumberDataSet.Clear();
			}
			else {
				// the database does not contain a version field.  It is version 1
				Value = 1;
			}
			return Value;
		}

		// ************************* private members ****************************************
		private string DB_CONN_STRING;
		private string mvarEmptyPath;
		private OleDbConnection mvarConnection;
		private DataSet mvarDataSet;
		private int ReadPosition;
		private int YearReadPosition;
		// raise an exception indicating that the datasource is not connected
		private void ThrowConnectionException()
		{
			throw new InvalidOperationException("Datasource has not been connected.");
		}
		// raise an exception indicating that the datasource is open for read-only
		private void ThrowReadOnlyException()
		{
			throw new InvalidOperationException("Datasource is open for reading only.");
		}
		// raise an exception indicating a null Cells parameter
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
