using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroPlayerGame
{
    public class Cell
    {
        public bool IsAlive { get; private set; } = false;
        public bool newState { get; set; } = false;
        public int x { get; private set; }
        public int y { get; private set; }

        private int currentStep { get; set; } = 0;

        public Cell(MatrixGrid mainGrid, int x, int y, bool status)
        {
            IsAlive = status;
            this.x = x;
            this.y = y;
            mainGrid.WorldStep += StepHandler;
        }

        public bool SwitchStatus(bool now = true)
        {
            if (IsAlive)
                newState = false;
            else
                newState = true;
            UpdateState(now);
            return IsAlive;
        }

        private void StepHandler(int step)
        {
            currentStep = step;
            UpdateState(true);
        }

        private void UpdateState(bool now)
        {
            if (now)
                IsAlive = newState;
        }

    }
}
