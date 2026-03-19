using Mono.Cecil;

namespace IKnowBetter.Commands;

/// <summary>
/// Make a method public.
/// Note that get/set properties are considered methods in this context.
/// </summary>
public class MakeMethodPublic : IIKnowBetterCommand
{
    private readonly ConfigurationComand _command;
    public MakeMethodPublic(ConfigurationComand command)
    {
        _command = command;
    }
    
    public void Execute(List<(string path, AssemblyDefinition assembly)> referencedAssemblies, Action<string> logMessageAction)
    {
        logMessageAction($"Attempting to make {_command.Class}.{_command.Method} public.");
        foreach ((string path, AssemblyDefinition assembly) assembly in referencedAssemblies)
        {
            logMessageAction($"Checking {assembly.assembly.Name}");
            TypeDefinition? typeForChange = assembly.assembly.MainModule.GetType(_command.Class);
            if (typeForChange is not null)
            {
                logMessageAction($"Found type {_command.Class}");
                MethodDefinition? method = typeForChange.Methods.SingleOrDefault(x => x.Name == _command.Method);
                if (method is not null)
                {
                    method.IsPublic = true;
                    assembly.assembly.Write(assembly.path);
                    logMessageAction($"Method {_command.Class}.{_command.Method} successfully made public.");
                }
            }
            else
            {
                logMessageAction($"Method {_command.Class}.{_command.Method} not found in assembly {assembly.path}.");
            }
        }
    }
}