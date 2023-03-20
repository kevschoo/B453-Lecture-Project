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
    [field: SerializeField] public GameObject BulletPrefab { get; set; }


    [field: SerializeField] public Rigidbody2D rb { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRender { get; set; }
    [field: SerializeField] public List<Flag> Flags { get; set; } //List of Flags
    [field: SerializeField] public Flag TargetFlag { get; set; } //Flag Game Object

    [field: SerializeField] public GameObject BillionBody { get; set; }
    [field: SerializeField] public GameObject GunCenter { get; set; }
    [field: SerializeField] public GameObject GunLeft { get; set; }
    [field: SerializeField] public GameObject GunRight { get; set; }
    [field: SerializeField] public Transform GunCenterFirePoint { get; set; }
    [field: SerializeField] public Transform GunLeftFirePoint { get; set; }
    [field: SerializeField] public Transform GunRightFirePoint { get; set; }

    [field: SerializeField] public int MaxHealth { get; set; }
    [field: SerializeField] public int CurHealth { get; set; }
    [field: SerializeField] public int FireRate { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public int BulletSpeed { get; set; }
    [field: SerializeField] public int Speed { get; set; }
    [field: SerializeField] public int Range { get; set; } = 5; //Attack Range
    [field: SerializeField] public float MaxSpeed { get; set; } = 3f;
    [field: SerializeField] public float Acceleration { get; set; } = .5f;
    [field: SerializeField] public bool CanAttack { get; set; }
    [field: SerializeField] public bool IsShooting { get; set; }
    [field: SerializeField] public bool CanMove { get; set; }

    Vector3 LocalScaleModifer = new Vector3(1,1,1);

    // Start is called before the first frame update
    void Start()
    {
        if(EntityTeam != null )
        {
            this.SpriteRender.color = EntityTeam.TeamColor;
        }
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float Size = ((CurHealth+MaxHealth)/(2f))/MaxHealth;
        if(Size > 1) {Size = 1;}
        LocalScaleModifer.x = Size; LocalScaleModifer.y = Size;
        BillionBody.transform.localScale = LocalScaleModifer;

        if(CanAttack)
        {
            TargetClosetEnemy();
            if(TargetedEntity != null)
            {
                AttackEnemy();
            }
        };
        if(CanMove)
        {
            GetClosetFlag();
        }
        if(CurHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if(TargetFlag != null)
        { 
            Vector2 direction = (TargetFlag.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, TargetFlag.transform.position);

            if (distance > 0f)
            {
                float dirSpeed = Mathf.Min(MaxSpeed, Acceleration * distance);
                rb.velocity = direction * dirSpeed;
            }
            else
            {
            rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
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
        Vector3 MyPos = this.GunCenter.transform.position;

        Vector2 direction = TargetPos - MyPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle , Vector3.forward);
        this.GunCenter.transform.rotation = Quaternion.AngleAxis(angle + 270, Vector3.forward);
        if(Vector2.Distance(TargetPos,MyPos) < Range && !IsShooting)
        {
            StartCoroutine(CreateBullet(rotation));
        }

    }
    void GetClosetFlag()
    {
        float DistanceToFlag = Mathf.Infinity;
        TargetFlag = null;

        if(MainBase != null)
        {
            Flags = MainBase.Flags;
        }

        foreach(Flag curFlag in Flags)
        {
            if(curFlag.GetComponent<Flag>().IsActive)
            {
            float distance = (curFlag.transform.position - this.transform.position).sqrMagnitude;
                if(distance < DistanceToFlag)
                {
                    DistanceToFlag = distance;
                    TargetFlag = curFlag;
                }
            }
            
            
        }
    }
    
    void OnDestroy()
    {
        if(MainBase != null)
        {
            MainBase.UnitDeath(this.gameObject);
        }
    }

    IEnumerator CreateBullet(Quaternion Rotation)
    {
        this.IsShooting = true;
        GameObject NewBullet = Instantiate(BulletPrefab,new Vector3(GunCenterFirePoint.position.x, GunCenterFirePoint.position.y, GunCenterFirePoint.position.z), Rotation);
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Hit by "+collision.gameObject.name);
        if(collision.gameObject.CompareTag("Projectile"))
        {
            //Debug.Log("Hit by Bullet");
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
}
