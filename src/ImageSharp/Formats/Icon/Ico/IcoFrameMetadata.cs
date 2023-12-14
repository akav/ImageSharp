// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.ImageSharp.Formats.Icon.Ico;

/// <summary>
/// IcoFrameMetadata. TODO: Remove base class and merge into this class.
/// </summary>
public class IcoFrameMetadata : IconFrameMetadata, IDeepCloneable<IcoFrameMetadata>, IDeepCloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IcoFrameMetadata"/> class.
    /// </summary>
    public IcoFrameMetadata()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IcoFrameMetadata"/> class.
    /// </summary>
    /// <param name="metadata">metadata</param>
    public IcoFrameMetadata(IconFrameMetadata metadata)
        : base(metadata)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IcoFrameMetadata"/> class.
    /// </summary>
    /// <param name="width">width</param>
    /// <param name="height">height</param>
    /// <param name="colorCount">colorCount</param>
    /// <param name="field1">field1</param>
    /// <param name="field2">field2</param>
    public IcoFrameMetadata(byte width, byte height, byte colorCount, ushort field1, ushort field2)
        : base(width, height, colorCount, field1, field2)
    {
    }

    /// <summary>
    /// Gets or sets the bits per pixel.
    /// TODO: This needs to be constrained and calculated using the metadata returned from the png/bmp.
    /// </summary>
    public ushort BitCount { get => this.Field2; set => this.Field2 = value; }

    /// <inheritdoc/>
    public override IcoFrameMetadata DeepClone() => new(this);
}
