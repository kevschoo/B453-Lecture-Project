using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] public abstract Team EntityTeam { get; set; }

    [SerializeField] public abstract GameObject TargetedLocation { get; set; }

    [SerializeField] public abstract GameObject TargetedEntity { get; set; }

    [SerializeField] public abstract Base MainBase { get; set; }


}
