using System;
using System.Data.OleDb;
using System.IO;
using Rabies_Model_Core;

namespace Raccoon
{
	/// <summary>
	///		A read-only cell data source stored in a Microsoft Access database.
	///		This class implements the IDisposable interface; the dispose method	calls the
	///		Disconnect method to free up any resources that may be tied up by the
	///		datasource.  The Dispose method will ignore any errors raised by the Disconnect
	///		method.
	/// </summary>
	public class cMDBCellDataSource : cCellsDataSource
	{
		/// <summary>
		///		Initialize the class.  The Connect method must be called to explicitly 
		///		connect to the database.
		/// </summary>
        /// <param name="IsUnix">A boolean value indicating that this is a Unix based datasource</param>
		public cMDBCellDataSource(bool IsUnix) : base(IsUnix) 
		{
			DB_CONN_STRING = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=";
		}

		/// <summary>
		///		Connects to the database with the name given by the DataSourceName property 
		///		that is in the path defined by the DataSourcePath property.  If the name or the
		///		path are invalid, the connection will fail and an OLEDBException exception will
		///		be raised.  If the database is already connected, an InvalidOperationException
		///		exception is raised.
		/// </summary>
		public override void Connect() 
		{
			ResetConnection(false);
			// now open a data reader on the cells table
			OleDbCommand cmd = new OleDbCommand("SELECT * FROM [AllCellData]", mvarConnection);
			mvarReader = cmd.ExecuteReader();
			// indicate the connection
			mvarConnected = true;
		}


		/// <summary>
		///		Reset the datasource so that the first cell is returned by the next read
		///		request.  Calling this function also causes a reset of the SuperCell reader.  If
		///		the datasource has not been connected, an InvalidOperationException exception
		///		is raised.
		/// </summary>
		protected override void Reset() 
		{
			if (mvarReader == null) ThrowConnectionException();
			// if mvarReader is currently open, close it
			if  (!mvarReader.IsClosed) mvarReader.Close();
			// reset the connection
			ResetConnection();
			// now, recreate mvar reader
			OleDbCommand cmd = new OleDbCommand("SELECT * FROM [AllCellData]", mvarConnection);
			mvarReader = cmd.ExecuteReader();
			// now indicate that this is a cell reader
			ReaderType = "CELL";
		}

		/// <summary>
		///		Reset the SuperCell reader so that the first SuperCell in the datasource is
		///		returned by the next read request.  Calling this function also causes a reset
		///		of the main cell reader.  If the datasource has not been connected, an
		///		InvalidOperationException exception is raised.
		/// </summary>
		protected override void ResetSupercells() 
		{
			if (mvarReader == null) ThrowConnectionException();
			// if mvarReader is currently open, close it
			if  (!mvarReader.IsClosed) mvarReader.Close();
			// reset the connection
			ResetConnection();
			// now, recreate mvar reader
			OleDbCommand cmd = new OleDbCommand("SELECT * FROM [SuperCells]", mvarConnection);
			mvarReader = cmd.ExecuteReader();
			// now indicate that this is a supercell reader
			ReaderType = "SC";
		}

		/// <summary>
		///		Disconnect the datasource and close the database.  If the datasource has not
		///		been connected, an InvalidOperationException exception is raised.
		/// </summary>
		public override void Disconnect()
		{
			if (mvarReader == null) ThrowConnectionException();
			// if mvarReader is currently open, close it
            try
            {
                if (!mvarReader.IsClosed) mvarReader.Close();
				mvarConnection.Close();
			}
			catch {}
			mvarReader = null;
			mvarConnection = null;
			// indicate that connection is closed
			mvarConnected = false;
		}

