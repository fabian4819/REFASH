using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp_REFASH
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (VerificationStatus)value;
            switch (status)
            {
                case VerificationStatus.Verified:
                    return "Verified";
                case VerificationStatus.InVerification:
                    return "In Review";
                case VerificationStatus.Rejected:
                    return "Rejected";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (VerificationStatus)value;
            switch (status)
            {
                case VerificationStatus.Verified:
                    return Brushes.LightGreen;
                case VerificationStatus.InVerification:
                    return Brushes.Orange;
                case VerificationStatus.Rejected:
                    return Brushes.Red;
                default:
                    return Brushes.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (VerificationStatus)value;
            switch (status)
            {
                case VerificationStatus.Verified:
                    return "M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"; // Checkmark icon
                case VerificationStatus.InVerification:
                    return "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm-1-4h2v2h-2zm0-2h2V7h-2z"; // Question mark icon
                case VerificationStatus.Rejected:
                    return "M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"; // X icon
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DivideByFourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
            {
                // Mengurangi margin total (50 pixel per item) dan padding tambahan
                double availableWidth = width - (4 * 50) - 40;
                return availableWidth / 4;
            }
            return 280; // default width
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
