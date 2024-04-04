using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Package
{
    public int size;
    public string destination;
    private string ID;
    private int? warehouseID;

    public Package(int size, string destination)
    {
        this.size = size;
        this.destination = destination;
        this.ID = null;
        this.warehouseID = null;
    }

    public void setID(string ID, int warehouseID)
    {
        this.ID = ID;
        this.warehouseID = warehouseID;
    }

    public Package get()
    {
        return this;
    }

}

