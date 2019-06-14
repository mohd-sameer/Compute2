using System;
using System.Collections;
using IndexedHashTable;
using Random;
using System.Collections.Generic;

namespace Rabies_Model_Core
{

	/// <summary>
	///		A list of animals.  This class is used by the rabies model to keep track of the
	///		animals in the model and to store smaller list of animals (i.e. the animals
	///		occupying a cell).  The animals are keyed by their ID values in this list.
	///		Therefore, all animals in this list must have unique ID values.  Animals in this
	///		list may be accessed either through their key value or through an integer index.
	/// </summary>
    /// <param name="ScrambleRandom">A random number generator to use if the list is to be scrambled</param>
	public class cAnimalList : IEnumerable
	{
		// *********************** Constructor *******************************************
		/// <summary>
		///		Initialize an animal list.
		/// </summary>
		public cAnimalList(cUniformRandom ScrambleRandom)
		{
			// create the list of values
			Values = new cIndexedHashTable(ScrambleRandom);
		}

        //EER create empty constructor for showing final population size in Form1.cs
        public cAnimalList()
        {
        }
        // *********************** Properties ********************************************
        /// <summary>
        ///		Retrieve a reference to the animal at the passed key value (read-only).  If the 
        ///		key is not found, an ArgumentException exception is raised.
        /// </summary>
        public cAnimal this[string key] 
		{
			get 
			{
				return (cAnimal)Values[key];
			}
		}

		/// <summary>
		///		Retrieve a reference to the animal at the passed index value (read-only).  An 
		///		ArgumentOutOfRangeException exception is raised if the index is less than zero
		///		or greater than the number of items in the list minus one.
		/// </summary>
		public cAnimal this[int index] 
		{
			get 
			{
				return (cAnimal)Values[index];
			}
		}

		/// <summary>
		///		The number of animals in the list (read-only).
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
		///		Add an animal to the cell list.  The animal will be keyed by its ID.  If
		///		another animal in the list already has this ID, an ArgumentException exception
		///		will be raised.  If item is null, an ArgumentNullException exception is raised.
		/// </summary>
		/// <param name="item">The animal to add to the list.</param>
		public virtual void Add(cAnimal item) 
		{
			if (item == null) throw new ArgumentNullException("item", "Cannot add a null item to the list");
			Values.Add(item.ID, item);
		}

		/// <summary>
		///		Remove the animal with the passed ID from the list.
		/// </summary>
		/// <param name="ID">
		///		The ID of the animal to remove.  An ArgumentNullException exception is raised
		///		if ID is null.
		///	</param>
		public void Remove(string ID) 
		{
			Values.Remove(ID);
		}

        
        /// <summary>
        ///		Remove the animal at the passed position from the list.
        /// </summary>
        /// <param name="index">
        ///		The position of the animal to remove.  An ArgumentOutOfRangeException exception
        ///		is raised if the index is less than zero or greater than the number of items
        ///		in the list minus one.
        ///	</param>
        public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		/// <summary>
		///		Get an enumerator that can iterate through the animals.
		/// </summary>
		/// <returns>The enumerator as an IEnumerator type.</returns>
		public IEnumerator GetEnumerator() 
		{
			return Values.GetEnumerator();
		}

		/// <summary>
		///		Scramble the order of the animal objects in this list.
		/// </summary>
		public void ScrambleOrder()
		{
			Values.ScrambleOrder();
		}

		/// <summary>
		///		Get a sub-list of all animals of a particular gender.
		/// </summary>
		/// <param name="Gender">The gender of interest.</param>
		/// <returns>A cAnimalList containing the sub-list.</returns>
		public cAnimalList GetByGender(enumGender Gender)
		{
			// create a new cAnimalList
			cAnimalList NewList = new cAnimalList(null);
			// loop through the entire list adding those animals of the selected gender
			// to the new list.
			foreach (cAnimal Animal in this) {
				if (Animal.Gender == Gender) NewList.Add(Animal);
			}
			// return the newly created list
			return NewList;
		}

		/// <summary>
		///		Get a count of all animals of a particular gender.
		/// </summary>
		/// <param name="Gender">The gender of interest.</param>
		/// <returns>
		///		The count of animals in the list belonging to the desired gender.
		/// </returns>
		public int GetCountByGender(enumGender Gender)
		{
			int Count = 0;
			// loop through the entire list adding those animals of the selected gender
			// to the new list
			foreach (cAnimal Animal in this) {
				if (Animal.Gender == Gender) Count++;
			}
			// return the count
			return Count;
		}

