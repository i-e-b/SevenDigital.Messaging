using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

// Note: the assembly version is always 0, because otherwise .Net's 
// stupid versioning breaks interface equivalence.
[assembly: AssemblyFileVersion("0.0.0")]
[assembly: AssemblyVersion("0.0.0")]

[assembly: AssemblyTitle("SevenDigital.Messaging")]
[assembly: AssemblyDescription("A distributed contracts-based sender/handler messaging system")]

[assembly: AssemblyProduct("SevenDigital.Messaging")]

[assembly: NeutralResourcesLanguageAttribute("en")]
[assembly: InternalsVisibleTo("SevenDigital.Messaging.Unit.Tests")]
[assembly: InternalsVisibleTo("SevenDigital.Messaging.Integration.Tests")]
