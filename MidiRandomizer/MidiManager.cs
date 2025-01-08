using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Note = Melanchall.DryWetMidi.Interaction.Note;

namespace MidiRandomizer
{
    public static class MidiManager
    {
        public const string FileName = "Song";
        public const string FileType = "mid";
        public const int DeviationPercent = 10;

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

            midiFile.Write($"{FileName}.{FileType}");
        }

        public static void ChangeNotePositions()
        {
            var midiFileDirectories = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), $"*.{FileType}", SearchOption.AllDirectories).ToList();

            foreach (var midiFileDirectory in midiFileDirectories)
            {
                var midiFile = MidiFile.Read(midiFileDirectory);

                Random increaseRnd = new Random();
                Random deviationRnd = new Random();

                foreach (var trackChunk in midiFile.GetTrackChunks())
                {
                    using (var notesManager = trackChunk.ManageNotes())
                    {
                        foreach (var note in notesManager.Objects)
                        {
                            int deviation = deviationRnd.Next(GetMaxRnd(note));

                            if (IsIncrease(increaseRnd))
                                note.Time += deviation;
                            else
                                note.Time -= deviation;
                        }
                    }
                }

                midiFile.Write($"{midiFileDirectory} {DeviationPercent}%.{FileType}", true);

                static int GetMaxRnd(Note note) => Convert.ToInt32(note.EndTime - note.Time) * DeviationPercent / 100;

                static bool IsIncrease(Random rnd) => rnd.Next(100) < 50;
            }
        }

        public static void ReadNotes()
        {
            var midiFile = MidiFile.Read($"{FileName}.{FileType}");

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