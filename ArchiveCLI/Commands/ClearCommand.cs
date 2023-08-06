using CommandSystem;

namespace ArchiveCLI.Commands
{
    internal class ClearCommand : ICommand
    {
        public ICommand? Execute()
        {
            Console.Clear();
            return null;
        }
    }
}
