namespace ONIONARCH.Presentation.API;

/// <summary>
/// Marker type identifying the Presentation.API assembly. Gives integration tests a stable,
/// explicitly named entry-point type argument for <c>WebApplicationFactory&lt;TEntryPoint&gt;</c>
/// without depending on the compiler-generated <c>Program</c> class from top-level statements.
/// </summary>
public sealed class ApiAssemblyMarker;