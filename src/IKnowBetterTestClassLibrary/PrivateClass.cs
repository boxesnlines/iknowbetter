namespace IKnowBetterTestClassLibrary;

/// <summary>
/// This class contains a private sub-type, which usually wouldn't be accessible to consumers of the library.
/// By using IKnowBetter we can make this class public.
/// </summary>
public class PrivateClass
{
    private class IAmAPrivateType()
    {
        public string GetString()
        {
            return "Accessible!";
        }
    }
}