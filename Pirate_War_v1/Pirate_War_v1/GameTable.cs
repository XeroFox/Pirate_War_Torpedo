using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace Pirate_War_v1
{
    internal class GameTable
    {
        public string Name { get; set; }
        public List<Coordinates> Table { get; set; }
        public List<Ships> ships { get; set; }

        public GameTable(string name)
        {
            Name = name;
            Table = new List<Coordinates>();
            this.clearTable();
            ships = new List<Ships>();
        }

        public Coordinates getCoordinate(int XX, int YY)
        {
            foreach(Coordinates coordinates in Table)
            {
                if (coordinates.X == XX && coordinates.Y == YY) return coordinates;
            }
            return new Coordinates(XX,YY);
        }

        public Ships getShipByCoordinate(int XX, int YY)
        {
            foreach (Ships ship in ships)
            {
                foreach(Coordinates coordinates in ship.PlacedCoordinates)
                {
                    if (coordinates.X == XX && coordinates.Y == YY) return ship;
                }
            }
            return new Ships(0, new List<Coordinates>(), new Coordinates(0, 0), 0, 0);
        }

        public void clearTable()
        {
            List<Coordinates> table = new List<Coordinates>();
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    Coordinates newCoordinate = new Coordinates(i,j);
                    if(i == 0 || i == 9 || j == 0 || j == 9)
                    {
                        newCoordinate.Value = -1;
                    }
                    table.Add(newCoordinate);
                }
            }
            this.Table = table;
        }

        public void placeShip(int xx, int yy, int rot, int shipType)
        {
            List<Coordinates> coordinates = new List<Coordinates>();
            
            coordinates.Add(new Coordinates(xx, yy));
            
            for(int i = 1; i < shipType; i++)
            {
                coordinates.Add(rot == 1 ? new Coordinates(xx + i, yy) : new Coordinates(xx, yy + i));
            }

            foreach(Coordinates coordinates1 in coordinates)
            {
                coordinates1.Value = shipType;
            }
            Ships newShip = new Ships(shipType, coordinates, coordinates[0],(shipType == 2 ? 0 : shipType == 3 ? 2 : 4),rot);
            ships.Add(newShip);
            updateTable();
        }

        public bool isShipPlaceable(int xx, int yy, int rot, int shipType)
        {
            bool result = true;

            for (int i = 0; i < shipType; i++)
            {
                if(rot == 0 && (getCoordinate(xx,yy + i).Value > 0 || getCoordinate(xx, yy + i).Value == -1))
                {
                    result = false;
                }
                else if (rot == 1 && (getCoordinate(xx + i, yy).Value > 0 || getCoordinate(xx + i, yy).Value == -1))
                {
                    result = false;
                }
            }
            return result;
        }

        public void generateRandomShips()
        {
            Random r = new Random();

            int tmpX = r.Next(1, 6);
            int tmpY = r.Next(1, 6);
            int rot = r.Next(0, 2);
            placeShip(tmpX, tmpY, rot, 4);

            
            int count = 2;
            while(count != 0)
            {
                tmpX = r.Next(1, 7);
                tmpY = r.Next(1, 7);
                rot = r.Next(0, 2);
                if (isShipPlaceable(tmpX, tmpY, rot, 3))
                {
                    placeShip(tmpX, tmpY, rot, 3);
                    count--;
                }
            }

            count = 4;
            while (count != 0)
            {
                tmpX = r.Next(1, 8);
                tmpY = r.Next(1, 8);
                rot = r.Next(0, 2);

                if (isShipPlaceable(tmpX, tmpY, rot, 2))
                {
                    placeShip(tmpX, tmpY, rot, 2);
                    count--;
                }
            }
            removeNines();
        }

        public void updateTable()
        {
            foreach(Ships ship in ships)
            {
                foreach(Coordinates coordinates in ship.PlacedCoordinates)
                {
                    getCoordinate(coordinates.X, coordinates.Y).Value = coordinates.Value;
                    if(getCoordinate(coordinates.X, coordinates.Y).Value != -1)
                    {
                        getCoordinate(coordinates.X+1, coordinates.Y).Value = getCoordinate(coordinates.X+1, coordinates.Y).Value == 0 ? 9 : getCoordinate(coordinates.X + 1, coordinates.Y).Value;
                        getCoordinate(coordinates.X-1, coordinates.Y).Value = getCoordinate(coordinates.X-1, coordinates.Y).Value == 0 ? 9 : getCoordinate(coordinates.X - 1, coordinates.Y).Value;
                        getCoordinate(coordinates.X, coordinates.Y+1).Value = getCoordinate(coordinates.X, coordinates.Y+1).Value == 0 ? 9 : getCoordinate(coordinates.X, coordinates.Y + 1).Value;
                        getCoordinate(coordinates.X, coordinates.Y-1).Value = getCoordinate(coordinates.X, coordinates.Y-1).Value == 0 ? 9 : getCoordinate(coordinates.X, coordinates.Y - 1).Value;
                    }
                }
            }
        }

        public void removeNines()
        {
            foreach(Coordinates coordinates in Table)
            {
                if (coordinates.Value == 9) coordinates.Value = 0;
            }
        }

        public bool makeAShot(int XX, int YY)
        {
            int shipType = 0;
            bool result = false;
            if(getCoordinate(XX,YY).Value == 0)
            {
                getCoordinate(XX, YY).Value = 1;
            }else if(getCoordinate(XX, YY).Value > 1 && getCoordinate(XX, YY).Value <= 4)
            {
                shipType = getCoordinate(XX, YY).Value;
                getCoordinate(XX, YY).Value = getCoordinate(XX, YY).Value + getCoordinate(XX, YY).Value * 10;
                result = checkIfDestroyed(XX, YY, shipType);
            }
            return result;
        }

        public bool checkIfDestroyed(int XX, int YY, int shipType)
        {
            int count = 0;
            bool result = false;
            foreach(Coordinates coordinates in getShipByCoordinate(XX, YY).PlacedCoordinates)
            {
                if(getCoordinate(coordinates.X,coordinates.Y).Value == shipType + shipType * 10)
                {
                    count++;
                    Debug.WriteLine(count);
                }
            }

            if(count == shipType)
            {
                string messageBoxText = "ANYUKAAAA";
                string caption = "Player Ships Dict";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult alerbox;

                alerbox = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                foreach (Coordinates coordinates in getShipByCoordinate(XX, YY).PlacedCoordinates)
                {
                    if (getCoordinate(coordinates.X, coordinates.Y).Value == shipType + shipType * 10)
                    {
                        getCoordinate(coordinates.X, coordinates.Y).Value = shipType + shipType * 10 + 1;
                    }
                }
                getShipByCoordinate(XX, YY).Destroyed = true;
                Panel.SetZIndex(getShipByCoordinate(XX, YY).shipBody,50);
                getShipByCoordinate(XX, YY).SpriteIndex = getShipByCoordinate(XX, YY).SpriteIndex+1;
                result = true;
            }

            return result;
            
        }

        public bool isScored(int XX, int YY)
        {
            if (getCoordinate(XX,YY).Value == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void printTable()
        {
            Debug.WriteLine(Name);
            var i = 1;
            foreach(Coordinates cord in Table)
            {
                Debug.Write(cord.Value + " ");
                if(i % 10 == 0)
                {
                    Debug.Write("\n");
                }
                i++;
            }
        }
    }
}
