using System.Text.RegularExpressions;

namespace AirStack.Core.Service.Validation
{
    public interface IItemValidationService
    {
        List<Regex> GetRegexes();
        bool IsItemCodeValid(string itemCode);
        void Load();
    }
}