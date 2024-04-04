using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class SortLine:MonoBehaviour
{ 
    public List<List<float>> Node_vertices;
    public Dictionary<int, List<int>> Node_Connections; 
    
    public void Start() 
    {
        Node_vertices = new List<List<float>>();

        Node_vertices.Add(new List<float>() { 0.0f, 0.0f });
        Node_vertices.Add(new List<float>() { 1.0f, 1.0f });
        Node_vertices.Add(new List<float>() { 2.0f, 2.0f });

        Node_Connections = new Dictionary<int, List<int>>();
        Node_Connections[0] = new List<int>();
        Node_Connections[1] = new List<int>();
        Node_Connections[2] = new List<int>();
        Node_Connections[0].Add(1);
        Node_Connections[1].Add(2);
        Node_Connections[2].Add(3);
    }


    public string Serialize()
    {
        string json = "{\"Node_vertices\":[";
        for (int i = 0; i < Node_vertices.Count; i++)
        {
            json += "[";
            for (int j = 0; j < Node_vertices[i].Count; j++)
            {
                json += Node_vertices[i][j].ToString();
                if (j < Node_vertices[i].Count - 1)
                    json += ",";
            }
            json += "]";
            if (i < Node_vertices.Count - 1)
                json += ",";
        }
        json += "],\"Node_Connections\":{";
        int index = 0;
        foreach (var kvp in Node_Connections)
        {
            json += "\"" + kvp.Key.ToString() + "\":[";
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                json += kvp.Value[i].ToString();
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
