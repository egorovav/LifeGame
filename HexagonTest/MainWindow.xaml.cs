using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexagonTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CreateChildren();
        }

        int FGameWidth = 20;

        void CreateChildren()
        {
            int _shift = 10;

            for (int i = 0; i < 400; i++)
            {
                int _cellId = i;
                int _cellX = _cellId % FGameWidth;
                int _cellY = _cellId / FGameWidth;

                Line _itemPoint = new Line();
                _itemPoint.Tag = i;
                _itemPoint.MouseDown += new MouseButtonEventHandler(_itemPoint_MouseDown);
                _itemPoint.StrokeThickness = 5;
                _itemPoint.StrokeStartLineCap = PenLineCap.Round;
                _itemPoint.StrokeEndLineCap = PenLineCap.Round;

                _itemPoint.Stroke = Brushes.Green;

                double _cellSize = _itemPoint.StrokeThickness + 1;

                _itemPoint.X1 = _shift + _cellX * _cellSize;
                _itemPoint.X2 = _shift + _cellX * _cellSize;
                _itemPoint.Y1 = _shift + _cellY * _cellSize;
                _itemPoint.Y2 = _shift + _cellY * _cellSize;

                if ((i / FGameWidth) % 2 == 0)
                {
                    _itemPoint.X1 += 3;
                    _itemPoint.X2 += 3;
                }


                this.cField.Children.Add(_itemPoint);
            }
        }

        void _itemPoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<int> _neighbour = GetNeighbour((int)((Line)sender).Tag);
            foreach (UIElement _child in this.cField.Children)
            {
                if (_child is Line)
                {
                    Line _line = (Line)_child;
                    if (_neighbour.Contains((int)_line.Tag))
                    {
                        _line.Stroke = Brushes.Red;
                    }
                }
            }
        }

        List<int> GetNeighbour(int i)
        {

            Life.HexagonCellTorGameField _hcgf = new Life.HexagonCellTorGameField(20, 20);
            

            List<int> _neighbour = new List<int>();
            Life.Cell _cell = _hcgf[i];
            for (int j = 0; j < _hcgf.Count; j++)
            {
                if (_cell.Neighbour.Contains(_hcgf[j]))
                    _neighbour.Add(j);
            }
            
            //int _x = i % FGameWidth;
            //int _y = i / FGameWidth;
            //int _incrX = IncrementCoordinate(_x, FGameWidth);
            //int _decrX = DecrementCoordinate(_x, FGameWidth);
            //int _incrY = IncrementCoordinate(_y, FGameWidth);
            //int _decrY = DecrementCoordinate(_y, FGameWidth);
            //_neighbour.Add(_y * FGameWidth + _incrX);
            //_neighbour.Add(_y * FGameWidth + _decrX);
            //_neighbour.Add(_incrY * FGameWidth + _x);
            //_neighbour.Add(_decrY * FGameWidth + _x);


            //if ((i / FGameWidth) % 2 == 0)
            //{
            //    _neighbour.Add(_incrY * FGameWidth + _incrX);
            //    _neighbour.Add(_decrY * FGameWidth + _incrX);
            //}
            //else
            //{
            //    _neighbour.Add(_incrY * FGameWidth + _decrX);
            //    _neighbour.Add(_decrY * FGameWidth + _decrX);
            //}

            return _neighbour;
        }

        int IncrementCoordinate(int aCoordinate, int aSize)
        {
            return (aCoordinate + 1) % aSize;
        }

        int DecrementCoordinate(int aCoordinate, int aSize)
        {
            return (aCoordinate + aSize - 1) % aSize;
        }
    }
}
