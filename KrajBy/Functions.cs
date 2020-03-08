using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.IsolatedStorage;

using Microsoft.Phone.Shell;


#if FORTILE
namespace ForTile
#else
namespace KrajBy
#endif

{
    class Functions
    {

        static string countername = "counter";
        static string lrname = "lastread";
        static string rCounter = "rCounter";
        static string wCityname = "wCity";

        public Functions()
        {
        }

        public string GetImageFromPostContents(string text2search)
        {
            return Regex.Matches(text2search,
                    @"(?<=<img\s+[^>]*?src=(?<q>['""]))(?<url>.+?)(?=\k<q>)",
                    RegexOptions.IgnoreCase)
                .Cast<Match>()
                .Where(m =>
                {
                    Uri url;
                    if (Uri.TryCreate(m.Groups[0].Value, UriKind.Absolute, out url))
                    {
                        string ext = Path.GetExtension(url.AbsolutePath).ToLower();
                        if (ext == ".png" || ext == ".jpg" || ext == ".bmp" || ext == ".jpeg") return true;
                    }
                    return false;
                })
                .Select(m => m.Groups[0].Value)
                .FirstOrDefault();
        }

        public void ChangeTile(PostMessage forImage, int cnt, bool setcount)
        {
            int count;

            if (setcount)
                count = cnt;
            else
                count = counter + cnt;

            if (count > 99) count = 99;  // Система больше не позволяет

            var apptile = ShellTile.ActiveTiles.First();
            var appTileData = new StandardTileData();

        //    appTileData.Title = "";
            appTileData.Count = count;
            appTileData.BackTitle = forImage.pubDate;
            appTileData.BackContent = "";
            appTileData.BackBackgroundImage = new Uri(forImage.mainImage, UriKind.RelativeOrAbsolute);
            apptile.Update(appTileData);
            counter = count;
            lastRead = Convert.ToDateTime(forImage.pubDate);
        }

        public int wCity
        {
            get
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (options.Contains(wCityname))
                    return (int)options[wCityname];
                else
                    return 0;
            }
            set
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (!options.Contains(wCityname))
                {
                    options.Add(wCityname, value);
                    options.Save();
                    return;
                }
                options[wCityname] = value;
                options.Save();
            }
        }

        public int counter
        {
            get
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (options.Contains(countername))
                    return (int)options[countername];
                else
                    return 0;
            }
            set
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (!options.Contains(countername))
                {
                    options.Add(countername, value);
                    options.Save();
                    return;
                }
                options[countername] = value;
                options.Save();
            }
        }
        
        public DateTime lastRead 
        { 
            get
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (options.Contains(lrname))
                    return (DateTime)options[lrname];
                else
                    return new DateTime(2013,1, 1);
            }
            set 
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (!options.Contains(lrname))
                {
                    options.Add(lrname, value);
                    options.Save();
                    return;
                }
                options[lrname] = value;
                options.Save();
            }
        
        }

        public int readCount 
        {
            get
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (options.Contains(rCounter))
                    return (int)options[rCounter];
                else
                    return 0;
            }
            set
            {
                var options = IsolatedStorageSettings.ApplicationSettings;
                if (!options.Contains(rCounter))
                {
                    options.Add(rCounter, value);
                    options.Save();
                    return;
                }
                options[rCounter] = value;
                options.Save();
            }

        }
    }
}
