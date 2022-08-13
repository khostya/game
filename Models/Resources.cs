using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Splat;
using WpfGame.Extensions;
using WpfGame.Models.Entities;
using WpfGame.Models.Map;

namespace WpfGame.Models
{
    public static class Resources
    {
        private static readonly string AssetsFolder =  Path.Combine(
            new DirectoryInfo(Directory.GetCurrentDirectory()).Parent!.Parent!.Parent!.ToString(), "Assets");
        private static readonly string SpritesFolder =  Path.Combine(AssetsFolder, "Sprites");
        
        private static readonly string EntitiesFolder = Path.Combine(SpritesFolder, "Entities");

        public static readonly Dictionary<EntityType, Frames> AllFrames =
            Directory
                .GetDirectories(EntitiesFolder)
                .ToDictionary(GetTypeEntity, 
                    x => new Frames(
                        GetAllByEntityFrames(x), Locator.Current.GetService<Settings>()!.UpdateInterval));

        public static class MapSprites
        {
            private static readonly string MapFolder = Path.Combine(SpritesFolder, "Map");
            public static readonly BitmapSource Floor = GetFrames(Path.Combine(MapFolder, "Floor")).First();
            public static readonly BitmapSource Wall = GetFrames(Path.Combine(MapFolder, "Wall")).First();
        }
        
        public static class Icons
        {
            private static readonly string IconsFolder = Path.Combine(AssetsFolder, "Icons");
            public static readonly BitmapSource SoundOn16px = new BitmapImage(new Uri(Path.Combine(IconsFolder, "sound-on16.png")));
            public static readonly BitmapSource Mute16px = new BitmapImage(new Uri(Path.Combine(IconsFolder, "mute16.png")));
        }
        
        public static class MapLevels
        {
            private static readonly string MapLevelsFolder = Path.Combine(AssetsFolder, "Levels");
            public static readonly MapCell[][] StartLevel = GetLevel("Start.txt");

            private static MapCell[][] GetLevel(string level)
            {
                return MapParser.Parse(File.ReadLines(Path.Combine(MapLevelsFolder, level)));
            }
        }

        private static Dictionary<EntityState, CircularLinkedList<BitmapSource>> GetAllByEntityFrames(string path)
        {
            return Directory.GetDirectories(path)
                .Select(x => (GetStateFrames(x),GetFrames(x).ToCircularLinkedList()))
                .ToDictionary(x => x.Item1, x => x.Item2);
        }

        private static IEnumerable<BitmapSource> GetFrames(string path)
        {
            var cellSize = Locator.Current.GetService<Size>();
            return Directory.GetFiles(path)
                .Select(x =>
                {
                    var bitmap = new BitmapImage(new Uri(x));
                    var imageSize = new Size(bitmap.Width + cellSize.Width - 20,
                        bitmap.Height + cellSize.Height - 20);
                    if (path.Contains("Map"))
                        imageSize = cellSize;
                    if (path.Contains(Path.Combine("Player","Standing"))) 
                        imageSize = new Size(31, 40);
                    var renderTargetBitmap = new RenderTargetBitmap((int)imageSize.Width, (int)imageSize.Height, 
                        96, 96, PixelFormats.Default);
                    var drawingVisual = new DrawingVisual();
                    using (var drawingContext = drawingVisual.RenderOpen())
                        drawingContext.DrawImage(bitmap, new Rect(new Point(0, 0), imageSize));

                    renderTargetBitmap.Render(drawingVisual);
                    return renderTargetBitmap;
                });
        }
        
        private static EntityState GetStateFrames(string path)
        {
            var nameState = Path.GetFileName(path);
            var state = (EntityState)Enum.Parse(typeof(EntityState), nameState);
            return state;
        }

        private static EntityType GetTypeEntity(string path)
        {
            var nameState = Path.GetFileName(path);
            var entityType = (EntityType)Enum.Parse(typeof(EntityType), nameState);
            return entityType;
        }
    }
}
