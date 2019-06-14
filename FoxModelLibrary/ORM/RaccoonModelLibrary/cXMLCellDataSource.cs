using System;
using System.Collections.Specialized;
using System.Data;
using Rabies_Model_Core;
using System.IO;
using System.Net;

namespace Raccoon
{
	/// <summary>
	///		A read-only cell data source stored in XML format.  This class implements the
	///		IDisposable interface; the dispose method calls the Disconnect method to free
	///		up any resources that may be tied up by the datasource.  The Dispose method will
	///		ignore any errors raised by the Disconnect method.
	/// </summary>
	public class cXMLCellDataSource : cCellsDataSource
	{

		/// <summary>
		///		Static function returns corresponding short table name for a long table name
		/// </summary>
		/// <param name="LongTableName">The long table name</param>
		/// <returns>The short table name</returns>
		public static string GetShortTableName(string LongTableName) 
		{
			// loop through array og long field names
			for (int i = 0; i < 5; i++) {
				if (TableNames[i, 0].ToLower() == LongTableName.ToLower())
					// found long field name - return the short field name
					return TableNames[i, 1];
			}
			// name not found - return empry string
			return "";
		}

		/// <summary>
		///		Static function returns corresponding short field name for a long field name
		/// </summary>
		/// <param name="LongFieldName">The long field name</param>
		/// <returns>The short field name</returns>
		public static string GetShortFieldName(string LongFieldName) 
		{
			// loop through array og long field names
			for (int i = 0; i < 23; i++) 
			{
				if (FieldNames[i, 0].ToLower() == LongFieldName.ToLower())
					// found long field name - return the short field name
					return FieldNames[i, 1];
			}
			// name not found - return empry string
			return "";
		}


		/// <summary>
		///		Initialize the class.  The Connect method must be called to explicitly 
		///		connect to the database.
		/// </summary>
        /// <param name="IsUnix">A boolean value indocating that this is a Unix based datasource</param>
        public cXMLCellDataSource(bool IsUnix)
            : base(IsUnix)
		{
			mvarFieldSize = enumFieldNameSize.LongNames;
		}

		/// <summary>
		///		Connects to the datasource with the name given by the DataSourceName property 
		///		that is in the path defined by the DataSourcePath property.  If the name or the
		///		path are invalid, the connection will fail and a FileNotFoundException exception
		///		will be raised.
		/// </summary>
		public override void Connect() 
		{
			if (this.DataSourceName.ToLower().StartsWith("http://")) {
				// get data from a web server
				CreateDataset();
				mvarConnected = true;
			}
			else {
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
				// if the datasource exists, open it;  otherwise, raise an exception
				if (DBSourceInfo.Exists) {
					CreateDataset();
					mvarConnected = true;
				}
				else {
					throw new FileNotFoundException(string.Format("Cell datasource \"{0}\" not found!!", FullName), 
					                                FullName);
				}
			}
		}

		/// <summary>
		///		Reset the datasource so that the first cell is returned by the next read
		///		request.  If the datasource has not been connected, an InvalidOperationException
		///		exception is raised.
		/// </summary>
		protected override void Reset() 
		{
			if (!mvarConnected) ThrowConnectionException();
			ReadPosition = 0;
		}

		/// <summary>
		///		Reset the SuperCell reader so that the first SuperCell in the datasource is
		///		returned by the next read request.  If the datasource has not been connected,
		///		an InvalidOperationException exception is raised.
		/// </summary>
		protected override void ResetSupercells() 
		{
			if (!mvarConnected) ThrowConnectionException();
			SuperCellReadPosition = 0;
		}

		/// <summary>
		///		Disconnect the datasource and close the database.  If the datasource has not
		///		been connected, an InvalidOperationException exception is raised.
		/// </summary>
		public override void Disconnect()
		{
			if (!mvarConnected) ThrowConnectionException();
			// close the recordsets
			mvarDataSet.Clear();
			mvarConnected = false;
		}

