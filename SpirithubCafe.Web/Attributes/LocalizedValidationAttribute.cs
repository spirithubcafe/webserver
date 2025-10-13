using System.ComponentModel.DataAnnotations;
using SpirithubCafe.Application.Interfaces;

namespace SpirithubCafe.Web.Attributes;

public class LocalizedRequiredAttribute : RequiredAttribute
{
    private readonly string _resourceKey;

    public LocalizedRequiredAttribute(string resourceKey)
    {
        _resourceKey = resourceKey;
    }

    public override string FormatErrorMessage(string name)
    {
        return ErrorMessage ?? $"{name} is required";
    }
}

public class LocalizedStringLengthAttribute : StringLengthAttribute
{
    private readonly string _resourceKey;

    public LocalizedStringLengthAttribute(int maximumLength, string resourceKey) : base(maximumLength)
    {
        _resourceKey = resourceKey;
    }

    public override string FormatErrorMessage(string name)
    {
        return ErrorMessage ?? $"{name} must be between {MinimumLength} and {MaximumLength} characters";
    }
}

public class LocalizedCompareAttribute : CompareAttribute
{
    private readonly string _resourceKey;

    public LocalizedCompareAttribute(string otherProperty, string resourceKey) : base(otherProperty)
    {
        _resourceKey = resourceKey;
    }

    public override string FormatErrorMessage(string name)
    {
        return ErrorMessage ?? $"{name} and {OtherProperty} do not match";
    }
}