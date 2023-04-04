using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class Base : Entity
{
    [field: SerializeField] public override Team EntityTeam { get; set; }
    [field: SerializeField] public override GameObject TargetedLocation { get; set; }
    [field: SerializeField] public override GameObject TargetedEntity { get; set; }
    [field: SerializeField] public override Base MainBase { get; set; }

    [field: SerializeField]  Material HealthMat { get; set; }
    [field: SerializeField]  Material ExpMat { get; set; }

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
    [field: SerializeField] public int Level { get; set; } = 0;
    [field: SerializeField] public int MaxLevel { get; set; } = 8;
    [field: SerializeField] public int Experience { get; set; } = 0;
    [field: SerializeField] public int MaxExperience { get; set; } = 360;
    [field: SerializeField] public int ExpValue{ get; set; } = 720;

    [field: SerializeField] public float RotationSpeed { get; set; } = 2; //Attack Turning Speed
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

    [SerializeField] private TMP_Text LevelText;

    //Bruh
    //Planed locations for spawning and potential powerup slots
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

        if(EntityTeam != null )
        {
            this.SpriteRender.color = EntityTeam.TeamColor;
        }

    }

    float BarAngle = 0;
    float HpPercent = 0;
    float XpPercent = 0;
    void FixedUpdate()
    {
        if(BarAngle >= 360)
        {BarAngle = 0;}
        BarAngle += 2;

        HpPercent =  360 * Mathf.Clamp(((float)CurHealth/MaxHealth), 0,1);
        float HealthAngle = 360 - (HpPercent);
        HealthMat.SetFloat("_Arc1", HealthAngle);   
        HealthMat.SetFloat("_Angle", BarAngle);

        XpPercent =  360 * Mathf.Clamp(((float)Experience/MaxExperience), 0,1);
        float XpAngle = 360 - (XpPercent);
        ExpMat.SetFloat("_Arc1", XpAngle);   
        ExpMat.SetFloat("_Angle", BarAngle+180);
        if(Experience/MaxExperience >= 1)
        {
            int RemainderXp = Experience % MaxExperience;
            int LevelsToAdd = Mathf.FloorToInt(Experience / MaxExperience);
            Experience = RemainderXp;
            LevelUp(LevelsToAdd);
        }

    }
    void LevelUp(int LevelsToAdd)
    {
        for(int i = 0; i < LevelsToAdd; i++)
        {
            if(this.Level < MaxLevel)
            {
                Level++;
                if(Level == 1){this.MaxAmountSpawned += 1;}
                else if(Level == 2){this.MaxAmountSpawned += 1;}
                else if(Level == 3){this.MaxAmountSpawned += 2;}
                else if(Level == 4){this.MaxAmountSpawned += 2;}
                else if(Level == 5){this.MaxAmountSpawned += 3;}
                else if(Level == 6){this.MaxAmountSpawned += 3;}
                else if(Level == 7){this.MaxAmountSpawned += 4;}
                else if(Level == 8){this.MaxAmountSpawned += 4;}
                this.MaxExperience = this.MaxExperience * 2;
            }
            else
            {
                //max level reward is a little healing
                this.CurHealth += this.MaxHealth/5;
                if(CurHealth > MaxHealth)
                {CurHealth = MaxHealth;}
            }
        }

        LevelText.text = ""+this.Level;
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

    public void ChangeExperience(int Amount)
    {
        this.Experience += Amount;
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
        for(int i = 0; (i < Amount) && (CurAmountSpawned < MaxAmountSpawned) && Spawns.Count != 0; i++)
        {
            int SID = Random.Range(0,Spawns.Count);
            float Offset = Random.Range(-0.05f,0.05f);
            GameObject NewBillion = Instantiate(BillionUnit,new Vector3(Spawns[SID].transform.position.x+Offset, Spawns[SID].transform.position.y + Offset, Spawns[SID].transform.position.z), Quaternion.identity);
            if (NewBillion.TryGetComponent<Billion>(out Billion BillionScript))
            {
                BillionScript.EntityTeam = this.EntityTeam;
                BillionScript.MainBase = this;
                BillionScript.Level = this.Level;
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
                    if(CurHealth <= 0)
                    {
                        if(BulletScript.EntityTeam.BaseObj != null)
                        {
                            BulletScript.EntityTeam.BaseObj.ChangeExperience(this.ExpValue);
                        }
                    }
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
