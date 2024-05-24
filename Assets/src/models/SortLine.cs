using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using System.ComponentModel;
using TreeEditor;
using Unity.VisualScripting;

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
    public int Index;
    public int Input_dir;
    public bool final;
    public int from_;

    // Konstruktor
    public Node(int x_, int y_, int val)
    {
        x = x_;
        y = y_;
        Index = 1;
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

    private GameObject SortLine_Mesh;

    public void Awake()
    {
        this.Node_vertices = new List<Node>();
        this.Node_Connections = new Dictionary<int, List<Edge>>();
        Node ScanPoint = new Node(0, 0, 0);
        ScanPoint.final = true;
        Node StartNode = new Node(1, 0, 0);
        StartNode.from_ = 1;
        StartNode.Input_dir = 2;
        this.SortLine_Mesh = new GameObject("Sort_Line");
        int ScP = Add_new_point(ScanPoint);
        int StN = Add_new_point(StartNode);
        Add_new_edge(ScP, StN);

    }
    public void Start()
    {
    }
    public void Update()
    {
    }
    public int? IsNodeExist(Node CheckNode) 
    { 
        for(int i=0; i<Node_vertices.Count; i++) 
        {
            if (CheckNode.x == Node_vertices[i].x && CheckNode.y == Node_vertices[i].y) { return i + 1; }
        }
        return null;
    }
    public int Add_new_point(Node VertexData)
    {
        VertexData.Index = Node_vertices.Count + 1;
        this.Node_vertices.Add(VertexData);
        this.Node_Connections[Node_vertices.Count] = new List<Edge>();
        //Debug.Log($"Punkt{this.Node_vertices.Count}");
        return this.Node_vertices.Count;
    }
    public void Add_new_edge(int StartIndex, int StopIndex)
    {
        int diff = 0;
        float rotation = 0;
        int dir = 0;

        Node StartNode = this.Node_vertices[StartIndex - 1];
        Node StopNode = this.Node_vertices[StopIndex - 1];

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
                //Debug.Log($"Skrzy¿owanie Node1:{Edge1Dir}, Node2:{Edge2Dir}, InputDir:{StartNode.Input_dir}  ");

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

                if (CorrectCross == null)
                {
                }
                else {
                    Delete_old_segment($"Segment{StartNode.x}_{StartNode.y}");
                    LinePart = Instantiate(CorrectCross, Location, Quaternion.Euler(new Vector3(rotationX, rotation, 0)));
                    LinePart.name = $"Segment{StartNode.x}_{StartNode.y}";
                    LinePart.transform.SetParent(Egde_.transform);
                }

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
    public List<Node> slidePointAlongEdge(Node pointPrev, Node pointNext, Node TargetNode, WarehouseFiled border) 
    {
        List<Node> slided = new List<Node>();
        if (pointPrev.x > pointNext.x && pointPrev.y == pointNext.y) 
        {
            //Debug.Log($"Slided_0");
            slided.Add(new Node(border.MinX - 1, pointPrev.y, 0));
            slided.Add(new Node(border.MinX - 1, TargetNode.y, 0));
            return slided;
        }
        if (pointPrev.x < pointNext.x && pointPrev.y == pointNext.y)
        {
            //Debug.Log($"Slided_1");
            slided.Add(new Node(border.MaxX + 1, pointPrev.y, 0));
            slided.Add(new Node(border.MaxX + 1, TargetNode.y, 0));
            return slided;
        }
        if (pointPrev.y > pointNext.y && pointPrev.x == pointNext.x)
        {
            //Debug.Log($"Slided_2");
            slided.Add(new Node(pointPrev.x, border.MaxY + 1, 0));
            slided.Add(new Node(TargetNode.x, border.MaxY + 1, 0));
            return slided;
        }
        if (pointPrev.y < pointNext.y && pointPrev.x == pointNext.x)
        {
            //Debug.Log($"Slided_3");
            slided.Add(new Node(pointPrev.x, border.MinY - 1, 0));
            slided.Add(new Node(TargetNode.x, border.MinY - 1, 0));
            return slided;
        }
        return slided;
    }
    public List<Node> SlideEgde(Node point1, Node point2,WarehouseFiled border) 
    {
        List<Node> slided = new List<Node>();
        if (point1.y == point2.y) 
        {
            if (Math.Abs(point1.y - border.MinY) < Math.Abs(point1.y - border.MaxY)) 
            {
                //Debug.Log($"{point1.y} a nowa wartoœæ {border.MinY - 1}");
                slided.Add(new Node(point1.x, border.MinY - 1, 0));
                slided.Add(new Node(point2.x, border.MinY - 1, 0));
                return slided;
            }
            else 
            {
                //Debug.Log($"{point1.y} a nowa wartoœæ {border.MaxY + 1}");
                slided.Add(new Node(point1.x, border.MaxY + 1, 0));
                slided.Add(new Node(point2.x, border.MaxY + 1, 0));
                return slided;
            }
        }
        if (point1.x == point2.x)
        {
            if (Math.Abs(point1.x - border.MinX) < Math.Abs(point1.x - border.MaxX))
            {
                //Debug.Log($"{point1.x} a nowa wartoœæ {border.MinX - 1}");
                slided.Add(new Node(border.MinX - 1, point1.y, 0));
                slided.Add(new Node(border.MinX - 1, point2.y, 0));
                return slided;
            }
            else
            {
                //Debug.Log($"{point1.x} a nowa wartoœæ {border.MaxX + 1}");
                slided.Add(new Node(border.MaxX + 1, point1.y, 0));
                slided.Add(new Node(border.MaxX + 1, point2.y, 0));
                return slided;
            }
        }
        return slided;
    }
    public List<Node> Modify_temp_line(List<Node> temp_nodes, List<WarehouseFiled> borders) 
    {
        int Len = temp_nodes.Count;


        Node Closet_Node = temp_nodes[Len - 1];
        Node Node_to_check = temp_nodes[Len - 2];
        int dir = 0;
        if (Closet_Node.x > Node_to_check.x) { dir = -2; }
        if (Closet_Node.x < Node_to_check.x) { dir = 2; }
        if (Closet_Node.y < Node_to_check.y) { dir = 1; }
        if (Closet_Node.y > Node_to_check.y) { dir = -1; }
        //Debug.Log($"Indeks najbli¿szego Noda {Closet_Node.Index}, {Closet_Node.x}, {Closet_Node.y} ,");

        List<Edge> Edges = Node_Connections[Closet_Node.Index];
        Edge? Old_Connections = null;
        int? Old_Target = null;
        int? Old_Edge_index = null;
        for (int i = 0; i < Edges.Count; i++)
        {
            if (Edges[i].dir == dir)
            {
                Old_Connections = Edges[i];
                Old_Target = Edges[i].target;
                Old_Edge_index = i;
                break;
            }
        }
        string str = "";


        if (Old_Edge_index != null)
        {
            Node New_Closet = ReplaceEgde((Edge)Old_Connections, Closet_Node.Index, (int)Old_Target, Node_to_check);
            //Debug.Log($"Indeks najbli¿szego nowego Noda {New_Closet.Index}, {New_Closet.x}, {New_Closet.y} ,");

            //temp_nodes[Len - 1] = New_Closet;
        }



        List<Node> Corrected_line = new List<Node>();
        Corrected_line.Add(temp_nodes[0]);

        if (temp_nodes.Count != 2)
        {
            for (int i = 1; i < Len - 1; i++)
            {
                bool inside = false;
                for (int j = 0; j < borders.Count; j++)
                {
                    if (borders[j].Check_point(temp_nodes[i]))
                    {
                        List<Node> SlidePoint = slidePointAlongEdge(temp_nodes[i - 1], temp_nodes[i], temp_nodes[i + 1], borders[j]);
                        Corrected_line.Add(SlidePoint[0]);
                        Corrected_line.Add(SlidePoint[1]);
                        inside = true;
                    }
                    if (borders[j].Check_edge(temp_nodes[i - 1], temp_nodes[i]))
                    {
                        List<Node> SlidePoint = SlideEgde(temp_nodes[i - 1], temp_nodes[i], borders[j]);
                        if (temp_nodes.Count == 2)
                        {
                            Corrected_line.Add(SlidePoint[0]);
                            Corrected_line.Add(SlidePoint[1]);
                        }
                        Corrected_line[Corrected_line.Count - 1] = SlidePoint[0];
                        Corrected_line.Add(SlidePoint[1]);
                        inside = true;
                    }
                }
                if (!inside) { Corrected_line.Add(temp_nodes[i]); }
            }

            Corrected_line.Add(temp_nodes[temp_nodes.Count - 1]);

        }
        else 
        {
            for (int j = 0; j < borders.Count; j++) 
            {
                if (borders[j].Check_edge(temp_nodes[0], temp_nodes[1]))
                {
                    Node Slided_out = new Node(0,0,0);
                    if (temp_nodes[0].x < temp_nodes[1].x) {Corrected_line.Add(Slided_out = new Node(temp_nodes[0].x+1, temp_nodes[0].y, 0));}
                    if (temp_nodes[0].x > temp_nodes[1].x) {Corrected_line.Add(Slided_out = new Node(temp_nodes[0].x - 1, temp_nodes[0].y, 0)); }
                    if (temp_nodes[0].y < temp_nodes[1].y) { Corrected_line.Add(Slided_out = new Node(temp_nodes[0].x, temp_nodes[0].y + 1, 0)); }
                    if (temp_nodes[0].y > temp_nodes[1].y) { Corrected_line.Add(Slided_out = new Node(temp_nodes[0].x, temp_nodes[0].y - 1, 0)); }
                    List<Node> SlidePoint = SlideEgde(Slided_out, temp_nodes[1], borders[j]);
                    Corrected_line.Add(SlidePoint[0]);
                    Corrected_line.Add(SlidePoint[1]);
                }
            }
            Corrected_line.Add(temp_nodes[temp_nodes.Count - 1]);

        }


        str = "Wstêpna lista: ";
        for (int i = 0; i < temp_nodes.Count; i++) { str += $"[{temp_nodes[i].x}, {temp_nodes[i].y}] ,"; }
        str += "]\n";
        //Debug.Log(str);
        str = "Poprawiona Lista: ";
        for (int i = 0; i < Corrected_line.Count; i++) { str += $"[{Corrected_line[i].x} , {Corrected_line[i].y}] ,"; }
        str += "]\n";
        //Debug.Log(str);
        return Corrected_line;

        return temp_nodes;
    }
    public void NewVertex(int Warehouse_x, int Warehouse_y, int rotation,int Max_Warehouse_x, int Min_Warehouse_x, int Max_Warehouse_y, int Min_Warehouse_y, List<WarehouseFiled> borders)
    {
        int new_nodes_number = 0;
        int x_1 = 0, y_1 = 0, x_2 = 0, y_2 = 0, x_3 = 0, y_3 = 0, newNode_index = 1, Previous_index = 1;

        Node new_Node = new Node(Warehouse_x, Warehouse_y, 0);
        new_Node.final = true;
        Previous_index = NearestVertexIndex(new_Node);
        Node Closest_node = this.Node_vertices[Previous_index];
        List<Node> new_nodes = new List<Node>();
        switch (rotation)
        {
            case 2:

                if (Closest_node.x < Min_Warehouse_x &&
                    Closest_node.y >= Min_Warehouse_y && Closest_node.y <= Max_Warehouse_y)
                {
                    new_nodes_number = 3;
                    x_1 = Warehouse_x + 1;
                    y_1 = Warehouse_y;
                    x_2 = x_1;
                    if (Math.Abs(Max_Warehouse_y) < Math.Abs(Min_Warehouse_y))
                    {
                        y_2 = Min_Warehouse_y - 1;
                    }
                    else
                    {
                        y_2 = Max_Warehouse_y + 1;
                    }
                    x_3 = Closest_node.x;
                    y_3 = y_2;
                }
                if ((Closest_node.y < Min_Warehouse_y || Closest_node.y > Max_Warehouse_y) && Closest_node.x <= Warehouse_x)
                {
                    new_nodes_number = 2;
                    x_1 = Warehouse_x + 1;
                    y_1 = Warehouse_y;
                    x_2 = x_1;
                    y_2 = Closest_node.y;
                }
                if (Closest_node.x > Warehouse_x)
                {
                    new_nodes_number = 1;
                    x_1 = Closest_node.x;
                    y_1 = Warehouse_y;
                }
                if(Closest_node.y == Warehouse_y && Warehouse_x < Closest_node.x)
                {
                    new_nodes_number = 0;
                }

                break;
            case -2:
                if (Closest_node.x > Max_Warehouse_x &&
                    Closest_node.y >= Min_Warehouse_y && Closest_node.y <= Max_Warehouse_y)
                {
                    new_nodes_number = 3;
                    x_1 = Warehouse_x - 1;
                    y_1 = Warehouse_y;
                    x_2 = x_1;
                    if (Math.Abs(Max_Warehouse_y) < Math.Abs(Min_Warehouse_y))
                    {
                        y_2 = Min_Warehouse_y - 1;
                    }
                    else
                    {
                        y_2 = Max_Warehouse_y + 1;
                    }
                    x_3 = Closest_node.x;
                    y_3 = y_2;

                }
                if ((Closest_node.y < Min_Warehouse_y || Closest_node.y > Max_Warehouse_y) && Closest_node.x >= Warehouse_x)
                {
                    new_nodes_number = 2;
                    x_1 = Warehouse_x - 1;
                    y_1 = Warehouse_y;
                    x_2 = x_1;
                    y_2 = Closest_node.y;


                }
                if (Closest_node.x < Warehouse_x)
                {
                    new_nodes_number = 1;
                    x_1 = Closest_node.x;
                    y_1 = Warehouse_y;

                }
                if (Closest_node.y == Warehouse_y && Warehouse_x > Closest_node.x)
                {
                    new_nodes_number = 0;
                }
                break;
            case -1:
                if (Closest_node.y > Max_Warehouse_y &&
                    Closest_node.x >= Min_Warehouse_x && Closest_node.x <= Max_Warehouse_x)
                {
                    new_nodes_number = 3;

                    x_1 = Warehouse_x;
                    y_1 = Warehouse_y - 1;
                    y_2 = y_1;
                    if (Math.Abs(Max_Warehouse_x) < Math.Abs(Min_Warehouse_x))
                    {
                        x_2 = Max_Warehouse_x + 1;
                    }
                    else
                    {
                        x_2 = Min_Warehouse_x - 1;
                    }
                    y_3 = Closest_node.y;
                    x_3 = x_2;

                }
                if ((Closest_node.x < Min_Warehouse_x || Closest_node.x > Max_Warehouse_x) && Closest_node.y >= Warehouse_y)
                {
                    new_nodes_number = 2;
                    y_1 = Warehouse_y - 1;
                    x_1 = Warehouse_x;
                    y_2 = y_1;
                    x_2 = Closest_node.x;

                }
                if (Closest_node.y < Warehouse_y)
                {
                    new_nodes_number = 1;
                    y_1 = Closest_node.y;
                    x_1 = Warehouse_x;
                }
                if (Closest_node.x == Warehouse_x && Warehouse_y > Closest_node.y)
                {
                    new_nodes_number = 0;
                }
                break;
            case 1:
                if (Closest_node.y < Min_Warehouse_y &&
                    Closest_node.x >= Min_Warehouse_x && Closest_node.x <= Max_Warehouse_x)
                {
                    new_nodes_number = 3;

                    x_1 = Warehouse_x;
                    y_1 = Warehouse_y + 1;
                    if (Math.Abs(Max_Warehouse_x) < Math.Abs(Min_Warehouse_x))
                    {
                        x_2 = Max_Warehouse_x + 1;
                    }
                    else
                    {
                        x_2 = Min_Warehouse_x - 1;
                    }
                    y_2 = y_1;
                    y_3 = Closest_node.y;
                    x_3 = x_2;
                }
                if ((Closest_node.x < Min_Warehouse_x || Closest_node.x > Max_Warehouse_x) && Closest_node.y <= Warehouse_y)
                {
                    new_nodes_number = 2;
                    y_1 = Warehouse_y + 1;
                    x_1 = Warehouse_x;
                    y_2 = y_1;
                    x_2 = Closest_node.x;

                }
                if (Closest_node.y > Warehouse_y)
                {
                    new_nodes_number = 1;
                    y_1 = Closest_node.y;
                    x_1 = Warehouse_x;
                }
                if (Closest_node.x == Warehouse_x && Warehouse_y < Closest_node.y)
                {
                    new_nodes_number = 0;
                }
                break;
        }
        switch (new_nodes_number)
        {
            case 3:
                new_nodes.Add(new_Node);
                new_nodes.Add(new Node(x_1, y_1, 0));
                new_nodes.Add(new Node(x_2, y_2, 0));
                new_nodes.Add(new Node(x_3, y_3, 0));
                new_nodes.Add(Closest_node);
                new_nodes = Modify_temp_line(new_nodes, borders);


                break;
            case 2:
                new_nodes.Add(new_Node);
                new_nodes.Add(new Node(x_1, y_1, 0));
                new_nodes.Add(new Node(x_2, y_2, 0));
                new_nodes.Add(Closest_node);
                new_nodes = Modify_temp_line(new_nodes, borders);

                break;
            case 1:
                new_nodes.Add(new_Node);
                new_nodes.Add(new Node(x_1, y_1, 0));
                new_nodes.Add(Closest_node);
                new_nodes = Modify_temp_line(new_nodes, borders);

                break;
            case 0:
                new_nodes.Add(new_Node);
                new_nodes.Add(Closest_node);
                new_nodes = Modify_temp_line(new_nodes, borders);
                break;
        }
        Previous_index += 1;
        int? index_ = null;
        new_nodes = Split_to_long_edges(new_nodes);
        for (int i = 1; i < new_nodes.Count; i++) 
        {
            index_ = IsNodeExist(new_nodes[new_nodes.Count - i - 1]);
            if (index_ == null)
            {
                newNode_index = Add_new_point(new_nodes[new_nodes.Count - i - 1]);
            }
            else { newNode_index = (int)index_; }
            if (index_ != newNode_index) 
            {
                Add_new_edge(Previous_index, newNode_index);
            }
            Previous_index = newNode_index;
        }

        string json = "";
        json += "\"Node_Connections\":{";
        int index = 0;
        foreach (var kvp in Node_Connections)
        {
            json += "\"" + (kvp.Key).ToString() + "\":[";
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
            json += $"{Node_vertices[i].Index}: x: {Node_vertices[i].x}, y: {Node_vertices[i].y},Input_dir: {Node_vertices[i].Input_dir},from_: {Node_vertices[i].from_}\n";
        }
        //Debug.Log($"{json}");
    }
    public Node ReplaceEgde(Edge Old_connection, int StartNodeIndex, int StopNodeIndex, Node NewMidPoint) 
    {
        NewMidPoint.from_ = StartNodeIndex;
        Node StopNode = this.Node_vertices[StopNodeIndex - 1];
        Node StartNode = this.Node_vertices[StartNodeIndex - 1];

        if (
            ((StartNode.x == StopNode.x) && ((StartNode.y > NewMidPoint.y && StopNode.y < NewMidPoint.y) || (StartNode.y < NewMidPoint.y && StopNode.y > NewMidPoint.y)))||
            ((StartNode.y == StopNode.y) && ((StartNode.x > NewMidPoint.x && StopNode.x < NewMidPoint.x) || (StartNode.x < NewMidPoint.x && StopNode.x > NewMidPoint.x)))
            )
        {
            NewMidPoint.Input_dir = StopNode.Input_dir;
            int NewMidPoint_Index = Add_new_point(NewMidPoint);
            Old_connection.target = NewMidPoint_Index;
            StopNode.from_ = NewMidPoint_Index;
            this.Node_vertices[StopNodeIndex - 1] = StopNode;
            Add_edge_without_mesh(NewMidPoint_Index, StopNodeIndex);
        }
        return NewMidPoint;


    }
    public List<Node> Split_to_long_edges(List<Node> Not_Splited) 
    {
        bool all_lenght_correct = false;
        while (!all_lenght_correct) 
        {
            List<Node> Splited = new List<Node>();
            all_lenght_correct = true;
            for (int i = 1; i < Not_Splited.Count; i++) 
            {
                float distance = Node_Distance(Not_Splited[i - 1], Not_Splited[i]);
                int distance_ = (int)distance;
                if (distance_ > 10)
                {
                    if (Not_Splited[i - 1].x == Not_Splited[i].x) 
                    {
                        Splited.Add(Not_Splited[i - 1]);
                        Splited.Add(new Node(Not_Splited[i - 1].x, (int)(Not_Splited[i - 1].y + Not_Splited[i].y) /2 , 0));
                        all_lenght_correct = false;
                    }
                    if (Not_Splited[i - 1].y == Not_Splited[i].y)
                    {
                        Splited.Add(Not_Splited[i - 1]);
                        Splited.Add(new Node((int)(Not_Splited[i - 1].x + Not_Splited[i].x) / 2, Not_Splited[i - 1].y, 0));
                        all_lenght_correct = false;
                    }
                }
                else 
                {
                    Splited.Add(Not_Splited[i - 1]);
                }
            }
            Splited.Add(Not_Splited[Not_Splited.Count - 1]);
            Not_Splited = Splited;
        }
        return Not_Splited;
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
    public void Add_edge_without_mesh(int StartIndex, int StopIndex) 
    {
        int diff = 0;
        float rotation = 0;
        int dir = 0;

        Node StartNode = this.Node_vertices[StartIndex - 1];
        Node StopNode = this.Node_vertices[StopIndex - 1];


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
    }
    public bool DFS(int start_index, int endIndex, List<Node> path)
    {
        path.Add(this.Node_vertices[start_index]);
        if (start_index  == endIndex)
            return true;
        foreach (Edge child in Node_Connections[start_index+1])
        {
            int childIndex = child.target - 1;
            if (DFS(childIndex, endIndex, path))
                return true;
        }
        path.RemoveAt(path.Count - 1);
        return false;
    }
    public List<Node> FindPath(int startValue, int Node_x, int Node_y)
    {
        int endValue = (int)(IsNodeExist(new Node(Node_x, Node_y, 0))-1);
        List<Node> path = new List<Node>();
        if (DFS(startValue, endValue, path))
            return path;
        return new List<Node>();
    }
}







