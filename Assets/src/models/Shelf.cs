using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Shelf: MonoBehaviour
{
    public GameObject Mesh_;
    public string Shelf_ID;
    public List<string> Packege_to_Slots;
    public int Small_packs_on_shelf;
    public int Medium_packs_on_shelf;
    public int Big_packs_on_shelf;
    public int next_small_Empty;
    public int next_small_Medium;
    public int next_small_Big;
    public int rotation;

    public void Start()
    {}
    public void Update()
    { }

    public void Initialize(GameObject mesh, string name,int rotation)
    { 
        this.Mesh_ = mesh;
        this.Shelf_ID = name;
        this.Packege_to_Slots = Enumerable.Repeat(string.Empty, 9).ToList();
        this.Small_packs_on_shelf = 0;
        this.Medium_packs_on_shelf = 0;
        this.Big_packs_on_shelf = 0;
        this.next_small_Empty = 0;
        this.next_small_Medium = 0;
        this.next_small_Big = 0;
        this.rotation = rotation;
    }

    private int totalCapacity;
    private Dictionary<int, List<Package>> packagesBySize;

    public void Cap(int capacity)
    {
        totalCapacity = capacity;
        packagesBySize = new Dictionary<int, List<Package>>()
        {
            { 0, new List<Package>() }, // Big packages
            { 1, new List<Package>() }, // Medium packages
            { 2, new List<Package>() }  // Small packages
        };
    }

    public void AddPackage(int size, Package package)
    {
        if (packagesBySize.ContainsKey(size))
        {
            packagesBySize[size].Add(package);
        }
    }

    public int GetPackageCount(int size)
    {
        if (packagesBySize.ContainsKey(size))
        {
            return packagesBySize[size].Count;
        }
        return 0;
    }

    public int GetEmptySlots()
    {
        int usedSlots = packagesBySize.Values.Sum(p => p.Count);
        return totalCapacity - usedSlots;
    }

}