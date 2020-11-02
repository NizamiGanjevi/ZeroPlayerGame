using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public Form1()
        {
            InitializeComponent();
            //создаем матрицу
            string[,] matrix = new string[30, 30];

            //создаем грид
            MainGrid = new MatrixGrid() { Parent = panel1, Dock = DockStyle.Fill };

            //задаем размер грида
            MainGrid.GridSize = new Size(matrix.GetLength(0), matrix.GetLength(1));

            //присваиваем событие в котором будем отдавать текст ячейки и ее цвет
            MainGrid.CellNeeded += (o, e) =>
            {
                e.Value = matrix[e.Cell.X, e.Cell.Y];
                e.BackColor = Color.White;
            };

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!MainGrid.IsAlive)
            {
                MainGrid.IsAlive = true;
            }
            else
            {
                MainGrid.IsAlive = false;
            }
            
        }       
    }
}
