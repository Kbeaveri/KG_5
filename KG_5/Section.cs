using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KG_5
{
    internal class Section
    {
        PointF P1;
        PointF P2;
        public
        float centeredX;
        float centeredY;
        List<PointF> pixels;
        public Section(PointF P1, PointF P2)
        {
            this.P1 = P1;
            this.P2 = P2;
            this.pixels = new List<PointF>();
            transformations();
        }

        private void transformations()
        {
            bool InvertX = false;
            bool InvertY = false;
            bool InvertXY = false;
            centered();
            if (P2.X < 0)
            {
                invertX();
                InvertX = true;
            }
            if (P2.Y < 0)
            {
                invertY();
                InvertY = true;
            }
            if (P2.X < P2.Y)
            {
                invertXY();
                InvertXY = true;
            }
            BZN();
            reverseTransformations(InvertX, InvertY,InvertXY);
        }
        private void reverseTransformations(bool InvertX, bool InvertY, bool InvertXY)
        {
            if (InvertXY)
            {
                invertXY();
            }
            if (InvertY)
            {
                invertY();
            }
            if (InvertX)
            {
                invertX();
            }
            reverseCentered();
        }
        private void BZN()
        {
            float x = P1.X, y = P1.Y;
            float delX = P2.X - P1.X;
            float delY = P2.Y - P1.Y;
            float e = 2 * delY - delX;
            for (int i = 0; i < delX; i++)
            {
                pixels.Add(new PointF(x, y));
                while (e >= 0)
                {
                    y += 1;
                    e = e - 2 * delX;
                }
                x += 1;
                e = e + 2 * delY;
            }
            pixels.Add(new PointF(P2.X, P2.Y));
        }
        private void centered()
        {
            centeredX = -P1.X;
            centeredY = -P1.Y;
            P1.X = 0;
            P1.Y = 0;
            P2.X += centeredX;
            P2.Y += centeredY;
        }
        void reverseCentered()
        {
            P1.X = -centeredX;
            P1.Y = -centeredY;
            P2.X -= centeredX;
            P2.Y -= centeredY;
            for (int i = 0; i < pixels.Count; i++)
            {
                pixels[i] = new PointF(pixels[i].X - centeredX, pixels[i].Y - centeredY);
            }
        }
        private void invertY()
        {
            P2.Y = -P2.Y;
            for (int i = 0; i < pixels.Count; i++)
            {
                pixels[i] = new PointF(pixels[i].X, -pixels[i].Y);
            }
        }
        private void invertX()
        {
            P2.X = -P2.X;
            for (int i = 0; i < pixels.Count; i++)
            {
                pixels[i] = new PointF(-pixels[i].X, pixels[i].Y);
            }
        }
        private void invertXY()
        {

            float tmp;
            tmp = P2.X;
            P2.X = P2.Y;
            P2.Y = tmp;
            for (int i = 0; i < pixels.Count; i++)
            {
                pixels[i] = new PointF(pixels[i].Y, pixels[i].X);
            }
        }
        public List<PointF> GetPixels()
        {
            return pixels;
        }
    }
}
