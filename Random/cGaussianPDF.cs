using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Random
{
    //EER: A class added by EER to calculate probability function distribution values
    //      assuming a Gaussian distribuion
    public class cGaussianPDF
    {
        #region Constructors

        // ************************* Constructors *************************************
        /// <summary>
        ///	Default constructor.
        /// </summary>        
        public cGaussianPDF()
        {    
        }

        /*/// <summary>
        ///	Constructor with arguements.
        /// </summary>
        /// <param name="x">Value from which to define the local pdf value.</param>
        /// <param name="mean">Mean of the overal pdf.</param>
        /// <param name="variance">Variance of the overal pdf.</param>
        public cGaussianPDF(int x, Single mean, Single variance)
        {
            int xValue = x;
            Single meanValue = mean;
            Single varValue = variance;
                       
        }*/
        #endregion

        #region Public Methods

        /// <summary>
        ///	EER:
        /// Calculate the local pdf value for x given the mean and variance   
        /// <param name="x">Value from which to define the local pdf value.</param>
        /// <param name="mean">Mean of the overal pdf.</param>
        /// <param name="variance">Variance of the overal pdf.</param>
        /// </summary>
        public double xGaussianPDF(int x, Single mean, Single variance)
        {

            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cGaussianPDF.cs: xGaussianPDF()");

            double valPDF = 1 / Math.Sqrt((variance*2*Math.PI))*Math.Exp((-1)*Math.Pow(x-mean,2)/(2*variance));

            return valPDF;
        }
        #endregion
    }
}