using System;
using System.Collections;
using System.Collections.Generic;
using IndexedHashTable;

namespace Rabies_Model_Core
{

	/// <summary>
	///		A list of vaccines.  Vaccines in this list are accessed via a numerical index.
	/// </summary>
	public class cVaccineList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize a vaccine list.
		/// </summary>
		public cVaccineList()
		{
			// create the list of values
			Values = new List<cVaccine>();
		}

		// *********************** Properties ********************************************
		/// <summary>
		///		Retrieve a reference to the vaccine at the passed index value (read-only).
		///		An ArgumentOutOfRangeException exception is raised if the index is less than
		///		zero or greater than the number of items in the list minus one.
		/// </summary>
		public cVaccine this[int index] 
		{
			get 
			{
				return Values[index];
			}
		}

		/// <summary>
		///		The number of vaccines in the list.
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
		///		Add avaccine to the vaccine list.
		/// </summary>
		/// <param name="item">
		///		The vaccine to add to the list.  If item is null, an ArgumentNullException
		///		exception is raised.
		/// </param>
		public virtual void Add(cVaccine item) 
		{
			Values.Add(item);
		}

		/// <summary>
		///		Remove the vaccine at the passed position from the list.
		/// </summary>
		/// <param name="index">
		///		The position of the vaccine to remove.  An ArgumentOutOfRangeException
		///		exception is raised if the index is less than zero or greater than the number
		///		of items in the list minus one.
		///	</param>
		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the vaccines.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		// ************************ Private Members **************************************
		// the table of values
		private List<cVaccine> Values;
	}
}