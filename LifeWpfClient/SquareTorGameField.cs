using Life;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace LifeWpfClient
{
    public class SquareTorGameFieldFrameworkElement : GameFieldFrameworkElement
    {
        private int FGameWidth = 10;
        public int GameWidth
        {
            get { return this.FGameWidth; }
            set { this.FGameWidth = value; }
        }

        private int FGameHeight = 10;
        public int GameHeight
        {
            get { return this.FGameHeight; }
            set { this.FGameHeight = value; }
        }

        private double FItemMargin = 2;
        public double ItemMargin
        {
            get { return this.FItemMargin; }
            set { this.FItemMargin = value; }
        }

        private double FCellSize;
        private double CellSize
        {
            get
            {
                if (this.FCellSize == 0)
                    this.FCellSize = this.ItemSize + this.ItemMargin * 2;
                return this.FCellSize;
            }
        }

        public SquareTorGameFieldFrameworkElement(SquareCellTorGameField aGameField, int itemSize) : 
            base(aGameField, itemSize)
        {
            this.FGameWidth = aGameField.GameWidth;
            this.FGameHeight = aGameField.GameHeight;

            this.Width = this.CellSize * this.GameWidth;
            this.Height = this.CellSize * this.GameHeight;
            this.FPointIndexConverter = new SquarePointIndexConverter(this.GameWidth, this.CellSize);
			this.DrawGrid();
			//this.DrawItems();
		}

        protected override void DrawGrid()
        {
            base.DrawGrid();

            Brush _brush = Brushes.Gray;
            double _strokeThickness = 0.2;
            Pen _pen = new Pen(_brush, _strokeThickness);

            DrawingVisual _gridVisual = new DrawingVisual();
            using (DrawingContext _gridContext = _gridVisual.RenderOpen())
            {
                for (int i = 0; i <= this.GameWidth; i++)
                {
                    double _x1 = i * this.CellSize + this.Margin.Left;
                    double _y1 = this.Margin.Top;
                    double _x2 = _x1;
                    double _y2 = this.GameHeight * this.CellSize + this.Margin.Top;

                    _gridContext.DrawLine(_pen, new Point(_x1, _y1), new Point(_x2, _y2));
                }

                for (int i = 0; i <= this.GameHeight; i++)
                {
                    double _x1 = this.Margin.Left;
                    double _y1 = i * this.CellSize + this.Margin.Top;
                    double _x2 = this.GameWidth * this.CellSize + this.Margin.Left;
                    double _y2 = _y1;

                    _gridContext.DrawLine(_pen, new Point(_x1, _y1), new Point(_x2, _y2));
                }
            }

            this.FGridVisuals.Add(_gridVisual);
        }

        public override void DrawHeader(string aHeader)
        {
            base.DrawHeader(aHeader);
            DrawingVisual _visual = new DrawingVisual();
            using (DrawingContext _context = _visual.RenderOpen())
            {
                FontFamily _ff = new FontFamily("Arial");
                Typeface _tf = new Typeface(_ff, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal, _ff);
                FormattedText _ft = new FormattedText(
                    "Square Tor", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, _tf, 14, Brushes.Orange);
                _context.DrawText(_ft, new Point(10, 24));
            }
            this.FHeaderVisual.Add(_visual);
        }
    }

    class SquarePointIndexConverter : IPointIndexConverter
    {
        public SquarePointIndexConverter(int aGameWidth, double aCellSize)
        {
            this.FGameWidth = aGameWidth;
            this.FCellSize = aCellSize;
            this.FHalfCellWidth = this.FCellSize / 2;
        }

        private int FGameWidth;
        private double FCellSize;
        private double FHalfCellWidth;

        public Point ConvertCellIndexToCenter(int aIndex)
        {
            int _cellX = aIndex % this.FGameWidth;
            int _cellY = aIndex / this.FGameWidth;
            double _x = _cellX * this.FCellSize + this.FHalfCellWidth;
            double _y = _cellY * this.FCellSize + this.FHalfCellWidth;

            return new Point(_x, _y);
        }

        public int ConvertPositionToIndex(Point aPosition)
        {
            int _cellX = (int)Math.Floor(aPosition.X / this.FCellSize);
            int _cellY = (int)Math.Floor(aPosition.Y / this.FCellSize);

            return _cellY * this.FGameWidth + _cellX;
        }

        public int ConvertCellCenterToIndex(Point aCenter)
        {
            throw new NotImplementedException();
        }
    }
}
