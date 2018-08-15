using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.UI.Models
{
    public class Data_container_analytics
    {
        //var templates : use non-use results
        public int in_use_var_temps ;
        public int not_used_var_temps ;

        //units : use non-use results
        public int in_use_unit ;
        public int not_used_unit ;

        //Data types : use non-use results
        public int in_use_DataType;
        public int not_used_DataType;

        public Data_container_analytics(int in_use_var_temps, int not_used_var_temps, 
            int in_use_unit, int not_used_unit,
            int in_use_DataType, int not_used_DataType)
        {
            this.in_use_var_temps = in_use_var_temps;
            this.not_used_var_temps = not_used_var_temps;

            this.in_use_unit = in_use_unit;
            this.not_used_unit = not_used_unit;

            this.in_use_DataType = in_use_DataType;
            this.not_used_DataType = not_used_DataType;
        }
    }
}