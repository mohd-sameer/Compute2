using System;

namespace Rabies_Model_Core
{
    /// <summary>
    ///		Represents an actual occurence of a disease.
    /// </summary>
    public class cInfection
    {
        // *********************** Constructor *******************************************
        /// <summary>
        ///		Initialize an Infection.
        /// </summary>
        /// <param name="Background">
        ///		The background object of the animal being infected.  An ArgumentNullException
        ///		exception is raised if Background is null.
        /// </param>
        /// <param name="DiseaseName">
        ///		The disease causing this infection.  An ArgumentException exception is raised
        ///		if DiseaseName is not the name of a disease in the Background object.
        ///	</param>
        /// <param name="Year">
        ///		The year the infection occured.  An ArgumentOutOfRangeException exception is
        ///		raised if Year is less than zero.
        ///	</param>
        /// <param name="Week">
        ///		The week the infection occured.  An ArgumentOutOfRangeException exception is
        ///		raised if Week is not in the range of 1-52.
        ///	</param>
        ///	<param name="InfectingAnimalID">
        ///		The ID of the animal causing the infection
        ///	</param>
        ///	<param name="NoIncubation">
        ///		If set to true, the infection will have no incubation period.  The infected animal will be
        ///		immediately infectious
        ///	</param>
        ///	<param name="NaturalImmunity">
        ///		EER: The infection history of the animal: Never_infected, Infected_died, Infected_recovered
        ///	</param>
        public cInfection(cBackground Background, string DiseaseName, int Year, int Week, string InfectingAnimalID,
                            bool NoIncubation)
        {
            // common initialization
            InitClass(Background, DiseaseName, Year, Week);
            // calculate the incubation and infectious period for this instance of the
            // disease
            if (NoIncubation)
            {
                mvarIncubationPeriod = 1;
            }
            else
            {
                mvarIncubationPeriod = mvarDisease.GetIncubationPeriod();
            }
            mvarInfectiousPeriod = mvarDisease.GetInfectiousPeriod();
            // set the InfectingAnimal
            mvarInfectingAnimalID = InfectingAnimalID;
            
            //System.Diagnostics.Debug.WriteLine("cInfection.cs: cInfection() 1: NaturalImmunity = " + NaturalImmunity);

        }

        /// <summary>
        ///		Initialize an Infection directly passing an incubation and infection period.
        /// </summary>
        /// <param name="Background">
        ///		The background object of the animal being infected.  An ArgumentNullException
        ///		exception is raised if Background is null.
        /// </param>
        /// <param name="DiseaseName">
        ///		The disease causing this infection.  An ArgumentException exception is raised
        ///		if DiseaseName is not the name of a disease in the Background object.
        ///	</param>
        /// <param name="Year">
        ///		The year the infection occured.  An ArgumentOutOfRangeException exception is
        ///		raised if Year is less than zero.
        ///	</param>
        /// <param name="Week">
        ///		The week the infection occured.  An ArgumentOutOfRangeException exception is
        ///		raised if Week is not in the range of 1-52.
        ///	</param>
        /// <param name="IncubationPeriod">
        ///		The incubation period of the infection in weeks.  An ArgumantOutOfRangeException
        ///		exception is raised if IncubationPeriod is less than zero.
        ///	</param>
        /// <param name="InfectiousPeriod">
        ///		The infectious period of the infectiion in weeks.  An ArgumantOutOfRangeException
        ///		exception is raised if IncubationPeriod is less than one.
        ///	</param> 
        ///	<param name="NaturalImmunity">
        ///		EER: The infection history of the animal: Never_infected, Infected_died, Infected_recovered
        ///	</param>
        public cInfection(cBackground Background, string DiseaseName, int Year, int Week,
                            int IncubationPeriod, int InfectiousPeriod, string InfectingAnimalID)
        {
            // common initialization
            InitClass(Background, DiseaseName, Year, Week);
            // make sure that the incubation and infectious period are in range
            if (IncubationPeriod < 0) ThrowIncubationPeriodException();
            if (InfectiousPeriod < 1) ThrowInfectiousPeriodException();
            // set the Incubation and Infectious period
            mvarIncubationPeriod = IncubationPeriod;
            mvarInfectiousPeriod = InfectiousPeriod;
            // set the InfectingAnimal
            mvarInfectingAnimalID = InfectingAnimalID;
            // EER: set the infection history 
            //mvarNaturalImmunity = NaturalImmunity;
            //System.Diagnostics.Debug.WriteLine("cInfection.cs: cInfection() 2: NaturalImmunity = " + NaturalImmunity);
        }

