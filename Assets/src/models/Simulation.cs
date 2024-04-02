using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Simulation: MonoBehaviour
{
    private List<int> Warehouses;

    void Start() 
    {
        Warehouses = new List<int>();
        for (int i = 0; i < 10; i++) 
        {
            Warehouses.Add(i);
        }


    }

}
