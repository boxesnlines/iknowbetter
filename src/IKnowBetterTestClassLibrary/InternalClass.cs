namespace IKnowBetterTestClassLibrary;

/// <summary>
/// This class is marked as internal, so wouldn't usually be accessible to consumers of the library.
/// By using IKnowBetter, we can make it public.
/// </summary>
internal class InternalClass
{
    public string Internal() => "Accessible!";
}