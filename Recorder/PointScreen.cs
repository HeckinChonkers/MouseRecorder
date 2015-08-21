using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public partial class PointScreen : Form
    {
        private Point currentPoint, destPoint;
        private bool drawLine = false, bDrawAction = false, clearScreen = false;
        private string drawAction = string.Empty;

        public PointScreen()
        {
            TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LightGreen;
            TransparencyKey = Color.LightGreen;
            this.Size = Screen.PrimaryScreen.Bounds.Size;

            currentPoint = new Point();
            destPoint = new Point();

            Paint += new PaintEventHandler(PointScreen_Paint);
        }

        public void DrawMouseEventLine(int startXCoord, int startYCoord, int endXCoord, int endYCoord)
        {
            currentPoint.X = startXCoord;
            currentPoint.Y = startYCoord;
            destPoint.X = endXCoord;
            destPoint.Y = endYCoord;
            drawLine = true;
        }

        public void DrawMouseAction(int xCoord, int yCoord, string action)
        {
            destPoint.X = xCoord;
            destPoint.Y = yCoord;
            drawAction = action;
            bDrawAction = true;
        }

        public void Clear()
        {
            clearScreen = true;
        }

        private void PointScreen_Paint(object sender, PaintEventArgs e)
        {
            Pen myPen = new Pen(Color.Red, 5);

            if (clearScreen)
            {
                e.Graphics.Clear(TransparencyKey);
                clearScreen = false;
            }
            
            if (drawLine)
            {
                e.Graphics.DrawEllipse(myPen, currentPoint.X - 5, currentPoint.Y - 5, 10, 10);
                e.Graphics.DrawEllipse(myPen, destPoint.X - 5, destPoint.Y - 5, 10, 10);
                e.Graphics.DrawLine(myPen, currentPoint, destPoint);
                drawLine = false;
            }
            else if (bDrawAction)
            {
                e.Graphics.DrawEllipse(myPen, destPoint.X - 15, destPoint.Y - 15, 30, 30);
                e.Graphics.DrawEllipse(myPen, destPoint.X - 5, destPoint.Y - 5, 10, 10);
                
                //e.Graphics.DrawString(drawAction, new Font("Arial", 12), new SolidBrush(Color.Green), destPoint.X - 13, destPoint.Y - 8);
            }

        }
    }
}
