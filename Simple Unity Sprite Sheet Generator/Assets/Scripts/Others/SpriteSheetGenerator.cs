using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SpriteSheetGenerator
{
    /// <summary>
    /// Generates a sprite sheet given the data and nodes.
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="spriteNodes"></param>
    /// <returns></returns>
    public static Texture2D RenderToTexture(SpriteSheet sheet, List<SpriteNode> spriteNodes, OutputImageFormat format)
    {
        // Width of the current sprite sheet.
        var w = sheet.Width;

        // Height of the current sprite sheet.
        var h = sheet.Height;

        // Check if the sheet surface area = 0.
        if (w < 1 || h < 1)
        {
            return new Texture2D(1, 1);
        }

        // Create a new texture and set it's width and height to the sheet's width and height.
        var texture = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            // Set it's name to sheet name.
            name = sheet.Name,
        };

        // Create a new color array.
        var transparentBG = new Color[w * h];
        
        // Loop through the array and populate it with a transparent color. 
        for (int xy = 0; xy < (w * h); xy++)
        {
            transparentBG[xy] = Color.clear;
        }
        
        // Set the texture colors to the color array that we have just created.
        texture.SetPixels(transparentBG);

        // Loop through the available sprite nodes.
        for (int i = 0; i < spriteNodes.Count; i++)
        {
            // Get the current sprite node from the list.
            var currentNode = spriteNodes[i];

            // Get it's colors.
            var colors = currentNode.Texture.GetPixels();

            // The X position of the node (texture) in the texture sheet.
            var x1 = (int)currentNode.X;

            // The Y position of the node (texture) in the texture sheet.
            var y1 = (int)currentNode.Y;

            // The width of the node (texture).
            var x2 = currentNode.Texture.width;

            // The height of the node (texture).
            var y2 = currentNode.Texture.height;            

            // Put the node (texture) onto the texture sheet surface.
            texture.SetPixels(x1, y1, x2, y2, colors);
        }

        // Apply the changes to the texture.
        texture.Apply();

        // Create a new byte array to populate with texture encoding data.
        var bytes = new byte[0];

        // Populate the byte array with the texture encoding data.
        switch (format)
        {
            case OutputImageFormat.JPG:
                bytes = texture.EncodeToJPG();
                break;
            case OutputImageFormat.PNG:
                bytes = texture.EncodeToPNG();
                break;
        }

        // Write the texture to a file.
        File.WriteAllBytes(Path.Combine(Serializer.SpriteSheetsOutputPath, $"{sheet.Name}.{format.ToString().ToLower()}"), bytes);

        // Return the created texture for later use.
        return texture;
    }
}
