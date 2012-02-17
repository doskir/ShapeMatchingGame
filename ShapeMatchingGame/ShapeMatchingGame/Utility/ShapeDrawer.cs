using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace ShapeMatchingGame.Utility
{
    class ShapeDrawer
    {
        public void DrawShapes()
        {
            DrawShape("triangle", 3, Color.White,0);
            DrawShape("rotatedRectangle", 4, Color.White, 0);            
            DrawShape("pentagon", 5, Color.White,0);
            DrawShape("hexagon", 6, Color.White,0);
            DrawShape("heptagon", 7, Color.White, 0);
            DrawShape("rectangle", 4, Color.White, 0);
            DrawShape("circle", 1000, Color.White, 0);
            DrawPolygon("star", Calculate5StarPoints(new PointF(24, 24), 20, 10).ToList(), Color.White);
        }
        public void DrawPolygon(string name,List<PointF> points,Color color)
        {
            Bitmap bmp = new Bitmap(50, 50);
            Graphics g = Graphics.FromImage(bmp);
            g.FillPolygon(new SolidBrush(color), points.ToArray());
            bmp.Save(name + ".png");
            bmp.Dispose();
        }
        public void DrawShape(string name, int edges, Color color,int additionalRotationInDegrees)
        {
            List<PointF> points = new List<PointF>();
            Vector2 origin = new Vector2(24, 24);
            Vector2 positionVector = new Vector2(0, -20);
            for (int i = 0; i < edges; i++)
            {
                Vector2 point = Vector2.Transform(positionVector, Matrix.CreateRotationZ(MathHelper.ToRadians(360.0f / edges * i + additionalRotationInDegrees)));
                point += origin;
                points.Add(new PointF(point.X, point.Y));
            }
            DrawPolygon(name, points, color);
        }

        //Copied from http://www.daniweb.com/software-development/csharp/code/360165
        private PointF[] Calculate5StarPoints(PointF Orig, float outerradius, float innerradius)
        {
            // Define some variables to avoid as much calculations as possible
            // conversions to radians
            double Ang36 = Math.PI / 5.0; // 36° x PI/180
            double Ang72 = 2.0 * Ang36; // 72° x PI/180
            // some sine and cosine values we need
            float Sin36 = (float)Math.Sin(Ang36);
            float Sin72 = (float)Math.Sin(Ang72);
            float Cos36 = (float)Math.Cos(Ang36);
            float Cos72 = (float)Math.Cos(Ang72);
            // Fill array with 10 origin points
            PointF[] pnts = { Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig };
            pnts[0].Y -= outerradius; // top off the star, or on a clock this is 12:00 or 0:00 hours
            pnts[1].X += innerradius * Sin36; pnts[1].Y -= innerradius * Cos36; // 0:06 hours
            pnts[2].X += outerradius * Sin72; pnts[2].Y -= outerradius * Cos72; // 0:12 hours
            pnts[3].X += innerradius * Sin72; pnts[3].Y += innerradius * Cos72; // 0:18
            pnts[4].X += outerradius * Sin36; pnts[4].Y += outerradius * Cos36; // 0:24
            // Phew! Glad I got that trig working.
            pnts[5].Y += innerradius;
            // I use the symmetry of the star figure here
            pnts[6].X += pnts[6].X - pnts[4].X; pnts[6].Y = pnts[4].Y; // mirror point
            pnts[7].X += pnts[7].X - pnts[3].X; pnts[7].Y = pnts[3].Y; // mirror point
            pnts[8].X += pnts[8].X - pnts[2].X; pnts[8].Y = pnts[2].Y; // mirror point
            pnts[9].X += pnts[9].X - pnts[1].X; pnts[9].Y = pnts[1].Y; // mirror point
            return pnts;
        }
    }
}
