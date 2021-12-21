using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlayer : MonoBehaviour
{
    public static sPlayer _player;
    public bool brokenPartNearby;
    [SerializeField] private sBrokenPart brokenPartRef;
    private float heldDownTime;
    [SerializeField] private float fixTime;
    public bool[] parts = { false, false, false, false };
    [SerializeField] private bool nearAirLockConsole;
    [SerializeField] private bool nearAntiGravConsole;
    private sDoor consoleDoorRef;
    public bool pickedUpThisFrame;
    private float lowGrav;
    private float lowJumpSped;
    [SerializeField] float highGrav;

    void Start()
    {
        _player = this;
    }
    private IEnumerator _OpenAirLock;
    // Update is called once per frame
    void Update()
    {
        if (!sShipHealth.shipHealth.gameOver)
        { 
            if (brokenPartNearby)
            { 
                if (Input.GetButton("Interact"))
                {
                    if (parts[brokenPartRef.neededPartIndex])
                    { 
                        heldDownTime += Time.deltaTime;
                        sUIManager.instance.UpdateFixingValues(true, heldDownTime / fixTime);
                        if (heldDownTime >= fixTime)
                        {
                            parts[brokenPartRef.neededPartIndex] = false;
                            brokenPartRef.Fixed();
                            if (sUIManager.instance.xboxInputs)
                            {
                                sUIManager.instance.InteractionTurnOn(false, "fix", "");
                            }
                            else
                            {
                                sUIManager.instance.InteractionTurnOn(false, "fix", "'E'");
                            }
                            brokenPartNearby = false;
                        }
                    }
                }
                else
                {
                    heldDownTime = 0;
                    sUIManager.instance.UpdateFixingValues(false, heldDownTime / fixTime);
                }
            }
            else if (nearAirLockConsole)
            {
                if (Input.GetButtonDown("Interact") && _OpenAirLock is null)
                {
                    _OpenAirLock = OpenAirLockDoor();
                    StartCoroutine(_OpenAirLock);
                }
            }
            
        }
    }

    private void LateUpdate()
    {
        if (nearAntiGravConsole && !pickedUpThisFrame)
        {
            if (Input.GetButtonDown("Interact") && _AntiGravCoolDown is null)
            {
                _AntiGravCoolDown = AntiGravCoolDown();
                StartCoroutine(_AntiGravCoolDown);
            }
        }
        else
        {
            pickedUpThisFrame = false;
        }
    }

    private IEnumerator _AntiGravCoolDown;
    private bool toggleGrav = false;
    public bool cd = false;

    private IEnumerator AntiGravCoolDown()
    {
        cd = true;
        sUIManager.instance.InteractionTurnOn(false, "", "");
        toggleGrav = !toggleGrav;
        if (toggleGrav)
        {
            EventController.Event_OnGravityEnabled();
        }
        else
        {
            EventController.Event_OnGravityDisabled();
        }
        yield return new WaitForSeconds(1.5f);
        _AntiGravCoolDown = null;
        cd = false;
    }

    IEnumerator OpenAirLockDoor()
    { 
        sDoor temp = consoleDoorRef;
        temp.OpenDoor();
        yield return new WaitForSeconds(2f);
        temp.CloseDoor();
        _OpenAirLock = null;
    }

    public void InSpace(Vector3 doorDir)
    {
        gameObject.GetComponent<CharacterController>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().AddForce(doorDir * 3f);
        sShipHealth.shipHealth.PrematureEnd();
    }

    public void NearAirLockConsole(sDoor refer)
    {
        nearAirLockConsole = true;
        consoleDoorRef = refer;
    } 
    
    public void NearAntiGravConsole()
    {
        nearAntiGravConsole = true;
    } 
    
    public void NoLongerNearAirLockConsole()
    {
        nearAirLockConsole = false;
        consoleDoorRef = null;
    }
    
    public void NoLongerNearAntiGravConsole()
    {
        nearAntiGravConsole = false;
    }

    public void NearBrokenPart(sBrokenPart reference)
    {
        print("player near broken part");
        brokenPartRef = reference;
        brokenPartNearby = true;
    }

    public void NoLongerNearBrokenPart()
    {
        brokenPartRef = null;
        brokenPartNearby = false;
    }

}