        // common initialization code
        private void InitClass(cBackground Background, string DiseaseName, int Year, int Week)
        {
            // check parameters
            // background cannot be null
            if (Background == null)
                throw new ArgumentNullException("Background",
                    "Background must reference a valid background object.");
            // the disease must be found in the background
            if (!Background.Diseases.ContainsKey(DiseaseName))
                throw new ArgumentException("Disease name passed is not contained in the background object.",
                                            "DiseaseName");
            // year must not be < 0 and week must be 1-52.
            if (Year < 0) ThrowYearException();
            if (Week < 1 || Week > 52) ThrowWeekException();
            // set parameters
            mvarBackground = Background;
            mvarDisease = Background.Diseases[DiseaseName];
            mvarYear = Year;
            mvarWeek = Week;
            mvarIsFatal = mvarDisease.GetAnimalDies();
            // EER: set the infection history            
            if (mvarIsFatal)
            {
                mvarNaturalImmunity = "Infected_died";
            }
            else
            {
                mvarNaturalImmunity = "Infected_recovered";
            }
            
            //System.Diagnostics.Debug.WriteLine("cInfection.cs: InitClass(): mvarIsFatal = " + mvarIsFatal + "; mvarNaturalImmunity = " + mvarNaturalImmunity);
            //System.Diagnostics.Debug.WriteLine("cInfection.cs: InitClass(): mvarNaturalImmunity = " + mvarNaturalImmunity);
        }

        // private constructor for cloning
        private cInfection(int IncubationPeriod, int InfectiousPeriod, int Year, int Week, cDisease Disease,
            cBackground Background, string InfectingAnimalID)
        {
            mvarIncubationPeriod = IncubationPeriod;
            mvarInfectiousPeriod = InfectiousPeriod;
            mvarYear = Year;
            mvarWeek = Week;
            mvarDisease = Disease;
            mvarBackground = Background;
            mvarInfectingAnimalID = InfectingAnimalID;
            mvarIsFatal = mvarDisease.GetAnimalDies();
            // EER: set the infection history 
            if (mvarIsFatal)
            {
                mvarNaturalImmunity = "Infected_died";
            }
            else
            {
                mvarNaturalImmunity = "Infected_recovered";
            }
        }

        // *********************** Properties ********************************************
        /// <summary>
        ///		The actual incubation period of this infection (read-only).
        /// </summary>
        public int IncubationPeriod
        {
            get
            {
                return mvarIncubationPeriod;
            }
        }

        /// <summary>
        ///		The actual infectious period of this infection (read-only).
        /// </summary>
        public int InfectiousPeriod
        {
            get
            {
                return mvarInfectiousPeriod;
            }
        }

        /// <summary>
        ///		The year the infection occured (read-only).
        /// </summary>
        public int Year
        {
            get
            {
                return mvarYear;
            }
            set
            {
                mvarYear = value;
            }
        }

        /// <summary>
        ///		The week the infection occured (read-only).
        /// </summary>
        public int Week
        {
            get
            {
                return mvarWeek;
            }
        }

        /// <summary>
        ///		The disease causing this infection (read-only).
        /// </summary>
        public cDisease Disease
        {
            get
            {
                return mvarDisease;
            }
        }

        /// <summary>
        ///		The ID of the animal that caused the infection (read-only).
        /// </summary>
        public string InfectingAnimalID
        {
            get
            {
                return mvarInfectingAnimalID;
            }
        }

