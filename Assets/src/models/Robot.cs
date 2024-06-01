
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
    

    private void Start()
    {
    }

    private void Update()
    {
        if (warehouse_id != null)
        {
            float X = LocationX * 13.05f;
            float Y = LocationY * 13.05f;
            robot_prefab.transform.position = new Vector3(X, 0, Y);
            Vector3 finalPosition = new Vector3(0, 0, 0 + 30);
            if (trigger)
            {
                transform.position = Vector3.Lerp(transform.position, finalPosition, 0.1f * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(X, 0, Y), 0.1f * Time.deltaTime);
            }
        }
    }
}
