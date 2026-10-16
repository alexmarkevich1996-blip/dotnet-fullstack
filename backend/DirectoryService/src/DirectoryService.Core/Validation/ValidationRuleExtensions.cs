using System.Runtime.CompilerServices;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using FluentValidation;
using FluentValidation.Results;

namespace DirectoryService.Core.Validation;

public static class ValidationRuleExtensions
{
    public static IRuleBuilderOptionsConditions<T, string> MustBeValueObject<T, TValueObject>(
      this IRuleBuilder<T, string> ruleBuilder,
      Func<string, Result<TValueObject, ErrorList>> factory)
    {
      return ruleBuilder.Custom((value, context) =>
      {
          var result = factory(value);
          if (result.IsFailure)
          {
              foreach (var error in result.Error)
              {
                  var failure = new ValidationFailure(context.PropertyPath, error.Message) { CustomState = error };
                  context.AddFailure(failure);
              }
          }
      });
    }
    
    public static IRuleBuilderOptionsConditions<T, TProperty> MustSatisfy<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        Func<TProperty, bool> predicate,
        Func<TProperty, Error> errorFactory)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            if (!predicate(value))
            {
                var error = errorFactory(value);
                context.AddFailure(new ValidationFailure(context.PropertyPath, error.Message)
                    { CustomState = error });
            }
        });
    }
}