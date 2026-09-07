using System.Net;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using PetstoreRestsharp.Core.Extensions;
using RestSharp;

namespace PetstoreRestsharp.Tests.Assertions;

/// <summary>
/// Custom FluentAssertions assertions for RestResponse.
/// Satisfies the DRY principle by moving repeated status/deserialization
/// checks into one place while keeping the fluent, readable API.
/// </summary>
public sealed class RestResponseAssertions : ReferenceTypeAssertions<RestResponse, RestResponseAssertions>
{
    public RestResponseAssertions(RestResponse subject) : base(subject, AssertionChain.GetOrCreate())
    {
    }

    protected override string Identifier => "RestResponse";

    public AndConstraint<RestResponseAssertions> HaveStatusCode(
        HttpStatusCode expected, string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.StatusCode == expected)
            .FailWith("Expected the response to have status code {0}{reason}, but found {1} (body: {2}).",
                expected, Subject.StatusCode, Subject.Content);

        return new AndConstraint<RestResponseAssertions>(this);
    }

    public AndConstraint<RestResponseAssertions> BeSuccessful(
        string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsSuccessful)
            .FailWith("Expected the response to be successful{reason}, but it was {0} (body: {1}).",
                Subject.StatusCode, Subject.Content);

        return new AndConstraint<RestResponseAssertions>(this);
    }

    public AndWhichConstraint<RestResponseAssertions, T> DeserializeAs<T>(
        string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrWhiteSpace(Subject.Content))
            .FailWith("Expected a response body to deserialize to {0}{reason}, but the body was empty.",
                typeof(T));

        var value = Subject.DeserializeOrThrow<T>();
        return new AndWhichConstraint<RestResponseAssertions, T>(this, value);
    }
}

public static class RestResponseAssertionExtensions
{
    public static RestResponseAssertions Should(this RestResponse subject) => new(subject);
}