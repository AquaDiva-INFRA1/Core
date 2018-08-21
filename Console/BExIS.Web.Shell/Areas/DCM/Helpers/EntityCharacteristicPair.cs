using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Dcm.UI.Helpers
{
    public class EntityCharacteristicPair
    {
        public string mappedEntityURI;
        public string mappedCharacteristicURI;

        public EntityCharacteristicPair()
        {
            this.mappedEntityURI = null;
            this.mappedCharacteristicURI = null;
        }

        public EntityCharacteristicPair(int headerId, String entity, String characteristic)
        {
            this.mappedEntityURI = entity;
            this.mappedCharacteristicURI = characteristic;
        }
    }
}