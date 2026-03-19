using Mono.Cecil;

namespace IKnowBetter.Commands;

public interface IIKnowBetterCommand
{
    public void Execute(List<(string path, AssemblyDefinition assembly)> referencedAssemblies, Action<string> logMessageAction);
}