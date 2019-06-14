using System;
using System.Collections;
using System.Collections.Generic;
using IndexedHashTable;

namespace Rabies_Model_Core
{

	/// <summary>
	///		A list of infections.  Infections stored in this list are accessed via an integer
	///		index.
	/// </summary>
	public class cInfectionList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize an infection list.
		/// </summary>
		public cInfectionList()
		{
			// create the list of values
			Values = new List<cInfection>();
		}

		// *********************** Properties ********************************************
		/// <summary>
		///		Retrieve a reference to the infection at the passed index value (read-only).  An
		///		ArgumentOutOfRangeException exception is raised if the index is less than zero
		///		or greater than the number of items in the list minus one.
		/// </summary>
		public cInfection this[int index] 
		{
			get 
			{
				return Values[index];
			}
		}

		/// <summary>
		///		The number of infections in the list.
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
		///		Add a infection to the infection list.
		/// </summary>
		/// <param name="item">
		///		The infection to add to the list.  If item is null, an ArgumentNullException
		///		exception is raised.
		///	</param>
		public virtual void Add(cInfection item) 
		{
			Values.Add(item);
		}

		/// <summary>
		///		Remove the infection at the passed position from the list.
		/// </summary>
		/// <param name="index">
		///		The position of the infection to remove.  An ArgumentOutOfRangeException
		///		exception is raised if the index is less than zero or greater than the number
		///		of items in the list minus one.
		///	</param>
		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the infections.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		// ************************ Private Members **************************************
		// the table of values
		private List<cInfection> Values;
	}
}
