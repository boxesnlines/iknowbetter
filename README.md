# iknowbetter
IKnowBetter allows you to adjust the protection levels of 3rd party libraries without needing access to their source.

- You can do this on a one-time-only basis by using [the CLI](src/IKnowBetter.CLI/README.md) to patch the library's DLL.
- You can integrate IKnowBetter with your project's build using [the NuGet package](src/IKnowBetter/README.md). 

For information on how to use the NuGet package, refer to [the README](src/IKnowBetter/README.md).

# Remember to clear your caches!
If you're here to complain that your IDE is still complaining after IKnowBetter made something public, it's very likely
that you're just seeing caching in your IDE.

Drop your IDE's caches after running IKnowBetter and it _should_ stop complaining!

# Building & Packaging
Everything here should build from source with the standard `dotnet build` command.

To package and publish, run `dotnet pack`, then `