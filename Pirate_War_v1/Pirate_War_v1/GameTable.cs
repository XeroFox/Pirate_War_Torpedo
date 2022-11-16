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


        public List<Coordinates> aiGeneratePossibleMoves()
        {
            List<Coordinates> possibleMoves = new List<Coordinates>();
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    if(getCoordinate(i, j).Value == 0 || (getCoordinate(i, j).Value <= 4 && getCoordinate(i, j).Value >= 2))
                    possibleMoves.Add(getCoordinate(i,j));
                }
            }
            return possibleMoves;
        }

        public List<Coordinates> aiAllDeadZones()
        {
            List<Coordinates> deadZones = new List<Coordinates>();
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    if (getCoordinate(i, j).Value == 1 || getCoordinate(i, j).Value > 4 || getCoordinate(i, j).Value == -1)
                        deadZones.Add(getCoordinate(i, j));
                }
            }
            return deadZones;
        }

        public List<Coordinates> aiGetNextTips(int XX, int YY, List<Coordinates> currentMoves)
        {
            List<Coordinates> possibleTips = new List<Coordinates>();
            foreach(Coordinates coord in currentMoves)
            {
                if((getCoordinate(coord.X, coord.Y).Value <= 4 && getCoordinate(coord.X, coord.Y).Value != 1 && getCoordinate(coord.X, coord.Y).Value != -1) &&
                    (coord.compareCoord(new Coordinates(XX+1, YY))
                    || coord.compareCoord(new Coordinates(XX - 1, YY)) || coord.compareCoord(new Coordinates(XX, YY + 1))
                    || coord.compareCoord(new Coordinates(XX, YY - 1))))
                {
                    possibleTips.Add(coord);
                }
            }

            return possibleTips;
        }

        public List<Coordinates> aiGetNextShipShot(Coordinates first, Coordinates Last, List<Coordinates> allShots)
        {
            List<Coordinates> newShots = new List<Coordinates>();
            int rotation = this.getShipByCoordinate(first.X, first.Y).Rotation;
            if (rotation == 0)
            {
                if (first.Y - 1 > 0 && first.Y - 1 != Last.Y) {
                    newShots.Add(new Coordinates(first.X, first.Y - 1));
                }
                if (first.Y + 1 < 9 && first.Y + 1 != Last.Y)
                {
                    newShots.Add(new Coordinates(first.X, first.Y + 1));
                }
                if (Last.Y - 1 > 0 && first.Y != Last.Y - 1)
                {
                    newShots.Add(new Coordinates(Last.X, Last.Y - 1));
                }
                if (Last.Y + 1 < 9 && first.Y != Last.Y + 1)
                {
                    newShots.Add(new Coordinates(Last.X, Last.Y + 1));
                }
            }
            else
            {
                if (first.X - 1 > 0 && first.X - 1 != Last.X)
                {
                    newShots.Add(new Coordinates(first.X - 1, first.Y));
                }
                if (first.X + 1 > 0 && first.X + 1 != Last.X)
                {
                    newShots.Add(new Coordinates(first.X + 1, first.Y));
                }
                if (Last.X - 1 > 0 && first.X != Last.X - 1)
                {
                    newShots.Add(new Coordinates(Last.X - 1, Last.Y));
                }
                if (Last.X + 1 > 0 && first.X != Last.X + 1)
                {
                    newShots.Add(new Coordinates(Last.X + 1, Last.Y));
                }
            }

            foreach(Coordinates coordinates in newShots)
            {
                coordinates.Value = this.getCoordinate(coordinates.X,coordinates.Y).Value;
            }
            List<Coordinates> actualShots = new List<Coordinates>();
            foreach (Coordinates coordinates1 in newShots)
            {
                if((coordinates1.Value <= 4 && coordinates1.Value >= 2) || coordinates1.Value == 0)
                {
                    actualShots.Add(coordinates1);
                }
            }

            return actualShots;
        }

        public void aiReplaceDestroyedShipSides()
        {
            foreach (Ships ship in ships)
            {
                
                foreach(Coordinates coordinates in ship.PlacedCoordinates)
                {
                    if (ship.Destroyed)
                    {
                        getCoordinate(coordinates.X + 1, coordinates.Y).Value = getCoordinate(coordinates.X + 1, coordinates.Y).Value == 0 ? 9 : getCoordinate(coordinates.X + 1, coordinates.Y).Value;
                        getCoordinate(coordinates.X - 1, coordinates.Y).Value = getCoordinate(coordinates.X - 1, coordinates.Y).Value == 0 ? 9 : getCoordinate(coordinates.X - 1, coordinates.Y).Value;
                        getCoordinate(coordinates.X, coordinates.Y + 1).Value = getCoordinate(coordinates.X, coordinates.Y + 1).Value == 0 ? 9 : getCoordinate(coordinates.X, coordinates.Y + 1).Value;
                        getCoordinate(coordinates.X, coordinates.Y - 1).Value = getCoordinate(coordinates.X, coordinates.Y - 1).Value == 0 ? 9 : getCoordinate(coordinates.X, coordinates.Y - 1).Value;
                    }
                }
            }
        }


        public void printTable()
        {
            Debug.WriteLine(Name);
            var i = 1;
            foreach(Coordinates cord in Table)
            {
                Debug.Write((cord.Value < 10 && cord.Value >= 0 ? "0"+cord.Value : cord.Value ) + " ");
                if(i % 10 == 0)
                {
                    Debug.Write("\n");
                }
                i++;
            }
        }
    }
}
