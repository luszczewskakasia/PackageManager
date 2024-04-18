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
        var (new_x, new_y) = Vertices.NewVertex(x1, y1, x2, y2);
        Console.WriteLine($"New Vertex: ({new_x}, {new_y})");

    }
}
public class Vertices
{
    public static (int new_x, int new_y) NewVertex(int x1, int y1, int x2, int y2 )
    {
        int new_x, new_y;
        // case gdy krótsza ściana jest na osi x
        // czyli trzeba znaleźć boki krótszej ściany z sprawdzić czy zmeiniają się dla x czy dla y. jak dla X, to to jest
        // ten case
        //if (x1 == x2)
        //{
        //    new_x = x1;
        //    new_y = y1;
        //}
        //else
        //{
        //    if (y1 < 0)
        //    {
        //        y1 += 1;
        //    }
        //    else
        //    {
        //        y1 -= 1;
        //    }
        //    // pośredni 
        //    Console.Write($"Intermediate vertex: ({x1}, {y1})\n");
        //    new_x = x2;
        //    new_y = y1;

        //}
        //case gdy sciana jest na osi y

        if (y1 == y2)
        {
            new_x = x1;
            new_y = y1;
        }
        else
        {
            //tu trzeba jeszcze dorzucić przypadek jak ten kontener jest skierowany. Dla wjazdu linii od prawej jest git, bo to jest
            //ten przypadek
            //
            //Dla wjazdu od prawej musi być na odwrót z tą inkrementacją i dekrementacją x.
            // czyli czy to oznacza, że musimy znać współrzędne magazynu i na ich podstawie stworzyć ten kod? XD
            if (x1 < 0)
            {
                x1 -= 1;
            }
            else
            {
                x1 += 1;
            }
            Console.Write($"Intermediate vertex: ({x1}, {y1})\n");
            new_x = x2;
            new_y = y1;

        }
        return (new_x, new_y);

    }
}