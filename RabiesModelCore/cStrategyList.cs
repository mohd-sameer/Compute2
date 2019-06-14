using System;
using System.Collections;
using System.Collections.Generic;
using IndexedHashTable;

namespace Rabies_Model_Core
{

	/// <summary>
	///		A list of strategies.  Strategies stored in this list are accessed via an integer
	///		index.
	/// </summary>
	public class cStrategyList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize a strategy list.
		/// </summary>
		public cStrategyList()
		{
			// create the list of values
			Values = new List<cStrategy>();
		}

		// *********************** Properties ********************************************
		/// <summary>
		///		Retrieve a reference to the strategy at the passed index value (read-only).  An
		///		ArgumentOutOfRangeException exception is raised if the index is less than zero
		///		or greater than the number of items in the list minus one.
		/// </summary>
		public cStrategy this[int index] 
		{
			get 
			{
				return Values[index];
			}
		}

		/// <summary>
		///		The number of strategies in the list.
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
		///		Add a strategy to the strategy list.
		/// </summary>
		/// <param name="item">
		///		The strategy to add to the list.  If item is null, an ArgumentNullException
		///		exception is raised.
		///	</param>
		public virtual void Add(cStrategy item) 
		{
			Values.Add(item);
		}

		/// <summary>
		///		Remove the strategy at the passed position from the list.
		/// </summary>
		/// <param name="index">
		///		The position of the strategy to remove.  An ArgumentOutOfRangeException
		///		exception is raised if the index is less than zero or greater than the number
		///		of items in the list minus one.
		///	</param>
		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the strategies.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		/// <summary>
		///		Sort the list by strategy year and week.
		/// </summary>
		public void Sort()
		{
			// don't bother with less than two items
			if (Values.Count < 2) return;
			const double ALN2I = 1.442695022;
			const float TINY = 1.0e-5F;
			// sort via a shell sort (good enough for the expected number of items in this list)
			// see Press et. al p.245
			int nn, m, j, i, lognb2;
			cStrategy Temp;
			lognb2 = Convert.ToInt32(Math.Log((double)Values.Count) * ALN2I + TINY);
			m = Values.Count;
			for (nn=1; nn<=lognb2; nn++) 
			{
				m >>= 1;
				for (j=m+1; j<=Values.Count; j++) 
				{
					i=j-m;
					Temp = Values[j-1];
					while (i>=1 && Temp.CompareTo(Values[i - 1]) == -1) 
					{
						Values[i + m - 1] = Values[i - 1];
						i -= m;
					}
					Values[i + m - 1] = Temp;
				}
			}
		}

		// ************************ Private Members **************************************
		// the table of values
		private List<cStrategy> Values;
	}
}