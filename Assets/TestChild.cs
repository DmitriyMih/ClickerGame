using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleResourcesSystem;

public class TestChild : TestParent
{
    public int a1;
    [HideInInspector] public int b1;
    public int c1 { get; private set; }
    [HideInInspector] public readonly int d1;
    int e1;
    [HideInInspector] public int f1 { get; private set; }
    private int j1;
    [SerializeField] private int k1;
    private int l1 { get; set; }
}