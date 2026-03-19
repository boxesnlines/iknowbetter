using Mono.Cecil;

namespace IKnowBetter.Commands;

/// <summary>
/// Make a class public so it can be instantiated by a consumer.
/// </summary>
public class MakeClassPublic : IIKnowBetterCommand
{
    private readonly ConfigurationComand _command;
    public MakeClassPublic(ConfigurationComand command)
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
                typeForChange.IsPublic = true;
                
                // Internal types are modeled as nested private types, so we make all nested types public too
                // This side-effect could be unexpected, but it's likely necessary in any scenario where nested types are involved
                foreach (TypeDefinition? type in typeForChange.NestedTypes)
                {
                    type.IsPublic = true;
                }
                
                assembly.assembly.Write(assembly.path);
                logMessageAction($"Class {_command.Class} successfully made public.");
            }
        }
    }
}