// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using UnityEngine;

namespace Cleverous.VaultSystem
{
    /// <summary>
    /// <para>Use on Fields and Properties to expose them to the Dashboard filtering system.</para>
    /// <para>Supports string, float and int types.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VaultFilterableAttribute : PropertyAttribute { }
}