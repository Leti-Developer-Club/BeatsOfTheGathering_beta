// ChartSpawner.cs (replace NoteSpawner)
using UnityEngine;
using System.Collections.Generic;

public class ChartSpawner : MonoBehaviour
{
    [Header("Music")]
    public AudioSource music;
    public TextAsset chartFile;   // drag your JSON here

    [Header("Prefab & Lanes")]
    public Transform[] spawnPoints;
    public GameObject notePrefab;
    public float noteSpeed = 8f;
    public float hitLineY = 0f;
    public float spawnY = 8f;

    // Internals
    private ChartData chart;
    private double dspSongStart;
    private double secPerBeat;
    private int nextNoteIndex = 0;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        // Load JSON
        chart = JsonUtility.FromJson<ChartData>(chartFile.text);
        secPerBeat = 60.0 / chart.bpm;

        // Prewarm pool
        for (int i = 0; i < 64; i++)
        {
            var n = Instantiate(notePrefab);
            n.SetActive(false);
            n.transform.SetParent(transform.parent, false);
            pool.Enqueue(n);
        }
    }

    void Start()
    {
        dspSongStart = AudioSettings.dspTime + 0.2;
        music.PlayScheduled(dspSongStart);
    }

    void Update()
    {
        if (nextNoteIndex >= chart.notes.Count) return;

        double now = AudioSettings.dspTime;
        double travelTime = Mathf.Abs(spawnY - hitLineY) / noteSpeed;

        var noteData = chart.notes[nextNoteIndex];
        double noteBeatTime = dspSongStart + chart.offset + (noteData.beat * secPerBeat);

        // Spawn early so note arrives on beat
        if (now + travelTime >= noteBeatTime)
        {
            SpawnNote(noteData.lane);
            nextNoteIndex++;
        }
    }

    void SpawnNote(int lane)
    {
        var p = (pool.Count > 0) ? pool.Dequeue() : Instantiate(notePrefab);

        p.transform.SetParent(transform.parent, false);
        var sp = spawnPoints[lane].localPosition;
        p.transform.localPosition = new Vector3(sp.x, spawnY, sp.z);
        p.transform.localRotation = Quaternion.identity;

        var mover = p.GetComponent<NoteMover>();
        if (mover) mover.speed = noteSpeed;

        p.SetActive(true);
    }
}
