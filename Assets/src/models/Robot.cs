//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//[System.Serializable]
//public class Robot : MonoBehaviour
//{
//    private warehouse Warehouse;
//    private void Start() 
//    {
//        Warehouse = new warehouse(("ODW", (-20), (0), -2, 50, 50, 50));
//        float X = Warehouse.LocationX;
//        float Y = Warehouse.LocationY;

//        Debug.Log($"Warehouse Location: X = {warehouseLocationX}, Y = {warehouseLocationY}");

//    }
//    private void Update() { }

//}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Robot : MonoBehaviour
{
    private Simulation simulation;
    private warehouse Warehouse;

    private void Start()
    {
        // Find the Simulation instance in the scene
        //simulation = 0;

        if (simulation != null)
        {
            // Assuming you want to get the warehouse with ID 0
            int warehouseId = 0;

            if (simulation.Warehouses.TryGetValue(warehouseId, out Warehouse))
            {
                float X = Warehouse.LocationX;
                float Y = Warehouse.LocationY;

                Debug.Log($"Warehouse Location: X = {X}, Y = {Y}");
            }
            else
            {
                Debug.LogError("Warehouse ID not found in the simulation.");
            }
        }
        else
        {
            Debug.LogError("Simulation instance not found.");
        }
    }

    private void Update()
    {
        // Update logic if needed
    }
}
