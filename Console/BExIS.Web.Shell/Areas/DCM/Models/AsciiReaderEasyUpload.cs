

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using BExIS.IO.Transform.Validation.DSValidation;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.IO.Transform.Input;

/// <summary>
///
/// </summary>        
namespace BExIS.Modules.Dcm.UI.Models
    {
        /// <summary>
        /// this class is used to read and validate ascii files
        /// </summary>
        /// <remarks></remarks>        
        public class AsciiReaderEasyUpload : AsciiReader
        {
            public AsciiReaderEasyUpload (StructuredDataStructure structuredDatastructure, AsciiFileReaderInfo fileReaderInfo) : base(structuredDatastructure, fileReaderInfo)
            {
            }

            /// <summary>
            /// Read line by line based on a packageSize. 
            /// Convert the lines into a datatuple based on the datastructure.
            /// Return value is a list of datatuples
            /// </summary>
            public DataTuple[] ReadFile(Stream file, string fileName, List<String[]> Json_table,
                AsciiFileReaderInfo fri, StructuredDataStructure sds, long datasetId, int packageSize,
                EasyUploadFileReaderInfo EasyUploadFileReader)
            {

                // clear list of datatuples
                this.DataTuples = new List<DataTuple>();
                this.VariableIdentifierRows = new List<List<string>>();
                this.SubmitedVariableIdentifiers = new List<VariableIdentifier>();

                this.FileStream = file;
                this.FileName = fileName;
                this.Info = fri;
                this.StructuredDataStructure = sds;
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
                if (this.Info.Variables <= 0)
                {
                    this.ErrorMessages.Add(new Error(ErrorType.Other, "Startrow of Variable can´t be 0"));
                }
                if (this.Info.Data <= 0)
                {
                    this.ErrorMessages.Add(new Error(ErrorType.Other, "Startrow of Data can´t be 0"));
                }

                if (this.ErrorMessages.Count == 0)
                {
                    int header_StartColumn = EasyUploadFileReader.VariablesStartColumn - 1;
                    int header_EndColumn = EasyUploadFileReader.VariablesEndColumn - 1;
                    int header_StartRow = EasyUploadFileReader.VariablesStartRow - 1;
                    int header_EndRow = EasyUploadFileReader.VariablesEndRow - 1;

                    int Var_StartColumn = EasyUploadFileReader.DataStartColumn - 1;
                    int Var_EndColumn = EasyUploadFileReader.DataEndColumn - 1;
                    int Var_StartRow = EasyUploadFileReader.DataStartRow - 1;
                    int Var_EndRow = EasyUploadFileReader.DataEndRow - 1;

                    char seperator = AsciiFileReaderInfo.GetSeperator(fri.Seperator);

                    for (int i = header_StartRow; i <= header_EndRow; i++)
                    {
                            string[] cells = Json_table[i];
                            List<String> vars = new List<string>();
                            for (int j = header_StartColumn; j <= header_EndColumn; j++)
                            {
                                vars.Add(cells[j]);
                            }
                            ValidateDatastructure(String.Join(",", vars.ToArray()), seperator);
                    }

                    convertAndAddToSubmitedVariableIdentifier();

                    for (int i = Var_StartRow; i <= Var_EndRow; i++)
                    {
                        string[] cells = Json_table[i];
                        List<String> vars = new List<string>();

                        for (int j = Var_StartColumn; j <= Var_EndColumn; j++)
                        {
                            vars.Add(cells[j]);
                        }
                        //index of the row is not needed in the method "ReadRow"
                        this.DataTuples.Add(ReadRow(vars, 0));
                    }
                }

                return this.DataTuples.ToArray();
            }
        
            /// <summary>
            /// Convert a list of variable names to 
            /// VariableIdentifiers
            /// </summary>
            /// <seealso cref="VariableIdentifier"/>
            /// <param name="variablesRow"></param>
            private void convertAndAddToSubmitedVariableIdentifier()
            {
                List<VariableIdentifier> temp_list = new List<VariableIdentifier>();
                if (this.SubmitedVariableIdentifiers.Count == 0)
                    if (VariableIdentifierRows != null)
                        foreach (List<string> l in VariableIdentifierRows)
                            foreach (string s in l)
                            {
                                VariableIdentifier hv = new VariableIdentifier();
                                hv.name = s;
                                temp_list.Add(hv);
                            }
                this.SubmitedVariableIdentifiers = temp_list.Count > 0 ? temp_list : this.SubmitedVariableIdentifiers;
            }


            public void setSubmittedVariableIdentifiers(List<VariableIdentifier> vi)
            {
                this.SubmitedVariableIdentifiers = vi;
            }

        }
    }

