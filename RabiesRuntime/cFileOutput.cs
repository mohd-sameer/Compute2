using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RabiesRuntime
{
    /// <summary>
    /// Represents a generic data output file
    /// </summary>
    public class cFileOutput : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Constructor (set the file name)
        /// </summary>
        /// <param name="FileName">The name of the output file</param>
        /// <param name="TabDelimited">A boolean value that indiates whether or not the file is tab-delimited</param>
        /// <param name="Append">If true, data will be appended to an existing file if the file exists</param>
        public cFileOutput(string FileName, bool TabDelimited, bool Append, bool NoOutputExceptions, cAppReport AppReport)
        {
            mvarFileName = FileName;
            this.TabDelimited = TabDelimited;
			this.NoOutputExceptions = NoOutputExceptions;
			this.AppReport = AppReport;
            OpenFile(Append);
        }


        /// <summary>
        /// Constructor (set the file name)
        /// </summary>
        /// <param name="FileName">The name of the output file</param>
        /// <param name="TabDelimited">A boolean value that indiates whether or not the file is tab-delimited</param>
        public cFileOutput(string FileName, bool TabDelimited, bool NoOutputExceptions, cAppReport AppReport)
        {
            mvarFileName = FileName;
            this.TabDelimited = TabDelimited;
			this.NoOutputExceptions = NoOutputExceptions;
			this.AppReport = AppReport;
            OpenFile(false);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public cFileOutput()
        {
            mvarFileName = "";
            this.TabDelimited = false;
            mvarIsOpen = false;
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        ~cFileOutput()
        {
            this.Dispose();
        }

        #endregion

        #region Public Properties

        // ****************************** public properties ******************************************
        /// <summary>
        /// Get or set the file name (including the path) 
        /// </summary>
        public string FileName
        {
            get { return mvarFileName; }
            set
            {
                // can't do this if the file is open
                if (mvarIsOpen)
                {
					if (this.NoOutputExceptions)
					{
						DisplayError(string.Format("Error occured setting output file name to '{0}'.  The error was 'The file is open'", value));	
					}
					else
					{
                    		throw new InvalidOperationException("The file is open.");
					}
                }
                mvarFileName = value;
            }
        }

        /// <summary>
        /// Get a boolean value indicating whether or not the file is open
        /// </summary>
        public bool IsOpen
        {
            get { return mvarIsOpen; }
        }

        /// <summary>
        /// Get or set a boolean value that indicates whether or not the output file will be Tab delimited.  If it is not,
        /// the file is output with constant cell sizes
        /// </summary>
        public bool TabDelimited { get; set; }

		/// <summary>
		/// Get or set a boolean value that indicates that output errors should be reported to the console rather than as an exception
		/// </summary>
		public bool NoOutputExceptions { get; set; }
		
		/// <summary>
		/// Get or set an app report object that will receive error messages (if NoOutputExceptions is true) 
		/// </summary>
		public cAppReport AppReport { get; set; }
		
        #endregion

        #region Public Methods

        // ****************************** public methods *********************************************
        
        /// <summary>
        /// Dispose this instance
        /// </summary>
        public void Dispose()
        {
            if (mvarIsOpen)
            {
                CloseFile();
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Open the file if the file is not already opened
        /// </summary>
        public void Open()
        {
            if (!mvarIsOpen) OpenFile(false);
        }

        /// <summary>
        /// Open the file if the file is not already opened
        /// </summary>
        /// <param name="Append">If true the file is opened for append if it already exists</param>
        public void Open(bool Append)
        {
            if (!mvarIsOpen) OpenFile(Append);
        }

        /// <summary>
        /// Close the file
        /// </summary>
        public virtual void Close()
        {
            CloseFile();
        }

        /// <summary>
        /// Actually write a string to the file 
        /// </summary>
        /// <param name="s"></param>
        public void WriteString(string s)
        {
            // if file is not open, raise an exception
            if (!mvarIsOpen)
            {
				if (this.NoOutputExceptions)
				{
					DisplayError(string.Format("Error occured writing to file '{0}'.  The error was 'The file is not currently open'", mvarFileName));
					
				}
				else
				{
                		throw new InvalidOperationException("The file is not currently open.");
				}
            }
            // let exceptions raised by this method be passed through (i.e. no try block)
			try {
            		mvarStream.WriteLine(s);
			}
			catch (Exception ex)
			{
				if (this.NoOutputExceptions)
				{
					DisplayError(string.Format("Error occured writing to file '{0}'.  The error was '{1}'", mvarFileName, ex.Message));
				}
				else
				{
                		throw;
				}
			}
        }

        #endregion

        #region Protected and Private Members

        // ****************************** protected methods ******************************************

        /// <summary>
        /// Open the file
        /// </summary>
        protected virtual void OpenFile()
        {
            OpenFile(false);
        }

        /// <summary>
        /// Open the file
        /// </summary>
        /// <param name="Append">If true, the file is opened for append if the file already exists</param>
        protected virtual void OpenFile(bool Append)
        {
            try
            {
                if (Append && File.Exists(mvarFileName))
                {
                    mvarStream = File.AppendText(mvarFileName);
                }
                else
                {
                    // first see if the specified path exists
                    string pathDelimiter = (mvarFileName.Contains("/") ? "/" : @"\");
                    int i = mvarFileName.LastIndexOf(pathDelimiter);
                    if (i >= 0)
                    {
                        string folder = mvarFileName.Substring(0, i);
                        // see if the folder exists
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    }
                    mvarStream = File.CreateText(mvarFileName);
                }
                mvarIsOpen = true;
            }
            catch (Exception ex)
            {
                mvarIsOpen = false;
				if (this.NoOutputExceptions) 
				{
					DisplayError(string.Format("Error opening file '{0}'.  The error was '{1}'", mvarFileName, ex.Message));
				}
				else
				{
					throw;
				}
            }
        }

		/// <summary>
		/// Display an error if one occurs 
		/// </summary>
		protected void DisplayError(string Msg)
		{
			if (this.AppReport != null)
			{
				this.AppReport.WriteLogEntry(Msg);
			}
			else 
			{
				Console.WriteLine(Msg);
			}
		}
		
        // ****************************** private members ********************************************
        private string mvarFileName;
        private StreamWriter mvarStream;
        private bool mvarIsOpen;

        /// <summary>
        /// Close the file
        /// </summary>
        private void CloseFile()
        {
            try
            {
                if ((mvarStream != null))
                {
                    mvarStream.Close();
                    mvarStream = null;
                    mvarIsOpen = false;
                }
            }
            catch (Exception ex)
            {
				if (this.NoOutputExceptions)
				{
					DisplayError(string.Format("Error closing file '{0}'. The error was '{1}'", mvarFileName, ex.Message));
				}
				else
				{
					throw;
				}
            }
        }

        #endregion
    }
}
