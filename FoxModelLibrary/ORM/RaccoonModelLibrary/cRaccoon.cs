using System;
using Rabies_Model_Core;
using Random;
using System.IO;
using System.Text;

namespace Raccoon
{
	/// <summary>
	///		A class that represents a single raccoon.  This class is derived from cAnimal.
	///		Mortality, birth rate and movement are all defined in ths object.  Before using
	///		this class, the user is responsible for setting the values of several static
	///		variables, in order to define specific behaviour.
	/// </summary>
	public class cRaccoon : cAnimal
	{
		// ********************* Constructors *******************************************
		/// <summary>
		///		Construct a raccoon by specifying its ID, Starting Cell and Background.
		///		This constructor is used to create the initial raccoons in the background.
		///		All	raccoons created by this constructor come into existance at year = 0,
		///		week = 1.  Although the raccoons created here will start out as juveniles,
		///		they will have no parents.
		/// </summary>
		/// <param name="ID">
		///		The ID of the raccoon.  An ArgumentException exception is raised if the ID has
		///		zero length.
		///	</param>
		/// <param name="CellID">
		///		The ID of the cell initially occupied by this raccoon.  An ArgumentException
		///		exception is raised if the cell is not part of the background object to
		///		which this raccoon belongs.
		///	</param>
		/// <param name="Background">
		///		The background object in which the raccoon lives.  An ArgumentNullException
		///		exception is raised if Background is null.
		/// </param>
		/// <param name="Gender">
		///		The gender of the raccoon.
		/// </param>
		public cRaccoon(string ID, string CellID, cBackground Background, 
						enumGender Gender) : base(ID, CellID, Background, Gender)
		{
		}

		/// <summary>
		///		Construct a raccoon by specifying its ID and Parent.  This constructor is
		///		used when giving birth to new raccoons.  The background, and initial cell 
		///		are obtained from the parent raccoon.  The year and week of birth are 
		///		obtained from the background that the parent belongs to.
		/// </summary>
		/// <param name="ID">
		///		The ID of the raccoon.  An ArgumentException exception is raised if the ID has
		///		zero length.
		///	</param>
		/// <param name="Parent">
		///		The parent of the raccoon.  An ArgumentNullException exception is raised if
		///		Parent is null.
		///	</param>
		/// <param name="Gender">The gender of the raccoon.</param>
		public cRaccoon(string ID, cRaccoon Parent, enumGender Gender) :
							base(ID, Parent, Gender)
		{
		}

		/// <summary>
		///		Construct a raccoon from a cAnimalAttributes object.  This constructor is
		///		used when loading raccoon data from a datasource.
		/// </summary>
		/// <param name="Attributes">
		///		The cAnimalAttributes object containing the attributes for the raccoon.  An
		///		ArgumentNullException exception is raised if Attributes is null.
		/// </param>
		/// <param name="Background">
		///		The background object in which the raccoon lives.  An ArgumentNullException
		///		exception is raised if Background is null.
		/// </param>
		public cRaccoon(cAnimalAttributes Attributes, cBackground Background) :
							base(Attributes, Background)
		{
		}

        //private void InitStaticValues()
        //{
        //    cRaccoonBackground rb = this.Background as cRaccoonBackground;
        //    this.RaccoonMaleMortality = rb.RaccoonMaleMortality;
        //    this.RaccoonFemaleMortality = rb.RaccoonFemaleMortality;
        //    this.RaccoonMortalityAdjustment = rb.RaccoonMortalityAdjustment;
        //    this.RaccoonJuvAdultMaleMovementDistribution = rb.RaccoonJuvAdultMaleMovementDistribution;
        //    this.RaccoonJuvAdultFemaleMovementDistribution = rb.RaccoonJuvAdultFemaleMovementDistribution;
        //    this.RaccoonYoungMaleMovementDistribution = rb.RaccoonYoungMaleMovementDistribution;
        //    this.RaccoonYoungFemaleMovementDistribution = rb.RaccoonYoungFemaleMovementDistribution;
        //    this.RaccoonJuvAdultMaleWeeklyMovement = rb.RaccoonJuvAdultMaleWeeklyMovement;
        //    this.RaccoonJuvAdultFemaleWeeklyMovement  = rb.RaccoonJuvAdultFemaleWeeklyMovement;
        //    this.RaccoonYoungMaleWeeklyMovement = rb.RaccoonYoungMaleWeeklyMovement;
        //    this.RaccoonYoungFemaleWeeklyMovement = rb.RaccoonYoungFemaleWeeklyMovement;
        //    this.RaccoonLitterSize = rb.RaccoonLitterSize;
        //    this.RaccoonMatingWeek = rb.RaccoonMatingWeek;
        //    this.RaccoonMaleHomeRange = rb.RaccoonMaleHomeRange;
        //    this.RaccoonFemaleHomeRange = rb.RaccoonFemaleHomeRange;
        //    this.RaccoonReproductionWeek = rb.RaccoonReproductionWeek;
        //    this.RaccoonGenderRatio = rb.RaccoonGenderRatio;
        //    this.RaccoonJuvenileBirthOdds = rb.RaccoonJuvenileBirthOdds;
        //    this.RaccoonAdultBirthOdds = rb.RaccoonAdultBirthOdds;
        //    this.RaccoonAdultAge = rb.RaccoonAdultAge;
        //    this.RaccoonIndependenceAge = rb.RaccoonIndependenceAge;
        //}

