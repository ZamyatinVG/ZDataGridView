﻿using System;
using System.Drawing;

namespace ZDGV
{
    class ColumnFilterClickedEventArg : EventArgs
    {
        public int ColumnIndex { get; private set; }
        public Rectangle ButtonRectangle { get; private set; }
        public ColumnFilterClickedEventArg(int colIndex, Rectangle btnRect)
        {
            this.ColumnIndex = colIndex;
            this.ButtonRectangle = btnRect;
        }
    }
}