		/// <summary>
		///		Get the path to the MapGuide map that corresponds to this cell datasource and
		///		the name of the map layer that contains the cells.  Calling this function will
		///		reset the main cell reader and the supercell reader.  If the datasource has
		///		not been connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <param name="Path">The path to the map.  This may be a local path or a URL.</param>
		/// <param name="CellLayer">The name of the map layer containing the cells.</param>
		public override void GetMapInfo(ref string Path, ref string CellLayer)
		{
			if (mvarReader == null) ThrowConnectionException();
			// close the existing reader
			if (!mvarReader.IsClosed) mvarReader.Close();
			// reset the connection
			ResetConnection();
			// create the command required to extract the name
			OleDbCommand cmd = new OleDbCommand("Select * FROM [CellMapLocation]", mvarConnection);
			mvarReader = cmd.ExecuteReader();
			// read the value
			mvarReader.Read();
			// get the path to the map
			Path = Convert.ToString(mvarReader["CellMapLocation"]);
			// if only a file name is specified, add the datasource path (but only if this is a local
			// path)
			if (Path.IndexOf(@"\") < 0 && Path.IndexOf("/") < 0) 
			{
				if (this.DataSourcePath.IndexOf(@"\") > 0) 
				{
					Path = this.DataSourcePath + @"\" + Path;
				}   
			}
			// get the layer containing the cells
			CellLayer = Convert.ToString(mvarReader["CellLayer"]);
			// close this reader and open the reader on the main datasource
			mvarReader.Close();
			this.Reset();
		}

		/// <summary>
		///		Retrieve metadata from the datasource.  Calling this function will
		///		reset the main cell reader and the supercell reader.  If the datasource has
		///		not been connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <param name="Metadata">
		///		The cCellsMetadata object into which the metadata is written.  If Metadata is
		///		null, an ArgumentNullException exception is raised.
		///	</param>
		public override void GetMetadata(cCellsMetadata Metadata)
		{
			if (mvarReader == null) ThrowConnectionException();
			if (Metadata == null)
				throw new ArgumentNullException("Metadata", "Metadata must not be null.");
			// close the existing reader
			if (!mvarReader.IsClosed) mvarReader.Close();
			// reset the connection
			ResetConnection();
			// create the command required to extract the name
			OleDbCommand cmd = new OleDbCommand("Select * FROM [MetaData]", mvarConnection);
			mvarReader = cmd.ExecuteReader();
			// read the value
			mvarReader.Read();
			Metadata.CreatorName = Convert.ToString(mvarReader["CreatorName"]);
			Metadata.CreatorAffiliation = Convert.ToString(mvarReader["CreatorAffiliation"]);
			Metadata.CreatorContactInfo = Convert.ToString(mvarReader["CreatorContactInfo"]);
			Metadata.CreationDate = Convert.ToString(mvarReader["CreationDate"]);
			Metadata.Description = Convert.ToString(mvarReader["Description"]);
			// close this reader and open the reader on the main datasource
			mvarReader.Close();
			this.Reset();
		}

		/// <summary>
		///		Read the next available cell from the datasource.  If the datasource has not
		///		been connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <param name="Cells">
		///		A cCellData object into which the data is written.  If Cells is null, an
		///		ArgumentNullException exception is raised.
		/// </param>
		/// <returns>False if there are no more cell records to read, True otherwise.</returns>
		protected override bool GetNextCellRecord(cCellData Cells) 
		{
			if (mvarReader == null) ThrowConnectionException();
			if (Cells == null) throw new ArgumentNullException("Cells", "Cells cannot be null.");
			// make sure reader is of the correct type
			if (ReaderType == "SC") this.Reset();
			// read the data
			if (!mvarReader.Read())
				return false;
			else 
			{
				Cells.ID = Convert.ToString(mvarReader["HEXID"]);
				Cells.SuperCellID = Convert.ToString(mvarReader["supercell"]);
				Cells.K = Convert.ToDouble(mvarReader["K"]);
				Cells.XLoc = Convert.ToDouble(mvarReader["easting"]);
				Cells.YLoc = Convert.ToDouble(mvarReader["northing"]);
				Cells.Neighbours[0] = Convert.ToString(mvarReader["n"]);
				Cells.Neighbours[1] = Convert.ToString(mvarReader["ne"]);
				Cells.Neighbours[2] = Convert.ToString(mvarReader["se"]);
				Cells.Neighbours[3] = Convert.ToString(mvarReader["s"]);
				Cells.Neighbours[4] = Convert.ToString(mvarReader["sw"]);
				Cells.Neighbours[5] = Convert.ToString(mvarReader["nw"]);
				return true;
			}
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
			if (mvarReader == null) ThrowConnectionException();
			// make sure the reader is of the correct type
			if (ReaderType == "CELL") this.ResetSupercells();
			// read the data
			if (!mvarReader.Read())
				return false;
			else 
			{
				Name = Convert.ToString(mvarReader["ID"]);
				InResistance = Convert.ToInt32(mvarReader["InResistance"]);
				OutResistance = Convert.ToInt32(mvarReader["OutResistance"]);
				return true;
			}
		}

		/// <summary>
		///		Returns the name of the datasource.  Calling this function will reset the main
		///		cell reader and the supercell reader.  If the datasource has not been
		///		connected, an InvalidOperationException exception is raised.
		/// </summary>
		/// <returns>The name of the datasource.</returns>
		protected override string GetName() 
		{
			if (mvarReader == null) ThrowConnectionException();
			// close the existing reader
			if (!mvarReader.IsClosed) mvarReader.Close();
			// reset the connection
			ResetConnection();
			// create the command required to extract the name
			OleDbCommand cmd = new OleDbCommand("Select * FROM [Name]", mvarConnection);
			mvarReader = cmd.ExecuteReader();
			// read the value
			mvarReader.Read();
			string Name = Convert.ToString(mvarReader["Name"]);
			// close this reader and open the reader on the main datasource
			mvarReader.Close();
			this.Reset();
			// return the name value
			return Name;
		}
		// *********************** private members ****************************************
		private string DB_CONN_STRING;
		private OleDbConnection mvarConnection;
		private OleDbDataReader mvarReader;
		private string ReaderType;

		// reset the connection to the datasource
		private void ResetConnection() 
		{
			ResetConnection(true);
		}

		// reset the connection to the datasource
		private void ResetConnection(bool CloseFirst) 
		{
			if (CloseFirst) mvarConnection.Close();			
			string FullName = null;
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
			if (!File.Exists(FullName)) 
				throw new FileNotFoundException(string.Format("Cell datasource \"{0}\" not found!!", FullName),
												FullName);
			
			// connect to the database
			mvarConnection = new OleDbConnection(DB_CONN_STRING + FullName);
			mvarConnection.Open();
		}

		// raise an exception indicating that the datasource is not connected
		private void ThrowConnectionException()
		{
			throw new InvalidOperationException("Datasource has not been connected.");
		}
	}
}
