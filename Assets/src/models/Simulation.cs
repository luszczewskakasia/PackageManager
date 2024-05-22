using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using System.Threading;
using System.Drawing;


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
        if (x <= MaxX && x >= MinX && y <= MaxY && y >= MinY ) { return true; }
        else { return false; }
    }
    public bool Check_edge(Node point1, Node point2)
    {
        if ((point1.y == point2.y && point1.y < MaxY && point1.y > MinY) && ((point1.x > MaxX && point2.x < MinX)||(point2.x > MaxX && point1.x < MinX)) ) 
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



public class Simulation: MonoBehaviour
{
    [JsonProperty]
    public Dictionary<int,warehouse> Warehouses { get; set; }
    public string sort_method;
    public int Line_start_x;
    public int Line_start_y;
    private int last_id = 0;
    private int last_pack_id = 0;
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


    private void Start() 
    {

        this.Warehouses = new Dictionary<int, warehouse>();
        this.warehouseFileds = new List<WarehouseFiled>();

        // start
        this.Add_warehouse(new warehouse("ODW", (-20), (0), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("ODW", (0), (-50), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("ODW", (50), (0), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("ODW", (0), (30), 1, 50, 50, 50));
        //wokó³ górnego
        this.Add_warehouse(new warehouse("GR", (-15), (35), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("GR", (15), (35), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("GR", (-15), (15), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("GR", (15), (20), 2, 50, 50, 50));


        //wokó³ lewego
        this.Add_warehouse(new warehouse("LEFT", (-20), (15), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-35), (15), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-20), (-15), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-35), (-15), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-20), (15), 2, 50, 50, 50));
        this.Add_warehouse(new warehouse("LEFT", (-35), (15), 1, 50, 50, 50));

        //wokó³ dolnego
        this.Add_warehouse(new warehouse("DOL", (15), (-65), 1, 50, 50, 50));
        this.Add_warehouse(new warehouse("DOL", (15), (-50), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("DOL", (-15), (-50), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("DOL", (-15), (-65), 1, 50, 50, 50));

        //wokó³ prawego
        this.Add_warehouse(new warehouse("RIGHT", (20), (-15), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("RIGHT", (35), (-15), -1, 50, 50, 50));
        this.Add_warehouse(new warehouse("RIGHT", (20), (15), -2, 50, 50, 50));
        this.Add_warehouse(new warehouse("RIGHT", (35), (15), 1, 50, 50, 50));
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
        int destroy = New_Whouse.Add_MeshObject(ML_Mesh,ML_W,ML_L,ML_H, last_id, warehouseFileds);
        if (destroy == 0)
        {
            this.Warehouses[last_id] = New_Whouse;
            last_id++;
            return;
        }
        Debug.Log("Magazyn na magazynie lub Magazyn na œcie¿ce");
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
     public void Add_package_instance(List<int> size_list,List<string> destination_list) 
    {
        int New_Packeges_Count=0;

        if (size_list.Count > destination_list.Count){ New_Packeges_Count = destination_list.Count; }
        else { New_Packeges_Count = size_list.Count; }
        for(int i= 0; i < New_Packeges_Count; i++) 
        {
            int? Final_warehouse_id = null;
            string Pack_id = "";
            foreach (int j in this.Warehouses.Keys) 
            {
                if (! Warehouses[j].PackegesOverload[size_list[i]])
                { 
                   Final_warehouse_id = j; 
                   Pack_id += $"{destination_list[i]}_{last_pack_id}_{j}";
                   last_pack_id ++;
                   break;
                } 
            }
            if (Final_warehouse_id != null) 
            {
                GameObject New_Package_mesh;
                Package New_Package;
                switch (size_list[i]) 
                {
                    case 0:
                        New_Package_mesh = Instantiate(Small_Package_Mesh_object,
                            new Vector3(Line_start_x, 5, Line_start_y), Quaternion.Euler(new Vector3(0, 0, 0)));
                        New_Package = new Package(size_list[i], Pack_id, (int)Final_warehouse_id,
                        this.Warehouses[(int)Final_warehouse_id].path, New_Package_mesh) ;
                        Warehouses[(int)Final_warehouse_id].New_packege(size_list[i], New_Package);
                        break;
                    case 1:
                        New_Package_mesh = Instantiate(Medium_Package_Mesh_object,
                            new Vector3(Line_start_x, 5, Line_start_y), Quaternion.Euler(new Vector3(0, 0, 0)));
                        New_Package = new Package(size_list[i], Pack_id, (int)Final_warehouse_id,
                        this.Warehouses[(int)Final_warehouse_id].path, New_Package_mesh);
                        Warehouses[(int)Final_warehouse_id].New_packege(size_list[i], New_Package);
                        break;
                    case 2:
                        New_Package_mesh = Instantiate(Big_Package_Mesh_object,
                            new Vector3(Line_start_x, 5, Line_start_y), Quaternion.Euler(new Vector3(0, 0, 0)));
                        New_Package = new Package(size_list[i], Pack_id, (int)Final_warehouse_id,
                        this.Warehouses[(int)Final_warehouse_id].path, New_Package_mesh);
                        Warehouses[(int)Final_warehouse_id].New_packege(size_list[i], New_Package);
                        break;
                }

            }
        }




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