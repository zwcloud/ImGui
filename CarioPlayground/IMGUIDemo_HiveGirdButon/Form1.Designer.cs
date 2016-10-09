using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ImGui;

namespace ImGuiDemo_HiveGirdButon
{
    partial class Form1
    {
        class Polygon
        {
            public Point[] Points;

            public Point Center;

            private const double sqrt3 = 1.732050807568877;
            private static Point[] unitPolygon = new Point[]
            {
                new Point(0,1),
                new Point(sqrt3/2,0.5),
                new Point(sqrt3/2,-0.5),
                new Point(0,-1),
                new Point(-sqrt3/2,-0.5),
                new Point(-sqrt3/2,0.5),
                new Point(0,1),
            };

            static public Polygon UnitPolygon
            {
                get
                {
                    var polygon = new Polygon();
                    polygon.Points = unitPolygon.Clone() as Point[];
                    polygon.Center = new Point();
                    return polygon;
                }
            }

            public void Translate(double x, double y)
            {
                Center.Offset(x, y);
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i].Offset(x, y);
                }
            }
            public void Scale(double scale)
            {
                Center.X *= scale;
                Center.Y *= scale;
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i].X *= scale;
                    Points[i].Y *= scale;
                }
            }

            private static double unitX = sqrt3 / 2;
            private static double unitY = 1.5;

            internal static Polygon[] GeneratePolygons(double firstX, double firstY, double edgeSize, int row, int column)
            {
                List<Polygon> polygons = new List<Polygon>();
                for (int y = 0; y < row; y++)
                {
                    for (int x = 0; x < column; x++)
                    {
                        if (x % 2 == 0 && y % 2 == 0)
                        {
                            var cx = x * unitX * edgeSize;
                            var cy = y * unitY * edgeSize;
                            var polygon = new Polygon();
                            polygon.Points = Polygon.unitPolygon.Clone() as Point[];
                            polygon.Scale(edgeSize);
                            polygon.Translate(firstX + cx, firstY + cy);
                            polygons.Add(polygon);
                        }
                        else if (x % 2 == 1 && y % 2 == 0)
                        {
                            var cx = x * unitX * edgeSize;
                            var cy = (y + 1) * unitY * edgeSize;
                            var polygon = new Polygon();
                            polygon.Points = Polygon.unitPolygon.Clone() as Point[];
                            polygon.Scale(edgeSize);
                            polygon.Translate(firstX + cx, firstY + cy);
                            polygons.Add(polygon);
                        }
                    }
                }
                return polygons.ToArray();
            }
        }

        private Polygon myPolygon = InitPolygon();
        static Polygon InitPolygon()
        {
            var unitPolygon = Polygon.UnitPolygon;
            unitPolygon.Scale(100);
            unitPolygon.Translate(100, 100);
            return unitPolygon;
        }

        private Polygon[] myPolygons = Polygon.GeneratePolygons(40, 40, 40, 5, 10);

        protected override void OnGUI()
        {
            for (int i = 0; i < myPolygons.Length; ++i)
            {
                //GUI.PolygonButton(myPolygons[i].Points, myPolygons[i].Center.ToString(),
                //    "hellosixpoly" + i);
            }
        }
    }

}
