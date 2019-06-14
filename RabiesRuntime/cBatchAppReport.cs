using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace RabiesRuntime
{
    /// <summary>
    /// A class for reporting program status to the user and logging program 
    //  status to a log file
    /// </summary>
    public class cBatchAppReport : cAppReport
    {
        private string mvarAppName;
        private bool mvarShowConsole;

        #region Constructor

        /// <summary>
        /// Constructor.  Must pass the name of the application
        /// </summary>
        /// <param name="AppName">The name of the application</param>
        /// <param name="ShowConsole">A boolean value that indicates whether or not messages should be sent to the console</param>
        /// <param name="IsUnix">A boolean value indicating whether or not the file paths should be Unix paths</param>
        public cBatchAppReport(string AppName, bool ShowConsole, bool IsUnix)
            : base(AppName, Assembly.GetExecutingAssembly().Location, IsUnix)
        {
            mvarAppName = AppName;
            mvarShowConsole = ShowConsole;
        }

        /// <summary>
        /// Constructor.  Must pass the name of the application
        /// </summary>
        /// <param name="AppName">The name of the application</param>
        /// <param name="ReportPath">The path to write the report (log) file to</param>
        /// <param name="ShowConsole">A boolean value that indicates whether or not messages should be sent to the console</param>
        /// <param name="IsUnix">A boolean value indicating whether or not the file paths should be Unix paths</param>
        public cBatchAppReport(string AppName, string ReportPath, bool ShowConsole, bool IsUnix)
            : base(AppName, ReportPath, IsUnix, false)
        {
            mvarAppName = AppName;
            mvarShowConsole = ShowConsole;
        }
		
		
        #endregion

        #region Protected Methods

        // **************************** Protected Methods ****************

        /// <summary>
        /// Show a message in a message box
        /// </summary>
        /// <param name="Message">The message to show</param>
        protected override void ShowMessage(string Message)
        {
            MessageBox.Show(Message, mvarAppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Override the writing of a log entry to also display entry on the console
        /// </summary>
        /// <param name="EntryString">The entry string to write and show</param>
        public override void WriteLogEntry(string EntryString)
        {
            base.WriteLogEntry(EntryString);
            if (!mvarLogOnly && mvarShowConsole) Console.WriteLine(EntryString);
        }

        #endregion
    }
}
