using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Dcm.UI.Models
{
    public class EasyUploadVariableInformation
    {
        public int headerId;
        public string variableName;
        public UnitInfo unitInfo;

        public EasyUploadVariableInformation(int headerId, string variableName, UnitInfo unitInfo)
        {
            this.headerId = headerId;
            this.variableName = variableName;
            this.unitInfo = unitInfo;
        }
    }
}