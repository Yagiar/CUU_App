using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetected
{
    internal class ResultItem
    {
        public double Xmin { get; set; }
        public double Ymin { get; set; }
        public double Xmax { get; set; }
        public double Ymax { get; set; }
        public double Confidence { get; set; }
        public int Class { get; set; }
        public string Name { get; set; }
    }
}
