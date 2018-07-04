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

            var currentNodeTexture = currentNode.texture;

            // The X position of the node (texture) in the texture sheet.
            var x1 = (int)currentNode.X;

            // The Y position of the node (texture) in the texture sheet.
            var y1 = (int)currentNode.Y;

            // The unscaled width of the node (texture).
            var normalW = (int)currentNode.Width;

            // The unscaled height of the node (texture).
            var normalH = (int)currentNode.Height;

            // The scaled width of the node (texture).
            var scaledWidth = (int)(currentNode.Width * currentNode.XScale);

            // The scaled height of the node (texture).
            var scaledHeight = (int)(currentNode.Height * currentNode.YScale);

            // Invert the position of the texture on the Y axis.
            var invertedY = sheet.Height - y1 - scaledHeight;

            // Make a copy of it's textures.
            //var copy = new Texture2D(normalW, normalH, currentNodeTexture.format, false, false);

            // Set it's contents to the orginal texture contents.
            //copy.SetPixels(0, 0, copy.width, copy.height, currentNodeTexture.GetPixels());

            // Apply the contents change.
            //copy.Apply();

            // Resize it.
            //copy.Resize(scaledWidth, scaledHeight);

            // Apply the resizing.
            //copy.Apply();

            // Get it's colors.
            var colors = currentNodeTexture.GetPixels();

            // Put the node (texture) onto the texture sheet surface.
            texture.SetPixels(x1, invertedY, scaledWidth, scaledHeight, colors);
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
