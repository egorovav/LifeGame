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
    public class GameFieldFrameworkElement : FrameworkElement
    {
        protected VisualCollection FItemsVisual;
        protected VisualCollection FGridVisuals;
        protected VisualCollection FHeaderVisual;
        protected IPointIndexConverter FPointIndexConverter; 
        protected GameField FGameField;

        private Dictionary<int, Brush> FItemBrushes = new Dictionary<int, Brush>();

        private double FItemRadius;
        protected double ItemRadius
        {
            get
            {
                if (this.FItemRadius == 0)
                    this.FItemRadius = this.ItemSize / 2;
                return this.FItemRadius;
            }
        }

        private double FItemSize = 10;
        public double ItemSize
        {
            get { return this.FItemSize; }
            set { this.FItemSize = value; }
        }

        public GameFieldFrameworkElement(GameField aGameField) : 
            base()
        {
            this.FGridVisuals = new VisualCollection(this);
            this.FItemsVisual = new VisualCollection(this);
            this.FHeaderVisual = new VisualCollection(this);
            this.FGameField = aGameField;
            this.Margin = new Thickness(5, 5, 0, 0);
        }

        public GameFieldFrameworkElement(GameField aGameField, double aItemSize) :
            this(aGameField)
        {
            this.ItemSize = aItemSize;
        }

        private Brush GetBrush(int aAge)
        {
            if (!FItemBrushes.ContainsKey(aAge))
            {
                Color _itemColor = new Color();
                _itemColor.A = 255;
                _itemColor.G = (byte)(aAge > 25 ? 0 : 255 - aAge * 10);
                RadialGradientBrush _brush = new RadialGradientBrush(Colors.White, _itemColor);
                _brush.Center = new Point(0.2, 0.2);
                this.FItemBrushes.Add(aAge, _brush);
            }

            return this.FItemBrushes[aAge];
        }

        protected void DrawItem(Point aCenter, int aAge, DrawingContext aContext)
        {
            Brush _brush = this.GetBrush(aAge);
            aContext.DrawEllipse(_brush, null, aCenter, this.ItemRadius, this.ItemRadius);
        }

        private Point ConvertCellIndexToCenter(int aIndex)
        {
            Point _center = this.FPointIndexConverter.ConvertCellIndexToCenter(aIndex);
            _center.Offset(this.Margin.Left, this.Margin.Top);
            return _center;
        }

        //private double FMaxX = 0;
        //public double MaxX
        //{
        //    get { return this.FMaxX; }
        //}

        //private double FMaxY = 0;
        //public double MaxY
        //{
        //    get { return this.FMaxY; }
        //}

        public virtual void DrawItems(GameField aCells)
        {
            DrawingVisual _itemsVisual = new DrawingVisual();
            using (DrawingContext _context = _itemsVisual.RenderOpen())
            {
                for (int i = 0; i < aCells.Count; i++)
                {
                    Life.Cell _cell = aCells[i];
                    if (_cell.CellItem != null)
                    {
                        Point _center = this.ConvertCellIndexToCenter(i);
                        this.DrawItem(_center, _cell.CellItem.Age, _context);
                    }
                }
            }

            this.FItemsVisual.Clear();
            this.FItemsVisual.Add(_itemsVisual);
        }

		public void DrawItems()
		{
			this.DrawItems(this.FGameField);
		}

        protected virtual void DrawGrid()
        {
            DrawingVisual _visual = new DrawingVisual();
            using (DrawingContext _context = _visual.RenderOpen())
            {
                Rect _rect = new Rect(this.Margin.Left, this.Margin.Top, this.Width, this.Height);
                Brush _brush = new SolidColorBrush(Colors.AliceBlue);
                _brush.Opacity = 0.8;
                _context.DrawRectangle(_brush, null, _rect);
            }

            this.FGridVisuals.Add(_visual);
        }

        public virtual void DrawHeader(string aHeader)
        {
            this.FHeaderVisual.Clear();
            DrawingVisual _visual = new DrawingVisual();
            using(DrawingContext _context = _visual.RenderOpen())
            {
                FontFamily _ff = new FontFamily("Arial");
                Typeface _tf = new Typeface(_ff, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal, _ff);
                FormattedText _ft = new FormattedText(
                    aHeader, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, _tf, 14, Brushes.Orange);
                _context.DrawText(_ft, new Point(10, 10));
            }
            this.FHeaderVisual.Add(_visual);
        }

        public int GetCellIndexByPoint(Point aPosition)
        {
            return FPointIndexConverter.ConvertPositionToIndex(aPosition);
        }

        protected override int VisualChildrenCount
        {
            get { return this.FItemsVisual.Count + this.FGridVisuals.Count + this.FHeaderVisual.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < this.FGridVisuals.Count)
                return this.FGridVisuals[index];
            else if (index < this.FGridVisuals.Count + this.FHeaderVisual.Count)
                return this.FHeaderVisual[index - this.FGridVisuals.Count];
            else
                return this.FItemsVisual[index - this.FGridVisuals.Count - this.FHeaderVisual.Count];
        }
    }
}
