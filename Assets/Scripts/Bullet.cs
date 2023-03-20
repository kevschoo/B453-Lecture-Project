using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Bullet : MonoBehaviour
{
    [field: SerializeField] public Team EntityTeam { get; set; }
    [field: SerializeField] public GameObject Parent { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRender { get; set; }

    [field: SerializeField] public int Damage { get; set; } = 1;
    [field: SerializeField] public float Speed { get; set; } = 3;
    [field: SerializeField] public float Life { get; set; } = 4f;


    // Start is called before the first frame update
    void Start()
    {
        SpriteRender.color = EntityTeam.TeamColor;
        Destroy(this.gameObject, Life);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.right * Time.deltaTime * this.Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Billion"))
        {
            //Debug.Log("Hit Bill");
        }
        else if(collision.gameObject.CompareTag("TilemapWall"))
        {
            //Debug.Log("Hit wall");
            Destroy(this.gameObject);
        }

        
    }

}
