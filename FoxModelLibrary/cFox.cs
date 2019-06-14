using System;
using Rabies_Model_Core;
using Random;
using System.IO;
using System.Text;

namespace Fox
{
	/// <summary>
	///		A class that represents a single fox.  This class is derived from cAnimal.
	///		Mortality, birth rate and movement are all defined in ths object.  Before using
	///		this class, the user is responsible for setting the values of several static
	///		variables, in order to define specific behaviour.
	/// </summary>
	public class cFox : cAnimal
	{
		// ********************* Constructors *******************************************
		/// <summary>
		///		Construct a fox by specifying its ID, Starting Cell and Background.
		///		This constructor is used to create the initial foxes in the background.
		///		All	foxes created by this constructor come into existance at year = 0,
		///		week = 1.  Although the foxes created here will start out as juveniles,
		///		they will have no parents.
		/// </summary>
		/// <param name="ID">
		///		The ID of the fox.  An ArgumentException exception is raised if the ID has
		///		zero length.
		///	</param>
		/// <param name="CellID">
		///		The ID of the cell initially occupied by this fox.  An ArgumentException
		///		exception is raised if the cell is not part of the background object to
		///		which this fox belongs.
		///	</param>
		/// <param name="Background">
		///		The background object in which the fox lives.  An ArgumentNullException
		///		exception is raised if Background is null.
		/// </param>
		/// <param name="Gender">
		///		The gender of the fox.
		/// </param>
		public cFox(string ID, string CellID, cBackground Background, 
						enumGender Gender) : base(ID, CellID, Background, Gender)
		{
		}

		/// <summary>
		///		Construct a fox by specifying its ID and Parent.  This constructor is
		///		used when giving birth to new foxes.  The background, and initial cell 
		///		are obtained from the parent fox.  The year and week of birth are 
		///		obtained from the background that the parent belongs to.
		/// </summary>
		/// <param name="ID">
		///		The ID of the fox.  An ArgumentException exception is raised if the ID has
		///		zero length.
		///	</param>
		/// <param name="Parent">
		///		The parent of the fox.  An ArgumentNullException exception is raised if
		///		Parent is null.
		///	</param>
		/// <param name="Gender">The gender of the fox.</param>
		public cFox(string ID, cFox Parent, enumGender Gender) :
							base(ID, Parent, Gender)
		{
		}

		/// <summary>
		///		Construct a fox from a cAnimalAttributes object.  This constructor is
		///		used when loading fox data from a datasource.
		/// </summary>
		/// <param name="Attributes">
		///		The cAnimalAttributes object containing the attributes for the fox.  An
		///		ArgumentNullException exception is raised if Attributes is null.
		/// </param>
		/// <param name="Background">
		///		The background object in which the fox lives.  An ArgumentNullException
		///		exception is raised if Background is null.
		/// </param>
		public cFox(cAnimalAttributes Attributes, cBackground Background) :
							base(Attributes, Background)
		{
		}

		// ********************* Properties **********************************************
		/// <summary>
		///		The age class of the fox. Categories are as follows:  juvenile - 
		///		dependent on its parent, yearling - independent animal up to a year old, 
		///		adult - independent animal over a year old.
		/// </summary>
		public override enumAgeClass AgeClass 
		{
			get 
			{
                //if (!this.IsIndependent) 
                //if (this.Age < 34)
                if (this.Age < (this.Background as cFoxBackground).FoxIndependenceAge) //YM
                    return enumAgeClass.YoungOfTheYear;
                else if (this.Age < (this.Background as cFoxBackground).FoxAdultAge)
					return enumAgeClass.Juvenile;
				else
					return enumAgeClass.Adult;
			}
		}

        // ********************* Protected  Methods **************************************
        /// <summary>
        ///  YM :
        ///     This function is used to determine the mating week.
        ///     It is calculated from the value given by user and the reproduction week values. 
        ///     By default, it is fixed to 7 weeks before the birth.
        /// </summary>
        /// <param name="Week">
        ///     The reproduction week. 
        /// </param>
        /// <returns> The value of the reproduction week.</returns>
        public override int ComputeMatingWeek(int Week)
        {
            int FoxGestationPeriodInt = Convert.ToInt32((this.Background as cFoxBackground).FoxGestationPeriod);
            if (Week == 0) return 0;
            
            if (Week - FoxGestationPeriodInt < 1)
            {
                return 1;
            }
            else return (Week - FoxGestationPeriodInt);
            //System.Diagnostics.Debug.WriteLine("cFox.cs: FoxGestationPeriodInt");
        }


