using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

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
        
        public static int[,] generateAiShips(int[,] matrix)
        {
            int[,] newMatrix = matrix;
            Random rotation = new Random();
            Random positionX = new Random();
            Random positionY = new Random();
            int[] ships = { 4, 2, 1 };
          
                int tmpX = positionX.Next(1,5);
                int tmpY = positionY.Next(1,5);
                int rot = rotation.Next(0, 2);

                newMatrix = updateMatrix(tmpX,tmpY,4,newMatrix,rot);


            /*
                 tmpX = positionX.Next(1, 6);
                 tmpY = positionY.Next(1, 6);
                 rot = rotation.Next(0, 1);

                if (is_ship_placeable(tmpX, tmpY, 3, newMatrix, rot))
                {
                    newMatrix = updateMatrix(tmpX, tmpY, 3, newMatrix, rot);
                    ships[1]--;
                }
                
            

            
                 tmpX = positionX.Next(1, 7);
                 tmpY = positionY.Next(1, 7);
                 rot = rotation.Next(0, 1);

                if (is_ship_placeable(tmpX, tmpY, 2, newMatrix, rot))
                {
                    newMatrix = updateMatrix(tmpX, tmpY, 2, newMatrix, rot);
                    ships[0]--;
                }
            */
            


            return newMatrix;
        }
    }
}
