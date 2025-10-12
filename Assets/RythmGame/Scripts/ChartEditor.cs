using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.IO;

public class ChartEditor : MonoBehaviour
{
    [Header("Music")]
    public AudioSource music;
    public float bpm = 120f;
    public float offset = 0f;
    public string fileName = "chart_output.json";

    private GameInput input; // auto-generated class from Input Actions
    private double dspSongStart;
    private double secPerBeat;
    private bool isRecording = false;

    private List<NoteData> recordedNotes = new List<NoteData>();

    void Awake()
    {
        input = new GameInput();
    }

    void OnEnable()
    {
        input.Enable();

        // Bind recording controls
        input.Gameplay.StartRecording.performed += _ => StartRecording();
        input.Gameplay.StopRecording.performed += _ => StopRecording();

        // Bind lane keys
        input.Gameplay.Lane0.performed += _ => RecordLane(0);
        input.Gameplay.Lane1.performed += _ => RecordLane(1);
        input.Gameplay.Lane2.performed += _ => RecordLane(2);
        input.Gameplay.Lane3.performed += _ => RecordLane(3);
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        secPerBeat = 60f / bpm;
    }

    void StartRecording()
    {
        if (isRecording) return;
        Debug.Log("Recording started...");

        dspSongStart = AudioSettings.dspTime + 0.2;
        music.PlayScheduled(dspSongStart);
        isRecording = true;
        recordedNotes.Clear();
    }

    void StopRecording()
    {
        if (!isRecording) return;
        isRecording = false;

        SaveChart();
        Debug.Log("Recording stopped. Chart saved.");
    }

    void RecordLane(int lane)
    {
        if (!isRecording) return;

        double songTime = AudioSettings.dspTime - dspSongStart - offset;
        float beat = (float)(songTime / secPerBeat);

        recordedNotes.Add(new NoteData { beat = beat, lane = lane });
        Debug.Log($"Note added: Lane {lane}, Beat {beat:F2}");
    }

    void SaveChart()
    {
        ChartData chart = new ChartData
        {
            bpm = bpm,
            offset = offset,
            notes = recordedNotes
        };

        string json = JsonUtility.ToJson(chart, true);
        string path = Path.Combine(Application.dataPath, "Resources/Charts/" + fileName);
        File.WriteAllText(path, json);
        Debug.Log("Chart saved to: " + path);
    }
}
