using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billion : MonoBehaviour
{

    [SerializeField] public Base BaseObj { get; set; }
    [SerializeField] public GameObject ParentObj { get; set; }
    [SerializeField] public Team TeamName { get; set; }

    

    [SerializeField] public SpriteRenderer SpriteRender { get; set; }
    [SerializeField] public int MaxHealth { get; set; }
    [SerializeField] public int CurHealth { get; set; }

    [SerializeField] public Vector3 MovementTarget { get; set; }
    [SerializeField] public Vector3 EnemyTarget { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
