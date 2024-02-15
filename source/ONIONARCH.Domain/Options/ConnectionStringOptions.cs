using ONIONARCH.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONIONARCH.Domain.Options;

public sealed class ConnectionStringOptions : BaseConfig
{
    public string SampleDb { get; set; } = null!;
    public override string Section => "ConnectionStrings";
}
