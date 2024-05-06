using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace KG_5
{
    public partial class Form1 : Form
    {
        float pictureBoxWidth;
        float pictureBoxHeight;
        Graphics g;
        Pen axisPen = new Pen(Color.Black, 2);
        Pen gridPen = new Pen(Color.Black, 0.5f);
        Font Fon = new Font("Arial", 9, FontStyle.Regular);
        Brush brush = new SolidBrush(Color.Black);
        Brush fillArea = new SolidBrush(Color.Red);
        float divX;
        float divY;
        const int countX = 20;
        const int countY = 20;
        float centerX, centerY;
        List<PointF> points = new List<PointF>();
        List<Section> sections = new List<Section>();
        List<List<int>> area = new List<List<int>>();
        List<Point> pointList = new List<Point>();
        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            pictureBoxWidth = pictureBox1.Width;
            pictureBoxHeight = pictureBox1.Height;
            divX = pictureBoxWidth / countX;
            divY = pictureBoxHeight / countY;
            centerX = pictureBoxWidth / 2;
            centerY = pictureBoxHeight / 2;
            for (int i = 0; i < countX + 10; i++)
            {
                area.Add(new List<int>());
                for (int j = 0; j < countY + 10; j++)
                {
                    area[i].Add(0);
                }
            }
        }

        private void inputFile()
        {
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("input.txt");
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    string[] coordinates = line.Split(' ');
                    float x = float.Parse(coordinates[0]);
                    float y = float.Parse(coordinates[1]);
                    points.Add(new PointF(x, y));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            for (int i = 0; i < points.Count; i++)
            {
                sections.Add(new Section(points[i], points[(i + 1) % points.Count]));
            }
        }
        private List<PointF> Convert(List<PointF> point)
        {
            List<PointF> result = new List<PointF>();
            for (int i = 0; i < point.Count; i++)
            {
                result.Add(Convert(point[i]));
            }
            return result;
        }
        private List<Point> Convert(List<Point> point)
        {
            List<Point> result = new List<Point>();
            for (int i = 0; i < point.Count; i++)
            {
                result.Add(Convert(point[i]));
            }
            return result;
        }
        private PointF Convert(PointF point)
        {
            return new PointF(divX + (point.X * divX), (pictureBoxHeight - divY) - point.Y * divY);
        }
        private Point Convert(Point point)
        {
            return new Point((int)(divX + (point.X * divX)), (int)((pictureBoxHeight - divY) - point.Y * divY));
        }
        private void drawAxis()
        {
            g.Clear(Color.White);
            PointF axisXStart = new PointF(divX, pictureBoxHeight - divY);
            PointF axisXEnd = new PointF(pictureBoxWidth, pictureBoxHeight - divY);
            PointF axisYStart = new PointF(divX, 0);
            PointF axisYEnd = new PointF(divX, pictureBoxHeight - divY);
            g.DrawLine(axisPen, axisXStart, axisXEnd);
            g.DrawLine(axisPen, axisYStart, axisYEnd);
            for (int i = 1; i <= countY; i++)
            {
                g.DrawString((i).ToString(), Fon, brush, divX - 17, pictureBoxHeight + divY * -i - divY);
            }
            for (int i = 1; i <= countX; i++)
            {
                g.DrawString(i.ToString(), Fon, brush, divX * i + 5, pictureBoxHeight - 15);
            }
        }
        private void drawGrid()
        {
            for (int i = 0; i <= countY; i++)
            {
                g.DrawLine(gridPen, 0, divY * i, pictureBoxWidth, divY * i);
            }
            for (int i = 0; i <= countX; i++)
            {
                g.DrawLine(gridPen, divX * i, 0, divX * i, pictureBoxHeight);
            }
        }
        private void drawBorder()
        {
            for (int i = 0; i < sections.Count; i++)
            {
                bool flag = false;
                List<PointF> tmp = Convert(sections[i].GetPixels());
                List<PointF> tmp2 = sections[i].GetPixels();
                Point min = new Point((int)tmp2[0].X, (int)tmp2[0].Y);
                Point max = new Point((int)tmp2[tmp2.Count - 1].X, (int)tmp2[tmp2.Count - 1].Y);
                if (max.Y == min.Y && tmp2.Count > 3)
                {
                    continue;
                }

                for (int j = 1; j < tmp.Count - 1; j++)
                {
                    area[(int)tmp2[j].X][(int)tmp2[j].Y] = 1;
                    g.FillRectangle(brush, tmp[j].X, tmp[j].Y, divX, divY);
                }
                if (min.Y > max.Y)
                {
                    Point a = min;
                    min = max;
                    max = a;
                }
                area[max.X][max.Y] = area[max.X][max.Y] ^ 1;
                for (int j = 1; j < tmp.Count; j++)
                {

                    if (tmp2[j].Y == tmp2[j - 1].Y)
                    {
                        if (tmp2[j].X >tmp2[j - 1].X)
                        {
                            area[(int)tmp2[j].X][(int)tmp2[j].Y] = 0;

                        }
                        else
                        {
                            area[(int)tmp2[j - 1].X][(int)tmp2[j - 1].Y] = 0;
                        }
                    }
                }
                if (area[max.X][max.Y] == 1 && max.X != 0)
                {
                    area[max.X - 1][max.Y] = 0;
                    area[max.X + 1][max.Y] = 0;
                }

            }
        }
        private void drawBorderBr()
        {
            for (int i = 0; i < sections.Count; i++)
            {
                bool flag = false;
                List<PointF> tmp = Convert(sections[i].GetPixels());
                List<PointF> tmp2 = sections[i].GetPixels();
                for (int j = 0; j < tmp.Count; j++)
                {
                    g.FillRectangle(brush, tmp[j].X, tmp[j].Y, divX, divY);
                }
            }
        }
        private void XOR()
        {
            bool condition = false;
            //for (int j = 1; j < countY + 10; j++)
            //{
            //    for (int i = 1; i < countX + 10; i++)
            //    {
            //        if (area[i][j] == 1 && area[i - 1][j] == 1)
            //        {
            //            if (area[i + 1][j] == 0)
            //            {
            //                area[i - 1][j] = 0;
            //            }
            //            else
            //            {
            //                while (area[i + 1][j] != 0)
            //                {
            //                    area[i][j] = 0;
            //                    i++;
            //                }
            //            }
            //        }
            //    }
            //}
            for (int j = 0; j < countY + 10; j++)
            {
                condition = false;
                for (int i = 1; i < countX + 9; i++)
                {
                    int border = 0;
                    if ((area[i][j] ^ area[i - 1][j]) == 1)
                    {
                        area[i][j] = 1;
                        condition = true;
                    }

                    else
                    {
                        condition = false;
                        area[i][j] = 0;
                    }
                }
            }
        }
        private void drawFillArea()
        {
            for (int i = 0; i < area.Count - 1; i++)
            {
                for (int j = 0; j < area[i].Count - 1; j++)
                {
                    if (area[i][j] == 1)
                    {
                        pointList.Add(new Point(i, j));

                    }
                }
            }
            pointList = Convert(pointList);
            for (int i = 0; i < pointList.Count; i++)
            {
                g.FillRectangle(fillArea, pointList[i].X, pointList[i].Y, divX, divY);
            }
        }
        private void outputFile1(string a)
        {
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter("output" + a + ".txt");
                //Write a line of text
                for (int i = 0; i < area.Count; i++)
                {
                    for (int j = 0; j < area[i].Count; j++)
                    {
                        sw.Write(area[i][j]);
                    }
                    sw.Write('\n');
                }
                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        private void outputFile2()
        {
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter("output2.txt");
                //Write a line of text
                for (int i = 0; i < area.Count; i++)
                {
                    for (int j = 0; j < area[i].Count; j++)
                    {
                        sw.Write(area[i][j]);
                    }
                    sw.Write('\n');
                }
                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            inputFile();
            drawAxis();
            drawGrid();
            drawBorder();
            outputFile1("1");

            XOR();
            drawFillArea();
            drawBorderBr();
            //Section sec = new Section(new PointF(8, 8), new PointF(8, 1));
            //List<PointF> tmp = Convert(sec.GetPixels());
            //for (int i = 0; i < tmp.Count; i++)
            //{
            //    g.FillRectangle(brush, tmp[i].X, tmp[i].Y, divX, divY);
            //}
        }
    }
}
