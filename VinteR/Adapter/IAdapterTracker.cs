using System;
using System.Collections.Generic;
using System.Numerics;
using VinteR.Model;

namespace VinteR.Adapter
{
    public interface IAdapterTracker
    {
        Vector3 Locate(string name);
    }
}