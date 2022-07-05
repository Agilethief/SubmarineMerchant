// (c) Copyright Cleverous 2022. All rights reserved.

#if !MIRROR && !FISHNET
namespace Cleverous.NetworkImposter
{
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Field)]
    public class SyncVarAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TargetRpcAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ServerAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ServerCallbackAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ClientAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ClientCallbackAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ClientRpcAttribute : System.Attribute
    {
    }

    public class SceneAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ShowInInspectorAttribute : System.Attribute
    {
    }
}
#endif