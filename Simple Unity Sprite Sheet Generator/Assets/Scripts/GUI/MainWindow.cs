using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class MainWindow : MonoBehaviour
{
    public static MainWindow Instance { get; private set; }

    [SerializeField] private GUISkin skin;

    /// <summary>
    /// The current sprite sheet.
    /// </summary>
    private SpriteSheet spriteSheet;

    /// <summary>
    /// Returns the currently open sprite sheet in the sheet designer.
    /// </summary>
    /// <returns></returns>
    public SpriteSheet GetOpenSpriteSheet()
    {
        return spriteSheet;
    }

    /// <summary>
    /// The vector used for drawing the sprite list scrollview.
    /// </summary>
    private Vector2 listVector;

    /// <summary>
    /// The vector used for drawing the editor scrollview.
    /// </summary>
    private Vector2 editorVector;

    /// <summary>
    /// The vector used for drawing the file browser scrollview.
    /// </summary>
    private Vector2 fileBrowserVector;

    /// <summary>
    /// The width of the device screen. (read-only)
    /// </summary>
    private float ScreenW => Screen.width;

    /// <summary>
    /// The height of the device screen. (read-only)
    /// </summary>
    private float ScreenH => Screen.height;

    /// <summary>
    /// The height of the toolbar relative to the current screen height.
    /// </summary>
    private float ToolbarHeight => ScreenH / 24f;

    /// <summary>
    /// The height of the inspector relative to the current screen height.
    /// </summary>
    private float InspectorWidth => ScreenH / 4f;

    /// <summary>
    /// The default dimensions of a window.
    /// </summary>
    private Rect DefaultWindowRect => new Rect(8f, 16f + (ToolbarHeight), ScreenW - 16f, ScreenH - (ToolbarHeight + 24f));

    /// <summary>
    /// The dimensions of the toolbar.
    /// </summary>
    private Rect ToolbarRect => new Rect(8f, 8f, ScreenW - 16f, ToolbarHeight);

    /// <summary>
    /// The dimensions of the sprite list box.
    /// </summary>
    private Rect ListRect => new Rect(8f, 16f + (ToolbarHeight), ScreenW / 4f, ScreenH - (ToolbarHeight + 24f));

    /// <summary>
    /// The dimensions of the edtior view box.
    /// </summary>
    private Rect EditorRect => new Rect((ScreenW / 4f) + 16f, 16f + (ToolbarHeight), (ScreenW - (ScreenW / 2f) - 24f), ScreenH - (ToolbarHeight + 24f));

    /// <summary>
    /// The dimensions of the editor scrollview box.
    /// </summary>
    private Rect EditorScrollViewRect => new Rect(0f, 0f, EditorRect.width, EditorRect.height);

    /// <summary>
    /// The dimensions of the insecptor view box.
    /// </summary>
    private Rect InspectorRect => new Rect(EditorRect.xMax + 8f, 16f + ToolbarHeight, (ScreenW / 4f) - 8f, ScreenH - (ToolbarHeight + 24f));

    /// <summary>
    /// The texture that's currently being dragged.
    /// </summary>
    private Texture2D draggedTexture;

    /// <summary>
    /// The sprite node that's currently being dragged.
    /// </summary>
    private SpriteNode draggedNode;

    /// <summary>
    /// The sprite node that's curretly selected.
    /// </summary>
    private SpriteNode selectedNode;

    /// <summary>
    /// Is the user dragging a texture or a sprite node?
    /// </summary>
    private bool IsDragging => (draggedTexture || draggedNode != null);

    /// <summary>
    /// The current displayed GUI window.
    /// </summary>
    private Window currentWindow;

    /// <summary>
    /// The name of the new sprite sheet.
    /// </summary>
    private string newSpriteSheetName;

    /// <summary>
    /// The string which represents width of the new sprite sheet.
    /// </summary>
    private string newSpriteSheetWidth;

    /// <summary>
    /// The string which represents height of the new sprite sheet.
    /// </summary>
    private string newSpriteSheetHeight;

    /// <summary>
    /// The regular expression used to clear the number input strings from any invalid charaters.
    /// </summary>
    private readonly string regexPattern = "[a-zA-Z]";

    /// <summary>
    /// The local regular expression handler.
    /// </summary>
    private Regex regex;

    /// <summary>
    /// The position of the last dragged sprite node on the x-axis.
    /// </summary>
    private float lastNodeX;

    /// <summary>
    /// The position of the last dragged sprite node on the y-axis.
    /// </summary>
    private float lastNodeY;

    /// <summary>
    /// Is the user pressing any shift key?
    /// </summary>
    private bool IsPressingShift => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    /// <summary>
    /// Is the user pressing any shift key?
    /// </summary>
    private bool IsPressingCtrl => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

    /// <summary>
    /// A list containing all of the textures located in <see cref="Serializer.TexturesDirectoryPath"/>.
    /// </summary>
    private List<Texture2D> loadedTextures;

    /// <summary>
    /// Returns the currently loaded editor textures.
    /// </summary>
    /// <returns></returns>
    public List<Texture2D> GetLoadedTextures()
    {
        return loadedTextures;
    }

    /// <summary>
    /// The file browser temporary data.
    /// </summary>
    private FileInfo[] fileBrowserTemp;

    /// <summary>
    /// Should the command manager window be drawn?
    /// </summary>
    private bool showCommandManager;

    /// <summary>
    /// The vector used to display the command window scroll view.
    /// </summary>
    private Vector2 commandWindowVector;

    private string xInputString;
    private string yInputString;
    private string xScaleInputString;
    private string yScaleInputString;
    private string widthInputString;
    private string heightInputString;

    private float parsedX;
    private float parsedY;
    private float parsedXScale;
    private float parsedYScale;
    private float parsedWidth;
    private float parsedHeight;

    private void Awake()
    {
        Instance = this;

        loadedTextures = Serializer.LoadTextures(Serializer.TexturesDirectoryPath);

        fileBrowserTemp = new FileInfo[0];

        spriteSheet = new SpriteSheet("new_sprite_sheet", 512, 512)
        {
            SpriteNodes = new List<SpriteNode>()
        };

        regex = new Regex(regexPattern);

        newSpriteSheetWidth = string.Empty;
        newSpriteSheetHeight = string.Empty;

        currentWindow = Window.None;

        // The command manager is still WIP enable it at your own risk...
        showCommandManager = false;
    }

    private void OnGUI()
    {
        GUI.skin = skin ?? GUI.skin;

        DrawToolbar();

        switch (currentWindow)
        {
            case Window.None:
                DrawBackgrounds(DefaultWindowRect);
                DrawDefaultScreen();
                break;
            case Window.SheetDesigner:
                DrawBackgrounds(ListRect, EditorRect, InspectorRect);
                DrawSpriteList();
                DrawSpriteNodeInspector();
                DrawSpriteNodes();
                HandleTexturesDragAndDrop();
                HandleNodesDragAndDrop();
                break;
            case Window.ExportSettings:
                break;
            case Window.Help:
                DrawBackgrounds(DefaultWindowRect);
                DrawHelpScreen();
                break;
            case Window.SavedFilesBrowser:
                DrawBackgrounds(DefaultWindowRect);
                DrawSavedFilesBrowser();
                break;
        }
    }

    /// <summary>
    /// Used to draw the default welcome screen.
    /// </summary>
    private void DrawDefaultScreen()
    {
        GUILayout.BeginArea(DefaultWindowRect);

        GUILayout.Box("Welcome to S.U.S.S.G.");

        GUILayout.Box("Create a new sprite sheet");

        GUILayout.BeginHorizontal();

        GUILayout.Box("Sheet Name");

        newSpriteSheetName = GUILayout.TextField(newSpriteSheetName);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Box("Width");

        newSpriteSheetWidth = regex.Replace(GUILayout.TextField(newSpriteSheetWidth), string.Empty);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Box("Height");

        newSpriteSheetHeight = regex.Replace(GUILayout.TextField(newSpriteSheetHeight), string.Empty);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Create Sheet"))
        {
            var w = 256;
            var h = 256;

            int.TryParse(newSpriteSheetWidth, out w);
            int.TryParse(newSpriteSheetHeight, out h);

            spriteSheet = new SpriteSheet(newSpriteSheetName, w, h);
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws a box for each specified rectangle.
    /// </summary>
    private void DrawBackgrounds(params Rect[] rects)
    {
        for (int i = 0; i < rects.Length; i++)
        {
            GUI.Box(rects[i], string.Empty);
        }
    }

    /// <summary>
    /// Used to draw the toolbar.
    /// </summary>
    private void DrawToolbar()
    {
        GUI.Box(ToolbarRect, string.Empty);

        GUILayout.BeginArea(ToolbarRect);

        GUILayout.BeginHorizontal();

        var options = new GUILayoutOption[] { GUILayout.Height(ToolbarHeight) };

        if (GUILayout.Button("Welcome Screen", options))
        {
            currentWindow = Window.None;
        }

        if (GUILayout.Button("Sheet Designer", options))
        {
            currentWindow = Window.SheetDesigner;
        }

        if (currentWindow.Equals(Window.SheetDesigner))
        {
            if (GUILayout.Button("Reload Texture From Disk", options))
            {
                loadedTextures = Serializer.LoadTextures(Serializer.TexturesDirectoryPath);
            }

            if (GUILayout.Button("Export as JPG", options))
            {
                SpriteSheetGenerator.RenderToTexture(spriteSheet, spriteSheet.SpriteNodes, OutputImageFormat.JPG);
            }

            if (GUILayout.Button("Export as PNG", options))
            {
                SpriteSheetGenerator.RenderToTexture(spriteSheet, spriteSheet.SpriteNodes, OutputImageFormat.PNG);
            }

            if (GUILayout.Button("Save As Json Data", options))
            {
                Serializer.Save(spriteSheet, Serializer.SavesDirectoryPath, SheetSerializationFormat.Json);
            }

            if (GUILayout.Button("Save As Binary Data", options))
            {
                Serializer.Save(spriteSheet, Serializer.SavesDirectoryPath, SheetSerializationFormat.Binary);
            }

            if (GUILayout.Button("Load Sprite Sheet", options))
            {
                currentWindow = Window.SavedFilesBrowser;
            }
        }

        if (GUILayout.Button("Exit", options))
        {
            Application.Quit();
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    /// <summary>
    /// Used to draw the loaded sprites list.
    /// </summary>
    private void DrawSpriteList()
    {
        GUI.BeginGroup(ListRect);

        var localWidth = ListRect.width;
        var localHeight = ListRect.height;

        var headerRect = new Rect(4f, 4f, localWidth - 8f, 32f);

        var textureCount = loadedTextures.Count;

        GUI.Box(headerRect, $"Textures [{textureCount}]");

        var scrollViewBoxRect = new Rect(4f, 40f, localWidth - 8f, localHeight - 32f);

        var scrollViewDisplayRect = new Rect(0f, 0f, localWidth - 8f, (textureCount * 34f) + 24f);

        listVector = GUI.BeginScrollView(scrollViewBoxRect, listVector, scrollViewDisplayRect);

        var e = Event.current;

        for (int i = 0; i < textureCount; i++)
        {
            var textureListRect = new Rect(4f, i * 34f, localWidth - 32f, 32f);

            if (i < textureCount)
            {
                GUI.Box(textureListRect, $"{i} - {loadedTextures[i].name}");

                var mousePosition = e.mousePosition;

                if (!IsDragging && e.type.Equals(EventType.MouseDrag) && textureListRect.Contains(mousePosition))
                {
                    draggedTexture = loadedTextures[i];
                }
            }
        }

        GUI.EndScrollView();

        GUI.EndGroup();
    }

    /// <summary>
    /// Used to draw the sprite inspector.
    /// </summary>
    private void DrawSpriteNodeInspector()
    {
        GUILayout.BeginArea(InspectorRect);

        GUILayout.Box("Inspector");

        if (selectedNode != null)
        {
            var regex = new Regex(regexPattern);

            xInputString = regex.Replace(xInputString, string.Empty);
            yInputString = regex.Replace(yInputString, string.Empty);
            xScaleInputString = regex.Replace(xScaleInputString, string.Empty);
            yScaleInputString = regex.Replace(yScaleInputString, string.Empty);
            widthInputString = regex.Replace(widthInputString, string.Empty);
            heightInputString = regex.Replace(heightInputString, string.Empty);

            GUILayout.Box($"X = {selectedNode.X}");
            xInputString = GUILayout.TextField(xInputString);
            selectedNode.X = float.TryParse(xInputString, out parsedX) ? parsedX : selectedNode.X;

            GUILayout.Box($"Y = {selectedNode.Y}");
            yInputString = GUILayout.TextField(yInputString);
            selectedNode.Y = float.TryParse(yInputString, out parsedY) ? parsedY : selectedNode.Y;

            GUILayout.Box($"X Scale = {selectedNode.XScale}");
            xScaleInputString = GUILayout.TextField(xScaleInputString);
            selectedNode.XScale = float.TryParse(xScaleInputString, out parsedXScale) ? parsedXScale : selectedNode.XScale;

            GUILayout.Box($"Y Scale = {selectedNode.YScale}");
            yScaleInputString = GUILayout.TextField(yScaleInputString);
            selectedNode.YScale = float.TryParse(yScaleInputString, out parsedYScale) ? parsedYScale : selectedNode.YScale;

            GUILayout.Box($"Width = {selectedNode.Width}");
            widthInputString = GUILayout.TextField(widthInputString);
            selectedNode.Width = float.TryParse(widthInputString, out parsedWidth) ? parsedWidth : selectedNode.X;

            GUILayout.Box($"Height = {selectedNode.Height}");
            heightInputString = GUILayout.TextField(heightInputString);
            selectedNode.Height = float.TryParse(heightInputString, out parsedHeight) ? parsedHeight : selectedNode.Height;
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// Used to draw the sprite editor design nodes.
    /// </summary>
    private void DrawSpriteNodes()
    {
        GUI.BeginGroup(EditorRect);

        var nodeViewRect = new Rect(0f, 0f, spriteSheet.Width, spriteSheet.Height);

        editorVector = GUI.BeginScrollView(EditorScrollViewRect, editorVector, nodeViewRect);

        GUI.Box(nodeViewRect, string.Empty);

        if (spriteSheet.SpriteNodes.Count > 0)
        {
            var e = Event.current;

            for (int i = 0; i < spriteSheet.SpriteNodes.Count; i++)
            {
                var currentNode = spriteSheet.SpriteNodes[i];

                var w = currentNode.Width;
                var h = currentNode.Height;
                var xs = currentNode.XScale;
                var ys = currentNode.YScale;

                currentNode.X = Mathf.Clamp(currentNode.X, 0, spriteSheet.Width - w);
                currentNode.Y = Mathf.Clamp(currentNode.Y, 0, spriteSheet.Height - h);

                var nodesRect = new Rect()
                {
                    x = currentNode.X,
                    y = currentNode.Y,
                    width = w * xs,
                    height = h * ys
                };

                GUI.DrawTexture(nodesRect, currentNode.texture);

                var mousePosition = e.mousePosition;

                if (nodesRect.Contains(mousePosition))
                {
                    if (!IsDragging && e.type.Equals(EventType.MouseDrag) && e.button == 0)
                    {
                        lastNodeX = currentNode.X;
                        lastNodeY = currentNode.Y;

                        if (selectedNode != null && selectedNode == currentNode)
                        {
                            selectedNode = null;
                        }

                        draggedNode = currentNode;

                        spriteSheet.SpriteNodes.RemoveAt(i);
                    }

                    if (e.type.Equals(EventType.MouseDown))
                    {
                        if (e.button == 0)
                        {
                            xInputString = currentNode.X.ToString();
                            yInputString = currentNode.Y.ToString();
                            xScaleInputString = currentNode.XScale.ToString();
                            yScaleInputString = currentNode.YScale.ToString();
                            widthInputString = currentNode.Width.ToString();
                            heightInputString = currentNode.Height.ToString();

                            selectedNode = currentNode;
                        }
                        else if (e.button == 1 && IsPressingShift)
                        {
                            spriteSheet.SpriteNodes.RemoveAt(i);
                        }
                    }

                    if (Input.GetMouseButton(1) && IsPressingShift && IsPressingCtrl)
                    {
                        spriteSheet.SpriteNodes.RemoveAt(i);
                    }
                }
            }
        }

        GUI.EndScrollView();

        GUI.EndGroup();
    }

    /// <summary>
    /// Used to handle & perform drag & drop operations on the texture list.
    /// </summary>
    private void HandleTexturesDragAndDrop()
    {
        var e = Event.current;

        var mousePosition = e.mousePosition;

        if (draggedTexture != null)
        {
            var w = draggedTexture.width;
            var h = draggedTexture.height;

            var drawingRect = new Rect()
            {
                x = mousePosition.x - (w / 2f),
                y = mousePosition.y - (h / 2f),
                width = w,
                height = h
            };

            GUI.DrawTexture(drawingRect, draggedTexture);

            if (e.type.Equals(EventType.MouseUp))
            {
                if (EditorRect.Contains(mousePosition))
                {
                    var relativeX = (int)(((mousePosition.x - EditorRect.x) + editorVector.x) - w / 2f);
                    var relativeY = (int)(((mousePosition.y - EditorRect.y) + editorVector.y) - h / 2f);

                    spriteSheet.SpriteNodes.Add(new SpriteNode(draggedTexture.name, relativeX, relativeY, 1f, 1f, w, h, draggedTexture));

                    draggedTexture = null;
                }
                else
                {
                    draggedTexture = null;
                }
            }
        }
    }

    /// <summary>
    /// Used to handle & perform drag & drop operations on the sprite nodes in the edtior design view.
    /// </summary>
    private void HandleNodesDragAndDrop()
    {
        var e = Event.current;

        var mousePosition = e.mousePosition;

        if (draggedNode != null)
        {
            var nodeTexture = draggedNode.texture;

            var w = draggedNode.Width;
            var h = draggedNode.Height;
            var xs = draggedNode.XScale;
            var ys = draggedNode.YScale;

            var drawingRect = new Rect()
            {
                x = mousePosition.x - ((w * xs) / 2f),
                y = mousePosition.y - ((h * ys) / 2f),
                width = w * xs,
                height = h * ys
            };

            GUI.DrawTexture(drawingRect, nodeTexture);

            if (e.type.Equals(EventType.MouseUp))
            {
                if (EditorRect.Contains(mousePosition))
                {
                    var relativeX = Mathf.FloorToInt(((mousePosition.x - EditorRect.x) + editorVector.x) - (w * xs) / 2f);
                    var relativeY = Mathf.FloorToInt(((mousePosition.y - EditorRect.y) + editorVector.y) - (h * ys) / 2f);

                    spriteSheet.SpriteNodes.Add(new SpriteNode(nodeTexture.name, relativeX, relativeY, xs, ys, w, h, nodeTexture));

                    draggedNode = null;
                }
                else
                {
                    spriteSheet.SpriteNodes.Add(new SpriteNode(nodeTexture.name, lastNodeX, lastNodeY, xs, ys, w, h, nodeTexture));

                    draggedNode = null;
                }
            }
        }
    }

    /// <summary>
    /// Used to draw a simple help screen to help users getting started.
    /// </summary>
    private void DrawHelpScreen()
    {

    }

    /// <summary>
    /// Used to draw a file browser which is used to display saved files.
    /// </summary>
    private void DrawSavedFilesBrowser()
    {
        GUILayout.BeginArea(DefaultWindowRect);

        GUILayout.Box("Load a saved sprite sheet");

        if (GUILayout.Button("Refresh"))
        {
            fileBrowserTemp = new DirectoryInfo(Serializer.SavesDirectoryPath).GetFiles();
        }

        fileBrowserVector = GUILayout.BeginScrollView(fileBrowserVector);

        for (int i = 0; i < fileBrowserTemp.Length; i++)
        {
            if (GUILayout.Button($"{i + 1} - {fileBrowserTemp[i].FullName}"))
            {
                Serializer.Load(out spriteSheet, fileBrowserTemp[i].FullName);
            }
        }

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }
}