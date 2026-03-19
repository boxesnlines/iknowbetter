This is the class library package used by the IKnowBetter demo - it's only intended to be used for testing and demo purposes.

You can see an example of this in the [IKnowBetterDemo](https://github.com/boxesnlines/iknowbetter/tree/main/src/IKnowBetterDemo) project.

To use this to demo IKnowBetter, you should:
- Add this package to your project
- Verify that the members of this library are inaccessible, e.g. `PrivateClass` is not accessible to your code
- Add the `BoxesNLines.IKnowBetter` package to your project
- Add an `IKnowBetter.jsonc` file to the root of your project with configuration like:
```
[
  {
    "IKnowBetter": "UnsealClass", // UnsealClass/MakeClassPublic/MakeMethodPublic/FullyUnlockClass
    "Assembly": "IKnowBetterTestClassLibrary", // Typically the same name as the DLL
    "Class":"IKnowBetterTestClassLibrary.SealedClass" // Use the fully-qualified name of the class
  },
  {
    "IKnowBetter": "MakeClassPublic",
    "Assembly": "IKnowBetterTestClassLibrary",
    "Class":"IKnowBetterTestClassLibrary.PrivateClass"
  },
  {
    "IKnowBetter": "MakeClassPublic",
    "Assembly": "IKnowBetterTestClassLibrary",
    "Class":"IKnowBetterTestClassLibrary.InternalClass" // MakeClassPublic works on internal classes as well as private!
  },
  {
    "IKnowBetter": "FullyUnlockClass",
    "Assembly": "IKnowBetterTestClassLibrary",
    "Class":"IKnowBetterTestClassLibrary.LockedClass"
  },
  {
    "IKnowBetter": "MakeMethodPublic",
    "Assembly": "IKnowBetterTestClassLibrary",
    "Class":"IKnowBetterTestClassLibrary.ClassWithPrivateMethod",
    "Method": "GetString"
  }
]
```
- Build your project

Classes that were previously inaccessible should now be accessible and your code should build successfully even if you are accessing them.

Your IDE may continue to complain because it has cached the original protection levels.

- Drop your IDE's caches

After dropping the IDE's cache, it should recognise the new protection levels and stop complaining!