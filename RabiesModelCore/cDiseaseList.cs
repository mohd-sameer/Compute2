using System;
using System.Collections;
using IndexedHashTable;
using Random;

namespace Rabies_Model_Core
{

	/// <summary>
	///		A list of diseases.  The diseases are keyed by their names in this list.
	///		Therefore, all diseases in this list must have unique names.  Diseases in this
	///		list may be accessed either through their key value or through an integer index.
	/// </summary>
	public class cDiseaseList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize a disease list.
		/// </summary>
        /// <param name="ScrambleRandom">A random number generator to use if the list is to be scrambled</param>
		public cDiseaseList(cUniformRandom ScrambleRandom)
		{
			// create the list of values
			Values = new cIndexedHashTable(ScrambleRandom);
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cDiseaseList: cDiseaseList");
        }

		// *********************** Properties ********************************************
		/// <summary>
		///		Retrieve a reference to the disease at the passed key value (read-only).  If
		///		the name is not found, an ArgumentException exception is raised.
		/// </summary>
		public cDisease this[string name] 
		{
			get 
			{
				return (cDisease)Values[name];
			}
		}

		/// <summary>
		///		Retrieve a reference to the disease at the passed index value (read-only).
		///		An ArgumentOutOfRangeException exception is raised if the index is less than
		///		zero or greater than the number of items in the list minus one.
		/// </summary>
		public cDisease this[int index] 
		{
			get 
			{
				return (cDisease)Values[index];
			}
		}

		/// <summary>
		///		The number of diseases in the list.
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
		///		Add a disease to the disease list.  The disease will be keyed by its name.  If
		///		another disease in the list already has this name, an ArgumentException
		///		exception will be raised.  If item is null, an ArgumentNullException exception
		///		is raised.
		/// </summary>
		/// <param name="item">The disease to add to the list.</param>
		public virtual void Add(cDisease item) 
		{
			Values.Add(item.Name, item);
		}

		/// <summary>
		///		Remove the disease with the passed name from the list.
		/// </summary>
		/// <param name="name">
		///		The name of the disease to remove.  An ArgumentNullException exception is
		///		raised if name is null.
		///	</param>
		public void Remove(string name) 
		{
			Values.Remove(name);
		}

		/// <summary>
		///		Remove the disease at the passed position from the list.
		/// </summary>
		/// <param name="index">
		///		The position of the disease to remove.  An ArgumentOutOfRangeException
		///		exception is raised if the index is less than zero or greater than the number
		///		of items in the list minus one.
		///	</param>
		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the diseases.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		/// <summary>
		///		Determine whether the list contains the passed key.
		/// </summary>
		/// <param name="name">The key to look for.</param>
		/// <returns>True if the key is found in this list.</returns>
		public bool ContainsKey(string name) 
		{
			return Values.ContainsKey(name);
		}

		// ************************ Private Members **************************************
		// the table of values
		private cIndexedHashTable Values;
	}
}