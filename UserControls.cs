/* Authors: Edwin Zhang, Sepehr Hosseini
 * Last Updated: 2019-08-15
 * 
 * Description: 
 * This script handles a multitude of interactions between the player character and its enviornment. 
 * Said interactions involves primarily user controls, sfx, interactions between enviornment and etc.
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UserControls : MonoBehaviour
{
    //Character Physics and Animations
    public Rigidbody2D rb;
    public Animator anim;


    //Sound Effects
    public AudioSource myAudio;
    public AudioClip jumpSound;
    public AudioClip wallSound;
    public AudioClip hurtSound;
    public AudioClip fireBombSound;
    public AudioClip grassSound;
    public AudioClip rockSound;
    
    //Grapple Mechanics and Physics
    private GameObject closest;
    private SpringJoint2D joint;
    private LineRenderer line;
    private bool isGrounded;
    private bool canGrapple;
    private bool isSwing;

    //Interaction Variables for Enviornment
    private int kbStrength = 15;
    private int spikeStrength = 25;
    private int minGrappleDistance = 2;
    private float triggerSpeed = 6;
    private float jungleBiomeBounds = 84;

    //Initializes Spring Joint 2D and Previous Save Instance
    void Start()
    {
        joint = GetComponent<SpringJoint2D>();
        joint.enabled = false;
        LoadPlayer();
    }

    //Finds Nearest Hinge That Is A Certain Y Value Above Player
    GameObject FindNearest()
    {
        GameObject[] hinges;
        hinges = GameObject.FindGameObjectsWithTag("hinge");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject go in hinges)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && go.transform.position.y > (transform.position.y + minGrappleDistance))
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
    
    //Checks if user is clicking on UI
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //Updates Every Frame
    void Update()
    {
        //Checks When Player Is Allowed To Grapple
        if (isGrounded && rb.velocity.x == 0 && rb.velocity.y == 0)
        {
            canGrapple = true;
        }

        //Finds Nearest Hinge
        if (joint.enabled == false)
        {
            closest = FindNearest();
        }

        //Executes Grappling
        if (closest != null)
        {
            //Creates Grapple Line
            line = gameObject.GetComponentInChildren<LineRenderer>();
            line.startColor = Color.cyan;
            line.SetWidth(0.1f, 0.1f);

            //Initiates Grapple Line
            if (Input.GetMouseButtonDown(0) && canGrapple && IsPointerOverUIObject() == false)
            {   
                    line.positionCount = 2;
                    joint.enabled = true;
                    anim.SetBool("grabRope", true);
                    myAudio.PlayOneShot(jumpSound, 0.5F);
            }

            //Grapples And Moves Player
            if (Input.GetMouseButton(0))
            {
                GameObject hand = GameObject.FindGameObjectWithTag("hand");

                //Flips Player
                if (joint.enabled == true)
                {
                    if (closest.transform.position.x < transform.position.x)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                    }

                    line.SetPosition(0, closest.transform.position);
                    anim.SetBool("swing", true);
                    joint.connectedBody = closest.GetComponentInChildren<Rigidbody2D>();
                    line.SetPosition(1, hand.transform.position);
                }
            }

            //Releases Grapple Line
            if (Input.GetMouseButtonUp(0))
            {
                joint.enabled = false;
                anim.SetBool("swing", false);
                anim.SetBool("grabRope", false);
                anim.SetBool("inAir", true);
                line.positionCount = 0;
            }
        }

        if (IsPointerOverUIObject())
        {
            SavePlayer();
        }
    }
    
    //Detects The Biome The Player Is In
    private string checkBiome()
    {
        if(rb.transform.position.y < jungleBiomeBounds)
        {
            return "JUNGLE";
        }
        else
        {
            return "VOLCANO";
        }
    }

    //Detects Collisions And Executes Corresponding Effects
    void OnCollisionEnter2D(Collision2D col)
    {
        //Knockback Animation
        if (col.gameObject.tag != "ground")
        {
            anim.SetBool("inAir", true);
        }
        else
        {
            anim.SetBool("inAir", false);
        }

        //Plant Spike Knockback
        if (col.gameObject.tag == "Spikes")
        {
            myAudio.PlayOneShot(hurtSound);
            Vector2 forceDir = rb.transform.position - col.transform.position ;
            rb.velocity = forceDir.normalized * kbStrength;
            line.positionCount = 0;
            joint.enabled = false;
            anim.SetBool("swing", false);
        }

        //Grass Collision
        if(col.gameObject.tag == "ground" && checkBiome() == "JUNGLE")
        {
            myAudio.PlayOneShot(grassSound, 0.5F);
            isGrounded = true;
        }

        //Rock Collision
        if (col.gameObject.tag == "ground" && checkBiome() == "VOLCANO")
        {
            myAudio.PlayOneShot(rockSound, 0.5F);
            isGrounded = true;
        }

        //Wooden Spike Variants
        if (col.gameObject.tag == "spike_up")
        {
            myAudio.PlayOneShot(hurtSound);
            rb.velocity = new Vector2(rb.velocity.x, spikeStrength);

        }

        if (col.gameObject.tag == "spike_left")
        {
            myAudio.PlayOneShot(hurtSound);
            rb.velocity = new Vector2(-(spikeStrength + 10), rb.velocity.y);
        }

        if (col.gameObject.tag == "spike_right")
        {
            myAudio.PlayOneShot(hurtSound);
            rb.velocity = new Vector2((spikeStrength + 10), rb.velocity.y);

        }

        //Fireball Collisions
        if (col.gameObject.tag == "lava")
        {

            myAudio.PlayOneShot(hurtSound);
            myAudio.PlayOneShot(fireBombSound);
            Vector2 forceDir = rb.transform.position - col.transform.position;
            rb.velocity = forceDir.normalized * kbStrength;
            line.positionCount = 0;
            joint.enabled = false;
            anim.SetBool("swing", false);
        }

        //Wall Collisions
        if (col.gameObject.tag == "Walls" && col.relativeVelocity.magnitude > triggerSpeed)
        {
            myAudio.PlayOneShot(wallSound);
        }

        //End Game Tile
        if(col.gameObject.tag == "end")
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    //Resets Collisions
    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "ground" || col.gameObject.tag == "Spikes")
        {
            isGrounded = false;
            canGrapple = false;
        }
    }

    //Saves Player Using Save System Class
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }
    
    //Loads Player Using Save System Class
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;

    }

    //Auto Saves Player Upon Exit
    void OnApplicationQuit()
    {
        SavePlayer();
    }
}