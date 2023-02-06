using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Base : Entity
{
    [field: SerializeField] public override Team EntityTeam { get; set; }
    [field: SerializeField] public override GameObject TargetedLocation { get; set; }
    [field: SerializeField] public override GameObject TargetedEntity { get; set; }
    [field: SerializeField] public override Base MainBase { get; set; }

    [field: SerializeField] public SpriteRenderer SpriteRender { get; set; }
    [field: SerializeField] public List<GameObject> Flags { get; set; } //List of Flags
    [field: SerializeField] public GameObject BillionType { get; set; } 



    [field: SerializeField] public int SpawnRate { get; set; } = 2;
    [field: SerializeField] public int SpawnedSize { get; set; } = 1;
    [field: SerializeField] public int MaxAmountSpawned { get; set; } = 10;
    [field: SerializeField] public int CurAmountSpawned { get; set; } = 0;

    [field: SerializeField] public int Defense { get; set; }
    [field: SerializeField] public int Level { get; set; }

    [field: SerializeField] public int FireRate { get; set; }
    [field: SerializeField] public int Damage { get; set; }

    [field: SerializeField] public int MaxHealth { get; set; }
    [field: SerializeField] public int CurHealth { get; set; }

    //Bruh
    [field: SerializeField] public GameObject NorthSlot { get; set; }
    [field: SerializeField] public GameObject NorthEastSlot { get; set; }
    [field: SerializeField] public GameObject EastSlot { get; set; }
    [field: SerializeField] public GameObject SouthEastSlot { get; set; }
    [field: SerializeField] public GameObject SouthSlot { get; set; }
    [field: SerializeField] public GameObject SouthWestSlot { get; set; }
    [field: SerializeField] public GameObject WestSlot { get; set; }
    [field: SerializeField] public GameObject NorthWestSlot { get; set; }

    [field: SerializeField] public List<GameObject> Spawns { get; set; }


    [field: SerializeField] public bool CanAttack { get; set; }
    [field: SerializeField] public bool CanSpawnBillion { get; set; } //Seperate Check from testing number spawned to max
    [field: SerializeField] public bool IsBillionSpawning { get; set; }


    public void UnitDeath()
    {
        this.CurAmountSpawned -=1;
    }
    public void UnitSpawn()
    {
        this.CurAmountSpawned +=1;
    }


    // Start is called before the first frame update
    void Start()
    {
        MainBase = this;

        CanAttack = false;
        CanSpawnBillion = true;
        IsBillionSpawning = false;

        Spawns.Add(NorthSlot);
        Spawns.Add(NorthEastSlot);
        Spawns.Add(EastSlot);
        Spawns.Add(SouthEastSlot);
        Spawns.Add(SouthSlot);
        Spawns.Add(SouthWestSlot);
        Spawns.Add(WestSlot);
        Spawns.Add(NorthWestSlot);



    }

    // Update is called once per frame
    void Update()
    {
        if(CanAttack)
        {TargetClosetEnemy();};
        
        if(!IsBillionSpawning && CanSpawnBillion && (CurAmountSpawned < MaxAmountSpawned))
        {
            StartCoroutine(CreateBillion(SpawnedSize,BillionType,SpawnRate));
        }

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

    IEnumerator CreateBillion(int Amount, GameObject BillionUnit, float RespawnDelay)
    {

        this.IsBillionSpawning = true;
        for(int i = 0; (i < Amount) && (CurAmountSpawned < MaxAmountSpawned); i++)
        {
        int SID = Random.Range(0,Spawns.Count);
        float Offset = Random.Range(-0.05f,0.05f);
        GameObject NewBillion = Instantiate(BillionUnit,new Vector3(Spawns[SID].transform.position.x+Offset, Spawns[SID].transform.position.y + Offset, Spawns[SID].transform.position.z), Quaternion.identity);
        if (NewBillion.TryGetComponent<Entity>(out Entity BillionScript))
            {
                BillionScript.EntityTeam = this.EntityTeam;
                BillionScript.MainBase = this;
            }
        this.CurAmountSpawned++;    
        }
        yield return new WaitForSeconds(RespawnDelay);
        this.IsBillionSpawning = false;
    
    }
}
