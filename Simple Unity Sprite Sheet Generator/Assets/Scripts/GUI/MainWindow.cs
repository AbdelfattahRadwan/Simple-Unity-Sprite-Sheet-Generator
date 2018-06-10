using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MainWindow : MonoBehaviour
{
    [SerializeField] private GUISkin skin;

    /// <summary>
    /// The current sprite sheet.
    /// </summary>
    private SpriteSheet spriteSheet;

    /// <summary>
    /// The vector used for drawing the sprite list scrollview.
    /// </summary>
    private Vector2 listVector;

    /// <summary>
    /// The vector used for drawing the editor scrollview.
    /// </summary>
    private Vector2 editorVector;

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
    private Rect EditorRect => new Rect((ScreenW / 4f) + 16f, 16f + (ToolbarHeight), (ScreenW - (ScreenW / 4f) - 24f), ScreenH - (ToolbarHeight + 24f));

    /// <summary>
    /// The dimensions of the edtior scrollview box.
    /// </summary>
    private Rect EditorScrollViewRect => new Rect(0f, 0f, EditorRect.width, EditorRect.height);

    /// <summary>
    /// The texture that's currently being dragged.
    /// </summary>
    private Texture2D draggedTexture;

    /// <summary>
    /// The sprite node that's currently being dragged.
    /// </summary>
    private SpriteNode draggedNode;

    /// <summary>
    /// Represents the current nodes that are on the current sprite sheet.
    /// </summary>
    private List<SpriteNode> spriteNodes;

    /// <summary>
    /// Is the user dragging a texture or a sprite node?
    /// </summary>
    private bool IsDragging => (draggedTexture || draggedNode != null);

    /// <summary>
    /// The current displayed GUI window.
    /// </summary>
    private Window currentWindow;

    /// <summary>
    /// Used to record the mouse position on every click.
    /// </summary>
    private Vector2 mouseClickPosition;

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
    private readonly string regexPattern = "[a-z-A-Z]";

    /// <summary>
    /// The local regular expression handler.
    /// </summary>
    private Regex regex;

    private void Awake()
    {
        spriteSheet = new SpriteSheet("debug_sheet", 512, 512, Serializer.LoadTextures(Serializer.TexturesDirectoryPath));

        spriteNodes = new List<SpriteNode>();

        regex = new Regex(regexPattern);

        newSpriteSheetWidth = string.Empty;
        newSpriteSheetHeight = string.Empty;

        currentWindow = Window.SheetDesigner;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SpriteSheetGenerator.RenderToTexture(spriteSheet, spriteNodes);
        }
    }

    private void OnGUI()
    {
        GUI.skin = skin ?? GUI.skin;

        var screenW = Screen.width;
        var screenH = Screen.height;

        DrawToolbar();

        switch (currentWindow)
        {
            case Window.None:
                DrawDefaultScreen();
                break;
            case Window.SheetSettings:
                break;
            case Window.SheetDesigner:
                DrawBackgrounds();
                DrawSpriteList();
                DrawSpriteNodes();
                HandleTexturesDragAndDrop();
                HandleNodesDragAndDrop();
                break;
            case Window.ExportSettings:
                break;
        }
    }

    /// <summary>
    /// Used to draw the default welcome screen.
    /// </summary>
    private void DrawDefaultScreen()
    {
        GUI.Box(DefaultWindowRect, string.Empty);

        GUILayout.BeginArea(DefaultWindowRect);

        GUILayout.Box("Welcome to S.U.S.S.G.");

        GUILayout.Box("Create a new sprite sheet");

        GUILayout.BeginHorizontal();

        GUILayout.Box("New Width");

        newSpriteSheetWidth = regex.Replace(GUILayout.TextField(newSpriteSheetWidth), string.Empty);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Box("New Height");

        newSpriteSheetHeight = regex.Replace(GUILayout.TextField(newSpriteSheetHeight), string.Empty);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Create Sheet"))
        {
            var w = 32;
            var h = 32;

            int.TryParse(newSpriteSheetWidth, out w);
            int.TryParse(newSpriteSheetHeight, out h);

            spriteSheet = new SpriteSheet("debug_sheet", w, h, Serializer.LoadTextures(Serializer.TexturesDirectoryPath));

            spriteNodes.Clear();
        }

        GUILayout.EndArea();
    }

    private void DrawBackgrounds()
    {
        GUI.Box(ListRect, string.Empty);
        GUI.Box(EditorRect, string.Empty);
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

        if (GUILayout.Button("Export Sprite Sheet", options))
        {
            SpriteSheetGenerator.RenderToTexture(spriteSheet, spriteNodes);
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

        var textureCount = spriteSheet.TextureCount;

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
                GUI.Box(textureListRect, spriteSheet.Textures[i].name);

                var mousePosition = e.mousePosition;

                if (!IsDragging && e.type.Equals(EventType.MouseDrag) && textureListRect.Contains(mousePosition))
                {
                    draggedTexture = spriteSheet.Textures[i];
                }
            }
        }

        GUI.EndScrollView();

        GUI.EndGroup();
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

        if (spriteNodes.Count > 0)
        {
            var e = Event.current;

            for (int i = 0; i < spriteNodes.Count; i++)
            {
                var currentNode = spriteNodes[i];

                var w = currentNode.Texture.width;
                var h = currentNode.Texture.height;

                var position = currentNode.Position;

                position.x = Mathf.Clamp(position.x, 0, spriteSheet.Width);
                position.y = Mathf.Clamp(position.y, 0, spriteSheet.Height);

                currentNode.Position = position;

                var nodesRect = new Rect()
                {
                    x = currentNode.Position.x,
                    y = currentNode.Position.y,
                    width = currentNode.Texture.width,
                    height = currentNode.Texture.height
                };

                GUI.DrawTexture(nodesRect, currentNode.Texture);

                var mousePosition = e.mousePosition;

                if (nodesRect.Contains(mousePosition))
                {
                    if (!IsDragging && e.type.Equals(EventType.MouseDrag) && e.button == 0)
                    {
                        draggedNode = currentNode;

                        spriteNodes.RemoveAt(i);
                    }

                    if (e.type.Equals(EventType.MouseDown) && e.button == 1 && e.modifiers.Equals(EventModifiers.Shift))
                    {
                        spriteNodes.RemoveAt(i);
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

        if (draggedTexture)
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

                    spriteNodes.Add(new SpriteNode()
                    {
                        Name = draggedTexture.name,
                        Position = new Vector2Int(relativeX, relativeY),
                        Texture = draggedTexture
                    });

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
            var nodeTexture = draggedNode.Texture;

            var w = nodeTexture.width;
            var h = nodeTexture.height;

            var drawingRect = new Rect()
            {
                x = mousePosition.x - (w / 2f),
                y = mousePosition.y - (h / 2f),
                width = w,
                height = h
            };

            GUI.DrawTexture(drawingRect, nodeTexture);

            if (e.type.Equals(EventType.MouseUp))
            {
                if (EditorRect.Contains(mousePosition))
                {
                    var relativeX = (int)(((mousePosition.x - EditorRect.x) + editorVector.x) - w / 2f);
                    var relativeY = (int)(((mousePosition.y - EditorRect.y) + editorVector.y) - h / 2f);

                    spriteNodes.Add(new SpriteNode()
                    {
                        Name = nodeTexture.name,
                        Position = new Vector2Int(relativeX, relativeY),
                        Texture = nodeTexture
                    });

                    draggedNode = null;
                }
            }
        }
    }
}