using System.ComponentModel;
using Mono.Cecil;
using Spectre.Console.Cli;

public class MakeMethodPublic : Command<MakeMethodPublic.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<dllpath>")]
        [Description("The path to a .dll file containing a CLI application.")]
        public string DllPath { get; init; } = string.Empty;
        
        [CommandArgument(1, "<className>")]
        [Description("The FULLY QUALIFIED class name.")]
        public string ClassName { get; init; } = string.Empty;

        [CommandArgument(1, "<methodName>")]
        [Description("The name of a method that belongs to the class.")]
        public string MethodName { get; init; } = string.Empty;
    }
    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        ReaderParameters readerParameters = new()
        { 
            ReadingMode = ReadingMode.Immediate,
            InMemory = true
        };
        AssemblyDefinition? assembly = AssemblyDefinition.ReadAssembly(settings.DllPath, readerParameters);
        
        TypeDefinition? typeForChange = assembly.MainModule.GetType(settings.ClassName);
        if (typeForChange is not null)
        {
            MethodDefinition? method = typeForChange.Methods.SingleOrDefault(x => x.Name == settings.MethodName);
            if (method is not null)
            {
                method.IsPublic = true;
                assembly.Write(settings.DllPath);
                Console.WriteLine($"Method {settings.ClassName}.{settings.MethodName} successfully made public.");
            }
        }
        else
        {
            Console.WriteLine($"Method {settings.ClassName}.{settings.MethodName} not found in assembly {settings.DllPath}.");
        }

        return 0;
    }
}