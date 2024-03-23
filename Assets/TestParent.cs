using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParent : MonoBehaviour
{
    public int a;
    [HideInInspector] public int b;
    public int c { get; private set; }
    [HideInInspector] public readonly int d;
 [SerializeField]  protected int e;
    [HideInInspector] public int f { get; private set; }
    private int j;
    [SerializeField] private int k;
    private int l { get; set; }
}