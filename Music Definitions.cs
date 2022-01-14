using System.Collections.Generic;
using UnityEngine;
public class MusicDefinitions : MonoBehaviour
{
    #region Dictionary
    public static Dictionary<string, int> StringtoIntDic = new Dictionary<string, int>()
    {
        //should probably add variations to noteas but am i gunna?
        { "A", 1},      {"a",1},    {"NoteA",1},
        { "As", 2},     {"Bb", 2},      {"as",2},   {"bb",2},   {"NoteAs",2},   {"NoteBb",2},
        { "B", 3},      {"b",3},    {"NoteB",3},
        { "C", 4 },     {"c",4},    {"NoteC",4},
        { "Cs", 5},     { "Db", 5 },    {"cs",5},   {"db",5},   {"NoteCs",5},   {"NoteDb",5},
        { "D", 6},      {"d",6},    {"NoteD",6},
        { "Ds", 7 },    { "Eb", 7},     {"ds",7},   {"eb", 7},
        {"E", 8},       {"e",8},    {"NoteE",8},
        { "F", 9},      {"f",9},    {"NoteF",9},
        { "Fs", 10},    { "Gb", 10},    {"fs",10},  {"gb",10}, {"NoteGb",10}, {"NoteFs",10},
        { "G", 11},     {"g", 11},  {"NoteG",11},
        { "Gs", 12},    { "Ab", 12 },   {"gs",12 }, {"ab" , 12}, {"NoteGs",12}, {"NoteAb",12}
    };
    #endregion
    #region Static Gets
    public static int GetSoundID(Note Note, int octave)
    {
        int SoundID = (octave - 1) * 12 + Note.noteID - 1;
        return SoundID;
    }
    public static int RandomNoteIDChromatic(int minRange = 1, int maxRange  = 12)
    {
        return Random.Range(minRange, maxRange + 1);
    }
    public static int RandomNoteIDScalar(int minRange = 1, int MaxRange = 7) //maxRange cant be higher than 7
    {
        int startValue = Random.Range(minRange, MaxRange + 1);

        switch (startValue)
        {
            case 1: return 1;
            case 2: return 3;
            case 3: return 4;
            case 4: return 6;
            case 5: return 8;
            case 6: return 9;
            case 7: return 11;
            default:
                Debug.Log("Invalid Range, Try Changing your min or max value");
                return 0;
        }
    }
    public static float[] MinNotesinMeasure(TimeSignature timeSig)
    {
        float notesPerMeasure = timeSig.topNumber*4f / timeSig.bottomNumber;
                                            //find the smallest amount of notes that can fit in the measure
        float[] maxNoteLengthStorage = new float[20]; //float size is max notesize
        int iterations = 0;
        while (SumFloats(maxNoteLengthStorage) + .49f * timeSig.bottomNumber < notesPerMeasure && iterations < maxNoteLengthStorage.Length)
        {

            float curLength = SumFloats(maxNoteLengthStorage);

            bool isWholeShorterthanLength = curLength + 4 * timeSig.bottomNumber < notesPerMeasure;
            bool doesWholeEqualLength = Mathf.Approximately(curLength + 4 / timeSig.bottomNumber, notesPerMeasure);

            bool isHalfShorterthanLength = curLength + 2 * timeSig.bottomNumber < notesPerMeasure;
            bool doesHalfEqualLength = Mathf.Approximately(curLength + 2 / timeSig.bottomNumber, notesPerMeasure);

            bool isQuarterShorterthanLength = curLength + 1 * timeSig.bottomNumber < notesPerMeasure;
            bool doesQuarterEqualLength = Mathf.Approximately(curLength + 1 / timeSig.bottomNumber, notesPerMeasure);

            bool isEighthShorterthanLength = curLength + .5f * timeSig.bottomNumber < notesPerMeasure;
            bool doesEighthEqualLength = Mathf.Approximately(curLength + .5f / timeSig.bottomNumber, notesPerMeasure);

            if ( isWholeShorterthanLength || doesWholeEqualLength)
            {
                maxNoteLengthStorage[iterations] = 4;
            }
            else if (isHalfShorterthanLength || doesHalfEqualLength) 
            {
                maxNoteLengthStorage[iterations] = 2;
            }

            else if ( isQuarterShorterthanLength|| doesQuarterEqualLength)
            {
                maxNoteLengthStorage[iterations] = 1;
            }
            else if ( isEighthShorterthanLength|| doesEighthEqualLength)
            {
                maxNoteLengthStorage[iterations] = .5f;
            }

            iterations++;
        }

        return maxNoteLengthStorage;
        int unNullLength = 0;

        for (int i = 0; i < maxNoteLengthStorage.Length; i++)
        {
            if (maxNoteLengthStorage[i] != 0) unNullLength++;
        }

        float[] noteLengths = new float[unNullLength];

        for (int i = 0; i < noteLengths.Length; i++)
        {
            noteLengths[i] = maxNoteLengthStorage[i];
        }
        //return noteLengths;
    }
    public static float SumFloats(float[] floats)
    {
        float additionProduct = 0;
        for (int i = 0; i < floats.Length; i++)
        {
            additionProduct += floats[i];
        }
        return additionProduct;
    }
    #endregion
    #region Class Defs
    public struct TimeSignature
    {
        [HideInInspector]//there is a reason its a float IDK what it is but there is one
        public float topNumber; //3.5/4 time sig FTW
        [HideInInspector]
        public float bottomNumber;

