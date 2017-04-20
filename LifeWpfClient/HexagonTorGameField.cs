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
    public class HexagonTorGameFieldFrameworkElement : GameFieldFrameworkElement
    {
        private double FCosOfPiDiv6 = Math.Cos(Math.PI / 6);

        private HexagonCellTorGameField FHexagonGameField;
        private HexagonCellTorGameField HexagonGameField
        {
            get
            {
                if (this.FHexagonGameField == null)
                    this.FHexagonGameField = (HexagonCellTorGameField)this.FGameField;
                return this.FHexagonGameField;
            }
        }

        public int GameWidth
        {
            get { return this.HexagonGameField.GameWidth; }
        }

        public int GameHeight
        {
            get { return this.HexagonGameField.GameHeight; }
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
                    this.FCellSize = this.CellWidth / (2 * this.FCosOfPiDiv6);
                return this.FCellSize;
            }
        }

        private double FCellWidth;
        private double CellWidth
        {
            get
            {
                if (this.FCellWidth == 0)
                    this.FCellWidth = this.ItemSize + this.ItemMargin * 2;
                return this.FCellWidth;
            }
        }

        public HexagonTorGameFieldFrameworkElement(HexagonCellTorGameField aGameField, int itemSize) :
            base(aGameField, itemSize)
        {
            this.Width = this.CellWidth * this.GameWidth;
            this.Height = 3 * this.CellSize * this.GameHeight / 2;

            this.FPointIndexConverter = 
                new HexagonPointIndexConverter(this.GameWidth, this.CellWidth, this.CellSize, this.FGameField.Count);
			this.DrawGrid();
			//this.DrawItems();
		}

        protected override void DrawGrid()
        {
            base.DrawGrid();

            Brush _brush = Brushes.Gray;
            double _strokeThickness = 0.2;
            DoubleCollection _dashCollection = 
                new DoubleCollection { this.CellSize / _strokeThickness, 2 * this.CellSize / _strokeThickness };
            Pen _pen = new Pen(_brush, _strokeThickness);
            _pen.DashStyle = new DashStyle(_dashCollection, 0);
            
            double _gameFieldHeight = 3 * this.GameHeight * this.CellSize / 2;
            double _gameFieldWidth = this.GameWidth * this.CellWidth;
            
            DrawingVisual _gridVisual = new DrawingVisual();
            using (DrawingContext _context = _gridVisual.RenderOpen())
            {
                for (int i = 0; i <= 2 * this.GameWidth; i++)
                {
                    double _x1 = i * this.CellWidth / 2 + this.Margin.Left;
                    double _y1 = this.Margin.Top;
                    double _x2 = _x1;
                    double _y2 = _gameFieldHeight + this.Margin.Top;

                    if (i % 2 == 0)
                        _y1 += 3 * this.CellSize / 2;

                    _context.DrawLine(_pen, new Point(_x1, _y1), new Point(_x2, _y2));
                }

                double _h = _gameFieldWidth / (this.FCosOfPiDiv6 * 2);
                int _maxIndex = (int)(3 * this.GameHeight / 2 + this.GameWidth);
                for (int i = 0; i <= _maxIndex; i++)
                {
                    double _y = i * this.CellSize + this.CellSize / 2;
                    double _y1 = Math.Min(_y, _gameFieldHeight) + this.Margin.Top;
                    double _x1 = Math.Max(0, 2 * (_y - _gameFieldHeight) * this.FCosOfPiDiv6) + this.Margin.Left;
                    double _y2 = Math.Max(0, _y - _h) + this.Margin.Top;
                    double _x2 = Math.Min(2 * _y * this.FCosOfPiDiv6, _gameFieldWidth) + this.Margin.Left;

                    if (i < 3 * GameHeight / 2)
                    {
                        if (i % 3 == 2)
                        {
                            _x1 += this.CellWidth;
                            _y1 -= this.CellSize;
                        }

                        if (i % 3 == 0)
                        {
                            _x1 += this.CellWidth / 2;
                            _y1 -= this.CellSize / 2;
                        }
                    }

                    _context.DrawLine(_pen, new Point(_x1, _y1), new Point(_x2, _y2));

                    double _center = _gameFieldWidth / 2 + this.Margin.Left;
                    double _x1s = this.SimmetricTransformX(_x1, _center);
                    double _x2s = this.SimmetricTransformX(_x2, _center);

                    _context.DrawLine(_pen, new Point(_x1s, _y1), new Point(_x2s, _y2));
                }
            }

            this.FGridVisuals.Add(_gridVisual);
        }

        private double SimmetricTransformX(double aX, double aCenter)
        {
            return 2 * aCenter - aX;
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
                    "Hex Tor", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, _tf, 14, Brushes.Orange);
                _context.DrawText(_ft, new Point(10, 24));
            }
            this.FHeaderVisual.Add(_visual);
        }
    }

    class HexagonPointIndexConverter : IPointIndexConverter
    {

        public HexagonPointIndexConverter(
            int aGameWidth, double aCellWidth, double aCellSize, int aCellCount)
        {
            this.FGameWidth = aGameWidth;
            this.FCellWidth = aCellWidth;
            this.FHalfCellSize = aCellSize / 2;
            this.FHalfCellWidth = this.FCellWidth / 2;
            this.FCellCount = aCellCount;
        }

        private int FGameWidth;
        private double FCellWidth;
        private double FHalfCellSize;
        private double FHalfCellWidth;
        private int FCellCount;

        public Point ConvertCellIndexToCenter(int aIndex)
        {
            int _cellX = aIndex % this.FGameWidth;
            int _cellY = aIndex / this.FGameWidth;

            double _x = _cellX * this.FCellWidth + this.FHalfCellWidth;
            double _y = _cellY * (3 * this.FHalfCellSize) + this.FHalfCellSize;


            if (_cellY % 2 == 0)
                _x += this.FHalfCellWidth;

            return new Point(_x, _y);
        }

        public int ConvertPositionToIndex(Point aPosition)
        {
            double _dist = Double.MaxValue;
            int _index = 0;
            while (_index < this.FCellCount)
            {
                Point _center = this.ConvertCellIndexToCenter(_index);
                _dist = Point.Subtract(aPosition, _center).Length;

                if (_dist <= this.FCellWidth / 2)
                    return _index;

                _index++;
            }

            return -1;
        }

        public int ConvertCellCenterToIndex(Point aCenter)
        {
            throw new NotImplementedException();
        }
    }
}
