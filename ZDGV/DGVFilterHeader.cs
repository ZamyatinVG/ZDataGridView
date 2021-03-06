﻿using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;

namespace ZDGV
{
    class DGVFilterHeader : DataGridViewColumnHeaderCell
    {
        public ComboBoxState currentState = ComboBoxState.Normal;
        Point cellLocation;
        Rectangle buttonRect;
        public event EventHandler<ColumnFilterClickedEventArg> FilterButtonClicked;
        protected override void Paint(Graphics graphics,
                                      Rectangle clipBounds,
                                      Rectangle cellBounds,
                                      int rowIndex,
                                      DataGridViewElementStates dataGridViewElementState,
                                      object value,
                                      object formattedValue,
                                      string errorText,
                                      DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds,
                       cellBounds, rowIndex,
                       dataGridViewElementState, value,
                       formattedValue, errorText,
                       cellStyle, advancedBorderStyle, paintParts);

            int width = 20; // 20 px
            buttonRect = new Rectangle(cellBounds.X + cellBounds.Width - width, cellBounds.Y + 2, width, cellBounds.Height - 3);
            cellLocation = cellBounds.Location;
            if (ComboBoxRenderer.IsSupported)
                ComboBoxRenderer.DrawDropDownButton(graphics, buttonRect, currentState);
            else
                graphics.DrawImage(Properties.Resources.downarrow, buttonRect);
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (this.IsMouseOverButton(e.Location))
                currentState = ComboBoxState.Pressed;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (this.IsMouseOverButton(e.Location))
            {
                currentState = ComboBoxState.Normal;
                this.OnFilterButtonClicked();
            }
            base.OnMouseUp(e);
        }

        private bool IsMouseOverButton(Point e)
        {
            Point p = new Point(e.X + cellLocation.X, e.Y + cellLocation.Y);
            if (p.X >= buttonRect.X && p.X <= buttonRect.X + buttonRect.Width && p.Y >= buttonRect.Y && p.Y <= buttonRect.Y + buttonRect.Height)
                return true;
            return false;
        }

        protected virtual void OnFilterButtonClicked()
        {
            this.FilterButtonClicked?.Invoke(this, new ColumnFilterClickedEventArg(this.ColumnIndex, this.buttonRect));
        }
    }
}