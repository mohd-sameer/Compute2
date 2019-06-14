using System;
using System.Collections.Generic;
using System.Text;
using Rabies_Model_Core;
using Random;

namespace RabiesRuntime
{
    /// <summary>
    /// Class representing the Rabies disease
    /// </summary>
    public abstract class cRabies : cDisease
    {
        #region Constructors

        // ********************* Constructors *******************************************
        // initialize the base class

        /// <summary>
        /// Constructor - Initial Rabies
        /// </summary>
        /// <param name="SpreadProbability">The spread probability</param>
        /// <param name="Rnd">The random number generator used to calculated spread</param>
        /// <param name="ChanceOfDeath">The % chance that an infection from this disease is fatal</param>
        /// <param name="RecoveredBecomeImmune">A flag indicating that recovered animal become immune</param>
        /// <param name="RecoveredNotInfectious">A flag the indicates that aninmals that recover from this disease will not become infectious</param>
        public cRabies(double SpreadProbability, cUniformRandom Rnd, int ChanceOfDeath, bool RecoveredBecomeImmune, bool RecoveredNotInfectious)
            : base("Rabies", 1, 1, 1, 1, SpreadProbability, ChanceOfDeath, true, RecoveredNotInfectious, Rnd)
        {
        }

        #endregion

        #region Public Methods

        // ******************** Methods *************************************************

        /// <summary>
        /// Calculate an infectious period for a new case of rabies
        /// </summary>
        /// <returns>An infectious period for a specific case of rabies</returns>
        public override int GetInfectiousPeriod()
        {
            //System.Diagnostics.Debug.WriteLine("cRabies.cs: GetInfectiousPeriod()");
            // get a random number between 0 and 100.
            double RandomNum = mvarRandom.RealValue(0, 100);
            // Add given probabilities until the total equals or exceeds the Random number.
            // the number of additions required to achieve this is the Infectious period in
            // weeks
            double Sum = 0;
            int i = 0;
            for (i = 0; i <= 49; i++) {

                Sum += Infection[i];
                                              
                //System.Diagnostics.Debug.WriteLine("    cRabies.cs: GetInfectiousPeriod() Infection[" + i + "] = " + Infection[i] + " Sum = " + Sum);
                
                if (Sum >= RandomNum) break;
            }
            //System.Diagnostics.Debug.WriteLine("    cRabies.cs: GetInfectiousPeriod() RandomNum = " + RandomNum);
            //System.Diagnostics.Debug.WriteLine("    cRabies.cs: GetInfectiousPeriod() Final infectious period = " + i + 1);
            return i + 1;
        }
        
        /// <summary>
        /// Calculate an incubation period for a new case of rabies
        /// </summary>
        /// <returns>An incubation period for a specifc case of rabies</returns>
        public override int GetIncubationPeriod()
        {
            //System.Diagnostics.Debug.WriteLine("cRabies.cs: GetIncubationPeriod()");
            // get a random number between 0 and 100.
            double RandomNum = mvarRandom.RealValue(0, 100);
            double Sum = 0;
            int i = 0;
            // Add given probabilities until the total equals or exceeds the Random number.
            // the number of additions required to achieve this is the Incubation period in
            // weeks.
            for (i = 0; i <= 49; i++) {

                Sum += Incubation[i]; 
                
                //System.Diagnostics.Debug.WriteLine("    cRabies.cs: GetIncubationPeriod() Incubation[" + i + "] = " + Incubation[i] + " Sum = " + Sum);
                
                if (Sum >= RandomNum) break;
            }
            // now, if the incubation period turns out to be 49 (50 weeks) then return the maximum
            // integer.  This will effectively give the animal immunity (essentially a life-long
            // incubation period)
            //System.Diagnostics.Debug.WriteLine("    cRabies.cs: GetIncubationPeriod() RandomNum = " + RandomNum);
            
            if (i < 49) {
                //System.Diagnostics.Debug.WriteLine("    cRabies.cs: GetIncubationPeriod() Final incubation period = " + i + 1);
                return i + 1;
            }
            else {
                //System.Diagnostics.Debug.WriteLine("    cRabies.cs: GetIncubationPeriod() Final incubation period = " + Int32.MaxValue);
                return Int32.MaxValue;
            }
        }

        #endregion

        #region Protected Members

        // ******************** Private Members *****************************************
        protected double[] Incubation = new double[51];
        protected double[] Infection = new double[51];

        #endregion
    }
}
