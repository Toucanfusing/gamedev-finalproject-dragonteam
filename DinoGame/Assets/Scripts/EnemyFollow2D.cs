using UnityEngine;

public class EnemyFollow2D : MonoBehaviour
{
    private GameObject target;
    private bool hasLineOfSight;
    public float chaseSpeed;
    public float chaseRange;
    private Rigidbody2D rigi;
    private bool facingRight;

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 toScale = new Vector3(-1 * this.transform.localScale.x,
                                        this.transform.localScale.y,
                                        this.transform.localScale.z);
        this.transform.localScale = toScale;
    }

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        rigi = GetComponent<Rigidbody2D>();
        facingRight = false; //<<<<<this will be false or true, depending the sprite
    }

    void FixedUpdate()
    {
        Vector3 toPlayer = (target.transform.position - this.transform.position).normalized;
        Debug.DrawRay(this.transform.position, toPlayer * chaseRange, Color.red);
        if (toPlayer.x < 0 && facingRight || toPlayer.x > 0 && !facingRight)
        {
            Flip();
        }
        
        RaycastHit2D seeing = Physics2D.Raycast(this.transform.position,
                                                toPlayer, chaseRange);
        /*For this to work, you need to disable "Queries Start In Colliders" in
        the Project Settings > Physics 2D > General Settings*/
        if (seeing.collider != null)
        {
            hasLineOfSight = seeing.collider.CompareTag("Player");
            if (hasLineOfSight)
            {
                
                rigi.AddForce(toPlayer * chaseSpeed);
            }
        }
        else
        {
            hasLineOfSight = false;
        }
        
    }
}
