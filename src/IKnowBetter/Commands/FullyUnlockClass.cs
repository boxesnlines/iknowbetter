using Mono.Cecil;

namespace IKnowBetter.Commands;

public class FullyUnlockClass : IIKnowBetterCommand
{
    private readonly ConfigurationComand _command;
    public FullyUnlockClass(ConfigurationComand command)
    {
        _command = command;
    }

    public void Execute(List<(string path, AssemblyDefinition assembly)> referencedAssemblies, Action<string> logMessageAction)
    {
        foreach ((string path, AssemblyDefinition assembly) assembly in referencedAssemblies)
        {
            TypeDefinition? typeForChange = assembly.assembly.MainModule.GetType(_command.Class);
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
                
                assembly.assembly.Write(assembly.path);
                logMessageAction($"Class {typeForChange} successfully fully unlocked.");
                return;
            }
        }
    }
}