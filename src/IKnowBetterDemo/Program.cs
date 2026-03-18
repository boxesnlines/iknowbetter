using IKnowBetterTestClassLibrary;

// SealedClass is sealed in the source
UnsealedClass unsealedClassInstance = new();
Console.WriteLine(unsealedClassInstance.CantOverrideMe());

// The GetString method is private in the source
ClassWithPrivateMethod instance = new();
Console.WriteLine(instance.GetString());

// InternalClass is internal in the source
InternalClass internalClassInstance = new();
Console.WriteLine(internalClassInstance.Internal());

// LockedClass has private methods and fields
LockedClass lockedClassInstance = new();
Console.WriteLine(lockedClassInstance.PrivateString);
Console.WriteLine(lockedClassInstance.GetString());

// IAmAPrivateType is private in the source
PrivateClass.IAmAPrivateType privateTypeInstance = new();
Console.WriteLine(privateTypeInstance.GetString());

class UnsealedClass : SealedClass
{
    public new string CantOverrideMe()
    {
        return "Yes I can";
    }
}