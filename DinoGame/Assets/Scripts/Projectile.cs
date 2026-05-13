using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeSpan;
    private Transform toFollow;
    private bool following;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeSpan);
    }
    void Update()
    {
        if(following)
        {
            this.transform.position =toFollow.position;
        }
    }
    
    public void setFollow(Transform follow)
    {
        toFollow = follow;
        following = true;
    } 

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }
}
