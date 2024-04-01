using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Test : ScriptableObject
{
    [LoadMarker(2)] public int testI; 
}