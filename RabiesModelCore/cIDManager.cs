using System;
using System.Text;

namespace Rabies_Model_Core
{
	/// <summary>
	///		A class to handle the generation of unique ID values.  The ID manager will return
	///		a numeric ID "wrapped" in a user defined prefix and/or suffix.
	/// </summary>
	public class cIDManager
	{
		// ********************** Constructors ******************************************
		/// <summary>
		///		Initialize an ID manager.  The first ID will have value 0.  The prefix will
		///		be "ID#", the suffix will be an empty string.
		/// </summary>
		public cIDManager()
		{
			Prefix = "ID#";
			Suffix = "";
			IDNum = 0;
		}

		/// <summary>
		///		Initialize an ID manager specifying the value of the first ID.  The prefix
		///		will be "ID#", the suffix will be an empty string.
		/// </summary>
		/// <param name="StartingID">
		///		The numeric value of the first ID.  An ArgumentOutOfRangeException exception
		///		is raised if StartingID is less than zero.
		///	</param>
		public cIDManager(int StartingID) 
		{
			if (StartingID < 0)
				throw new ArgumentOutOfRangeException("StartingID",
					"StartingID must be zero or greater");
			Prefix = "ID#";
			Suffix = "";
			IDNum = StartingID;
		}

		// ********************** Properties ********************************************
		/// <summary>
		///		A constant string Prefix for all IDs.
		/// </summary>
		public string Prefix;
		/// <summary>
		///		A constant string Suffix for all IDs.
		/// </summary>
		public string Suffix;
		/// <summary>
		///		The numeric value of the current ID.
		/// </summary>
		public int IDNum;

		// ********************** Methods ***********************************************
		/// <summary>
		///		Get the next unique ID.
		/// </summary>
		/// <returns>The next unique ID.</returns>
		public string GetNextID() 
		{
			StringBuilder NewID = new StringBuilder(Prefix);
			NewID.Append(IDNum++);
			NewID.Append(Suffix);
			return NewID.ToString();
		}
	}
}
