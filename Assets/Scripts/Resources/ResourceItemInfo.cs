using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Resources
{
    [CreateAssetMenu(menuName = "Resources/New Recource")]
    public class ResourceItemInfo : ScriptableObject
    {
        [field: SerializeField] public ResourceType ResourcesType { get; private set; }
        [field: SerializeField] public int ResourcesAmount { get; private set; }

        [field: Header("Item View")]
        [field: SerializeField] public Sprite ResourceSprite;
    } 
}