        //todo make a 4+4+3 time signature different than an 8+3

        public string TimeSignatureDisplay;

        //constructor
        public TimeSignature(float TopNumber, float BottomNumber)
        {
            topNumber = TopNumber;
            bottomNumber = BottomNumber;

            TimeSignatureDisplay = topNumber.ToString() + "/" + bottomNumber.ToString();
        }
        //add your own here and YOU make up names for them
        public static TimeSignature CommonTime = new TimeSignature(4, 4);
        public static TimeSignature CutTime = new TimeSignature(2, 2);
    }
    public class Note
    {
        public string name;
        public int noteID;

        public float noteLength = 1;
        public int octave = 1;

        //for constructing Phrases
        public bool isAccent = false;
        public bool isRelative = false;
        public bool toAccent = false;

        #region Constructors
        private Note(int ID, string name) //for static definition do not use
        {
            noteID = ID;
            this.name = name;
        }
        public Note(Note note, float length, int octave) //note has ID and name Preconstructed
        {
            this.noteID = note.noteID;
            this.name = octave.ToString() + "_" + note.name;

            this.noteLength = length;
            this.octave = octave;
        }
        public Note(Note note, int octave)//Note has ID and Name Preconstructed
        {
            this.noteID = note.noteID;
            this.name = note.name;
            this.octave = octave;
            //default length
        }
        #endregion
        #region Note Defs
        public static Note NoteA = new Note(1, "NoteA");
        public static Note NoteAs = new Note(2, "NoteAs");
        public static Note NoteBb = new Note(2, "NoteBb");
        public static Note NoteB = new Note(3, "NoteB");
        public static Note NoteC = new Note(4, "NoteC");
        public static Note NoteCs = new Note(5, "NoteCs");
        public static Note NoteDb = new Note(5, "NoteDb");
        public static Note NoteD = new Note(6, "NoteD");
        public static Note NoteDs = new Note(7, "NoteDs");
        public static Note NoteEb = new Note(7, "NoteEb");
        public static Note NoteE = new Note(8, "NoteE");
        public static Note NoteF = new Note(9, "NoteF");
        public static Note NoteFs = new Note(10, "NoteFs");
        public static Note NoteGb = new Note(10, "NoteGb");
        public static Note NoteG = new Note(11, "NoteG");
        public static Note NoteGs = new Note(12, "NoteGs");
        public static Note NoteAb = new Note(0, "NoteAb");
        public static Note Rest = new Note(-1, "Rest");
        #endregion

        public static implicit operator Note(int noteID)
        {
            if (noteID == -1) { return Note.Rest; } //will give null value without exception

            noteID = noteID % 12;
            int octave = 1 + noteID / 12;

            if (noteID == 0) { noteID = 12; } //12 is 0 mod 12

            switch (noteID)
            {
                case 1: return new Note(Note.NoteA, octave);
                case 2: return new Note(Note.NoteAs, octave);
                case 3: return new Note(Note.NoteB, octave);
                case 4: return new Note(Note.NoteC, octave);
                case 5: return new Note(Note.NoteCs, octave);
                case 6: return new Note(Note.NoteD, octave);
                case 7: return new Note(Note.NoteDs, octave);
                case 8: return new Note(Note.NoteE, octave);
                case 9: return new Note(Note.NoteF, octave);
                case 10: return new Note(Note.NoteFs, octave);
                case 11: return new Note(Note.NoteG, octave);
                case 12: return new Note(Note.NoteGs, octave);
                case 0: return new Note(Note.NoteAb, octave);
                default: return Note.Rest;
            }
        }//used for Unique ID and Rest
        public static implicit operator int(Note noteclass)
        {
            return noteclass.noteID; //Leverage Preconstructed NoteID in class def
        }//used NoteType not Unique NoteID
        public static implicit operator Note(string noteString)
        {
            // if you want explicite note
            if (noteString[0] - 48 > 0 && noteString[0] - 48 < 11) //if notestring char is a number
            {
                //format should be #_notestring
                int octave = noteString[0] - 48;
                StringtoIntDic.TryGetValue(noteString.Substring(2), out int noteID);
                return new Note(noteID, octave);
            }
            else //for getting NoteType not unique ID
            {
                // format should be notestring
                StringtoIntDic.TryGetValue(noteString, out int noteID); //Leveraged Implicit int To Note format
                return noteID;
            }
        }//used for NoteType and Unique ID
        public static implicit operator string(Note noteclass)
        {
            return noteclass.name; //if its a note it has a name
        }//used for NoteType and Unique ID
    }
    public class Phrase
    {
        //storage hopefuly i can do some fun UI shenanigans
        public Note[] notes;
        public bool isChromatic = true; // is not relative, is relative to last note, is relative to last accent
        public TimeSignature timeSignature; //only for randomization //Unused

