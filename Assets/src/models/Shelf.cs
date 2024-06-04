using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public bool Mag_side; //true patrz¹c przez wejœcie po prawej

    public void Start()
    {}
    public void Update()
    { }

    public void Initialize(GameObject mesh, string name,int rotation, bool Mag_side)
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
        this.Mag_side = Mag_side;

        Cap(9.0f);

    }

    public float totalCapacity;
    private Dictionary<int, List<Package>> packagesBySize;

    public void Cap(float capacity)
    {
        totalCapacity = capacity;
        packagesBySize = new Dictionary<int, List<Package>>()
        {
            { 0, new List<Package>() }, // Big packages
            { 1, new List<Package>() }, // Medium packages
            { 2, new List<Package>() }  // Small packages
        };
    }

    //public void AddPackage(int size, Package package)
    //{
    //    if (packagesBySize.ContainsKey(size))
    //    {
    //        packagesBySize[size].Add(package);
    //    }
    //}

    //public int GetPackageCount(int size)
    //{
    //    if (packagesBySize.ContainsKey(size))
    //    {
    //        return packagesBySize[size].Count;
    //    }
    //    return 0;
    //}

    public float GetEmptySlots()
    {
        float usedSlots = 0.0f;

        foreach (var keyValuePair in packagesBySize)
        {
            int size = keyValuePair.Key;
            int number_of_packages = keyValuePair.Value.Count;

            switch (size)
            {
                case 0:
                    usedSlots += number_of_packages * 1.0f;
                    break;
                case 1:
                    usedSlots += number_of_packages * 1.5f;
                    break;
                case 2:
                    usedSlots += number_of_packages * 3.0f;
                    break;
            }
        }
        return totalCapacity - usedSlots;
    }

    public Vector3 GetPosition()
    {
        if (Mesh_ != null)
        {
            if (totalCapacity >= 0.0f && totalCapacity <= 3.0f) 
            {
                Vector3 offset = new Vector3(1.0f, 0.0f, 1.0f);
                return Mesh_.transform.position - offset; 
            }
  
            else if (totalCapacity > 3.0f && totalCapacity <= 6.0f)
            {
                return Mesh_.transform.position;
            }

            else
            {
                Vector3 offset = new Vector3(1.0f, 0.0f, 1.0f);
                return Mesh_.transform.position + offset; 
            }
        }
        else { return Vector3.zero; }
    }
}