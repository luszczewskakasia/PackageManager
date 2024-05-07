using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

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
    public bool final;
    public int from_;

    // Konstruktor
    public Node(int x_, int y_, int val)
    {
        x = x_;
        y = y_;

        Input_dir = val;
        final = false;
        from_ = -1;
    }


}




public class SortLine : MonoBehaviour
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


    public void Awake()
    {
        this.Node_vertices = new List<Node>();
        this.Node_Connections = new Dictionary<int, List<Edge>>();
        Add_new_point(new Node(0, 0, 0));
        this.SortLine_Mesh = new GameObject("Sort_Line");

    }

    public void Start()
    {
    }


    public void Update()
    {
    }



    public void Add_new_point(Node VertexData)
    {

        this.Node_vertices.Add(VertexData);
        this.Node_Connections[Node_vertices.Count] = new List<Edge>();

    }

    public void Add_new_edge(int StartIndex, int StopIndex)
    {
        int diff = 0;
        float rotation = 0;
        int dir = 0;

        Node StartNode = this.Node_vertices[StartIndex - 1];
        Node StopNode = this.Node_vertices[StopIndex - 1];

        try
        {
            if (StartNode.x == StopNode.x)
            {
                diff = StopNode.y - StartNode.y;
                dir = (diff > 0) ? 1 : -1;
                rotation = 90f;
            }
            else if (StartNode.y == StopNode.y)
            {
                diff = StopNode.x - StartNode.x;
                dir = (diff > 0) ? 2 : -2;
                rotation = 0f;
            }
            Node_Connections[StartIndex].Add(new Edge(dir, StopIndex));
            StopNode.from_ = StartIndex;
            StopNode.Input_dir = dir;
            this.Node_vertices[StopIndex - 1] = StopNode;
        }
        catch (Exception e) { return; }
        Vector3 Location = new Vector3();
        GameObject Egde_ = this.SortLine_Mesh;
        GameObject LinePart = null;
        for (int i = 0; i < Math.Abs(diff) + 1; i++)
        {
            int current_x = 0;
            int current_y = 0;
            switch (dir)
            {
                case 2: current_x = StartNode.x + i; current_y = StartNode.y; break;
                case 1: current_x = StartNode.x; current_y = StartNode.y + i; break;
                case -2: current_x = StartNode.x - i; current_y = StartNode.y; break;
                case -1: current_x = StartNode.x; current_y = StartNode.y - i; break;
            }


            if (i != 0)
            {
                Location = new Vector3(current_x * normilize_field_size, 1, current_y * normilize_field_size);
                LinePart = Instantiate(Straight_Mesh, Location, Quaternion.Euler(new Vector3(0, rotation, 90)));
                LinePart.name = $"Segment{current_x}_{current_y}";
                LinePart.transform.SetParent(Egde_.transform);
            }
        }
        Location = new Vector3(StartNode.x * normilize_field_size, 1, StartNode.y * normilize_field_size);


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
    }

    public float Node_Distance(Node start, Node stop)
    {
        return (float)Math.Sqrt(Math.Pow((stop.x - start.x), 2) + Math.Pow((stop.y - start.y), 2));
    }
    public int NearestVertexIndex(Node new_Vertex)
    {
        if (this.Node_vertices.Count == 1) { return 0;}
        int index = 0;
        float distance = float.MaxValue;
        Node currently_closest_Node = this.Node_vertices[index];
        for (int i = 0; i < this.Node_vertices.Count; i++) 
        {
            float new_distance = Node_Distance(new_Vertex, this.Node_vertices[i]);
            if (new_distance  < distance && !this.Node_vertices[i].final) 
            {
                index = i; 
                distance = new_distance;
            }
        }
        return index;
    }
        

    public void Delete_old_segment(string objectNameToDelete)
    {

        GameObject ParentLine = GameObject.Find("Sort_Line");
        Transform childTransform = ParentLine.transform.Find(objectNameToDelete);

        if (childTransform != null)
        {
            GameObject childObject = childTransform.gameObject;
            DestroyImmediate(childObject);
        }
    }


    public void NewVertex(int x2, int y2, int rotation)
    {

        Node new_Node = new Node(x2, y2, 0);
        new_Node.final = true;
        int Old_node_index = NearestVertexIndex(new_Node);
        Node Closest_node = this.Node_vertices[Old_node_index];
        int x1 = Closest_node.x;
        int y1 = Closest_node.y;
        int new_x = x1, new_y = y1;

        // case gdy krótsza œciana jest na osi x
        // czyli trzeba znaleŸæ boki krótszej œciany z sprawdziæ czy zmeiniaj¹ siê dla x czy dla y. jak dla X, to to jest
        // ten case
        if (x1 == x2)
        {
            new_x = x1;
            new_y = y1;
        }
        else
        {
            //wejœcie od góry
            if (rotation == -2)
            {
                if (y1 < 0)
                {
                    y1 += 1;
                }
                else
                {
                    y1 -= 1;
                }
                new_x = x2;
                new_y = y1;

            }
            //wejœcie od do³u
            else if (rotation == 2)
            {
                if (y1 < 0)
                {
                    y1 -= 1;
                }
                else
                {
                    y1 += 1;
                }
                new_x = x2;
                new_y = y1;

            }
        }
        //case gdy sciana jest na osi y
        if (y1 == y2)
        {
            new_x = x1;
            new_y = y1;
        }
        else
        {
            //wejœcie od prawej
            if (rotation == 1)
            {
                if (x1 < 0)
                {
                    x1 -= 1;
                }
                else
                {
                    x1 += 1;
                }
                new_x = x1;
                new_y = y2;
            }
            //wejœcie od lewej
            else if (rotation == -1)
            {
                if (x1 < 0)
                {
                    x1 += 1;
                }
                else
                {
                    x1 -= 1;
                }
                new_x = x2;
                new_y = y1;
            }
        }


        Add_new_point(new_Node);
        int warehouse_Node_index = this.Node_vertices.Count;
        Add_new_point(new Node(new_x, new_y,0));
        int mid_Node_index = this.Node_vertices.Count;
        Add_new_edge(Old_node_index + 1, mid_Node_index);
        Add_new_edge(mid_Node_index, warehouse_Node_index);

        string json = "";
        json += "\"Node_Connections\":{";
        int index = 0;
        foreach (var kvp in Node_Connections)
        {
            json += "\"" + kvp.Key.ToString() + "\":[";
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                json += $"{kvp.Value[i].target}";
                if (i < kvp.Value.Count - 1)
                    json += ",";
            }
            json += "]";
            if (index < Node_Connections.Count - 1)
                json += ",";
            index++;
        }
        json += "}}\n\n";

        for (int i = 0; i < this.Node_vertices.Count; i++) 
        {
            json += $"{i}: x: {Node_vertices[i].x}, y: {Node_vertices[i].y}\n";
        }




        Debug.Log($"{json}");
    }




    public void ReplaceEgde(Edge Old_connection, int StartNodeIndex, int StopNodeIndex, Node NewMidPoint) 
    {
        NewMidPoint.from_ = StartNodeIndex;
        Add_new_point(NewMidPoint);
        int NewMidPoint_Index = this.Node_vertices.Count;
        Old_connection.target = NewMidPoint_Index;
        Node StopNode = this.Node_vertices[StopNodeIndex-1];
        StopNode.from_ = NewMidPoint_Index;
        NewMidPoint.Input_dir = StopNode.Input_dir;
        this.Node_vertices[StopNodeIndex-1] = StopNode;
        Add_new_edge(NewMidPoint_Index, StopNodeIndex);

    }


    public string Serialize()
    {
        string json ="";
        json += "\"Node_Connections\":{";
        int index = 0;
        foreach (var kvp in Node_Connections)
        {
            json += "\"" + kvp.Key.ToString() + "\":[";
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                json += $"{kvp.Value[i].target}";
                if (i < kvp.Value.Count - 1)
                    json += ",";
            }
            json += "]";
            if (index < Node_Connections.Count - 1)
                json += ",";
            index++;
        }
        json += "}}";

        return json;
    }

}
