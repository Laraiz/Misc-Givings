using UnityEngine;
using static MusicDefinitions;

public class MusicPlayer : MonoBehaviour
{
    public Instrument instrument;
    [HideInInspector] public AudioSource audioSource;

    [Header("Controls")]
    public bool SimpleControls = true;
    public bool isMuted;

    public enum SelectiontoPickFrom { Mario, Maria, Chromatic_Scale, RandomQuarters, TrueRandom }
    public SelectiontoPickFrom pick_Phrase;
    public Phrase phrase;
    [HideInInspector] public Note scale;
    [HideInInspector] public int mode;

    public float timer;
    public float[] noteTimeToPlay;
    public int phrasePointer;

    private bool loadOnReset = false;

    [Header("Debug")]
    public float[] vs;

    private void Start()
    {
        gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();

        InitializePhrase();
        LoadPhrase();

        vs = MinNotesinMeasure(new TimeSignature(4, 4));
    }

    private void InitializePhrase()
    {
        if (SimpleControls)
        {
            switch (pick_Phrase)
            {
                case SelectiontoPickFrom.Maria:
                    phrase = Phrase.Maria;
                    break;
                case SelectiontoPickFrom.Mario:
                    phrase = Phrase.Mario;
                    break;
                case SelectiontoPickFrom.Chromatic_Scale:
                    phrase = Phrase.Chromatic_Scale;
                    break;
                case SelectiontoPickFrom.RandomQuarters:
                    phrase = Phrase.Random_Quarters();
                    loadOnReset = true;
                    break;
                case SelectiontoPickFrom.TrueRandom:
                    phrase = Phrase.TrueRandom();
                    loadOnReset = true;
                    break;
            }
        }
    }

    private void LoadPhrase()
    {
        noteTimeToPlay = new float[phrase.notes.Length];
        for (int i = 0; i < phrase.notes.Length; i++)
        {
            if (i != 0)
            {
                noteTimeToPlay[i] = noteTimeToPlay[i - 1] + phrase.notes[i - 1].noteLength;
            }
            else
            {
                noteTimeToPlay[i] = 0;
            }
        }
    }

    public void TimerCheck(float delta)
    {

        if (timer >= noteTimeToPlay[phrase.notes.Length - 1] + phrase.notes[phrase.notes.Length - 1].noteLength)
        {
            timer = 0;
            if (loadOnReset)
            {
                InitializePhrase();
                LoadPhrase();
            }
        }
        if (phrasePointer == phrase.notes.Length)
        {
            phrasePointer = 0;
        }
        timer += delta;
        if (noteTimeToPlay[phrasePointer] <= timer && timer <= noteTimeToPlay[phrase.notes.Length - 1] + delta)
        {
            PlayNote();
            phrasePointer++;
        }
    }

    public void PlayNote()
    {
        float length = phrase.notes[phrasePointer].noteLength;
        Note note = phrase.notes[phrasePointer];
        if (note.noteID != Note.Rest.noteID || isMuted == false)
        {
            Debug.Log(note.name);
            AudioClip clip = instrument.MakeSubClip(length, note);
            audioSource.PlayOneShot(clip);
        }
        else return;
    }
}