        /// <summary>
        ///		A function called each week for each female fox.  This function is used
        ///		to determine whether or not the female should mate in the passed week.
        /// </summary>
        /// <param name="Week">
        ///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
        ///		is not in the range of 1-52.
        ///	</param>
        /// <returns>True if the fox should mate.</returns>
        protected override bool Mate(int Week)
		{
            /*
            // make sure week is a valid value
			if (Week < 1 || Week > 52) ThrowWeekException("Week");
			// if this is the mating week then return true
            return (Week == (this.Background as cFoxBackground).FoxMatingWeek);
            */

            // make sure week is a valid value, from a non-event of "0" to week values 1 to 52
            if (Week < 0 || Week > 52) ThrowWeekException("Week");
            // if this is the mating week then return true
            // EER: 
            //Week = (this.Background as cFoxBackground).FoxMatingWeek | (this.Background as cFoxBackground).FoxMatingWeek2 | (this.Background as cFoxBackground).FoxMatingWeek3;
            //bool mateBool = true;//YM
            bool mateBool = false;
            if (Week == this.MatingWeek || Week == this.MatingWeek2 || Week == this.MatingWeek3) { mateBool = true; }
            //if (Week == 0) { mateBool = false; }
            return (mateBool);
        }



        /// <summary>
        ///  YM :
        ///		A function called each week for each female fox.  This function is used
        ///		to determine the reproduction week: It is a probability function distribution value
        ///      assuming a Gaussian distribuion, where the Mean and the Variance were given by the user.
        /// </summary>
        /// <returns> The value of the reproduction week.</returns>
        public override int ComputeReproductionWeekGaussian()
        {
            double FoxReproductionWeekMeanDouble = Convert.ToDouble((this.Background as cFoxBackground).FoxReproductionWeekMean);
            double FoxReproductionWeekVarianceDouble = Convert.ToDouble((this.Background as cFoxBackground).FoxReproductionWeekVariance);
            
            if (FoxReproductionWeekMeanDouble < 0 || FoxReproductionWeekMeanDouble > 52) ThrowWeekException("Week");

            int FoxReproductionWeekInt = (int)FoxReproductionWeekMeanDouble;
            
            // if the variance > 0, calculate the reproduction week, otherwise, the reproduction week is fixed
            if (FoxReproductionWeekVarianceDouble > 0)
            {
                cRandomBase rnd = new cUniformRandom();
                cGaussianRandom RandomGaussianValue = new cGaussianRandom(FoxReproductionWeekMeanDouble, FoxReproductionWeekVarianceDouble, rnd);
                do
                {
                    FoxReproductionWeekInt = (int)RandomGaussianValue.Value;
                } while (FoxReproductionWeekInt < 0 || FoxReproductionWeekInt > 52);
            }
            return (FoxReproductionWeekInt);
            //System.Diagnostics.Debug.WriteLine("cFox.cs: FoxReproductionWeekInt");
        }

