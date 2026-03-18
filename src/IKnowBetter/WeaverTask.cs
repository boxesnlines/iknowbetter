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
        List<string> classesToMakePublic = ["IKnowBetterTestClassLibrary.PrivateClass", "IKnowBetterTestClassLibrary.InternalClass"];
        List<string> classesToUnseal = ["IKnowBetterTestClassLibrary.SealedClass"];
        List<string> classesToFullyUnlock = ["IKnowBetterTestClassLibrary.LockedClass"];
        List<(string, string)> methodsToMakePublic = [("IKnowBetterTestClassLibrary.ClassWithPrivateMethod","GetString")];
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"*** IKnowBetter - Code Weaving Started ({TargetAssemblyPath})", string.Empty, string.Empty, MessageImportance.High));

        InMemoryAssemblyResolver resolver = new();
        resolver.AddSearchDirectory(TargetAssemblyPath);

        ReaderParameters readerParameters = new()
        {
            AssemblyResolver = resolver,
            ReadingMode = ReadingMode.Immediate,
            InMemory = true
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

        // Make methods public
        foreach ((string className, string methodName) methodToMakePublic in methodsToMakePublic)
        {
            MakeMethodPublic(methodToMakePublic.className, methodToMakePublic.methodName);
        }

        // Make classes public
        foreach (string className in classesToMakePublic)
        {
            MakeClassPublic(className);
        }

        // Unseal classes
        foreach (string className in classesToUnseal)
        {
            UnsealClass(className);
        }
        
        // Fully unlock classes
        foreach (string className in classesToFullyUnlock)
        {
            FullyUnlockClass(className);
        }
        
        foreach ((string path, AssemblyDefinition assembly) referencedAssembly in _referencedAssembles)
        {
            referencedAssembly.assembly.Dispose();
        }
        
        return true;
    }

    /// <summary>
    /// Make a class public so it can be instantiated by a consumer.
    /// </summary>
    /// <param name="classToMakePublic">Fully qualified name of the class, including namespace.</param>
    private void MakeClassPublic(string classToMakePublic) {
        foreach ((string path, AssemblyDefinition assembly) assembly in _referencedAssembles)
        {
            TypeDefinition? typeForChange = assembly.assembly.MainModule.GetType(classToMakePublic);
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
                LogMessage($"Class {classToMakePublic} successfully made public.");
            }
        }
    }

    /// <summary>
    /// Make a method public.
    /// Note that get/set properties are considered methods in this context.
    /// </summary>
    /// <param name="className">Fully qualified class name (including namespace)</param>
    /// <param name="methodName">Method name</param>
    private void MakeMethodPublic(string className, string methodName)
    {
        LogMessage($"Attempting to make {className}.{methodName} public.");
        foreach ((string path, AssemblyDefinition assembly) assembly in _referencedAssembles)
        {
            LogMessage($"Checking {assembly.assembly.Name}");
            TypeDefinition? typeForChange = assembly.assembly.MainModule.GetType(className);
            if (typeForChange is not null)
            {
                    LogMessage($"Found type {className}");
                    //var resolvedType = typeReference.Resolve();
                    MethodDefinition? method = typeForChange.Methods.SingleOrDefault(x => x.Name == methodName);
                    if (method is not null)
                    {
                        method.IsPublic = true;
                        assembly.assembly.Write(assembly.path);
                        LogMessage($"Method {className}.{methodName} successfully made public.");
                    }

                    return; // Return if we found the type - even if we didn't find the method we're not going to
            }
        }
    }

    /// <summary>
    /// Fully unlock a class, including making it public, unsealing it and making all methods and fields public.
    /// </summary>
    /// <param name="classToUnseal">Fully qualified name of class to unlock (including namespace)</param>
    private void FullyUnlockClass(string classToUnseal)
    {
        foreach ((string path, AssemblyDefinition assembly) assembly in _referencedAssembles)
        {
            TypeDefinition? typeForChange = assembly.assembly.MainModule.GetType(classToUnseal);
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
                LogMessage($"Class {typeForChange} successfully fully unlocked.");
                return;
            }
        }
    }

    /// <summary>
    /// Removed "sealed" from a class so that it can be extended.
    /// </summary>
    /// <param name="classToUnlock">Full name of class, including namespace</param>
    private void UnsealClass(string classToUnlock)
    {
        foreach ((string path, AssemblyDefinition assembly) assembly in _referencedAssembles)
        {
            TypeDefinition? typeForChange = assembly.assembly.MainModule.GetType(classToUnlock);
            if (typeForChange is not null)
            {
                typeForChange.Attributes &= ~TypeAttributes.Sealed;
                assembly.assembly.Write(assembly.path);
                LogMessage($"Class {typeForChange} successfully unsealed.");
            }
        }
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

    private sealed class InMemoryAssemblyResolver : DefaultAssemblyResolver
    {
        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            ReaderParameters readerParameters = new()
            {
                AssemblyResolver = this,
                ReadingMode = ReadingMode.Immediate,
                InMemory = true
            };

            return base.Resolve(name, readerParameters);
        }
    }
}