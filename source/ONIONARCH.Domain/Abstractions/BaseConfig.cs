using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONIONARCH.Domain.Abstractions;

public abstract class BaseConfig
{
    public abstract string Section { get; }
}
