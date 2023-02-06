using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Flag : MonoBehaviour
{
    [field: SerializeField] public Base BaseObj { get; set; }
    [field: SerializeField] public GameObject ParentObj { get; set; }
    [field: SerializeField] public Team TeamName { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRender { get; set; }

    [field: SerializeField] public bool IsActive { get; set; }

    [field: SerializeField] public List<GameObject> Followers { get; set; } //List of Flags



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
