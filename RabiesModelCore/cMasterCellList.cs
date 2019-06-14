using System;
using Random;

namespace Rabies_Model_Core
{
	/// <summary>
	///		The master list of all cells in the model.  This is a list maintained by the
	///		Background object of all cells in the background.  This object also has a method
	///		for calculating paths through the cells.  The cells	stored in this list are keyed
	///		by their ID values.  Therefore, all cells in this list must have unique ID values.
	///		Cells in this list may be accessed either through their key value or through an
	///		integer index.
	/// </summary>
	public class cMasterCellList : cCellList
	{
		// ************************** Constructors *************************************
		/// <summary>
		///		Initialize the master cell list.
		/// </summary>
		/// <param name="Background">
		///		The background object that owns this master cell list.  An ArgumentNullEception
		///		exception is raised if Background is null.
		/// </param>
        /// <param name="ScrambleRandom">A random number generator to use if the list is to be scrambled</param>
		public cMasterCellList(cBackground Background, cUniformRandom ScrambleRandom ) 
            : base(ScrambleRandom)
		{
			// make sure background is valid
			if (Background == null) 
				throw new ArgumentNullException("Background", 
					"Background must reference a valid background object.");
			// set the background
			mvarBackground = Background;
		}

		// ************************** Properties ****************************************
		/// <summary>
		///		The background object that owns this master list of cells (read-only).
		/// </summary>
		public cBackground Background 
		{
			get 
			{
				return mvarBackground;
			}
		}

		// ************************** Methods *******************************************
		/// <summary>
		///		Calculate a path through a series of cells with a bias in the specified
		///		direction.  Note, the returned path may not be as long as expected for two
		///		possible reasons; 1) The path encounters the boundary of the region under
		///		study or 2) The path attempts to cross a supercell boundary and cannot
		///		overcome the resistances involved to do so.
		/// </summary>
		/// <param name="StartCellID">
		///		The ID of the cell at the start of the path.  If if a cell with the passed ID
		///		is not in the list, an ArgumentException exception is raised.
		///	</param>
		/// <param name="PathLength">
		///		The number of cells the path should pass through.
		/// </param>
		/// <param name="DirectionBias">The directional bias of the path.</param>
		/// <returns>
		///		A cCellList containing the cells within the path in order to the end of
		///		the path.  The starting cell IS NOT included in this list.  
		///	</returns>
		public cCellList CalculatePath(string StartCellID, int PathLength, 
										enumNeighbourPosition DirectionBias) 
		{
			cCellList Path = new cCellList(null);
			cCell CurrentCell = this[StartCellID];
			cCell NextCell;
			int RanNum;
			enumNeighbourPosition Direction;
			// if requested path is zero length or less, then do nothing
			if (PathLength > 0) {
				// loop for desired path length
				for (int i = 0; i < PathLength; i++) {
					// get a Random number from the random number generator
					//mvarBackground.RandomNum.MinValue = 0;
					//mvarBackground.RandomNum.MaxValue = 100;
					RanNum = mvarBackground.RandomNum.IntValue(1, 100);
					// get a direction for the next cell based on the value of the
					// random number
					if (RanNum <= 20) {
						Direction = DirectionBias - 1;
						if (Direction < 0) Direction += 6;
					}
					else if (RanNum <= 80) {
						Direction = DirectionBias;
					}
					else {
						Direction = DirectionBias + 1;
						if ((int) Direction > 5) Direction -= 6;
					}
					// now try to get the neighbour.  If this is a boundary cell, we can go
					// no further and will stop calculating the path.
					// is this neighbour on the boundary.  If it is, break out of the loop
					// immediately.
					if (CurrentCell.IsBoundary(Direction)) break;
					// get the neighbouring cell
					NextCell = CurrentCell.GetNeighbour(Direction);
					// is the next cell in the same super group as the current cell.
					// If not see if we can overcome both the exiting and entering
					// resistances.
					if (NextCell.SuperCell != CurrentCell.SuperCell) {
						// the two cells do not share the same supercell.  We must check
						// resistances
						// check out resistance of current cell.  If we do not overcome it,
						// stop here.
						if (CurrentCell.SuperCell.OutResistance > 0) {
							//mvarBackground.RandomNum.MinValue = 0;
							//mvarBackground.RandomNum.MaxValue = 100;
							if (Background.RandomNum.IntValue(1, 100) <= CurrentCell.SuperCell.OutResistance) break;
						}
						// check in resistance of next cell.  If we do not overcome it,
						// stop here
						if (NextCell.SuperCell.InResistance > 0) {
							//mvarBackground.RandomNum.MinValue = 0;
							//mvarBackground.RandomNum.MaxValue = 100;
							if (Background.RandomNum.IntValue(1, 100) <= NextCell.SuperCell.InResistance) break;
						}
					}
					// add the neigbour to our list
					try {
						Path.Add(NextCell);
					}
					catch {
						return Path;
					}
					// now make next cell the current cell
					CurrentCell = NextCell;
				}
			}
			// return the calculted path
			return Path;
		}

    // ************************** Private members ***********************************
    private cBackground mvarBackground;
	}
}
