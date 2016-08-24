using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster
{
    //data structure that stores data set numerical values
    public class Pattern
    {
        #region private members
        private int _id; // pattern id
        private int _classAttribute; //class atribute used for external validation (i.e., rand and jaccard indexes)
        private List<double> _attributeCollection; //attribute collection of a pattern
        #endregion

        #region class constructor
        public Pattern()
        {
            _attributeCollection = new List<double>();
        }
        #endregion

        #region class properties
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int ClassAttribute
        {
            get
            {
                return _classAttribute;
            }

            set
            {
                _classAttribute = value;
            }
        }
        #endregion

        #region class methods
        public void AddAttribute(double attribute)
        {
            _attributeCollection.Add(attribute);
        }

        public void RemoveAttributeAt(int i)
        {
            _attributeCollection.RemoveAt(i);
        }

        public double GetAttribute(int index)
        {
            return _attributeCollection[index];
        }

        public void AddAttributes(double[] attributes)
        {
            _attributeCollection.AddRange(attributes);
        }

       public double[] GetAttributes()
        {
            return _attributeCollection.ToArray<double>();
        }

        public int GetDimension()
        {
            return _attributeCollection.Count;
        }
        #endregion

    }
}
