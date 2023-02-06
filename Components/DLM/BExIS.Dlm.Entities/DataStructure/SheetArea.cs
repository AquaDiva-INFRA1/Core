﻿using System.Collections.Generic;
using Vaiona.Entities.Common;

/// <summary>
///
/// </summary>        
namespace BExIS.Dlm.Entities.DataStructure
{
    public class SheetArea : BaseEntity
    {
        #region Attributes

        public int StartColumn { get; private set; }
        public int EndColumn { get; private set; }
        public int StartRow { get; private set; }
        public int EndRow { get; private set; }

        #endregion


        #region Methods

        /// <summary>
        /// Initializes a new SheetArea
        /// </summary>
        public SheetArea()
        {
            this.StartColumn = 0;
            this.EndColumn = 0;
            this.StartRow = 0;
            this.EndRow = 0;
        }

        /// <summary>
        /// Initializes a new SheetArea by given start- and endvalues
        /// </summary>
        /// <param name="StartColumn">First coulumn field</param>
        /// <param name="EndColumn">Last coulumn field</param>
        /// <param name="StartRow">First row field</param>
        /// <param name="EndRow">Last row field</param>
        public SheetArea(int StartColumn, int EndColumn, int StartRow, int EndRow)
        {
            this.StartColumn = StartColumn;
            this.EndColumn = EndColumn;
            this.StartRow = StartRow;
            this.EndRow = EndRow;
        }

        #endregion
    }
}