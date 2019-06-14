using System;
using System.Collections;
using IndexedHashTable;
using Random;
using System.Collections.Generic;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A list of cells.  This class is used by the rabies model to store the main list of
	///		cells in the landscape and to keep track of smaller list of cells in other objects
	///		(for example, every animal keeps a list of the cells it has occupied).  The cells
	///		stored in this list are keyed by their ID values.  Therefore, all cells in this
	///		list must have unique ID values.  Cells in this list may be accessed either 
	///		through their key value or through an integer index.
	/// </summary>
    /// <param name="ScrambleRandom">A random number generator to use if the list is to be scrambled</param>
	public class cCellList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize a cell list.
		/// </summary>
		public cCellList(cUniformRandom ScrambleRandom)
		{
			// create the list of values
			Values = new cIndexedHashTable(ScrambleRandom);
		}

		// *********************** Properties ********************************************
		/// <summary>
		///		The name of this list of cells.  Changing this value does not affect the
		///		operation of the class in any way.
		/// </summary>
		public string Name;

		/// <summary>
		///		Retrieve a reference to the cell at the passed key value (read-only).  If the 
		///		key is not found, an ArgumentException exception is raised.
		/// </summary>
		public cCell this[string key] 
		{
			get 
			{
				return (cCell)Values[key];
			}
		}

		/// <summary>
		///		Retrieve a reference to the cell at the passed index value (read-only).  An 
		///		ArgumentOutOfRangeException exception is raised if the index is less than zero
		///		or greater than the number of items in the list minus one.
		/// </summary>
		public cCell this[int index] 
		{
			get 
			{
				return (cCell)Values[index];
			}
		}

		/// <summary>
		///		The number of cells in the list (read-only).
		/// </summary>
		public int Count 
		{
			get 
			{
				return Values.Count;
			}
		}

		// ************************ Methods **********************************************
		/// <summary>
		///		Add a cell to the cell list.  The cell will be keyed by its ID.  If another
		///		cell in the list already has this ID, an ArgumentException exception will be
		///		raised.  If item is null, an ArgumentNullException exception is raised.
		/// </summary>
		/// <param name="item">The cell to add to the list.</param>
		public virtual void Add(cCell item) 
		{
			if (item == null) throw new ArgumentNullException("item", "Cannot add a null item to the list");
			Values.Add(item.ID, item);
		}

		/// <summary>
		///		Remove the cell with the passed ID from the list.
		/// </summary>
		/// <param name="ID">
		///		The ID of the cell to remove.  An ArgumentNullException exception is raised if
		///		ID is null.
		/// </param>
		public void Remove(string ID) 
		{
			Values.Remove(ID);
		}

		/// <summary>
		///		Remove the cell at the passed position from the list.
		/// </summary>
		/// <param name="index">
		///		The position of the cell to remove.  An ArgumentOutOfRangeException exception
		///		is raised if the index is less than zero or greater than the number of items
		///		in the list minus one.
		///	</param>
		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the cells.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		/// <summary>
		///		Determine whether the list contains the passed key.
		/// </summary>
		/// <param name="key">The key to look for.</param>
		/// <returns>True if the key is found in this list.</returns>
		public bool ContainsKey(string key) 
		{
			return Values.ContainsKey(key);
		}

		/// <summary>
		///		Scramble the order of the cell objects in this list.
		/// </summary>
		public void ScrambleOrder()
		{
			Values.ScrambleOrder();
		}        

        // ************************ Private Members **************************************
        // the table of values
        private cIndexedHashTable Values;
	}
}
