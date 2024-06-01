using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using System.Threading;
using System.Drawing;
using Unity.VisualScripting;


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

public class WarehouseFiled
{
    public int MaxX;
    public int MaxY;
    public int MinY;
    public int MinX;
    public WarehouseFiled(int minx, int miny, int maxx, int maxy) { this.MaxX = maxx; this.MaxY = maxy; this.MinX = minx; this.MinY = miny; }

    public bool Check_point(Node point)
    {
        if (point.x < MaxX && point.x > MinX && point.y < MaxY && point.y > MinY && !point.final) { return true; }
        else { return false; }
    }
    public bool Check_point(int x, int y)
    {
        if (x <= MaxX && x >= MinX && y <= MaxY && y >= MinY) { return true; }
        else { return false; }
    }
    public bool Check_edge(Node point1, Node point2)
    {
        if ((point1.y == point2.y && point1.y < MaxY && point1.y > MinY) && ((point1.x > MaxX && point2.x < MinX) || (point2.x > MaxX && point1.x < MinX)))
        {
            return true;
        }
        if ((point1.x == point2.x && point1.x < MaxX && point1.x > MinX) && ((point1.y > MaxY && point2.y < MinY) || (point2.y > MaxY && point1.y < MinY)))
        {
            return true;
        }


        else { return false; }
    }
};



public class Simulation : MonoBehaviour
{
    [JsonProperty]
    public Dictionary<int, warehouse> Warehouses { get; set; }
    public string sort_method;
    public int Line_start_x;
    public int Line_start_y;
    private int last_id = 0;
    private int last_pack_id = 0;
    private float spawn_delay;
    [JsonIgnore]
    public GameObject Small_Package_Mesh_object;
    [JsonIgnore]
    public GameObject Medium_Package_Mesh_object;
    [JsonIgnore]
    public GameObject Big_Package_Mesh_object;
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
    private List<WarehouseFiled> warehouseFileds;
    private List<queue_struct> Objects_to_spawn;
    public GameObject robot_prefab;

