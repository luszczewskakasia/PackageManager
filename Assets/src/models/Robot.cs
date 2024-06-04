using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Robot : MonoBehaviour
{
    public GameObject robot_prefab;
    public int warehouse_id;
    public string warehouse_name;
    public int warehouse_rotation;
    public bool trigger;
    public int LocationX;
    public int LocationY;
    public Vector3 start_pos;
    public int put_pack_on_shelf_step = 0;
    public int go_back_step = 0;
    public List<Vector3> put_pack_commands;
    public List<Vector3> go_back_commands;
    public bool animations;
    public Package transported_packege;
    public bool change_parent;

    private void Start()
    {
    }
    private void Update()
    {
        if (transported_packege != null) { trigger = true; }
        if (trigger && transported_packege != null )
        {
            Vector3 final_vector = this.put_pack_commands[put_pack_on_shelf_step];
            Animator animator = robot_prefab.GetComponent<Animator>();
            if (put_pack_on_shelf_step == 0 || put_pack_on_shelf_step == 2 || put_pack_on_shelf_step == 4)
            {
                float angle = robot_prefab.transform.rotation.eulerAngles.y;
                if (angle < 0)
                {
                    angle += 360;
                }
                float new_rot = final_vector.y;
                if (new_rot < 0)
                {
                    new_rot += 360;
                }
                if (warehouse_id == 0)
                {
                    Debug.Log($"{angle}    {new_rot}");
                }

                if (Math.Abs(angle - new_rot) > 3 && Math.Abs(angle - new_rot) < 357)
                { transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(final_vector), 1.5f * Time.deltaTime); }
                else
                {
                    transform.rotation = Quaternion.Euler(final_vector);
                    if (put_pack_on_shelf_step < put_pack_commands.Count - 1)
                    {
                        put_pack_on_shelf_step += 1;
                    }
                }
            }
            if (put_pack_on_shelf_step == 1 || put_pack_on_shelf_step == 3 || put_pack_on_shelf_step == 5)
            {
                if ((Math.Abs(final_vector.x - robot_prefab.transform.position.x) > 0.4f) || (Math.Abs(final_vector.z - robot_prefab.transform.position.z) > 0.4f))
                {
                    robot_prefab.transform.position = Vector3.MoveTowards(robot_prefab.transform.position, final_vector, 5f * Time.deltaTime);
                }
                else
                {
                    robot_prefab.transform.position = final_vector;
                    if (put_pack_on_shelf_step < put_pack_commands.Count - 1)
                    {
                        put_pack_on_shelf_step += 1;

                    }
                }
            }
            else
            {

                if (put_pack_on_shelf_step == put_pack_commands.Count - 1)
                {
                    if (!animator.IsInTransition(0) && !animations)
                    {
                        animations = true;
                        switch (transported_packege.Place_on_Shelf.y)
                        {
                            case 1f:
                                animator.SetInteger("Height", 3);
                                break;
                            case 8f:
                                animator.SetInteger("Height", 2);
                                break;
                            case 5f:
                                animator.SetInteger("Height", 1);
                                break;
                            case 2f:
                                animator.SetInteger("Height", 0);
                                break;

                        }
                        animator.SetBool("Ready_to_put", true);
                    }
                    else
                    {
                        animator.SetBool("Ready_to_put", false);
                    }
                }
            }
        }
        if (animations)
        {
            GameObject pack = transported_packege.Package_Mesh;
            Vector3 targetPosition;
            switch (transported_packege.size)
            {
                case 0:
                    targetPosition = new Vector3(pack.transform.position.x, transported_packege.Place_on_Shelf.y - 0.65f, pack.transform.position.z);
                    pack.transform.position = Vector3.MoveTowards(pack.transform.position, targetPosition, 1.5f * Time.deltaTime);
                    if(Math.Abs(pack.transform.position.y - (transported_packege.Place_on_Shelf.y - 0.65f)) < 0.2f) { change_parent = true;}
                    break;
                case 1:
                    targetPosition = new Vector3(pack.transform.position.x, transported_packege.Place_on_Shelf.y - 0.25f, pack.transform.position.z);
                    pack.transform.position = Vector3.MoveTowards(pack.transform.position, targetPosition, 1.5f * Time.deltaTime);
                    if (Math.Abs(pack.transform.position.y - (transported_packege.Place_on_Shelf.y - 0.65f)) < 0.4f) { change_parent = true; }
                    break;
                case 2:
                    targetPosition = new Vector3(pack.transform.position.x, transported_packege.Place_on_Shelf.y - 0.25f, pack.transform.position.z);
                    pack.transform.position = Vector3.MoveTowards(pack.transform.position, targetPosition, 1.5f * Time.deltaTime);
                    if (Math.Abs(pack.transform.position.y - (transported_packege.Place_on_Shelf.y - 0.65f)) < 0.4f) { change_parent = true; }
                    break;
            }

        }
        if (change_parent) 
        {
            GameObject pack = transported_packege.Package_Mesh;
            pack.transform.position = Vector3.MoveTowards(pack.transform.position, transported_packege.Place_on_Shelf, 3f * Time.deltaTime);
            float pack_tolerance = 0.1f;
            switch (transported_packege.size) { case 0: pack_tolerance = 0.1f; break; case 1: pack_tolerance = 0.2f; break; case 2: pack_tolerance = 0.4f; break; }

            if ((Math.Abs(pack.transform.position.x - transported_packege.Place_on_Shelf.x) < pack_tolerance) &&
                    (Math.Abs(pack.transform.position.y - transported_packege.Place_on_Shelf.y ) < pack_tolerance) &&
                    (Math.Abs(pack.transform.position.y - transported_packege.Place_on_Shelf.y ) < pack_tolerance)) 
            {
                GameObject simGO = GameObject.Find("Simulation");
                Simulation sim = simGO.GetComponent<Simulation>();
                warehouse Wh = sim.Warehouses[transported_packege.warehouseID];
                Shelf shelf = Wh.Shelf_List[transported_packege.shelf_name];
                pack.transform.SetParent(shelf.transform);
                animations = false;
                change_parent = false;
                transported_packege = null;

            }
        }
        if (trigger && transported_packege == null)
        {
            if (go_back_step == go_back_commands.Count)
            {
                go_back_step = 0;
                put_pack_on_shelf_step = 0;
                put_pack_commands = new List<Vector3>();
                go_back_commands = new List<Vector3>();
                trigger = false;
            }
            else 
            {
                Vector3 final_vector = this.go_back_commands[go_back_step];
                if (go_back_step == 0 || go_back_step == 2 || go_back_step == 4)
                {
                    float angle = robot_prefab.transform.rotation.eulerAngles.y;
                    if (angle < 0)
                    {
                        angle += 360;
                    }
                    float new_rot = final_vector.y;
                    if (new_rot < 0)
                    {
                        new_rot += 360;
                    }
                    if (warehouse_id == 0)
                    {
                        Debug.Log($"{angle}    {new_rot}");
                    }

                    if (Math.Abs(angle - new_rot) > 3 && Math.Abs(angle - new_rot) < 357)
                    { transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(final_vector), 1.5f * Time.deltaTime); }
                    else
                    {
                        transform.rotation = Quaternion.Euler(final_vector);
                        if (go_back_step < go_back_commands.Count)
                        {
                            go_back_step += 1;
                        }
                    }
                }
                if (go_back_step == 1 || go_back_step == 3 || go_back_step == 5)
                {
                    if ((Math.Abs(final_vector.x - robot_prefab.transform.position.x) > 0.4f) || (Math.Abs(final_vector.z - robot_prefab.transform.position.z) > 0.4f))
                    {
                        robot_prefab.transform.position = Vector3.MoveTowards(robot_prefab.transform.position, final_vector, 5f * Time.deltaTime);
                    }
                    else
                    {
                        robot_prefab.transform.position = final_vector;
                        if (go_back_step < go_back_commands.Count)
                        {
                            go_back_step += 1;

                        }
                    }

                }
            }
        }
    }
    public void set_move_steps(Vector3 shelf_pos, int shelf_rotation,bool Mag_side)
    {
        put_pack_commands = new List<Vector3>();
        go_back_commands = new List<Vector3>();
        float X = shelf_pos.x;
        float Y = shelf_pos.z;
        float Height = shelf_pos.y;
        float offset_ = 12f;
        if (shelf_rotation == -2 || shelf_rotation == -1) { offset_ = -12f; }
        switch (this.warehouse_rotation)
        {
            case -1: //od dolu
                put_pack_commands.Add(new Vector3(0, 180f, 0));
                put_pack_commands.Add(new Vector3(this.start_pos.x, 1.45f, Y - offset_));
                if (Mag_side)
                {
                    put_pack_commands.Add(new Vector3(0, -90f, 0));
                }
                else
                {
                    put_pack_commands.Add(new Vector3(0, 90f, 0));
                }
                put_pack_commands.Add(new Vector3(X, 1.45f, Y - offset_));
                if (shelf_rotation == 1)
                {
                    put_pack_commands.Add(new Vector3(0, 180f, 0));
                }
                else
                {
                    put_pack_commands.Add(new Vector3(0, 0f, 0));

                }
                // powrót
                if (Mag_side)
                {
                    go_back_commands.Add(new Vector3(0, 90f, 0));
                }
                else
                {
                    go_back_commands.Add(new Vector3(0, -90f, 0));
                }
                go_back_commands.Add(new Vector3(this.start_pos.x , 1.45f, Y - offset_));
                go_back_commands.Add(new Vector3(0, 0f, 0));
                go_back_commands.Add(new Vector3(this.start_pos.x, 1.45f, this.start_pos.z));



                break;
            case 1:
                put_pack_commands.Add(new Vector3(0, 0f, 0));
                put_pack_commands.Add(new Vector3(this.start_pos.x, 1.45f, Y - offset_));
                if (Mag_side)
                {
                    put_pack_commands.Add(new Vector3(0, 90f, 0));
                }
                else
                {
                    put_pack_commands.Add(new Vector3(0, -90f, 0));
                }
                put_pack_commands.Add(new Vector3(X, 1.45f, Y - offset_));
                if (shelf_rotation == 1)
                {
                    put_pack_commands.Add(new Vector3(0, 180f, 0));
                }
                else
                {
                    put_pack_commands.Add(new Vector3(0, 0f, 0));

                }
                // powrót
                if (Mag_side)
                {
                    go_back_commands.Add(new Vector3(0, -90f, 0));
                }
                else
                {
                    go_back_commands.Add(new Vector3(0, 90f, 0));
                }
                go_back_commands.Add(new Vector3(this.start_pos.x, 1.45f, Y - offset_));
                go_back_commands.Add(new Vector3(0, 180f, 0));
                go_back_commands.Add(new Vector3(this.start_pos.x, 1.45f, this.start_pos.z));


                break;
            case -2: //od lewej
                put_pack_commands.Add(new Vector3(0, -90f, 0));
                put_pack_commands.Add(new Vector3(X - offset_, 1.45f, this.start_pos.z));
                if (Mag_side)
                { 
                    put_pack_commands.Add(new Vector3(0, 0f, 0));
                }
                else {
                    put_pack_commands.Add(new Vector3(0, 180f, 0)); 
                }
                put_pack_commands.Add(new Vector3(X - offset_, 1.45f, Y));
                if (shelf_rotation == 2)
                {
                    put_pack_commands.Add(new Vector3(0, -90f, 0));
                }
                else 
                {
                    put_pack_commands.Add(new Vector3(0, 90f, 0));

                }
                // powrót
                if (Mag_side)
                {
                    go_back_commands.Add(new Vector3(0, 180f, 0));
                }
                else
                {
                    go_back_commands.Add(new Vector3(0, 0f, 0));
                }
                go_back_commands.Add(new Vector3(X - offset_, 1.45f, this.start_pos.z));
                go_back_commands.Add(new Vector3(0, 90f, 0));
                go_back_commands.Add(new Vector3(this.start_pos.x, 1.45f, this.start_pos.z));

                break;
            case 2:
                put_pack_commands.Add(new Vector3(0, 90f, 0));
                put_pack_commands.Add(new Vector3(X - offset_, 1.45f, this.start_pos.z));
                if (Mag_side) 
                { 
                    put_pack_commands.Add(new Vector3(0, 180f, 0)); 
                }
                else 
                { 
                    put_pack_commands.Add(new Vector3(0, 0f, 0)); 
                }
                put_pack_commands.Add(new Vector3(X - offset_, 1.45f, Y));
                if (shelf_rotation == 2)
                {
                    put_pack_commands.Add(new Vector3(0, -90f, 0));
                }
                else
                {
                    put_pack_commands.Add(new Vector3(0, 90f, 0));

                }
                // powrót
                if (Mag_side)
                {
                    go_back_commands.Add(new Vector3(0, 0f, 0));
                }
                else
                {
                    go_back_commands.Add(new Vector3(0, 180f, 0));
                }
                go_back_commands.Add(new Vector3(X - offset_, 1.45f, this.start_pos.z));
                go_back_commands.Add(new Vector3(0, -90f, 0));
                go_back_commands.Add(new Vector3(this.start_pos.x, 1.45f, this.start_pos.z));




                break;

        }
    }
    public void Take_pack(Package pack_)
    {
        GameObject pack = pack_.Package_Mesh;
        this.transported_packege = pack_;
        Vector3 robot_middle = this.robot_prefab.transform.position;
        Vector3 Range_to_platform;
        switch (this.warehouse_rotation)
        {
            case -2:
                switch (pack_.size)
                {
                    case 0:
                        Range_to_platform = new Vector3(-4, 2.2f, 0f);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 1:
                        Range_to_platform = new Vector3(-4, 2.5f, 0);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 2:
                        Range_to_platform = new Vector3(-4, 2.7f, 0);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                }
                break;
            case 2:
                switch (pack_.size)
                {
                    case 0:
                        Range_to_platform = new Vector3(4, 2.2f, 0f);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 1:
                        Range_to_platform = new Vector3(4, 2.5f, 0);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 2:
                        Range_to_platform = new Vector3(4, 2.7f, 0);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                }
                break;
            case -1:
                switch (pack_.size)
                {
                    case 0:
                        Range_to_platform = new Vector3(0, 2.2f, -4);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 1:
                        Range_to_platform = new Vector3(0, 2.5f, -4);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 2:
                        Range_to_platform = new Vector3(0, 2.7f, -4);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                }
                break;
            case 1:
                switch (pack_.size)
                {
                    case 0:
                        Range_to_platform = new Vector3(0, 2.2f, 4);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 1:
                        Range_to_platform = new Vector3(0, 2.5f, 4);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                    case 2:
                        Range_to_platform = new Vector3(0, 2.7f, 4);
                        pack.transform.position = robot_middle + Range_to_platform;
                        break;
                }
                break;
        }
        pack.transform.SetParent(robot_prefab.transform);

        GameObject simGO = GameObject.Find("Simulation");
        Simulation sim = simGO.GetComponent<Simulation>();
        warehouse Wh = sim.Warehouses[transported_packege.warehouseID];
        Wh.Packege_in_delivery = false;
    }
    public bool Is_robot_trigger()
    {
        return trigger;
    }
    public void set_start_pos(Vector3 pos)
    {
        this.start_pos = pos;
    }
}
