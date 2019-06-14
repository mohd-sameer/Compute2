using System;
using System.Collections;
using IndexedHashTable;

namespace Rabies_Model_Core
{

	/// <summary>
	///		A list of supercells.  The supercells are keyed by their ID values in 
	///		this list.  Therefore, all supercells in this list must have unique ID values.
	///		Supercells in this list may be accessed either through their key value or
	///		through an integer index.
	/// </summary>
	public class cSuperCellList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize a supercell list.
		/// </summary>
		public cSuperCellList()
		{
			// create the list of values
			Values = new cIndexedHashTable(null);
		}

		// *********************** Properties ********************************************
		/// <summary>
		///		Retrieve a reference to the supercell at the passed key value (read-only).  If
		///		the key is not found, an ArgumentException exception is raised.
		/// </summary>
		public cSuperCell this[string key] 
		{
			get 
			{
				return (cSuperCell)Values[key];
			}
		}

		/// <summary>
		///		Retrieve a reference to the supercell at the passed index value (read-only).
		///		An ArgumentOutOfRangeException exception is raised if the index is less than
		///		zero or greater than the number of items in the list minus one.
		/// </summary>
		public cSuperCell this[int index] 
		{
			get 
			{
				return (cSuperCell)Values[index];
			}
		}

		/// <summary>
		///		The number of supercells in the list.
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
		///		Add a supercell to the supercell list.  The supercell will be keyed by 
		///		its ID.  If another cell in the list already has this ID, an ArgumentException
		///		exception will be raised.  If item is null, an ArgumentNullException exception
		///		is raised.
		/// </summary>
		/// <param name="item">The supercell to add to the list.</param>
		public void Add(cSuperCell item) 
		{
			if (item == null) throw new ArgumentNullException("item", "Cannot add a null item to the list");
			Values.Add(item.ID, item);
		}

		/// <summary>
		///		Remove the supercell with the passed ID from the list.
		/// </summary>
		/// <param name="ID">
		///		The ID of the supercell to remove.  An ArgumentNullException exception is
		///		raised if ID is null.
		///	</param>
		public void Remove(string ID) 
		{
			Values.Remove(ID);
		}

		/// <summary>
		///		Remove the supercell at the passed position from the list.
		/// </summary>
		/// <param name="index">
		///		The position of the supercell to remove.  An ArgumentOutOfRangeException
		///		exception is raised if the index is less than zero or greater than the number
		///		of items in the list minus one.
		///	</param>
		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the supercells.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		// ************************ Private Members **************************************
		// the table of values
		private cIndexedHashTable Values;
	}
}