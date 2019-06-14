using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A class used to store animal atrributes retrieved from a datasource.  This class
	///		is primarily intended for internal use but may be used or overridden for use by
	///		derived animal classes and/or specialized animal data sources.
	/// </summary>
	public class cAnimalAttributes
	{
		/// <summary>
		///		Initialize the animal attributes class.
		/// </summary>
		public cAnimalAttributes()
		{
			// create valid cell and infection lists
			Cells = new cCellTimeList();
			Infections = new cInfectionList();
			// create the collection of offspring IDs
			Offspring = new List<string>();
			// create the collection of vaccines
			Vaccines = new cVaccineList();
		}

		// ******************** properties ************************************************
		/// <summary>
		///		The ID if the animal
		/// </summary>
		public string ID;
		/// <summary>
		///		The age of the animal.
		/// </summary>
		public int Age;
		/// <summary>
		///		The year the animal died.
		/// </summary>
		public int YearDied;
		/// <summary>
		///		The week the animal died.
		/// </summary>
		public int WeekDied;
		/// <summary>
		///		The gender of the animal.
		/// </summary>
		public enumGender Gender;
		/// <summary>
		///		The ID of the animal's parent.
		/// </summary>
		public string ParentID;
		/// <summary>
		///		A flag indicating whether or not the animal is independent.
		/// </summary>
		public bool IsIndependent;
		/// <summary>
		///		A flag indicating whether or not the animal is alive.
		/// </summary>
		public bool IsAlive;
		/// <summary>
		///		A list of the offspring of this animal.
		/// </summary>
		public List<string> Offspring;
		/// <summary>
		///		A list of cells occupied by this animal and the time that they were occupied.
		/// </summary>
		public cCellTimeList Cells;
        /// <summary>
        ///		EER: A string indicating this animal's infection history: Never_infected, Infected_died, Infected_recovered.
        /// </summary>        
		public cInfectionList Infections;
		/// <summary>
		///		A list of all vaccines given to this animal.
		/// </summary>
		public cVaccineList Vaccines;
		/// <summary>
		///		A marker automatically passed from Mother to offspring.
		/// </summary>
		public string AutoMarker;
		/// <summary>
		///		A marker than can be passed from Mother (or Father) to offspring.
		/// </summary>
		public string Marker;
		/// <summary>
		///		give birth flag 
		/// </summary>
		public int CannotGiveBirth;
		/// <summary>
		///		marker passed by mating partner
		/// </summary>
		public string PartnerMarker;
        /// <summary>
        ///     The list index of the animal when it was saved
        /// </summary>
        public int ListIndex;

		// ********************* methods ***************************************************
		/// <summary>
		///		Set the current attributes to those belonging to the passed animal.
		/// </summary>
		/// <param name="Animal">
		///		The passed animal.  An ArgumentNullException exception is raised if Animal is
		///		null.
		/// </param>
		public virtual void SetAttributes(cAnimal Animal) 
		{
			// make sure animal is not null
			if (Animal == null)
				throw new ArgumentNullException("Animal", "Animal must not be null.");
			// set values
			ID = Animal.ID;
			Age = Animal.Age;
			YearDied = Animal.YearDied;
			WeekDied = Animal.WeekDied;
			Gender = Animal.Gender;
			ParentID = Animal.ParentID;
			IsIndependent = Animal.IsIndependent;
			IsAlive = Animal.IsAlive;
			Offspring = Animal.Offspring;
			Cells = Animal.GetCellsAndTime();            
            Infections = Animal.GetInfections();
			Vaccines = Animal.GetVaccines();
			Marker = Animal.Marker;
			AutoMarker = Animal.AutoMarker;
			CannotGiveBirth = Animal.CannotGiveBirthValue;
			PartnerMarker = Animal.PartnerMarker;
		}
	}
}