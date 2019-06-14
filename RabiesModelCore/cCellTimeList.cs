using System;
using System.Collections;
using System.Collections.Generic;

namespace Rabies_Model_Core
{

	/// <summary>
	///		A list of cell-time associations.
	/// </summary>
	public class cCellTimeList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize a CellTime list.  Cell-Time associations stored in this list are
		///		accessed via an integer index.
		/// </summary>
		public cCellTimeList()
		{
			// create the list of values
			Values = new List<cCellTime>();
		}

		// *********************** Properties ********************************************
		/// <summary>
		///		Retrieve a reference to the cCellTime object at the passed index value
		///		(read-only).    An ArgumentOutOfRangeException exception is raised if the index
		///		is less than zero or greater than the number of items in the list minus one.
		/// </summary>
		public cCellTime this[int index] 
		{
			get 
			{
				return Values[index];
			}
		}

		/// <summary>
		///		The number of cCellTime objects in the list.
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
		///		Add a cCellTime object the list.
		/// </summary>
		/// <param name="item">
		///		The cell to add to the list.  If item is null, an ArgumentNullException
		///		exception is raised. 
		///	</param>
		public virtual void Add(cCellTime item) 
		{
			Values.Add(item);
		}

		/// <summary>
		///		Remove the cCellTime object the passed position from the list.
		/// </summary>
		/// <param name="index">
		///		The position of the cell to remove.  An ArgumentOutOfRangeException
		///		exception is raised if the index is less than zero or greater than the number
		///		of items in the list minus one.
		///	</param>
		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Make a deep copy of this list.
		/// </summary>
		/// <returns>The cCellTimeList that is a copy of this list.</returns>
		public cCellTimeList Clone()
		{
			// create the new list
			cCellTimeList CloneList = new cCellTimeList();
			foreach (cCellTime item in this) {
				CloneList.Add(item);
			}
			// return the new list
			return CloneList;
		}

		/// <summary>
		///		Get an enumerator that can iterate through the cells.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		// ************************ Private Members **************************************
		// the table of values
		private List<cCellTime> Values;
	}
}
