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
    [field: SerializeField] public int Range { get; set; } = 5;

    [field: SerializeField] public bool CanAttack { get; set; }
    [field: SerializeField] public bool IsShooting { get; set; }
    [field: SerializeField] public bool CanMove { get; set; }



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
            if(TargetFlag != null)
            { 
                Vector2 direction = (TargetFlag.transform.position - transform.position).normalized;
                rb.velocity = direction * Speed;
            }
        }
        if(CurHealth <= 0)
        {
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
        Vector3 MyPos = this.GunCenter.transform.position;

        Vector2 direction = TargetPos - MyPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + 270, Vector3.forward);
        Quaternion Brotation = Quaternion.AngleAxis(angle , Vector3.forward);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
        this.GunCenter.transform.rotation = rotation;
        if(Vector2.Distance(TargetPos,MyPos) < Range && !IsShooting)
        {
            StartCoroutine(CreateBullet(Brotation));
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
            MainBase.UnitDeath();
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
        
        if(collision.gameObject.GetComponent<Bullet>())
        {
            Bullet BulletScript = collision.gameObject.GetComponent<Bullet>();
            if(BulletScript.EntityTeam != this.EntityTeam)
            {
                this.CurHealth -= BulletScript.Damage;
                Debug.Log("Hit by" + collision.gameObject.name + "  damage " + BulletScript.Damage);
                Destroy(collision.gameObject);
            }
        }
    }
}