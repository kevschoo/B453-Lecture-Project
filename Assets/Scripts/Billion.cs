using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Billion : Entity
{
    [field: SerializeField]public override Team EntityTeam { get; set; }
    [field: SerializeField]public override GameObject TargetedLocation { get; set; }
    [field: SerializeField]public override GameObject TargetedEntity { get; set; }
    [field: SerializeField]public override Base MainBase { get; set; }

    [field: SerializeField] public SpriteRenderer SpriteRender { get; set; }
    [field: SerializeField] public List<GameObject> Flags { get; set; } //List of Flags
    [field: SerializeField] public GameObject TargetFlag { get; set; } //Flag Game Object

    [field: SerializeField] public GameObject GunCenter { get; set; }
    [field: SerializeField] public GameObject GunLeft { get; set; }
    [field: SerializeField] public GameObject GunRight { get; set; }

    [field: SerializeField] public int MaxHealth { get; set; }
    [field: SerializeField] public int CurHealth { get; set; }
    [field: SerializeField] public int FireRate { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public int Speed { get; set; }

    [field: SerializeField] public bool CanAttack { get; set; }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CanAttack)
        {TargetClosetEnemy();};
        
    }

    void TargetClosetEnemy()
    {
        float DistanceToEnemy = Mathf.Infinity;
        TargetedEntity = null;

        Entity[] AllTargets = GameObject.FindObjectsOfType<Entity>();

        foreach( Entity curEnt in AllTargets)
        {
            float distance = (curEnt.transform.position - this.transform.position).sqrMagnitude;
            if(distance < DistanceToEnemy)
            {
                DistanceToEnemy = distance;
                TargetedEntity = curEnt.gameObject;
            }
        }
    }


}
