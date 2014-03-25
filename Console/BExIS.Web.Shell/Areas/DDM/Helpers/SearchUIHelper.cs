﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.DataStructure;
using Vaiona.Util.Cfg;

namespace BExIS.Web.Shell.Areas.DDM.Helpers
{
    public class SearchUIHelper
    {
        public static string ConvertXmlToHtml(string m, string xslPath="")
        {
            string url = "";
            if (xslPath != ""){
                url = AppConfiguration.GetModuleWorkspacePath("DDM") + "\\UI\\HtmlShowMetadata.xsl";
            }
            else {
                url = AppConfiguration.GetModuleWorkspacePath("DDM") + xslPath;
            }

            if (m != null)
            {
                
                StringReader stringReader = new StringReader(m);
                XmlReader xmlReader = XmlReader.Create(stringReader);

                XslCompiledTransform xslt = new XslCompiledTransform(true);
                XsltSettings xsltSettings = new XsltSettings(true, false);
                xslt.Load(url, xsltSettings, new XmlUrlResolver());

                XsltArgumentList xsltArgumentList = new XsltArgumentList();

                StringWriter stringWriter = new StringWriter();
                xslt.Transform(xmlReader, xsltArgumentList, stringWriter);
                return stringWriter.ToString().Replace("bgc:","");

                
            }

            return "";

        }


        public static DataTable ConvertPrimaryDataToDatatable(DatasetVersion dsv, List<DataTuple> dsVersionTuples)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Primary data table";
            DataStructureManager dsm = new DataStructureManager();
            StructuredDataStructure sds = dsm.StructuredDataStructureRepo.Get(dsv.Dataset.DataStructure.Id);

            if (dsVersionTuples != null && sds != null)
            {
                foreach (var vu in sds.Variables)
                {
                    // use vu.Label or vu.DataAttribute.Name
                    DataColumn col = dt.Columns.Add(vu.Label.Replace(" ","")); // or DisplayName also
                    col.Caption = vu.Label;

                    switch (vu.DataAttribute.DataType.SystemType)
                    {
                        case "String":
                            {
                                col.DataType = Type.GetType("System.String");
                                break;
                            }

                        case "Double":
                            {
                                col.DataType = Type.GetType("System.Double");
                                break;
                            }

                        case "Int16":
                            {
                                col.DataType = Type.GetType("System.Int16");
                                break;
                            }

                        case "Int32":
                            {
                                col.DataType = Type.GetType("System.Int32");
                                break;
                            }

                        case "Int64":
                            {
                                col.DataType = Type.GetType("System.Int64");
                                break;
                            }

                        case "Decimal":
                            {
                                col.DataType = Type.GetType("System.Decimal");
                                break;
                            }

                        case "DateTime":
                            {
                                col.DataType = Type.GetType("System.DateTime");
                                break;
                            }

                        default:
                            {
                                col.DataType = Type.GetType("System.String");
                                break;
                            }
                    }
                   


                    if(vu.Parameters.Count>0)
                    {
                        foreach (var pu in vu.Parameters)
                        {
                            DataColumn col2 = dt.Columns.Add(pu.Label.Replace(" ", "")); // or DisplayName also
                            col2.Caption = pu.Label;
                            
                        }
                    }
                }

                foreach (var tuple in dsVersionTuples)
                {
                     dt.Rows.Add(ConvertTupleIntoDataRow(dt,tuple));
                }
            }

            return dt;
        }

        private static DataRow ConvertTupleIntoDataRow(DataTable dt, DataTuple t)
        {

            DataRow dr = dt.NewRow();

            DataStructureManager dsm = new DataStructureManager();

            foreach(var vv in t.VariableValues)
            {
                if (vv.Variable != null)
                {
                   

                    switch (vv.DataAttribute.DataType.SystemType)
                    { 
                        case "String":
                        {
                            if (vv.Value == null)
                            {
                                dr[vv.Variable.Label.Replace(" ", "")] = "null";
                            }
                            else
                            {
                                dr[vv.Variable.Label.Replace(" ", "")] = vv.Value.ToString();
                            }
                            break;
                        }

                        case "Double":
                        {
                            dr[vv.Variable.Label.Replace(" ", "")] = Convert.ToDouble(vv.Value);
                            
                            break;
                        }

                        case "Int16":
                        {
                            dr[vv.Variable.Label.Replace(" ", "")] = Convert.ToInt16(vv.Value);
                            break;
                        }

                        case "Int32":
                        {
                            dr[vv.Variable.Label.Replace(" ", "")] = Convert.ToInt32(vv.Value);
                            break;
                        }

                        case "Int64":
                        {
                            dr[vv.Variable.Label.Replace(" ", "")] = Convert.ToInt64(vv.Value);
                            break;
                        }

                        case "Decimal":
                        {
                            dr[vv.Variable.Label.Replace(" ", "")] = Convert.ToDecimal(vv.Value);
                            break;
                        }

                        case "DateTime":
                        {
                            dr[vv.Variable.Label.Replace(" ", "")] = Convert.ToDateTime(vv.Value, CultureInfo.InvariantCulture);
                            break;
                        }
                
                        default:
                        {
                            dr[vv.Variable.Label.Replace(" ", "")] = vv.Value;
                            break;
                        }
                    }

                    

                    /*if (vv.ParameterValues.Count > 0)
                    {
                        foreach (var pu in vv.ParameterValues)
                        {
                            dr[pu.Parameter.Label.Replace(" ", "")] = pu.Value;
                        }
                    }*/
                }
            }

            return dr;
        }

        public static DataTable ConvertStructuredDataStructureToDataTable(StructuredDataStructure sds)
        { 
            DataTable dt = new DataTable();

            dt.TableName = "DataStruture";
            dt.Columns.Add("VariableName");
            dt.Columns.Add("Parameters");
            dt.Columns.Add("Unit");
            dt.Columns.Add("Description");

            DataStructureManager dsm = new DataStructureManager();
            StructuredDataStructure datastructure = dsm.StructuredDataStructureRepo.Get(sds.Id);
            if (datastructure != null)
            {
                foreach (Variable var in datastructure.Variables)
                {
                    Variable sdvu = dsm.VariableRepo.Get(var.Id);

                    DataRow dr = dt.NewRow();
                    if (sdvu.Label != null) dr["VariableName"] = sdvu.Label;
                    else dr["VariableName"] = "n/a";


                    if (sdvu.Parameters.Count > 0) dr["Parameters"] = "current not shown";
                    else dr["Parameters"] = "n/a";

                    if (sdvu.DataAttribute.Unit != null) dr["Unit"] = sdvu.DataAttribute.Unit.Name;
                    else dr["Unit"] = "n/a";

                    if (sdvu.DataAttribute.Description != null || sdvu.DataAttribute.Description != "")
                    {

                        dr["Description"] = sdvu.DataAttribute.Description;
                    }
                    else dr["Description"] = "n/a";

                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private string GetParameterNamesAsString(ICollection<Parameter> vpuList)
        {
            string parameters = "";
            foreach (Parameter vpu in vpuList)
            {
                if (vpu.Equals(vpuList.First()))
                    parameters = vpu.Label;
                else
                    parameters += ", " + vpu.Label;
            }

            return "";
        }
    }
}