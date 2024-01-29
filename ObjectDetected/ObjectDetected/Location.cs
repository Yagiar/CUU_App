using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetected
{
    internal class Location
    {
        public Location(Point p)
        {
            x = p.X;
            y = p.Y;
        }
        private double x;
        private double y;
        public double X
        {
            get { return x; }
            set { x = value; }
        }
        public double Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
