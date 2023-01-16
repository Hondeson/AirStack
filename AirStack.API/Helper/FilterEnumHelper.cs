using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;

namespace AirStack.API.Helper
{
    [Flags]
    public enum StatusFilterEnum
    {
        Production = 1,
        Tests = 2,
        Dispatched = 4,
        Complaint = 8,
        ComplaintToSupplier = 16
    }

    public static class FilterEnumHelper
    {
        public static bool ValidateEnumAgainstMain(Type mainEnum, Type filterEnum)
        {
            var filterNames = Enum.GetNames(filterEnum);
            var mainNames = Enum.GetNames(mainEnum);

            if (filterNames.Length != mainNames.Length
                || !mainNames.All(x => filterNames.Contains(x)))
                return false;

            return true;
        }

        public static List<TMainEnum> GetMainEnumListFromFilter<TMainEnum, TFilterEnum>(TFilterEnum? filterEnum)
            where TMainEnum : Enum
            where TFilterEnum : struct, Enum
        {
            if (filterEnum is null || !filterEnum.HasValue)
                return new List<TMainEnum>();

            var names = filterEnum.ToString().Split(',', StringSplitOptions.TrimEntries);
            return Enum.GetNames(typeof(TMainEnum)).Where(mainEnumName => names.Any(name => name.Equals(mainEnumName))).Select(x => (TMainEnum)Enum.Parse(typeof(TMainEnum), x)).ToList();
        }
    }
}
