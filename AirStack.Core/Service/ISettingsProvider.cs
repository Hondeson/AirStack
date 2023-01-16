namespace AirStack.Core.Service
{
    public interface ISettingsProvider
    {
        List<string> GetCodeRegexes();
    }
}