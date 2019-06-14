using System;
using System.Collections.Generic;
using System.Text;

namespace RabiesRuntime
{
    /// <summary>
    /// Class representing a cell's name and selected state
    /// </summary>
    public class cCellID
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CellName">The name of the cell</param>
        public cCellID(string CellName)
        {
            Name = CellName;
            Selected = false;
        }

        /// <summary>
        /// Get or set the name of the cell
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the selected status of the cell
        /// </summary>
        public bool Selected { get; set; }
    }
}
