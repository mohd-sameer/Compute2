using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A class for retrieving metadata stored in animal datasources.  All animal
	///		datasources should define this data.
	/// </summary>
	public class cCellsMetadata
	{
		/// <summary>
		///		The name of the individual who created the datasource.
		/// </summary>
		public string CreatorName;
		/// <summary>
		///		The organization the creator of the datasource is affiliated with.
		/// </summary>
		public string CreatorAffiliation;
		/// <summary>
		///		Contact information for the creator of the datasource.
		/// </summary>
		public string CreatorContactInfo;
		/// <summary>
		///		The date and time the datasource was created.
		/// </summary>
		public string CreationDate;
		/// <summary>
		///		A description of the cells in the datasource.
		/// </summary>
		public string Description;
	}

	/// <summary>
	///		A class representing the raw data required to build a cell.
	/// </summary>
	public class cCellData 
	{
		/// <summary>
		///		Initialize.
		/// </summary>
		public cCellData()
		{
			// create the neighbours array.
			Neighbours = new string[6];
		}

		/// <summary>
		///		The ID of the cell.
		/// </summary>
		public string ID;
		/// <summary>
		///		The ID of the supercell that this cell will belong to.
		/// </summary>
		public string SuperCellID;
		/// <summary>
		///		The ID's of the six neighbouring cells in the order of top, top right, bottom
		///		right, bottom, bottom left, top left.
		/// </summary>
		public string[] Neighbours;
		/// <summary>
		///		The carrying capacity of the cell.
		/// </summary>
		public double K;
		/// <summary>
		///		The 'X'-coordinate of the location of the cell.
		/// </summary>
		public double XLoc;
		/// <summary>
		///		The 'Y'-coordinate of the location of the cell.
		/// </summary>
		public double YLoc;
	}

	/// <summary>
	///		An abstract base class for all classes implementing a datasource for cells.  This
	///		class cannot directly instantiated.  It must be overridden to provide specific
	///		functionality for each datasource type.
	///		The cCellsDataSource class implements the IDisposable interface; the dispose method
	///		calls the Disconnect method to free up any resources that may be tied up by the
	///		datasource.  The Dispose method will ignore an errors raised by the Disconnect
	///		method.
	/// </summary>
	public abstract class cCellsDataSource : IDisposable
	{
		// ************************** Constructor *****************************************
		/// <summary>
		///		Initialize a cells datasource.
		/// </summary>
        /// <param name="IsUnix">A boolean value indocating that this is a Unix based datasource</param>
		public cCellsDataSource(bool IsUnix) 
        {
            mvarIsUnix = IsUnix;
        }

		/// <summary>
		///		Destructor.  Disconnect the datasource if it is not already disconnected.
		/// </summary>
		~cCellsDataSource() 
		{
			this.Dispose();
		}
		
		// ************************* Properties ********************************************
		/// <summary>
		///		The name of the data source.
		/// </summary>
		public string DataSourceName;

		/// <summary>
		///		The path to the data source.
		/// </summary>
		public string DataSourcePath;

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
		///		Open a data source and connect to it.
		/// </summary>
		public abstract void Connect();

		/// <summary>
		///		Disconnect from and close the data source.
		/// </summary>
		public abstract void Disconnect();

		/// <summary>
		///		Get the path to the MapGuide map that corresponds to this cell datasource and
		///		the name of the map layer that contains the cells.
		/// </summary>
		/// <param name="Path">The path to the map.  This may be a local path or a URL.</param>
		/// <param name="CellLayer">The name of the map layer containing the cells.</param>
		public abstract void GetMapInfo(ref string Path, ref string CellLayer);
		
		/// <summary>
		///		Retrieve metadata from the datasource.
		/// </summary>
		/// <param name="Metadata">
		///		The cCellsMetadata object into which the metadata is written.
		///	</param>
		public abstract void GetMetadata(cCellsMetadata Metadata);

		/// <summary>
		///		Read cell data from the data source and fill the passed cell list.  This
		///		method will raise an exception if 1) a supercell ID in the database is not
		///		found in the Background assigned to this class (ArgumentException exception) or
		///		2) a cell is assigned a reference to a neighbour that does not exist
		///		(ArgumentException exception).
		/// </summary>
		/// <param name="Cells">
		///		The cell list to fill.  The cells are added to any that are already in the
		///		list.  An ArgumentNullException exception is raised if Cells is null.
		/// </param>
		/// <param name="SuperCells">
		///		A list of Supercells that these cells will belong to.  An ArgumentNullException
		///		is raised if SuperCells is null.  An ArgumentException is raised if SuperCells
		///		is an empty list.
		/// </param>
		public void GetCellData(cCellList Cells, cSuperCellList Supercells) 
		{
			// make sure Cells is not null
			if (Cells == null)
				throw new ArgumentNullException("Cells", "Cells must not be null.");
			// make sure Supercells is not null
			if (Supercells == null) ThrowSupercellsException();
			// the supercell list must contain at least one super cell
			if (Supercells.Count == 0)
				throw new ArgumentException("Supercell list must contain at least one supercell.",
											"Supercells");
			// retrieve the name of this list of cells
			Cells.Name = this.GetName();
			// create a cell data object
			cCellData CellData = new cCellData();
			// loop, loading cell data one at a time
			this.Reset();
			while (this.GetNextCellRecord(CellData)) {
				// create the new cell
				cCell Cell = new cCell(CellData.ID, Supercells[CellData.SuperCellID],
										CellData.K,	CellData.XLoc, CellData.YLoc);
				Cells.Add(Cell);
			}
			// loop again, this time assigning neighbours.  If an invalid refence exception
			// is thrown, then the neighbour ID is invalid.
			this.Reset();
			while (this.GetNextCellRecord(CellData)) {
				// loop through the six neighbour positions
				for (int i = 0; i < 6; i++) {
					// note: if the neighbour value in the datasource is "b", this indicates
					// that the neighbour is on the bouandary
					if (CellData.Neighbours[i] == "b") {
						Cells[CellData.ID].SetNeighbour((enumNeighbourPosition) i, null);
					}
					else {
						Cells[CellData.ID].SetNeighbour((enumNeighbourPosition) i, 
														Cells[CellData.Neighbours[i]]);
					}
				}
			}
		}

		/// <summary>
		///		Read supercell data from the data source and fill the passed supercell
		///		list.
		/// </summary>
		/// <param name="Supercells">
		///		The cell list to fill.  The cells are added to any that are already in the
		///		list. An ArgumentNullException exception is raised if Supercells is null.
		/// </param>
		public void GetSuperCellData(cSuperCellList Supercells) 
		{
			// make sure Supercells is not null
			if (Supercells == null) ThrowSupercellsException();
			// reset the supercell recordset
			this.ResetSupercells();
			string Name = "";
			int InResistance = 0, OutResistance = 0;
			while (this.GetNextSupercellRecord(ref Name, ref InResistance, 
												ref OutResistance)) {
				// create a new supercell
				cSuperCell SuperCell = new cSuperCell(Name, InResistance, OutResistance);
				// add the new supercell to the supercell list
				Supercells.Add(SuperCell);
			}
		}

		/// <summary>
		///		Dispose this object (calls Disconnect).
		/// </summary>
		public void Dispose()
		{
			if (IsDisposed) return;
            try
            {
                if (mvarConnected) Disconnect();
            }
            catch { }
            finally
            {
                IsDisposed = true;
            }
		}

		// ***************************** Protected Methods *********************************
		/// <summary>
		///		Retrieve the name of this list of cells from the data source.
		/// </summary>
		/// <returns>The name of the cells list source.</returns>
		protected abstract string GetName(); 

		/// <summary>
		///		Flag indicating whether or not the datasource is connected.
		/// </summary>
		protected bool mvarConnected;

        /// <summary>
        ///     Flag indicating that this is a Unix based datasource
        /// </summary>
        protected bool mvarIsUnix;

		/// <summary>
		///		Retrieves a single cell record from the data source and places it in the
		///		passed cell data object.
		/// </summary>
		/// <param name="Data">The cell data object that will contain the cell data.</param>
		/// <returns>
		///		True if the retrieval was successful, False if there are no more records to
		///		retrieve.
		///	</returns>
		protected abstract bool GetNextCellRecord(cCellData Data);

		protected abstract bool GetNextSupercellRecord(ref string Name, 
														ref int InResistance,
														ref int OutResistance);

		/// <summary>
		///		Reset the datasource so that the next call to GetNextCellRecord returns the
		///		first cell record.
		/// </summary>
		protected abstract void Reset();

		/// <summary>
		///		Reset the datasource so that the next call to GetNextSupercellRecord returns
		///		the first supercell record.
		/// </summary>
		protected abstract void ResetSupercells();

		// ***************************** Private Members ***********************************
		private bool IsDisposed = false;
		// raise an exception indicating Supercells is null
		private void ThrowSupercellsException()
		{
			throw new ArgumentNullException("Supercells", "Supercells cannot be null.");
		}
	}
}