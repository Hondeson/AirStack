namespace AirStack.Core.Services
{
    public interface ISettingsProvider
    {
        List<string> GetCodeRegexes();
    }
}