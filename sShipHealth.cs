using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sShipHealth : MonoBehaviour
{
    public static sShipHealth shipHealth;
    [SerializeField] private float health;
    private float maxHealth;
    [SerializeField] public float timer;
    public float timeLimit;
    [SerializeField] public float maxDistance;
    public bool gameOver;
    [SerializeField] public float[] maxTimers;
    [SerializeField] public float[] maxDistances;
    [SerializeField] private float[] maxHealths;
    // Start is called before the first frame update
    private void Awake()
    {
        shipHealth = this;
        timer = 0f;
    }
    void Start()
    {
        EventController.OnGameStart += GameStart;
    }

    public void GameStart()
    {
        timeLimit = maxTimers[sUIManager.instance.selectedDifficulty];
        health = maxHealths[sUIManager.instance.selectedDifficulty];
        maxHealth = health;
        sUIManager.instance.UpdateHealthBar(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (sUIManager.instance.gameStarted)
        {
            if (health > 0)
            {
                timer += Time.deltaTime;
            }
            if (timeLimit == 0)
            {
                sUIManager.instance.UpdateTimerValues(timer, 0);
            }
            else
            { 
                sUIManager.instance.UpdateTimerValues(timer, timeLimit);
                if (health > 0)
                {
                    if (timer >= timeLimit)
                    {
                        sUIManager.instance.EndGame(true, 100, maxDistance);
                    }
                }
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        sUIManager.instance.UpdateHealthBar(health, maxHealth);

        if (health <= maxHealth / 4)
        {
            sParticleSystemManager._pManager.StartShipHealthLow();
        }

        if (health <= 0)
        {
            sUIManager.instance.EndGame(false, timer / timeLimit, maxDistance);
            gameOver = true;
        }
    }

    public void PrematureEnd()
    {
        sUIManager.instance.EndGame(false, timer / timeLimit, maxDistance);
        gameOver = true;
    }
}