        /// <summary>
        ///  YM :
        ///		This function is used to determine the reproduction week. 
        ///		It is a probability function distribution value
        ///     assuming a Gaussian distribuion, where the Mean and the Variance were given by the user.
        /// </summary>
        /// <returns> The value of the reproduction week.</returns>
        public override int ComputeReproductionWeekGaussian2()
        {
            Double FoxReproductionWeekMeanDouble = Convert.ToDouble((this.Background as cFoxBackground).FoxReproductionWeekMean2);
            Double FoxReproductionWeekVarianceDouble = Convert.ToDouble((this.Background as cFoxBackground).FoxReproductionWeekVariance2);

            int FoxReproductionWeekInt = (int)FoxReproductionWeekMeanDouble;
            if (FoxReproductionWeekMeanDouble > 0)
            {
                // if the variance > 0, calculate the reproduction week, otherwise, the reproduction week is fixed
                if (FoxReproductionWeekVarianceDouble > 0)
                {
                    cRandomBase rnd = new cUniformRandom();
                    cGaussianRandom RandomGaussianValue = new cGaussianRandom(FoxReproductionWeekMeanDouble, FoxReproductionWeekVarianceDouble, rnd);
                    do
                    {
                        FoxReproductionWeekInt = (int)RandomGaussianValue.Value;
                    } while (FoxReproductionWeekInt < 0 || FoxReproductionWeekInt > 52);
                }
            }
           
            return (FoxReproductionWeekInt);

            //System.Diagnostics.Debug.WriteLine("cFox.cs: FoxReproductionWeekInt");
        }
        /// <summary>
        ///  YM :
        ///		This function is used to determine the reproduction week. 
        ///		It is a probability function distribution value
        ///     assuming a Gaussian distribuion, where the Mean and the Variance were given by the user.
        /// </summary>
        /// <returns> The value of the reproduction week.</returns>
        public override int ComputeReproductionWeekGaussian3()
        {
            Double FoxReproductionWeekMeanDouble = Convert.ToDouble((this.Background as cFoxBackground).FoxReproductionWeekMean3);
            Double FoxReproductionWeekVarianceDouble = Convert.ToDouble((this.Background as cFoxBackground).FoxReproductionWeekVariance3);

            int FoxReproductionWeekInt = (int)FoxReproductionWeekMeanDouble;

            if (FoxReproductionWeekMeanDouble > 0)
            {
                // if the variance > 0, calculate the reproduction week, otherwise, the reproduction week is fixed
                if (FoxReproductionWeekVarianceDouble > 0)
                {
                    cRandomBase rnd = new cUniformRandom();
                    cGaussianRandom RandomGaussianValue = new cGaussianRandom(FoxReproductionWeekMeanDouble, FoxReproductionWeekVarianceDouble, rnd);
                    do
                    {
                        FoxReproductionWeekInt = (int)RandomGaussianValue.Value;
                    } while (FoxReproductionWeekInt < 0 || FoxReproductionWeekInt > 52);
                }
            }
            return (FoxReproductionWeekInt);
            //System.Diagnostics.Debug.WriteLine("cFox.cs: FoxReproductionWeekInt");
        }

        /// <summary>
        ///		A function called each week for each female fox.  This function is used
        ///		to determine whether or not the female should give birth in the passed week.
        /// </summary>
        /// <param name="Week">
        ///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
        ///		is not in the range of 1-52.
        ///	</param>
        /// <returns>True if the fox should give birth.</returns>
        protected override bool GiveBirth(int Week) 
		{
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cFox.cs: GiveBirth()");
            
            // make sure week is a valid value
            if (Week < 1 || Week > 52) ThrowWeekException("Week");
            // give birth
            //System.Diagnostics.Debug.WriteLine("cFox.cs: GiveBirth(): Week = " + Week);

            //if (Week == (this.Background as cFoxBackground).FoxReproductionWeek)
            //EER:
            //YM: //if (Week == (this.Background as cFoxBackground).FoxReproductionWeek | Week == (this.Background as cFoxBackground).FoxReproductionWeek2 | Week == (this.Background as cFoxBackground).FoxReproductionWeek3)

            if (Week == this.ReproductionWeek | Week == this.ReproductionWeek2 | Week == this.ReproductionWeek3)
            {
                //System.Diagnostics.Debug.WriteLine("cFox.cs: GiveBirth(): If here, give birth in Week = " + Week);

                if (this.AgeClass == enumAgeClass.Juvenile)
                    return (this.Background.RandomNum.IntValue(1, 100) <= (this.Background as cFoxBackground).FoxJuvenileBirthOdds);
				else if (this.AgeClass == enumAgeClass.Adult)
                    return (this.Background.RandomNum.IntValue(1, 100) <= (this.Background as cFoxBackground).FoxAdultBirthOdds);
				else
					return false;
			}
            //System.Diagnostics.Debug.WriteLine("cFox.cs: GiveBirth() END");
            return false;
		}