    private void Start()
    {

        this.Warehouses = new Dictionary<int, warehouse>();
        this.warehouseFileds = new List<WarehouseFiled>();

        // start
        this.Add_warehouse(new warehouse("KRK", (-20), (0), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("RZE", (0), (-50), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("LUB", (50), (0), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("BIAL", (0), (30), 1, 50, 50, 50));
        //wokó³ górnego
        this.Add_warehouse(new warehouse("GORZ", (-15), (35), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("BYD", (15), (35), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("GDA", (-15), (15), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("OLSZ", (15), (20), 2, 50, 50, 50));


        //wokó³ lewego
        this.Add_warehouse(new warehouse("KAT", (-20), (15), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("OPO", (-35), (15), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("WRC", (-20), (-15), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("SZCZ", (-35), (-15), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("POZ", (-20), (15), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("LOD", (-35), (15), 1, 50, 50, 50));

        //wokó³ dolnego
        this.Add_warehouse(new warehouse("WAR", (15), (-65), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("KIE", (15), (-50), -1, 50, 50, 50));

        this.Add_warehouse(new warehouse("TOR", (-15), (-50), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("ZIEL", (-15), (-65), 1, 50, 50, 50));

        //wokó³ prawego
        this.Add_warehouse(new warehouse("ZAK", (20), (-15), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("CZES", (35), (-15), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("PRZEM", (20), (15), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("KOSZ", (35), (15), 1, 50, 50, 50));
        this.Line_start_x = 0;
        this.Line_start_y = 0;
        this.spawn_delay = 3;
        this.Objects_to_spawn = new List<queue_struct>();

        //Dodawanie Paczek:

        List<int> sizes = new List<int> { 0, 0, 2, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 };
        List<string> Destinations = new List<string> { "KRK", "RZE", "LUB", "BIAL", "GORZ", "BYD", "GDA", "OLSZ", "KAT", "OPO", "WRC", "SZCZ", "POZ", "LOD", "WAR", "KIE", "TOR", "ZIEL", "ZAK", "CZES", "PRZEM", "KOSZ" };
        //Add_package_instance(sizes, Destinations);
        //List<int> sizes = new List<int> { 0 };
        //List<string> Destinations = new List<string> { "BYD" };
        Add_package_instance(sizes, Destinations);

        this.sort_method = "Destination";
    }

    void Update()
    {
        if (this.spawn_delay > 3)
        {
            if (this.Objects_to_spawn.Count > 0)
            {
                queue_struct First_in_queue = this.Objects_to_spawn[0];
                GameObject New_Package_mesh;
                Package package;
                switch (First_in_queue.size)
                {
                    case 0:
                        New_Package_mesh = Instantiate(Small_Package_Mesh_object,
                            new Vector3(Line_start_x, 3.5f, Line_start_y), Quaternion.Euler(new Vector3(0, 0, 0)));
                        package = New_Package_mesh.GetComponent<Package>();
                        package.Initialize(First_in_queue);
                        package.Add_Mesh_to_Package(New_Package_mesh);
                        this.Warehouses[package.warehouseID].New_package(package);
                        break;
                    case 1:
                        New_Package_mesh = Instantiate(Medium_Package_Mesh_object,
                            new Vector3(Line_start_x, 4f, Line_start_y), Quaternion.Euler(new Vector3(0, 0, 0)));
                        package = New_Package_mesh.GetComponent<Package>();
                        package.Initialize(First_in_queue);
                        package.Add_Mesh_to_Package(New_Package_mesh);
                        this.Warehouses[package.warehouseID].New_package(package);
                        break;
                    case 2:
                        New_Package_mesh = Instantiate(Big_Package_Mesh_object,
                            new Vector3(Line_start_x, 4.5f, Line_start_y), Quaternion.Euler(new Vector3(0, 0, 0)));
                        package = New_Package_mesh.GetComponent<Package>();
                        package.Initialize(First_in_queue);
                        package.Add_Mesh_to_Package(New_Package_mesh);
                        this.Warehouses[package.warehouseID].New_package(package);
                        break;
                }
                Objects_to_spawn.RemoveAt(0);

            }
            this.spawn_delay = 0;
        }
        this.spawn_delay += Time.deltaTime;
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
        int destroy = New_Whouse.Add_MeshObject(ML_Mesh, ML_W, ML_L, ML_H, last_id, warehouseFileds, robot_prefab);
        if (destroy == 0)
        {
            this.Warehouses[last_id] = New_Whouse;
            last_id++;
            return;
        }
        //Debug.Log("Magazyn na magazynie lub Magazyn na œcie¿ce");
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
    public void Add_package_instance(List<int> size_list, List<string> destination_list)
    {
        int New_Packeges_Count = 0;
        if (size_list.Count > destination_list.Count) { New_Packeges_Count = destination_list.Count; }
        else { New_Packeges_Count = size_list.Count; }
        for (int i = 0; i < New_Packeges_Count; i++)
        {
            {
                int? Final_warehouse_id = null;
                string Pack_id = "";
                foreach (int j in this.Warehouses.Keys)
                {
                    if (Warehouses[j].Destination == destination_list[i] && !Warehouses[j].PackegesOverload[size_list[i]])
                    {
                        Final_warehouse_id = j;
                        Pack_id += $"{destination_list[i]}_{last_pack_id}_{j}";
                        last_pack_id++;
                        break;
                    }
                }
                if (Final_warehouse_id != null)
                {
                    queue_struct New_Package;
                    switch (size_list[i])
                    {
                        case 0:
                            New_Package = new queue_struct(size_list[i], Pack_id, (int)Final_warehouse_id,
                            this.Warehouses[(int)Final_warehouse_id].path);
                            Objects_to_spawn.Add(New_Package);
                            break;
                        case 1:
                            New_Package = new queue_struct(size_list[i], Pack_id, (int)Final_warehouse_id,
                            this.Warehouses[(int)Final_warehouse_id].path);
                            Objects_to_spawn.Add(New_Package);
                            break;
                        case 2:
                            New_Package = new queue_struct(size_list[i], Pack_id, (int)Final_warehouse_id,
                            this.Warehouses[(int)Final_warehouse_id].path);
                            Objects_to_spawn.Add(New_Package);
                            break;
                    }

                }
            }
        }
    }

    public void robot_instance ()
    {

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