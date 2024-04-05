//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Simulation: MonoBehaviour
//{ 
//    public List<warehouse> Warehouses;
//    public string sort_method;
//    public float Line_start_x;
//    public float Line_start_y;
//    public SortLine SortLineStructure;
//    private void Start() 
//    {
//        Warehouses = new List<warehouse> ();
//        for (int i = 0; i < 2; i++) 
//        {
//            Warehouses.Add( new warehouse("Krk", i, 1f, 1f, 1f, i, i, i));

//        }
//        Line_start_x = 0f;
//        Line_start_y = 0f;
//        sort_method = "Destination";
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public List<warehouse> Warehouses;

    private void Start()
    {
        Warehouses = new List<warehouse>();
        for (int i = 0; i < 5; i++)
        {
            Warehouses.Add(new warehouse("Krk", i, 1f, 1f, 1f, i, i, i));

        }
    }
}