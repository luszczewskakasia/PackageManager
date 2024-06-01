using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Robot : MonoBehaviour
{
    public GameObject robot_prefab;
    public int warehouse_id;
    public string warehouse_name;
    public bool trigger;
    public int LocationX;
    public int LocationY;
    public Vector3 start_pos;
    public int put_pack_on_shelf_step = 0;
    public int go_back_step = 0;
    public List<Vector3> put_pack_commands;
    public List<Vector3> go_back_commands;
    public float put_time;
    public bool animations;


    private void Start()
    {
    }

    private void Update()
    {
        if (trigger)
        {
            Vector3 final_vector = this.put_pack_commands[put_pack_on_shelf_step];
            Animator animator = robot_prefab.GetComponent<Animator>();
            if (put_pack_on_shelf_step == 0 || put_pack_on_shelf_step == 2 || put_pack_on_shelf_step == 4)
            {
                if (Math.Abs(robot_prefab.transform.rotation.eulerAngles.y - final_vector.y) > 3)
                { transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(final_vector), 1.0f * Time.deltaTime); }
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
                    robot_prefab.transform.position = Vector3.Lerp(robot_prefab.transform.position, final_vector, 1f * Time.deltaTime);
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

            else {

                if (put_pack_on_shelf_step == put_pack_commands.Count - 1)
                {
                    Debug.Log($"KoniecListy");

                    if (!animator.IsInTransition(0) && !animations)
                    {
                        Debug.Log($"W transition");
                        animations = true;
                        animator.SetInteger("Height", 3);
                        animator.SetBool("Ready_to_put", true);
                    }
                    else 
                    {
                        Debug.Log($"po wywo³aniu ");
                        animator.SetBool("Ready_to_put", false);
                    }
                }

            }



        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, start_pos, 1f * Time.deltaTime);
        }


    }
    public void set_move_steps(Vector3 shelf_pos,int shelf_rotation) 
    {
        put_pack_commands = new List<Vector3>();
        go_back_commands = new List<Vector3>();
        float X = shelf_pos.x;
        float Y = shelf_pos.z;
        float Height = shelf_pos.y;    
        switch (shelf_rotation) 
        {
            case -1:
                put_pack_commands.Add(new Vector3(0,180f,0 ));
                put_pack_commands.Add(new Vector3(this.start_pos.x, 0, Y - 5f));
                put_pack_commands.Add(new Vector3(0, 90f, 0));
                put_pack_commands.Add(new Vector3(X, 0, Y - 5f));
                put_pack_commands.Add(new Vector3(0, 90f, 0));
                put_pack_commands.Add(new Vector3(X, 0, Y - 1f));
                break;
            case 1:

                break;
            case -2:

                break;
             case 2:

                break;


        }
        
        




    
    }
    public bool Is_robot_trigger() 
    { 
        return trigger;
    }
    public void set_start_pos (Vector3 pos)
    {
        this.start_pos = pos;
    }





}
