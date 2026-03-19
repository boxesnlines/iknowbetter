namespace IKnowBetterTestClassLibrary;

/// <summary>
/// This class has a private method that would usually only be accessible internally.
/// By using IKnowBetter, we can make this method public.
/// </summary>
public class ClassWithPrivateMethod
{
    private string GetString()
    {
        return "Accessible!";
    }
}