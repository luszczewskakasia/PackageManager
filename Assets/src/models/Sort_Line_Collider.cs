using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sort_Line_Collider : MonoBehaviour
{
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
}
