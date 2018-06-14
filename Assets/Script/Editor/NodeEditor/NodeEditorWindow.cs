using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class NodeEditorWindow : EditorWindow
{
    private List<Node> nodes;
    private List<Connection> connections;

    private GUIStyle defaultNodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inStyle;
    private GUIStyle outStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    [MenuItem("Window/Node Editor")]
    private static void OpenWindow()
    {
        NodeEditorWindow window = GetWindow<NodeEditorWindow>();
        window.titleContent = new GUIContent("Node Editor");
    }

    private void OnEnable()
    {
        nodes = new List<Node>();
        connections = new List<Connection>();

        defaultNodeStyle = new GUIStyle();
        defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inStyle = new GUIStyle();
        inStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inStyle.border = new RectOffset(4, 4, 12, 12);

        outStyle = new GUIStyle();
        outStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outStyle.border = new RectOffset(4, 4, 12, 12);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int i = 0; i < heightDivs; i++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset,
                new Vector3(position.width, gridSpacing * i, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        foreach (Node n in nodes)
        {
            n.Draw();
        }
    }

    private void DrawConnections()
    {
        foreach (Connection con in connections)
        {
            con.Draw();
        }
    }

    private void DrawConnectionLine(Event currentEvent)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.Rect.center,
                currentEvent.mousePosition,
                selectedInPoint.Rect.center + Vector2.left * 50f,
                currentEvent.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.Rect.center,
                currentEvent.mousePosition,
                selectedOutPoint.Rect.center - Vector2.left * 50f,
                currentEvent.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessNodeEvents(Event currentEvent)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].ProcessEvents(currentEvent))
                GUI.changed = true;
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        drag = Vector2.zero;
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                if (currentEvent.button == 0)
                    ClearConnectionSelection();

                if (currentEvent.button == 1)
                    ProcessContextMenu(currentEvent.mousePosition);
                break;

            case EventType.MouseDrag:
                if (currentEvent.button == 0)
                {
                    OnDrag(currentEvent.delta);
                }
                break;
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        foreach (Node node in nodes)
        {
            node.Drag(delta);
        }

        GUI.changed = true;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu gMenu = new GenericMenu();
        gMenu.AddItem(new GUIContent("Add node"), false, () => AddNode(mousePosition));
        gMenu.ShowAsContext();
    }

    private void AddNode(Vector2 mousePosition)
    {
        nodes.Add(new Node(mousePosition, 200, 50,
            defaultNodeStyle, selectedNodeStyle, inStyle, outStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void OnClickRemoveNode(Node node)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            if (connections[i].InPoint == node.InPoint || connections[i].OutPoint == node.OutPoint)
            {
                connections.RemoveAt(i);
            }
        }
        nodes.Remove(node);
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.Node != selectedInPoint.Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedInPoint.Node != selectedOutPoint.Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void CreateConnection()
    {
        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickConnection));
    }

    private void OnClickConnection(Connection con)
    {
        connections.Remove(con);
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
}

public class Node
{
    public Rect Rect;
    public string Title;
    public bool IsDragged;
    public bool IsSelected;

    public GUIStyle Nstyle;
    public GUIStyle DefaultStyle;
    public GUIStyle SelectedStyle;

    public ConnectionPoint InPoint;
    public ConnectionPoint OutPoint;

    public Action<Node> OnRemoveNode;

    public Node(Vector2 position, float width, float height,
        GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
    {
        Rect = new Rect(position.x, position.y, width, height);
        Nstyle = defaultStyle;
        InPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, onClickInPoint);
        OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, onClickOutPoint);
        DefaultStyle = defaultStyle;
        SelectedStyle = selectedStyle;
        OnRemoveNode = onRemoveNode;
    }

    public void Drag(Vector2 delta)
    {
        Rect.position += delta;
    }

    public void Draw()
    {
        InPoint.Draw();
        OutPoint.Draw();
        GUI.Box(Rect, Title, Nstyle);
    }

    public bool ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                if (currentEvent.button == 0)
                {
                    if (Rect.Contains(currentEvent.mousePosition))
                    {
                        IsDragged = true;
                        GUI.changed = true;
                        IsSelected = true;
                        Nstyle = SelectedStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        IsSelected = false;
                        Nstyle = DefaultStyle;
                    }
                }

                if (currentEvent.button == 1 && IsSelected && Rect.Contains(currentEvent.mousePosition))
                {
                    ProcessContextMenu();
                    currentEvent.Use();
                }
                break;

            case EventType.MouseUp:
                IsDragged = false;
                break;

            case EventType.MouseDrag:
                if (currentEvent.button == 0 && IsDragged)
                {
                    Drag(currentEvent.delta);
                    currentEvent.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu gMenu = new GenericMenu();
        gMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        gMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        OnRemoveNode?.Invoke(this);
    }
}

public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public Rect Rect;
    public ConnectionPointType Type;
    public Node Node;
    public GUIStyle Gstyle;
    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(Node node, ConnectionPointType type,
        GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
    {
        Node = node;
        Type = type;
        Gstyle = style;
        OnClickConnectionPoint = onClickConnectionPoint;
        Rect = new Rect(0, 0, 10f, 20f);
    }

    public void Draw()
    {
        Rect.y = Node.Rect.y + (Node.Rect.height * 0.5f) - Rect.height * 0.5f;

        switch (Type)
        {
            case ConnectionPointType.In:
                Rect.x = Node.Rect.x - Rect.width + 8f;
                break;

            case ConnectionPointType.Out:
                Rect.x = Node.Rect.x + Node.Rect.width - 8f;
                break;
        }

        if (GUI.Button(Rect, "", Gstyle))
            OnClickConnectionPoint?.Invoke(this);
    }
}

public class Connection
{
    public ConnectionPoint InPoint;
    public ConnectionPoint OutPoint;
    public Action<Connection> OnClickRemoveConnection;

    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> onClickRemoveConnection)
    {
        InPoint = inPoint;
        OutPoint = outPoint;
        OnClickRemoveConnection = onClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            InPoint.Rect.center,
            OutPoint.Rect.center,
            InPoint.Rect.center + Vector2.left * 50f,
            OutPoint.Rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f);

        if (Handles.Button((InPoint.Rect.center + OutPoint.Rect.center) * 0.5f,
            Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            OnClickRemoveConnection?.Invoke(this);
        }
    }
}