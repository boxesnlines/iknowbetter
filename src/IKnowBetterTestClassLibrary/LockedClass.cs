namespace IKnowBetterTestClassLibrary;

/// <summary>
/// This class is marked as internal and has a private field and method.
/// By using IKnowBetter, we can make this class and all of its members public.
/// </summary>
internal class LockedClass
{
#pragma warning disable CS0414 // Field is assigned but its value is never used
    private string PrivateString = "Private";
#pragma warning restore CS0414 // Field is assigned but its value is never used
    private string GetString() => "Accessible!";
}