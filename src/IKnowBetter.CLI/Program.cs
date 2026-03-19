using IKnowBetter.CLI.Commands;
using Spectre.Console.Cli;

CommandApp app = new();
app.Configure(config =>
{
    config.SetApplicationName("bnl-iknowbetter");
    config.AddCommand<FullyUnlockClass>("fullyunlockclass");
    config.AddCommand<MakeClassPublic>("makeclasspublic");
    config.AddCommand<UnsealClass>("unsealclass");
    config.AddCommand<MakeMethodPublic>("makemethodpublic");
});

return app.Run(args);