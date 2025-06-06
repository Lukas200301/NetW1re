using NetW1reAvalonia.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NetW1reAvalonia.Core.Compairers
{
    public class DeviceEqualityComparer : IEqualityComparer<Device>
    {
        public bool Equals(Device? x, Device? y)
        {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);

            return x.Mac.Equals(y.Mac);
        }

        public int GetHashCode([DisallowNull] Device obj)
        {
            return obj.Mac.GetHashCode();
        }
    }
}
