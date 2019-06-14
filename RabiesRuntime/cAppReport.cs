using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace RabiesRuntime
{
    // a class for reporting program status to the user and logging program 
    // status to a log file
    public abstract class cAppReport
    {
        /// <summary>
        /// Constructor.  Must pass the name of the application
        /// </summary>
        /// <param name="AppName">The name of the application</param>
        /// <param name="AppPath">The path to the application</param>
        /// <param name="IsUnix">A boolean value indicating whther or not the file paths should be Unix paths</param>
        public cAppReport(string AppName, string AppPath, bool IsUnix)
        {
            mAppName = AppName;
            mAppPath = AppPath;
			mIsUnix = IsUnix;
            if (!Directory.Exists(AppPath))
            {
                // assume last part of the passed path is a file name
				if (mIsUnix)
				{
	                int i = AppPath.LastIndexOf("/");
	                if (i > 0)
	                {
	                    AppPath = AppPath.Substring(0, i);
	                    if (Directory.Exists(AppPath)) mAppPath = AppPath;
	                }
				}
				else
				{
	                int i = AppPath.LastIndexOf(@"\");
	                if (i > 0)
	                {
	                    AppPath = AppPath.Substring(0, i);
	                    if (Directory.Exists(AppPath)) mAppPath = AppPath;
	                }
				}
            }
            WriteLog = false;
            ShowDialogs = true;
        }

		
		/// <summary>
        /// Constructor.  Must pass the name of the application
        /// </summary>
        /// <param name="AppName">The name of the application</param>
        /// <param name="LogPath">The path to the log file</param>
        /// <param name="IsUnix">A boolean value indicating whether or not the file paths should be Unix paths</param>
        /// <param name="IsAppPath">A boolean value that indicates whether or not the passed LogPath is the application path</param>
        public cAppReport(string AppName, string LogPath, bool IsUnix, bool IsAppPath)
        {
		    mAppName = AppName;
			mIsUnix = IsUnix;
			if (IsAppPath)
			{
		        mAppPath = LogPath;
		        if (!Directory.Exists(LogPath))
		        {
		            // assume last part of the passed path is a file name
					if (mIsUnix)
						{
			            int i = LogPath.LastIndexOf("/");
			            if (i > 0)
			            {
			                LogPath = LogPath.Substring(0, i);
	                    		if (Directory.Exists(LogPath)) mAppPath = LogPath;
		            		}
					}
					else
					{
			            int i = LogPath.LastIndexOf(@"\");
			            if (i > 0)
			            {
		                    LogPath = LogPath.Substring(0, i);
		                    if (Directory.Exists(LogPath)) mAppPath = LogPath;
		                }
					}
				}			
			}
			else
			{
				mAppPath = LogPath;
			}
	        WriteLog = false;
	        ShowDialogs = true;
			try 
			{
				if (!string.IsNullOrEmpty(mAppPath))
				{
					if (!Directory.Exists(mAppPath)) Directory.CreateDirectory(mAppPath);
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("Error occured attempting to create the log file path '{0}'.  The error was '{1}'", mAppPath, ex.Message), ex);
			}
		}
		
        #region Public Properties

        // **************************** Public Properties ****************
        /// <summary>
        /// Get or set a boolean indicating whether or not the class should write to a log file
        /// </summary>
        public bool WriteLog { get; set; }
        /// <summary>
        /// Get or set a boolean values indicating whether or not the class should display dialogs
        /// </summary>
        public bool ShowDialogs { get; set; }

        #endregion

        #region Public Methods

        // **************************** Public Methods *******************
        /// <summary>
        /// Create an entry indicating the start of a run
        /// </summary>
        /// <param name="RunName">The name of the run</param>
        public void IndicateRunStart(string RunName)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: IndicateRunStart()");

            WriteLogEntry(string.Format("{0}-> started...", RunName));
        }

        /// <summary>
        /// Create an entry indicating the completion of a run
        /// </summary>
        /// <param name="RunName">The name of the run</param>
        public void IndicateRunCompletion(string RunName, DateTime StartTime)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: IndicateRunCompletion()");

            // calculate run time
            int RunTime = (int)(DateTime.Now - StartTime).TotalSeconds;
            WriteLogEntry(string.Format("{0}-> completed. RunTime {1}min {2}sec", RunName, RunTime / 60, RunTime % 60));
        }

        /// <summary>
        /// Create an entry indicating that a run was aborted
        /// </summary>
        /// <param name="RunName">The name of the run</param>
        public void IndicateRunAbort(string RunName)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: IndicateRunAbort()");

            WriteLogEntry(string.Format("{0}-> *****aborted*****.", RunName));
            if (ShowDialogs)
            {
                ShowMessage(string.Format("Run *{0}* aborted by user.", RunName));
            }
        }

        /// <summary>
        /// Create an entry indicating that an error occured
        /// </summary>
        /// <param name="Message">The name of the run</param>
        /// <param name="ex">The exception that caused the error</param>
        public void IndicateError(string Message, Exception ex)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: IndicateError()");

            WriteLogEntry(string.Format("Error - {0}.", Message));
            if (ex != null) WriteLogEntry(string.Format("Stack Trace - {0}", ex.StackTrace));
            if (ShowDialogs)
            {
                ShowMessage(string.Format("Error indicated!!\n{0}", Message));
            }
        }

        /// <summary>
        /// Send a message to a dialog box
        /// </summary>
        /// <param name="Message">The message to send</param>
        public void SendMessage(string Message)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: SendMessage()");

            WriteLogEntry(Message);
            if (ShowDialogs)
            {
                ShowMessage(Message);
            }
        }

        /// <summary>
        /// Send a message that is only written to the log file
        /// </summary>
        /// <param name="Message">The message</param>
        public void WriteToLogOnly(string Message)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: WriteToLogOnly()");

            mvarLogOnly = true;
            WriteLogEntry(Message);
			mvarLogOnly = false;
        }
		protected bool mvarLogOnly = false;

        #endregion

        #region Protected and Private Members

        /// <summary>
        /// Display a message to the user
        /// </summary>
        /// <param name="Message">The message to display</param>
        protected abstract void ShowMessage(string Message);

        // **************************** Private Members ******************
        private string mAppName;
        private string mAppPath;
        private bool mIsUnix = false;

        /// <summary>
        /// Get the name of the log file
        /// </summary>
        /// <returns>The name of the log file</returns>
        private string GetLogFileName()
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: GetLogFileName()");

            string AppPath = mAppPath;
			if (!string.IsNullOrEmpty(AppPath))
			{
	            if (mIsUnix)
	            {
					if (!AppPath.EndsWith("/")) AppPath = string.Format("{0}/", AppPath);
	                return string.Format("{0}{1}.log", AppPath, mAppName);
	            }
	            else
	            {
					if (!AppPath.EndsWith(@"\")) AppPath = string.Format(@"{0}\", AppPath);
	                return string.Format(@"{0}{1}.log", AppPath, mAppName);
	            }
			}
			else
			{
				return string.Format("{0}.log", mAppName);
			}
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: GetLogFileName(): success");
        }

        /// <summary>
        /// Write an entry into the log file
        /// </summary>
        /// <param name="EntryString">The entry to write</param>
        public virtual void WriteLogEntry(string EntryString)
        {
            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("cAppReport.cs: WriteLogEntry()");

            string FName = GetLogFileName();
            string Message = string.Format("{0} {1}: {2}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), EntryString);
            if (WriteLog)
            {
                lock (this)
                {
                    try
                    {
						// append text to log file (NOTE: file is created if it does not exist)
                        StreamWriter sw = File.AppendText(FName);
                        sw.WriteLine(Message);
                        sw.Close();
                    }
                    finally
                    {
                    }
                }
            }
        }

        #endregion

    }
}
