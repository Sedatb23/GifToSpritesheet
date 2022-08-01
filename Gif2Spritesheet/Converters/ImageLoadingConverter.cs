using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gif2Spritesheet.Converters
{
    /// <summary>
    /// Source : https://nicksnettravels.builttoroam.com/winui-uno-imagesharp/
    /// </summary>
    public class ImageLoadingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var img = (Image)value;
            var stream = new MemoryStream();
            img.SaveAsBmp(stream);

            return stream;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
