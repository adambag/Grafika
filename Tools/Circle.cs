using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafika.Tools
{
    public class Circle
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }

        public Circle(double centerX, double centerY, double radius)
        {
            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
        }
    }
}
