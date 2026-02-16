using FluentAssertions;

namespace AlertHub.Tests.Architecture;

public sealed class LayeringTests
{
    [Fact]
    public void ApplicationFiles_ShouldNotReferenceInfrastructureNamespace()
    {
        var repoRoot = FindRepoRoot();
        var applicationDir = Path.Combine(repoRoot, "AlertHub", "Application");
        var files = Directory.GetFiles(applicationDir, "*.cs", SearchOption.AllDirectories);

        var violations = files
            .Where(path => File.ReadAllText(path).Contains("using AlertHub.Infrastructure", StringComparison.Ordinal))
            .Select(path => Path.GetRelativePath(repoRoot, path))
            .ToList();

        violations.Should().BeEmpty("Application layer must not reference infrastructure namespaces.");
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var solutionPath = Path.Combine(current.FullName, "AlertHub.sln");
            if (File.Exists(solutionPath))
                return current.FullName;

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root from test runtime path.");
    }
}
