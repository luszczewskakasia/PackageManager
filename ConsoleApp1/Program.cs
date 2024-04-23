using System;
using System.ComponentModel.Design;
public class Program
{
    public static void Main()
    {
        //x1, y1 - niestartowe punkty
        //x2, y2 - startowe punkty
        int x1 = 4;
        int y1 = -7;
        int x2 = 0;
        int y2 = 0;
        int rotation = 3;
        List<(int, int)> result = Vertices.NewVertex(x1, y1, x2, y2, rotation);
        foreach (var point in result)
        {
            Console.WriteLine($"({point.Item1}, {point.Item2})");
        }

    }
}
public class Vertices
{
    public static List<(int, int)> NewVertex(int x1, int y1, int x2, int y2, int rotation)
    {
        int new_x = x1, new_y = y1;
        List<(int, int)> points = new List<(int, int)>();
        // case gdy krótsza ściana jest na osi x
        // czyli trzeba znaleźć boki krótszej ściany z sprawdzić czy zmeiniają się dla x czy dla y. jak dla X, to to jest
        // ten case
        if (x1 == x2)
        {
            new_x = x1;
            new_y = y1;
        }
        else
        {
            //wejście od góry
            if (rotation == 0)
            {
                if (y1 < 0)
                {
                    y1 += 1;
                }
                else
                {
                    y1 -= 1;
                } 
                new_x = x2;
                new_y = y1;

            }
            //wejście od dołu
            else if (rotation == 2)
            {
                if (y1 < 0)
                {
                    y1 -= 1;
                }
                else
                {
                    y1 += 1;
                }
                new_x = x2;
                new_y = y1;

            }


        }
        //case gdy sciana jest na osi y

        if (y1 == y2)
        {
            new_x = x1;
            new_y = y1;
        }
        else
        {
            //wejście od prawej
            if (rotation == 1)
            {
                if (x1 < 0)
                {
                    x1 -= 1;
                }
                else
                {
                    x1 += 1;
                }
                new_x = x1;
                new_y = y2;
            }
            //wejście od lewej
            else if (rotation == 3)
            {
                if (x1 < 0)
                {
                    x1 += 1;
                }
                else
                {
                    x1 -= 1;
                }
                new_x = x2;
                new_y = y1;
            }


        }
        points.Add((x1, y1));
        points.Add((new_x, new_y));

        return points;

    }
}
