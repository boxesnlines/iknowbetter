# iknowbetter
Code libraries often contain types that are private or sealed that you would really like to instantiate or extend regardless.

You don't have access to the source code so you can't modify the protection levels directly, and you're left with no option
other than to create your own implementation of the classes you want.

IKnowBetter provides a solution in these cases - it lets you declare that you know better than the library's original author
and makes classes and fields public and unsealed according to your requirements.

## Don't use this unless you actually Know Better
When the library author made a class private or sealed, they made a decision that you shouldn't be able to instantiate or extend them in your own code.

By using IKnowBetter, you are asserting that you know better than the original author, but you shouldn't be surprised if
changing the protection levels leads to unexpected results.

Be mindful when using this, and make sure you actually do know better than the original library author!

## How to use

1. Install the `BoxesNLines.IKnowBetter` NuGet package into your project.

2. Add a configuration file for IKnowBetter to the root of your project, called `IKnowBetter.jsonc`.

3. Populate the configuration file to determine which classes you want to make public, unseal, etc.

Commands available are: `UnsealClass`, `MakeClassPublic`, `FullyUnlockClass` and `MakeMethodPublic`.

Example configuration:
```
{
  "IKnowBetter": [
    {
      "IKnowBetter": "UnsealClass",
      "Assembly": "IKnowBetterClassLibrary",
      "Class":"IKnowBetterClassLibrary.SealedClass"
    },
    {
      "IKnowBetter": "MakeClassPublic",
      "Assembly": "IKnowBetterClassLibrary",
      "Class":"IKnowBetterClassLibrary.PrivateClass"
    },
    {
      "IKnowBetter": "FullyUnlockClass",
      "Assembly": "IKnowBetterClassLibrary",
      "Class":"IKnowBetterClassLibrary.LockedClass"
    },
    {
      "IKnowBetter": "MakeMethodPublic",
      "Assembly": "IKnowBetterClassLibrary",
      "Class":"IKnowBetterClassLibrary.ClassWithPrivateMethod",
      "Method": "GetString"
    }
  ]
}
```

4. Build your project.

The build logs should show IKnowBetter taking effect during the build. After the build, the classes you specified will be unlocked according to the configuration!

5. **IMPORTANT - Drop caches from your IDE**

Your IDE will probably cache the protection levels of the classes modified by IKnowBetter, causing it to continue to show warnings.

The caches should eventually update by themselves, but you probably don't want to wait. To refresh the caches:
- In Rider: `File > Invalidate Caches... > Invalidate and Restart`
- In Visual Studio: Delete the `C:\Users\<you>\AppData\Local\Microsoft\VisualStudio\<version>\ComponentModelCache` folder

# Warnings
If you Know Better you probably shouldn't need to be told these things, but here are some warnings and disclaimers:

- Running `dotnet restore --force` will retrieve a fresh copy of your referenced packages, reversing any changes made by IKnowBetter
- You can remove IKnowBetter by simply removing the package reference from your project, but the changes it made will persist until you run `dotnet restore --force` or otherwise replace the package DLL
- IKnowBetter tacks some metadata onto the DLL when it modifies it so it won't attempt to modify it multiple times and slow down your builds unnecessarily. You can force IKnowBetter to re-run by performing `dotnet restore --force`
- Don't try to modify types from your local source code - only target referenced libraries. IKnowBetter will ignore the contents of your project itself
- Don't try to modify types from project references - the build will likely fail due to the compiler locking the local project
- Don't try to modify types from .NET itself or from the Microsoft.Build namespace - the build will likely hang as it will hold locks on these
- Beware of changes introduced when you update your NuGet packages - the library's author may have changes the structure of the code
 