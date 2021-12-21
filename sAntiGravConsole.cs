using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sAntiGravConsole : MonoBehaviour
{
    [SerializeField] private Vector3 boxCastDimensions;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private string interactionText;
    private bool wasNear;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!sPlayer._player.cd)
        { 
            if (Physics.OverlapBox(transform.position, boxCastDimensions, Quaternion.identity, playerMask).Length > 0)
            {

                sPlayer._player.NearAntiGravConsole();
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
                sPlayer._player.NoLongerNearAntiGravConsole();
                if (sUIManager.instance.xboxInputs)
                {
                    sUIManager.instance.InteractionTurnOn(false, interactionText, sUIManager.instance.xButton);
                }
                else
                {
                    sUIManager.instance.InteractionTurnOn(false, interactionText, "'F'");
                }
            }
        }
    }
}
