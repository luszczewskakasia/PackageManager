using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.InputSystem;
using System.Xml.Linq;

[System.Serializable]

public class warehouse : MonoBehaviour
{
    //dane do inicjalizacji
    public string Destination;
    public int BigPackagesSlots;
    public int MediumPackagesSlots;
    public int SmallPackagesSlots;
    public Dictionary<string, Shelf> Shelf_List;

    //dane operacyjne
    public List<int> Empty_slots;
    public List<bool> PackegesOverload;
    private Dictionary<string, Shelf> storageList;
    [JsonIgnore]
    public GameObject instantiatedObject;

    //dane z linii produkcyjnej
    public int Grid_X;
    public int Grid_Y;
    public int Grid_rotation;

    public float LocationX;
    public float LocationY;
    private float rotation;

    public List<Node> path;

    public int maxX;
    public int maxY;
    public int minX;
    public int minY;
    public bool Packege_in_delivery = false;
    public int shelves_number;

    public Robot robot;
    private int warehouse_id;


    public warehouse(string Destination, int X, int Y, int rotation, int BigPackagesSlots, int MediumPackagesSlots, int SmallPachagesSlots)
    {
        this.Destination = Destination;
        this.BigPackagesSlots = BigPackagesSlots;
        this.MediumPackagesSlots = MediumPackagesSlots;
        this.SmallPackagesSlots = SmallPachagesSlots;
        this.Empty_slots = new List<int> { BigPackagesSlots, MediumPackagesSlots, SmallPachagesSlots };
        this.PackegesOverload = new List<bool> { false, false, false };
        this.storageList = new Dictionary<string, Shelf>();
        this.Grid_X = X;
        this.Grid_Y = Y;
        this.Grid_rotation = rotation;
        this.Shelf_List = new Dictionary<string, Shelf>();

        this.LocationX = (float)(X * 13.05);
        this.LocationY = (float)(Y * 13.05);
        switch (rotation)
        {
            case -1: this.rotation = 0f; break;
            case -2: this.rotation = 90f; break;
            case 1: this.rotation = 180f; break;
            case 2: this.rotation = -90; break;
        }
        this.shelves_number = (int)Math.Ceiling((BigPackagesSlots * 3 + Math.Ceiling(MediumPackagesSlots * 1.5) + SmallPachagesSlots) / 36);

    }


    //odbiór paczek z pomieszczenia
    //private void Shipping(List<string> shippingList)
    //{
    //    foreach (string packageID in shippingList)
    //    {
    //        int size = storageList[packageID];
    //        this.Empty_slots[size]++;
    //        if (PackegesOverload[size]) { PackegesOverload[size] = false; }
    //        storageList.Remove(packageID);
    //    }
    //}

    //dodaj paczkê nadaj ID

