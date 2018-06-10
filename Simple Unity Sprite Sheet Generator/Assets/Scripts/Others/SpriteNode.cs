using UnityEngine;

public class SpriteNode
{
    /// <summary>
    /// The name of this sprite node.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The position of this sprite node.
    /// </summary>
    public Vector2Int Position { get; set; }

    /// <summary>
    /// The texture of this sprite node.
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Creates a new sprite node with the specified name, position and texture.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="position"></param>
    /// <param name="texture"></param>
    public SpriteNode(string name, Vector2Int position, Texture2D texture)
    {
        Name = name;
        Position = position;
        Texture = texture;
    }

    /// <summary>
    /// Creates an empty sprite node.
    /// </summary>
    public SpriteNode() { }
}
