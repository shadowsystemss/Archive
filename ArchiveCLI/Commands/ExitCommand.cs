using CommandSystem;

namespace ArchiveCLI.Commands
{
    internal class ExitCommand : ICommand
    {
        public ICommand? Execute()
        {
            Environment.Exit(0);
            return null;
        }
    }
}
