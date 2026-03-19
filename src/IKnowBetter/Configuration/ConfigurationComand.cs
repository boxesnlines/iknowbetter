using System.Text.Json.Serialization;
using IKnowBetter.Commands;

namespace IKnowBetter;

public class ConfigurationComand
{
    public IKnowBetterCommand CommandType => Enum.Parse<IKnowBetterCommand>(CommandName);

    [JsonPropertyName("IKnowBetter")]
    public string CommandName { get; set; }

    [JsonPropertyName("Assembly")]
    public string Assembly { get; set; }

    [JsonPropertyName("Class")]
    public string? Class { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Method")]
    public string? Method { get; set; }

    public IIKnowBetterCommand Command
    {
        get
        {
            switch (CommandType)
            {
                case IKnowBetterCommand.UnsealClass: return new UnsealClass(this);
                case IKnowBetterCommand.MakeClassPublic: return new MakeClassPublic(this);
                case IKnowBetterCommand.MakeMethodPublic: return new MakeMethodPublic(this);
                case IKnowBetterCommand.FullyUnlockClass: return new FullyUnlockClass(this);
                default: throw new Exception();
            }
        }
    }
}