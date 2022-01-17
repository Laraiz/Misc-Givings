using System.Collections.Generic;
using UnityEngine;
public class MusicDefinitions : MonoBehaviour
{
    #region Dictionaries
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
    #endregion
    #region Class Defs
    public class Beat
    {
        public float length = 0;
        public int Priority = 0;

        #region Constructors
        public Beat(float length, int priority = 0)
        {
            this.length = length;
        }
        #endregion
        #region Beat Defs
        public static Beat Whole = new Beat(4);
        public static Beat DottedHalf = new Beat(3);
        public static Beat Half = new Beat(2);
        public static Beat DottedQuarter = new Beat(1.5f);
        public static Beat Quarter = new Beat(1);
        public static Beat DottedEighth = new Beat(.75f);
        public static Beat Eighth = new Beat(.5f);
        #endregion
        public static implicit operator Beat(float length)
        {
            return new Beat(length);
        }
    }// just for readability and random assignment
    public class TimeSignature
    { //Rhythm constructed based on time signature
        public int topNumber;
        public int bottomNumber;
        public Beat[] beatLengths;
        //constructor
        private TimeSignature(int TopNumber, int BottomNumber) //for singles shouldnt need a new construction
        {
            topNumber = TopNumber;
            bottomNumber = BottomNumber;

            //time signature is topnumber bottomNumbers per measure
            Beat[] timeSigStorage = new Beat[topNumber];
            for (int i = 0; i < timeSigStorage.Length; i++)
            {
                timeSigStorage[i] = new Beat(1 / (float)bottomNumber, GetAccentsFromInt(topNumber)[i]);
            }
            beatLengths = timeSigStorage;
            
        }
        public TimeSignature(int[] topNumbers, int bottomNumber) //for combination time signatures
        {
            topNumber = 0;
            this.bottomNumber = bottomNumber;

            for (int i = 0; i < topNumbers.Length; i++)
            {
                topNumber += topNumbers[i];
            }

            beatLengths = new Beat[topNumber];
            for (int i = 0; i < topNumbers.Length; i++)
            {
                for (int ii = 0; ii < topNumbers[i]; ii++)
                {
                    beatLengths[i] = new Beat(1 / (float)bottomNumber, GetAccentsFromInt(topNumbers[i])[ii]);
                }
            }
        }

        #region TimeSignature Defs
        public static TimeSignature CommonTime = new TimeSignature(4, 4); //emphasis on 1 and 3
        public static TimeSignature CutTime = new TimeSignature(2, 2); //emphasis on 1 and 2

        public static TimeSignature SimpleDuple = new TimeSignature(2, 4);
        public static TimeSignature SimpleTriple = new TimeSignature(3, 4);

        public static TimeSignature ComplexDuple = new TimeSignature(6, 8); //2+2+2 emphasis on 1 3 5
        public static TimeSignature ComplexTriple = new TimeSignature(9, 8); //3+3+3 emphasis on 1 4 7
        public static TimeSignature ComplexQuadRuple = new TimeSignature(12, 8); // 3+3 +3+3 +3+3 emphasis on 1 4 7 11

        public static TimeSignature TwoPlusThree = new TimeSignature(new int[] { 2, 3 }, 4);
        public static TimeSignature ThreeThree = new TimeSignature(3, 3);
        #endregion
        private static int[] GetAccentsFromInt(int topNumber)
        {
            int[] intStorage = new int[topNumber];
            if (topNumber == 6) //specifically for duples
            {
                int counter = 1;
                for (int i = 0; i < topNumber; i++)
                {
                    if (counter == 1) intStorage[i] = 1;
                    else intStorage[i] = 2;
                    counter++;
                    if (counter >= 2) counter = 1;
                }
            }
            else if (topNumber % 3 == 0)
            {
                int counter = 1;
                for (int i = 0; i < topNumber; i++)
                {
                    if (counter == 1) intStorage[i] = 1;
                    else intStorage[i] = topNumber/3+1;
                    counter++;
                    if (counter >= 3) counter = 1;
                }
            }
            else if (topNumber % 2 == 0)
            {
                int counter = 1;
                for (int i = 0; i < topNumber; i++)
                {
                    if (counter == 1) intStorage[i] = 1;
                    else intStorage[i] = topNumber/2+1;
                    counter++;
                    if (counter >= 2) counter = 1;
                }
            }
            return intStorage;
        }
    }
    public class Rhythm
    {
        //Phrase Assigns constructs notes based on rhythm
        public TimeSignature timeSignature = TimeSignature.CommonTime;
        public Beat[] beats;

