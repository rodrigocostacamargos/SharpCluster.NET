using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster
{
    public class PatternMatrix : IEnumerable
    {
        private HashSet<Pattern> _patternCollection;

        public PatternMatrix()
        {
            _patternCollection = new HashSet<Pattern>();
        }

        public void AddPattern(Pattern pattern)
        {
            _patternCollection.Add(pattern);
        }

        
        public Pattern[] GetPatterns()
        {
            return _patternCollection.ToArray<Pattern>();
        }

        public Pattern GetPattern(int index)
        {
            return _patternCollection.ElementAt(index);
        }

            
        public IEnumerator GetEnumerator()
        {
            return _patternCollection.GetEnumerator();
        }

        public int Size()
        {
            return _patternCollection.Count;
        }

    }
}
