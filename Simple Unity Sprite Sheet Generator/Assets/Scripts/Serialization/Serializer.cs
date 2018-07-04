using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    public static void Save(SpriteSheet spriteSheet, string outputDirectory, SheetSerializationFormat format)
    {
        if (spriteSheet == null || string.IsNullOrEmpty(outputDirectory))
        {
            return;
        }

        var path = string.Empty;

        if (format.Equals(SheetSerializationFormat.Json))
        {
            path = Path.Combine(outputDirectory, $"{spriteSheet.Name}.json");

            var json = JsonConvert.SerializeObject(spriteSheet, Formatting.Indented);

            File.WriteAllText(path, json);
        }
        else if (format.Equals(SheetSerializationFormat.Binary))
        {
            path = Path.Combine(outputDirectory, $"{spriteSheet.Name}.sussg");

            var formatter = new BinaryFormatter();
            
            var bytes = new byte[0];
            
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, spriteSheet);
                bytes = ms.ToArray();
            }

            File.WriteAllBytes(path, bytes);
        }
    }

    /// <summary>
    /// Loads a sprite sheet from the specified filePath.
    /// </summary>
    /// <param name="spriteSheet"></param>
    public static void Load(out SpriteSheet spriteSheet, string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            spriteSheet = new SpriteSheet("broken_sheet", 64, 64);
            return;
        }

        var extension = Path.GetExtension(filePath);

        var isJson = string.Compare(extension, ".json", true) == 0;
        var isBinary = string.Compare(extension, ".sussg", true) == 0;

        var loadedSheet = new SpriteSheet("new_sprite_sheet", 64, 64);

        if (isJson)
        {
            var json = File.ReadAllText(Path.Combine(filePath));

            loadedSheet = JsonConvert.DeserializeObject<SpriteSheet>(json);

            foreach (var sprite in loadedSheet.SpriteNodes)
            {
                sprite.InitializeTexture();
            }
        }
        else if (isBinary)
        {
            var bytes = File.ReadAllBytes(filePath);

            var formatter = new BinaryFormatter();
            
            using (var ms = new MemoryStream(bytes))
            {
                loadedSheet = formatter.Deserialize(ms) as SpriteSheet;
            }

            foreach (var sprite in loadedSheet.SpriteNodes)
            {
                sprite.InitializeTexture();
            }
        }

        spriteSheet = loadedSheet;
    }
}
