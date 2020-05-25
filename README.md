# Issue

> Fixed: https://issuetracker.unity3d.com/issues/the-sizeof-of-struct-with-structlayout-returns-a-smaller-value-in-the-burst-compiled-code-compared-to-the-mono-compiled-code

Test code here: [Assets/Scripts/Tests.cs](Assets/Scripts/Tests.cs)

> Given these structs

```
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
```

> sizeof(Test) is different (and incorrect) when compiled using burst

Program output when burst compiled

```
SIZE = 24
BURST_SIZE = 16
```

Program output when burst compilation disabled OR when using Unity.Burst: 1.1.2

```
SIZE = 24
BURST_SIZE = 24
```


## Initially Reproduced In

- Unity: 2019.3.0f3
- Unity.Burst: 1.3.0-preview.9

> I do not believe Collections is relevent to the bug, but as we use NativeArray it was worth


## Earliest version that shows problem behaviour

- Unity: 2019.3.0f3
- Unity.Burst: 1.1.3-preview.3

> To clarify: Unity.Burst: 1.1.2 does not show this issue


## Most recent version that shows problem behaviour

- Unity: 2020.1.0.b6
- Unity.Burst: 1.3.0-preview.10


## Theory

It could be related to an entry in the changelog entry from `[1.1.3-preview.1] - 2019-08-26`:

> Improve codegen for structs with explicit layout and overlapping fields.

We can set an explicit size on the `Test` struct at which point both .net and burst agree on the size.


## Complications

Make sure you restart unity after package changes. I've seen cases where the job was not recompiled when I would have expected after such changes.