        #region Constructors
        public Rhythm(Beat[] beats)
        {
            this.beats = beats;
        }
        public Rhythm(TimeSignature timeSignature)// First Iteration to provide structure
        {
            this.timeSignature = timeSignature;
            Beat[] beatStorage = timeSignature.beatLengths;

            beatStorage = SubDivide(beatStorage);
            beatStorage = Combine(beatStorage);//add this to combine method
            this.beats = beatStorage;
        }
        #endregion
        #region static Methods
        private static Beat[] SubDivide(Beat[] oldBeats)
        {
            Beat[] MaxStorage = new Beat[oldBeats.Length * 2];
            for (int i = 0; i < oldBeats.Length; i++)
            {
                MaxStorage[i * 2] = oldBeats[i];
            }
            for (int i = 0; i < oldBeats.Length; i++)
            {
                if (Random.Range(0, (float)1) < 1f / (float)oldBeats[i].Priority + 2)
                {
                    MaxStorage[i] = new Beat(oldBeats[i].length / 2f, oldBeats[i].Priority);
                    MaxStorage[i + 1] = new Beat(oldBeats[i].length / 2f, oldBeats[i].Priority);
                }
            }
            MaxStorage = PurgeNull(MaxStorage);
            return MaxStorage;
        } //should redesign this future me problem
        private static Beat[] Combine(Beat[] oldBeats)
        {
            Beat[] beatStorage = new Beat[oldBeats.Length];
            for (int i = 0; i < oldBeats.Length - 1; i++)
            {
                if(Random.Range(0,(float)1) > 1f / (float)oldBeats[i].Priority + 2)
                {
                    oldBeats[i] = new Beat(oldBeats[i].length * 2f, oldBeats[i].Priority);
                    oldBeats[i + 1] = new Beat(0);
                }
            }

            beatStorage = PurgeNull(beatStorage);
            return beatStorage;
        }
        private  static Beat[] PurgeNull(Beat[] oldBeats) //Call once
        {
            int counter = 0;
            for (int i = 0; i < oldBeats.Length - 1; i++)
            {
                Debug.Log(i);
                Debug.Log(oldBeats[i].length);
                if (oldBeats[i].length == 0 || oldBeats[i] == null)
                {
                    counter++;
                }
            }
            Beat[] BeatStorage = new Beat[oldBeats.Length - counter];

            counter = 0;
            for (int i = 0; i < oldBeats.Length; i++)
            {
                if(oldBeats[i].length < 0) BeatStorage[i + counter] = oldBeats[i];
                if (oldBeats[i].length == 0) counter++;
            }
            return BeatStorage;
        }
        #endregion
    }
    public class Note
    {
        public string name;
        public int noteID;

        public Beat beat = 1;
        public int octave = 1;

        //for constructing Phrases
        public bool isAccent = false;

        #region Constructors
        private Note(int ID, string name) //for static definition do not use
        {
            noteID = ID;
            this.name = name;
        }
        public Note(Note note, Beat length, int octave) //note has ID and name Preconstructed
        { 
            this.noteID = note.noteID;
            this.name = note.name;

            this.beat = length;
            this.octave = octave;
        }
        public Note(Note note, int octave)//Note has ID and Name Preconstructed
        {
            this.noteID = note.noteID;
            this.name = octave.ToString() + "_" + note.name;
            this.octave = octave;
            //default length
        }
        public Note(Beat beat, bool isAccent, Note note) //Phrase calls using Rhythm
        {
            this.beat = beat;
            this.isAccent = isAccent;

            noteID = note.noteID;
            octave = note.octave;
            name = note.name;

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
    }
    public class Phrase
    {
        //storage hopefuly i can do some fun UI shenanigans
        public Note[] notes;
        public Rhythm Rhythm;

        #region Constructors
        public Phrase(Note[] notes) //assumes notes has valid info, assums not Gen Phrase
        {
            this.notes = notes;
        }
        public Phrase(Rhythm rhythm)
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
                notes[i] = notes[i - 1].noteID + vs[i - 1];
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
    public class Measure
    {

    }
    public class Song
    {
        TimeSignature timeSignature;
        Phrase phrase;
    }
    #endregion
}
