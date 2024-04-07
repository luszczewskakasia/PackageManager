using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
    public List<KeyValuePair> Warehouses;
    public string sort_method;
    public float Line_start_x;
    public float Line_start_y;
    private int last_id = 0;
    public GameObject warehouse_Mesh;
    private void Start() 
    {
        this.Warehouses = new List<KeyValuePair>();
        for (int i = 0; i < 5; i++)
        {
            this.Add_warehouse(new warehouse("Krk", (float)(i*3), (float)(i * 3), (float)(i * 3), i, i, i));
        }
        this.Line_start_x = 0f;
        this.Line_start_y = 0f;
        this.sort_method = "Destination";
    }

    private void Update()
    {
    }


    public List<KeyValuePair> GetList() { return Warehouses; }
    public void SetList(List<KeyValuePair> data) { Warehouses = data; }
    public void set_start(float x, float y) 
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
        GameObject InstanceWarehouse = Instantiate(warehouse_Mesh, new Vector3(New_Whouse.LocationX, 1f, New_Whouse.LocationY),Quaternion.Euler(new Vector3(0, New_Whouse.rotation, 0)));
        New_Whouse.Add_MeshObject(InstanceWarehouse);
        this.Warehouses.Add(new KeyValuePair(last_id, New_Whouse));
        last_id++;
    }

    public void Delete_Warehouse(int keyToRemove)
    {
        for (int i = 0; i < Warehouses.Count; i++)
        {
            if (Warehouses[i].key == keyToRemove)
            {
                Destroy(this.Warehouses[i].val.instantiatedObject);
                Warehouses.RemoveAt(i);
                break;
            }

        }
    }
     




}
