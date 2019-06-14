using System;
using System.Collections;
using System.Collections.Generic;

namespace Rabies_Model_Core
{
	/// <summary>
	///		The 5 possible winter types: VerySevere, Severe, Normal, Mild, VeryMild.  No
	///		attempt is made to define what actually defines the different winter types.
	///	</summary>
	public enum enumWinterType 
	{
		/// <summary>
		///		Very severe winter.
		/// </summary>
		VerySevere = 1,
		/// <summary>
		///		Severe winter.
		/// </summary>
		Severe = 2,
		/// <summary>
		///		Normal winter.
		/// </summary>
		Normal = 3,
		/// <summary>
		///		Mild winter.
		/// </summary>
		Mild = 4,
		/// <summary>
		///		Very mild winter.
		/// </summary>
		VeryMild = 5
	}

    /// <summary>
    ///    A list of winter types indexed by integer.
    /// </summary>
	public class cWinterTypeList : IEnumerable
	{
		// ********************* constructor **************************************
		/// <summary>
		///		Initialize a WinterType list.
		/// </summary>
		public cWinterTypeList() 
		{
			// initialize the arraylist that will hold the values
			Values = new List<enumWinterType>();
		}

		// ********************* properties ***************************************
		/// <summary>
		///		Return an indexed value from the list as an enumWinterType.  An 
		///		ArgumentOutOfRangeException exception is raised if the index is less than zero
		///		or greater than the number of items in the list minus one.
		/// </summary>
		public enumWinterType this[int index]
		{
			get 
			{
				return Values[index];
			}
			set 
			{
				Values[index] = value;
			}
		}

		/// <summary>
		///		The number of items in the list (read-only).
		/// </summary>
		public int Count 
		{
			get 
			{
				return Values.Count;
			}
		}

		// ********************* methods ******************************************
		/// <summary>
		///     Add a new winter type to the list.  The new value is put at the end
		///     of the list.
		/// </summary>
		/// <param name="eWinterType">
		///		The winter type: Very Severe, Severe, Normal, Mild, or Very Mild.
		/// </param>
		public void Add(enumWinterType eWinterType) 
		{
			Values.Add(eWinterType);
		}

		/// <summary>
		///		Remove the item at the passed index from the list.
		/// </summary>
		/// <param name="index">The index of the item to be removed.</param>
		public void RemoveAt(int index) 
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the winter types.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		// ******************* private members *************************************
		// the arraylist that holds the values
		private List<enumWinterType> Values;
	}
}