    public void New_package(Package new_package)
    {
        if (!PackegesOverload[new_package.size])
        {
            Empty_slots[new_package.size]--;
            if (Empty_slots[new_package.size] == 0) { PackegesOverload[new_package.size] = true; }
        }

        foreach (var shelfEntry in this.Shelf_List)
        {
            Shelf shelfValue = shelfEntry.Value;

            float emptySlots = shelfValue.GetEmptySlots();
            if (emptySlots > 0.0f)
            {
                Vector3 shelfPosition = shelfValue.GetPosition();
                new_package.Place_on_Shelf = shelfPosition;
                new_package.shelf_name = shelfValue.name;
                new_package.shelf_rot = shelfValue.rotation;
                new_package.Mag_side = shelfValue.Mag_side;

                this.storageList.Add(new_package.ID, shelfValue);
                this.Packege_in_delivery = true;
                break;
            }
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

    public int Add_MeshObject(GameObject InstanceML, float MLwidth, float MLlength, float MLheigth, int ID, List<WarehouseFiled> borders, GameObject robot_prefab)
    {
        this.shelves_number = (int)Math.Ceiling((this.BigPackagesSlots * 3 + Math.Ceiling(this.MediumPackagesSlots * 1.5) + this.SmallPackagesSlots) / 45);
        float start_x = this.LocationX - 2.5f * MLlength - 1.5f * MLheigth;
        float incrementation;
        float new_rotation = 0f;
        float new_y;
        float max_offset_w = 0f;
        (int length_, int width_) = optimizerectangle();
        //Debug.Log($"{length_}, {width_}");

        float offset_w = 16f;

        for (int w_index = 0; w_index < width_; w_index++)
        {
            if (w_index % 2 == 0)
            {
                offset_w += 20f;
            }
            else
            {
                offset_w += MLwidth;
            }
        }
        int wall_length = (int)(Mathf.RoundToInt((float)(length_ * MLlength + MLlength + 16f + 2 * 1) / 13.05f) + 1);
        int wall_width = (int)(Mathf.RoundToInt(offset_w / 13.05f) + 1);
        offset_w = 0f;

        switch (this.Grid_rotation)
        {
            case -1:

                this.maxY = this.Grid_Y + wall_width;
                this.maxX = this.Grid_X + (wall_length - 1) / 2;
                this.minY = this.Grid_Y;
                this.minX = this.Grid_X - (wall_length - 1) / 2;

                break;
            case 1:
                this.maxY = this.Grid_Y;
                this.maxX = this.Grid_X + (wall_length) / 2;
                this.minY = this.Grid_Y - wall_width;
                this.minX = this.Grid_X - (wall_length) / 2;

                break;
            case -2:

                this.maxX = this.Grid_X + wall_width;
                this.maxY = this.Grid_Y + (wall_length) / 2;
                this.minX = this.Grid_X;
                this.minY = this.Grid_Y - (wall_length) / 2;

                break;
            case 2:
                this.maxX = this.Grid_X;
                this.maxY = this.Grid_Y + (wall_length) / 2;
                this.minX = this.Grid_X - wall_width;
                this.minY = this.Grid_Y - (wall_length) / 2;
                break;

        }

        for (int i = 0; i < borders.Count; i++)
        {
            if (borders[i].Check_point(this.maxX, this.maxY) || borders[i].Check_point(this.minX, this.maxY)
                || borders[i].Check_point(this.maxX, this.minY) || borders[i].Check_point(this.minX, this.minY))
            {
                return 1;
            }
        }

        for (int x_ = this.minX; x_ < this.maxX + 1; x_++)
        {
            for (int y_ = this.minY; y_ < this.maxY + 1; y_++)
            {
                GameObject ParentLine = GameObject.Find("Sort_Line");
                Transform childTransform = ParentLine.transform.Find($"Segment{x_}_{y_}");
                if (childTransform != null)
                {
                    return 1;
                }
            }
        }

        GameObject instantiatedObject = new GameObject($"Warehouse{this.Destination}{ID}");
        instantiatedObject.transform.position = new Vector3(this.LocationX, 0, this.LocationY);
        int shelf_counter = 0;
        int counter_offset_l = 0;

        offset_w = 0f;
        float offset_l = 0f;

        if (length_ % 2 == 0)
        {
            offset_l = -((length_ * MLlength + (MLlength + 16f)) / 2);
            //-(length_ * MLlength + (MLlength + 16f) / 2);
        }
        else
        {
            offset_l = -((length_ * MLlength + (MLlength + 16f) / 2 + MLlength) / 2);
        }
        int path_l = length_ / 2;
        for (int l_index = 0; l_index < length_; l_index++)
        {
            if (l_index == path_l)
            {
                // do ewentualnej zmiany
                offset_l += MLlength + 16f;
                counter_offset_l++;
            }
            else
            {
                offset_l += MLlength;
            }

            offset_w = 16f; //(width_ * MLwidth + ((width_ / 2) * MLwidth)); //16f
            for (int w_index = 0; w_index < width_; w_index++)
            {
                if (w_index % 2 == 0)
                {
                    new_rotation = 180f;
                    // do ewentualnej zmiany
                    offset_w += 20f;


                }
                else
                {
                    new_rotation = 0f;
                    offset_w += MLwidth;

                }

                for (int hight_number = 0; hight_number < 4; hight_number++)
                {
                    int shelf_index = (l_index * width_ * 4) + (w_index * 4) + hight_number;

                    GameObject InstanceShelf = Instantiate(InstanceML, new Vector3(this.LocationX + offset_l, 2f + hight_number * MLheigth, this.LocationY + offset_w), Quaternion.Euler(new Vector3(-90f, new_rotation, 0)));
                    InstanceShelf.name = $"Shelf{shelf_index}";
                    InstanceShelf.transform.SetParent(instantiatedObject.transform);
                    Shelf Shelf_component = InstanceShelf.GetComponent<Shelf>();
                    //Debug.Log($" {this.Grid_rotation}");

                    if (new_rotation == 0f) 
                    {
                        Shelf_component.Initialize(InstanceShelf, $"Shelf{shelf_index}", this.Grid_rotation, l_index >= path_l);
                    }
                    else
                    {
                        Shelf_component.Initialize(InstanceShelf, $"Shelf{shelf_index}", -this.Grid_rotation, l_index >= path_l);
                    }
                    this.Shelf_List.Add($"Shelf{shelf_index}",Shelf_component);

                    if (offset_w > max_offset_w) { max_offset_w = offset_w; }

                }
                shelf_counter++;
                if (shelf_counter >= shelves_number)
                {
                    break;
                }
            }

        }

        borders.Add(new WarehouseFiled(this.minX, this.minY, this.maxX, this.maxY));
        //Debug.Log($"{this.Destination}{ID}: Minima: X {this.minX}, Y {this.minY}, Maxima: X {this.maxX}, Y {this.maxY}, Rotation: {this.Grid_rotation}");

        float holeWidth = 6.8f;
        float holeHeight = 5.0f;

        //dluzsze sciany
        GameObject front_wall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // x, y, z, gdzie y to wysokosc
        front_wall1.transform.position = new Vector3(this.LocationX + (( wall_length * 13.05f ) - holeWidth) /4 + holeWidth / 2, 8f, this.LocationY + 6.75f);
        front_wall1.transform.localScale = new Vector3((wall_length * 13.05f) / 2 - holeWidth / 2, 15f, 1f);
        front_wall1.transform.SetParent(instantiatedObject.transform);

        GameObject front_wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);

        front_wall2.transform.position = new Vector3(this.LocationX - ((wall_length * 13.05f) - holeWidth) / 4 - holeWidth / 2, 8f, this.LocationY + 6.75f);
        front_wall2.transform.localScale = new Vector3((wall_length * 13.05f) / 2 - holeWidth / 2, 15f, 1f);
        front_wall2.transform.SetParent(instantiatedObject.transform);

        GameObject hole = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hole.transform.position = new Vector3(this.LocationX, 13f, this.LocationY + 6.75f);
        hole.transform.localScale = new Vector3(holeWidth, holeHeight, 1f);
        hole.transform.SetParent(instantiatedObject.transform);


        GameObject back_wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back_wall.transform.position = new Vector3(this.LocationX, 8f, this.LocationY + wall_width * 13.05f);
        back_wall.transform.localScale = new Vector3(wall_length * 13.05f, 15f, 1f);
        back_wall.transform.SetParent(instantiatedObject.transform);

        //krotsze
        GameObject left_wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        left_wall.transform.position = new Vector3(this.LocationX + wall_length * 13.05f / 2, 8f, this.LocationY + wall_width * 13.05f / 2 + MLlength / 4);
        left_wall.transform.localScale = new Vector3(1f, 15f, wall_width * 13.05f - MLlength / 2 + 1);
        left_wall.transform.SetParent(instantiatedObject.transform);

        GameObject right_wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        right_wall.transform.position = new Vector3(this.LocationX - wall_length * 13.05f / 2, 8f, this.LocationY + wall_width * 13.05f / 2 + MLlength / 4);
        right_wall.transform.localScale = new Vector3(1f, 15f, wall_width * 13.05f - MLlength / 2 + 1);
        right_wall.name = $"RightWall";
        right_wall.transform.SetParent(instantiatedObject.transform);


        GameObject SortLineObject = GameObject.Find("SortingLine");
        SortLine Sorting = SortLineObject.GetComponent<SortLine>();
        //Debug.Log("Sorting");

        Sorting.NewVertex(this.Grid_X, this.Grid_Y, this.Grid_rotation, this.maxX, this.minX, this.maxY, this.minY, borders);
        this.path = Sorting.FindPath(0, this.Grid_X, this.Grid_Y);
        //string str = $"{this.Destination}{ID} : [ ";
        //for (int i = 0; i < Path.Count; i++) 
        //{ 
        //    str += $" {Path[i]} ";
        //}
        //str += " ]";
        //Debug.Log(str);
        this.warehouse_id = ID;

        GameObject cube = Instantiate(robot_prefab, new Vector3(this.LocationX, 1.45f, this.LocationY + 13f), Quaternion.Euler(new Vector3(0, 0, 0)));
        cube.transform.SetParent(instantiatedObject.transform);
        Robot robot = cube.GetComponent<Robot>();
        robot.robot_prefab = cube;
        robot.warehouse_id = ID;
        robot.LocationX = this.Grid_X;
        robot.LocationY = this.Grid_Y;
        robot.warehouse_rotation = this.Grid_rotation;
        this.robot = robot;

        instantiatedObject.transform.rotation = Quaternion.Euler(new Vector3(0, this.rotation, 0));

        robot.set_start_pos(robot.transform.position);

        return 0;

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