using System;
using System.Collections.Generic;
using System.Text;
using Rabies_Model_Core;
using Random;
using System.Data;

namespace Raccoon
{
    /// <summary>
    /// A background specifically for raccoons.  It will hold "static" raccoon values (BirthOdds, Mortality etc.) as well
    /// as be the background for the model
    /// </summary>
    public class cRaccoonBackground : cBackground
    {
        /// <summary>
		///		Initialize the background.  Defaults to an initial 10 year time period
		///		with a normal winter bias.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the background</param>
        /// <param name="Name">
		///		The name to assign to this background.  An ArgumentException is raised if
		///		Name is zero length.
		///	</param>
		/// <param name="KeepAllAnimals">
		///		A flag indicating whether a record of all animals should be kept during
		///		a run.
		///	</param>
        public cRaccoonBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals) 
            : base(Rnd, Name, KeepAllAnimals)
		{
            InitValues();
		}

		/// <summary>
		///		Initialize the background specifying the initial number of years.  Years 
		///		are created with a normal winter bias.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the background</param>
        /// <param name="Name">
		///		The name to assign to this background.  An ArgumentException is raised if
		///		Name is zero length.
		///	</param>
		/// <param name="KeepAllAnimals">
		///		A flag indicating whether a record of all animals should be kept during
		///		a run.
		///	</param>
		/// <param name="NYears">
		///		The initial number of years. An ArgumentException is raised in NYears is
		///		less than or equal to zero.
		///	</param>
        public cRaccoonBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, int NYears)
            : base(Rnd, Name, KeepAllAnimals, NYears)
		{
            InitValues();
		}

		/// <summary>
		///		Initialize the background specifying the initial number of years and their
		///		winter bias.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the background</param>
		/// <param name="Name">
		///		The name to assign to this background.  An ArgumentException is raised if
		///		Name is zero length.
		///	</param>
		/// <param name="KeepAllAnimals">
		///		A flag indicating whether a record of all animals should be kept during
		///		a run.
		///	</param>
		/// <param name="NYears">
		///		The initial number of years. An ArgumentException is raised in NYears is
		///		less than or equal to zero.
		///	</param>
		/// <param name="WinterBias">The winter bias of the initial years.</param>
		public cRaccoonBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, int NYears, enumWinterType WinterBias)
            : base(Rnd, Name, KeepAllAnimals, NYears, WinterBias) 
        {
            InitValues();
        }

		/// <summary>
		///		Initialize the background specifying numbers of years and their winter
		///		types in a winter type list.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the background</param>
        /// <param name="Name">
		///		The name to assign to this background.  An ArgumentException is raised if
		///		Name is zero length.
		///	</param>
		/// <param name="KeepAllAnimals">
		///		A flag indicating whether a record of all animals should be kept during
		///		a run.
		///	</param>
		/// <param name="WTList">
		///		The list of winter types.  An ArgumentException exception is raised if
		///		WTList is empty.
		///	</param>
		public cRaccoonBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, cWinterTypeList WTList)
            : base(Rnd, Name, KeepAllAnimals, WTList)
        {
            InitValues();
        }

		/// <summary>
		///		Initialize the background, passing a list of years and a seed for all 
		///		random number generators in the model.  This is an internal constructor
		///		and may only be used by classes within the Rabies_Model_Core namespace.
		/// </summary>
        /// <param name="Rnd">The random number generator to be used by the background</param>
        /// <param name="Name">
		///		The name to assign to this background.  An ArgumentException is raised if
		///		Name is zero length.
		///	</param>
		/// <param name="KeepAllAnimals">
		///		A flag indicating whether a record of all animals should be kept during
		///		a run.
		///	</param>
		/// <param name="YList">
		///		The list of years.  An ArgumentException exception is raised if YList is empty.
		///	</param>
		///	<param name="DList">
		///		A list of diseases that will affect the animals in this background.
		///	</param>
		public cRaccoonBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, cYearList YList, cDiseaseList DList)
            : base(Rnd, Name, KeepAllAnimals, YList, DList)
        {
            InitValues();
        }

        /// <summary>
        /// Initialize values
        /// </summary>
        private void InitValues()
        {
            this.RaccoonMortalityAdjustment = 0;
            this.RaccoonMatingWeek = 9;
            this.RaccoonReproductionWeek = 18;
            this.RaccoonGenderRatio = 50;
            this.RaccoonJuvenileBirthOdds = 60;
            this.RaccoonAdultBirthOdds = 95;
        }

        #region Public Properties ("Static Raccoon Values")

        // ************* static members that hold user defined behaviour ******************
        /// <summary>
        ///		Get an array of expected mortalities for male raccoons by year.  Each
        ///		position in the array represents the expected mortality of a male Raccoon in a 
        ///		single week for that year.  The mortality is represented as a percentage, with
        ///		0 representing no chance of death and 100 representing certain death.  The array
        ///		has 8 positions representing years 0 through 7.  No raccoon in the model will
        ///		live for more than eight years.
        /// </summary>
        public double[] RaccoonMaleMortality
        {
            get
            {
                return _raccoonMaleMortality;
            }
        }
        private double[] _raccoonMaleMortality = new double[8];

        /// <summary>
        ///		Get an array of expected mortalities for female raccoons by year.
        ///		Each position in the array represents the expected mortality of a female
        ///		Raccoon in a single week for that year.  The mortality is represented as a
        ///		percentage, with 0 representing no chance of death and 100 representing certain
        ///		death.  The array has 8 positions representing years 0 through 7.  No raccoon
        ///		in the model will live for more than eight years.
        /// </summary>
        public double[] RaccoonFemaleMortality
        {
            get
            {
                return _raccoonFemaleMortality;
            }
        }
        private double[] _raccoonFemaleMortality = new double[8];

        /// <summary>
        ///		Get or set an adjustment applied to all mortality calculations to allow
        ///		fine tuning of the population to "expected" values.
        /// </summary>
        public double RaccoonMortalityAdjustment { get; set; }

        /// <summary>
        ///		Get an array of probabilities defining how far a juvenile or adult
        ///		male raccoon will move when it gets its chance to move.  Each position in the
        ///		array represents the probability that a raccoon will move that distance, from
        ///		position 0, representing a movement of 0 cells, to position 49, representing a
        ///		movement of 49 cells.  The values stored in each array position are the
        ///		percentage chance that the raccoon will move that distance.  All of the values
        ///		in the array should sum to 100.
        /// </summary>
        public double[] RaccoonJuvAdultMaleMovementDistribution
        {
            get
            {
                return _raccoonJuvAdultMaleMovementDistribution;
            }
        }
        private double[] _raccoonJuvAdultMaleMovementDistribution = new double[50];

        /// <summary>
        ///		Get an array of probabilities defining how far a juvenile or adult
        ///		female raccoon will move when it gets its chance to move.  Each position in the
        ///		array represents the probability that a raccoon will move that distance, from
        ///		position 0, representing a movement of 0 cells, to position 49, representing a
        ///		movement of 49 cells.  The values stored in each array position are the
        ///		percentage chance that the raccoon will move that distance.  All of the values
        ///		in the array should sum to 100.
        /// </summary>
        public double[] RaccoonJuvAdultFemaleMovementDistribution
        {
            get
            {
                return _raccoonJuvAdultFemaleMovementDistribution;
            }
        }
        private double[] _raccoonJuvAdultFemaleMovementDistribution = new double[50];
        
        /// <summary>
        ///		Get an array of probabilities defining how far a young male raccoon
        ///		will move when it gets its chance to move.  This is the raccoon's first movement
        ///		after leaving its mother.  Each position in the array represents the probability
        ///		that a raccoon will move that distance, from position 0, representing a
        ///		movement of	0 cells, to position 49, representing a movement of 49 cells.  The
        ///		values stored in each array position are the percentage chance that the raccoon
        ///		will move that distance.  All of the values in the array should sum to 100.
        /// </summary>
        public double[] RaccoonYoungMaleMovementDistribution
        {
            get
            {
                return _raccoonYoungMaleMovementDistribution;
            }
        }
        private double[] _raccoonYoungMaleMovementDistribution = new double[50];

        /// <summary>
        ///		Get an array of probabilities defining how far a young female raccoon
        ///		will move when it gets its chance to move.  This is the raccoon's first movement
        ///		after leaving its mother.  Each position in the array represents the probability
        ///		that a raccoon will move that distance, from position 0, representing a
        ///		movement of	0 cells, to position 49, representing a movement of 49 cells.  The
        ///		values stored in each array position are the percentage chance that the raccoon
        ///		will move that distance.  All of the values in the array should sum to 100.
        /// </summary>
        public double[] RaccoonYoungFemaleMovementDistribution
        {
            get
            {
                return _raccoonYoungFemaleMovementDistribution;
            }
        }
        private double[] _raccoonYoungFemaleMovementDistribution = new double[50];

        /// <summary>
        ///		Every raccoon will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a juvenile or adult male raccoon may move.  The array has 52 positions, each
        ///		position representing a week in a year (be careful - position 0 corresponds to
        ///		week 1,	and so on).  If a value in the array is zero, the animal may not move
        ///		that week.  If it is any other value, the animal can move that week.  Note that
        ///		this array only defines when a raccoon CAN move.  The raccoon will only actually
        ///		move in one of the allowed weeks.
        /// </summary>
        public bool[] RaccoonJuvAdultMaleWeeklyMovement
        {
            get
            {
                return _raccoonJuvAdultMaleWeeklyMovement;
            }
        }
        private bool[] _raccoonJuvAdultMaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Every raccoon will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a juvenile or adult female raccoon may move.  The array has 52 positions, each
        ///		position representing a week in a year (be careful - position 0 corresponds to
        ///		week 1,	and so on).  If a value in the array is zero, the animal may not move
        ///		that week.  If it is any other value, the animal can move that week.  Note that
        ///		this array only defines when a raccoon CAN move.  The raccoon will only actually
        ///		move in one of the allowed weeks.
        /// </summary>
        public bool[] RaccoonJuvAdultFemaleWeeklyMovement
        {
            get
            {
                return _raccoonJuvAdultFemaleWeeklyMovement;
            }
        }
        private bool[] _raccoonJuvAdultFemaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Every raccoon will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a young male raccoon may move.  This is the raccoon's first movement after
        ///		leaving its mother.  The array has 52 positions, each position representing a
        ///		week in a year (be careful - position 0 corresponds to week 1, and so on).  If
        ///		a value in the array is zero, the animal may not move that week.  If it is any
        ///		other value, the animal can move that week.  Note that this array only defines
        ///		when a raccoon CAN move.  The raccoon will only actually move in one of the
        ///		allowed weeks.
        /// </summary>
        public bool[] RaccoonYoungMaleWeeklyMovement
        {
            get
            {
                return _raccoonYoungMaleWeeklyMovement;
            }
        }
        private bool[] _raccoonYoungMaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Every raccoon will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a young female raccoon may move.  This is the raccoon's first movement after
        ///		leaving its mother.  The array has 52 positions, each position representing a
        ///		week in a year (be careful - position 0 corresponds to week 1, and so on).  If
        ///		a value in the array is zero, the animal may not move that week.  If it is any
        ///		other value, the animal can move that week.  Note that this array only defines
        ///		when a raccoon CAN move.  The raccoon will only actually move in one of the
        ///		allowed weeks.
        /// </summary>
        public bool[] RaccoonYoungFemaleWeeklyMovement
        {
            get
            {
                return _raccoonYoungFemaleWeeklyMovement;
            }
        }
        private bool[] _raccoonYoungFemaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Get an array of probabilities for litter sizes when a female
        ///		raccoon gives birth.  The size of the litter is represented by the array
        ///		position (be careful, position 0 represents a litter of 1 young, and so on to
        ///		position 7, which represents a litter of 8 young).  The value at an array
        ///		position represents the probability of a litter of that size, given as a
        ///		percentage.  The array has 8 positions, meaning litter sizes cannot exceed 8
        ///		young.  All of the values in this array should sum to 100.
        /// </summary>
        public double[] RaccoonLitterSize
        {
            get
            {
                return _raccoonLitterSize;
            }
        }
        private double[] _raccoonLitterSize = new double[8];

        /// <summary>
        ///		Get the week in which mating occurs.  This should occur before the
        ///		ReproductionWeek and should be a value between 1 and 52.  If it is outside this
        ///		range or is set after the ReproductiveWeek, no births will take place in the model.
        ///		By default, this value is set to 9 (late-Winter).
        /// </summary>
        public int RaccoonMatingWeek { get; set; } 

        /// <summary>
        ///		Get a measure of how much the home range of a male raccoon overlaps with
        ///		the	neighbouring cells to its home cells.  The value is specified by week.
        /// </summary>
        public double[] RaccoonMaleHomeRange
        {
            get
            {
                return _raccoonMaleHomeRange;
            }
        }
        private double[] _raccoonMaleHomeRange = new double[52];

        /// <summary>
        ///		Get a measure of how much the home range of a female raccoon overlaps with
        ///		the	neighbouring cells to its home cells.  The value is specified by week.
        /// </summary>
        public double[] RaccoonFemaleHomeRange
        {
            get
            {
                return _raccoonFemaleHomeRange;
            }
        }
        private double[] _raccoonFemaleHomeRange = new double[52];

        /// <summary>
        ///		Get the week in which female raccoons will give birth.  The
        ///		ReproductionWeek should be a value between 1 and 52.  If it is outside this
        ///		range, no births will take place in the model.  By default, this value is set
        ///		to 18 (mid-Spring).
        /// </summary>
        public int RaccoonReproductionWeek { get; set; }

        /// <summary>
        ///		Get the gender ratio of young animals born in the model.  This value
        ///		is actually the percentage of young animals that will be male.  This value
        ///		should not be less than 0 or greater than 100.  By default, it is given a value
        ///		of 50.
        /// </summary>
        public int RaccoonGenderRatio { get; set; }

        /// <summary>
        ///		Get the percentage chance that a juvenile female raccoon will give
        ///		birth during a ReproductionWeek.  This value should not be less than 0 or
        ///		greater than 100.  By default, it has a value of 60.
        /// </summary>
        public int RaccoonJuvenileBirthOdds { get; set; }

        /// <summary>
        ///		Get the percentage chance that an adult female raccoon will give
        ///		birth during a ReproductionWeek.  This value should not be less than 0 or
        ///		greater than 100.  By default, it has a value of 95.
        /// </summary>
        public int RaccoonAdultBirthOdds { get; set; }

        /// <summary>
        ///		Get the age, in weeks, that a raccoon becomes an adult.
        /// </summary>
        public int RaccoonAdultAge { get; set; }

        /// <summary>
        ///		Get the age, in weeks, that a raccoon becomes independent.  That is,
        ///		the age at which the raccoon leaves it's mother.
        /// </summary>
        public int RaccoonIndependenceAge { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load the static Raccoon information from the XML settings file
        /// </summary>
        /// <param name="TrialSettings">The dataset containing trial settings</param>
        public void LoadStaticAnimalData(DataSet TrialSettings)
        {
            int i = 0;
            DataRow drAnimals = TrialSettings.Tables["AnimalInformation"].Rows[0];
            // start by setting static values
            this.RaccoonIndependenceAge = Convert.ToInt32(drAnimals["AgeOfIndependence"]);
            this.RaccoonAdultAge = Convert.ToInt32(drAnimals["AdultAge"]);
            this.RaccoonGenderRatio = Convert.ToInt32(drAnimals["GenderRatio"]);
            this.RaccoonAdultBirthOdds = Convert.ToInt32(drAnimals["AdultBirthOdds"]);
            this.RaccoonJuvenileBirthOdds = Convert.ToInt32(drAnimals["JuvenileBirthOdds"]);
            this.RaccoonReproductionWeek = Convert.ToInt32(drAnimals["ReproductionWeek"]);
            this.RaccoonMatingWeek = Convert.ToInt32(drAnimals["MatingWeek"]);
            this.RaccoonMortalityAdjustment = Convert.ToDouble(drAnimals["MortalityAdjustment"]);
            // litter sizes
            DataTable dt = TrialSettings.Tables["AnimalLitterSizeProbability"];
            DataRow dr = null;
            for (i = 0; i <= 7; i++)
            {
                dr = dt.Rows[i];
                this.RaccoonLitterSize[i] = Convert.ToDouble(dr["Value"]);
            }
            // movement distributions
            dt = TrialSettings.Tables["AnimalMovementOddsNormalized"];
            for (i = 0; i <= 49; i++)
            {
                dr = dt.Rows[i];
                this.RaccoonJuvAdultMaleMovementDistribution[i] = Convert.ToDouble(dr["JA_Male"]);
                this.RaccoonJuvAdultFemaleMovementDistribution[i] = Convert.ToDouble(dr["JA_Female"]);
                this.RaccoonYoungMaleMovementDistribution[i] = Convert.ToDouble(dr["YY_Male"]);
                this.RaccoonYoungFemaleMovementDistribution[i] = Convert.ToDouble(dr["YY_Female"]);
            }
            // movement weeks
            dt = TrialSettings.Tables["AnimalMovementWeeks"];
            for (i = 0; i <= 51; i++)
            {
                dr = dt.Rows[i];
                this.RaccoonJuvAdultMaleWeeklyMovement[i] = (Convert.ToInt32(dr["JA_Male"]) != 0);
                this.RaccoonJuvAdultFemaleWeeklyMovement[i] = (Convert.ToInt32(dr["JA_Female"]) != 0);
                this.RaccoonYoungMaleWeeklyMovement[i] = (Convert.ToInt32(dr["YY_Male"]) != 0);
                this.RaccoonYoungFemaleWeeklyMovement[i] = (Convert.ToInt32(dr["YY_Female"]) != 0);
            }
            // home ranges
            dt = TrialSettings.Tables["AnimalHomeRange"];
            for (i = 0; i <= 51; i++)
            {
                dr = dt.Rows[i];
                this.RaccoonMaleHomeRange[i] = Convert.ToDouble(dr["Male"]);
                this.RaccoonFemaleHomeRange[i] = Convert.ToDouble(dr["Female"]);
            }
            // mortality
            dt = TrialSettings.Tables["AnimalInformationWeeklyMortality"];
            for (i = 0; i <= 7; i++)
            {
                dr = dt.Rows[i];
                this.RaccoonMaleMortality[i] = Convert.ToDouble(dr["Male"]);
                this.RaccoonFemaleMortality[i] = Convert.ToDouble(dr["Female"]);
            }
        }

        #endregion

    }
}