		/// <summary>
		///		Determine the number of babies to give birth to.
        ///		If it is Eve, she will also return the max number of babies.
		/// </summary>
		/// <returns>The number of babies to give birth to.</returns>
		protected override int GetNBabies() 
		{
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cFox.cs: GetNBabies()");

            double RanValue = this.Background.RandomNum.RealValue(0, 100);
			double OddsSum = 0;
            double[] LitterSize = (this.Background as cFoxBackground).FoxLitterSize;
            int LitterMean = (this.Background as cFoxBackground).FoxLitterMean;
            int LitterVariance = (this.Background as cFoxBackground).FoxLitterVariance;
            // loop, adding up litter size values until the value of the random number is
            // equaled or exceeded
            for (int i = 0; i < 20; i++)
            {
                OddsSum += LitterSize[i];
				if (OddsSum >= RanValue) return i + 1;
			}
            // just in case
            //return 20;
            return (LitterMean + LitterVariance);
        }

		/// <summary>
		///		Create a new baby fox.
		/// </summary>
		/// <param name="ID">
		///		The ID to be assigned to the baby animal.  An ArgumentException exception is
		///		raised if ID has zero length.
		///	</param>
		/// <returns>The new fox returned as a cAnimal.</returns>
		protected override cAnimal CreateBaby(string ID) 
		{
			// make sure ID is not zero length
			if (ID.Length == 0)
				throw new ArgumentException("ID cannot have zero length.", "ID");
			// create the baby
			enumGender TheGender;
            if (this.Background.RandomNum.IntValue(1, 100) <= (this.Background as cFoxBackground).FoxGenderRatio) 
				TheGender = enumGender.male;
			else
				TheGender = enumGender.female;
			return new cFox(ID, this, TheGender); 
		}

		/// <summary>
		///		Determine whether or not a fox should disperse (ie. leave mum and become
		///		independent.  If the fox does disperse, this function should define the
		///		movement behaviour associated with dispersal.  This function is called each
		///		week until the animal disperses.
		/// </summary>
		/// <param name="CurrentWeek">
		///		The week value which the function can test to see if the animal should become
		///		independent.  An ArgumentOutOfRangeException exception is raised if CurrentWeek
		///		is not in the range of 1-52.
		/// </param>
		/// <returns>True if the animal disperses.</returns>
		protected override bool BecomeIndependent(int CurrentWeek)
		{
			if (CurrentWeek < 1 || CurrentWeek > 52) ThrowWeekException("CurrentWeek");
			// happens at defined Independence age
            return (this.Age == (this.Background as cFoxBackground).FoxIndependenceAge);
		}

