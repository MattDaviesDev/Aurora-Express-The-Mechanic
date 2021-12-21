using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSparksound : MonoBehaviour
{
    public AudioSource playSound;
    
    float t = 0f;

    Coroutine sparkNoise;

    void Start()
    {
        
    }

    IEnumerator SparkNoise()
    {
        do
        {
            playSound.pitch = playSound.pitch * Random.Range(0.8f, 1.2f);
            if (!playSound.isPlaying)
            {
                playSound.Play();

            }
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        } while (true);
    }

    private void OnEnable()
    {
        if (sparkNoise == null)
        {
            sparkNoise = StartCoroutine(SparkNoise());
        }
    }

    private void OnDisable()
    {
        if (sparkNoise != null)
        {
            StopCoroutine(sparkNoise);
            sparkNoise = null;
        }
    }

}

  
