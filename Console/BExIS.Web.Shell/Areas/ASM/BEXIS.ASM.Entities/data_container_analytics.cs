using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Modules.Rpm.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.Etities
{
    public class Data_container_analytics
    {

        public List<EditUnitModel> EditUnitModel_list_in_use = new List<EditUnitModel>();
        public List<EditUnitModel> EditUnitModel_list_non_use = new List<EditUnitModel>();

        public List<DataType> DataType_in_use = new List<DataType>();
        public List<DataType> DataType_non_use = new List<DataType>();

        public List<DataStructureResultStruct> DataStruct_in_use = new List<DataStructureResultStruct>();
        public List<DataStructureResultStruct> DataStruc_non_use = new List<DataStructureResultStruct>();

        public Data_container_analytics()
        {
            this.fill_lists();
        }

        private void fill_lists()
        {
            UnitManagerModel umm = new UnitManagerModel();
            foreach (EditUnitModel unit in umm.editUnitModelList)
            {
                if (unit.inUse) EditUnitModel_list_in_use.Add(unit);
                else EditUnitModel_list_non_use.Add(unit);
            }

            
            DataTypeManager dataTypeManager = null;
            try
            {
                dataTypeManager = new DataTypeManager();
                List<DataType> datatypeList = dataTypeManager.Repo.Get().Where(d => d.DataContainers.Count != null).ToList();
                foreach (DataType datatype in datatypeList)
                {
                    if (datatype.DataContainers.Count > 0) DataType_in_use.Add(datatype);
                    else DataType_non_use.Add(datatype);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
            finally
            {
                dataTypeManager.Dispose();
            }

            List < DataStructureResultStruct> datastruct_list = new DataStructureResultsModel(null, "").dataStructureResults;
            foreach (DataStructureResultStruct ds in datastruct_list)
            {
                if (ds.inUse) this.DataStruct_in_use.Add(ds);
                else this.DataStruc_non_use.Add(ds);
            }

        }

    }
}