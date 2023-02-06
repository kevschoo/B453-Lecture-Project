using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Bullet : MonoBehaviour
{
    [field: SerializeField] public Team EntityTeam { get; set; }
    [field: SerializeField] public GameObject Parent { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRender { get; set; }

    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public float Speed { get; set; }
    [field: SerializeField] public float Life { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
