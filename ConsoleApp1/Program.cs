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
        var (new_x, new_y) = Vertices.NewVertex(x1, y1, x2, y2, rotation);
        Console.WriteLine($"New Vertex: ({new_x}, {new_y})");

    }
}
public class Vertices
{
    public static (int new_x, int new_y) NewVertex(int x1, int y1, int x2, int y2, int rotation)
    {
        int new_x = x1, new_y = y1 ;
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
                // pośredni 
                Console.Write($"Intermediate vertex: ({x1}, {y1})\n");
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
                // pośredni 
                Console.Write($"Intermediate vertex: ({x1}, {y1})\n");
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
                Console.Write($"Intermediate vertex: ({x1}, {y1})\n");
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
                Console.Write($"Intermediate vertex: ({x1}, {y1})\n");
                new_x = x2;
                new_y = y1;
            }


        }
        return (new_x, new_y);

    }
}
////public class Vertices
//{
//    public static (int new_x, int new_y) NewVertex(int x1, int y1, int x2, int y2, int rotation)
//    {
//        int new_x = x2, new_y = y1;

//        // Calculate intermediate vertex based on rotation
//        switch (rotation)
//        {
//            case 0: // Entrance from top
//                new_y = y1 < 0 ? y1 + 1 : y1 - 1;
//                break;
//            case 1: // Entrance from left
//                new_x = x1 < 0 ? x1 - 1 : x1 + 1;
//                break;
//            case 2: // Entrance from bottom
//                new_y = y1 < 0 ? y1 - 1 : y1 + 1;
//                break;
//            case 3: // Entrance from right
//                new_x = x1 < 0 ? x1 + 1 : x1 - 1;
//                break;
//        }

//        // Print intermediate vertex
//        Console.Write($"Intermediate vertex: ({x1}, {new_y})\n");

//        return (new_x, new_y);
//    }
//}
