using System.ComponentModel;
using Mono.Cecil;
using Spectre.Console.Cli;

namespace IKnowBetter.CLI.Commands;

public class FullyUnlockClass : Command<FullyUnlockClass.Settings>
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
            InMemory = true
        };
        AssemblyDefinition? assembly = AssemblyDefinition.ReadAssembly(settings.DllPath, readerParameters);
        TypeDefinition? typeForChange = assembly.MainModule.GetType(settings.ClassName);
        if (typeForChange is not null)
        {
            typeForChange.IsPublic = true; // Make public
            typeForChange.Attributes &= ~TypeAttributes.Sealed; // Unseal
                
            // Internal types are modeled as nested private types, so we make all nested types public too
            // This side-effect could be unexpected, but it's likely necessary in any scenario where nested types are involved
            foreach (TypeDefinition? type in typeForChange.NestedTypes)
            {
                type.IsPublic = true;
            }
                
            // We remove sealed but not abstract/virtual, since those would very likely cause bad side effects
                
            // Now make all methods, fields and properties public too
            // Note that get/set properties are mapped as methods, so covered here
            foreach (MethodDefinition? method in typeForChange.Methods)
            {
                method.IsPublic = true;
            }

            foreach (FieldDefinition? field in typeForChange.Fields)
            {
                field.IsPublic = true;
            }
                
            assembly.Write(settings.DllPath);
            Console.WriteLine($"** IKB: Class {typeForChange} successfully fully unlocked.");
        }

        return 0;
    }
}