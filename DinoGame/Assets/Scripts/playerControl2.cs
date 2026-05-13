using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerControl2 : MonoBehaviour
{
    private InputSystem_Actions ctrl;
    private bool jumpTriggered;
    private float jumpDuration;
    public float maxJumpDuration;  //typical 0.8
    public float speed;  //typical 480
    public float jumpForce;  //typical 2100
    public bool grounded;  //no typical, only public for debugging
    private Rigidbody2D rigi;
    private bool facingRight;
    //counts number of times player has collided with ground object
    //depletes when player leaves ground object--can only jump 
    //when this is greater than 0
    public int collisionCount;
    public float maxWalk;
    public bool isAlive;
    public GameObject restartBtn;
    public GameObject projectile;
    public float ramSpeed;
    private Vector2 preRam;
    private bool raming;
    public float ramDuration;
    private float ramTimer;
    private bool cooling;
    public float cooldownTime;
    private float cooldownCounter;
    public float sOff;

    //method called when jump button depressed
    private void jumpOn(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (collisionCount > 0)
        {
            jumpTriggered = true;
        }
    }

    //method called when jump button released
    private void jumpOff(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        jumpTriggered = false;
    }
    

    //Awake is called when object first instantiates in game
    void Awake()
    {
        isAlive = true;
        collisionCount = 0;
        grounded = false;
        facingRight = true;
        rigi = GetComponent<Rigidbody2D>();
        ctrl = new InputSystem_Actions();
        ctrl.Enable();
        restartBtn.SetActive(false);
        //subscribe jump methods to input action
        ctrl.Player.Jump.started += jumpOn;
        ctrl.Player.Jump.canceled += jumpOff;
        ctrl.Player.Attack.started += attack;
        jumpDuration = 0;
        cooling = false;
        raming = false;
    }

    private void OnDisable()
    {
        ctrl.Disable();
    }
    
    void attack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (isAlive && !cooling)
        {
            preRam = rigi.linearVelocity;
           GameObject ram = Instantiate(projectile, this.transform.position + this.transform.forward * sOff, this.transform.rotation);
            Vector2 ramDir = new Vector2(1f, 0f);
            Projectile ramScript = ram.GetComponent<Projectile>();
            ramScript.setFollow(this.transform);
            if(!facingRight)
           {
            ramDir *= -1;
           }
            rigi.AddForce(ramDir * ramSpeed);
            raming = true;
            cooling = true;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(isAlive)
        {
            Vector2 moveVect = ctrl.Player.Move.ReadValue<Vector2>();
            //check if facing right and moving left
            if (moveVect.x < 0 && facingRight)
            flip();
            else if (moveVect.x > 0 && !facingRight)
            flip();
            //check for vertical movement (i.e. jump)
            if (jumpTriggered && jumpDuration < maxJumpDuration)
            {
                float jumpPercent = (maxJumpDuration - jumpDuration) / maxJumpDuration;
                moveVect.y = jumpForce * Time.deltaTime * jumpPercent * jumpPercent;
             jumpDuration += Time.deltaTime;
             //grounded = false;
            }
            else
            {
                moveVect.y = 0f;
                jumpDuration = 0f;
                jumpTriggered = false;
                rigi.linearVelocity = Vector2.ClampMagnitude(rigi.linearVelocity, maxWalk);
            }
            //scale horizontal movement
            moveVect.x = moveVect.x * speed * Time.deltaTime;
            //apply movement vector
            rigi.AddForce(moveVect);
            

            if (cooling)
            {
                cooldownCounter -= Time.deltaTime;
                if (cooldownCounter < 0f)
                {
                    cooling = false;
                    cooldownCounter = cooldownTime;
                }
            }

            if(raming)
            {
                ramTimer -= Time.deltaTime;
                if(ramTimer < 0f)
                {
                    raming = false;
                    ramTimer = ramDuration;
                    rigi.linearVelocity = preRam;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            collisionCount++;
        }
        else if (other.transform.tag == "Enemy")
        {
            isAlive = false;
            restartBtn.SetActive(true);
        }
        else if(other.transform.tag == "Finish")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            collisionCount--;
        }
    }


    private void flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = this.transform.localScale;
        theScale.x = -1 * theScale.x;
        this.transform.localScale = theScale;
    }
}
