using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 第二题
{
    class Line
    {
        public Point start;
        public Point end;
        public Root root;
        public Point[] roots;
        public Line()
        {
            roots = new Point[2];
            roots[0] = new Point(0,0);
            roots[1] = new Point(0,0);
        }
    }
    public enum Root
    {
        Zero = 0,
        One = 1,
        Two = 2,
    };
}
