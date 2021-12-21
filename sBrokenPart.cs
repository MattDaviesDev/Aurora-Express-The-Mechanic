using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sBrokenPart : MonoBehaviour
{
    public bool broken;
    [SerializeField] private float[] dmgFreqLevels;
    [SerializeField] private float[] dmgIncreaseLevels;
    [SerializeField] private float[] dmgIncreaseTimeLevels;
    [SerializeField] private float increaseDamageTime;
    [SerializeField] private float damage;
    [SerializeField] private float standardDamage;
    [SerializeField] private float damageIncrease;
    [SerializeField] private float damageFrequency;
    private IEnumerator _CurrentlyBroken;
    private IEnumerator _DamageOverTime;
    [SerializeField] private Vector3 boxCastDimensions;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private bool playerNear;
    private Vector3 boxcastCenter;
    [SerializeField] private bool adjustCenter;
    public int neededPartIndex;
    GameObject brokenImage;
    Transform player;
    [SerializeField] string myName = "";
    [SerializeField] string myReason = "";
    [SerializeField] string shipArea = "";
    Transform fixingObject;
    [SerializeField] Transform endPosition;
    [SerializeField] GameObject sparkEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        EventController.OnGameStart += GameStart;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        brokenImage = transform.GetChild(0).gameObject;
        standardDamage = damage;
        boxcastCenter = transform.position;
        if (adjustCenter)
        { 
            boxcastCenter.y += boxCastDimensions.y / 2;
        }
    }

    private void GameStart()
    {
        increaseDamageTime = dmgIncreaseTimeLevels[sUIManager.instance.selectedDifficulty];
        damageIncrease = dmgIncreaseLevels[sUIManager.instance.selectedDifficulty];
        damageFrequency = dmgFreqLevels[sUIManager.instance.selectedDifficulty];
    }

    private void OnDrawGizmos()
    {
        boxcastCenter = transform.position;
        if (adjustCenter)
        {
            boxcastCenter.y += boxCastDimensions.y / 2;
        }
        Gizmos.DrawWireCube(boxcastCenter, boxCastDimensions);
    }

    // Update is called once per frame
    void Update()
    {
        if (broken)
        {
            brokenImage.SetActive(true);
            brokenImage.transform.LookAt(player.position);
            if (Physics.OverlapBox(boxcastCenter, boxCastDimensions, Quaternion.identity, playerMask).Length > 0)
            {
                playerNear = true;
                
                if (Physics.OverlapBox(boxcastCenter, boxCastDimensions, Quaternion.identity, playerMask)[0].transform.GetChild(0).GetChild(0).childCount > 0)
                {
                    fixingObject = Physics.OverlapBox(boxcastCenter, boxCastDimensions, Quaternion.identity, playerMask)[0].transform.GetChild(0).GetChild(0).GetChild(0);
                }
                sPlayer._player.NearBrokenPart(this);
            }
            else if (playerNear)
            {
                
                playerNear = false;
                fixingObject = null;
                sPlayer._player.NoLongerNearBrokenPart();
            }
        }
        else
        {
            brokenImage.SetActive(false);
        }
    }
    [Header("Camera Shake Values")]
    [SerializeField] private float magnitude;
    [SerializeField] private float rougness;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;
    public void Break()
    {
        Debug.Log("break");
        sparkEffect.SetActive(true);
        if (neededPartIndex == 3)
        {
            print("Shake Once");
            EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.Explosion);
            gameObject.GetComponent<AudioSource>().Play();
        }
        sSparePartSpawn.instance.SpawnSpareParts(neededPartIndex);
        broken = true;
        sUIManager.instance.MachineBroken(myName, myReason, shipArea);
        if (_CurrentlyBroken is null)
        {
            print("start routines");
            _CurrentlyBroken = CurrentlyBroken();
            _DamageOverTime = DamageOverTime();
            StartCoroutine(_CurrentlyBroken);
            StartCoroutine(_DamageOverTime);
        }
    }

    private IEnumerator CurrentlyBroken()
    {
        while (broken)
        {
            yield return new WaitForSeconds(increaseDamageTime);
            damage += damageIncrease;
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (broken)
        {
            yield return new WaitForSeconds(damageFrequency);
            sShipHealth.shipHealth.TakeDamage(damage);
        }
    }

    public void Fixed()
    {
        sparkEffect.SetActive(false);
        StopAllCoroutines();
        _CurrentlyBroken = null;
        _DamageOverTime = null;
        sUIManager.instance.UpdateFixingValues(false, 0);
        broken = false;
        damage = standardDamage;
        StartCoroutine(LerpObjectIntoMachine());
        sUIManager.instance.UpdateRepairValues();
    }

    IEnumerator LerpObjectIntoMachine()
    {
        float t = 0f;
        if (fixingObject != null)
        {
            Vector3 startingPos = fixingObject.position;
            Vector3 startingScale = fixingObject.localScale;
            fixingObject.parent = null;
            Destroy(fixingObject.GetComponent<MeshCollider>());
            fixingObject.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(100, 200),
                Random.Range(100, 200), Random.Range(100, 200)));
            fixingObject.GetComponent<sPickUpItem>().enabled = false;
            do
            {
                t += Time.deltaTime / 2f;
                if (fixingObject != null)
                {
                    fixingObject.position = Vector3.Lerp(startingPos, endPosition.position, t);
                    fixingObject.localScale = Vector3.Lerp(startingScale, Vector3.zero, t);
                }
                yield return null;
            } while (t <= 1f);
            Destroy(fixingObject.gameObject);
        }
    }
}
