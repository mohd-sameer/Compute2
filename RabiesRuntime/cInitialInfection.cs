using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace RabiesRuntime
{
    /// <summary>
    /// CLass representing an initial disease infection
    /// </summary>
    public class cInitialInfection
    {
        /// <summary>
        /// Get or set the year of the initial infection
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Get or set the week of the initial infection
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// Get or set the ID of the cell where the initial infection occurs
        /// </summary>
        public string InfectionCell { get; set; }
        /// <summary>
        /// Get or set the level of the initial infection
        /// </summary>
        public int InfectionLevel { get; set; }
    }

    /// <summary>
    /// A list of initial infections
    /// </summary>
    public class cInitialInfectionList
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public cInitialInfectionList()
        {
            mvarValues = new List<cInitialInfection>();
        }
        
        /// <summary>
        /// Get the number of items in the list
        /// </summary>
        public int Count 
        {
            get { return mvarValues.Count; }
        }
        
        /// <summary>
        /// Get the item in the list at the specified index
        /// </summary>
        /// <param name="index">The index of the item</param>
        /// <returns></returns>
        public cInitialInfection this[int index]
        {
            get { return mvarValues[index]; }
        }
        
        /// <summary>
        /// Remove the item at a specified index
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        public void RemoveAt(int index)
        {
            mvarValues.RemoveAt(index);
        }
        
        /// <summary>
        /// Clear all items from the list
        /// </summary>
        public void Clear()
        {
            mvarValues.Clear();
        }
        
        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="Value">The item to add</param>
        public void Add(cInitialInfection Value)
        {
            mvarValues.Add(Value);
        }
        
        protected List<cInitialInfection> mvarValues;
    }
}
