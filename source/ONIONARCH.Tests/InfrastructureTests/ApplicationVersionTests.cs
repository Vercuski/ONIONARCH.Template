using ONIONARCH.Infrastructure.Versioning;
using System.Reflection;

namespace ONIONARCH.Tests.InfrastructureTests;

/// <summary>
/// Unit tests for <see cref="ApplicationVersion"/>.
/// </summary>
[TestFixture]
public class ApplicationVersionTests
{
    /// <summary>
    /// Verifies that build metadata is split off a release version.
    /// </summary>
    [Test]
    public void FromInformationalVersion_Should_SplitBuildMetadata_FromReleaseVersion()
    {
        var version = ApplicationVersion.FromInformationalVersion("1.4.0+2057147a6910abdd18c2360adb13dea4c2cb3df6");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(version.SemanticVersion, Is.EqualTo("1.4.0"));
            Assert.That(version.BuildMetadata, Is.EqualTo("2057147a6910abdd18c2360adb13dea4c2cb3df6"));
            Assert.That(version.InformationalVersion, Is.EqualTo("1.4.0+2057147a6910abdd18c2360adb13dea4c2cb3df6"));
            Assert.That(version.IsPreRelease, Is.False);
        }
    }

    /// <summary>
    /// Verifies that a MinVer height-based pre-release version is recognized as a pre-release.
    /// </summary>
    [Test]
    public void FromInformationalVersion_Should_DetectPreRelease()
    {
        var version = ApplicationVersion.FromInformationalVersion("1.4.1-alpha.0.3+2057147");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(version.SemanticVersion, Is.EqualTo("1.4.1-alpha.0.3"));
            Assert.That(version.BuildMetadata, Is.EqualTo("2057147"));
            Assert.That(version.IsPreRelease, Is.True);
        }
    }

    /// <summary>
    /// Verifies that a version without build metadata is returned unchanged.
    /// </summary>
    [Test]
    public void FromInformationalVersion_Should_HandleVersionWithoutBuildMetadata()
    {
        var version = ApplicationVersion.FromInformationalVersion("2.0.0-rc.1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(version.SemanticVersion, Is.EqualTo("2.0.0-rc.1"));
            Assert.That(version.BuildMetadata, Is.Null);
            Assert.That(version.ToString(), Is.EqualTo("2.0.0-rc.1"));
        }
    }

    /// <summary>
    /// Verifies that an empty version string is rejected.
    /// </summary>
    [Test]
    public void FromInformationalVersion_Should_Throw_WhenEmpty()
    {
        Assert.Throws<ArgumentException>(() => ApplicationVersion.FromInformationalVersion(" "));
    }

    /// <summary>
    /// Verifies that <see cref="ApplicationVersion.Current"/> reflects the version MinVer stamped on
    /// the build, and that it is a valid SemVer 2.0 version.
    /// </summary>
    [Test]
    public void Current_Should_MatchStampedInformationalVersion_AndBeValidSemVer()
    {
        var stamped = typeof(ApplicationVersion).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ApplicationVersion.Current.InformationalVersion, Is.EqualTo(stamped));
            Assert.That(ApplicationVersion.Current.SemanticVersion, Does.Match(SemVerPattern));
        }
    }

    /// <summary>
    /// The official SemVer 2.0 regular expression (https://semver.org), without build metadata.
    /// </summary>
    private const string SemVerPattern =
        @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)"
        + @"(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?$";
}
