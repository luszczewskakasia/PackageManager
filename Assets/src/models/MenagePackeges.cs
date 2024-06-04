using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenagePackeges : MonoBehaviour
{
    public List<Package> Package_List;
    void Start()
    {
        Package_List = new List<Package>();
    }

    public void Add_package(Package pack) 
    { 
        
    }

    public List<Package> GetList() { return Package_List; }

    public void SetList(List<Package> data) { Package_List = data; }
}
