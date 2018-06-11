using System;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Represents a serializable sprite sheet.
/// </summary>
[Serializable]
public class SpriteSheet
{
    /// <summary>
    /// The name of this sprite sheet.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The width of this sprite sheet.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of this sprite sheet.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// The sprite nodes on the sprite sheet.
    /// </summary>
    public List<SpriteNode> SpriteNodes { get; set; }

    /// <summary>
    /// Creates a new sprite sheet with the specified name, width, height and texture set.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="textures"></param>
    public SpriteSheet(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;

        SpriteNodes = new List<SpriteNode>();
    }

    /// <summary>
    /// Creates a new sprite sheet.
    /// </summary>
    public SpriteSheet() { }
}
