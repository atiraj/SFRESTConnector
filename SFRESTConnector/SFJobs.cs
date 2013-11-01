using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Serialization;

namespace SFNetRestLib
{
    public class SFJobs
    {
        private SFJob[] jobs;
        [XmlArray("SFJobs")]
        public SFJob[] Jobs
        {
            get
            {
                return jobs;
            }
            set
            {
                jobs = value;
            }
        }
    }
}
