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
        public const int TimeDeviationPercent = 10;
        public const int MaxVelocityDeviation = 5;

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

        public static void RandomizeTimeAndVelocity()
        {
            var midiFileDirectories = GetMidiFileDirectories();

            foreach (var midiFileDirectory in midiFileDirectories)
            {
                var midiFile = MidiFile.Read(midiFileDirectory);

                Random increaseRnd = new Random();
                Random deviationTimeRnd = new Random();
                Random deviationVelocityRnd = new Random();

                foreach (var trackChunk in midiFile.GetTrackChunks())
                {
                    using (var notesManager = trackChunk.ManageNotes())
                    {
                        foreach (var note in notesManager.Objects)
                        {
                            int deviation = deviationTimeRnd.Next(GetMaxRndTime(note));

                            if (IsIncrease(increaseRnd))
                                note.Time += deviation;
                            else if (note.Time >= deviation)
                                note.Time -= deviation;

                            if (IsIncrease(increaseRnd))
                                note.Velocity = (SevenBitNumber)((int)note.Velocity + MaxVelocityDeviation);
                            else
                                note.Velocity = (SevenBitNumber)((int)note.Velocity - MaxVelocityDeviation);
                        }
                    }
                }

                midiFile.Write($"{midiFileDirectory} {TimeDeviationPercent}%.{FileType}", true);

                static int GetMaxRndTime(Note note) => Convert.ToInt32(note.EndTime - note.Time) * TimeDeviationPercent / 100;

                static bool IsIncrease(Random rnd) => rnd.Next(100) < 50;
            }
        }

        public static void ReadNotes()
        {
            var midiFileDirectories = GetMidiFileDirectories();

            foreach (var midiFileDirectory in midiFileDirectories)
            {
                var midiFile = MidiFile.Read(midiFileDirectory);

                foreach (var trackChunk in midiFile.GetTrackChunks())
                {
                    using (var notesManager = trackChunk.ManageNotes())
                    {
                        notesManager.Objects.RemoveAll(n => n.NoteName == NoteName.CSharp);

                        if (notesManager.Objects.Any())
                        {
                            var timeAndVelocity = notesManager.Objects.Select(t => new { t.Time, t.Velocity }).ToList();
                            timeAndVelocity.ForEach(t => Console.WriteLine($"Time: {t.Time}. Velocity: {t.Velocity}"));
                        }
                    }
                }
            }
        }

        private static List<string> GetMidiFileDirectories()
            => Directory.EnumerateFiles(Directory.GetCurrentDirectory(), $"*.{FileType}", SearchOption.AllDirectories).ToList();
    }
}