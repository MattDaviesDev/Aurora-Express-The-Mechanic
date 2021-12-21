using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sParticleSystemManager : MonoBehaviour
{
    public static sParticleSystemManager _pManager;
    [SerializeField] private GameObject[] ShipCoreParticles;
    private ParticleSystemRenderer rend;
    [SerializeField] private Color color;
    // Start is called before the first frame update
    void Start()
    {
        _pManager = this;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator _ShipHealthLow;

    public void StartShipHealthLow()
    {
        if (_ShipHealthLow is null)
        {
            _ShipHealthLow = ShipHealthLow();
            StartCoroutine(_ShipHealthLow);
        }
    }

    private IEnumerator ShipHealthLow()
    {
        float t = 0;

        while (t <= 2)
        {
            t += Time.deltaTime;
            foreach (var item in ShipCoreParticles)
            {
                rend = item.GetComponent<ParticleSystemRenderer>();
                Material temp = rend.material;
                Color tempcol = Color.Lerp(temp.GetColor("_TintColor"), color, t / 2);
                tempcol.a = 147;
                temp.SetColor("_TintColor", tempcol);
                rend.material = temp;

            }
            yield return null;
        }
    }
}
