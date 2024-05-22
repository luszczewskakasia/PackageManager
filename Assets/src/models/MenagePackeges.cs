using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenagePackeges : MonoBehaviour
{
    public List<Package> Package_List;

    // Start is called before the first frame update
    void Start()
    {
        Package_List = new List<Package>();

        for (int i = 0; i < 5; i++)
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, 3);
        }

    }

    public List<Package> GetList() { return Package_List; }

    public void SetList(List<Package> data) { Package_List = data; }
}
