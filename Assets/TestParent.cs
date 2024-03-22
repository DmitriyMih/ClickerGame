using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParent : MonoBehaviour
{
    public int a;
    [HideInInspector] public int b;
    public int c { get; private set; }
    public readonly int d;
}