using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

//------------------------------------------------------------
// Types

public struct Bar
{
    public uint U;
}

public struct Baz
{
    public ulong L;
    public uint U;
    public ushort S;
    public byte B;
}

// Explicit layout without 'Size' allows this issue to emerge.
[StructLayout(LayoutKind.Explicit)]
public struct Test
{
    [FieldOffset(0)] public int A;
    [FieldOffset(4)] public Bar B;
    [FieldOffset(4)] public Baz C;
}

//------------------------------------------------------------
// Job to collect sizeof(Test)

[BurstCompile]
public unsafe struct FooJob : IJob
{
    NativeArray<int> _res;

    public static FooJob Create(NativeArray<int> res)
    {
        return new FooJob() {
            _res = res
        };
    }

    public void Execute()
    {
        _res[0] = sizeof(Test);
    }
}

//------------------------------------------------------------
// Show issue

public unsafe class Tests : MonoBehaviour
{
    void Start()
    {
        UnityEngine.Debug.LogWarning("SIZE = " + sizeof(Test));

        using (var foo = new NativeArray<int>(1, Allocator.Persistent))
        {
            var jh = FooJob.Create(foo).Schedule();
            jh.Complete();
            UnityEngine.Debug.LogWarning("BURST_SIZE = " + foo[0]);
        }
    }
}

//------------------------------------------------------------
