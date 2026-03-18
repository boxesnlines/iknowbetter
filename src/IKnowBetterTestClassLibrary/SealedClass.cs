namespace IKnowBetterTestClassLibrary;

public sealed class SealedClass
{
    public string CantOverrideMe()
    {
        return "Can't override me!";
    }
}