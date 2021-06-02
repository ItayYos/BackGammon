using Common.BackGammonModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Client.Convertors
{
    public class ElipseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ObservableCollection<Ellipse> col = new ObservableCollection<Ellipse>();
            GameTile gt = (GameTile)value;

            for (int i = 0; i < gt.Pieces.Count; i++)
            {
                Ellipse e = new Ellipse();
                e.Width = 30;
                e.Height = 30;
                if (gt.Pieces.FirstOrDefault()?.Player == "Player1")
                {
                    e.Fill = new SolidColorBrush(Colors.Blue);
                }
                else if ((gt.Pieces.FirstOrDefault()?.Player == "Player2"))
                    e.Fill = new SolidColorBrush(Colors.Yellow);

                col.Add(e);
            }

            return col;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // throw new NotImplementedException();
            return new object();
        }
    }
}