		/// <summary>
		///		Get the path to the MapGuide map that corresponds to this cell datasource and
		///		the name of the map layer that contains the cells.  If the datasource has
		///		not been connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <param name="Path">The path to the map.  This may be a local path or a URL.</param>
		/// <param name="CellLayer">The name of the map layer containing the cells.</param>
		public override void GetMapInfo(ref string Path, ref string CellLayer)
		{
			if (!mvarConnected) ThrowConnectionException();
			// get the name table
			DataTable dt = mvarDataSet.Tables[TName["CellMapLocation"]];
			DataRow dr = dt.Rows[0];
			// get the values
			// get the path to the map
			Path = Convert.ToString(dr[FName["CellMapLocation"]]);
			// if only a file name is specified, add the datasource path (but only if this is a local
			// path)
			if (Path.IndexOf(@"\") < 0 && Path.IndexOf("/") < 0) 
            {
                string MyPath = this.DataSourcePath;
                if (this.IsUnix)
                {
                    if (!MyPath.EndsWith("/")) MyPath = string.Format("{0}/", MyPath);
                }
                else
                {
                    if (!MyPath.EndsWith(@"\")) MyPath = string.Format(@"{0}\", MyPath);
                }
				Path = string.Format("{0}{1}", MyPath, Path);
			}
			// get the layer containing the cells
			CellLayer = Convert.ToString(dr[FName["CellLayer"]]);
		}

		/// <summary>
		///		Retrieve metadata from the datasource.  If the datasource has
		///		not been connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <param name="Metadata">
		///		The cCellsMetadata object into which the metadata is written.  If Metadata is
		///		null, an ArgumentNullException exception is raised.
		///	</param>
		public override void GetMetadata(cCellsMetadata Metadata)
		{
			if (!mvarConnected) ThrowConnectionException();
			if (Metadata == null)
				throw new ArgumentNullException("Metadata", "Metadata must not be null.");
			// get the name table
			DataTable dt = mvarDataSet.Tables[TName["MetaData"]];
			DataRow dr = dt.Rows[0];
			// get the values
			Metadata.CreatorName = Convert.ToString(dr[FName["CreatorName"]]);
			Metadata.CreatorAffiliation = Convert.ToString(dr[FName["CreatorAffiliation"]]);
			Metadata.CreatorContactInfo = Convert.ToString(dr[FName["CreatorContactInfo"]]);
			Metadata.CreationDate = Convert.ToString(dr[FName["CreationDate"]]);
			Metadata.Description = Convert.ToString(dr[FName["Description"]]);
		}

		// *********************** protected members ****************************************
		/// <summary>
		///		Read the next available cell from the datasource.    If the datasource has not
		///		been connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <param name="Cells">
		///		A cCellData object into which the data is written.  If Cells is null, an
		///		ArgumentNullException exception is raised.
		/// </param>
		/// <returns>False if there are no more cell records to read, True otherwise.</returns>
		protected override bool GetNextCellRecord(cCellData Cells) 
		{
			if (!mvarConnected) ThrowConnectionException();
			if (Cells == null) throw new ArgumentNullException("Cells", "Cells cannot be null.");
			// return false if we have reached the end of the record set
			if (ReadPosition == mvarDataSet.Tables[TName["AllCellData"]].Rows.Count) return false;
			// get the data
			DataTable dt = mvarDataSet.Tables[TName["AllCellData"]];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[ReadPosition];
			// read the data at the next position
			Cells.ID = Convert.ToString(dr[FName["HEXID"]]);
			Cells.SuperCellID = Convert.ToString(dr[FName["supercell"]]);
			Cells.K = Convert.ToDouble(dr[FName["K"]]);
			Cells.XLoc = Convert.ToDouble(dr[FName["easting"]]);
			Cells.YLoc = Convert.ToDouble(dr[FName["northing"]]);
			Cells.Neighbours[0] = Convert.ToString(dr[FName["n"]]);
			Cells.Neighbours[1] = Convert.ToString(dr[FName["ne"]]);
			Cells.Neighbours[2] = Convert.ToString(dr[FName["se"]]);
			Cells.Neighbours[3] = Convert.ToString(dr[FName["s"]]);
			Cells.Neighbours[4] = Convert.ToString(dr[FName["sw"]]);
			Cells.Neighbours[5] = Convert.ToString(dr[FName["nw"]]);
			ReadPosition++;
			return true;
		}

		/// <summary>
		///		Read the next available SuperCell from the datasource.  If the datasource has
		///		not	been connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <param name="Name">
		///		The name of the supercell is written into this parameter.
		/// </param>
		/// <param name="InResistance">
		///		The InResistance of the Supercell is written into this parameter.
		/// </param>
		/// <param name="OutResistance">
		///		The OutResistance of the supercell is written into this parameter.
		/// </param>
		/// <returns>
		///		False if there are no more Supercell records to read, True otherwise.
		/// </returns>
		protected override bool GetNextSupercellRecord(ref string Name, ref int InResistance,
														ref int OutResistance) 
		{
			if (!mvarConnected) ThrowConnectionException();
			// return false if we have reached the end of the record set
			if (SuperCellReadPosition == mvarDataSet.Tables[TName["SuperCells"]].Rows.Count) return false;
			// get the data
			DataTable dt = mvarDataSet.Tables[TName["SuperCells"]];
			// get the datarow at the current read position
			DataRow dr = dt.Rows[SuperCellReadPosition];
			// read the data at the next position
			Name = Convert.ToString(dr[FName["ID"]]);
			InResistance = Convert.ToInt32(dr[FName["InResistance"]]);
			OutResistance = Convert.ToInt32(dr[FName["OutResistance"]]);
			SuperCellReadPosition++;
			return true;
		}

		/// <summary>
		///		Returns the name of the datasource.  If the datasource has not been
		///		connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <returns>The name of the datasource.</returns>
		protected override string GetName() 
		{
			if (!mvarConnected) ThrowConnectionException();
			// get the name table
			DataTable dt = mvarDataSet.Tables[TName["Name"]];
			DataRow dr = dt.Rows[0];
			// get the values
			string Name = Convert.ToString(dr[FName["Name"]]);
			// return the name value
			return Name;
		}

		// ********************* private members *********************************************
		// the main dataset used for reading data
		private DataSet mvarDataSet;
		// the current read position in the datasource
		private int ReadPosition;
		// the current supercell read position in the datasource
		private int SuperCellReadPosition;
		// Create the datatables for reading the data
		private void CreateDataset() {
			// create the data set
			mvarDataSet = new DataSet("Cells_Data");
			// read the XML file (from disk or http)
			if (this.DataSourceName.ToLower().StartsWith("http://")) 
			{
				// create the stream for reading data from this IP Address
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
                if (mvarDataSet.Tables.Contains("NameSize"))
                {
				    DataTable dt = mvarDataSet.Tables["NameSize"];
				    DataRow dr = dt.Rows[0];
				    if (Convert.ToInt32(dr["NameSize"]) == 1) 
					    mvarFieldSize = enumFieldNameSize.ShortNames;
				    else
					    mvarFieldSize = enumFieldNameSize.LongNames;
                }
                else
                {
                    // we can look for a table named "A".  If found then we are using short fieldnames
                    if (mvarDataSet.Tables.Contains("A"))
                    {
                        mvarFieldSize = enumFieldNameSize.ShortNames;
                    }
                    else
                    {
                        mvarFieldSize = enumFieldNameSize.LongNames;
                    }
                }
			}
			catch 
            {
				mvarFieldSize = enumFieldNameSize.LongNames;
			}
			// set the names of the tables and fields (long or short)
			SetTableAndFieldNames(mvarFieldSize);
		}


		// 2-D array mapping long table names to short table names
		private static string[,] TableNames = new string[5, 2] {
															{"AllCellData", "A"},
															{"CellMapLocation", "B"},
															{"Name", "C"},
															{"SuperCells", "D"},
															{"MetaData", "E"}
														};
		// 2-D array mapping long field names to short field names
		private static string[,] FieldNames = new string[23, 2] {
															{"HEXID", "A"},
															{"n", "B"},
															{"ne", "C"},
															{"se", "D"},
															{"s", "E"},
															{"sw", "F"},
															{"nw", "G"},
															{"supercell", "H"},
															{"easting", "I"},
															{"northing", "J"},
															{"K", "K"},
															{"CellMapLocation", "L"},
															{"CellLayer", "M"},
															{"Name", "N"},
															{"ID", "O"},
															{"InResistance", "P"},
															{"OutResistance", "Q"},
															{"CreatorName", "R"},
															{"CreatorAffiliation", "S"},
															{"CreatorContactInfo", "T"},
															{"CreationDate", "U"},
															{"Description", "V"},
															{"Version", "W"}
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
			for (int i = 0; i < 5; i++) 
			{
				TName.Add(TableNames[i, 0], TableNames[i, FS]);
			}
			// set field names
			FName = new StringDictionary();
			for (int i = 0; i < 23; i++) 
			{
				FName.Add(FieldNames[i, 0], FieldNames[i, FS]);
			}
		}
		// throw an exception indicating that the datasource is not connected
		private void ThrowConnectionException() 
		{
			throw new InvalidOperationException("The datasource is not connected.");
		}

	}
}