		// ********************* Properties **********************************************
		/// <summary>
		///		The age class of the raccoon. Categories are as follows:  juvenile - 
		///		dependent on its parent, yearling - independent animal up to a year old, 
		///		adult - independent animal over a year old.
		/// </summary>
		public override enumAgeClass AgeClass 
		{
			get 
			{
				//if (!this.IsIndependent) 
				if (this.Age < 34)
					return enumAgeClass.YoungOfTheYear;
                else if (this.Age < (this.Background as cRaccoonBackground).RaccoonAdultAge)
					return enumAgeClass.Juvenile;
				else
					return enumAgeClass.Adult;
			}
		}

		// ********************* Protected  Methods **************************************
		/// <summary>
		///		A function called each week for each female raccoon.  This function is used
		///		to detirmine whether or not the female should mate in the passed week.
		/// </summary>
		/// <param name="Week">
		///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
		///		is not in the range of 1-52.
		///	</param>
		/// <returns>True if the raccoon should mate.</returns>
		protected override bool Mate(int Week)
		{
			// make sure week is a valid value
			if (Week < 1 || Week > 52) ThrowWeekException("Week");
			// if this is the mating week then return true
            return (Week == (this.Background as cRaccoonBackground).RaccoonMatingWeek);
		}

		/// <summary>
		///		A function called each week for each female raccoon.  This function is used
		///		to detirmine whether or not the female should give birth in the passed week.
		/// </summary>
		/// <param name="Week">
		///		The current week.  An ArgumentOutOfRangeException exception is raised if Week
		///		is not in the range of 1-52.
		///	</param>
		/// <returns>True if the raccoon should give birth.</returns>
		protected override bool GiveBirth(int Week) 
		{
			// make sure week is a valid value
			if (Week < 1 || Week > 52) ThrowWeekException("Week");
			// give birth
            if (Week == (this.Background as cRaccoonBackground).RaccoonReproductionWeek)
            {
				if (this.AgeClass == enumAgeClass.Juvenile)
                    return (this.Background.RandomNum.IntValue(1, 100) <= (this.Background as cRaccoonBackground).RaccoonJuvenileBirthOdds);
				else if (this.AgeClass == enumAgeClass.Adult)
                    return (this.Background.RandomNum.IntValue(1, 100) <= (this.Background as cRaccoonBackground).RaccoonAdultBirthOdds);
				else
					return false;
			}
			return false;
		}

		/// <summary>
		///		Detirmine the number of babies to give birth to.
		/// </summary>
		/// <returns>The number of babies to give birth to.</returns>
		protected override int GetNBabies() 
		{
			double RanValue = this.Background.RandomNum.RealValue(0, 100);
			double OddsSum = 0;
            double[] LitterSize = (this.Background as cRaccoonBackground).RaccoonLitterSize;
			// loop, adding up litter size values until the value of the random number is
			// equaled or exceeded
			for (int i = 0; i < 8; i++) {
                OddsSum += LitterSize[i];
				if (OddsSum >= RanValue) return i + 1;
			}
			// just in case
			return 8;
		}

		/// <summary>
		///		Create a new baby raccoon.
		/// </summary>
		/// <param name="ID">
		///		The ID to be assigned to the baby animal.  An ArgumentException exception is
		///		raised if ID has zero length.
		///	</param>
		/// <returns>The new raccoon returned as a cAnimal.</returns>
		protected override cAnimal CreateBaby(string ID) 
		{
			// make sure ID is not zero length
			if (ID.Length == 0)
				throw new ArgumentException("ID cannot have zero length.", "ID");
			// create the baby
			enumGender TheGender;
            if (this.Background.RandomNum.IntValue(1, 100) <= (this.Background as cRaccoonBackground).RaccoonGenderRatio) 
				TheGender = enumGender.male;
			else
				TheGender = enumGender.female;
			return new cRaccoon(ID, this, TheGender); 
		}

		/// <summary>
		///		Detirmine whether or not a raccoon should disperse (ie. leave mum and become
		///		independent.  If the raccoon does disperse, this function should define the
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
            return (this.Age == (this.Background as cRaccoonBackground).RaccoonIndependenceAge);
		}

