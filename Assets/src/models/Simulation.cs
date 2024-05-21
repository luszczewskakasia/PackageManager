using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


[System.Serializable]
public class KeyValuePair
{
    public int key;
    public warehouse val;

    public KeyValuePair(int key_, warehouse w_) 
    { 
        this.key = key_;
        this.val = w_; 
    }
}


[System.Serializable]
public class ID_List_To_Delete 
{
    public List<int> IDs;

    public ID_List_To_Delete() { this.IDs = new List<int>(); }

}





public class Simulation: MonoBehaviour
{
    [JsonProperty]
    public Dictionary<int,warehouse> Warehouses { get; set; }
    public string sort_method;
    public int Line_start_x;
    public int Line_start_y;
    private int last_id = 0;
    [JsonIgnore]
    public GameObject Small_Mesh;
    [JsonIgnore]
    public GameObject ML_Mesh;
    [JsonIgnore]
    public float ML_W;
    [JsonIgnore]
    public float ML_L;
    [JsonIgnore]
    public float ML_H;

    public GameObject small_package;
    //small_package.transform.position = new Vector3(0, 1, 0);
    //small_package.transform.localScale = new Vector3(1f, 1f, 1f);

    private void Start() 
    {

        this.Warehouses = new Dictionary<int, warehouse>();

        // start
        this.Add_warehouse(new warehouse("ODW", (-20), (0), -2, 50, 50, 50));
        //this.Add_warehouse(new warehouse("ODW", (0), (-50), -1, 50, 50, 50));
        //this.Add_warehouse(new warehouse("ODW", (50), (0), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("ODW", (0), (20), 1, 50, 50, 50));

        //wok� g�rnego
        this.Add_warehouse(new warehouse("GR", (-15), (35), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("GR", (15), (35), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("GR", (-15), (15), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("GR", (15), (20), 2, 50, 50, 50));


        //wok� lewego
        this.Add_warehouse(new warehouse("LEFT", (-20), (15), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-35), (15), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-20), (-15), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-35), (-15), -1, 50, 50, 50));

        //wok� dolnego
        //this.Add_warehouse(new warehouse("DOL", (15), (-65), 1, 50, 50, 50));
        //this.Add_warehouse(new warehouse("DOL", (15) , (-50), -1, 50, 50, 50));
        //this.Add_warehouse(new warehouse("DOL", (-15), (-50), -1, 50, 50, 50));
        //this.Add_warehouse(new warehouse("DOL", (-15), (-65), 1, 50, 50, 50));

        //
        this.Line_start_x = 0;
        this.Line_start_y = 0;
        this.sort_method = "Destination";

        this.small_package = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Change KeyCode.Space to the key you want to use
        {
            // Set the position where the object will be spawned
            Vector3 spawnPosition = new Vector3(0f, 1f, 0f);

            // Set the scale of the object
            Vector3 objectScale = new Vector3(1f, 1f, 1f);

            // Instantiate the object
            GameObject newObject = Instantiate(small_package, spawnPosition, Quaternion.identity);

            // Set the scale of the object
            newObject.transform.localScale = objectScale;
        }
    }



    public Dictionary<int, warehouse> GetList() { return Warehouses; }
    public void SetList(Dictionary<int, warehouse> data) { Warehouses = data; }
    public void set_start(int x, int y) 
    {
        this.Line_start_x = x;
        this.Line_start_y = y;
    }
    public void set_method(string meth) 
    {
        this.sort_method = meth;
    }

    public void Add_warehouse(warehouse New_Whouse) 
    {
        New_Whouse.Add_MeshObject(ML_Mesh,ML_W,ML_L,ML_H, last_id);
        this.Warehouses[last_id] = New_Whouse;
        last_id++;
    }

    public void Delete_Warehouse(int keyToRemove)
    {
        if (Warehouses.ContainsKey(keyToRemove))
        {
            string objectNameToDelete = $"Warehouse{Warehouses[keyToRemove].Destination}{keyToRemove}";
            GameObject objectToDelete = GameObject.Find(objectNameToDelete);
            if (objectToDelete != null)
            {
                Destroy(objectToDelete);
            }
            Warehouses.Remove(keyToRemove);
        }
    }

    public string ToJson()
    {
        string sb = "{ \"Warehouses\": [";
        foreach (int key in Warehouses.Keys)
        {
            warehouse warehouse_ = Warehouses[key];

            sb += $"{key}:";
            sb += "{";
            sb += $"\"Destination\": \"{warehouse_.Destination}\",";
            sb += $"\"BigPackagesSlots\": {warehouse_.BigPackagesSlots},";
            sb += $"\"MediumPackagesSlots\": {warehouse_.MediumPackagesSlots},";
            sb += $"\"SmallPackagesSlots\": {warehouse_.SmallPackagesSlots},";
            sb += $"\"Grid_X\": {warehouse_.Grid_X},";
            sb += $"\"Grid_Y\": {warehouse_.Grid_Y},";
            sb += $"\"MaxX\": {warehouse_.maxX},";
            sb += $"\"MaxY\": {warehouse_.maxY},";
            sb += $"\"MinX\": {warehouse_.minX},";
            sb += $"\"MinY\": {warehouse_.minY},";
            sb += $"\"Grid_rotation\": {warehouse_.Grid_rotation}";
            sb += "},"; 
        }

        if (sb.EndsWith(","))
        {
            sb = sb.Remove(sb.Length - 1);
        }

        sb += "]}";

        return sb;
    }



}
//public string Destination;
//public int BigPackagesSlots;
//public int MediumPackagesSlots;
//public int SmallPackagesSlots;

////dane operacyjne
//private List<int> Empty_slots;
//private List<bool> PackegesOverload;
//private Dictionary<string, int> storageList;
//[JsonIgnore]
//public GameObject instantiatedObject;

////dane z linii produkcyjnej
//public float LocationX;
//public float LocationY;
//public float rotation;