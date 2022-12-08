using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Pirate_War_v1
{
    public class Ships
    {
        public int Type { get; }
        public List<Coordinates> PlacedCoordinates { get; }
        public Coordinates StartingCoordinates { get; }
        public int SpriteIndex { get; set; }
        public int Rotation { get; }
        public bool Destroyed { get; set; }
        public Rectangle shipBody { get; set; }

        public Ships(int type, List<Coordinates> placedCoordinates, Coordinates startingCoordinates, int spriteIndex, int rotation)
        {
            Type = type;
            PlacedCoordinates = placedCoordinates;
            StartingCoordinates = startingCoordinates;
            SpriteIndex = spriteIndex;
            Rotation = rotation;
            Destroyed = false;
            shipBody = new Rectangle
            {
                Width = (Type == 2 ? 100 : Type == 3 ? 150 : 215),
                Height = (Type == 2 ? 30 : Type == 3 ? 75 : 100),
                RenderTransform = (Rotation == 1 ? new RotateTransform(270) : new RotateTransform(0)),
            };
        }
    }
}
