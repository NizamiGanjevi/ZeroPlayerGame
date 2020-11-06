using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;

namespace ZeroPlayerGame
{
    public partial class MatrixGrid : UserControl
    {
        public Size GridSize { get; set; }
        private int Step = 0;
        private MatrixGrid Instance;
        public bool IsAlive { get; set; } = false;
        public Point HoveredCell = new Point(-1, -1);
        public Cell[,] Cells;
        private int MaxNeighbors = 3;
        private int MinNeighbors = 2;
        private int BornNeighbors = 3;

        public event EventHandler<CellNeededEventArgs> CellNeeded;
        public delegate void WorldStepHandler(int step);
        public event WorldStepHandler WorldStep;

        public MatrixGrid(Size size)
        {
            InitializeComponent();
            GridSize = size;
            Instance = this;
            Cells = GetCells();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Task.Run(() =>
            {
                while (true)
                {
                    while (IsAlive)
                    {
                        Start();
                        RefreshMap();
                        WorldStep?.Invoke(Step++);
                    }
                    Task.Delay(500);
                }
            });
        }

        public void ResetGrid()
        {
            Cells = GetCells();
            WorldStep?.Invoke(Step = 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var gr = e.Graphics;
            gr.SmoothingMode = SmoothingMode.HighQuality;

            if (CellNeeded == null)
                return;

            var cw = ClientSize.Width / GridSize.Width;
            var ch = ClientSize.Height / GridSize.Height;

            for (int i = 0; i < GridSize.Width; i++)
            {
                for (int j = 0; j < GridSize.Height; j++)
                {
                    var cell = Cells[i, j];
                    var x = cell.x;
                    var y = cell.y;
                    var point = new Point(x, y);

                    //получаем значение ячейки от пользователя
                    var ea = new CellNeededEventArgs(point);
                    CellNeeded(this, ea);

                    //рисуем ячейку
                    var rect = new Rectangle(cw * x, ch * y, cw, ch);
                    rect.Inflate(-1, -1);

                    if (point == HoveredCell)
                        gr.DrawRectangle(Pens.Red, rect);

                    //фон
                    if (cell.IsAlive)
                        using (var brush = new SolidBrush(Color.Black))
                            gr.FillRectangle(brush, rect);
                    else
                    {
                        using (var brush = new SolidBrush(Color.White))
                            gr.FillRectangle(brush, rect);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!IsAlive)
            {
                base.OnMouseMove(e);
                var cell = PointToCell(e.Location);
                HoveredCell = cell;
                Invalidate();
            }            
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!IsAlive)
            {
                base.OnMouseDown(e);
                if (e.Button == MouseButtons.Left)
                {
                    var cell = PointToCell(e.Location);
                    OnCellClick(new CellClickEventArgs(cell));
                    HoveredCell = cell;
                }
            }            
        }

        private Cell[,] GetCells()
        {
            Cell[,] cells = new Cell[GridSize.Width, GridSize.Height];
            for (int j = 0; j < GridSize.Height; j++)
                for (int i = 0; i < GridSize.Width; i++)
                {
                    cells[i, j] = new Cell(Instance, i, j, false);
                }
            return cells;
        }

        protected virtual void OnCellClick(CellClickEventArgs cellClickEventArgs)
        {
            CellClick(this, cellClickEventArgs);
        }

        private void CellClick(MatrixGrid matrixGrid, CellClickEventArgs cellClickEventArgs)
        {
            int index = cellClickEventArgs.Cell.X + (cellClickEventArgs.Cell.Y * GridSize.Width);
            Cells[cellClickEventArgs.Cell.X, cellClickEventArgs.Cell.Y].SwitchStatus();
        }

        private void Start()
        {
            var CTS = new CancellationTokenSource();
            int deviderX = GridSize.Width / 2;
            int deviderY = GridSize.Height / 2;

            var task1 = Task.Run(() => CheckDesk(0, deviderX, 0, deviderY), CTS.Token);
            var task2 = Task.Run(() => CheckDesk(deviderX, GridSize.Width, 0, deviderY), CTS.Token);
            var task3 = Task.Run(() => CheckDesk(0, deviderX, deviderY, GridSize.Height), CTS.Token);
            var task4 = Task.Run(() => CheckDesk(deviderX, GridSize.Width, deviderY, GridSize.Height), CTS.Token);

            Task.WhenAll(task1, task2, task3, task4).Wait();
        }

        private async Task<bool> CheckDesk(int fromX, int toX, int fromY, int toY)
        {
            await Task.Run(() =>
            {
                for (int i = fromX; i < toX; i++)
                {
                    for (int j = fromY; j < toY; j++)
                    {
                        int nbCount = GetNeighborsCount(Cells[i, j].x, Cells[i, j].y);
                        if (Cells[i, j].IsAlive)
                        {
                            if (nbCount < MinNeighbors || nbCount > MaxNeighbors)
                                Cells[i, j].SwitchStatus(false);
                        }
                        else
                        {
                            if (nbCount == BornNeighbors)
                                Cells[i, j].SwitchStatus(false);
                        }
                    }
                    
                }
                /*
                foreach (Cell cell in Cells)
                {
                    int nbCount = GetNeighborsCount(cell.x, cell.y);
                    if (cell.IsAlive)
                    {
                        if (nbCount < MinNeighbors || nbCount > MaxNeighbors)
                        cell.SwitchStatus();
                    }
                    else
                    {
                        if (nbCount == BornNeighbors)
                        cell.SwitchStatus();
                    }


                /*
                 * */           

            });
            return true;
        }

        private void RefreshMap()
        {
            Instance.Invoke((MethodInvoker)delegate
            {
                Refresh();
            });
        }

        private int GetNeighborsCount(int x, int y)
        {
            int result = 0;
            int i = 0;
            int[] range = new int[] { 1, -1, -1, 0, -1, 1, 0, 1, 1 };
            while (i < 8)
            {
                int k = i + 1;
                int xNb = x + range[i];
                int yNb = y + range[k];
                i++;
                try
                {
                    if (Cells[xNb, yNb].IsAlive)
                        result++;
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }               
                
            }
            return result;
        }
            

        Point PointToCell(Point p)
        {
            var cw = ClientSize.Width / GridSize.Width;
            var ch = ClientSize.Height / GridSize.Height;
            return new Point(p.X / cw, p.Y / ch);
        }

        public class CellNeededEventArgs : EventArgs
        {
            public Point Cell { get; private set; }
            public string Value { get; set; }
            public Color BackColor { get; set; }

            public CellNeededEventArgs(Point cell)
            {
                Cell = cell;
            }
        }

        public class CellClickEventArgs : EventArgs
        {
            public Point Cell { get; private set; }

            public CellClickEventArgs(Point cell)
            {
                Cell = cell;
            }
        }
    }
}
