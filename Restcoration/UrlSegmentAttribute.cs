using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restcoration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UrlSegmentAttribute : Attribute
    {
        public UrlSegmentAttribute(string segment, bool ignore = true)
        {
            Segment = segment;
            Ignore = ignore;
        }

        public string Segment { get; set; }
        public bool Ignore { get; set; }
    }
}
