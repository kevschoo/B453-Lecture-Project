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
    [field: SerializeField] public int FlagLimit { get; set; } = 2; //Max Flags
    [field: SerializeField] public List<Flag> Flags { get; set; } //List of Flags
    [field: SerializeField] public GameObject FlagType { get; set; } 
    [field: SerializeField] public GameObject BillionType { get; set; } 

    [field: SerializeField] public List<GameObject> SpawnedBillions { get; set; } //List of this objects billions
    [field: SerializeField] public float SpawnRate { get; set; } = 2;
    [field: SerializeField] public int SpawnedSize { get; set; } = 1;
    [field: SerializeField] public int MaxAmountSpawned { get; set; } = 10;
    [field: SerializeField] public int CurAmountSpawned { get; set; } = 0;

    [field: SerializeField] public int Defense { get; set; }
    [field: SerializeField] public int Level { get; set; }

    [field: SerializeField] public float RotationSpeed { get; set; } = 2; //Attack Range
    [field: SerializeField] public float Range { get; set; } = 5; //Attack Range
    [field: SerializeField] public float FireRate { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public bool IsShooting { get; set; }
    [field: SerializeField] public float BulletSpeed { get; set; }
    [field: SerializeField] public bool CanAttack { get; set; }

    [field: SerializeField] public int MaxHealth { get; set; }
    [field: SerializeField] public int CurHealth { get; set; }

    [field: SerializeField] public GameObject MainTurret { get; set; }
    [field: SerializeField] public Transform MTBulletSpawn { get; set; }
    [field: SerializeField] public GameObject BulletPrefab { get; set; }
    //Bruh
    //Planed locations for spawning and potential powerup slots
    [field: SerializeField] public GameObject NorthSlot { get; set; }
    [field: SerializeField] public GameObject NorthEastSlot { get; set; }
    [field: SerializeField] public GameObject EastSlot { get; set; }
    [field: SerializeField] public GameObject SouthEastSlot { get; set; }
    [field: SerializeField] public GameObject SouthSlot { get; set; }
    [field: SerializeField] public GameObject SouthWestSlot { get; set; }
    [field: SerializeField] public GameObject WestSlot { get; set; }
    [field: SerializeField] public GameObject NorthWestSlot { get; set; }

    [field: SerializeField] public List<GameObject> Spawns { get; set; } //Refers to spawn locations for billions

    [field: SerializeField] public bool CanSpawnBillion { get; set; } //Seperate Check from testing number spawned to max
    [field: SerializeField] public bool IsBillionSpawning { get; set; }


    public void UnitDeath(GameObject Billion)
    {
        this.SpawnedBillions.Remove(Billion);
        this.CurAmountSpawned -= 1;
    }
    public void UnitSpawn(GameObject Billion)
    {
        this.SpawnedBillions.Add(Billion);
        this.CurAmountSpawned += 1;
    }


    // Start is called before the first frame update
    void Start()
    {
        MainBase = this;

        CanAttack = true;
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

        if(EntityTeam != null )
        {
            this.SpriteRender.color = EntityTeam.TeamColor;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(CanAttack)
        {
            TargetClosetEnemy();
            if(TargetedEntity != null)
            {
                AttackEnemy();
            }
        };
        
        if(!IsBillionSpawning && CanSpawnBillion && (CurAmountSpawned < MaxAmountSpawned))
        {
            StartCoroutine(CreateBillion(SpawnedSize,BillionType,SpawnRate));
        }
        if(CurHealth <= 0)
        {
            CanAttack = false;
            CanSpawnBillion = false;
            Destroy(this.gameObject);
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
                if(curEnt.EntityTeam != this.EntityTeam)
                {
                DistanceToEnemy = distance;
                TargetedEntity = curEnt.gameObject;
                }
            }
        }
    }

    void AttackEnemy()
    {
        Vector3 TargetPos = TargetedEntity.gameObject.transform.position;
        Vector3 MyPos = this.MainTurret.transform.position;

        Vector2 direction = TargetPos - MyPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle , Vector3.forward);
        this.MainTurret.transform.rotation = Quaternion.Slerp(MainTurret.transform.rotation, rotation, RotationSpeed * Time.deltaTime);
        if(Vector2.Distance(TargetPos,MyPos) < Range && !IsShooting)
        {
            StartCoroutine(CreateBullet(rotation));
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
        UnitSpawn(NewBillion); 
        }
        yield return new WaitForSeconds(RespawnDelay);
        this.IsBillionSpawning = false;
    
    }

    public void CreateFlag(Vector2 MousePos)
    {
        if(Flags.Count < FlagLimit)
        {
            GameObject NewFlag = Instantiate(FlagType,new Vector3(MousePos.x, MousePos.y, 0), Quaternion.identity);
            if (NewFlag.TryGetComponent<Flag>(out Flag FlagScript))
            {
                FlagScript.EntityTeam = this.EntityTeam;
                FlagScript.MainBase = this;
                FlagScript.IsActive = true;
            }
            this.Flags.Add(FlagScript);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Hit by Bullet");
            if(collision.TryGetComponent<Bullet>(out Bullet BulletScript))
            {
                BulletScript = collision.gameObject.GetComponent<Bullet>();
                if(BulletScript.EntityTeam != this.EntityTeam)
                {
                    this.CurHealth -= BulletScript.Damage;
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    void OnDestroy()
    {
        foreach(GameObject Billion in SpawnedBillions)
        {
            Destroy(Billion.gameObject);
        }
        foreach(Flag Flaggy in Flags)
        {
            Destroy(Flaggy.gameObject);
        }
    }

    IEnumerator CreateBullet(Quaternion Rotation)
    {
        this.IsShooting = true;
        GameObject NewBullet = Instantiate(BulletPrefab,new Vector3(MTBulletSpawn.position.x, MTBulletSpawn.position.y, MTBulletSpawn.position.z), Rotation);
        if (NewBullet.TryGetComponent<Bullet>(out Bullet BulletScript))
        {
            {
                BulletScript.EntityTeam = this.EntityTeam;
                BulletScript.Parent = this.gameObject;
                BulletScript.Damage = this.Damage;
                BulletScript.Speed = this.BulletSpeed;
            }
        }

        yield return new WaitForSeconds(this.FireRate);
        this.IsShooting = false;
    
    }









}
