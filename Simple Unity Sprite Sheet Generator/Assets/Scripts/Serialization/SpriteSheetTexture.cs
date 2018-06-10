using ZeroFormatter;

/// <summary>
/// Represents a serializable texture used when creating sprite sheets.
/// </summary>
[ZeroFormattable]
public class SpriteSheetTexture
{
    /// <summary>
    /// The name of the texture.
    /// </summary>
    [Index(0)] public virtual string Name { get; set; }

    /// <summary>
    /// The width of the texture.
    /// </summary>
    [Index(1)] public virtual int Width { get; set; }

    /// <summary>
    /// The height of the texture.
    /// </summary>
    [Index(2)] public virtual int Height { get; set; }

    /// <summary>
    /// The colors (or pixels) of the texture.
    /// </summary>
    [Index(3)] public virtual byte[] Pixels { get; set; }

    /// <summary>
    /// Creates a new texture with the specified name, width, height and color (or pixel) data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="pixels"></param>
    public SpriteSheetTexture(string name, int width, int height, byte[] pixels)
    {
        Name = name;
        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public SpriteSheetTexture() { }
}
