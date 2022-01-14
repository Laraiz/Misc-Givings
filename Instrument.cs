using UnityEditor;
using UnityEngine;
using static MusicDefinitions;
// all audioclips must be named in the pattern
// 
//     SampleFileName_Desiredoctave_Notename
//     SampleFileName_Desiredoctave_Notenameb
//     SampleFileName_Desiredoctave_Notenames
// 
//      I.E.
//      Piano_3_Ab

//it is most helpful if your containing files are named after Instruments
// Instrument Name needs to be the same as FileName

[ExecuteInEditMode]
public class Instrument : MonoBehaviour
{
    //how samples are sorted in order
    public int Octaves = 2;      //amount of octaves per instrument
    const int Notes = 12;     //12 notes in a chromatic scale
    /// accents
    /// Articulation
    /// seperate these into different instruments feels wrong 
    /// use overloads to default to normal?
    /// :
    /// They are also different across instruments and cant be called uniformly

    public int OctaveOffset;

    #region  Load Samples
    public bool LoadSamples;
    private void Update()
    {
        if (LoadSamples)
        {
            LoadSamples = false;
            OnMenuLoad();
        }
    }
    #endregion

    public Object SampleFolder;
    public AudioClip[] Samples;
    private Object[] UnsortedSamples;

    #region Sorting

    public void OnMenuLoad()
    {
        PullSamples();
        SortByOctaveByNote();
        DeleteUnsortedSamples();
    }

    private void PullSamples()
    {
        string[] SampleString = AssetDatabase.FindAssets(SampleFolder.name);
        UnsortedSamples = new Object[SampleString.Length];

        for (int i = 0; i < SampleString.Length; i++)
        {
            SampleString[i] = AssetDatabase.GUIDToAssetPath(SampleString[i]);
        }

        for (int i = 0; i < SampleString.Length; i++)
        {
            UnsortedSamples[i] = AssetDatabase.LoadAssetAtPath<Object>(SampleString[i]);
        }
    }

    private void SortByOctaveByNote()
    {
        Samples = new AudioClip[Octaves * Notes + 1];

        for (int a = 0; a < UnsortedSamples.Length; a++)
        {
            string sampleName = UnsortedSamples[a].name;
            string fileName = SampleFolder.name;

            if (UnsortedSamples[a] is AudioClip)
            {
                if (string.CompareOrdinal(sampleName, 0, fileName, 0, fileName.Length) == 0)
                {
                    /*naming convention is
                        Filename_Octave#_noteName
                        filename.length_#_#
                        filename.Length_#_##
                    */
                    char sampleOctave = sampleName[fileName.Length + 1];

                    int displayedOctave = sampleOctave - 48; //48 is the 0 in ASCII charactors
                    int Octave = displayedOctave - OctaveOffset + 1;

                    string noteName = sampleName.Substring(fileName.Length + 1 + 2);
                    int noteInt; StringtoIntDic.TryGetValue(noteName, out noteInt);

                    Samples[(Octave - 1) * Notes + (noteInt - 1)] = UnsortedSamples[a] as AudioClip;
                }
            }
        }
    }

    private void DeleteUnsortedSamples()
    {
        UnsortedSamples = null;
    }
    #endregion
    #region Get Methods
    public AudioClip GetAudioClip(int NoteID) //unused
    {
        return Samples[NoteID];
    }
    public AudioClip GetAudioClip(Note Note, int octave = -1)
    {
        if (octave == -1)
        {
            octave = Note.octave;
        }
        if (Note.noteID == -1 || Note.noteID == 0 && octave == 1)
        {
            return AudioClip.Create("rest", 1, 1, 1000, false);
        }
        return Samples[GetSoundID(Note.noteID, octave)];
    }
    private AudioClip MakeSubClip(float start, float stop, AudioClip original)
    {
        string name = original.name;
        int frequency = original.frequency;
        float timeLength = stop - start;
        int Length = (int)(frequency * timeLength * original.channels);

        AudioClip newAudioClip = AudioClip.Create(name, Length, original.channels, frequency, false);

        float[] data = new float[Length];
        original.GetData(data, (int)(frequency * start));
        newAudioClip.SetData(data, 0);

        return newAudioClip;
    }
    public AudioClip MakeSubClip(float start, float stop, Note note, int octave = 1)
    {
        AudioClip original = GetAudioClip(note, octave);
        return MakeSubClip(start, stop, original);
    }
    private AudioClip MakeSubClip(float timeLength, AudioClip original)
    {
        float randomStart = Random.Range(0f, 2f);

        AudioClip newAudioClip = MakeSubClip(0, timeLength, original);
        return newAudioClip;
    }
    public AudioClip MakeSubClip(float Length, Note note)
    {
        AudioClip original = GetAudioClip(note, note.octave);
        return MakeSubClip(Length, original);
    }
    #endregion
}
