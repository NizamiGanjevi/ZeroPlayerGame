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

namespace ZeroPlayerGame
{
    public partial class MatrixGrid : UserControl
    {
        public Size GridSize { get; set; }
        public bool IsAlive { get; set; }
        public Point HoveredCell = new Point(-1, -1);
        public Dictionary<int, Cell> Cells = new Dictionary<int, Cell>();

        public event EventHandler<CellNeededEventArgs> CellNeeded;

        public MatrixGrid()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnCreateControl()
        {
            int count = 0;
            for (int j = 0; j < GridSize.Height; j++)
                for (int i = 0; i < GridSize.Width; i++)
                {
                    Cells.Add(count, new Cell(i, j, false));
                    count++;
                }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var gr = e.Graphics;
            gr.SmoothingMode = SmoothingMode.HighQuality;

            if (CellNeeded == null)
                return;

            var cw = ClientSize.Width / GridSize.Width;
            var ch = ClientSize.Height / GridSize.Height;

            foreach (Cell cell in Cells)
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
            base.OnMouseMove(e);
            var cell = PointToCell(e.Location);
            HoveredCell = cell;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                var cell = PointToCell(e.Location);
                OnCellClick(new CellClickEventArgs(cell));
                HoveredCell = cell;
            }
        }

        protected virtual void OnCellClick(CellClickEventArgs cellClickEventArgs)
        {
            CellClick(this, cellClickEventArgs);
        }

        private void CellClick(MatrixGrid matrixGrid, CellClickEventArgs cellClickEventArgs)
        {
            Cells.
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
