using System;
using System.Collections.Generic;
using System.Text;
using Rabies_Model_Core;
using Random;
using System.Data;

namespace Fox
{
    /// <summary>
    /// A background specifically for foxes.  It will hold "static" fox values (BirthOdds, Mortality etc.) as well
    /// as be the background for the model
    /// </summary>
    public class cFoxBackground : cBackground
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
        public cFoxBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals) 
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
        public cFoxBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, int NYears)
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
		public cFoxBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, int NYears, enumWinterType WinterBias)
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
		public cFoxBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, cWinterTypeList WTList)
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
		public cFoxBackground(cUniformRandom Rnd, string Name, bool KeepAllAnimals, cYearList YList, cDiseaseList DList)
            : base(Rnd, Name, KeepAllAnimals, YList, DList)
        {
            InitValues();
        }

        /// <summary>
        /// Initialize values
        /// </summary>
        private void InitValues()
        {
            this.FoxMortalityAdjustment = 0;
            //this.FoxMatingWeek = 9;
            //this.FoxMatingWeek2 = 0; //EER
            //this.FoxMatingWeek3 = 0; //EER
            this.FoxGestationPeriod = 7;
            //this.FoxReproductionWeek = 18; //YM
            //this.FoxReproductionWeek2 = 0;
            //this.FoxReproductionWeek3 = 0;
            this.FoxReproductionWeekMean = 18; // YM
            this.FoxReproductionWeekVariance = 4; //YM
            this.FoxReproductionWeekMean2 = 0; // YM
            this.FoxReproductionWeekVariance2 = 0; //YM
            this.FoxReproductionWeekMean3 = 0; // YM
            this.FoxReproductionWeekVariance3 = 0; //YM
            this.FoxGenderRatio = 50;
            this.FoxJuvenileBirthOdds = 60;
            this.FoxAdultBirthOdds = 95;
            this.Beta = 1.5;
        }

        #region Public Properties ("Static Fox Values")

        // ************* static members that hold user defined behaviour ******************
        /// <summary>
        ///		Get an array of expected mortalities for male foxes by year.  Each
        ///		position in the array represents the expected mortality of a male Fox in a 
        ///		single week for that year.  The mortality is represented as a percentage, with
        ///		0 representing no chance of death and 100 representing certain death.  The array
        ///		has 8 positions representing years 0 through 7.  No fox in the model will
        ///		live for more than eight years.
        /// </summary>
        public double[] FoxMaleMortality
        {
            get
            {
                return _foxMaleMortality;
            }
        }
        private double[] _foxMaleMortality = new double[8];

        /// <summary>
        ///		Get an array of expected mortalities for female foxes by year.
        ///		Each position in the array represents the expected mortality of a female
        ///		Fox in a single week for that year.  The mortality is represented as a
        ///		percentage, with 0 representing no chance of death and 100 representing certain
        ///		death.  The array has 8 positions representing years 0 through 7.  No fox
        ///		in the model will live for more than eight years.
        /// </summary>
        public double[] FoxFemaleMortality
        {
            get
            {
                return _foxFemaleMortality;
            }
        }
        private double[] _foxFemaleMortality = new double[8];

        /// <summary>
        ///		Get or set an adjustment applied to all mortality calculations to allow
        ///		fine tuning of the population to "expected" values.
        /// </summary>
        public double FoxMortalityAdjustment { get; set; }

        /// <summary>
        ///		Get an array of probabilities defining how far a juvenile or adult
        ///		male fox will move when it gets its chance to move.  Each position in the
        ///		array represents the probability that a fox will move that distance, from
        ///		position 0, representing a movement of 0 cells, to position 49, representing a
        ///		movement of 49 cells.  The values stored in each array position are the
        ///		percentage chance that the fox will move that distance.  All of the values
        ///		in the array should sum to 100.
        /// </summary>
        public double[] FoxJuvAdultMaleMovementDistribution
        {
            get
            {
                return _foxJuvAdultMaleMovementDistribution;
            }
        }
        private double[] _foxJuvAdultMaleMovementDistribution = new double[50];

        /// <summary>
        ///		Get an array of probabilities defining how far a juvenile or adult
        ///		female fox will move when it gets its chance to move.  Each position in the
        ///		array represents the probability that a fox will move that distance, from
        ///		position 0, representing a movement of 0 cells, to position 49, representing a
        ///		movement of 49 cells.  The values stored in each array position are the
        ///		percentage chance that the fox will move that distance.  All of the values
        ///		in the array should sum to 100.
        /// </summary>
        public double[] FoxJuvAdultFemaleMovementDistribution
        {
            get
            {
                return _foxJuvAdultFemaleMovementDistribution;
            }
        }
        private double[] _foxJuvAdultFemaleMovementDistribution = new double[50];
        
        /// <summary>
        ///		Get an array of probabilities defining how far a young male fox
        ///		will move when it gets its chance to move.  This is the fox's first movement
        ///		after leaving its mother.  Each position in the array represents the probability
        ///		that a fox will move that distance, from position 0, representing a
        ///		movement of	0 cells, to position 49, representing a movement of 49 cells.  The
        ///		values stored in each array position are the percentage chance that the fox
        ///		will move that distance.  All of the values in the array should sum to 100.
        /// </summary>
        public double[] FoxYoungMaleMovementDistribution
        {
            get
            {
                return _foxYoungMaleMovementDistribution;
            }
        }
        private double[] _foxYoungMaleMovementDistribution = new double[50];

        /// <summary>
        ///		Get an array of probabilities defining how far a young female fox
        ///		will move when it gets its chance to move.  This is the fox's first movement
        ///		after leaving its mother.  Each position in the array represents the probability
        ///		that a fox will move that distance, from position 0, representing a
        ///		movement of	0 cells, to position 49, representing a movement of 49 cells.  The
        ///		values stored in each array position are the percentage chance that the fox
        ///		will move that distance.  All of the values in the array should sum to 100.
        /// </summary>
        public double[] FoxYoungFemaleMovementDistribution
        {
            get
            {
                return _foxYoungFemaleMovementDistribution;
            }
        }
        private double[] _foxYoungFemaleMovementDistribution = new double[50];

        /// <summary>
        ///		Every fox will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a juvenile or adult male fox may move.  The array has 52 positions, each
        ///		position representing a week in a year (be careful - position 0 corresponds to
        ///		week 1,	and so on).  If a value in the array is zero, the animal may not move
        ///		that week.  If it is any other value, the animal can move that week.  
        /// </summary>
        public bool[] FoxJuvAdultMaleWeeklyMovement
        {
            get
            {
                return _foxJuvAdultMaleWeeklyMovement;
            }
        }
        private bool[] _foxJuvAdultMaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Every fox will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a juvenile or adult female fox may move.  The array has 52 positions, each
        ///		position representing a week in a year (be careful - position 0 corresponds to
        ///		week 1,	and so on).  If a value in the array is zero, the animal may not move
        ///		that week.  If it is any other value, the animal can move that week.  
        /// </summary>
        public bool[] FoxJuvAdultFemaleWeeklyMovement
        {
            get
            {
                return _foxJuvAdultFemaleWeeklyMovement;
            }
        }
        private bool[] _foxJuvAdultFemaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Every fox will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a young male fox may move.  This is the fox's first movement after
        ///		leaving its mother.  The array has 52 positions, each position representing a
        ///		week in a year (be careful - position 0 corresponds to week 1, and so on).  If
        ///		a value in the array is zero, the animal may not move that week.  If it is any
        ///		other value, the animal can move that week.  
        /// </summary>
        public bool[] FoxYoungMaleWeeklyMovement
        {
            get
            {
                return _foxYoungMaleWeeklyMovement;
            }
        }
        private bool[] _foxYoungMaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Every fox will get one chance to move between cells in a
        ///		year.  Get an array of binary values indicating which weeks of the year
        ///		a young female fox may move.  This is the fox's first movement after
        ///		leaving its mother.  The array has 52 positions, each position representing a
        ///		week in a year (be careful - position 0 corresponds to week 1, and so on).  If
        ///		a value in the array is zero, the animal may not move that week.  If it is any
        ///		other value, the animal can move that week. 
        /// </summary>
        public bool[] FoxYoungFemaleWeeklyMovement
        {
            get
            {
                return _foxYoungFemaleWeeklyMovement;
            }
        }
        private bool[] _foxYoungFemaleWeeklyMovement = new bool[52];
        
        /// <summary>
        ///		Get an array of probabilities for litter sizes when a female
        ///		fox gives birth.  The size of the litter is represented by the array
        ///		position (be careful, position 0 represents a litter of 1 young, and so on to
        ///		position 7, which represents a litter of 8 young).  The value at an array
        ///		position represents the probability of a litter of that size, given as a
        ///		percentage.  The array has 8 positions, meaning litter sizes cannot exceed 8
        ///		young.  All of the values in this array should sum to 100.
        ///		EER: I increased the upper limit for the litter size from 8 to 100
        /// </summary>
        public double[] FoxLitterSize
        {
            get
            {
                return _foxLitterSize;
            }
        }
        //private double[] _foxLitterSize = new double[8];
        private double[] _foxLitterSize = new double[100];

        /// <summary>
        /// //YM: 
        ///		Get the mean of the litter size 
        /// </summary>
        public int FoxLitterMean { get; set; }

        /// <summary>
        /// //YM: 
        ///		Get the mean of the litter size 
        /// </summary>
        public int FoxLitterVariance { get; set; }

        ///// <summary>
        /////		Get the week in which mating occurs.  This should occur before the
        /////		ReproductionWeek and should be a value between 1 and 52.  If it is outside this
        /////		range or is set after the ReproductiveWeek, no births will take place in the model.
        /////		By default, this value is set to 9 (late-Winter).
        ///// </summary>
        //public int FoxMatingWeek { get; set; }

        ///// <summary>
        /////		EER:
        /////     Get the week in which mating occurs.  This should occur before the
        /////		ReproductionWeek and should be a value between 1 and 52.  If it is outside this
        /////		range or is set after the ReproductiveWeek, no births will take place in the model.
        /////		By default, this value is set to 0 (no mating).
        ///// </summary>
        //public int FoxMatingWeek2 { get; set; }

        ///// <summary>
        /////     EER:
        /////		Get the week in which mating occurs.  This should occur before the
        /////		ReproductionWeek and should be a value between 1 and 52.  If it is outside this
        /////		range or is set after the ReproductiveWeek, no births will take place in the model.
        /////		By default, this value is set to 0 (no mating).
        ///// </summary>
        //public int FoxMatingWeek3 { get; set; }

        ///<summary>
        ///     YM: 
        ///     Get the number of gestation weeks. It is the time between the mating week and the giving 
        ///     birth week.
        /// <summary>
        public int FoxGestationPeriod { get; set; }
        

        ///		Probability of a male fox being inside any one of the neighbouring cells
        ///		for a given week. Note that array items start at 0 (i.e. week 1 of 52).
        /// </summary>
        public double[] FoxMaleHomeRange
        {
            get
            {
                return _foxMaleHomeRange;
            }
        }
        private double[] _foxMaleHomeRange = new double[52];

        /// <summary>
        ///		Probability of a female fox being inside any one of the neighbouring cells
        ///		for a given week. Note that array items start at 0 (i.e. week 1 of 52).
        /// </summary>
        public double[] FoxFemaleHomeRange
        {
            get
            {
                return _foxFemaleHomeRange;
            }
        }
        private double[] _foxFemaleHomeRange = new double[52];

        ///// <summary>
        /////		Get the week in which female foxes will give birth.  The
        /////		ReproductionWeek should be a value between 1 and 52.  If it is outside this
        /////		range, no births will take place in the model.  By default, this value is set
        /////		to 18 (mid-Spring).
        ///// </summary>
        //public int FoxReproductionWeek { get; set; }

        ///// <summary>
        /////     EER:
        /////		Get the week in which female foxes will give birth.  The
        /////		ReproductionWeek should be a value between 1 and 52.  If it is outside this
        /////		range, no births will take place in the model.  By default, this value is set
        /////		to 0 (no 2nd birth pulse).
        ///// </summary>
        //public int FoxReproductionWeek2 { get; set; }

        ///// <summary>
        /////     EER:
        /////		Get the week in which female foxes will give birth.  The
        /////		ReproductionWeek should be a value between 1 and 52.  If it is outside this
        /////		range, no births will take place in the model. By default, this value is set
        /////		to 0 (no 3rd birth pulse). 
        ///// </summary>
        //public int FoxReproductionWeek3 { get; set; }

        ///<summary>
        ////// YM:
        ///		Get the mean week in which female foxes will give birth.  The
        ///		ReproductionWeekMean should be a value between 1 and 52.  If it is outside this
        ///		range, no births will take place in the model. 
        /// </summary>
        public int FoxReproductionWeekMean { get; set; }

        /// <summary>
        /// YM:
        ///		Get the mean week in which female foxes will give birth.  The
        ///		ReproductionWeekMean should be a value between 1 and 52.  If it is outside this
        ///		range, no births will take place in the model.
        /// </summary>
        public int FoxReproductionWeekMean2 { get; set; }

        /// <summary>
        /// YM:
        ///		Get the variance week in which female foxes will give birth.  The
        ///		ReproductionWeekVariance should be a value between 1 and 52. The max sum of ReproductionWeekMean and ReproductionWeekVariance 
        ///		should be lesser than 52. If it is outside this
        ///		range, no births will take place in the model.
        /// </summary>
        public int FoxReproductionWeekVariance2 { get; set; }

        /// <summary>
        /// YM:
        ///		Get the mean week in which female foxes will give birth.  The
        ///		ReproductionWeekMean should be a value between 1 and 52.  If it is outside this
        ///		range, no births will take place in the model. 
        /// </summary>
        public int FoxReproductionWeekMean3 { get; set; }

        /// <summary>
        /// YM:
        ///		Get the variance week in which female foxes will give birth.  The
        ///		ReproductionWeekVariance should be a value between 1 and 52. The max sum of ReproductionWeekMean and ReproductionWeekVariance 
        ///		should be lesser than 52. If it is outside this
        ///		range, no births will take place in the model.  
        /// </summary>
        public int FoxReproductionWeekVariance3 { get; set; }

        /// YM:
        ///		Get the variance week in which female foxes will give birth.  The
        ///		ReproductionWeekVariance should be a value between 1 and 52. The max sum of ReproductionWeekMean and ReproductionWeekVariance 
        ///		should be lesser than 52. If it is outside this
        ///		range, no births will take place in the model.  By default, this value is set
        ///		to 0 (no 2nd birth pulse).
        /// </summary>
        public int FoxReproductionWeekVariance { get; set; }

        /// <summary>
        ///		Get the gender ratio of young animals born in the model.  This value
        ///		is actually the percentage of young animals that will be male.  This value
        ///		should not be less than 0 or greater than 100.  By default, it is given a value
        ///		of 50.
        /// </summary>
        public int FoxGenderRatio { get; set; }

        /// <summary>
        ///		Get the percentage chance that a juvenile female fox will give
        ///		birth during a ReproductionWeek.  This value should not be less than 0 or
        ///		greater than 100.  By default, it has a value of 60.
        /// </summary>
        public int FoxJuvenileBirthOdds { get; set; }

        /// <summary>
        ///		Get the percentage chance that an adult female fox will give
        ///		birth during a ReproductionWeek.  This value should not be less than 0 or
        ///		greater than 100.  By default, it has a value of 95.
        /// </summary>
        public int FoxAdultBirthOdds { get; set; }

        /// <summary>
        ///		Get the age, in weeks, that a fox becomes an adult.
        /// </summary>
        public int FoxAdultAge { get; set; }

        /// <summary>
        ///		Get the age, in weeks, that a fox becomes independent.  That is,
        ///		the age at which the fox leaves it's mother.
        /// </summary>
        public int FoxIndependenceAge { get; set; }

        /// <summary>
        ///		Get the inflexion point of the logistic function, that regulate the mortality density-dependent 
        /// </summary>
        public double Beta { get; set; }

        /// <summary>
        ///		Get the slope at the inflexion point of the logistic function, that regulate the mortality density-dependent 
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        ///		Use Beta, the inflexion point of the logistic function given by user 
        /// </summary>
        public bool LogisticBeta { get; set; }

        /// <summary>
        ///		Use Alpha, the slope at inflexion point of the logistic function given by user 
        /// </summary>
        public bool LogisticAlpha { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Load the static Fox information from the Excel template settings file
        /// </summary>
        /// <param name="TrialSettings">The dataset containing trial settings</param>
        public void LoadStaticAnimalDataExcelTemplate(DataSet TrialSettings)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cFoxBackground.cs: LoadStaticAnimalDataExcelTemplate()");

            // Define different data tables containing the run definitions from the Excel template
            DataTable dtAnimalBehav = TrialSettings.Tables[0];
            DataTable dtAnimalBiol = TrialSettings.Tables[1];
            //DataTable dtDiseaseCtrl = TrialSettings.Tables[2];
            //DataTable dtEpi = TrialSettings.Tables[3];
            //DataTable dtRunInfo = TrialSettings.Tables[4];
            //DataTable dtWinter = TrialSettings.Tables[6];

            // start by setting static values           
            this.FoxIndependenceAge = Convert.ToInt32(dtAnimalBiol.Rows[0][1]);            
            this.FoxAdultAge = Convert.ToInt32(dtAnimalBiol.Rows[1][1]);            
            this.FoxGenderRatio = Convert.ToInt32(dtAnimalBiol.Rows[10][1]);            
            this.FoxAdultBirthOdds = Convert.ToInt32(dtAnimalBiol.Rows[7][1]);            
            this.FoxJuvenileBirthOdds = Convert.ToInt32(dtAnimalBiol.Rows[6][1]);
            this.FoxMortalityAdjustment = Convert.ToDouble(dtAnimalBiol.Rows[2][1]);
            //this.FoxMatingWeek = Convert.ToInt32(dtAnimalBiol.Rows[3][1]); //YM
            //this.FoxMatingWeek2 = Convert.ToInt32(dtAnimalBiol.Rows[3][2]);//YM
            //this.FoxMatingWeek3 = Convert.ToInt32(dtAnimalBiol.Rows[3][3]);//YM
            //this.FoxReproductionWeek = Convert.ToInt32(dtAnimalBiol.Rows[4][1]);//YM
            //this.FoxReproductionWeek2 = Convert.ToInt32(dtAnimalBiol.Rows[4][2]);//YM
            //this.FoxReproductionWeek3 = Convert.ToInt32(dtAnimalBiol.Rows[4][3]);//YM

            // YM: given the reproduction week mean and variance, the porbability to have 
            //      a reproduction week assuming a gaussian distribution were calculated
            this.FoxReproductionWeekMean = Convert.ToInt32(dtAnimalBiol.Rows[4][1]);
            this.FoxReproductionWeekVariance = Convert.ToInt32(dtAnimalBiol.Rows[4][2]);
            this.FoxReproductionWeekMean2 = Convert.ToInt32(dtAnimalBiol.Rows[4][3]);
            this.FoxReproductionWeekVariance2 = Convert.ToInt32(dtAnimalBiol.Rows[4][4]);
            this.FoxReproductionWeekMean3 = Convert.ToInt32(dtAnimalBiol.Rows[4][5]);
            this.FoxReproductionWeekVariance3 = Convert.ToInt32(dtAnimalBiol.Rows[4][6]);
            this.FoxGestationPeriod = Convert.ToInt32(dtAnimalBiol.Rows[3][1]);
            this.LogisticBeta = Convert.ToBoolean(dtAnimalBiol.Rows[24][1]);
            this.LogisticAlpha = Convert.ToBoolean(dtAnimalBiol.Rows[22][1]);
            this.Beta = Convert.ToDouble(dtAnimalBiol.Rows[25][1]);
            this.Alpha = Convert.ToDouble(dtAnimalBiol.Rows[23][1]);


            //litter sizes
            // EER: Given litter size mean and variance, calculate the probability of having a litter size of x
            //      Assumes a normal distribution for the pdf
            //      Store results in the array: this.FoxLitterSize[i]
            // EER: Get litter size mean
            Single litterMean = Convert.ToSingle(dtAnimalBiol.Rows[8][1]);
            // EER: Get litter size variance
            Single litterVar = Convert.ToSingle(dtAnimalBiol.Rows[9][1]);
            //System.Diagnostics.Debug.WriteLine("    litterMean = " + litterMean + "; litterVar = " + litterVar);
            // EER: A probability value is not calculated for a litter size of 0
            //      So, calculate pdf values for (litterMean x 2) - 1, so the mean is at the peak probability
            cGaussianPDF valPDF = new cGaussianPDF();
            float valueUpperLimit;
            // Need litterMean as an integer (i.e. whole) number for the pdf calculations
            // Check if litterMean is an integer
            bool isInt = litterMean == (int)litterMean;
            //System.Diagnostics.Debug.WriteLine("    isInt = " + isInt);
            if (isInt == false) {
                litterMean = Convert.ToInt32(litterMean);
                //Calculate pdf values over 2 * the mean - 1
                valueUpperLimit = 2 * litterMean - 1;
            }
            else
            {
                //Calculate pdf values over 2 * the mean
                valueUpperLimit = 2 * litterMean;                
            }
            //System.Diagnostics.Debug.WriteLine("    litterMean = " + litterMean + "; valueUpperLimit = " + valueUpperLimit);                        
            double pdf;
            int localPDF = 1;                 
            for (int i = 0; i <= valueUpperLimit-1; i++) {
                //System.Diagnostics.Debug.WriteLine("    i = " + i);
                pdf = valPDF.xGaussianPDF(localPDF, litterMean,litterVar);
                //Add pdf value for each possible litter size
                this.FoxLitterSize[i] = pdf;                
                //System.Diagnostics.Debug.WriteLine("    pdf at " + localPDF + " = " + pdf);
                localPDF++; // Increment the local PDF value
            }
            /*for (int i = 0; i < this.FoxLitterSize.Length; i++) {
                double test = this.FoxLitterSize[i];
                System.Diagnostics.Debug.WriteLine("    pdf at litter size " + i + " = " + test);
            }*/
            //YM:  //litter sizes
            FoxLitterMean = Convert.ToInt32(dtAnimalBiol.Rows[8][1]);
            FoxLitterVariance = Convert.ToInt32(dtAnimalBiol.Rows[9][1]);

            // movement distributions defined for upto 50 landscape cells
            // Get user-input distances
            double[] yoym = new double[50];
            double[] yoyf = new double[50];
            double[] jam = new double[50];
            double[] jaf = new double[50];
            double yoymSUM = 0;
            double yoyfSUM = 0;
            double jamSUM = 0;
            double jafSUM = 0;
            for (int i = 0; i <= 49; i++) {
                yoym[i] = Convert.ToDouble(dtAnimalBehav.Rows[8+i][11]);                
                yoyf[i] = Convert.ToDouble(dtAnimalBehav.Rows[8 + i][12]);
                jam[i] = Convert.ToDouble(dtAnimalBehav.Rows[8 + i][13]);
                jaf[i] = Convert.ToDouble(dtAnimalBehav.Rows[8 + i][14]);
                //System.Diagnostics.Debug.WriteLine("    YOYM dist prob at " + i + " = " + yoym[i] + "; YOYF at " + i + " = " + yoyf[i]);
                //Sum the distance probabilities
                yoymSUM = yoymSUM + yoym[i];
                yoyfSUM = yoyfSUM + yoyf[i];
                jamSUM = jamSUM + jam[i];
                jafSUM = jafSUM + jaf[i];
                //System.Diagnostics.Debug.WriteLine("    yoymSUM = " + yoymSUM + "; yoyfSUM = " + yoyfSUM + "; jamSUM = " + jamSUM + "; jafSUM = " + jafSUM);
            }
            // If the distance probabilities do not sum to 100, then scale the values from 0 to 100
            // young-of-year male
            if (yoymSUM > 0 & yoymSUM != 100)
            {
                for (int i = 0; i <= 49; i++)
                {
                    this.FoxYoungMaleMovementDistribution[i] = yoym[i]/yoymSUM;
                    //System.Diagnostics.Debug.WriteLine("    YOYM scale value at " + i + " = " + this.FoxYoungMaleMovementDistribution[i]);
                }
            }
            else {
                for (int i = 0; i <= 49; i++) {
                    this.FoxYoungMaleMovementDistribution[i] = yoym[i];
                    //System.Diagnostics.Debug.WriteLine("    YOYM nonscale value at " + i + " = " + this.FoxYoungMaleMovementDistribution[i]);
                }
            }           
            // young-of-year female
            if (yoyfSUM > 0 & yoyfSUM != 100)
            {
                for (int i = 0; i <= 49; i++)
                {
                    this.FoxYoungFemaleMovementDistribution[i] = yoyf[i] / yoyfSUM;
                    //System.Diagnostics.Debug.WriteLine("    YOYF scale value at " + i + " = " + this.FoxYoungFemaleMovementDistribution[i]);
                }
            }
            else {
                for (int i = 0; i <= 49; i++)
                {
                    this.FoxYoungFemaleMovementDistribution[i] = yoyf[i];
                    //System.Diagnostics.Debug.WriteLine("    YOYF nonscale value at " + i + " = " + this.FoxYoungFemaleMovementDistribution[i]);
                }
            }
            // juvenile adult male
            if (jamSUM > 0 & jamSUM != 100)
            {
                for (int i = 0; i <= 49; i++)
                {
                    this.FoxJuvAdultMaleMovementDistribution[i] = jam[i] / jamSUM;
                    //System.Diagnostics.Debug.WriteLine("    JAM scale value at " + i + " = " + this.FoxJuvAdultMaleMovementDistribution[i]);
                }
            }
            else {
                for (int i = 0; i <= 49; i++)
                {
                    this.FoxJuvAdultMaleMovementDistribution[i] = jam[i];
                    //System.Diagnostics.Debug.WriteLine("    JAM nonscale value at " + i + " = " + this.FoxJuvAdultMaleMovementDistribution[i]);
                }
            }
            // juvenile adult female
            if (jafSUM > 0 & jafSUM != 100)
            {
                for (int i = 0; i <= 49; i++)
                {
                    this.FoxJuvAdultFemaleMovementDistribution[i] = jaf[i] / jafSUM;
                    //System.Diagnostics.Debug.WriteLine("    JAF scale value at " + i + " = " + this.FoxJuvAdultFemaleMovementDistribution[i]);
                }
            }
            else {
                for (int i = 0; i <= 49; i++)
                {
                    this.FoxJuvAdultFemaleMovementDistribution[i] = jaf[i];
                    //System.Diagnostics.Debug.WriteLine("    JAF nonscale value at " + i + " = " + this.FoxJuvAdultFemaleMovementDistribution[i]);
                }
            }

            // movement weeks for the year
            for (int i = 0; i <= 51; i++)
            {               
                this.FoxYoungMaleWeeklyMovement[i] = Convert.ToBoolean(dtAnimalBehav.Rows[8 + i][5]);
                this.FoxYoungFemaleWeeklyMovement[i] = Convert.ToBoolean(dtAnimalBehav.Rows[8 + i][6]);
                this.FoxJuvAdultMaleWeeklyMovement[i] = Convert.ToBoolean(dtAnimalBehav.Rows[8 + i][7]);
                this.FoxJuvAdultFemaleWeeklyMovement[i] = Convert.ToBoolean(dtAnimalBehav.Rows[8 + i][8]);
                //System.Diagnostics.Debug.WriteLine("    i = " + i + "; yoym = " + this.FoxYoungMaleWeeklyMovement[i] + "; yoyf = " + this.FoxYoungFemaleWeeklyMovement[i] + "; jam = " + this.FoxJuvAdultMaleWeeklyMovement[i] + "; jaf = " + this.FoxJuvAdultFemaleWeeklyMovement[i]);
            }

            // home ranges for males and females 
            // EER: the probability of being outside the home cell during the week time step            
            for (int i = 0; i <= 51; i++)
            {                
                this.FoxMaleHomeRange[i] = Convert.ToDouble(dtAnimalBehav.Rows[8 + i][1]);
                this.FoxFemaleHomeRange[i] = Convert.ToDouble(dtAnimalBehav.Rows[8 + i][2]);
                //System.Diagnostics.Debug.WriteLine("    i = " + i + "; maleHR = " + this.FoxMaleHomeRange[i] + "; femaleHR = " + this.FoxFemaleHomeRange[i]);
            }

            // mortality          
            /*for (int i = 0; i <= 7; i++)
            {                
                this.FoxMaleMortality[i] = Convert.ToDouble(dtAnimalBiol.Rows[13 + i][1]);
                this.FoxFemaleMortality[i] = Convert.ToDouble(dtAnimalBiol.Rows[13 + i][2]);
                //System.Diagnostics.Debug.WriteLine("    i = " + i + "; maleMort = " + this.FoxMaleMortality[i] + "; femaleMort = " + this.FoxFemaleMortality[i]);
            }*/           

            for (int i = 0; i <= 7; i++)
            {
                this.FoxMaleMortality[i] = Convert.ToDouble(dtAnimalBiol.Rows[13 + i][1]);
                this.FoxFemaleMortality[i] = Convert.ToDouble(dtAnimalBiol.Rows[13 + i][2]);
                //System.Diagnostics.Debug.WriteLine("    i = " + i + "; maleMort = " + this.FoxMaleMortality[i] + "; femaleMort = " + this.FoxFemaleMortality[i]);
            }

        }

        /// <summary>
        /// Load the static Fox information from the XML settings file
        /// </summary>
        /// <param name="TrialSettings">The dataset containing trial settings</param>
        public void LoadStaticAnimalData(DataSet TrialSettings)
        {
            int i = 0;
            DataRow drAnimals = TrialSettings.Tables["AnimalInformation"].Rows[0];
            // start by setting static values
            this.FoxIndependenceAge = Convert.ToInt32(drAnimals["AgeOfIndependence"]);
            this.FoxAdultAge = Convert.ToInt32(drAnimals["AdultAge"]);
            this.FoxGenderRatio = Convert.ToInt32(drAnimals["GenderRatio"]);
            this.FoxAdultBirthOdds = Convert.ToInt32(drAnimals["AdultBirthOdds"]);
            this.FoxJuvenileBirthOdds = Convert.ToInt32(drAnimals["JuvenileBirthOdds"]);
            //this.FoxReproductionWeek = Convert.ToInt32(drAnimals["ReproductionWeek"]); //YM
            this.FoxReproductionWeekMean = Convert.ToInt32(drAnimals["ReproductionWeekMean"]); //YM
            this.FoxReproductionWeekVariance = Convert.ToInt32(drAnimals["ReproductionWeekVariance"]); //YM
            this.FoxReproductionWeekMean2 = Convert.ToInt32(drAnimals["ReproductionWeekMean2"]);//YM
            this.FoxReproductionWeekVariance2 = Convert.ToInt32(drAnimals["ReproductionWeekVariance2"]);//YM
            this.FoxReproductionWeekMean3 = Convert.ToInt32(drAnimals["ReproductionWeekMean3"]);//YM
            this.FoxReproductionWeekVariance3 = Convert.ToInt32(drAnimals["ReproductionWeekVariance3"]);//YM
            //this.FoxMatingWeek = Convert.ToInt32(drAnimals["MatingWeek"]); //YM
            this.FoxGestationPeriod = Convert.ToInt32(drAnimals["GestationPeriod"]);
            this.FoxMortalityAdjustment = Convert.ToDouble(drAnimals["MortalityAdjustment"]);
            // litter sizes
            DataTable dt = TrialSettings.Tables["AnimalLitterSizeProbability"];
            DataRow dr = null;
            //for (i = 0; i <= 7; i++)
            //{
            //    dr = dt.Rows[i];
            //    this.FoxLitterSize[i] = Convert.ToDouble(dr["Value"]);
            //}
            this.FoxLitterMean = Convert.ToInt32(drAnimals["LitterMean"]);//YM
            this.FoxLitterVariance = Convert.ToInt32(drAnimals["LitterVariance"]);//YM

            // movement distributions
            dt = TrialSettings.Tables["AnimalMovementOddsNormalized"];
            for (i = 0; i <= 49; i++)
            {
                dr = dt.Rows[i];
                this.FoxJuvAdultMaleMovementDistribution[i] = Convert.ToDouble(dr["JA_Male"]);
                this.FoxJuvAdultFemaleMovementDistribution[i] = Convert.ToDouble(dr["JA_Female"]);
                this.FoxYoungMaleMovementDistribution[i] = Convert.ToDouble(dr["YY_Male"]);
                this.FoxYoungFemaleMovementDistribution[i] = Convert.ToDouble(dr["YY_Female"]);
            }
            // movement weeks
            dt = TrialSettings.Tables["AnimalMovementWeeks"];
            for (i = 0; i <= 51; i++)
            {
                dr = dt.Rows[i];
                this.FoxJuvAdultMaleWeeklyMovement[i] = (Convert.ToInt32(dr["JA_Male"]) != 0);
                this.FoxJuvAdultFemaleWeeklyMovement[i] = (Convert.ToInt32(dr["JA_Female"]) != 0);
                this.FoxYoungMaleWeeklyMovement[i] = (Convert.ToInt32(dr["YY_Male"]) != 0);
                this.FoxYoungFemaleWeeklyMovement[i] = (Convert.ToInt32(dr["YY_Female"]) != 0);
            }
            // home ranges
            dt = TrialSettings.Tables["AnimalHomeRange"];
            for (i = 0; i <= 51; i++)
            {
                dr = dt.Rows[i];
                this.FoxMaleHomeRange[i] = Convert.ToDouble(dr["Male"]);
                this.FoxFemaleHomeRange[i] = Convert.ToDouble(dr["Female"]);
            }
            // mortality
            dt = TrialSettings.Tables["AnimalInformationWeeklyMortality"];
            for (i = 0; i <= 7; i++)
            {
                dr = dt.Rows[i];
                this.FoxMaleMortality[i] = Convert.ToDouble(dr["Male"]);
                this.FoxFemaleMortality[i] = Convert.ToDouble(dr["Female"]);
            }
        }

        #endregion

    }
}
