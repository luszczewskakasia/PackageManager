using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation: MonoBehaviour
{ 
    public List<warehouse> Warehouses;

    private void Start() 
    {
        Warehouses = new List<warehouse> ();
        for (int i = 0; i < 5; i++) 
        {
            Warehouses.Add( new warehouse("Krk", i, 1f, 1f, 1f, i, i, i));

        }
    }
}
