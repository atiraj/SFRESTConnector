using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SFNetRestLib
{
[XmlRoot("SFJob")]
    public class SFJob
    {
        private string operationType;

        public string OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }

        private string objectType;
        public string ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }
     }
}
