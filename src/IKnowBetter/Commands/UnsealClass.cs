using Mono.Cecil;

namespace IKnowBetter.Commands;

public class UnsealClass : IIKnowBetterCommand
{
    private readonly ConfigurationComand _command;
    public UnsealClass(ConfigurationComand command)
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
                typeForChange.Attributes &= ~TypeAttributes.Sealed;
                assembly.assembly.Write(assembly.path);
                logMessageAction($"Class {typeForChange} successfully unsealed.");
            }
        }
    }
}