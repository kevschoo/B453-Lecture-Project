using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Team : MonoBehaviour
{
    [field: SerializeField] public Base BaseObj { get; set; }
    [field: SerializeField] public int TeamID { get; set; }
    [field: SerializeField] public int Score { get; set; }
    [field: SerializeField] public Color TeamColor { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
