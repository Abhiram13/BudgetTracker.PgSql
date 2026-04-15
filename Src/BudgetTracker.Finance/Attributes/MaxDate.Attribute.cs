using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BudgetTracker.Finance.Attributes;

internal class MaxDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null) return false;
        if (value is not DateOnly date) return false;

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);        

        return today >= date;
    }
}