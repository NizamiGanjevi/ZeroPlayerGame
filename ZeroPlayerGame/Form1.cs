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

namespace ZeroPlayerGame
{
    public partial class Form1 : Form
    {
        private bool IsAlive = false;
        private MatrixGrid MainGrid;
        private Form Instance;
        DateTime LifeTime = new DateTime();

        public Form1()
        {
            InitializeComponent();
            Instance = this;
            stepCounter.Text = "Step is: 0";
            //создаем матрицу
            string[,] matrix = new string[30, 30];

            //создаем грид
            MainGrid = new MatrixGrid(new Size(matrix.GetLength(0), matrix.GetLength(1))) { Parent = panel1, Dock = DockStyle.Fill };

            //присваиваем событие в котором будем отдавать текст ячейки и ее цвет
            MainGrid.CellNeeded += (o, e) =>
            {
                e.Value = matrix[e.Cell.X, e.Cell.Y];
                e.BackColor = Color.White;
            };
            MainGrid.WorldStep += WorldUpdate;

        }
        private void WorldUpdate(int step)
        {
            Instance.Invoke((MethodInvoker)delegate
            {
                stepCounter.Text = string.Format("Step is: {0}", step.ToString());
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SwitchWorldState();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (MainGrid.IsAlive)
            {
                SwitchWorldState();
            }
            MainGrid.ResetGrid();
            Refresh();
        }

        private bool SwitchWorldState()
        {
            if (!MainGrid.IsAlive)
            {
                resetButton.Enabled = false;
                MainGrid.IsAlive = true;
                LifeTimer.Enabled = true;
            }
            else
            {
                resetButton.Enabled = true;
                MainGrid.IsAlive = false;
                LifeTimer.Enabled = false;
            }
            return MainGrid.IsAlive;
        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            LifeTime = LifeTime.AddMilliseconds(100);
            timeLabel.Text = LifeTime.TimeOfDay.ToString();
        }
    }
}
