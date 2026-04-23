using System.Text.RegularExpressions;
using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Services;

public class NaturalLanguageParser
{
    public QueryParams Parse(string query)
    {
        var result = new QueryParams();
        var lower = query.ToLower();

        // Gender
        if (Regex.IsMatch(lower, @"\b(males?|men|boys?)\b")) result.Gender = "male";
        else if (Regex.IsMatch(lower, @"\b(females?|women|girls?)\b")) result.Gender = "female";

        // Age group
        if (Regex.IsMatch(lower, @"\badults?\b")) result.AgeGroup = "adult";
        else if (Regex.IsMatch(lower, @"\b(teens?|teenagers?)\b")) result.AgeGroup = "teenager";
        else if (Regex.IsMatch(lower, @"\b(children?|kids?)\b")) result.AgeGroup = "child";
        else if (Regex.IsMatch(lower, @"\b(seniors?|elderly)\b")) result.AgeGroup = "senior";

        // Young (16-24)
        if (Regex.IsMatch(lower, @"\byoung\b")) { result.MinAge = 16; result.MaxAge = 24; }

        // People from [country] - FIXED: use 'lower' not 'lowerQuery'
        var peopleFromMatch = Regex.Match(lower, @"people from\s+([a-z\s]+)");
        if (peopleFromMatch.Success)
        {
            var country = peopleFromMatch.Groups[1].Value.Trim();
            result.CountryId = GetCountryCode(country);
        }

        // Male and female teenagers above 17 - FIXED: use 'lower'
        if (Regex.IsMatch(lower, @"(male|female).*?(teenagers?)", RegexOptions.IgnoreCase))
        {
            result.AgeGroup = "teenager";
            // Don't set gender for "male and female" - leave it null to include both
        }

        // Age comparisons
        var above = Regex.Match(lower, @"above\s+(\d+)|over\s+(\d+)");
        if (above.Success) result.MinAge = int.Parse(above.Groups[1].Success ? above.Groups[1].Value : above.Groups[2].Value);

        var below = Regex.Match(lower, @"below\s+(\d+)|under\s+(\d+)");
        if (below.Success) result.MaxAge = int.Parse(below.Groups[1].Success ? below.Groups[1].Value : below.Groups[2].Value);

        // Country mapping
        var countryMatch = Regex.Match(lower, @"from\s+([a-z\s]+)");
        if (countryMatch.Success)
        {
            var country = countryMatch.Groups[1].Value.Trim();
            var code = GetCountryCode(country);
            if (code != null) result.CountryId = code;
        }

        return result;
    }

    private string? GetCountryCode(string countryName)
    {
        var countryMap = new Dictionary<string, string>
        {
            {"nigeria", "NG"}, {"kenya", "KE"}, {"south africa", "ZA"}, {"tanzania", "TZ"},
            {"uganda", "UG"}, {"ethiopia", "ET"}, {"angola", "AO"}, {"ghana", "GH"},
            {"morocco", "MA"}, {"egypt", "EG"}, {"algeria", "DZ"}, {"sudan", "SD"},
            {"rwanda", "RW"}, {"zambia", "ZM"}, {"zimbabwe", "ZW"}, {"mali", "ML"},
            {"senegal", "SN"}, {"congo", "CD"}, {"france", "FR"}, {"united kingdom", "GB"},
            {"united states", "US"}, {"india", "IN"}, {"brazil", "BR"}, {"china", "CN"},
            {"japan", "JP"}, {"angola", "AO"}, {"cameroon", "CM"}, {"canada", "CA"},
            {"australia", "AU"}, {"germany", "DE"}
        };

        return countryMap.TryGetValue(countryName, out var code) ? code : null;
    }

    public bool CanInterpret(QueryParams q) =>
        q.Gender != null || q.AgeGroup != null || q.CountryId != null || q.MinAge.HasValue || q.MaxAge.HasValue;
}