		/// <summary>
		///		Define movement behaviour for independent (yearling and adult) foxes.
		/// </summary>
		/// <param name="CurrentWeek">
		///		The current week of the year.  An ArgumentOutOfRangeException exception is
		///		raised if CurrentWeek is not in the range of 1-52.
		///	</param>
		/// <returns>True if the fox changes cell.</returns>
		protected override bool Move(int CurrentWeek)
		{           

            if (CurrentWeek < 1 || CurrentWeek > 52) ThrowWeekException("CurrentWeek");
            // see if this is week 0, if it is, reset has moved
            ///////////////////////////////EER: the next line has been commented to enable >1 movement per year
            //if (CurrentWeek == 1) HasMoved = false;
            if (CurrentWeek > 0 || CurrentWeek < 53) HasMoved = false; //EER            

            // next, if this animal has moved, leave now
            //EER: the next line has been commented out to enable >1 movement per year
            //if (HasMoved) return false; 

            //EER: Do not all Adam & Eve to move, to help grow the population
            if (this.ID == "0" | this.ID == "1")
            {
                //System.Diagnostics.Debug.WriteLine("cFox.cs: Move(): Adam or Eve, ID = " + this.ID + "; don't move");
                return false;
            }
           
            // now load the appropriate movement arrays depending upon what type of animal
            // you are
            bool[] MovementArray;
			double[] MovementDistribution;
			if (this.AgeClass == enumAgeClass.YoungOfTheYear) {
				if (this.Gender == enumGender.female) {
                    MovementArray = (this.Background as cFoxBackground).FoxYoungFemaleWeeklyMovement;
                    MovementDistribution = (this.Background as cFoxBackground).FoxYoungFemaleMovementDistribution;
				}
				else {
                    MovementArray = (this.Background as cFoxBackground).FoxYoungMaleWeeklyMovement;
                    MovementDistribution = (this.Background as cFoxBackground).FoxYoungMaleMovementDistribution;
				}
			}
			else {
				if (this.Gender == enumGender.female) {
                    MovementArray = (this.Background as cFoxBackground).FoxJuvAdultFemaleWeeklyMovement;
                    MovementDistribution = (this.Background as cFoxBackground).FoxJuvAdultFemaleMovementDistribution;
				}
				else {
                    MovementArray = (this.Background as cFoxBackground).FoxJuvAdultMaleWeeklyMovement;
                    MovementDistribution = (this.Background as cFoxBackground).FoxJuvAdultMaleMovementDistribution;
				}
			}
            
            // can the animal even move this week?  If not, leave now!!
            if (!MovementArray[CurrentWeek - 1]) return false;

            // if it can, work out the current odds of moving for the current week
            double MovementOdds = 0;
			double OddsDenominator = 0;
			for (int j = 0; j < 52; j++) {
				if (MovementArray[j]) {
					OddsDenominator += 1;
					if (j <= CurrentWeek) MovementOdds += 1;
				}
			}            
            // if odds denominator is 0, then no movement is defined, leave now
            if (OddsDenominator == 0) {
                // set the HasMoved flag to true and exit as if movement has happened
                HasMoved = true;
                return true;                
			}
            
            // calculate current movement odds 
            // EER: this is needed to calculate the odds of moving for the current permissible week, given that there may be
            //  more than one permissible weeks, and for the raccoon model, only 1 movement allowed per year
            // EER: For ARM, the animal is allowed >1 movement per year, so this code is not needed
            /*
            MovementOdds = MovementOdds / OddsDenominator * 100;
			// now determine if the animal will move
			if (MovementOdds < 100) {
				// if the random number is greater than the movement odds, the animal does not move
                double RanValue = this.Background.RandomNum.RealValue(0, 100);
				if (RanValue > MovementOdds) return false;
			}
			*/

            // if we get here, we are going to move!!
            // generate a random number
            double MoveRandom = this.Background.RandomNum.RealValue(0, 100);

			// loop until movement distance is found
            // EER: This loops, interating i, until the sum of the movement probabilities, defined by array MovementDistribution
            //  are greater than the random number, to randomly define the probability of moving i cells
            //  So, the final i defines the number of cells to move
            //  I think the limit of i should be set at the max allowable cells to move
            //  Dave had this at 26, but I changed it to 49 to align with the ORM ArcGIS interface
			double sum = 0;
			int i;
			for (i = 0; i < 49; i++) {
				sum += MovementDistribution[i] * 100;
                //System.Diagnostics.Debug.WriteLine("cFox.cs: Move(): MovementDistribution[" + i + "] = " + MovementDistribution[i]);
                //System.Diagnostics.Debug.WriteLine("cFox.cs: Move(): sum = " + sum);
                //System.Diagnostics.Debug.WriteLine("cFox.cs: Move(): MoveRandom = " + MoveRandom);
                if (sum >= MoveRandom) break;
			}
            //System.Diagnostics.Debug.WriteLine("cFox.cs: Move(): HERE 5 Move this many cells = " + i);           
            // at this point, i is the dispersal distance
            // call the calculate path of the MasterCell object belonging to the
            // background to find a path through the cells.  Remember that the path 
            // returned may not be the distance requested because it may stop at
            // a supercell boundary

            //EER:
            string lastCell;
            lastCell = null;
            if (i > 0) {                
                // calculate a random direction bias
                //NOTE: Generating integer value now in range of 0 to 5.  For this call we WANT the range 0 to 5 since the values
                //      0, 1, 2, 3, 4, 5 are all equally probable.  D. Ball February 8, 2010
                enumNeighbourPosition DirectionBias = (enumNeighbourPosition)this.Background.RandomNum.IntValue(0, 5);                
                cCellList Path = this.Background.Cells.CalculatePath(this.CurrentCell.ID, i, DirectionBias);                
                // move this animal to the last cell in this path
                if (Path.Count > 0)	this.MoveAnimal(Path[Path.Count - 1]);              
                
            }           
            
            // set the HasMoved flag to true
            HasMoved = true;            
            return true;            
		}

