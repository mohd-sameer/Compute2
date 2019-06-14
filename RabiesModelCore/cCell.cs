using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		An enumeration of neighbouring cell positions on a hexagonal grid of cells.
	/// </summary>
	public enum enumNeighbourPosition 
	{
		/// <summary>
		///		The position directly above the cell.
		/// </summary>
		Top = 0,
		/// <summary>
		///		The position above and to the right of the cell.
		/// </summary>
		TopRight = 1,
		/// <summary>
		///		The position below and to the right of the cell.
		/// </summary>
		BottomRight = 2,
		/// <summary>
		///		The position directly below the cell.
		/// </summary>
		Bottom = 3,
		/// <summary>
		///		The position below and to the left of the cell.
		/// </summary>
		BottomLeft = 4,
		/// <summary>
		///		The position above and to the left of the cell.
		/// </summary>
		TopLeft = 5
	}

	/// <summary>
	///		Represents a single hexagonal landscape cell in the rabies model.
	///		Each cell defines a property K, called the "carrying capacity".  How K is used in
	///		a particular implemetation of the model depends on how it is used in the mortality
	///		function(s) for the animal(s) in the model.  The value of K for each cell is
	///		predetirmined by the landscape being modeled and the type of animals in the model.
	///		Each cell belongs to a supercell.  A supercell represents a series of cells in
	///		which movement between those cells is unrestricted.  An attempt to move from
	///		a cell in one supercell to a cell in another supercell may fail because of
	///		resistance to either leaving the first supercell or to entering the second
	///		supercell.  The supercell concept allows natural barriers to movement in the
	///		landscape to be modelled.
	///		In a hexagonal grid, each cell has six neighbours.  These neighbours are
	///		stored in a list with index values of type enumNeighbour position.  The
	///		grid as a whole has a boundary.  Before attempting to access a reference to a
	///		boundary cell, use the IsBoundary function with the appropriate neighbour index
	///		to first make sure that there is a valid neighbour reference.  By default, when
	///		a cCell object is initialized, all neighbours are set to the boundary.  The
	///		user is responsible for setting	neighbour cells.
	/// </summary>
	public class cCell
	{
		// ******************** constructors **********************************************
		/// <summary>
		///		Initialize a cell object.  K is set to 0.
		/// </summary>
		/// <param name="ID">
		///		The ID of this cell.  If the ID is zero length, an ArgumentException
		///		exception is raised.
		///	</param>
		/// <param name="SuperCell">
		///		A reference to the supercell that this cell belongs to.  If SuperCell is
		///		null, an ArgumentException is raised.
		/// </param>
		public cCell(string ID, cSuperCell SuperCell)
		{
			InitClass(ID, SuperCell, 0, 0, 0);
		}

		/// <summary>
		///		Initiliaze a cell object with a passed K value.
		/// </summary>
		/// <param name="ID">
		///		The ID of this cell.  If the ID is zero length, an ArgumentException
		///		exception is raised.
		///	</param>
		/// <param name="SuperCell">
		///		A reference to the supercell that this cell belongs to.  If SuperCell is
		///		null, an ArgumentException is raised.
		/// </param>
		/// <param name="K">
		///		The "carrying capacity" of this cell. If K is set to less than zero, an 
		///		ArgumnetOutOfRangeException exception is raised.
		///	</param>
		public cCell(string ID, cSuperCell SuperCell, double K)
		{
			InitClass(ID, SuperCell, K, 0, 0);
		}

		/// <summary>
		///		Initilaize a cell object with a passed K value and a geographic location.
		///		Note that the geographic location is for reference only.  It does not
		///		affect the operation of the model and does not restrict which cells can
		///		be neighbours of which cells.
		/// </summary>
		/// <param name="ID">
		///		The ID of this cell.  If the ID is zero length, an ArgumentException
		///		exception is raised.
		///	</param>
		/// <param name="SuperCell">
		///		A reference to the supercell that this cell belongs to.  If SuperCell is
		///		null, an ArgumentException is raised.
		/// </param>
		/// <param name="K">
		///		The "carrying capacity" of this cell. If K is set to less than zero, an 
		///		ArgumnetOutOfRangeException exception is raised.
		///	</param>
		/// <param name="XLoc">The "X" geographic coordinate.</param>
		/// <param name="YLoc">The "Y" geographic coordinate.</param>
		public cCell(string ID, cSuperCell SuperCell, double K, double XLoc, double YLoc) 
		{
			InitClass(ID, SuperCell, K, XLoc, YLoc);
		}
		
		// a private common initilization function
		private void InitClass(string ID, cSuperCell SuperCell, double K, double XLoc, 
								double YLoc) 
		{
			// check the parameters to make sure that they are valid.  Throw exceptions if
			// they are not.
			// ID must not be 0 length.
			if (ID.Length == 0)
				throw new ArgumentException("ID must not be an empty string.", "ID");
			// SuperCell must be a valid reference
			if (SuperCell == null) ThrowSuperCellException();
			// K must be greater than or equal to 0
			if (K < 0) ThrowKException();
			// set the ID value and K
			mvarID = ID;
			mvarK = K;
			// add this cell to the supercell
			SuperCell.Add(this);
			// set the x,y location
			mvarXLoc = XLoc;
			mvarYLoc = YLoc;
			// create the neighbours array
			mvarNeighbours = new cCell[6];
			// create the list of animals
			mvarAnimals = new cAnimalList(null);
		}

		// ***************** properties ***************************************************
		/// <summary>
		///		A double precision value available to the user to do with as they please.  
		///		Changing the value of this property will not affect the cell in any way.
		///		Normal cell operations will not affect this value.
		/// </summary>
		public double UserValue;

		/// <summary>
		///		The ID of the cell (read-only).
		/// </summary>
		public string ID 
		{
			get 
			{
				return mvarID;
			}
		}

		/// <summary>
		///		The supercell that this cell belongs to (read-only).
		/// </summary>
		public cSuperCell SuperCell 
		{
			get 
			{
				return mvarSuperCell;
			}
			set 
			{
				mvarSuperCell = value;
			}
		}

		/// <summary>
		///		The "carrying capacity" of the cell.  An ArgumentOutOfRangeException exception
		///		is raised if an attempt is made to set this value to less than zero.
		///	</summary>
		public double K 
		{
			get 
			{
				return mvarK;
			}
			set 
			{
				// make sure value is not less than 0
				if (value < 0) ThrowKException();
				// set K
				mvarK = value;
			}
		}

		/// <summary>
		///		The "X" geographic location.  Note that the geographic location is for
		///		reference only.  It has no effect on the operation of the model.
		/// </summary>
		public double XLoc 
		{
			get 
			{
				return mvarXLoc;
			}
			set 
			{
				mvarXLoc = value;
			}
		}

		/// <summary>
		///		The "Y" geographic location.  Note that the geographic location is for
		///		reference only.  It has no effect on the operation of the model.
		/// </summary>
		public double YLoc 
		{
			get 
			{
				return mvarYLoc;
			}
			set 
			{
				mvarYLoc = value;
			}
		}

		/// <summary>
		///		A list of animals currently occupying the cell (read-only).
		/// </summary>
		public cAnimalList Animals 
		{
			get 
			{
				return mvarAnimals;
			}
		}

		// ********************* methods *************************************************
		/// <summary>
		///		Determines if the neighbouring cell at the passed index is the boundary.
		/// </summary>
		/// <param name="index">
		///		The index of the neighbour of interest.  This is an enumNeighbourPosition
		///		type.
		/// </param>
		/// <returns>True if the neighbour of interest is the boundary.</returns>
		public bool IsBoundary(enumNeighbourPosition index) 
		{
			return (mvarNeighbours[(int) index] == null);
		}

		/// <summary>
		///		A reference to one of the six neighbours of this cell.  This value is
		///		indexed by an enumNeighbourPosition type.  Before using this method, you
		///		should call the IsBoundary function with the same index to make sure that
		///		this neighbour is not the boundary.
		/// </summary>
		/// <param name="index">The neighbour position of interest.</param>
		/// <returns>
		///		The cell at the passed neighbour position.  If the neighbour is the boundary,
		///		null is returned.
		/// </returns>
		public cCell GetNeighbour(enumNeighbourPosition index)
		{
			return mvarNeighbours[(int) index];
		}
		
		/// <summary>
		///		Set the neighbouring cell of this cell at the passed index position.  
		///		Setting a neighbour to 'null' sets the neighbour to the boundary.
		/// </summary>
		/// <param name="index">The desired position of the neighbour.</param>
		/// <param name="item">The neighbouring cell.</param>
		public void SetNeighbour(enumNeighbourPosition index, cCell item)
		{
			mvarNeighbours[(int) index] = item;
		}

        // ******************** To string function **************************************

        /// <summary>
        /// Get the string representing this object
        /// </summary>
        /// <returns>The string representing this object</returns>
        public override string ToString()
        {
            return this.ID;
        }

        // ***************** internal methods *********************************************
		/// <summary>
		///		Set the supercell of this cell.  This is an internal function and can only
		///		be called by objects within the Rabies_Model_Core namespace.
		/// </summary>
		/// <param name="SuperCell">
		///		The reference to the desired super cell.  An ArgumentException exception is
		///		raised if this value is null.
		/// </param>
		internal void SetSuperCell(cSuperCell SuperCell) 
		{
			// make sure passed supercell is not null
			if (SuperCell == null) ThrowSuperCellException();
			// set the value
			mvarSuperCell = SuperCell;
		}

		// ***************** private members **********************************************
		// the ID of this cell
		private string mvarID;
		// the supercell of this cell
		private cSuperCell mvarSuperCell;
		// the carrying capacity of this cell
		private double mvarK;
		// the x,y geographic coordinates of this cell
		private double mvarXLoc, mvarYLoc;
		// an array of neighbours
		private cCell[] mvarNeighbours;
		// a list of animals occupying this cell
		private cAnimalList mvarAnimals;

		// throw an exception indicating that an attempt was made to set K less than 0
		private void ThrowKException()
		{
			throw new ArgumentOutOfRangeException("K", "K must be >= 0.");
		}

		// throw an exception indicating that an attempt was made to set the supercell object
		// to null
		private void ThrowSuperCellException()
		{
			throw new ArgumentException("SuperCell must be a valid reference", "SuperCell");
		}
	}
}
