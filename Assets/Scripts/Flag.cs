using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Flag : MonoBehaviour
{
    [field: SerializeField] public Base MainBase { get; set; }
    [field: SerializeField] public Team EntityTeam { get; set; }

    //I could use the same sprite render but its faster to not
    [field: SerializeField] public GameObject FlagBody { get; set; }
    [field: SerializeField] public GameObject FlagOffBody { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRender { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRenderOff { get; set; }

    [field: SerializeField] public bool IsActive { get; set; }
    [field: SerializeField] public bool HasFollowerLimit { get; set; }
    [field: SerializeField] public int FollowerLimit { get; set; }
    [field: SerializeField] public List<GameObject> Followers { get; set; } //List of Flag Followers


    // Start is called before the first frame update
    void Start()
    {
        SpriteRender = FlagBody.GetComponent<SpriteRenderer>();
        SpriteRenderOff = FlagOffBody.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSprites();
    }


    void ChangeSprites()
    {
        if(IsActive)
        {
            FlagBody.SetActive(true);
            FlagOffBody.SetActive(false);
        }
        else
        {
            FlagBody.SetActive(false);
            FlagOffBody.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if(MainBase != null)
        {
            MainBase.Flags.Remove(this);
        }
    }

}
