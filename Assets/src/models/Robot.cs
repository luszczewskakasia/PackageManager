
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
    

    private void Start()
    {
    }

    private void Update()
    {
        if (warehouse_id != null)
        {
            float X = LocationX * 13.05f;
            float Y = LocationY * 13.05f;
            Vector3 finalPosition = new Vector3(0, 0, 0 + 30);
            if (trigger)
            {
                transform.position = Vector3.Lerp(transform.position, finalPosition, 1.0f * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, start_pos, 0.1f * Time.deltaTime);
            }
        }
    }
    public void set_start_pos (Vector3 pos)
    {
        this.start_pos = pos;
    }
}
