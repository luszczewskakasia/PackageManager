using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

[System.Serializable]

public class warehouse:MonoBehaviour
{
    //dane do inicjalizacji
    public string Destination;
    public int BigPackagesSlots;
    public int MediumPackagesSlots;
    public int SmallPackagesSlots;

    //dane operacyjne
    private List<int> Empty_slots;
    private List<bool> PackegesOverload;
    private Dictionary<string, int> storageList;
    [JsonIgnore]
    public GameObject instantiatedObject;

    //dane z linii produkcyjnej
    public float LocationX;
    public float LocationY;
    public float rotation;
    [JsonIgnore]
    public bool useGUILayout;

    public warehouse(string Destination, float LocationX, float LocationY, float rotation, int BigPackagesSlots, int MediumPackagesSlots, int SmallPachagesSlots)
    {
        this.Destination = Destination;
        this.BigPackagesSlots = BigPackagesSlots;
        this.MediumPackagesSlots = MediumPackagesSlots;
        this.SmallPackagesSlots = SmallPachagesSlots;
        this.Empty_slots = new List<int> { BigPackagesSlots, MediumPackagesSlots, SmallPachagesSlots };
        this.PackegesOverload = new List<bool> { false, false, false };
        this.storageList = new Dictionary<string, int>();
        this.LocationX = LocationX;
        this.LocationY = LocationY;
        this.rotation = rotation;
    }

    //odbiór paczek z pomieszczenia
    private void Shipping(List<string> shippingList)
    {
        foreach (string packageID in shippingList)
        {
            int size = storageList[packageID];
            this.Empty_slots[size]++;
            if (PackegesOverload[size]) { PackegesOverload[size] = false; }
            storageList.Remove(packageID);
        }
    }

    //dodaj paczkê nadaj ID
    private void New_packege(int size, Package new_packge)
    {
        if (!PackegesOverload[size])
        {
            Empty_slots[size]--;
            if (Empty_slots[size] == 0) { PackegesOverload[size] = true; }
            new_packge.setID("KRK", 12);
        }
    }

    public warehouse get()
    {
        return this;
    }

    public void Add_MeshObject(GameObject InstanceS, GameObject InstanceML,float MLwidth, float MLlength, float MLheigth) 
    {
        float start_x = this.LocationX - 2.5f * MLlength - 1.5f * MLheigth;
        float incrementation;
        float new_rotation;
        float new_y;

        for (int current_column_number = 0; current_column_number < 14; current_column_number++)
        {
            if (current_column_number % 4 < 2)
            {
                incrementation = MLlength;
                new_rotation = 90f;
            }
            else
            {
                new_rotation = 270f;
                incrementation = MLwidth;
            }
            for (int current_row_number = 1; current_row_number < 5; current_row_number++) 
            {
                if (current_column_number % 2 == 0)
                {
                    new_y = this.LocationY - current_row_number * MLlength;
                }
                else
                {
                    new_y = this.LocationY + current_row_number * MLlength;
                }
                for (int hight_number = 0; hight_number < 5; hight_number++) 
                {

                    GameObject InstanceWarehouse = Instantiate(InstanceML, new Vector3(start_x, 1f + hight_number* MLheigth, new_y), Quaternion.Euler(new Vector3(-90f, new_rotation, 0)));
                }
            }

            if(current_column_number%2 == 1) 
            {
                start_x += incrementation;
            }

        }
        this.instantiatedObject = InstanceS;    
    }

    public void UpdateMeshRotation(float posX, float posY)
    {
        this.LocationX = posX;
        this.LocationY = posY;

        if (instantiatedObject != null)
        {
            this.instantiatedObject.transform.position = new Vector3(posX, 1f, posY);
        }
    }

    public void UpdateMeshPosition(float Rot)
    {
        this.rotation = Rot;
        if (instantiatedObject != null)
        {
            this.instantiatedObject.transform.position = new Vector3(0, Rot, 0);

        }

    }

    public string ToJson()
    {

        string responseJson = JsonUtility.ToJson(this);
        return responseJson;
    }
}

