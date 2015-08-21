using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace Recorder
{
    public partial class EditRecording : Form
    {
        private PointScreen ps;
        private List<string> tempList;
        string[] mouseActions = new[] { "LCU", "LCD", "RCU", "RCD" };

        public EditRecording(ConcurrentQueue<string> actionQueue )
        {
            tempList = actionQueue.ToList();
            InitializeComponent();
            bool firstMove = true;
            int count = 0;
            string moveAction = string.Empty;
            ps = new PointScreen();
            int testFirstXCoord, testFirstYCoord, testSecondXCoord, testSecondYCoord;
            testFirstXCoord = testFirstYCoord = testSecondXCoord = testSecondYCoord = 0;
            foreach(string action in tempList)
            {
                if (action.StartsWith("[") && action.EndsWith("];"))
                {
                    double seconds = Convert.ToInt64(action.Replace("];", "").Replace("[", ""));
                    seconds = TimeSpan.FromMilliseconds(seconds).TotalSeconds;
                    //ActionList.Items.Add(string.Format("Wait for {0} seconds", seconds));
                }
                else if (action.StartsWith("{") && action.EndsWith("};"))
                {
                    if (firstMove)
                    {
                        string tempAction = action.Replace("{", "").Replace("};", "");
                        testFirstXCoord = Convert.ToInt32(tempAction.Substring(0, tempAction.IndexOf(",")));
                        testFirstYCoord = Convert.ToInt32(tempAction.Substring(tempAction.IndexOf(",") + 2));
                        moveAction += string.Format("Move from X: {0} Y: {1} to ",
                            tempAction.Substring(0, tempAction.IndexOf(",")),
                            tempAction.Substring(tempAction.IndexOf(",") + 2));
                        firstMove = false;
                    }
                }
                else if (mouseActions.Contains(action.Replace(";", "")))
                {
                    string lastMove = string.Empty;
                    if (tempList[count - 1].Contains("{"))
                    {
                        lastMove = tempList[count - 1];
                    }
                    else if (tempList[count - 2].Contains("{"))
                    {
                        lastMove = tempList[count - 2];
                    }

                    if (!string.IsNullOrEmpty(lastMove))
                    {
                        string tempAction = lastMove.Replace("{", "").Replace("};", "");
                        testSecondXCoord = Convert.ToInt32(tempAction.Substring(0, tempAction.IndexOf(",")));
                        testSecondYCoord = Convert.ToInt32(tempAction.Substring(tempAction.IndexOf(",") + 2));
                        moveAction += string.Format("X: {0} Y: {1}",
                            tempAction.Substring(0, tempAction.IndexOf(",")),
                            tempAction.Substring(tempAction.IndexOf(",") + 2));

                        if (testFirstXCoord != testSecondXCoord && testFirstYCoord != testSecondYCoord)
                        {
                            ActionList.Items.Add(moveAction);
                        }
                        testFirstXCoord = testSecondXCoord = testFirstYCoord = testSecondYCoord = 0;
                        moveAction = string.Empty;
                        firstMove = true;
                    }

                    if (action.Replace(";", "") == "LCU")
                    {
                        ActionList.Items.Add("Left Click");
                    }
                    else if (action.Replace(";", "") == "RCU")
                    {
                        ActionList.Items.Add("Right Click");
                    }
                }
                else if (action.StartsWith("(") && action.EndsWith(");"))
                {
                    ActionList.Items.Add(string.Format("Press the {0} key.",
                        ((VirtualKeyCode) Convert.ToInt32(action.Replace("(", "").Replace(");", "")))));
                }

                count++;
            }
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string [])e.Data.GetData(DataFormats.FileDrop, false);
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void ActionList_MouseEnter(object sender, EventArgs e)
        {
            //Point point = ActionList.PointToClient(Cursor.Position);
            //int index = ActionList.IndexFromPoint(point);
            //if (index < 0) return;
            //else
            //{
            //    string action = ActionList.Items[index].ToString();
            //    if (action.StartsWith("Move from"))
            //    {
            //        action = action.Replace("Move from ", "").Replace("to", "");
            //        string fromX = action.Substring(0, action.IndexOf("Y")).Trim();
            //        string fromY = action.Substring(action.IndexOf("Y"), action.LastIndexOf("X") - action.IndexOf("Y")).Trim();
            //        string toX = action.Substring(action.LastIndexOf("X"), action.LastIndexOf("Y") - action.LastIndexOf("X")).Trim();
            //        string toY = action.Substring(action.LastIndexOf("Y"), action.Length - action.LastIndexOf("Y"));
            //        try
            //        {
            //            int iFromX = Convert.ToInt32(fromX.Replace("X: ", ""));
            //            int iFromY = Convert.ToInt32(fromY.Replace("Y: ", ""));
            //            int iToX = Convert.ToInt32(toX.Replace("X: ", ""));
            //            int iToY = Convert.ToInt32(toY.Replace("Y: ", ""));
            //            ps.DrawMouseEventLine(iFromX, iFromY, iToX, iToY);
            //            ps.Show();
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex);
            //        }
            //    }
            //
            //}
        }

        private void ActionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string toX = string.Empty, toY = string.Empty;
            int index = ActionList.SelectedIndex, iToX = 0, iToY = 0;
            if (index < 0) return;
            else
            {
                string action = ActionList.Items[index].ToString();
                if (action.StartsWith("Move from"))
                {
                    action = action.Replace("Move from ", "").Replace("to", "");
                    string fromX = action.Substring(0, action.IndexOf("Y")).Trim();
                    string fromY = action.Substring(action.IndexOf("Y"), action.LastIndexOf("X") - action.IndexOf("Y")).Trim();
                    toX = action.Substring(action.LastIndexOf("X"), action.LastIndexOf("Y") - action.LastIndexOf("X")).Trim();
                    toY = action.Substring(action.LastIndexOf("Y"), action.Length - action.LastIndexOf("Y"));
                    try
                    {
                        int iFromX = Convert.ToInt32(fromX.Replace("X: ", ""));
                        int iFromY = Convert.ToInt32(fromY.Replace("Y: ", ""));
                        iToX = Convert.ToInt32(toX.Replace("X: ", ""));
                        iToY = Convert.ToInt32(toY.Replace("Y: ", ""));
                        ps.Hide();
                        ps.Clear();
                        ps.DrawMouseEventLine(iFromX, iFromY, iToX, iToY);
                        ps.Show();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else if (action == "Left Click" || action == "Right Click")
                {
                    string previousCoord = ActionList.Items[index - 1].ToString();
                    if (!previousCoord.Contains("Click"))
                    {
                        previousCoord = previousCoord.Replace("Move from ", "").Replace("to", "");
                        toX =
                            previousCoord.Substring(previousCoord.LastIndexOf("X"),
                                previousCoord.LastIndexOf("Y") - previousCoord.LastIndexOf("X")).Trim();
                        toY = previousCoord.Substring(previousCoord.LastIndexOf("Y"),
                            previousCoord.Length - previousCoord.LastIndexOf("Y"));
                        iToX = -100;
                        iToY = -100;
                        try
                        {
                            iToX = Convert.ToInt32(toX.Replace("X: ", ""));
                            iToY = Convert.ToInt32(toY.Replace("Y: ", ""));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    ps.Hide();
                    ps.Clear();
                    if (action == "Left Click")
                    {
                        if (iToX != -100 && iToY != -100)
                        {
                            ps.DrawMouseAction(iToX, iToY, "LC");
                            ps.Show();
                        }
                    }
                    else
                    {
                        if (iToX != -100 && iToY != -100)
                        {
                            ps.DrawMouseAction(iToX, iToY, "RC");
                            ps.Show();
                        }
                    }
                }

            }
        }

        private void EditRecording_FormClosing(object sender, FormClosingEventArgs e)
        {
            ps.Hide();
            ps.Close();
        }
    }
}
