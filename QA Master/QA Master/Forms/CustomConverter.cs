using QA_Master.Models.Enums;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace QA_Master.Forms
{
    public class StatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            if (value != null)
            {
                if (value.GetType() == typeof(EnumRunStatus))
                {
                    EnumRunStatus status = (EnumRunStatus)value;

                    if (status != EnumRunStatus.None) visibility = Visibility.Visible;
                }
                else
                {
                    visibility = Visibility.Visible;
                }
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class PassFailColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                EnumRunStatus status = (EnumRunStatus)value;

                switch (status)
                {
                    case EnumRunStatus.Running: return new SolidColorBrush(Colors.Blue);
                    case EnumRunStatus.Passed: return new SolidColorBrush(Colors.Green);
                    case EnumRunStatus.Failed: return new SolidColorBrush(Colors.Red);
                    case EnumRunStatus.PartiallyPassed: return new SolidColorBrush(Colors.Brown);
                    default: return new SolidColorBrush(Colors.Black);
                }
            }
            catch
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumRunStatus.None;
        }
    }

    public class EnumDescriptionConverter : IValueConverter
    {
        private string? GetEnumDescription(Enum enumObj)
        {
            FieldInfo? fieldInfo = enumObj?.GetType().GetField(enumObj.ToString());

            object[]? attribArray = fieldInfo?.GetCustomAttributes(false);

            if (attribArray != null && attribArray.Length == 0)
            {
                return enumObj?.ToString();
            }
            else
            {
                DescriptionAttribute? attrib = attribArray?[0] as DescriptionAttribute;
                return attrib?.Description;
            }
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string? description = GetEnumDescription(myEnum);
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
    /*
    public class TextBlockMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fontSize = 10;

            if (value != null)
            {
                _ = double.TryParse(value.ToString()?.Replace("px", ""), out fontSize);
            }

            return new Thickness(fontSize / 5, -fontSize, fontSize / 5, -fontSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class  TextBlockPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fontSize = 10;

            if (value != null)
            {
                _ = double.TryParse(value.ToString()?.Replace("px", ""), out fontSize);
            }

            return new Thickness(fontSize / 3, 0, fontSize / 3, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RectangleMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fontSize = 30;

            if (value != null)
            {
                _ = double.TryParse(value.ToString()?.Replace("px", ""), out fontSize);
            }

            return new Thickness(-fontSize / 2d, fontSize * 8d / 10d, -fontSize / 2d, -fontSize / 2d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [MarkupExtensionReturnType(typeof(Thickness))]
    public class UserControlPaddingConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fontSize = 30;

            if (value != null)
            {
                _ = double.TryParse(value.ToString()?.Replace("px", ""), out fontSize);
            }

            return new Thickness(fontSize / 2d, 0, fontSize / 2d, fontSize / 2d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            
            throw new NotImplementedException();
        }
    }
    */
}
