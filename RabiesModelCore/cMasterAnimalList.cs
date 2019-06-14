using System;
using Random;

namespace Rabies_Model_Core
{
	/// <summary>
	///		The master list of all animals in the model.  This list is maintained by the 
	///		Background object.  The main list (accessed via	the indexers of this class)
	///		contains only living animals.  For a list of all animals dead and alive, access the
	///		AllAnimals property.
	/// </summary>
	public class cMasterAnimalList : cAnimalList
	{
		// ******************* Constructors *********************************************
		/// <summary>
		///		Initialize a Master animal list.
		/// </summary>
		/// <param name="Background">
		///		The background object that owns this master list.  An ArgumentNullEception
		///		exception is raised if Background is null.
		///	</param>
		/// <param name="StartingID">
		///		The numeric value of the first animal ID generated when the model is run.
		///	</param>
		///	<param name="KeepAllAnimals">
		///		A flag indicating whether animals should remain in the database after they
		///		die.
		///	</param>
        /// <param name="ScrambleRandom">A random number generator to use if the list is to be scrambled</param>
		public cMasterAnimalList(int StartingID, bool KeepAllAnimals, cUniformRandom ScrambleRandom)
            : base(ScrambleRandom)
		{
			mvarIDManager = new cIDManager(StartingID);
			mvarIDManager.Prefix = "";
			mvarIDManager.Suffix = "";
			mvarKeepAllAnimals = KeepAllAnimals;
            if (mvarKeepAllAnimals) mvarAllAnimals = new cAnimalList(ScrambleRandom);
            Babies = new cAnimalList(null);
		}

		// **************** Properties ***************************************************
		/// <summary>
		///		A list of all animals, dead and alive.  Note: the indexer for this class
		///		only returns animals that are currently alive.  If you need access to all
		///		animals, use this property.  An InvalidOperationException exception is raised
		///		if the KeepAllAnimals property is false.
		/// </summary>
		public cAnimalList AllAnimals 
		{
			get 
			{
				if (!mvarKeepAllAnimals)
					throw new InvalidOperationException("The AllAnimals list is not currently enabled.");
				return mvarAllAnimals;
			}
		}

		/// <summary>
		///		A flag that tells the master list whether or not all animals, dead or alive
		///		should be kept in the All Animals list (read-only).  If this property is false,
		///		animals	are discarded when they die.  Attempting to access the AllAnimals
		///		property when this value is false will cause an InvalidOperationException
		///		exception to be raised.
		/// </summary>
		public bool KeepAllAnimals 
		{
			get 
			{
				return mvarKeepAllAnimals;
			}
		}

		// **************** Methods ******************************************************
		/// <summary>
		///		Add an animal to the master animal list.  The animal will be keyed by its ID.  If
		///		another animal in the list already has this ID, an ArgumentException exception
		///		will be raised.  If item is null, an ArgumentNullException exception is raised.
		/// </summary>
		/// <param name="item">The animal to add to the list.</param>
		public override void Add(cAnimal item) 
		{
			// add the animal to the list
			base.Add(item);
			// update the ID manager
			int intID = Convert.ToInt32(item.ID);
			if (mvarIDManager.IDNum <= intID) mvarIDManager.IDNum = intID + 1;
		}

		// **************** Internal functions *******************************************
		/// <summary>
		///		Add a baby to the list of babies.  The babies added to this list will become
		///		part of the main list once the AddBabiesToList method is called.  An
		///		ArgumentNullException exception is raised if Baby is null.  This is an internal
		///		method that may only be called by classes in the Rabies_Model_Core namespace.
		/// </summary>
		/// <param name="Baby">The new baby animal to add to the list.</param>
		internal void AddBaby(cAnimal Baby)
		{
			Babies.Add(Baby);
		}

		/// <summary>
		///		Add the babies stored in the Babies list to the main list.  This is an
		///		internal method that may only be called by classes in the Rabies_Model_Core
		///		namespace.
		/// </summary>
		internal void AddBabiesToList() 
		{
			if (Babies.Count > 0) {
				// loop through all animals in the babies list
				foreach (cAnimal BabyAnimal in Babies) {
					// add to main list
					this.Add(BabyAnimal);
					// add to all animals list
					if (mvarKeepAllAnimals) AllAnimals.Add(BabyAnimal);
				}
				// destroy this version of the BabyAnimal list and create a new one
				Babies = new cAnimalList(null);
			}
		}

		/// <summary>
		///		Add the babies stored in the Babies list to the main list.  This is an
		///		internal method that may only be called by classes in the Rabies_Model_Core
		///		namespace.
		/// </summary>
		/// <param name="EHandler">An event handler for the AnimalInfected event.</param>
		internal void AddBabiesToList(AnimalInfectedEventHandler EHandler) 
		{
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cMasterAnimalList.cs: AddBabiesToList()");

            if (Babies.Count > 0) 
			{
				// loop through all animals in the babies list
				foreach (cAnimal BabyAnimal in Babies) 
				{
					// set the event handler
					if (EHandler != null) BabyAnimal.AnimalInfected += EHandler;
					// add to main list
					this.Add(BabyAnimal);
					// add to all animals list
					if (mvarKeepAllAnimals) AllAnimals.Add(BabyAnimal);
				}
				// destroy this version of the BabyAnimal list and create a new one
				Babies = new cAnimalList(null);
			}
		}

		/// <summary>
		///		Set the handler for the AnimalInfected event for every animal in the list
		/// </summary>
		/// <param name="EHandler">The event handler for the AnimalInfected event.</param>
		internal void SetAnimalInfectedHandler(AnimalInfectedEventHandler EHandler)
		{
			// loop through all animals, setting the value
			foreach (cAnimal Animal in this)
			{
				Animal.AnimalInfected += EHandler;
			}
		}

		/// <summary>
		///		Get the ID of the next animal to add to the list.  This is an internal
		///		method that may only be called by classes in the Rabies_Model_Core namespace.
		/// </summary>
		/// <returns>The next ID available.</returns>
		internal string GetNextID()
		{
			return mvarIDManager.GetNextID();
		}

		/// <summary>
		///		Remove deceased animals from the main list, leaving them only in the AllAnimals
		///		list.   This is an internal method that may only be called by classes in the
		///		Rabies_Model_Core namespace.
		/// </summary>
		internal void RemoveDeceased() 
		{
			// loop through all animals in the main list, removing the dead ones
			for (int i = this.Count - 1; i >= 0; i--) 
            {
				if (!this[i].IsAlive) this.RemoveAt(i);
			}
		}

		// **************** Private members **********************************************
		// an ID manager to handle naming new animals
		private cIDManager mvarIDManager;
		// a list of all animals, dead or alive
		private cAnimalList mvarAllAnimals;
		// a temporary holding list for new babies before they are added to the main list
		private cAnimalList Babies;
		// keep all animals flag
		private bool mvarKeepAllAnimals;
	}
}