using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

namespace MidiRandomizer
{
    public static class MidiManager
    {
        public const string FileName = "Song.mid";

        public static void CreateSingleNoteTrack()
        {
            var midiFile = new MidiFile(
                new TrackChunk(
                    new SetTempoEvent(500000)),
                new TrackChunk(
                    new TextEvent("It's just single note track..."),
                    new NoteOnEvent((SevenBitNumber)60, (SevenBitNumber)45),
                    new NoteOffEvent((SevenBitNumber)60, (SevenBitNumber)0)
                    {
                        DeltaTime = 400
                    }));

            midiFile.Write(FileName);
        }

        public static void ChangeNotePositions()
        {
            var midiFile = MidiFile.Read(FileName);

            foreach (var trackChunk in midiFile.GetTrackChunks())
            {
                using (var notesManager = trackChunk.ManageNotes())
                {
                    notesManager.Objects.RemoveAll(n => n.NoteName == NoteName.CSharp);

                    Random gen = new Random();
                    Random gen2 = new Random();

                    foreach (var note in notesManager.Objects)
                    {
                        int prob = gen.Next(100);
                        int prob2 = gen2.Next(12);

                        if (prob < 50)
                        {
                            note.Time -= prob2;
                        }
                        else
                        {
                            note.Time += prob2;
                        }
                    }
                }
            }

            midiFile.Write(FileName, true);
        }

        public static void ReadNotes()
        {
            var midiFile = MidiFile.Read(FileName);

            foreach (var trackChunk in midiFile.GetTrackChunks())
            {
                using (var notesManager = trackChunk.ManageNotes())
                {
                    notesManager.Objects.RemoveAll(n => n.NoteName == NoteName.CSharp);

                    if (notesManager.Objects.Any())
                    {
                        var times = notesManager.Objects.Select(t => t.Time).ToList();
                        times.ForEach(t => Console.WriteLine(t));
                    }
                }
            }
        }
    }
}