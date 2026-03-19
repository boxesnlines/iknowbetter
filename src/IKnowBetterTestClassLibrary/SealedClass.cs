namespace IKnowBetterTestClassLibrary;

/// <summary>
/// This class is marked as sealed so can't usually be extended by consumers of the library.
/// By using IKnowBetter, we can unseal the class an allow it to be extended.
/// </summary>
public sealed class SealedClass
{
    /// <summary>
    /// This method can't usually be overridden because it is within a sealed class.
    /// </summary>
    /// <returns></returns>
    public string CantOverrideMe()
    {
        return "Can't override me!";
    }
}