		/// <summary>
		///		Define the mortality function for the fox.  This function is called once
		///		a week.
		/// </summary>
		/// <param name="CurrentWeek">
		///		The current week in the model.  An ArgumentOutOfRangeException exception is
		///		raised if CurrentWeek is not in the range of 1-52.
		/// </param>
		/// <returns>True if the animal should die.</returns>
		protected override bool Mortality(int CurrentWeek) 
		{
            double ProbMort;
            double annualMortFemale;
            double annualMortMale;
            double weekMort;
            double alpha;
            double beta;
            double po = 0;

            if (CurrentWeek < 1 || CurrentWeek > 52) ThrowWeekException("CurrentWeek");
            //System.Diagnostics.Debug.WriteLine("cFox.cs: Mortality(): CurrentWeek = " + CurrentWeek);

            // if the animal is already dead, don't bother
            if (!this.IsAlive) return true;

            // get animal's age in years
            int AgeInYears = Convert.ToInt32(Math.Floor((double)this.Age / 52));

            // now handle regular chance of death
            // calculate a combined mortality adjustment, including the provided adjustment to the adjustment
            // EER: In ARM the mortality calculation does not need adjustment defined by the user
            // EER:     though, if the user thinks there is an additive cause of mortality that is equal each week
            // EER:     then, the user can add this via the mortality adjustment
            // double MortAdjust = this.CurrentCell.Animals.Count / this.CurrentCell.K + (this.Background as cFoxBackground).FoxMortalityAdjustment;  

            //YM: the mortality was adjusted with only the count of juveniles and adults
            double MortAdjust = (this.CurrentCell.Animals.GetCountByAgeClass(enumAgeClass.Juvenile) + this.CurrentCell.Animals.GetCountByAgeClass(enumAgeClass.Adult)) / this.CurrentCell.K;

            // calculate mortality  (modified by EER)                      
            double RanNum = this.Background.RandomNum.RealValue(0, 1);

            // calculate the mean of mothers mortality 
            double propSAge1 = (1 - (this.Background as cFoxBackground).FoxFemaleMortality[0]) * (1 - (this.Background as cFoxBackground).FoxFemaleMortality[1]);
            double propSAge2 = propSAge1 * (1 - (this.Background as cFoxBackground).FoxFemaleMortality[2]);
            double propSAge3 = propSAge2 * (1 - (this.Background as cFoxBackground).FoxFemaleMortality[3]);
            double propSAge4 = propSAge3 * (1 - (this.Background as cFoxBackground).FoxFemaleMortality[4]);
            double propSAge5 = propSAge4 * (1 - (this.Background as cFoxBackground).FoxFemaleMortality[5]);
            double propSAge6 = propSAge5 * (1 - (this.Background as cFoxBackground).FoxFemaleMortality[6]);
            double propSAge7 = propSAge6 * (1 - (this.Background as cFoxBackground).FoxFemaleMortality[7]);

            double propMAge1 = (1 - (this.Background as cFoxBackground).FoxFemaleMortality[0]) * (this.Background as cFoxBackground).FoxFemaleMortality[1];
            double propMAge2 = propSAge1 * (this.Background as cFoxBackground).FoxFemaleMortality[2];
            double propMAge3 = propSAge2 * (this.Background as cFoxBackground).FoxFemaleMortality[3];
            double propMAge4 = propSAge3 * (this.Background as cFoxBackground).FoxFemaleMortality[4];
            double propMAge5 = propSAge4 * (this.Background as cFoxBackground).FoxFemaleMortality[5];
            double propMAge6 = propSAge5 * (this.Background as cFoxBackground).FoxFemaleMortality[6];
            double propMAge7 = propSAge6 * (this.Background as cFoxBackground).FoxFemaleMortality[7];


            double MeanMort = (propMAge1 * (this.Background as cFoxBackground).FoxFemaleMortality[1] +
                            propMAge2 * (this.Background as cFoxBackground).FoxFemaleMortality[2] +
                            propMAge3 * (this.Background as cFoxBackground).FoxFemaleMortality[3] +
                            propMAge4 * (this.Background as cFoxBackground).FoxFemaleMortality[4] +
                            propMAge5 * (this.Background as cFoxBackground).FoxFemaleMortality[5] +
                            propMAge6 * (this.Background as cFoxBackground).FoxFemaleMortality[6] +
                            propMAge7 * (this.Background as cFoxBackground).FoxFemaleMortality[7]) /
                            (propMAge1 + propMAge2 + propMAge3 + propMAge4 + propMAge5 + propMAge6 + propMAge7);


            bool UserLogisticBeta = (this.Background as cFoxBackground).LogisticBeta;
            bool UserLogisticAlpha = (this.Background as cFoxBackground).LogisticAlpha;

            if (UserLogisticBeta) beta = (this.Background as cFoxBackground).Beta;
            else beta = 1.5;

            if (this.Gender == enumGender.female)
            {
                annualMortFemale = (this.Background as cFoxBackground).FoxFemaleMortality[AgeInYears];

                if (this.AgeClass == enumAgeClass.YoungOfTheYear)
                {
                    ProbMort = 1 - Math.Exp(-(annualMortFemale - MeanMort) / 52);
                    //ProbMort = 1 - Math.Exp(-(annualMortFemale) / 52);

                    return (RanNum < ProbMort);
                }
                else
                {
                    if (AgeInYears == 0)
                    {
                        //po = 1 - Math.Exp(-(annualMortFemale - MeanMort));
                        po = 1 - Math.Exp(-(annualMortFemale));

                        if (UserLogisticAlpha) alpha = (this.Background as cFoxBackground).Alpha;
                        else alpha = beta * 4 / po;
                        ProbMort = po / (1 + Math.Exp((-1) * (MortAdjust - beta) * alpha));

                        return (RanNum < ProbMort);
                    }
                    else
                    {
                        po = 1 - Math.Exp(-annualMortFemale);

                        if (UserLogisticAlpha) alpha = (this.Background as cFoxBackground).Alpha;
                        else alpha = beta * 4 / po;

                        ProbMort = po / (1 + Math.Exp((-1) * (MortAdjust - beta) * alpha));
                        return (RanNum < ProbMort);
                    }
                }
            }
            else
            {
                annualMortMale = (this.Background as cFoxBackground).FoxMaleMortality[AgeInYears];
                //double NewMortRate = annualMortMale - MeanMort;

                if (this.AgeClass == enumAgeClass.YoungOfTheYear)
                {
                    ProbMort = 1 - Math.Exp(-(annualMortMale - MeanMort) / 52);
                    //ProbMort = 1 - Math.Exp(-(annualMortMale) / 52);

                    return (RanNum < ProbMort);
                }
                else
                {
                    if (AgeInYears == 0)
                    {
                        //po = 1 - Math.Exp(-(annualMortMale - MeanMort));
                        po = 1 - Math.Exp(-(annualMortMale));
                        if (UserLogisticAlpha) alpha = (this.Background as cFoxBackground).Alpha;
                        else alpha = beta * 4 / po;
                        ProbMort = po / (1 + Math.Exp((-1) * (MortAdjust - beta) * alpha));

                        return (RanNum < ProbMort);
                    }
                    else
                    {
                        po = 1 - Math.Exp(-annualMortMale);
                        if (UserLogisticAlpha) alpha = (this.Background as cFoxBackground).Alpha;
                        else alpha = beta * 4 / po;

                        ProbMort = po / (1 + Math.Exp((-1) * (MortAdjust - beta) * alpha));
                        return (RanNum < ProbMort);
                    }
                }

            }
        }


