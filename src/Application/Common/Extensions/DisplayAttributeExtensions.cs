using System.ComponentModel.DataAnnotations;

namespace Cfo.Cats.Application.Common.Extensions;

public static class DisplayAttributeExtensions
{
    public static string? HelperTextFor<TModel, TProperty>(
        this TModel _,
        Expression<Func<TModel, TProperty>> expression)
    {
        if (expression.Body is MemberExpression member)
        {
            return member.Member
                .GetCustomAttribute<DisplayAttribute>()?
                .GetDescription();
        }

        return null;
    }

    public static string? LabelFor<TModel, TProperty>(
        this TModel _,
        Expression<Func<TModel, TProperty>> expression)
    {
        if (expression.Body is MemberExpression member)
        {
            return member.Member
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
        }

        return null;
    }
}