        /// <summary>
        ///     A flag indicating whther or not this infection will be fatal (read-only).
        /// </summary>
        public bool IsFatal
        {
            get
            {
                return mvarIsFatal;
            }
        }

        /// <summary>
        ///     EER: The infection history of the animal: Never_infected, Infected_died, Infected_recovered (read-only).
        /// </summary>
        public string NaturalImmunity
        {
            get
            {
                return mvarNaturalImmunity;
            }            
        }
        
        // *********************** Methods ***********************************************
        /// <summary>
        ///		Determine whether the infection is in the incubation stage.
        /// </summary>
        /// <param name="Year">
        ///		The current year.  An ArgumentOutOfRangeException exception is raised if Year
        ///		is less than zero.
        ///	</param>
        /// <param name="Week">
        ///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
        ///		is not in the range of 1-52.
        ///	</param>
        /// <returns>True if the infection is incubating.</returns>
        public bool IsIncubating(int Year, int Week)
        {
            // get the time since infection
            int TimeLength = GetTimeLength(Year, Week);
            // compare that time to the Incubation Period
            return (TimeLength >= 0 && TimeLength < mvarIncubationPeriod);
        }

        /// <summary>
        ///		Determine whether the infection is in the infectious stage.
        /// </summary>
        /// <param name="Year">
        ///		The current year.  An ArgumentOutOfRangeException exception is raised if Year
        ///		is less than zero.
        ///	</param>
        /// <param name="Week">
        ///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
        ///		is not in the range of 1-52.
        ///	</param>
        /// <returns>
        ///		True if the infection is currently infectious (ie. able to be passed to
        ///		other animals.
        ///	</returns>
        public bool IsInfectious(int Year, int Week)
        {
            // get the time since infection
            int TimeLength = GetTimeLength(Year, Week);
            // compare that time to the Incubation Period
            return (TimeLength >= mvarIncubationPeriod &&
                    TimeLength < mvarInfectiousPeriod + mvarIncubationPeriod);
        }

        /// <summary>
        ///		Determine whether or not the infection has run its course.
        /// </summary>
        /// <param name="Year">
        ///		The current year.  An ArgumentOutOfRangeException exception is raised if Year
        ///		is less than zero.
        ///	</param>
        /// <param name="Week">
        ///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
        ///		is not in the range of 1-52.
        ///	</param>
        /// <returns>True if the infection has run its course.</returns>
        public bool HasRunCourse(int Year, int Week)
        {
            //System.Diagnostics.Debug.WriteLine("    cInfection.cs: HasRunCourse()");
            // get the time since infection
            int TimeLength = GetTimeLength(Year, Week);
            // compare that time to the Incubation Period
            //System.Diagnostics.Debug.WriteLine("    cInfection.cs: HasRunCourse(): mvarIncubationPeriod = " + mvarIncubationPeriod);
            //System.Diagnostics.Debug.WriteLine("    cInfection.cs: HasRunCourse(): mvarInfectiousPeriod = " + mvarInfectiousPeriod);            
            return (TimeLength == mvarInfectiousPeriod + mvarIncubationPeriod + 1);
        }

        /// <summary>
        ///		Determine whether the infection is still active.
        /// </summary>
        /// <param name="Year">
        ///		The current year.  An ArgumentOutOfRangeException exception is raised if Year
        ///		is less than zero.
        ///	</param>
        /// <param name="Week">
        ///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
        ///		is not in the range of 1-52.
        ///	</param>
        /// <returns>True if the infection is still active.</returns>
        public bool IsActive(int Year, int Week)
        {
            // get the time since infection
            int TimeLength = GetTimeLength(Year, Week);
            // compare that time to the Incubation Period
            return (TimeLength > mvarInfectiousPeriod + mvarIncubationPeriod);
        }

