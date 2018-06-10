using System.Collections.Generic;
using UnityEngine;

public class MainWindow : MonoBehaviour
{
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
    /// The diminsions of the sprite list box.
    /// </summary>
    private Rect ListRect => new Rect(8f, 8f, ScreenW / 4f, ScreenH - 16f);

    /// <summary>
    /// The diminsions of the edtior view box.
    /// </summary>
    private Rect EditorRect => new Rect((ScreenW / 4f) + 16f, 8f, (ScreenW - (ScreenW / 4f) - 24f), ScreenH - 16f);

    /// <summary>
    /// The texture that's currently being dragged.
    /// </summary>
    private Texture2D draggedTexture;

    private SpriteNode draggedNode;

    /// <summary>
    /// Represents the current nodes that are on the current sprite sheet.
    /// </summary>
    private List<SpriteNode> spriteNodes;

    /// <summary>
    /// Is the user dragging a texture or a sprite node?
    /// </summary>
    private bool IsDragging => (draggedTexture || draggedNode != null);

    private void Awake()
    {
        spriteSheet = new SpriteSheet("debug_sheet", 512, 512, Serializer.LoadTextures(Serializer.TexturesDirectoryPath));

        spriteNodes = new List<SpriteNode>();
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
        var screenW = Screen.width;
        var screenH = Screen.height;

        GUI.Box(ListRect, string.Empty);
        GUI.Box(EditorRect, string.Empty);

        DrawSpriteList();
        DrawSpriteNodes();
        HandleTexturesDragAndDrop();
        HandleNodesDragAndDrop();
    }

    private void DrawSpriteList()
    {
        GUI.BeginGroup(ListRect);

        var localWidth = ListRect.width;
        var localHeight = ListRect.height;

        var headerRect = new Rect(4f, 4f, localWidth - 8f, 32f);

        var textureCount = spriteSheet.TextureCount;

        GUI.Box(headerRect, $"Textures [{textureCount}]");

        var scrollViewBoxRect = new Rect(4f, 40f, localWidth - 8f, localHeight - 8f);

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

    private void DrawSpriteNodes()
    {
        GUI.BeginGroup(EditorRect);

        var nodeViewRect = new Rect(0f, 0f, spriteSheet.Width, spriteSheet.Height);

        editorVector = GUI.BeginScrollView(new Rect(0f, 0f, EditorRect.width, EditorRect.height), editorVector, nodeViewRect);

        GUI.Box(nodeViewRect, string.Empty);

        if (spriteNodes.Count > 0)
        {
            var e = Event.current;

            for (int i = 0; i < spriteNodes.Count; i++)
            {
                var currentNode = spriteNodes[i];

                var nodesRect = new Rect()
                {
                    x = currentNode.Position.x,
                    y = currentNode.Position.y,
                    width = currentNode.Texture.width,
                    height = currentNode.Texture.height
                };

                GUI.DrawTexture(nodesRect, currentNode.Texture);
                
                // BEGIN DEBUG CODE.

                GUI.color = Color.red;
             
                GUI.Label(new Rect(nodesRect.x+8f, nodesRect.y+8f, 128f, 32f), currentNode.Position.ToString());

                GUI.color = Color.white;

                // END DEBUG CODE.

                var mousePosition = e.mousePosition;

                if (!IsDragging && e.type.Equals(EventType.MouseDrag) && nodesRect.Contains(mousePosition))
                {
                    draggedNode = currentNode;

                    spriteNodes.RemoveAt(i);
                }
            }
        }

        GUI.EndScrollView();

        GUI.EndGroup();
    }

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
                    var relativeX = (int)((mousePosition.x - EditorRect.x) - draggedTexture.width / 2f);
                    var relativeY = (int)((mousePosition.y - EditorRect.y) - draggedTexture.height / 2f);

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
                    var relativeX = (int)((mousePosition.x - EditorRect.x) - nodeTexture.width / 2f);
                    var relativeY = (int)((mousePosition.y - EditorRect.y) - nodeTexture.height / 2f);

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