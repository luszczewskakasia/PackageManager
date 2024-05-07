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


    private void Start() 
    {

        this.Warehouses = new Dictionary<int, warehouse>();

        this.Add_warehouse(new warehouse("Krk", (7), (-13), -2, 250, 250, 250));
        this.Add_warehouse(new warehouse("WWa", (-5), (-2), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("WWa", (5), (-4), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("WWa", (-1), (-10), 2, 350, 350, 350));

        this.Line_start_x = 0;
        this.Line_start_y = 0;
        this.sort_method = "Destination";
    }

    private void Update()
    {
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