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
        public bool IsAlive { get; set; } = false;
        public Point HoveredCell = new Point(-1, -1);
        public Dictionary<int, Cell> Cells = new Dictionary<int, Cell>();
        private int MaxNeighbors = 3;
        private int MinNeighbors = 2;
        private int BornNeighbors = 3;

        public event EventHandler<CellNeededEventArgs> CellNeeded;

        public MatrixGrid()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnCreateControl()
        {
            
            for (int j = 0; j < GridSize.Height; j++)
                for (int i = 0; i < GridSize.Width; i++)
                {
                    int index = i + j * GridSize.Width;
                    Cells.Add(index, new Cell(i, j, false));
                }
            Task.Run(() =>
            {
                while (true)
                {
                    while (IsAlive)
                    {
                        Start();
                    }
                    Task.Delay(500);
                }
            });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var gr = e.Graphics;
            gr.SmoothingMode = SmoothingMode.HighQuality;

            if (CellNeeded == null)
                return;

            var cw = ClientSize.Width / GridSize.Width;
            var ch = ClientSize.Height / GridSize.Height;

            foreach (Cell cell in Cells.Values)
            {
                var point = new Point(cell.x, cell.y);

                //получаем значение ячейки от пользователя
                var ea = new CellNeededEventArgs(point);
                CellNeeded(this, ea);

                //рисуем ячейку
                var rect = new Rectangle(cw * cell.x, ch * cell.y, cw, ch);
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

        protected virtual void OnCellClick(CellClickEventArgs cellClickEventArgs)
        {
            CellClick(this, cellClickEventArgs);
        }

        private void CellClick(MatrixGrid matrixGrid, CellClickEventArgs cellClickEventArgs)
        {
            int index = cellClickEventArgs.Cell.X + (cellClickEventArgs.Cell.Y * GridSize.Width);
            Cells[index].SwitchStatus();
        }

        private void Start()
        {
            var CTS = new CancellationTokenSource();

            var task1 = Task.Run(() => CheckDesk(), CTS.Token);
            Task.WhenAll(task1).Wait();
        }

        private async Task<bool> CheckDesk()
        {
            await Task.Run(() =>
            {
                foreach(Cell cell in Cells.Values)
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

                }
                
            });
            return true;
        }

        private int GetNeighborsCount(int x, int y)
        {
            int result = 0;
            int[][] offsets = new int[][] { new int[] { 1, 1 }, new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { 1, -1 }, new int[] { -1, 1 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { -1, -1 } };
            foreach (int[] offset in offsets)
            {
                int index = x - offset[0] + ((y - offset[1]) * GridSize.Width);
                try
                {
                    if (Cells[index].IsAlive)
                        result++;
                }
                catch (KeyNotFoundException)
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
