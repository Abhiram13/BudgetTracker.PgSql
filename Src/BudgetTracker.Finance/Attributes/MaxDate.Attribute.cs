using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BudgetTracker.Finance.Attributes;

public class MaxDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        // string date = (string?)value ?? throw new NullReferenceException($"{value} from MaxDateAttribute is required, but was null");
        // Regex regex = new Regex(@"^\d{4}-\d{2}-\d{2}$");
        // Match match = regex.Match(date);

        // if (!match.Success) return false;

        // DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        // DateTime now = DateTime.Now;
        // return now > dateTime;

        if (value is null) return false;
        if (value is not DateOnly date) return false;

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);        

        return today > date;
    }
}