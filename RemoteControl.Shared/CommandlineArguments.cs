using RemoteControl.Shared.Extensions;

namespace RemoteControl.Shared
{
    public class CommandlineArguments
    {
        public static Dictionary<string, string> Get() =>
            Environment.CommandLine.Matches("(?<=--)(.*?)(?= *--|$)")
                .ToDictionary(m => m.Match("^(.*?)(?= |$)"), m => m.Match("(?<= )(.*?)(?=$)"));
    }
}
