using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sLocationManager : MonoBehaviour
{
    private bool[] inSector = { true, false, false, false, false, false };
    [SerializeField] private GameObject[] sector = new GameObject[6];
    [SerializeField] private string[] sectorName = new string[6];
    [SerializeField] private TextMeshProUGUI currentSectorText;
    private Color fadedColor;
    private Color unfadedColor;


    private void Awake()
    {
        fadedColor = currentSectorText.color;
        unfadedColor = currentSectorText.color;
        unfadedColor.a = 255;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < inSector.Length; i++)
        {
            Vector3 curSec = sector[GetCurrentSector()].transform.position;
            curSec.y = 0f;
            Vector3 checkSec = sector[i].transform.position;
            checkSec.y = 0f;
            if (Vector3.Distance(curSec, sPlayer._player.transform.position) > Vector3.Distance(checkSec, sPlayer._player.transform.position))
            {
                inSector[GetCurrentSector()] = false;
                inSector[i] = true;
                UpdateText();
            }
        }
    }
    private IEnumerator _FadeOut;

    private void UpdateText()
    {
        currentSectorText.text = sectorName[GetCurrentSector()];
        if (_FadeOut is null)
        {
            _FadeOut = FadeOut();
            StartCoroutine(_FadeOut);
        }
        else
        {
            StopCoroutine(_FadeOut);
            _FadeOut = FadeOut();
            StartCoroutine(_FadeOut);
        }
    }

    private IEnumerator FadeOut()
    {
        currentSectorText.color = unfadedColor;
        yield return new WaitForSeconds(1f);
        float t = 0;
        Color32 newColor = unfadedColor;
        while (t <= 1)
        {
            newColor = Color32.Lerp(unfadedColor, fadedColor, t);
            currentSectorText.color = newColor;
            t += Time.deltaTime;
            
            yield return null;
        }
        _FadeOut = null;
    }

    private int GetCurrentSector()
    {
        for (int i = 0; i < inSector.Length; i++)
        {
            if (inSector[i])
                return i;
        }
        return 0;
    }
}
