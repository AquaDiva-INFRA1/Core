﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using BExIS.Dlm.Entities.DataStructure;

/// <summary>
///
/// </summary>        
namespace BExIS.Dlm.Entities.Data
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>        
    public class VariableValue: DataValue
    {
        #region Attributes

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>        
        [XmlIgnore]
        public DataTuple Tuple { get; set; } // reference to the containing tuple

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>        
        public Int64 VariableId { get; set; } // when variable is not loaded!

        #endregion

        #region Associations

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>        
        [XmlIgnore]
        public IList<ParameterValue> ParameterValues { get; set; }

        private DataAttribute _dataAttribute;

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>    
        [XmlIgnore]
        public DataAttribute DataAttribute
        {
            get
            {
                if (_dataAttribute != null) return _dataAttribute;
                
                if (this.Tuple.DatasetVersion.Dataset.DataStructure.Self is StructuredDataStructure)
                {
                    return (this.Variable.DataAttribute);
                }
                return (null);
            }
            set {
                _dataAttribute = value;
            }
        }


        private Variable _variable;

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>        
        [XmlIgnore]
        public Variable Variable
        {
            get
            {
                if (_variable != null) return _variable;
                {
                    if (this.Tuple.DatasetVersion.Dataset.DataStructure.Self is StructuredDataStructure)
                    {
                        Variable u = (this.Tuple.DatasetVersion.Dataset.DataStructure.Self as StructuredDataStructure).Variables
                            .Where(p => p.Id.Equals(this.VariableId))
                            .Select(p => p).FirstOrDefault();
                        return (u);
                    }
                }


                return (null);
            }
            set{
                _variable = value;
            }
        }

        #endregion

        #region Mathods

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        /// <param>NA</param>       
        public VariableValue()
        {
            ParameterValues = new List<ParameterValue>();
        }

        #endregion

    }
}