		/// <summary>
		///		Define movement behaviour for independent (yearling and adult) raccoons.
		/// </summary>
		/// <param name="CurrentWeek">
		///		The current week of the year.  An ArgumentOutOfRangeException exception is
		///		raised if CurrentWeek is not in the range of 1-52.
		///	</param>
		/// <returns>True if the raccoon changes cell.</returns>
		protected override bool Move(int CurrentWeek)
		{
			if (CurrentWeek < 1 || CurrentWeek > 52) ThrowWeekException("CurrentWeek");
			// see if this is week 0, if it is, reset has moved
			if (CurrentWeek == 1) HasMoved = false;

            //if (this.ID == "3683630" && CurrentWeek == 12 && this.Background.Years.CurrentYearNum == 81)
            //{
            //    int Stopper = 1;
            //}


			// next, if this animal has moved, leave now
			if (HasMoved) return false;

			// now load the appropriate movement arrays depending upon what type of animal
			// you are
			bool[] MovementArray;
			double[] MovementDistribution;
			if (this.AgeClass == enumAgeClass.YoungOfTheYear) {
				if (this.Gender == enumGender.female) {
                    MovementArray = (this.Background as cRaccoonBackground).RaccoonYoungFemaleWeeklyMovement;
                    MovementDistribution = (this.Background as cRaccoonBackground).RaccoonYoungFemaleMovementDistribution;
				}
				else {
                    MovementArray = (this.Background as cRaccoonBackground).RaccoonYoungMaleWeeklyMovement;
                    MovementDistribution = (this.Background as cRaccoonBackground).RaccoonYoungMaleMovementDistribution;
				}
			}
			else {
				if (this.Gender == enumGender.female) {
                    MovementArray = (this.Background as cRaccoonBackground).RaccoonJuvAdultFemaleWeeklyMovement;
                    MovementDistribution = (this.Background as cRaccoonBackground).RaccoonJuvAdultFemaleMovementDistribution;
				}
				else {
                    MovementArray = (this.Background as cRaccoonBackground).RaccoonJuvAdultMaleWeeklyMovement;
                    MovementDistribution = (this.Background as cRaccoonBackground).RaccoonJuvAdultMaleMovementDistribution;
				}
			}

			// can the animal even move this week?  If not, leave now!!
			if (!MovementArray[CurrentWeek - 1]) return false;

			// if it can, work out the current odds of moving
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
			MovementOdds = MovementOdds / OddsDenominator * 100;
			// now detirmine if the animal will move
			if (MovementOdds < 100) {
				// if the random number is greater than the movement odds, the animal does not move
                double RanValue = this.Background.RandomNum.RealValue(0, 100);
				if (RanValue > MovementOdds) return false;
			}
			
			// if we get here, we are going to move!!
			// generate a random number
			double MoveRandom = this.Background.RandomNum.RealValue(0, 100);

			// loop until movement distance is found
			double sum = 0;
			int i;
			for (i = 0; i < 26; i++) {
				sum += MovementDistribution[i];
				if (sum >= MoveRandom) break;
			}

			// at this point, i is the dispersal distance
			// call the calculate path of the MasterCell object belonging to the
			// background to find a path through the cells.  Remember that the path 
			// returned may not be the distance requested because it may stop at
			// a supercell boundary
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
		///		Define the mortality function for the raccoon.  This function is called once
		///		a week.
		/// </summary>
		/// <param name="CurrentWeek">
		///		The current week in the model.  An ArgumentOutOfRangeException exception is
		///		raised if CurrentWeek is not in the range of 1-52.
		/// </param>
		/// <returns>True if the animal should die.</returns>
		protected override bool Mortality(int CurrentWeek) 
		{
			if (CurrentWeek < 1 || CurrentWeek > 52) ThrowWeekException("CurrentWeek");
			// if the animal is already dead, don't bother
			if (!this.IsAlive) return true;
			// get animal's age in years
			int AgeInYears = Convert.ToInt32(Math.Floor((double)this.Age / 52));
			// now handle regular chance of death
			// calculate a combined mortality adjustment, including the provided adjustment to the adjustment
            double MortAdjust = this.CurrentCell.Animals.Count / this.CurrentCell.K + (this.Background as cRaccoonBackground).RaccoonMortalityAdjustment;
			// calculate mortality
            double RanNum = this.Background.RandomNum.RealValue(0, 100);
			if (this.Gender == enumGender.female) 
			{
                return (RanNum < (this.Background as cRaccoonBackground).RaccoonFemaleMortality[AgeInYears] * MortAdjust);

			}
			else 
            {
                return (RanNum < (this.Background as cRaccoonBackground).RaccoonFemaleMortality[AgeInYears] * MortAdjust);
            }
		}

		/// <summary>
		///		Detirmine a contact probability for the animal either in its own cell or to
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
                ProbValue = (this.Background as cRaccoonBackground).RaccoonMaleHomeRange[CurrentWeek - 1] / 100.0;
			else
                ProbValue = (this.Background as cRaccoonBackground).RaccoonFemaleHomeRange[CurrentWeek - 1] / 100.0;
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


		// ************* private members **************************************************
		// a flag, reset every year that indicates whether or not the animal has moved
		private bool HasMoved;
		// throw an exception, indicating a week value is out of range
		private void ThrowWeekException(string ParamName)
		{
			throw new ArgumentOutOfRangeException(ParamName, ParamName + " must be in the range of 1 to 52");
		}
	}
}