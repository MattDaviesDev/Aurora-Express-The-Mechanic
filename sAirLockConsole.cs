using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sAirLockConsole : MonoBehaviour
{
    [SerializeField] private string interactionText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] private Vector3 boxCastDimensions;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private sDoor connectedDoor;
    private bool wasNear;

    // Update is called once per frame
    void Update()
    {
        if (Physics.OverlapBox(transform.position, boxCastDimensions, Quaternion.identity, playerMask).Length > 0)
        {
            
            sPlayer._player.NearAirLockConsole(connectedDoor);
            if (sUIManager.instance.xboxInputs)
            {
                sUIManager.instance.InteractionTurnOn(true, interactionText, sUIManager.instance.xButton);
            }
            else
            {
                sUIManager.instance.InteractionTurnOn(true, interactionText, "'F'");
            }
            wasNear = true;
        }
        else if (wasNear)
        {
            wasNear = false;
            sPlayer._player.NoLongerNearAirLockConsole();
            sUIManager.instance.InteractionTurnOn(false, interactionText, "");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, boxCastDimensions);
    }
}
