using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

#if FORTILE
namespace ForTile
#else
namespace KrajBy
#endif
{
    public class PostMessage
    {
        public string pubDate { get; set; }

        public string title { get; set; }

        public string link { get; set; }

        public string description { get; set; }

        public string mainImage { get; set; }
    }
}
