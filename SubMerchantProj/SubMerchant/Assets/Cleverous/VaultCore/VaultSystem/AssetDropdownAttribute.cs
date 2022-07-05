﻿// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using UnityEngine;

namespace Cleverous.VaultSystem
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Property)]
    public class AssetDropdownAttribute : PropertyAttribute
    {
        public Type SourceType { get; private set; }
        public AssetDropdownAttribute(Type sourceType) { SourceType = sourceType; }
    }
}