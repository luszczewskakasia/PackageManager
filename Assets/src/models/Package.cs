using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Package:MonoBehaviour
{
    public int size;
    private string ID;
    private int warehouseID;
    private List<Node> road;
    private GameObject Package_Mesh;
    public bool Destination_achived;
    public int current_location;
    public Rigidbody RiggedBody;
    public Package(int size, string ID,int warehouseID, List<Node> road, GameObject Mesh)
    {
        this.size = size;
        this.ID = null;
        this.warehouseID = warehouseID;
        this.road = road;
        this.Package_Mesh = Mesh;
        this.Destination_achived = false;
        this.current_location = 0;
        this.RiggedBody = Package_Mesh.GetComponent<Rigidbody>();

    }

    private void Update()
    {
        Vector3 dir = Vector3.zero;
        float speed = 2.0f;
        Vector3 Pack_position = Package_Mesh.transform.position;
        switch (road[current_location + 1].Input_dir)
        {
            case 1:
                dir = new Vector3(0, 0, 1);
                break;
            case -1:
                dir = new Vector3(0, 0, -1);
                break;
            case 2:
                dir = new Vector3(1, 0, 0);
                break;
            case -2:
                dir = new Vector3(-1, 0, 0);
                break;
        }

        if ((float)Mathf.Abs((float)(Pack_position.x / 13.05) - road[road.Count - 1].x) < 13.05 / 2 &&
            (float)Mathf.Abs((float)(Pack_position.z / 13.05) - road[road.Count - 1].y) < 13.05 / 2)
        {
            Destination_achived = true;
        }

        if ((float)Mathf.Abs((float)(Pack_position.x / 13.05) - road[current_location + 1].x) < 13.05 / 2 &&
        (float)Mathf.Abs((float)(Pack_position.z / 13.05) - road[current_location + 1].y) < 13.05 / 2)
        {
            current_location += 1;
        }

        RiggedBody.AddForce(dir * speed);

    }




    public Package get()
    {
        return this;
    }

}

