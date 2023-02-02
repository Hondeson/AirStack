using System.Text.RegularExpressions;

namespace AirStack.Core.Service.Validation
{
    public class ItemValidationService : IItemValidationService
    {
        readonly ISettingsProvider _settings;
        public ItemValidationService(ISettingsProvider settings)
        {
            _settings = settings;
        }

        bool _initialized = false;
        readonly List<Regex> _itemRegexes = new List<Regex>();
        public void Load()
        {
            _itemRegexes.Clear();

            var regStrings = _settings.GetCodeRegexes();
            foreach (string item in regStrings)
            {
                try
                {
                    _itemRegexes.Add(new Regex(item));
                }
                catch { }
            }

            _initialized = true;
        }

        public bool IsItemCodeValid(string itemCode)
        {
            if (!_initialized)
                Load();

            if (_itemRegexes.Count == 0)
                return true;

            return _itemRegexes.Any(x => x.IsMatch(itemCode));
        }

        public List<Regex> GetRegexes()
        {
            if (!_initialized)
                Load();

            return _itemRegexes;
        }
    }
}
