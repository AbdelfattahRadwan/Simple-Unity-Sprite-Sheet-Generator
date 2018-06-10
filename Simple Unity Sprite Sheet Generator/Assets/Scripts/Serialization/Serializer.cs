using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZeroFormatter;

/// <summary>
/// A class used to save/load projects, sprite sheets and textures.
/// </summary>
public static class Serializer
{
    /// <summary>
    /// The path of the directory where all save data goes.
    /// </summary>
    public static string SavesDirectoryPath => "bin/saves";

    /// <summary>
    /// The path of the directory where all texture should be placed to be usable in the editor.
    /// </summary>
    public static string TexturesDirectoryPath => "bin/res";

    /// <summary>
    /// The path of the directory where output sprite sheets go.
    /// </summary>
    public static string SpriteSheetsOutputPath => "bin/output";

    /// <summary>
    /// Loads all texture in jpg or png in the specified <see cref="TexturesDirectoryPath"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<Texture2D> LoadTextures(string path)
    {
        var files = new DirectoryInfo(path).GetFiles();

        if (files.Length == 0)
        {
            return new List<Texture2D>();
        }

        var textures = new List<Texture2D>();

        foreach (var file in files)
        {
            var filePath = file.FullName;

            var extension = Path.GetExtension(filePath);

            var fileName = Path.GetFileNameWithoutExtension(filePath);

            var isJPG = string.Compare(extension, ".jpg", true) == 0;
            var isPNG = string.Compare(extension, ".png", true) == 0;

            if (isJPG || isPNG)
            {
                var bytes = File.ReadAllBytes(filePath);

                var texture = new Texture2D(4, 4, TextureFormat.RGBA32, false)
                {
                    name = fileName
                };

                texture.LoadImage(bytes, false);

                textures.Add(texture);
            }
        }

        return textures;
    }

    /// <summary>
    /// Saves a sprite sheet to the <see cref="SavesDirectoryPath"/>.
    /// </summary>
    /// <param name="spriteSheet"></param>
    public static void Save(SpriteSheet spriteSheet)
    {
        var bytes = ZeroFormatterSerializer.Serialize(spriteSheet);

    }
}
