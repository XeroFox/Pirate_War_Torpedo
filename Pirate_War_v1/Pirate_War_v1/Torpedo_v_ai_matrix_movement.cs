using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Shell;

namespace Pirate_War_v1
{
    public static class Torpedo_v_ai_matrix_movement
    {

        public static int[,] clearMatrixElements()
        {
            int[,] matrix = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i == 0 || i == 9 || j == 0 || j == 9)
                    {
                        matrix[i, j] = -1;
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }
                }
            }
            return matrix;
        }

        public static bool is_ship_placeable(int XX, int YY, int shipType, int[,] matrix, int rotation)
        {
            if (shipType == 2)
            {
                if (rotation == 1)
                {
                    if (matrix[XX, YY] <= 0 && matrix[XX+1, YY] <= 0 && matrix[XX-1, YY] <= 0 && matrix[XX, YY-1] <= 0 &&
                        matrix[XX+1, YY+1] <= 0 && matrix[XX-1, YY+1] <= 0 && matrix[XX, YY+2] <= 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 0)
                {
                    if (matrix[XX, YY] <= 0 && matrix[XX - 1, YY] <= 0 && matrix[XX, YY + 1] <= 0 && matrix[XX, YY - 1] <= 0 &&
                        matrix[XX + 1, YY - 1] <= 0 && matrix[XX + 1, YY + 1] <= 0 && matrix[XX + 2, YY] <= 0)
                    {
                        return true;
                    }
                }
            }
            else if(shipType == 3)
            {
                if (rotation == 1)
                {
                    if (matrix[XX, YY] <= 0 && matrix[XX + 1, YY] <= 0 && matrix[XX - 1, YY] <= 0 && matrix[XX, YY - 1] <= 0 &&
                        matrix[XX + 1, YY + 1] <= 0 && matrix[XX - 1, YY + 1] <= 0 && matrix[XX, YY + 2] <= 0 &&
                        matrix[XX + 1, YY + 2] <= 0 && matrix[XX - 1, YY + 2] <= 0 && matrix[XX, YY + 3] <= 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 0)
                {
                    if (matrix[XX, YY] <= 0 && matrix[XX - 1, YY] <= 0 && matrix[XX, YY + 1] <= 0 && matrix[XX, YY - 1] <= 0 &&
                        matrix[XX + 1, YY - 1] <= 0 && matrix[XX + 1, YY + 1] <= 0 && matrix[XX + 2, YY] <= 0 &&
                        matrix[XX + 2, YY - 1] <= 0 && matrix[XX + 2, YY + 1] <= 0 && matrix[XX + 3, YY] <= 0)
                    {
                        return true;
                    }
                }
            }
            else if(shipType == 4)
            {
                if (rotation == 1)
                {
                    if (matrix[XX, YY] <= 0 && matrix[XX + 1, YY] <= 0 && matrix[XX - 1, YY] <= 0 && matrix[XX, YY - 1] <= 0 &&
                        matrix[XX + 1, YY + 1] <= 0 && matrix[XX - 1, YY + 1] <= 0 && matrix[XX, YY + 2] <= 0 &&
                        matrix[XX + 1, YY + 2] <= 0 && matrix[XX - 1, YY + 2] <= 0 && matrix[XX, YY + 3] <= 0 &&
                        matrix[XX + 1, YY + 3] <= 0 && matrix[XX - 1, YY + 3] <= 0 && matrix[XX, YY + 4] <= 0)
                    {
                        return true;
                    }
                }
                else if (rotation == 0)
                {
                    if (matrix[XX, YY] <= 0 && matrix[XX - 1, YY] <= 0 && matrix[XX, YY + 1] <= 0 && matrix[XX, YY - 1] <= 0 &&
                        matrix[XX + 1, YY - 1] <= 0 && matrix[XX + 1, YY + 1] <= 0 && matrix[XX + 2, YY] <= 0 &&
                        matrix[XX + 2, YY - 1] <= 0 && matrix[XX + 2, YY + 1] <= 0 && matrix[XX + 3, YY] <= 0 &&
                        matrix[XX + 3, YY - 1] <= 0 && matrix[XX + 3, YY + 1] <= 0 && matrix[XX + 4, YY] <= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int[,] updateMatrix(int XX, int YY, int shipType, int[,] matrix, int rotation)
        {
            int[,] newMatrix = matrix;

            if (shipType == 2)
            {
                if (rotation == 0) {
                    newMatrix[XX, YY] = shipType;
                    newMatrix[XX+1, YY] = shipType;
                }
                else
                {
                    newMatrix[XX, YY] = shipType;
                    newMatrix[XX, YY + 1] = shipType;
                }
            }else if(shipType == 3)
            {
                if (rotation == 0)
                {
                    newMatrix[XX, YY] = shipType;
                    newMatrix[XX + 1, YY] = shipType;
                    newMatrix[XX + 2, YY] = shipType;
                }
                else
                {
                    newMatrix[XX, YY] = shipType;
                    newMatrix[XX, YY + 1] = shipType;
                    newMatrix[XX, YY + 2] = shipType;
                }
            }
            else if(shipType == 4)
            {
                if (rotation == 0)
                {
                    newMatrix[XX, YY] = shipType;
                    newMatrix[XX + 1, YY] = shipType;
                    newMatrix[XX + 2, YY] = shipType;
                    newMatrix[XX + 3, YY] = shipType;
                }
                else
                {
                    newMatrix[XX, YY] = shipType;
                    newMatrix[XX, YY + 1] = shipType;
                    newMatrix[XX, YY + 2] = shipType;
                    newMatrix[XX, YY + 3] = shipType;
                }
            }

            return newMatrix;
        }
        
        public static object[] generateAiShips(int[,] matrix)
        {
            List<int[]> indexPairs = new List<int[]>();

            int[,] newMatrix = matrix;
            Random r = new Random();
            int[] ships = { 4, 2, 1 };

            ships[2]--;
            int tmpX = r.Next(1,6);
            int tmpY = r.Next(1,6);
            int rot = r.Next(0, 2);
            int[] tmpindx = new int[] { tmpX, tmpY, rot, 4};

            newMatrix = updateMatrix(tmpX,tmpY,4,newMatrix,rot);
            indexPairs.Add(tmpindx);


            while (ships[1] != 0)
            {
                tmpX = r.Next(1, 7);
                tmpY = r.Next(1, 7);
                rot = r.Next(0, 2);

                if (is_ship_placeable(tmpX, tmpY, 3, newMatrix, rot))
                {
                    tmpindx = new int[] { tmpX, tmpY, rot, 3};
                    newMatrix = updateMatrix(tmpX, tmpY, 3, newMatrix, rot);
                    ships[1]--;
                    indexPairs.Add(tmpindx);
                }
            }


            while (ships[0] != 0)
            {
                tmpX = r.Next(1, 8);
                tmpY = r.Next(1, 8);
                rot = r.Next(0, 2);

                if (is_ship_placeable(tmpX, tmpY, 2, newMatrix, rot))
                {
                    tmpindx = new int[] { tmpX, tmpY, rot, 2 };
                    newMatrix = updateMatrix(tmpX, tmpY, 2, newMatrix, rot);
                    ships[0]--;
                    indexPairs.Add(tmpindx);
                }
            }
            object[] ret_values = { newMatrix, indexPairs };

            return ret_values;
        }

        public static int[,] p1MakeAMove(int[,] matrix,int XX, int YY,ref int turn, int[] maxShips, ref int[] currShips, TextBlock[] aiShips,
            TextBlock turnName, TextBlock[] playerNames, List<int[]> aiShips_dict , ref int[] hit_miss, ref int[] destroyedIndex)
        {
            int[,] newMatrix = matrix;
            if (newMatrix[XX,YY] == 0)
            {
                newMatrix[XX, YY] = 1;
                turn = 2;
                hit_miss[1]++;
            }
            else if (newMatrix[XX,YY] > 1 && newMatrix[XX,YY] <= 4)
            {
                var shipType = newMatrix[XX, YY];
                newMatrix[XX, YY] = newMatrix[XX,YY] + newMatrix[XX,YY] * 10;
                turn = 1;
                hit_miss[0]++;

                List<int[]> tmpindx = new List<int[]>();
                tmpindx = checkIfShipDestroyed(newMatrix, XX, YY, shipType, new int[] {XX,YY});
                if(tmpindx.Count == shipType)
                {

                    var entries = tmpindx.Select(d =>
                    string.Format("\"{0}:{1}\"]", d[0], d[1]));
                    string messageBoxText = "{" + string.Join(",", entries) + "}";
                    string caption = "Player Ships Dict";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                    foreach (int[] indexes in tmpindx)
                    {
                        if (aiShips_dict.Any(p => p.SequenceEqual(indexes)))
                        {
                            destroyedIndex[0] = indexes[0];
                            destroyedIndex[1] = indexes[1];
                        }
                        newMatrix[indexes[0], indexes[1]] = shipType + shipType * 10 + 1;
                    }
                    currShips[shipType - 2]--;
                }
            }

            turnName.Text = (turn == 1 ? playerNames[0].Text : playerNames[1].Text);
            aiShips[0].Text = currShips[0] + "/" + maxShips[0];
            aiShips[1].Text = currShips[1] + "/" + maxShips[1];
            aiShips[2].Text = currShips[2] + "/" + maxShips[2];

            return newMatrix;
        }

        public static List<int[]> checkIfShipDestroyed(int[,] matrix, int XX, int YY, int shipType, int[] selectedIndx)
        {
            List<int[]> tmpList = new List<int[]>();

            for(int i = 0; i < 10; i++)
            {
                if (matrix[i,YY] == shipType + shipType * 10)
                {
                    tmpList.Add(new int[]{i,YY});
                    if(tmpList.Count == shipType && tmpList.Any(p => p.SequenceEqual(selectedIndx)))
                    {
                        Debug.WriteLine("i: " + tmpList.Count + " - " + i + ":" + YY + " - ShipType: " + shipType);
                        return tmpList;
                    }
                    else if (tmpList.Count == shipType && !tmpList.Any(p => p.SequenceEqual(selectedIndx)))
                    {
                        tmpList = new List<int[]>();
                    }
                }
                else
                {
                    tmpList = new List<int[]>();
                }
                Debug.WriteLine("i: " + tmpList.Count + " - " + i + ":" + YY + " - ShipType: " + shipType);
            }

            tmpList = new List<int[]>();

            for (int j = 0; j < 10; j++)
            {
                if (matrix[XX, j] == shipType + shipType * 10)
                {
                    tmpList.Add(new int[] { XX, j });
                    if (tmpList.Count == shipType && tmpList.Any(p => p.SequenceEqual(selectedIndx)))
                    {
                        Debug.WriteLine("j: " + tmpList.Count + " - " + XX + ":" + j + " - ShipType: " + shipType);
                        return tmpList;
                    }
                    else if(tmpList.Count == shipType && !tmpList.Any(p => p.SequenceEqual(selectedIndx)))
                    {
                        tmpList = new List<int[]>();
                    }
                }
                else
                {
                    tmpList = new List<int[]>();
                }
                Debug.WriteLine("j: " + tmpList.Count + " - " + XX + ":" + j + " - ShipType: " + shipType);
            }

            return new List<int[]>();
        }
    }
}
