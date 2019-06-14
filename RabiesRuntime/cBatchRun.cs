using System;
using System.Collections.Generic;
using System.Text;
using Rabies_Model_Core;
using Fox;
using System.Threading;
using System.Net;
using System.IO;
using System.Collections;

namespace RabiesRuntime
{
    /// <summary>
    /// A class to define a batch run for the Fox Rabies Model (essentially a list of 
    /// settings files)
    /// </summary>
    public class cBatchRun : IEnumerable
    {
        // the list of files to batch
        private List<string> mvarBatchFiles;
 
        #region Constructor

        // **************************** Constructor ******************************************
        public cBatchRun()
        {
            // create the list of files to batch
            mvarBatchFiles = new List<string>();
        }

        #endregion

        #region Public Properties

        // *************************** Public Properties *************************************
        /// <summary>
        /// Get an item in the list
        /// </summary>
        /// <param name="index">The index of the item</param>
        public string this[int index]
        {
            get { return mvarBatchFiles[index]; }
        }

        /// <summary>
        /// Get the number of items in the list
        /// </summary>
        public int Count
        {
            get { return mvarBatchFiles.Count; }
        }

        #endregion

        #region Public Methods

        // ************************** Public Methods *****************************************

        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(string item)
        {
            mvarBatchFiles.Add(item);
        }
        
        /// <summary>
        /// Add an item to the list at the specified index position
        /// </summary>
        /// <param name="item">The item to aadd</param>
        /// <param name="index">The index position for the item</param>
        public void Add(string item, int index)
        {
            if (index > mvarBatchFiles.Count) throw new IndexOutOfRangeException();
            // add on the last item in the list as a new ite
            mvarBatchFiles.Add(mvarBatchFiles[mvarBatchFiles.Count - 1]);
            // move all other items up one
            for (int i = mvarBatchFiles.Count - 2; i >= index + 1; i += -1)
            {
                mvarBatchFiles[i] = mvarBatchFiles[i - 1];
            }
            // finally set the value at position index
            mvarBatchFiles[index] = item;
        }

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        public void RemoveAt(int index)
        {
            mvarBatchFiles.RemoveAt(index);
        }

        /// <summary>
        /// Swap the positions of two items in the list
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void Swap(int index1, int index2)
        {
            string temp = null;
            temp = mvarBatchFiles[index1];
            mvarBatchFiles[index1] = mvarBatchFiles[index2];
            mvarBatchFiles[index2] = temp;
        }

        /// <summary>
        /// Return an enumerator for this collection
        /// </summary>
        /// <returns>An enumerator for this collection</returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < mvarBatchFiles.Count; i++) yield return mvarBatchFiles[i];
        }

        /// <summary>
        /// Clear all items
        /// </summary>
        public void Clear()
        {
            mvarBatchFiles.Clear();
        }

        #endregion
    }
}
