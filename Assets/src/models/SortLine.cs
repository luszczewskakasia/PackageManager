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
    public GameObject T_Cross_Mesh;
    public GameObject Cross_Mesh;
    public GameObject Turn_Mesh;

    public int punkt_1;
    public int punkt_2;
    public int punkt_3;
    public int punkt_4;



    private GameObject SortLine_Mesh;


    public void Start() 
    {
        Node_vertices = new List<Node>();
        Node_Connections = new Dictionary<int, List<Edge>>();
        this.SortLine_Mesh = new GameObject("Sort_Line");

        Add_new_point(new Node(7, 0, 0));
        Add_new_point(new Node(0, 0, 0));
        Add_new_point(new Node(0, 6, 0));
        Add_new_point(new Node(0, -5, 0));
        Add_new_point(new Node(5, -5, 0));
        Add_new_point(new Node(0, -7, 0));
        Add_new_point(new Node(-2, -7, 0));
        Add_new_point(new Node(0, -9, 0));
        Add_new_point(new Node(2, 6, 0));
        Add_new_point(new Node(-3, 6, 0));
        Add_new_point(new Node(-3, 1, 0));
        Add_new_point(new Node(-6, 1, 0));
        Add_new_point(new Node(-3, 0, 0));

        Add_new_edge(1, 2);
        Add_new_edge(2, 3); 
        Add_new_edge(2,4);
        Add_new_edge(2, 13);
        Add_new_edge(3, 9);
        Add_new_edge(3, 10);
        Add_new_edge(4, 5);
        Add_new_edge(4, 6);
        Add_new_edge(6, 7);
        Add_new_edge(6, 8);
        Add_new_edge(13, 11);
        Add_new_edge(11, 12);


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

        Node StartNode = Node_vertices[StartIndex - 1];
        Node StopNode = Node_vertices[StopIndex - 1];

        try
        {
            if (StartNode.x == StopNode.x)
            {
                diff =StopNode.y - StartNode.y;
                dir = (diff > 0) ? 1 : -1;
                rotation = 90f;
            }
            else if (StartNode.y == StopNode.y)
            {
                diff = StopNode.x - StartNode.x;
                dir = (diff > 0) ? 2 : -2;
                rotation = 0f;
            }
            Node_Connections[StartIndex].Add(new Edge(dir,StopIndex));
            StopNode.Input_dir = dir;
            Node_vertices[StopIndex - 1] = StopNode; 
        }
        catch (Exception e) { return;}
        Vector3 Location = new Vector3();
        GameObject Egde_ = this.SortLine_Mesh;
        GameObject LinePart = null;
        for (int i = 0; i < Math.Abs(diff)+1; i++)
        {
            int current_x=0;
            int current_y=0;
            switch (dir)
            {
                case 2: current_x = StartNode.x + i; current_y = StartNode.y; break;
                case 1: current_x = StartNode.x; current_y = StartNode.y + i; break;
                case -2: current_x = StartNode.x - i; current_y = StartNode.y; break;
                case -1: current_x = StartNode.x; current_y = StartNode.y - i; break;
            }

            Debug.Log($"X: {current_x}, Y:{current_y}");

            if (i != 0)
            {
                Location = new Vector3(current_x * normilize_field_size, 1, current_y * normilize_field_size);
                LinePart = Instantiate(Straight_Mesh, Location, Quaternion.Euler(new Vector3(0, rotation, 90)));
                LinePart.name = $"Segment{current_x}_{current_y}";
                LinePart.transform.SetParent(Egde_.transform);
            }
        }
        Location = new Vector3(StartNode.x * normilize_field_size, 1, StartNode.y * normilize_field_size);
        Debug.Log($"Iloœæ krawêdzi dla punktu {StartIndex}: {Node_Connections[StartIndex].Count}");

        switch (Node_Connections[StartIndex].Count)
        {
            case 1://prosta lub zakrêt
                int EdgeDir = Node_Connections[StartIndex][0].dir;
                if (StartNode.Input_dir == EdgeDir || StartNode.Input_dir == 0)
                {
                    rotation = (EdgeDir % 2 == 0) ? 0f : 90f;
                    Delete_old_segment($"Segment{StartNode.x}_{StartNode.y}");
                    LinePart = Instantiate(Straight_Mesh, Location, Quaternion.Euler(new Vector3(0, rotation, 90)));
                    LinePart.name = $"Segment{StartNode.x}_{StartNode.y}";
                    LinePart.transform.SetParent(Egde_.transform);
                }
                else
                {
                    if ((EdgeDir == -2 && StartNode.Input_dir == 1) || (EdgeDir == -1 && StartNode.Input_dir == 2)) { rotation = 90f; } //przypadek 3
                    if ((EdgeDir == -2 && StartNode.Input_dir == -1) || (EdgeDir == 1 && StartNode.Input_dir == 2)) { rotation = 180f; }
                    if ((EdgeDir == 2 && StartNode.Input_dir == -1) || (EdgeDir == 1 && StartNode.Input_dir == -2)) { rotation = 270f; } //przypadek 1 
                    if ((EdgeDir == 2 && StartNode.Input_dir == 1) || (EdgeDir == -1 && StartNode.Input_dir == -2)) { rotation = 0f; }
                    Delete_old_segment($"Segment{StartNode.x}_{StartNode.y}");
                    LinePart = Instantiate(Turn_Mesh, Location, Quaternion.Euler(new Vector3(0, rotation, 90)));
                    LinePart.name = $"Segment{StartNode.x}_{StartNode.y}";
                    LinePart.transform.SetParent(Egde_.transform);
                }

                break;
            case 2://zakrêt i prosta lub 2 zakrêty jednoczeœnie
                int Edge1Dir = Node_Connections[StartIndex][0].dir;
                int Edge2Dir = Node_Connections[StartIndex][1].dir;
                int MainDir = 0;
                int SecondDir = 0;
                bool LeftSide = true;
                float rotationX = 90f;
                GameObject CorrectCross = null;

                if (StartNode.Input_dir == Edge1Dir || StartNode.Input_dir == Edge2Dir)
                {
                    if (StartNode.Input_dir == Edge1Dir) { MainDir = Edge1Dir; SecondDir = Edge2Dir; } else { MainDir = Edge2Dir; SecondDir = Edge1Dir; }
                    if ((MainDir == -2 && SecondDir == 1) || (MainDir == -1 && SecondDir == -2) || (MainDir == 1 && SecondDir == 2) || (MainDir == 2 && SecondDir == -1)) { LeftSide = false; }
                    if ((MainDir == -2 && SecondDir == -1) || (MainDir == -1 && SecondDir == 2) || (MainDir == 1 && SecondDir == -2) || (MainDir == 2 && SecondDir == 1)) { LeftSide = true; }

                    CorrectCross = (LeftSide) ? CrossLeft_Mesh : CrossRight_Mesh;
                    rotationX = (LeftSide) ? 90f : 270f;
                    Debug.Log($"Skrêt {LeftSide} ");
                }
                if (Edge1Dir == -Edge2Dir)
                {
                    CorrectCross = T_Cross_Mesh;
                }
                switch (StartNode.Input_dir)
                {
                    case -1: rotation = 0f; break;
                    case -2: rotation = 90f; break;
                    case 1: rotation = 180f; break;
                    case 2: rotation = 270f; break;
                }
                Delete_old_segment($"Segment{StartNode.x}_{StartNode.y}");
                LinePart = Instantiate(CorrectCross, Location, Quaternion.Euler(new Vector3(rotationX, rotation, 0)));
                LinePart.name = $"Segment{StartNode.x}_{StartNode.y}";
                LinePart.transform.SetParent(Egde_.transform);



                break;
            case 3://prosta i 2 zakrêty

                switch (StartNode.Input_dir)
                {
                    case -1: rotation = 0f; break;
                    case -2: rotation = 90f; break;
                    case 1: rotation = 180f; break;
                    case 2: rotation = 270f; break;
                }
                Delete_old_segment($"Segment{StartNode.x}_{StartNode.y}");
                LinePart = Instantiate(Cross_Mesh, Location, Quaternion.Euler(new Vector3(0, rotation, -90)));
                LinePart.name = $"Segment{StartNode.x}_{StartNode.y}";
                LinePart.transform.SetParent(Egde_.transform);

                break;
        
        }


    //    if (Node_Connections[StartIndex].Count == 2)
    //    {
    //        int Edge_1dir = Node_Connections[StartIndex][0].dir;
    //        int Edge_2dir = Node_Connections[StartIndex][1].dir;


    //        if (Edge_1dir == 2) { rotation = 0f; }
    //        if (Edge_1dir == 1 ) { rotation = 90f; }
    //        if (Edge_2dir == 0 ) { rotation = 180f; }
    //        if (Edge_2dir == 3) { rotation = 270f; }


    //        GameObject LinePart = Instantiate(CrossLeft_Mesh, new Vector3((Node_vertices[StartIndex - 1].x) * normilize_field_size, 1, Node_vertices[StartIndex - 1].y * normilize_field_size), Quaternion.Euler(new Vector3(0, rotation, -90)));
    //        LinePart.name = $"Edge{StartIndex}-{StopIndex}_cross";
    //        LinePart.transform.SetParent(this.SortLine_Mesh.transform);
    //    }


    }




    public void Delete_old_segment(string objectNameToDelete)
    {

        GameObject ParentLine = GameObject.Find("Sort_Line");

        Transform childTransform = ParentLine.transform.Find(objectNameToDelete);

        Debug.Log($"{childTransform}");

        if (childTransform != null)
        {
            GameObject childObject = childTransform.gameObject;
            DestroyImmediate(childObject);
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