        /// <summary>
        ///		Determine a contact probability for the animal either in its own cell or to
        ///		all neighbouring cells given an overall contact rate and the current week.
        /// </summary>
        /// <param name="CurrentWeek">The current week of the year.</param>
        /// <param name="Gender"> The Gender of the animal making contact.</param>
        /// <param name="InCell">
        ///		True if the contact rate is for within the cell, False if it is for neighbouring cells.
        /// </param>
        /// <param name="OverAllContactRate">The overall odds of contact with a single animal.</param>
        /// <returns>The contact rate either in the cell or with all neighbours.</returns>
        protected override double getCellContactRate(int CurrentWeek, enumGender Gender, bool InCell, double OverAllContactRate) 
		{
			// make sure passed week is in range
			if (CurrentWeek < 1 || CurrentWeek > 52) ThrowWeekException("CurrentWeek");
			// now calculate required probability
			// first get male or female probability for outside the cell
			double ProbValue;
			if (Gender == enumGender.male)
                ProbValue = (this.Background as cFoxBackground).FoxMaleHomeRange[CurrentWeek - 1] / 100.0;
			else
                ProbValue = (this.Background as cFoxBackground).FoxFemaleHomeRange[CurrentWeek - 1] / 100.0;
			// is this in cell or outside cell
			if (InCell) // we want 1 - the actual value
				ProbValue = 1 - ProbValue;
			// now multiply by the overall rate
			return OverAllContactRate * ProbValue;
		}