        /// <summary>
        ///		Creates a deep copy of this infection object.
        /// </summary>
        /// <returns>The deep copy of this infection as a cInfection.</returns>
        public virtual cInfection Clone()
        {
            //cInfection Copy = new cInfection(mvarBackground, mvarDisease.Name, mvarYear, mvarWeek, mvarInfectingAnimalID, false);
            //Copy.SetIncubationPeriod(mvarIncubationPeriod);
            //Copy.SetInfectiousPeriod(mvarInfectiousPeriod);
            return new cInfection(mvarIncubationPeriod, mvarInfectiousPeriod, mvarYear, mvarWeek,
                                  mvarDisease, mvarBackground, mvarInfectingAnimalID);
        }

        // ******************** To string function **************************************

        /// <summary>
        /// Get the string representing this object
        /// </summary>
        /// <returns>The string representing this object</returns>
        public override string ToString()
        {
            return string.Format("{0} - Infecting {1} at Year: {2}, Week{3}", this.Disease.Name, this.InfectingAnimalID, this.Year, this.Week);
        }



        // *************** internal methods (can only be called by classes in the  *******
        //**************** Rabies_Model_Core namespace) **********************************
        /// <summary>
        ///		Set the value of the incubation period.  This is an internal function that may
        ///		only be called by classes within the Rabies_Model_Core namespace.
        /// </summary>
        /// <param name="IncubationPeriod">
        ///		The value to the the incubation period to.  An ArgumentOutOfRangeException
        ///		exception is raised if Value is less than zero.
        ///	</param>
        internal void SetIncubationPeriod(int IncubationPeriod)
        {
            if (IncubationPeriod < 0) ThrowIncubationPeriodException();
            mvarIncubationPeriod = IncubationPeriod;
        }

        /// <summary>
        ///		Set the value of the infectious period.  This is an internal function that may
        ///		only be called by classes within the Rabies_Model_Core namespace.
        /// </summary>
        /// <param name="InfectiousPeriod">
        ///		The value to the the infectious period to.  An ArgumentOutOfRangeException
        ///		exception is raised if Value is less than one.
        ///	</param>
        internal void SetInfectiousPeriod(int InfectiousPeriod)
        {
            if (InfectiousPeriod < 1) ThrowInfectiousPeriodException();
            mvarInfectiousPeriod = InfectiousPeriod;
        }

        // *********************** Private members ***************************************
        private int mvarIncubationPeriod;
        private int mvarInfectiousPeriod;
        private int mvarYear;
        private int mvarWeek;
        private cDisease mvarDisease;
        private cBackground mvarBackground;
        private cAnimal mvarAnimal;
        private string mvarInfectingAnimalID;
        private bool mvarIsFatal;
        private string mvarNaturalImmunity;

        // determine the length of time (in weeks) between the passed year and week and
        // the year and week of infection.
        private int GetTimeLength(int CurrentYear, int CurrentWeek)
        {
            // check parameters
            // CurrentYear must not be < 0 and CurrentWeek must be 1-52.
            if (CurrentYear < 0) ThrowYearException();
            if (CurrentWeek < 1 || CurrentWeek > 52) ThrowWeekException();
            // determine whether of not the disease is in the incubation stage
            // how long has it been since infection
            int TimeLength = 52 * (CurrentYear - mvarYear) + CurrentWeek - mvarWeek;
            return TimeLength;
        }

        // throw an exception indicating an Invalid Incubation Period
        private void ThrowIncubationPeriodException()
        {
            throw new ArgumentOutOfRangeException("IncubationPeriod",
                "The incubation period must not be less than zero.");
        }

        // throw an exception indicating an Invalid Infectious Period
        private void ThrowInfectiousPeriodException()
        {
            throw new ArgumentOutOfRangeException("InfectiousPeriod",
                "The infectious period must not be less than one.");
        }
        // throw an exception indicating an invalid year value
        private void ThrowYearException()
        {
            throw new ArgumentOutOfRangeException("Year", "Year must not be less than zero.");
        }
        // throw an exception indicating an invalid week value
        private void ThrowWeekException()
        {
            throw new ArgumentOutOfRangeException("Week", "Week must be in the range of 1-52.");
        }


    }
}
