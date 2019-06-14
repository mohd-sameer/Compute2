using System;
using System.Collections.Generic;
using System.Text;
using RabiesRuntime;
using Random;
using System.Data;

namespace RabiesRuntime
{
    /// <summary>
    /// Implemetation of the rabies disease for batch processes
    /// </summary>
    public class cBatchRabies : cRabies
    {
        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="SpreadProbability">The probability of spread</param>
        /// <param name="Rnd">The random number generator used to calculate spread probabilities</param>
        /// <param name="ChanceOfDeath">The % chance that an infection from this disease is fatal</param>
        /// <param name="RecoveredBecomeImmune">A flag indicating that recovered animal become immune</param>
        /// <param name="RecoveredNotInfectious">A flag the indicates that aninmals that recover from this disease will not become infectious</param>
        /// <param name="TrialSettings">The trial settings dataset</param>        
        public cBatchRabies(double SpreadProbability, cUniformRandom Rnd, int ChanceOfDeath,  bool RecoveredBecomeImmune, bool RecoveredNotInfectious, DataSet TrialSettings)
            : base(SpreadProbability, Rnd, ChanceOfDeath, RecoveredBecomeImmune, RecoveredNotInfectious)
        {
            // get datatables containing Incubation and Infectious Periods
            DataTable dtIncubation = TrialSettings.Tables["DiseaseInformationIncubationPeriodNormalized"];
            DataTable dtInfectious = TrialSettings.Tables["DiseaseInformationInfectiousPeriodNormalized"];
            // read Incubation and Infectious period arrays
            for (int i = 0; i <= 49; i++)
            {
                DataRow dr = dtIncubation.Rows[i];
                Incubation[i] = Convert.ToDouble(dr["Value"]);
                dr = dtInfectious.Rows[i];
                Infection[i] = Convert.ToDouble(dr["Value"]);
            }
        }
        /// <summary>
        /// EER: a second contructor added to accomodate ARM Excel template settings file
        /// </summary>
        /// <param name="SpreadProbability">The probability of spread</param>
        /// <param name="Rnd">The random number generator used to calculate spread probabilities</param>
        /// <param name="ChanceOfDeath">The % chance that an infection from this disease is fatal</param>
        /// <param name="RecoveredBecomeImmune">A flag indicating that recovered animal become immune</param>
        /// <param name="RecoveredNotInfectious">A flag the indicates that aninmals that recover from this disease will not become infectious</param>
        /// <param name="TrialSettings">The trial settings dataset</param>
        /// <param name="runningARM">A boolean flag to indicate that settings data come from ARM Excel template</param>        
        public cBatchRabies(double SpreadProbability, cUniformRandom Rnd, int ChanceOfDeath, bool RecoveredBecomeImmune, bool RecoveredNotInfectious, DataSet TrialSettings, bool runningARM)
            : base(SpreadProbability, Rnd, ChanceOfDeath, RecoveredBecomeImmune, RecoveredNotInfectious)
        {            
            //System.Diagnostics.Debug.WriteLine("cBatchRabies: cBatchRabies");
            
            // get datatable containing Incubation and Infectious Periods
            DataTable dtEpi = TrialSettings.Tables[3];

            /*double testIncub = Convert.ToDouble(dtEpi.Rows[8][6].ToString());
            double testInf = Convert.ToDouble(dtEpi.Rows[8][7].ToString());
            System.Diagnostics.Debug.WriteLine("    testIncub = " + testIncub);
            System.Diagnostics.Debug.WriteLine("    testInf = " + testInf);*/

            // read Incubation and Infectious period arrays
            for (int i = 0; i <= 49; i++)
            {
                Incubation[i] = Convert.ToDouble(dtEpi.Rows[i + 8][6]);               
                Infection[i] = Convert.ToDouble(dtEpi.Rows[i + 8][7]);
                //System.Diagnostics.Debug.WriteLine("    cBatchRabies: cBatchRabies: Incubation[i] = " + Incubation[i] + "; Infection[i] = " + Infection[i]);
            }
        }
    }
}
