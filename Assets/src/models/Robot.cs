//using UnityEngine;

//[System.Serializable]
//public class Robot : MonoBehaviour
//{
//    public Simulation simulation;
//    private warehouse Warehouse;

//    private void Start()
//    {
//        //if (simulation != null)
//        //{
//        //    int targetWarehouseId = 0; // ID of the warehouse you want to retrieve

//        //    // Check if the warehouse with the target ID exists in the simulation
//        //    if (simulation.Warehouses.ContainsKey(targetWarehouseId))
//        //    {
//        //        Warehouse = simulation.Warehouses[targetWarehouseId];

//        //        float X = Warehouse.LocationX;
//        //        float Y = Warehouse.LocationY;

//        //        Debug.Log($"Warehouse Location: X = {X}, Y = {Y}");
//        //    }
//        //    else
//        //    {
//        //        Debug.Log("Warehouse ID not found in the simulation.");
//        //    }
//        //}
//        //else
//        //{
//        //    Debug.Log("Simulation reference not assigned.");
//        //}
//    }


//    private void Update()
//    {
//        float X = Warehouse.LocationX;
//        float Y = Warehouse.LocationY;

//        // Set the object's position
//        transform.position = new Vector3(X, 0, Y);
//    }

//}

using UnityEngine;

[System.Serializable]
public class Robot : MonoBehaviour
{
    public Simulation simulation;
    public GameObject robot_prefab;
    private warehouse Warehouse;
    public Rigidbody RiggedBody;
    public string warehouse_name;

    private void Start()
    {
    }

    private void Update()
    {
        // Ensure simulation and Warehouse are not null
        if (simulation != null && Warehouse != null)
        {
            // Get the X and Y location from the Warehouse
            float X = Warehouse.LocationX;
            float Y = Warehouse.LocationY;

            // Set the object's position
            this.robot_prefab.transform.position = new Vector3(X, 0, Y);
            //Time.deltaTime

        }
    }
}
