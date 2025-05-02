using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
    GameObject pauseMenuObject;
    Button pauseButtonObject;
    Button continueButtonObject;
    Button nextDayButtonObject;
    Button mainMenuButtonObject;
    GameObject pointsPanelObject;
    GameObject backgroundObject;
    TMP_Text pointsText;
    TMP_Text dayText;
    TMP_Text gradeText;
    Animator gradeAnimator;
    public bool isPaused
    {
        get
        {
            return pauseMenuObject.activeSelf;
        }
        private set
        {
            pauseMenuObject.SetActive(value);
            backgroundObject.SetActive(value || dialogueHandler.showingDialogue);
            pauseButtonObject.gameObject.SetActive(!value);
            if (!value)
            {
                pauseMenuState = PauseMenuState.Unpaused;
            }
        }
    }
    public bool canPause = true;
    PauseMenuState _pauseMenuState = PauseMenuState.Unpaused;
    public PauseMenuState pauseMenuState
    {
        get
        {
            return _pauseMenuState;
        }
        set
        {
            _pauseMenuState = value;
            continueButtonObject.gameObject.SetActive(value == PauseMenuState.Paused);
            nextDayButtonObject.gameObject.SetActive(value == PauseMenuState.DayEnded);
            pointsPanelObject.SetActive(value == PauseMenuState.DayEnded);
            mainMenuButtonObject.gameObject.SetActive(true);
        }
    }

    public DialogueHandler dialogueHandler;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuObject = transform.Find("PausePanel").gameObject;
        pauseButtonObject = transform.Find("PauseButton").GetComponent<Button>();
        Button[] buttons = pauseMenuObject.GetComponentsInChildren<Button>();
        continueButtonObject = buttons.Where(b => b.gameObject.name == "Continue Button").First();
        nextDayButtonObject = buttons.Where(b => b.gameObject.name == "Next Day Button").First();
        mainMenuButtonObject = buttons.Where(b => b.gameObject.name == "Main Menu Button").First();
        pointsPanelObject = pauseMenuObject.transform.Find("PointsPanel").gameObject;
        pointsText = pointsPanelObject.transform.Find("Points").GetComponent<TMP_Text>();
        dayText = pointsPanelObject.transform.Find("Day").GetComponent<TMP_Text>();
        gradeText = pointsPanelObject.transform.Find("Grade").GetComponent<TMP_Text>();
        gradeAnimator = pointsPanelObject.transform.Find("Grade").GetComponent<Animator>();
        backgroundObject = transform.parent.Find("UI Background").gameObject;
        isPaused = false;
        pauseButtonObject.onClick.AddListener(TogglePause);
        continueButtonObject.onClick.AddListener(ContinueGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (canPause && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void PauseGame()
    {
        if (!canPause) { throw new Exception("Cannot pause game"); }
        isPaused = true;
        pauseMenuState = PauseMenuState.Paused;
    }

    public void ContinueGame()
    {
        if (!canPause) { throw new Exception("Cannot continue game"); }
        isPaused = false;
        pauseMenuState = PauseMenuState.Unpaused;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void DayEnded(string day, int points, Grade grade)
    {
        canPause = false;
        isPaused = true;
        pauseMenuState = PauseMenuState.DayEnded;
        pointsText.text = "Points: " + points.ToString();
        dayText.text = "Day: " + day.ToLower().FirstCharacterToUpper();
        gradeText.text = grade.ToString().ToUpper();
        gradeAnimator.SetBool("ShowGrade", true);
    }
}

public enum PauseMenuState
{
    Paused,
    Unpaused,
    DayEnded
}
