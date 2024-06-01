using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]


public struct queue_struct {
    public int size;
    public string ID;
    public int warehouseID;
    public List<Node> road;

    public queue_struct(int size, string ID, int warehouseID, List<Node> road)
    {
        this.size = size;
        this.ID = ID;
        this.warehouseID = warehouseID;
        this.road = road;
    }

    public void set_ID(){}

};


public class Package:MonoBehaviour
{
    public int size;
    public string ID;
    public int warehouseID;
    public List<Node> road;
    public GameObject Package_Mesh;
    public bool Destination_achived;
    public int current_location;
    public Rigidbody RiggedBody;
    public void Initialize(queue_struct init_data)
    {
        this.size = init_data.size;
        this.ID = init_data.ID;
        this.warehouseID = init_data.warehouseID;
        this.road = init_data.road;
        this.Destination_achived = false;
        this.current_location = 0;
        //Debug.Log($" D³ugoœæ drogi {road.Count}");
        foreach (Node nd in road) { Debug.Log($" {nd.Input_dir} ,{nd.Index}"); }

    }

    private void Start()
    {}

    private void Update()
    {
        Vector3 dir = Vector3.zero;
        float speed = 25f;
        Vector3 Pack_position = Package_Mesh.transform.position;
        int path_length = road.Count;
        if (!Destination_achived)
        {
            int direction = road[current_location + 1].Input_dir;
            float x_to_mid = (float)(road[current_location + 1].x * 13.05) - Pack_position.x;
            float y_to_mid = (float)(road[current_location + 1].y * 13.05) - Pack_position.z;
            float x_to_last = (float)(road[road.Count - 1].x * 13.05) - Pack_position.x;
            float y_to_last = (float)(road[road.Count - 1].y * 13.05) - Pack_position.z;

            if ((int)(Pack_position.x / 13.05) == road[road.Count - 1].x && (int)((Pack_position.z - 13.05 / 2) / 13.05) == road[road.Count - 1].y || (Mathf.Abs(x_to_last) < 1.05 && Mathf.Abs(y_to_last) < 1.05))
            {
                Destination_achived = true;
            }
            switch (direction)
            {
                case 1:
                    dir = new Vector3(x_to_mid, 0, 1);
                    break;
                case -1:
                    dir = new Vector3(x_to_mid, 0, -1);
                    break;
                case 2:
                    dir = new Vector3(1, 0, y_to_mid);
                    break;
                case -2:
                    dir = new Vector3(-1, 0, y_to_mid);
                    break;
            }
            if ((int)(Pack_position.x / 13.05) == road[current_location + 1].x && (int)((Pack_position.z - 13.05 / 2) / 13.05) == road[current_location + 1].y || (Mathf.Abs(x_to_mid) < 1.05 && Mathf.Abs(y_to_mid) < 1.05))
            {
                current_location += 1;
            }


            RiggedBody.velocity = dir * speed;
        }
        else 
        {
            int direction = road[road.Count - 1].Input_dir;
            switch (direction)
            {
                case 1:
                    dir = new Vector3(0, 0, 13.05f / 2);
                    break;
                case -1:
                    dir = new Vector3(0, 0, -13.05f / 2);
                    break;
                case 2:
                    dir = new Vector3(13.05f / 2, 0, 0);
                    break;
                case -2:
                    dir = new Vector3(-13.05f / 2, 0, 0);
                    break;
            }
            float x_to_last = (float)(road[road.Count - 1].x * 13.05) - Pack_position.x + dir.x;
            float y_to_last = (float)(road[road.Count - 1].y * 13.05) - Pack_position.z + dir.z;
            if (Math.Abs(x_to_last) > 0.5 || Math.Abs(y_to_last) > 0.5)
            {
                Vector3 finalPosition = new Vector3((float)(road[road.Count - 1].x * 13.05), Pack_position.y, (float)(road[road.Count - 1].y * 13.05)) + dir;
                float distance = Vector3.Distance(transform.position, finalPosition);
                float interpolationSpeed = speed / distance; 
                transform.position = Vector3.Lerp(transform.position, finalPosition, interpolationSpeed * Time.deltaTime);

            }
            else
            {
                RiggedBody.velocity = Vector3.zero;
                RiggedBody.angularVelocity = Vector3.zero;
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sort_Line")
        {
            Debug.Log("Enter");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Sort_Line")
        {
            Debug.Log("Stay");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Sort_Line")
        {
            Debug.Log("Exit");
        }
    }


    public Package get()
    {
        return this;
    }

    public void Add_Mesh_to_Package(GameObject Mesh)
    {
        this.Package_Mesh = Mesh;
        this.RiggedBody = Mesh.GetComponent<Rigidbody>();
        RiggedBody.isKinematic = false;
        RiggedBody.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

    }
}

