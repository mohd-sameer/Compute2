using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexedHashTable
{
    // EER: Created to add duplicate key items to a dictionary
    // EER: For filling in user-form information (i.e. supercells, K)
    public class ListWithDuplicates : List<KeyValuePair<string, double>>
    {
        public void Add(string key, double value)
        {
            var element = new KeyValuePair<string, double>(key, value);
            this.Add(element);
        }
    }
}
