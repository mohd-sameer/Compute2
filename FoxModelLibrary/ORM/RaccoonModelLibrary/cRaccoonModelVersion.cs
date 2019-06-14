using System;
using System.Collections.Generic;
using System.Text;

namespace Raccoon_Model_Library
{
    /// <summary>
    /// Class for containing the major model version and the accepted settings file version
    /// </summary>
    public class cRaccoonModelVersion
    {
        /// <summary>
        /// Get or set the program version
        /// </summary>
        public static string ProgramVersion
        {
            get
            {
                return "1.7";
            }
        }

        /// <summary>
        /// Get or set the settings file version
        /// </summary>
        public static string SettingsFileVersion
        {
            get
            {
                return "1.6";
            }
        }

        /// <summary>
        /// Prevent construction of instances
        /// </summary>
        private cRaccoonModelVersion() { }
    }
}
