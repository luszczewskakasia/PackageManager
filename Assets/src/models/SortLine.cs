using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

[System.Serializable]


public struct Edge
{
    public int dir;  
    public int target;   

    public Edge(int dir_, int val)
    {
        dir = dir_;
        target = val;
    }
}

public struct Node
{
    public int x; 
    public int y; 
    public int Input_dir; 

    // Konstruktor
    public Node(int x_, int y_, int val)
    {
        x = x_;
        y = y_;

        Input_dir = val;
    }
}


public class SortLine:MonoBehaviour
{ 
    public List<Node> Node_vertices;
    public Dictionary<int, List<Edge>> Node_Connections;
    public float normilize_field_size;
    public GameObject Straight_Mesh;
    public GameObject CrossRight_Mesh;
    public GameObject CrossLeft_Mesh;
    public GameObject Cross_Mesh;
    public GameObject Turn_Mesh;

    private GameObject SortLine_Mesh;


    public void Start() 
    {
        Node_vertices = new List<Node>();
        Node_Connections = new Dictionary<int, List<Edge>>();

        for (int index = 0; index < 3; index++)
        {
            Add_new_point(new Node(0, index*3,0));
        }
        Add_new_point(new Node(3, 0, 0));
        Add_new_edge(1, 2);
        Add_new_edge(2, 3);
        Add_new_edge(1, 4);

    }

    public void Update() 
    {
    }



    public void Add_new_point(Node VertexData) 
    {

        Node_vertices.Add(VertexData);
        Node_Connections[Node_vertices.Count]= new List<Edge>();
        Debug.Log($"Nowy punkt {Node_vertices[Node_vertices.Count - 1].x}, {Node_vertices[Node_vertices.Count - 1].y}");

    }

    public void Add_new_edge(int StartIndex, int StopIndex)
    {
        int diff=0;
        float rotation =0;
        int dir = 0;


        try
        {
            if (Node_vertices[StartIndex-1].x == Node_vertices[StopIndex-1].x)
            {
                Debug.Log($"KrawêdŸ sta³y x");
                diff = Node_vertices[StopIndex - 1].y - Node_vertices[StartIndex - 1].y;
                dir = (diff > 0) ? 1 : 3;
                rotation = 90f;
                Debug.Log($"diff {diff} rotacja {rotation}");

            }
            else if (Node_vertices[StopIndex - 1].y == Node_vertices[StartIndex - 1].y)
            {
                Debug.Log($"KrawêdŸ sta³y y");
                diff = Node_vertices[StopIndex - 1].x - Node_vertices[StartIndex - 1].x;
                dir = (diff > 0) ? 0 : 2;
                rotation = 0f;
            }
            Node_Connections[StartIndex].Add(new Edge(dir,StopIndex));
            Debug.Log($"i leci instancjonowanie");

        }
        catch (Exception e) { return;}

        this.SortLine_Mesh = new GameObject("Sort_Line_");
        for (int i = 0; i < Math.Abs(diff); i++)
        {
            Debug.Log($"i leci instancjonowanie");
            Vector3 Location = new Vector3();
            if (dir == 0) 
            {
                Location = new Vector3((Node_vertices[StartIndex - 1].x + i) * normilize_field_size, 1, Node_vertices[StartIndex - 1].y * normilize_field_size);

            }
            if (dir == 2)
            {
                Location = new Vector3((Node_vertices[StartIndex - 1].x - i) * normilize_field_size, 1, Node_vertices[StartIndex - 1].y * normilize_field_size);

            }
            if (dir == 1)
            {
                Location = new Vector3(Node_vertices[StartIndex - 1].x * normilize_field_size, 1, (Node_vertices[StartIndex - 1].y + i) * normilize_field_size);
            }

            if (dir == 3)
            {
                Location = new Vector3(Node_vertices[StartIndex - 1].x * normilize_field_size, 1, (Node_vertices[StartIndex - 1].y - i) * normilize_field_size);

            }
            GameObject LinePart = Instantiate(Straight_Mesh, Location , Quaternion.Euler(new Vector3(0, rotation, 90)));
            LinePart.name = $"Edge{StartIndex}-{StopIndex}_part{i + 1}";
            LinePart.transform.SetParent(this.SortLine_Mesh.transform); 
        }
        if (Node_Connections[StartIndex].Count == 2)
        {
            int Edge_1dir = Node_Connections[StartIndex][0].dir;
            int Edge_2dir = Node_Connections[StartIndex][1].dir;

            Delete_old_segment($"Edge{StartIndex}-{Node_Connections[StartIndex][0].target}_part1");
            Delete_old_segment($"Edge{StartIndex}-{Node_Connections[StartIndex][1].target}_part1");

            if (Edge_1dir == 2) { rotation = 0f; }
            if (Edge_1dir == 1 ) { rotation = 90f; }
            if (Edge_2dir == 0 ) { rotation = 180f; }
            if (Edge_2dir == 3) { rotation = 270f; }


            GameObject LinePart = Instantiate(CrossLeft_Mesh, new Vector3((Node_vertices[StartIndex - 1].x) * normilize_field_size, 1, Node_vertices[StartIndex - 1].y * normilize_field_size), Quaternion.Euler(new Vector3(0, rotation, -90)));
            LinePart.name = $"Edge{StartIndex}-{StopIndex}_cross";
            LinePart.transform.SetParent(this.SortLine_Mesh.transform);
        }


    }




    public void Delete_old_segment(string objectNameToDelete)
    {
        GameObject objectToDelete = GameObject.Find(objectNameToDelete);
        if (objectToDelete != null)
        {
            Destroy(objectToDelete);
        }
    }





    public string Serialize()
    {
        string json = "{\"Node_vertices\":[";
        //for (int i = 0; i < Node_vertices.Count; i++)
        //{
        //    json += "[";
        //    for (int j = 0; j < Node_vertices[i].Count; j++)
        //    {
        //        json += Node_vertices[i][j].ToString();
        //        if (j < Node_vertices[i].Count - 1)
        //            json += ",";
        //    }
        //    json += "]";
        //    if (i < Node_vertices.Count - 1)
        //        json += ",";
        //}
        //json += "],\"Node_Connections\":{";
        //int index = 0;
        //foreach (var kvp in Node_Connections)
        //{
        //    json += "\"" + kvp.Key.ToString() + "\":[";
        //    for (int i = 0; i < kvp.Value.Count; i++)
        //    {
        //        json += kvp.Value[i].ToString();
        //        if (i < kvp.Value.Count - 1)
        //            json += ",";
        //    }
        //    json += "]";
        //    if (index < Node_Connections.Count - 1)
        //        json += ",";
        //    index++;
        //}
        //json += "}}";

        return json;
    }

}