		/// <summary>
		///		Get a sub-list of all animals of a particular age class.
		/// </summary>
		/// <param name="AgeClass">The age class of interest.</param>
		/// <returns>A cAnimalList containing the sub-list.</returns>
		public cAnimalList GetByAgeClass(enumAgeClass AgeClass)
		{
			// create a new cAnimalList
			cAnimalList NewList = new cAnimalList(null);
			// loop through the entire list adding those animals of the selected age class
			// to the new list.
			foreach (cAnimal Animal in Values) {
				if (Animal.AgeClass == AgeClass) NewList.Add(Animal);
			}
			// return the newly created list
			return NewList;
		}

		/// <summary>
		///		Get a count of all animals of a particular age class.
		/// </summary>
		/// <param name="AgeClass">The age class of interest.</param>
		/// <returns>
		///		The count of animals in the list belonging to the desired age class.
		/// </returns>
		public int GetCountByAgeClass(enumAgeClass AgeClass)
		{
			int Count = 0;
			// loop through the entire list adding those animals of the selected age class
			// to the new list
			foreach (cAnimal Animal in this) {
				if (Animal.AgeClass == AgeClass) Count++;
			}
			// return the count
			return Count;
		}



        public int GetCountbyAgeGender(int AgeinYear, enumGender Gender)
        {
            cAnimalList Animals = this.GetByGender(Gender);
            int count = 0;
            foreach (cAnimal anim in Animals)
            {
                int AgeInYears = Convert.ToInt32(Math.Floor((double)anim.Age / 52));
                //Console.WriteLine("AgeInYears {0} et i {1}", AgeInYears, i);
                if (AgeInYears == AgeinYear)
                {
                    count++;
                }
            }
            return (count);
        }


        


        public int RemoveGetCountbyAgeClass(cAnimal Animal)
        {
            int count = 0;
            Console.WriteLine("RemoveGetCountbyAgeClass - Animal count1: " + this.Count);
            this.Remove(Animal.ID);
            foreach (cAnimal a in this)
            {
                if(a.AgeClass == Animal.AgeClass) count++;
            }
            
            Console.WriteLine("RemoveGetCountbyAgeClass - Animal count2: " + count);
            return count;
        }


        /// <summary>
        ///		Determine whether or not a key is found in the animal list.
        /// </summary>
        /// <param name="Key">The key value to find.</param>
        /// <returns>True if this key is in the animal list.</returns>
        public bool ContainsKey(string Key) 
		{
			return Values.ContainsKey(Key);
		}

		/// <summary>
		///		Internally rebuilds the list of animals.  This function should be run on
		///		occasion for performance reasons.
		/// </summary>
		public void Rebuild()
		{
			// clone the current main list
			cIndexedHashTable NewValues = (cIndexedHashTable) Values.Clone();
			// make the clone the main list
			Values = NewValues;
		}

        /// <summary>
        ///     Reorder the animals in this list by their list index value.  Function does nothing if non-unique
        ///     list index values are found
        /// </summary>
        public void ReorderByListIndex()
        {
            List<int> IndexValues = new List<int>();
            // confirm that the index values are unique
            foreach (cAnimal Animal in this)
            {
                // is list index unique? - if not, leave now!!
                if (IndexValues.Contains(Animal.ListIndex)) 
                    return;
                // otherwise add list index to list
                IndexValues.Add(Animal.ListIndex);
            }
            // OK we have confirmed uniqueness of index values - now reorder the list accordingly
            int currentIndex = 0;
            // loop until the current index is equal to the count
            while (currentIndex < this.Count)
            {
                // get animal currently at the current index
                cAnimal currentAnimal = this[currentIndex];
                // is this animal in the right place
                if (currentAnimal.ListIndex != currentIndex)
                {
                    // no - put it in the right place and moce the animal at that locartion to the current index
                    Values[currentIndex] = this[currentAnimal.ListIndex];
                    Values[currentAnimal.ListIndex] = currentAnimal;
                }
                else
                {
                    // yes - increment the current index
                    currentIndex++;
                }
            }
        }

		// ************************ Private Members **************************************
		// the table of values
		private cIndexedHashTable Values;
	}
}