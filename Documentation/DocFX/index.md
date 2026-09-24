---
_layout: landing
---

# ONIONARCH.Template

A .NET reference implementation of Onion Architecture with CQRS dispatch and swappable
persistence backends (SQL Server, PostgreSQL, MySQL).

![Onion Architecture](../Images/OnionArchitecture.png)

## API reference

The **API Reference** (top navigation) is generated from the XML documentation comments (`///`)
in the source code. Entry points by layer:

| Layer          | Namespace                                                   |
|----------------|-------------------------------------------------------------|
| Domain         | <xref:ONIONARCH.Domain.Entities>                            |
| Application    | <xref:ONIONARCH.Application>                                |
| Persistence    | <xref:ONIONARCH.Persistence>                                |
| Infrastructure | <xref:ONIONARCH.Infrastructure>                             |
| Presentation   | <xref:ONIONARCH.Presentation.API>, <xref:ONIONARCH.Presentation.Web>, <xref:ONIONARCH.Presentation.Console> |

Source, build instructions, and versioning details are in the
[repository README](https://github.com/Vercuski/ONIONARCH.Template#readme).
