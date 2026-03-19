using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Build.Framework;
using Mono.Cecil;
using Task = Microsoft.Build.Utilities.Task;

namespace IKnowBetter;

/// <summary>
/// This code weaver is a <see cref="Task"/> from Microsoft.Build.Utilities.
/// This allows it to integrate with the build process.
/// </summary>
public class WeaverTask : Task
{
    public string TargetAssemblyPath { get; set; } = string.Empty;
    public ITaskItem[] ReferencePaths { get; set; } = Array.Empty<ITaskItem>();
    public string ConfigPath { get; set; } = string.Empty;

    /// <summary>
    /// Collection to store referenced assemblies, resolved at the start of the execution process.
    /// Note that IKB only targets references assemblies - it doesn't attempt to modify the assembly being built.
    /// </summary>
    private readonly List<(string path, AssemblyDefinition assembly)> _referencedAssembles = new();
    
    /// <summary>
    /// Entrypoint for the task. This will be called on build, according to the setup in the .csproj file.
    /// </summary>
    /// <returns></returns>
    public override bool Execute()
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"*** IKnowBetter - Code Weaving Started ({TargetAssemblyPath})", string.Empty, string.Empty, MessageImportance.High));

        // Set up code weaving
        ReaderParameters readerParameters = new()
        { 
            ReadingMode = ReadingMode.Immediate,
            InMemory = true,
        };
        
        foreach (ITaskItem referencePath in ReferencePaths)
        {
            string referenceAssemblyPath = referencePath.ItemSpec;
            if (referenceAssemblyPath.Contains("IKnowBetterTestClassLibrary")) // Avoid recursing on IKB and Microsoft.Build
            {
                LogMessage($"Adding {referencePath.ItemSpec} to referenced assemblies.");
                _referencedAssembles.Add((referencePath.ItemSpec, AssemblyDefinition.ReadAssembly(referenceAssemblyPath, readerParameters)));
            }
        }

        if (_referencedAssembles.Count == 0)
        {
            LogMessage("No matching referenced assemblies found to weave.");
            return true;
        }
        
        // Now retrieve configuration and execute commands
        if (!File.Exists(ConfigPath)) throw new ConfigurationException("Configuration file not found. Please add IKnowBetter.jsonc to the root of the project being built.");
        List<ConfigurationComand>? configuration = JsonSerializer.Deserialize<List<ConfigurationComand>>(File.ReadAllText(ConfigPath), new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip
        });
        foreach (ConfigurationComand command in configuration)
        {
            command.Command.Execute(_referencedAssembles, LogMessage);
        }
        
        // Now tidy up after ourselves
        foreach ((string path, AssemblyDefinition assembly) referencedAssembly in _referencedAssembles)
        {
            referencedAssembly.assembly.Dispose();
        }
        
        return true;
    }
    
    /// <summary>
    /// Save message to the build log.
    /// Messages are prefixed with ** IKB: to indicate their source.
    /// </summary>
    /// <param name="message"></param>
    private void LogMessage(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"** IKB: {message}", string.Empty, string.Empty, MessageImportance.High));
    }
}