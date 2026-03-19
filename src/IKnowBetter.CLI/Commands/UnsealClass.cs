using System.ComponentModel;
using Mono.Cecil;
using Spectre.Console.Cli;

public class UnsealClass : Command<UnsealClass.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<dllpath>")]
        [Description("The path to a .dll file containing a CLI application.")]
        public string DllPath { get; init; } = string.Empty;
        
        [CommandArgument(1, "<className>")]
        [Description("The FULLY QUALIFIED class name.")]
        public string ClassName { get; init; } = string.Empty;
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        ReaderParameters readerParameters = new()
        {
            ReadingMode = ReadingMode.Immediate,
            InMemory = true,
        };
        AssemblyDefinition? assembly = AssemblyDefinition.ReadAssembly(settings.DllPath, readerParameters);
        TypeDefinition? typeForChange = assembly.MainModule.GetType(settings.ClassName);
        
        if (typeForChange is not null)
        {
            typeForChange.Attributes &= ~TypeAttributes.Sealed;
            assembly.Write(settings.DllPath);
            Console.WriteLine($"Class {typeForChange} successfully unsealed.");
        }
        else
        {
            Console.WriteLine($"Class {settings.ClassName} not found in assembly {settings.DllPath}.");
        }

        return 0;
    }
}