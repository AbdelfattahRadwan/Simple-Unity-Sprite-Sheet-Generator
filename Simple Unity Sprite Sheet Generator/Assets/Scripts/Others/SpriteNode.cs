using System;
using UnityEngine;

[Serializable]
public class SpriteNode
{
    /// <summary>
    /// The name of this sprite node.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The position of this sprite node on the x-axis.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// The position of this sprite node on the y-axis.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// The horizontal scale of this sprite node.
    /// </summary>
    public float XScale { get; set; }

    /// <summary>
    /// The vertical scale of this sprite node.
    /// </summary>
    public float YScale { get; set; }

    /// <summary>
    /// The width of this sprite node.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// The height of this sprite node.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// The default width of this sprite node.
    /// </summary>
    public float DefaultWidth => texture != null ? texture.width : 0f;

    /// <summary>
    /// The default height of this sprite node.
    /// </summary>
    public float DefaultHeight => texture != null ? texture.height : 0f;

    /// <summary>
    /// Represents the texture data of this sprite node's texture.
    /// </summary>
    public byte[] TextureData { get; set; }

    /// <summary>
    /// The texture of this sprite node.
    /// </summary>
    [NonSerialized] public Texture2D texture;

    /// <summary>
    /// Initialized the node's texture from it's texture byte data.
    /// </summary>
    public void InitializeTexture()
    {
        texture = new Texture2D(32, 32, TextureFormat.RGBA32, false, false);
        texture.LoadImage(TextureData);
    }

    /// <summary>
    /// Creates a new sprite node with the specified name, position and texture.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="xScale"></param>
    /// <param name="yScale"></param>
    /// <param name="texture"></param>
    public SpriteNode(string name, float x, float y, float xScale, float yScale, float w, float h, Texture2D texture)
    {
        Name = name;
        X = x;
        Y = y;
        XScale = xScale;
        YScale = yScale;
        this.texture = texture;
        Width = w;
        Height = h;

        TextureData = texture.EncodeToPNG();
    }

    /// <summary>
    /// Creates a new sprite node.
    /// </summary>
    public SpriteNode() { }
}
