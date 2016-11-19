using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 第二题
{
    class Point
    {
        public double x;
        public double y;
        public bool inside;
        public bool vertex;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        public bool outline;
    }
}
