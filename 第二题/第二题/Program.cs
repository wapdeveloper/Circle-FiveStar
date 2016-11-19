using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 第二题
{
    class Program
    {
        static void Main(string[] args)
        {
            //Circle cir = new Circle(new Point(0,7),4);
            //Circle cir = new Circle(new Point(2,2),5);
            Circle cir = new Circle(new Point(3.2265,7.2095 ), 5);
            FivePointStar f = new FivePointStar(new Point(0,7),new Point(2,2));
            double s = f & cir;
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}
