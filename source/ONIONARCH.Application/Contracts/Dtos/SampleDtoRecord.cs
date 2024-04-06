using MassTransit.Saga;
using ONIONARCH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ONIONARCH.Application.Contracts.Dtos
{
    public sealed record SampleDtoRecord(int Id, string? SomeValue)
    {
        public static SampleDtoRecord Create(SampleEntity1 entity1, SampleEntity2 entity2)
        {
            return new SampleDtoRecord(entity1.SampleId1, entity2.SampleString2);
        }
    }
}
