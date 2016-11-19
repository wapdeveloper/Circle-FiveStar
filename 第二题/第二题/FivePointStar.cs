using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace 第二题
{
    class FivePointStar
    {
        public Point[] a;
        public Point[] b;
        public Point center;
        ArrayList alist = new ArrayList();
        Line arcstartline = new Line();
        Line arcendline = new Line();
        List<Line> clist;
        bool start_f = false;
        bool end_f = false;
        bool onetime = false; //大跨度的圆弧
        static Circle cir;

        public FivePointStar(Point p1, Point p2)
        {
            GetAllPoints(p1, p2);
        }

        //坐标反算
        public void BackCalCoordinate(out double s, out double α, Point a, Point b)
        {
            α = Math.Atan2(b.y - a.y, b.x - a.x);
            if (α < 0) α += 2 * Math.PI;
            s = Math.Sqrt((b.y - a.y) * (b.y - a.y) + (b.x - a.x) * (b.x - a.x));
        }
        //坐标正算
        public Point CalCoordinate(Point a, double s, double α) //弧度
        {
            double x = a.x + s * Math.Cos(α);
            double y = a.y + s * Math.Sin(α);
            return new Point(x, y);
        }

        public void GetAllPoints(Point c, Point p)
        {
            double s, α, s1;
            BackCalCoordinate(out s, out α, c, p); //反算
            s1 = Math.Sin(Math.PI / 10) / Math.Sin(126.0 / 180 * Math.PI) * s;
            double β = 72.0 / 180 * Math.PI;
            a = new Point[5];
            b = new Point[5];
            a[4] = p;
            a[4].vertex = true;
            double r = α + β / 2;
            if (r > Math.PI * 2) r -= Math.PI * 2;
            b[4] = CalCoordinate(c, s1, r);
            for (int i = 0; i < 4; i++)
            {
                r = α + β * (i + 1);
                if (r > Math.PI * 2) r -= Math.PI * 2;
                a[i] = CalCoordinate(c, s, r);
                a[i].vertex = true;
                r = α + β * (i + 1) + Math.PI / 5;
                if (r > Math.PI * 2) r -= Math.PI * 2;
                b[i] = CalCoordinate(c, s1, r);
            }
        }

        public List<Point> ContiunePoint()
        {
            List<Point> list = new List<Point>();
            list.Add(a[4]);
            list.Add(b[4]);
            for (int i = 0; i < 4; i++)
            {
                list.Add(a[i]);
                list.Add(b[i]);
            }
            return list;
        }

        public void IsInCircle(Point p)
        {
            double s = Math.Sqrt((p.y - cir.center.y) * (p.y - cir.center.y) + (p.x - cir.center.x) * (p.x - cir.center.x));
            if (s < cir.r)
            {
                p.inside = true;
            }
        }


        public void PointsInCircle(List<Point> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                IsInCircle(list[i]);
            }
        }
        public double CalculatePolygonArea(List<Point> list)
        {
            double area;
            if (list.Count < 3)
                return 0;
            area = list[0].y * (list[list.Count - 1].x - list[1].x);
            for (int i = 1; i < list.Count; i++)
                area += list[i].y * (list[(i - 1)].x - list[(i + 1) % list.Count].x);
            return area / 2;
        }
        public double CalculateArcArea(Point a, Point b)
        {
            double s = Math.Sqrt((a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y));
            double α = Math.Acos((2 * cir.r * cir.r - s * s) / (2 * cir.r * cir.r));
            return 0.5 * cir.r * cir.r * (α - Math.Sin(α));
        }
        public double TwoArea(ArrayList list)
        {
            List<Point> area=new List<Point>();
            List<Arc> arcs=new List<Arc>();
            foreach (var item in list)
            {
                if (item as Segment != null)
                {
                    area.Add((item as Segment).start);
                    if(item!=list[list.Count-1])
                       area.Add((item as Segment).end);
                }
                if (item as Arc != null)
                {
                    arcs.Add((item as Arc));
                }
            }
            double s1 = CalculatePolygonArea(area);
            double s2 = 0;
            foreach (var item in arcs)
            {
                s2+=CalculateArcArea(item.start, item.end);
            }
            return s1 + s2;

        }

        public double CalCulateAera(Circle cir)
        {
            //解方程求交点啊
            List<Point> plist = ContiunePoint();
            PointsInCircle(plist);
            clist= GenerateLine(plist);//得到多对
            track(cir);
            return TwoArea(alist);;
        }

        //运算符重载
        public static double operator &(FivePointStar f, Circle c)
        {
            cir = c;
            return f.CalCulateAera(cir);
        }

        public List<Line> GenerateLine(List<Point> plist)
        {
            List<Line> clist = new List<Line>();
            for (int i = 0; i < plist.Count; i++)
            {
                Line line = new Line();
                line.start = plist[i];
                line.end = plist[(i + 1)%plist.Count];
                SolveEquations(line.start, line.end, line);             
                clist.Add(line);
            }
            return clist;
        }

        public Point NearestVertexPoint(Point root1,Point root2, Point vertex)
        {
            double s1 = (root1.x - vertex.x) * (root1.x - vertex.x)
                + (root1.y - vertex.y) * (root1.y - vertex.y);
            double s2 = (root2.x - vertex.x) * (root2.x - vertex.x)
                + (root2.y - vertex.y) * (root2.y - vertex.y);
            if (s1 <= s2)
                return root1;
            else return root2;
        }

        public Point NearestVertex(Line line)
        {
            if (line.roots == null) return null;
            if (line.root == Root.One)
                return line.roots[0];
            if (line.root == Root.Two)
            {
                if (line.start.vertex == true)
                {
                    return  NearestVertexPoint(line.roots[0],line.roots[1],line.start);
                }
                else
                {
                    return  NearestVertexPoint(line.roots[0], line.roots[1], line.end);
                }
            }
            return null;
        }


        public Point FarestVertex(Line line)
        {
                if (line.root == Root.One)
                    return line.roots[0];
                if(line.root == Root.Two)
                {
                    Point notvertex=new Point(0,0);
                    Point vertex= NearestVertex(line);
                    if(line.roots[0]==vertex)
                       return notvertex=line.roots[1];
                    else
                       return notvertex=line.roots[0];
                }
                return null;
        }


        public void GetArcStartLine(int k)
        {
            if (start_f) return;
            if (clist[(k + 1) % clist.Count].end.inside) return;
            if (clist[k].root != Root.Zero && clist[(k + 1) % clist.Count].root == Root.Zero)
            {
                arcstartline = clist[k];
                start_f = true;
            }
        }
        public void GetArcEndtLine(int k,out int index)
        {    

            index = 0;
            if (alist.Count == 0) return;
            if (clist[k].start.inside) return;
            if (clist[k].root == Root.Zero && clist[(k + 1) % clist.Count].root != Root.Zero)
            {
                arcendline = clist[(k + 1) % clist.Count]; //要找最近根
                end_f = true;
                index = k;
            }

        }


        public void Choosetype(Line line, Circle c, int k)
        {
            //判断起点啊在圆内还是圆外
            //判断终点啊在圆内还是圆外
            //起点在圆内，只能有0个交点或者一个交点不可能有两个交点
            if (line.start.inside && line.end.inside)//两个点都在圆内
            {
                alist.Add(new Segment(line.start, line.end));
            }
            if (line.start.inside && !line.end.inside)//起点在圆内，终点在圆外
            {
                alist.Add(new Segment(line.start, line.roots[0]));
            }
            if (!line.start.inside && line.end.inside)//起点在圆外，终点在圆内
            {
                alist.Add(new Segment(line.roots[0], line.end));
            }

            //增加圆弧
            if (k % 2 != 0 && clist[k].root != Root.Zero && clist[(k + 1) % clist.Count].root != Root.Zero)
            {
                alist.Add(new Arc(NearestVertex(clist[k]), NearestVertex(clist[(k + 1) % clist.Count])));
            }
            
            GetArcStartLine(k);
            int a = 0;
            GetArcEndtLine(k,out a);

            if (start_f && end_f&&onetime==false)
            {
                alist.Add(new Arc((alist[alist.Count - 1] as Segment).end, FarestVertex(clist[(k + 1) % clist.Count])));
                onetime = true;
            }
            if(k==clist.Count-1&&start_f&&!end_f)
            {
                int index = 0;
                for (int i = 0; i <clist.Count; i++)
                {
                     if (end_f) break;   
                     GetArcEndtLine(i,out index);
                }
                alist.Add(new Arc((alist[alist.Count - 1] as Segment).end, FarestVertex(clist[(index+1) % clist.Count])));
            }

        }

        public void track(Circle c)
        {
            for (int i = 0; i < clist.Count ; i++)
            {
                Choosetype(clist[i], c, i);
            }
        }

        public void GetXbound(out double xmin, out double xmax,Point a,Point b)
        {
            xmin = a.x;
            xmax = b.x;
            if (b.x<a.x)
            {
                xmin = b.x;
                xmax = a.x;
            }
        }

        public bool IsOnLine(Point a,Point b,double x)
        {
            double x1,x2;
            GetXbound(out x1,out x2,a,b);
            if (x1 < x && x < x2)
            {
                return true;
            }
            return false;
        }

        public void Offset(Point a,Point b,Point p,double x,double y)
        {
            a.x -= x; a.y -= y;
            b.x -= x; b.y -= y;
            p.x -= x; p.y -= y;
        }
        public void SolveEquations(Point p1, Point p2 ,Line line)
        {
            double x, y, r = cir.r;
            double offx = 0, offy = 0;
            offx = cir.center.x;
            offy = cir.center.y;
            Offset(p1, p2, cir.center, offx, offy);
            double k, b;
            GetKBLine(out k,out b,p1,p2);
            double x1 = (-k * b + Math.Sqrt(k * k * r * r + r * r - b * b)) / (k * k + 1);
            double x2 = (-k * b - Math.Sqrt(k * k * r * r + r * r - b * b)) / (k * k + 1);
            if (IsOnLine(p1, p2, x1)&&IsOnLine(p1, p2, x2))//有两个根
            {
                line.roots[0].x=x1;
                line.roots[0].y = k*line.roots[0].x + b;
                line.roots[0].x +=offx;                line.roots[0].y +=offy;
                line.roots[1].x=x2;            
                line.roots[1].y = k*line.roots[1].x + b;
                line.roots[1].x +=offx;                line.roots[1].y +=offy;
                line.root = Root.Two;
            }
            if (IsOnLine(p1, p2, x1) && !IsOnLine(p1, p2, x2))
            {
                line.roots = new Point[2];
                line.roots[0] = new Point(0,0);
                line.roots[0].x=x1;
                line.roots[0].y = k*line.roots[0].x + b;
                line.roots[0].x +=offx;                line.roots[0].y +=offy;
                line.root = Root.One;
            }
             if(!IsOnLine(p1, p2, x1) && IsOnLine(p1, p2, x2))
             {
                line.roots = new Point[2];
                line.roots[0] = new Point(0, 0);
                line.roots[0].x = x2;
                line.roots[0].y = k*line.roots[0].x + b;
                line.roots[0].x +=offx;                line.roots[0].y +=offy;
                line.root = Root.One;
             }
             if(!IsOnLine(p1, p2, x1) && !IsOnLine(p1, p2, x2))
             {
                 line.root = Root.Zero;
             }
             Offset(p1, p2, cir.center, -offx, -offy);

        } 
         public void GetKBLine(out double k, out double b,Point p1, Point p2)
         {
             k=(p2.y-p1.y)/(p2.x-p1.x);
             b=p1.y-k*p1.x;
         }
    }
}
