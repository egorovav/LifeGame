using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LifeWpfClient
{
    public interface IPointIndexConverter
    {
        Point ConvertCellIndexToCenter(int aIndex);

        int ConvertCellCenterToIndex(Point aCenter);

        int ConvertPositionToIndex(Point aPosition);
    }
}
