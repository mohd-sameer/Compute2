using System;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A group of related cells.  If an animal attemps to move into or out of a supercell
	///		it may face "resistance" to that movement.  If it cannot overcome the "resistance"
	///		it must either stop moving or seek another path.  A resistance value of 0 indicates
	///		no resistance.  A value of 100 indicates absolute resistance, that cannot be
	///		overcome.  Supercells allow the modelling of landscape barriers.  Please note:
	///		there is no requirement that the cells in a supercell be physically connected to
	///		one another.
	/// </summary>
	public class cSuperCell : cCellList
	{
		// ********************* Constructors *******************************************
		/// <summary>
		///		Initialize a supercell.  In resistance and out resistance are set to 0.
		/// </summary>
		/// <param name="ID">
		///		The ID of the supercell.  An ArgumentException exception is raised if the
		///		ID has zero length.
		/// </param>
		public cSuperCell(string ID) : base(null)
		{
			InitClass(ID, 0, 0);
		}

		/// <summary>
		///		Initialize a supercell, specifying the in resistance.  Out resistance is
		///		set to 0.
		/// </summary>
		/// <param name="ID">
		///		The ID of the supercell.  An ArgumentException exception is raised if the
		///		ID has zero length.
		///	</param>
		/// <param name="InResistance">
		///		The resistance to entering this supercell.  An ArgumentOutOfRangeException
		///		exception is raised if InResistance is not between 0 and 100.
		///	</param>
		public cSuperCell(string ID, int InResistance) : base(null)
		{
			InitClass(ID, InResistance, 0);
		}

		/// <summary>
		///		Initialize a supercell specifying both in resistance and out resistance.
		/// </summary>
		/// <param name="ID">
		///		The ID of the supercell.  An ArgumentException exception is raised if the
		///		ID has zero length.
		///	</param>
		/// <param name="InResistance">
		///		The resistance to entering this supercell.  An ArgumentOutOfRangeException
		///		exception is raised if InResistance is not between 0 and 100.
		///	</param>
		/// <param name="OutResistance">
		///		The resistance to leaving this supercell.  An ArgumentOutOfRangeException
		///		exception is raised if OutResistance is not between 0 and 100.
		///	</param>
		public cSuperCell(string ID, int InResistance, int OutResistance) : base(null)
		{
			InitClass(ID, InResistance, OutResistance);
		}

		// common initilization code.
		private void InitClass(string ID, int InResistance, int OutResistance) 
		{
			// make sure that the ID is not zero length.
			if (ID.Length == 0)
				throw new ArgumentException("ID cannot be zero length.", "ID");
			// make sure InResistance is correct.
			if (InResistance < 0 || InResistance > 100)
				ThrowResistanceOutOfRangeException("InResistance");
			// make sure OutResistance is correct
			if (OutResistance < 0 || OutResistance > 100)
				ThrowResistanceOutOfRangeException("OutResistance");
			// set the class values
			mvarID = ID;
			mvarInResistance = InResistance;
			mvarOutResistance = OutResistance;
		}
		
		// ********************* Properties *********************************************
		/// <summary>
		///		The ID of this supercell (read-only).
		/// </summary>
		public string ID
		{
			get 
			{
				return mvarID;
			}
		}

		/// <summary>
		///		The resistance to entering the group of cells defined by this supercell.  An
		///		ArgumentOutOfRangeException exception is raised if an attempt is made to set
		///		this value to less than 0 or greater than 100.
		/// </summary>
		public int InResistance 
		{
			get 
			{
				return mvarInResistance;
			}
			set 
			{
				// throw an exception if the valu is no between 0 and 100
				if (value < 0 || value > 100) 
					ThrowResistanceOutOfRangeException("InResistance");
				mvarInResistance = value;
			}
		}

		/// <summary>
		///		The resistance to leaving the group of cells defined by this supercell.  An
		///		ArgumentOutOfRangeException exception is raised if an attempt is made to set
		///		this value to less than 0 or greater than 100.
		/// </summary>
		public int OutResistance
		{
			get 
			{
				return mvarOutResistance;
			}
			set 
			{
				// throw an exception if the value is no between 0 and 100
				if (value < 0 || value > 100) 
					ThrowResistanceOutOfRangeException("InResistance");
				mvarOutResistance = value;
			}
		}

		// ********************* Methods ************************************************
		/// <summary>
		///		Add a cell to this supercell.  The supercell property of the cell is set
		///		to this supercell.
		/// </summary>
		/// <param name="item">
		///		The cell to add to this supercell.  An ArgumentNullException exception is
		///		raised if item is null.
		/// </param>
		public override void Add(cCell item) 
		{
			// throw an exception if item is null
			if (item == null) throw new ArgumentNullException("item", "Item cannot be null.");
			// assign the passed cell to this supercell
			item.SetSuperCell(this);
			// call the base add method
			base.Add(item);
		}

        /// <summary>
        /// Get the string representing this object
        /// </summary>
        /// <returns>The string representing this object</returns>
        public override string ToString()
        {
            return string.Format("{0} (In: {1}% Out{2}%)", this.Name, this.InResistance, this.OutResistance);
        }

		// ********************* Private Members ****************************************
		// resistance values
		private int mvarInResistance, mvarOutResistance;
		// super cell ID
		private string mvarID;
		// throw an exception indicating a resistance value is out of range
		private void ThrowResistanceOutOfRangeException(string Param)
		{
			throw new ArgumentOutOfRangeException(Param, "Resistance values must be between 0 and 100.");
		}
	}
}
