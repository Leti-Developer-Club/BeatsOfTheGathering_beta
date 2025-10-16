using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // sfx //
    public AudioSource theMusic;
    [SerializeField] public AudioClip crowdCheer;
    [SerializeField] private AudioClip goodHitSound;
    [SerializeField] private AudioClip perfectHitSound;
    [SerializeField] private AudioClip missHitSound;
    [SerializeField] private AudioClip wrongHitSound; // For wrong button press
    [SerializeField] private ParticleSystem perfectHitParticles;
    public bool startPlaying;
    public BeatScroller beatScroller;

    // Score variables
    public int currentScore;
    public int currentMultiplier = 1;
    public int currentStreak = 0;
    public int maxStreak = 0;
    public int scorePerGoodNote = 50;
    public int scorePerPerfectNote = 100;
    public Text scoreText;
    public Text multiplierText;

    // Timing windows (in seconds, per GDD)
    public float perfectTimingWindow = 0.03f;
    public float goodTimingWindow = 0.08f;

    // Results Screen Stats
    public float goodHits;
    public float perfectHits;
    public float missedHits;

    public GameObject resultsScreen;
    public Text percentHitText;
    public Text normalHitText;
    public Text goodHitText;
    public Text perfectHitText;
    public Text missedHitText;
    public Text rankText;
    public Text finalScoreText;
    public Text maxStreakText;

    // Celebration Meter
    public Slider celebrationSlider;

    // Game over flag
    private bool isGameOver = false;

    // Active notes per lane
    private List<NoteObject>[] activeNotesPerLane;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Initialize active notes lists (assuming we're still going with 4 lanes)
        activeNotesPerLane = new List<NoteObject>[4];
        for (int i = 0; i < 4; i++)
        {
            activeNotesPerLane[i] = new List<NoteObject>();
        }
    }

    private void Start()
    {
        currentScore = 0;
        scoreText.text = "Score: " + currentScore;
        multiplierText.text = "Multiplier: x" + currentMultiplier;

        // Celebration meter start (GDD: half full)
        celebrationSlider.value = 0.5f;

        GameObject activator = GameObject.FindWithTag("Activator");
        if (activator != null)
        {
            BoxCollider col = activator.GetComponent<BoxCollider>();
            if (col != null)
            {
                float speed = beatScroller.beatTempo;
                float fullWindow = goodTimingWindow * 2f;
                col.size = new Vector3(col.size.x, speed * fullWindow, col.size.z);
                col.isTrigger = true;
            }
        }
    }

    private void Update()
    {
        if (celebrationSlider == null || maxStreakText == null) return;

        celebrationSlider.value = Mathf.Clamp(celebrationSlider.value, 0f, 1f);

        if (!startPlaying)
        {
            if (Input.anyKeyDown || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                startPlaying = true;
                beatScroller.hasStarted = true;
                theMusic.Play();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A)) PressLane(0);
            if (Input.GetKeyDown(KeyCode.S)) PressLane(1);
            if (Input.GetKeyDown(KeyCode.K)) PressLane(2);
            if (Input.GetKeyDown(KeyCode.L)) PressLane(3);

            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        float screenWidth = Screen.width;
                        float laneWidth = screenWidth / 4f;
                        float touchX = touch.position.x;

                        int lane = Mathf.FloorToInt(touchX / laneWidth);
                        lane = Mathf.Clamp(lane, 0, 3);
                    }
                }
            }

            if (!isGameOver && celebrationSlider.value <= 0)
            {
                isGameOver = true;
                theMusic.Stop();
                StopNotes(); // Stop all notes when meter reaches 0
                ShowResultsScreen();
            }

            if (!theMusic.isPlaying && !resultsScreen.activeInHierarchy && !isGameOver)
            {
                ShowResultsScreen();
            }
        }
    }

    private void StopNotes()
    {
        beatScroller.hasStarted = false;
        foreach (List<NoteObject> laneNotes in activeNotesPerLane)
        {
            foreach (NoteObject note in laneNotes)
            {
                if (note != null && note.gameObject.activeSelf)
                {
                    note.gameObject.SetActive(false);
                }
            }
            laneNotes.Clear();
        }
    }

    private void ShowResultsScreen()
    {
        resultsScreen.SetActive(true);
        normalHitText.text = "";
        goodHitText.text = goodHits.ToString();
        perfectHitText.text = perfectHits.ToString();
        missedHitText.text = missedHits.ToString();
        maxStreakText.text = "Max Streak: " + maxStreak;

        float totalHit = goodHits + perfectHits;
        float totalNotes = totalHit + missedHits;
        float percentHit = (totalNotes > 0) ? (totalHit / totalNotes) * 100f : 0f;
        percentHitText.text = Mathf.Min(percentHit, 100f).ToString("F1") + "%";

        string rankVal = isGameOver ? "F" : "C"; // "F" on game over, otherwise base rank
        if (!isGameOver)
        {
            if (percentHit >= 50) rankVal = "B";
            if (percentHit >= 90) rankVal = "A+";
        }

        rankText.text = rankVal;
        finalScoreText.text = currentScore.ToString();
    }

    public void NoteHit()
    {
        Debug.Log("Hit on time");
        currentStreak++;
        if (currentStreak > maxStreak) maxStreak = currentStreak;
        UpdateMultiplier();
        scoreText.text = "Score: " + currentScore;
    }

    private void UpdateMultiplier()
    {
        if (currentStreak >= 30) currentMultiplier = 4;
        else if (currentStreak >= 15) currentMultiplier = 3;
        else if (currentStreak >= 5) currentMultiplier = 2;
        else currentMultiplier = 1;
        multiplierText.text = "Multiplier: x" + currentMultiplier;
    }

    public void GoodHit(Vector3 pos)
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        goodHits++;
        theMusic.PlayOneShot(goodHitSound, 0.7f);
        NoteHit();
        celebrationSlider.value += 0.02f;
    }

    public void PerfectHit(Vector3 pos)
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        perfectHits++;
        Instantiate(perfectHitParticles, pos, perfectHitParticles.transform.rotation);
        theMusic.PlayOneShot(perfectHitSound, 0.7f);
        theMusic.PlayOneShot(crowdCheer, 0.7f);
        NoteHit();
        celebrationSlider.value += 0.05f;
    }

    public void NoteMissed()
    {
        missedHits++;
        currentStreak = 0;
        UpdateMultiplier();
        theMusic.PlayOneShot(missHitSound, 0.7f);
        celebrationSlider.value -= 0.07f;
    }

    public void WrongButtonPress()
    {
        currentStreak = 0;
        UpdateMultiplier();
        theMusic.PlayOneShot(wrongHitSound, 0.7f);
        if (celebrationSlider.value > 0)
        {
            celebrationSlider.value -= 0.03f;
        }
    }

    public void AddActiveNote(int lane, NoteObject note)
    {
        activeNotesPerLane[lane].Add(note);
    }

    public void RemoveActiveNote(int lane, NoteObject note)
    {
        activeNotesPerLane[lane].Remove(note);
    }

    public void PressLane(int lane)
    {
        if (activeNotesPerLane[lane].Count == 0)
        {
            WrongButtonPress();
            return;
        }

        activeNotesPerLane[lane].Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
        NoteObject note = activeNotesPerLane[lane][0];
        float speed = beatScroller.beatTempo;
        float errorTime = Mathf.Abs(note.transform.position.y) / speed;

        RemoveActiveNote(lane, note);
        note.gameObject.SetActive(false);

        if (errorTime <= perfectTimingWindow)
        {
            Instantiate(note.perfectHitEffect, note.transform.position, note.transform.rotation);
            PerfectHit(note.transform.position);
        }
        else
        {
            Instantiate(note.goodHitEffect, note.transform.position, note.transform.rotation);
            GoodHit(note.transform.position);
        }
    }
}