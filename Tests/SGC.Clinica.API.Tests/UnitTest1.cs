using System.IO;

namespace SGC.Clinica.API.Tests;

public class UnitTest1
{
    private static readonly string SolutionRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    [Fact]
    public void Solution_ShouldReferenceOnlyTheOfficialApiProject()
    {
        var solutionPath = Path.Combine(SolutionRoot, "SGC.Clinica.sln");
        var solutionContent = File.ReadAllText(solutionPath);

        Assert.Contains(@"src\SGC.Clinica.API\SGC.Clinica.API.csproj", solutionContent);
        Assert.DoesNotContain(@"SGC.Clinica.Api\SGC.Clinica.Api.csproj", solutionContent);
    }

    [Fact]
    public void ApiProject_ShouldDependOnApplicationAndInfrastructureOnly()
    {
        var apiProjectPath = Path.Combine(
            SolutionRoot,
            "src",
            "SGC.Clinica.API",
            "SGC.Clinica.API.csproj");

        var projectContent = File.ReadAllText(apiProjectPath);

        Assert.Contains(@"..\SGC.Clinica.Application\SGC.Clinica.Application.csproj", projectContent);
        Assert.Contains(@"..\SGC.Clinica.Infrastructure\SGC.Clinica.Infrastructure.csproj", projectContent);
        Assert.DoesNotContain(@"..\SGC.Clinica.Domain\SGC.Clinica.Domain.csproj", projectContent);
    }
}