        public int phraseSize;
        public float time;

        #region Constructors
        public Phrase(Note[] notes) //assumes notes has valid info, assums not Gen Phrase
        {
            this.isChromatic = true;

            this.notes = notes;
            this.phraseSize = notes.Length;

            for (int i = 0; i < notes.Length; i++)
            {
                this.time += notes[i].noteLength;
            }
        }
        public Phrase(Note[] notes, float[] notelength, int[] octaves) //assumes not Gen Phrase
        {
            for (int i = 0; i < notes.Length; i++)
            {
                notes[i] = new Note(notes[i], notelength[i], octaves[i]);
            }
            new Phrase(notes);
        }
        public Phrase(Note[] notes, float[] noteLength) //Assumes Octaves, assumes not Gen phrase, should remove? 
        {
            for (int i = 0; i < notes.Length; i++)
            {
                notes[i] = new Note(notes[i], noteLength[i], 1);
            }
            new Phrase(notes);
        }
        public Phrase(TimeSignature timSig, int mode, bool isChromatic, bool isAscending,bool isNoteDistanceClose, Note minNote, Note MaxNote)
        {

        }
        #endregion

        #region Mario
        private static Note[] MarioNotes = new Note[]
        {
            new Note(Note.NoteE,.25f,1),new Note(Note.NoteE,.25f,1),new Note(Note.Rest,.25f,1),new Note(Note.NoteE,.25f,1),
            new Note(Note.NoteC,.25f,1),new Note(Note.Rest,.25f,1),new Note(Note.NoteG,.5f,1), new Note(Note.Rest,.25f,1),new Note(Note.NoteA,.5f,1)
        };
        public static Phrase Mario = new Phrase(MarioNotes);
        #endregion

        #region Maria!
        private static Note[] MariaNotes = new Note[]
        {
            new Note(Note.NoteC,.5f,1), new Note(Note.NoteG,.5f,1), new Note(Note.NoteE,1f,1)
        };
        public static Phrase Maria = new Phrase(MariaNotes);
        #endregion

        #region Chromatic Scale
        private static Note[] Chromatic_ScaleNotes = new Note[]
        {
            new Note(1,.25f,1), new Note(2,.25f,1), new Note(3,.25f,1),new Note(4,.25f,1),new Note(5,.25f,1),new Note(6,.25f,1),new Note(7,.25f,1),new Note(8,.25f,1),
                new Note(9,.25f,1),new Note(10,.25f,1),new Note(11,.25f,1),new Note(12,.25f,1),
            new Note(1,.25f,2),new Note(2,.25f,2),new Note(3,.25f,2),new Note(4,.25f,2),new Note(5,.25f,2),new Note(6,.25f,2),new Note(7,.25f,2),new Note(8,.25f,2),
            new Note(9,.25f,2),new Note(10,.25f,2),new Note(11,.25f,2),new Note(12,.25f,2),new Note(1,.25f,3)
        };
        public static Phrase Chromatic_Scale = new Phrase(Chromatic_ScaleNotes);
        #endregion

        #region Randomized Phrases
        private static Note[] Random_QuarterNotes()
        {
            return new Note[] { new Note(RandomNoteIDChromatic(), 1, 1) };
        }
        public static Phrase Random_Quarters()
        {
            return new Phrase(Random_QuarterNotes());
        }
        private static Note[] TrueRandom_Notes()
        {
            return new Note[] { new Note(Random.Range(1, 12), Random.Range(.25f, 1f), Random.Range(1, 2)) };
        }
        public static Phrase TrueRandom()
        {
            return new Phrase(TrueRandom_Notes());
        }
        #endregion
    }
    public class Chord
    {
        public string ChordName = null; //may be useless?
        public Note[] notes;
        private int[] intervals;
        #region Constructors
        private Note[] GetNotesFromInterval(int[] vs)
        {
            Note[] notes = new Note[vs.Length];
            notes[0] = 1;

            for (int i = 1; i < vs.Length; i++)
            {
                notes[i] = notes[i - 1] + vs[i - 1];
            }
            return notes;
        }
        public Chord(int[] intervals)
        {
            this.intervals = intervals;
        }
        #endregion
        #region Chord Definitions                                                               
        #endregion
    }
    public class ChordProgression
    {
        Chord[] chords;
        int size;

    }
    #endregion
}