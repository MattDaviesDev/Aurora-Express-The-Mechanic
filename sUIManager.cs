using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sUIManager : MonoBehaviour
{

    public static sUIManager instance;

    [SerializeField] EventSystem sceneEventSystem;
    [SerializeField] public bool xboxInputs = false;

    [Header("Interaction")]
    [SerializeField] GameObject interactionObject;
    [SerializeField] TextMeshProUGUI interactionText;

    [Header("Health")]
    [SerializeField] GameObject healthObject;
    [SerializeField] Image healthImage;
    [SerializeField] TextMeshProUGUI healthText;

    [Header("Part broken")]
    [SerializeField] GameObject brokenPartObject;
    [SerializeField] Image brokenPartImage;
    [SerializeField] TextMeshProUGUI brokenPartText;
    [SerializeField] float flashTime;
    [SerializeField] Color flashStartColor;
    [SerializeField] Color flashEndColor;
    Coroutine brokenPartFlasher;
    Coroutine brokenPartScaler;

    [Header("Fixing")]
    [SerializeField] GameObject fixingObject;
    [SerializeField] Image fixingAmount;
    [SerializeField] TextMeshProUGUI fixingPercent;

    [Header("Repairs On Screen")]
    [SerializeField] GameObject repairsObject;
    [SerializeField] TextMeshProUGUI repairsMadeOnScreen;

    [Header("End Screen")]
    [SerializeField] RectTransform shipObject;
    [SerializeField] RectTransform startPos;
    [SerializeField] RectTransform endPos;
    [SerializeField] TextMeshProUGUI distanceCovered;
    [SerializeField] TextMeshProUGUI repairsMade;
    [SerializeField] TextMeshProUGUI endText;
    [SerializeField] GameObject rulerObject;
    [SerializeField] TextMeshProUGUI quarterDistanceText;
    [SerializeField] TextMeshProUGUI halfDistanceText;
    [SerializeField] TextMeshProUGUI threeQuarterDistanceText;
    [SerializeField] TextMeshProUGUI maxDistanceText;
    [SerializeField] TextMeshProUGUI timeSurvivedText;
    [SerializeField] Camera EndGameCamera;
    [SerializeField] Camera PlayerCamera;
    Coroutine moveShip;
    Coroutine blendCanvases;
    public int numberOfRepairs = 0;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI arrivalTimeText;

    [Header("Game HUDs")]
    [SerializeField] GameObject mainHUD;
    [SerializeField] GameObject pauseHUD;
    [SerializeField] GameObject endHUD;
    [SerializeField] GameObject[] allHUDS;

    [Header("Main Menu")]
    [SerializeField] GameObject firstScreen;
    [SerializeField] GameObject secondScreen;
    [SerializeField] TextMeshProUGUI difficultyDescText;
    [SerializeField] TextMeshProUGUI inputTypeText;
    [SerializeField] Image inputTypeImage;
    [SerializeField] string switchToXbox;
    [SerializeField] string switchToKAM;
    [SerializeField] Sprite xboxSprite;
    [SerializeField] Sprite keyboardAndMouseSprite;
    [SerializeField] GameObject PlayBtn;
    [SerializeField] GameObject easyDiffBtn;
    public int selectedDifficulty;
    public bool gameStarted = false;

    [Header("Player")]
    [SerializeField] sMouseCamControl cameraControl;
    [SerializeField] sPlayerController playerController;

    [Header("Pause")]
    [SerializeField] GameObject resumeBtn;
    [SerializeField] Image pauseInputTypeImage;
    [SerializeField] TextMeshProUGUI pauseInputTypeText;

    [Header("TMP Sprite texts")]
    [SerializeField] public string xButton;
    [SerializeField] public string bButton;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        gameStarted = false;
        cameraControl.enabled = false;
        playerController.enabled = false;
        ToggleinputChange();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstScreen.activeInHierarchy || pauseHUD.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return) && xboxInputs)
            {
                xboxInputs = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                sceneEventSystem.SetSelectedGameObject(null);
                ToggleinputChange();
            }
            else if (Input.GetButtonDown("X") && !xboxInputs)
            {
                xboxInputs = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (firstScreen.activeInHierarchy)
                {
                    sceneEventSystem.SetSelectedGameObject(PlayBtn);
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    sceneEventSystem.SetSelectedGameObject(resumeBtn);
                }
                ToggleinputChange();
            }
        }
        if (secondScreen.activeInHierarchy)
        {
            if (sceneEventSystem.currentSelectedGameObject != null && sceneEventSystem.currentSelectedGameObject.transform.childCount > 1)
            {
                DifficultyHighlighted(sceneEventSystem.currentSelectedGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
            }
        }
        if (Input.GetButtonDown("Pause") && mainHUD.activeInHierarchy)
        {
            TogglePause();
        }
    }

    public bool paused;

    public void TogglePause()
    {
        if (paused)
        {
            cameraControl.enabled = true;
            playerController.enabled = true;
            Time.timeScale = 1f;
            paused = false;
            HideHUDs(mainHUD);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseHUD.SetActive(false);
        }
        else
        {
            cameraControl.enabled = false;
            playerController.enabled = false;
            UpdateRepairValues();
            Time.timeScale = 0f;
            paused = true;
            HideHUDs(pauseHUD);
            if (!xboxInputs)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                sceneEventSystem.SetSelectedGameObject(resumeBtn);
            }
        }
    }

    public void InteractionTurnOn(bool On, string interactiontext, string key)
    {
        interactionText.text = key + " to " + interactiontext;
        interactionObject.SetActive(On);
    }
    //public void InteractionTurnOn(bool On, string interactiontext)
    //{
    //    interactionText.text = "'F' to " + interactiontext;
    //    interactionObject.SetActive(On);
    //}

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthText.text = currentHealth + "/" + maxHealth;
        healthImage.fillAmount = currentHealth / maxHealth;
    }

    public void UpdateRepairValues()
    {
        numberOfRepairs++;
        repairsMadeOnScreen.text = numberOfRepairs.ToString();
    }

    public void UpdateFixingValues(bool visible, float t)
    {
        fixingObject.SetActive(visible);
        fixingAmount.fillAmount = t;
        fixingPercent.text = (int)Mathf.Lerp(0f, 100f, t) + "%";
    }

    public void UpdateTimerValues(float currentTime, float maxTime)
    {
        if (maxTime == 0)
        {
            arrivalTimeText.text = "Time survived: " + ConvertTimeToString(currentTime);
        }
        else
        { 
            arrivalTimeText.text = "Time to arrival: " + ConvertTimeToString(maxTime - currentTime);       
        }
    }

    public string ConvertTimeToString(float f)
    {
        string returnString = "";
        int hours = 0;
        string hoursText;
        int mins = 0;
        string minsText;
        int secs = 0;
        string secsText;
        if (f >= 60)
        {
            mins = (int)(f / 60); // this is the syntax to cast a variable to a different type in C#, seeing as timer is a float casting it to an int truncates the value / 60 
            if (mins >= 60)
            {
                hours = (int)(mins / 60);
                mins = (int)(mins - (hours / 60));
            }
        }
        if (hours > 0)
        {
            secs = (int)(f - ((mins * hours * 60)));
        }
        else
        {
            secs = (int)(f - (mins * 60));
        }
        if (hours < 10)
        {
            hoursText = "0" + hours;
        }
        else
        {
            hoursText = hours.ToString();
        }
        if (mins < 10)
        {
            minsText = "0" + mins;
        }
        else
        {
            minsText = mins.ToString();
        }
        if (secs < 10)
        {
            secsText = "0" + secs;
        }
        else
        {
            secsText = secs.ToString();
        } // although all of this looks super long-winded, I'm pretty sure its all necessary to get the right strings of text
        if (sShipHealth.shipHealth.maxDistance != 0)
        {
            returnString = minsText + ":" + secsText;
        }
        else
        {
            returnString = hoursText + ":" + minsText + ":" + secsText;
        }
        return returnString;
    }

    public void MachineBroken(string name, string reason, string shipPosition)
    {
        brokenPartText.text = "A " + name + " " + reason + " in " + shipPosition + "!";
        brokenPartScaler = StartCoroutine(ScaleMachineBroken());
    }

    IEnumerator ScaleMachineBroken() 
    {
        brokenPartObject.SetActive(true);
        brokenPartFlasher = StartCoroutine(FlashMachineColor());
        float t = 0f;
        while (t <= 1f)
        {
            yield return null;
            t += Time.deltaTime / flashTime * 4;
            brokenPartObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
        }
        t = 0f;
        while (t <= 1f)
        {
            yield return null;
            t += Time.deltaTime / flashTime * 4;
            brokenPartObject.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.95f, 0.95f, 0.95f), t);
        }
        t = 0f;
        while (t <= 1f)
        {
            yield return null;
            t += Time.deltaTime / flashTime * 4;
            brokenPartObject.transform.localScale = Vector3.Lerp(new Vector3(0.95f, 0.95f, 0.95f), Vector3.one, t);
        }
        t = 0f;
        while (t <= 1f)
        {
            yield return null;
            t += Time.deltaTime / flashTime * 4;
            brokenPartObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
        }
        brokenPartObject.SetActive(false);
    }

    IEnumerator FlashMachineColor()
    {
        bool flash = false;
        while (true)
        {
            //brokenPartText.color = flash ? Color.white : Color.red;
            brokenPartText.color = flash ? flashEndColor : flashStartColor;
            flash = !flash;
            yield return new WaitForSeconds(flashTime / 10);
        }
    }

    IEnumerator LerpShipPosition(float percentDistanceCovered, float maxDistance)
    {
        NumberFormatInfo num = new NumberFormatInfo();
        num.NumberGroupSeparator = ",";
        float t = 0f;
        int number = 0;
        do
        {
            t += Time.deltaTime / 5f;
            number = (int)Mathf.Lerp(0, maxDistance, t);
            distanceCovered.text = number.ToString("N0", num) 
                + "km/" + sShipHealth.shipHealth.maxDistance.ToString("N0", num) + "km";
            shipObject.position = Vector2.Lerp(startPos.position, endPos.position, t);
            yield return null;
        } while (t <= percentDistanceCovered);

    }

    IEnumerator BlendCanvas(float percentDistanceCovered, float maxDistance)
    {
        CanvasGroup endScreenGroup = endHUD.GetComponent<CanvasGroup>();
        CanvasGroup mainHUDGroup = mainHUD.GetComponent<CanvasGroup>();
        float t = 0f;
        endScreenGroup.alpha = 0f;
        endHUD.SetActive(true);
        do
        {
            t += Time.deltaTime / 5f;
            endScreenGroup.alpha = Mathf.Lerp(0, 1, t);
            mainHUDGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        } while (t <= 1f);
        mainHUD.SetActive(false);
        if (moveShip == null)
        {
            moveShip = StartCoroutine(LerpShipPosition(percentDistanceCovered, maxDistance));
        }
    }

    public void EndGame(bool win, float percentDistanceCovered, float maxDistance)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainHUD.SetActive(false);
        EndGameCamera.enabled = true;
        PlayerCamera.enabled = false;
        PlayerCamera.transform.GetChild(1).GetComponent<Camera>().enabled = false;
        playerController.enabled = false;
        cameraControl.enabled = false;
        if (win)
        {
            endText.text = "Congratulations on a successful journey!";
        }
        else
        {
            endText.text = "The journey proved to much for a meer mechanic...";
        }
        repairsMade.text = "Successful repairs: " + numberOfRepairs;
        if (blendCanvases == null)
        {
            blendCanvases = StartCoroutine(BlendCanvas(percentDistanceCovered, maxDistance));
        }
        
    }


    void HideHUDs(GameObject newHUD)
    {
        for (int i = 0; i < allHUDS.Length; i++)
        {
            if (allHUDS[i] != newHUD)
            {
                allHUDS[i].SetActive(false);
            }
        }
        newHUD.SetActive(true);
    }

    public void EndBTNPressed(bool replay)
    {
        if (replay)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        else
        {
            Application.Quit();
        }
    }

    public void MainMenuBTNPressed(bool play)
    {
        if (play)
        {
            firstScreen.SetActive(false);
            secondScreen.SetActive(true);
            sceneEventSystem.SetSelectedGameObject(null);
            if (xboxInputs)
            {
                sceneEventSystem.SetSelectedGameObject(easyDiffBtn);
            }
        }
        else
        {
            Application.Quit();
        }
    }

    public void DifficultyHighlighted(string difficultyText)
    {
        difficultyDescText.text = difficultyText;
    }
    
    public void DifficultyBTNPressed(int difficultyIndex)
    {
        selectedDifficulty = difficultyIndex;
        print(selectedDifficulty);
        PlayerCamera.enabled = true;
        EndGameCamera.enabled = false;
        HideHUDs(mainHUD);
        sShipHealth.shipHealth.gameOver = false;
        EventController.Event_OnGameStart();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameStarted = true;
        cameraControl.enabled = true;
        playerController.enabled = true;
        switch (difficultyIndex)
        {
            case 0: 
                sShipHealth.shipHealth.maxDistance = sShipHealth.shipHealth.maxDistances[0];
                break;
            case 1:
                sShipHealth.shipHealth.maxDistance = sShipHealth.shipHealth.maxDistances[1];
                break;
            case 2:
                sShipHealth.shipHealth.maxDistance = sShipHealth.shipHealth.maxDistances[2];
                break;
            case 3:
                sShipHealth.shipHealth.maxDistance = 0f;
                break;
            default:
                break;
        }
        UpdateEndRulerText(sShipHealth.shipHealth.maxDistance);
        
    }

    public void UpdateEndRulerText(float maxDistance)
    {
        if (maxDistance == 0)
        {
            rulerObject.SetActive(false);
            timeSurvivedText.text = "Time survived: " + ConvertTimeToString(sShipHealth.shipHealth.timer);
            timeSurvivedText.gameObject.SetActive(true);
        }
        else
        {
            NumberFormatInfo num = new NumberFormatInfo();
            num.NumberGroupSeparator = ",";
            distanceCovered.text = 0 + "km/" + sShipHealth.shipHealth.maxDistance.ToString("N0", num) + "km";
            quarterDistanceText.text = (maxDistance / 4).ToString("N0", num) + "km";
            halfDistanceText.text = (maxDistance / 2).ToString("N0", num) + "km";
            threeQuarterDistanceText.text = ((maxDistance / 4) * 3).ToString("N0", num) + "km";
            maxDistanceText.text = maxDistance.ToString("N0", num) + "km";
        }

    }

    public void ToggleinputChange()
    {
        inputTypeImage.sprite = xboxInputs ? xboxSprite : keyboardAndMouseSprite;
        pauseInputTypeImage.sprite = xboxInputs ? xboxSprite : keyboardAndMouseSprite;
        inputTypeText.text = xboxInputs ? switchToKAM : switchToXbox;
        pauseInputTypeText.text = xboxInputs ? switchToKAM : switchToXbox;
        
    }

}
