using System.Collections.Generic;
using UnityEngine;
using ZeroFormatter;

/// <summary>
/// Represents a serializable sprite sheet.
/// </summary>
[ZeroFormattable]
public class SpriteSheet
{
    /// <summary>
    /// The name of this sprite sheet.
    /// </summary>
    [Index(0)] public virtual string Name { get; set; }

    /// <summary>
    /// The width of this sprite sheet.
    /// </summary>
    [Index(1)] public virtual int Width { get; set; }

    /// <summary>
    /// The height of this sprite sheet.
    /// </summary>
    [Index(2)] public virtual int Height { get; set; }

    /// <summary>
    /// The textures included in this sprite sheet.
    /// </summary>
    [Index(3)] public virtual List<Texture2D> Textures { get; set; }

    /// <summary>
    /// The number of textures in this sprite sheet. (instead of writing <see cref="Textures"/>.Count)
    /// </summary>
    [Index(4)] public virtual int TextureCount => Textures.Count;

    /// <summary>
    /// The sprite nodes on the sprite sheet.
    /// </summary>
    [IgnoreFormat] public List<SpriteNode> SpriteNodes { get; set; }

    /// <summary>
    /// Arranges the sprite nodes on the sprite sheet according to their size.
    /// </summary>
    public void ArrangeNodes()
    {
        if (SpriteNodes.Count == 0)
        {
            return;
        }

        for (int i = 0; i < SpriteNodes.Count; ++i)
        {
            var currentNode = SpriteNodes[i];

            currentNode.Position = new Vector2(i * (currentNode.Texture.width / 2f), i * (currentNode.Texture.height / 2f));
        }
    }

    /// <summary>
    /// Creates a new sprite sheet with the specified name, width, height and texture set.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="textures"></param>
    public SpriteSheet(string name, int width, int height, List<Texture2D> textures)
    {
        Name = name;
        Width = width;
        Height = height;
        Textures = textures;
    }

    public SpriteSheet() { }
}