		/// <summary>
		///		Get the maximum age that this animal can live to
		/// </summary>
		/// <returns>The maximum age in weeks</returns>
		protected override int getMaxAge() { return 415; }  // 8 Years

        /// <summary>
		///		Get the maximum age in weeks that this animal can live to
        ///		EER: Added so that animals do not live beyond yearly mortality rates of 100%
        ///		EER:    and so that max age can differ among sex
        ///		EER: If user does not define annual mortality rates of 100%, 
        ///		EER:    the max possible age is 8 years
		/// </summary>
		/// <returns>The maximum age in weeks</returns>
		protected override int getMaxAgeFemale()
        {            
            for (int i = 1; i < 8; i++)
            {
                double deathYear = (this.Background as cFoxBackground).FoxFemaleMortality[i];
                //if (deathYear == 100) {                
                if (deathYear == 1) //YM: the mortality rate is between 0 and 1 
                {
                    maxAgeFemale = i * 52;
                    break;
                }
                else
                {
                    maxAgeFemale = 415;
                }
            }
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cFox.cs: getMaxAgeFemale(): maxAgeFemale = " + maxAgeFemale);
            return maxAgeFemale;
        }  

        /// <summary>
		///		Get the maximum age in weeks that this animal can live to
        ///		EER: Added so that animals do not live beyond yearly mortality rates of 100%
        ///		EER:    and so that max age can differ among sex
        ///		EER: If user does not define annual mortality rates of 100%, 
        ///		EER:    the max possible age is 8 years
		/// </summary>
		/// <returns>The maximum age in weeks</returns>
		protected override int getMaxAgeMale()        
        {
            for (int i = 1; i < 8; i++)
            {
                double deathYear = (this.Background as cFoxBackground).FoxMaleMortality[i];
                //if (deathYear == 100) {                
                if (deathYear == 1) //YM: the mortality rate is between 0 and 1 
                {
                    maxAgeMale = i * 52;
                    break;
                }
                else
                {
                    maxAgeMale = 415;
                }
            }
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cFox.cs: getMaxAgeMale(): maxAgeMale = " + maxAgeMale);
            return maxAgeMale;
        }

        /// <summary>
        ///     Get a description of the type of the animal
        /// </summary>
        /// <returns>A description of the type of the animal</returns>
        protected override string getAnimalDescription()
        {
            return "Fox";
        }


		// ************* private members **************************************************
		// a flag, reset every year that indicates whether or not the animal has moved
		private bool HasMoved;
		// throw an exception, indicating a week value is out of range
		private void ThrowWeekException(string ParamName)
		{
			throw new ArgumentOutOfRangeException(ParamName, ParamName + " must be in the range of 1 to 52");
		}
        // EER: Used to define the max ages of males/females given user-defined annual mortality rates
        private int maxAgeFemale;
        private int maxAgeMale;
    }
}