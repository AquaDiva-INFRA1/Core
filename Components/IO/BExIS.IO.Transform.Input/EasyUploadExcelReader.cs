﻿using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.IO.Transform.Input;
using BExIS.IO.Transform.Validation.DSValidation;
using BExIS.IO.Transform.Validation.Exceptions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BExIS.IO.Transform.Input
{
    public class EasyUploadExcelReader : ExcelReader
    {
        private char[] alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public DataTuple[] ReadFile(Stream file, string fileName, EasyUploadFileReaderInfo fri, StructuredDataStructure sds, long datasetId)
        {
            this.FileStream = file;
            this.FileName = fileName;

            this.StructuredDataStructure = sds;
            this.Info = fri;
            this.DatasetId = datasetId;

            // Check params
            if (this.FileStream == null)
            {
                this.ErrorMessages.Add(new Error(ErrorType.Other, "File not exist"));
            }
            if (!this.FileStream.CanRead)
            {
                this.ErrorMessages.Add(new Error(ErrorType.Other, "File is not readable"));
            }
            if (fri.VariablesStartRow <= 0)
            {
                this.ErrorMessages.Add(new Error(ErrorType.Other, "Startrow of Variable can´t be 0"));
            }
            if (fri.DataStartRow <= 0)
            {
                this.ErrorMessages.Add(new Error(ErrorType.Other, "Startrow of Data can´t be 0"));
            }

            if (this.ErrorMessages.Count == 0)
            {
                // open excel file
                spreadsheetDocument = SpreadsheetDocument.Open(this.FileStream, false);

                // get workbookpart
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

                //SheetDimension dimension = workbookPart.WorksheetParts.First().Worksheet.GetFirstChild<SheetDimension>();

                // get all the defined area 
                //List<DefinedNameVal> namesTable = BuildDefinedNamesTable(workbookPart);

                this._areaOfVariables.StartColumn = alphabet[fri.VariablesStartColumn - 1].ToString();
                this._areaOfVariables.EndColumn = alphabet[fri.VariablesEndColumn - 1].ToString();
                this._areaOfVariables.StartRow = fri.VariablesStartRow;
                this._areaOfVariables.EndRow = fri.VariablesEndRow;

                this._areaOfData.StartColumn = alphabet[fri.DataStartColumn - 1].ToString();
                this._areaOfData.EndColumn = alphabet[fri.DataEndColumn - 1].ToString();
                this._areaOfData.StartRow = fri.DataStartRow;
                this._areaOfData.EndRow = fri.DataEndRow;


                // Get intergers for reading data
                startColumn = fri.VariablesStartColumn;
                endColumn = fri.VariablesEndColumn;

                numOfColumns = (endColumn - startColumn) + 1;
                offset = this.Info.Offset;

                int endRowData = fri.DataEndRow;

                // select worksheetpart by selected defined name area like data in sheet
                // sheet where data area is inside
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First(); //GetWorkSheetPart(workbookPart, this._areaOfData);

                // get styleSheet
                _stylesheet = workbookPart.WorkbookStylesPart.Stylesheet;

                // Get shared strings
                _sharedStrings = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ToArray();


                if (this.SubmitedVariableIdentifiers != null)
                {
                    ReadRows(worksheetPart, fri.DataStartRow, endRowData);
                }

                return this.DataTuples.ToArray();
            }

            return this.DataTuples.ToArray();
        }

        public void setSubmittedVariableIdentifiers(List<VariableIdentifier> vi)
        {
            this.SubmitedVariableIdentifiers = vi;
        }

    }
}