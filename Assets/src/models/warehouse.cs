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
    public int shelves_number;


    public warehouse(string Destination, int X, int Y, float rotation, int BigPackagesSlots, int MediumPackagesSlots, int SmallPachagesSlots)
    {
        this.Destination = Destination;
        this.BigPackagesSlots = BigPackagesSlots;
        this.MediumPackagesSlots = MediumPackagesSlots;
        this.SmallPackagesSlots = SmallPachagesSlots;
        this.Empty_slots = new List<int> { BigPackagesSlots, MediumPackagesSlots, SmallPachagesSlots };
        this.PackegesOverload = new List<bool> { false, false, false };
        this.storageList = new Dictionary<string, int>();
        this.LocationX = (float)(X * 13.05);
        this.LocationY = (float)(Y * 13.05);
        this.rotation = rotation;
        //this.shelves_number = (int)Math.Ceiling((BigPackagesSlots * 3 + Math.Ceiling(MediumPackagesSlots * 1.5) + SmallPachagesSlots)/45);
        Debug.Log($"{this.shelves_number}");

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

    public (int length_, int width_) optimizerectangle()
    {
        int squareroot = (int)Math.Sqrt(this.shelves_number);
        int length_ = squareroot; 
        int width_ = squareroot; 

        while (length_ * width_ != this.shelves_number)
        {
            if (length_ * width_ < this.shelves_number)
            {
                width_++;
            }
            else
            {
                break;
            }
        }

        return (length_, width_);
    }

    public void Add_MeshObject(GameObject InstanceML,float MLwidth, float MLlength, float MLheigth,int ID) 
    {
        this.shelves_number = (int)Math.Ceiling((this.BigPackagesSlots * 3 + Math.Ceiling(this.MediumPackagesSlots * 1.5) + this.SmallPackagesSlots) / 45);
        float start_x = this.LocationX - 2.5f * MLlength - 1.5f * MLheigth;
        float incrementation;
        float new_rotation;
        float new_y;
        (int length_, int width_) = optimizerectangle();
        Debug.Log($"{length_}, {width_}");

        GameObject instantiatedObject = new GameObject($"Warehouse{this.Destination}{ID}");
        int shelf_counter = 0;
        int counter_offset_l = 0;
        float offset_l = 0;
        //ok na parzyste
        if (length_ % 2 == 0) {
            offset_l = -((length_ * MLlength + (MLlength + 16f) / 2);
        }
        else
        {
            offset_l = -((length_ * MLlength + (MLlength + 16f) - (MLlength + 16f - MLlength/4) / 2);
        }

        int path_l = length_ / 2;
        for (int l_index = 0; l_index < length_; l_index++)
        {
            if (l_index == path_l) {
                // do ewentualnej zmiany
                offset_l += MLlength+16f;
                counter_offset_l++;
            }
            else
            {
                offset_l += MLlength;
            }

            float offset_w = 16f;// (width_ * MLwidth + ((width_ / 2) * MLwidth)); //16f
            for (int w_index = 0; w_index < width_; w_index++) 
            {
                if (w_index % 2 == 0)
                {
                    new_rotation = 180f;
                    // do ewentualnej zmiany
                    offset_w += 10f;
                    

                }
                else
                {
                    new_rotation = 0f;
                    offset_w += MLwidth;

                }

                for (int hight_number = 0; hight_number < 5; hight_number++) 
                {

                    GameObject InstanceWarehouse = Instantiate(InstanceML, new Vector3(this.LocationX+offset_l, 1f + hight_number* MLheigth, this.LocationY+offset_w), Quaternion.Euler(new Vector3(-90f, this.rotation+new_rotation, 0)));
                    InstanceWarehouse.name = $"Shelf{4*(w_index-1)+l_index+(4+4)*hight_number}";
                    InstanceWarehouse.transform.SetParent(instantiatedObject.transform);

                }
                shelf_counter++;
                if (shelf_counter >= shelves_number)
                {
                    break;
                }
            }

        }
        int wall_length = (int)((length_ * MLlength + ((length_ / 2) - 1) * (MLlength + 16f)) + 2 * 13.05);
        float wall_width = 2*13.05f + width_ * MLwidth + ((width_ / 2) * MLwidth + 2*13.05f);

        //dluzsze sciany
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // x, y, z, gdzie y to wysokosc
        cube.transform.position = new Vector3(0, 8f, MLlength/2);
        cube.transform.localScale = new Vector3(wall_length, 15f, 1f);

        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube2.transform.position = new Vector3(0, 8f, wall_width);
        cube2.transform.localScale = new Vector3(wall_length, 15f, 1f);

        //krotsze
        GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube3.transform.position = new Vector3(wall_length/2, 8f, wall_width/2 + MLlength / 4);
        cube3.transform.localScale = new Vector3(1f, 15f, wall_width- MLlength / 2+1);

        GameObject cube4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube4.transform.position = new Vector3(-wall_length / 2, 8f, wall_width / 2 + MLlength / 4);
        cube4.transform.localScale = new Vector3(1f, 15f, wall_width - MLlength / 2 + 1);

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

