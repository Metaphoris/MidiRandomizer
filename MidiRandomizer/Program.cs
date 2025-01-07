namespace MidiRandomizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PrintStartText();
            ExecuteUserAction();
            Console.WriteLine("Done!");
        }
        
        static void PrintStartText()
        {
            Console.WriteLine("Hello! What do you want to do?");
            Console.WriteLine("Enter \"1\" to create single note track");
            Console.WriteLine("Enter \"2\" to print all notes positions");
            Console.WriteLine("Enter \"3\" to randomize all notes positions");
        }

        static void ExecuteUserAction()
        {
            var choise = Console.ReadLine();
            switch (choise)
            {
                case "1":
                    MidiManager.CreateSingleNoteTrack();
                    break;
                case "2":
                    MidiManager.ReadNotes();
                    break;
                case "3":
                    MidiManager.ChangeNotePositions();
                    break;
                default:
                    Console.WriteLine("Uncorrect input");
                    break;
            }
        }
    }
}