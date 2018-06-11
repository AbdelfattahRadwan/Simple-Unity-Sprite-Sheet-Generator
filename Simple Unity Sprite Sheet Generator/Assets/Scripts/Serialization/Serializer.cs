using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public static void RegisterZeroFormatterTypes()
    {
        ///TODO: Added serialization types...
    }

    /// <summary>
    /// Loads all texture in jpg or png in the specified the specified directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static List<Texture2D> LoadTextures(string directory)
    {
        var files = new DirectoryInfo(directory).GetFiles();

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

                var texture = new Texture2D(4, 4, TextureFormat.RGBA32, false, false)
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
    /// Saves a sprite sheet to the specified output directory.
    /// </summary>
    /// <param name="spriteSheet"></param>
    public static void Save(SpriteSheet spriteSheet, string outputDirectory)
    {
        if (spriteSheet == null || string.IsNullOrEmpty(outputDirectory))
        {
            return;
        }

        var path = Path.Combine(outputDirectory, $"{spriteSheet.Name}.json");

        var json = JsonConvert.SerializeObject(spriteSheet, Formatting.Indented);

        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Loads a sprite sheet from the specified filePath.
    /// </summary>
    /// <param name="spriteSheet"></param>
    public static void Load(out SpriteSheet spriteSheet, string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            spriteSheet = new SpriteSheet("empty", 512, 512);
            return;
        }

        var json = File.ReadAllText(Path.Combine(filePath));

        spriteSheet = JsonConvert.DeserializeObject<SpriteSheet>(json);

        foreach (var sprite in spriteSheet.SpriteNodes)
        {
            sprite.InitializeTextureFromData();
        }
    }
}
