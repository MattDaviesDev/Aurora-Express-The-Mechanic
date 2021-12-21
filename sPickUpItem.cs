using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPickUpItem : MonoBehaviour
{
    float throwForce = 300;
    Vector3 objectPos;
    float distance;

    public bool canHold = true;
    public Transform tempParent;
    public bool isHolding = false;
    bool updatedUi = false;
    [SerializeField] Transform interactionHint;
    Transform player;
    [SerializeField] int partIndex;
    Transform startParent;
    int startingLayer;
    private void Start()
    {
        start = false;
        EventController.OnGameStart += GameStart;
        if (!sShipHealth.shipHealth.gameOver)
        {
            GameStart();
        }
        startParent = transform.parent;
        startingLayer = gameObject.layer;
    }

    private bool start;

    public void GameStart()
    {
        Cursor.visible = false;
        start = true;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //interactionHint = transform.GetChild(0);
        tempParent = Camera.main.transform.GetChild(0).transform;
        EventController.OnGravityEnabled += GravityEnabled;
        EventController.OnGravityDisabled += GravityDisabled;
    }

    // Update is called once per frame
    void Update()
    {
        if (!sShipHealth.shipHealth.gameOver && start)
        {
            interactionHint.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            interactionHint.LookAt(player.position);
            //checking distance of object from parent drop if to far
            distance = Vector3.Distance(transform.position, tempParent.transform.position);
            if (tempParent.childCount == 0)
            {
                if (distance <= 3f && gameObject.GetComponent<Renderer>().isVisible)
                {
                    if (!isHolding)
                    {
                        //if (!updatedUi)
                        //{
                        //    sUIManager.instance.InteractionTurnOn(true, "pick up");
                        //    updatedUi = true;
                        //}
                        if (sUIManager.instance.xboxInputs)
                        {
                            sUIManager.instance.InteractionTurnOn(true, "pick up", sUIManager.instance.xButton);
                        }
                        else
                        {
                            sUIManager.instance.InteractionTurnOn(true, "pick up", "'F'");
                        }
                        updatedUi = true;
                    }
                    else
                    {
                        //if (!updatedUi)
                        //{
                        if (sUIManager.instance.xboxInputs)
                        {
                            sUIManager.instance.InteractionTurnOn(true, "throw", sUIManager.instance.bButton);
                        }
                        else
                        {
                            sUIManager.instance.InteractionTurnOn(true, "throw", "'E'");
                        }
                            
                        updatedUi = true;
                        //}
                    }
                    if (Input.GetButtonDown("Interact"))
                    {
                        if(partIndex < 4)
                        {
                            sPlayer._player.parts[partIndex] = true;
                        }
                        gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                        gameObject.GetComponent<AudioSource>().Play();
                        isHolding = true;
                        sPlayer._player.pickedUpThisFrame = true;
                        updatedUi = false;
                        GetComponent<Rigidbody>().detectCollisions = true;
                    }
                }
                else
                {
                    if (updatedUi)
                    {
                       sUIManager.instance.InteractionTurnOn(false, "throw", "");
                    }
                    updatedUi = false;
                }

                //check isHolding

            }
            if(isHolding == true)
            {
                sUIManager.instance.InteractionTurnOn(false, "pick up", "");
                if (gameObject.layer != 8)
                {
                    gameObject.layer = 8;
                }
                if (!sPlayer._player.brokenPartNearby)
                {
                    sUIManager.instance.InteractionTurnOn(false, "", "");
                    if (sUIManager.instance.xboxInputs)
                    {
                        sUIManager.instance.InteractionTurnOn(true, "throw", sUIManager.instance.bButton);
                    }
                    else
                    {
                        sUIManager.instance.InteractionTurnOn(true, "throw", "'E'");
                    }
                }
                else if (partIndex != 4)
                {
                    if (sUIManager.instance.xboxInputs)
                    {
                        sUIManager.instance.InteractionTurnOn(true, "fix", sUIManager.instance.xButton);
                    }
                    else
                    {
                        sUIManager.instance.InteractionTurnOn(true, "fix", "'F'");
                    }
                }
                interactionHint.gameObject.SetActive(false);
                GetComponent<MeshCollider>().enabled = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                transform.SetParent(tempParent.transform);
                transform.position = tempParent.transform.position;
                
                if (Physics.Raycast(transform.position, Vector3.down))
                {
                    if (Input.GetButtonDown("Throw"))
                    { // The player wants to throw the object in their hands
                        if (partIndex < 4)
                        { 
                            sPlayer._player.parts[partIndex] = false;
                        }
                        // Throw it
                        if (sUIManager.instance.xboxInputs)
                        {
                            sUIManager.instance.InteractionTurnOn(true, "throw", sUIManager.instance.bButton);
                        }
                        else
                        {
                            sUIManager.instance.InteractionTurnOn(true, "throw", "'E'");
                        }
                        GetComponent<Rigidbody>().AddForce(tempParent.transform.forward * throwForce);
                        // Add some random spin to the object to make it more fun
                        GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(100, 200),
                            Random.Range(100, 200), Random.Range(100, 200)));
                        isHolding = false;
                    }
                }
                else // The ray does not collide with anything, hide all interact hints
                {    
                    if (sUIManager.instance.xboxInputs)
                    {
                        sUIManager.instance.InteractionTurnOn(false, "throw", sUIManager.instance.bButton);
                    }
                    else
                    {
                        sUIManager.instance.InteractionTurnOn(false, "throw", "'E'");
                    }
                }
            }
            else
            {
                interactionHint.gameObject.SetActive(true);
                GetComponent<MeshCollider>().enabled = true;
                transform.SetParent(startParent);
                gameObject.layer = startingLayer;
            }
        }

    }

    public void GravityEnabled()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }
    public void GravityDisabled()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }
}
