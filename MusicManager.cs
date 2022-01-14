using UnityEngine;
using static MusicDefinitions;

public class MusicManager : MonoBehaviour
{
    public MusicPlayer[] musicPlayers;

    [Header("Master Variables")]
    public int BPM = 100;
    public Note Scale = Note.NoteC;
    public int Mode = 0;
    public TimeSignature timeSignature = TimeSignature.CommonTime;

    [Header("Notes Being Played")]
    public Note[] NotesPlaying;


    private void Start()
    {
        musicPlayers = GetComponentsInChildren<MusicPlayer>();
        for (int i = 0; i < musicPlayers.Length; i++)
        {
            musicPlayers[i].scale = Scale;
            musicPlayers[i].mode = Mode;
        }
    }
    private void Update()
    {
        for (int i = 0; i < musicPlayers.Length; i++)
        {
            musicPlayers[i].TimerCheck(Time.deltaTime);
        }
    }

    public void UpdateNotesPlayed()
    {
        for (int i = 0; i < musicPlayers.Length; i++)
        {
            NotesPlaying[i] = musicPlayers[i].phrase.notes[musicPlayers[i].phrasePointer];
        }
    }
}
