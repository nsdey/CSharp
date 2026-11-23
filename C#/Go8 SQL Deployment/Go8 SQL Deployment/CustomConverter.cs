using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Go8_SQL_Deployment
{
    public enum RunStatus
    {
        None,
        Running,
        Success,
        Failed,
    }

    public class RunStatusSuccessVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            try
            {
                if (value != null && value is RunStatus && (RunStatus)value == RunStatus.Success) visibility = Visibility.Visible;
            }
            catch
            {
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return RunStatus.None;
        }
    }

    public class RunStatusFailedVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            try
            {
                if (value != null && value is RunStatus && (RunStatus)value == RunStatus.Failed) visibility = Visibility.Visible;
            }
            catch
            {
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return RunStatus.None;
        }
    }

    public class RunStatusRunningVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            try
            {
                if (value != null && value is RunStatus && (RunStatus)value == RunStatus.Running) visibility = Visibility.Visible;
            }
            catch
            {
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return RunStatus.None;
        }
    }

    public class RunStatusVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            try
            {
                if (value != null && value is RunStatus && (RunStatus)value != RunStatus.None) visibility = Visibility.Visible;
            }
            catch
            {
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return RunStatus.None;
        }
    }

    public class NullVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            if (value != null) visibility = Visibility.Visible;

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
