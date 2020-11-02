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
        public Form1()
        {
            InitializeComponent();
            //создаем матрицу
            string[,] matrix = new string[30, 30];

            //создаем грид
            var grid = new MatrixGrid() { Parent = panel1, Dock = DockStyle.Fill };

            //задаем размер грида
            grid.GridSize = new Size(matrix.GetLength(0), matrix.GetLength(1));

            //присваиваем событие в котором будем отдавать текст ячейки и ее цвет
            grid.CellNeeded += (o, e) =>
            {
                e.Value = matrix[e.Cell.X, e.Cell.Y];
                e.BackColor = Color.White;
            };

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsAlive)
            {
                IsAlive = true;
                Run();
            }
            else
            {
                IsAlive = false;
            }
            
        }

        private void Run()
        {
            while (IsAlive)
            {

            }
        }

        private void CheckDesk()
        {

        }
    }
}
