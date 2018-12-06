using System;
using System.Windows;
using System.Windows.Data;

namespace CountingApp3
{
    /// <summary>
    /// Returns the inverse of a boolean (!value) or a number (1/value).
    /// </summary>
    public class InverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return !(bool)value;
            double d;
            if (double.TryParse(value.ToString(), out d))
                return 1 / d;
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return !(bool)value;
            double d;
            if (double.TryParse(value.ToString(), out d))
                return 1 / d;
            return value;
        }
    }

    /// <summary>
    /// XAML draws ellipses to the right and down from its point. It is more intuitive to have its point be the centroid.
    /// </summary>
    public class EllipsePositionConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a Point from its (X,Y) position to (X-radius,Y-radius) as a Thickness to set to Ellipse.Margin.
        /// </summary>
        /// <param name="value">value[0] = the X or Y double value. value[1] = the diameter of the ellipse.</param>
        /// <returns>value[0] - value[1] / 2</returns>
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.Length >= 2 && value[0] is Point && value[1] is double && value[2] is double)
            {
                Point p = (Point)value[0];
                double diameter = (double)value[1];
                double scaleFactor = (double)value[2];
                double left = p.X - diameter / scaleFactor / 2;
                double top = p.Y - diameter / scaleFactor / 2;
                Thickness t = new Thickness(left, top, 0, 0);
                return t;
            }
            else
                return null;
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Easier to interpret a bool rather than a Visibility in the view-model and model layers. So, have to convert from bool to visibility for the view layer.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            else
                return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Have to keep track of the rectangle start and stop points for when the user directs it to the left and above the start point.
    /// Could calculate the Thickness in the model layer, but that is a redundant property that may become difficult to maintain. 
    /// So, only keep track of start and end points for the rectangle, and convert to thickness in a converter.
    /// </summary>
    public class PointsToThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.Length > 1 && value[0] is Point && value[1] is Point)
            {
                Point p1 = (Point)value[0];
                Point p2 = (Point)value[1];
                double x = Math.Min(p1.X, p2.X);
                double y = Math.Min(p1.Y, p2.Y);
                return new Thickness(x, y, 0, 0);
            }
            else
                return null;
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Could calculate the width in the model layer, but that is a redundant property that may become difficult to maintain.
    /// So, only keep track of start and end points for the rectangle, and convert to width in a converter.
    /// </summary>
    public class PointsToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.Length > 1 && value[0] is Point && value[1] is Point)
            {
                Point p1 = (Point)value[0];
                Point p2 = (Point)value[1];
                return Math.Abs(p1.X - p2.X);
            }
            else
                return 0;
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Could calculate the height in the model layer, but that is a redundant property that may become difficult to maintain.
    /// So, only keep track of start and end points for the rectangle, and convert to height in a converter.
    /// </summary>
    public class PointsToHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.Length > 1 && value[0] is Point && value[1] is Point)
            {
                Point p1 = (Point)value[0];
                Point p2 = (Point)value[1];
                return Math.Abs(p1.Y - p2.Y);
            }
            else
                return 0;
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Convert the double Scale Factor to a percentage for UI.
    /// </summary>
    public class ScaleFactorToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                return (int)((double)value * 100);
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d;
            if (double.TryParse(value.ToString(), out d))
            {
                return d / 100;
            }
            return value;
        }
    }

    /// <summary>
    /// Divide value[0] / value[1]. If there is a 3rd value, it returns value[0] / value[1] / value[2]. Great for resizing count marks and lines when the scale factor changes.
    /// </summary>
    public class ActualMarkSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d1 = System.Convert.ToDouble(value[0]);
            double d2 = System.Convert.ToDouble(value[1]);
            double d3 = 1;
            if (value.Length > 2)
            {
                d3 = System.Convert.ToDouble(value[2]);
            }
            return d1 / d2 / d3;
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
