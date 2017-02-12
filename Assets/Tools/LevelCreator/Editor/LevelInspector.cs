using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace RunAndJump.LevelCreator
{
    [CustomEditor(typeof(Level))]
    public class LevelInspector : Editor
    {
        private Level _myTarget;

        private SerializedObject _mySerializedObject;
        private SerializedProperty _serializedTotalTime;

        private int _newTotalRows;
        private int _newTotalColumns;

        private PaletteItem _itemSelected;
        private Texture2D _itemPreview;
        private LevelPiece _pieceSelected;

        private Mode _selectedMode;
        private Mode _currentMode;

        private PaletteItem _itemInspected;

        private int _originalPosX;
        private int _originalPosY;

        public enum Mode
        {
            View,
            Paint,
            Edit,
            Erase
        }

        private void OnEnable()
        {
            _myTarget = (Level)target;
            InitLevel();
            ResetResizeValues();
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void OnSceneGUI()
        {
            DrawModeGUI();
            HandleMode();
            HandleEvent();
        }

        private void InitLevel()
        {
            _mySerializedObject = new SerializedObject(_myTarget);
            _serializedTotalTime = _mySerializedObject.FindProperty("_totalTime");
            _myTarget.transform.hideFlags = HideFlags.NotEditable;
            _myTarget.InitializePieces();
        }

        private void ResetResizeValues()
        {
            _newTotalColumns = _myTarget.TotalColumns;
            _newTotalRows = _myTarget.TotalRows;
        }

        public override void OnInspectorGUI()
        {
            DrawLevelDataGUI();
            DrawLevelSizeGUI();
            DrawPieceSelectedGUI();
            DrawInspectedItemGUI();

            SetDirtyIfNeeded();
        }

        private void SetDirtyIfNeeded()
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_myTarget);
            }
        }

        private void DrawLevelDataGUI()
        {
            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(_serializedTotalTime);
            _myTarget.Gravity = EditorGUILayout.FloatField("Gravity", _myTarget.Gravity);
            _myTarget.Bgm = (AudioClip)EditorGUILayout.ObjectField("Bgm", _myTarget.Bgm, typeof(AudioClip), false);
            _myTarget.Background = (Sprite)EditorGUILayout.ObjectField("Background", _myTarget.Background, typeof(Sprite), false);

            EditorGUILayout.EndVertical();
        }

        private void DrawLevelSizeGUI()
        {
            EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal("box");

            DrawRowsAndColumnsData();
            DrawButtons();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawRowsAndColumnsData()
        {
            EditorGUILayout.BeginVertical();

            _newTotalRows = EditorGUILayout.IntField("Rows", Mathf.Max(1, _newTotalRows));
            _newTotalColumns = EditorGUILayout.IntField("Columns", Mathf.Max(1, _newTotalColumns));

            EditorGUILayout.EndVertical();
        }

        public void DrawButtons()
        {
            EditorGUILayout.BeginVertical();

            ExecuteAndPreserveGUIEnabled(() =>
            {
                GUI.enabled = _newTotalColumns != _myTarget.TotalColumns || _newTotalRows != _myTarget.TotalRows;
                DrawResizeButton();
                DrawResetButton();
            });

            EditorGUILayout.EndVertical();
        }

        public void ExecuteAndPreserveGUIEnabled(Action action)
        {
            var oldEnabled = GUI.enabled;
            action();
            GUI.enabled = oldEnabled;
        }

        private void DrawResetButton()
        {
            var buttonReset = GUILayout.Button("Reset");

            if (buttonReset)
            {
                ResetResizeValues();
            }
        }

        private void DrawResizeButton()
        {
            var buttonResize = GUILayout.Button("Resize");

            if (buttonResize)
            {
                if (EditorUtility.DisplayDialog(
                    "Level Creator",
                    "Are you sure you want to resize the level?\nThis action cannot be undone.",
                    "Yes",
                    "No"))
                {
                    ResizeLevel();
                }
            }
        }

        private void ResizeLevel()
        {
            _myTarget.ResizePiecesMatrix(_newTotalColumns, _newTotalRows);
        }

        private void UpdateCurrentPieceInstance(PaletteItem item, Texture2D preview)
        {
            _itemSelected = item;
            _itemPreview = preview;
            _pieceSelected = item.GetComponent<LevelPiece>();
            Repaint();
        }

        private void SubscribeEvents()
        {
            PaletteWindow.ItemSelectedEvent += new PaletteWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
        }

        private void UnsubscribeEvents()
        {
            PaletteWindow.ItemSelectedEvent -= new PaletteWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
        }

        private void DrawPieceSelectedGUI()
        {
            EditorGUILayout.LabelField("Piece Selected", EditorStyles.boldLabel);

            if (_pieceSelected == null)
            {
                EditorGUILayout.HelpBox("No piece selected!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(new GUIContent(_itemPreview), GUILayout.Height(40));
                EditorGUILayout.LabelField(_itemSelected.itemName);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawModeGUI()
        {
            var modes = EditorUtils.GetListFromEnum<Mode>();
            var modeLabels = new List<string>();

            foreach (var mode in modes)
            {
                modeLabels.Add(mode.ToString());
            }

            Handles.BeginGUI();

            GUILayout.BeginArea(new Rect(10f, 10f, 360, 40f));
            _selectedMode = (Mode)GUILayout.Toolbar(
                (int)_currentMode,
                modeLabels.ToArray(),
                GUILayout.ExpandHeight(true));
            GUILayout.EndArea();

            Handles.EndGUI();
        }

        private void DrawInspectedItemGUI()
        {
            if (_currentMode != Mode.Edit)
            {
                return;
            }

            EditorGUILayout.LabelField("Piece Edited", EditorStyles.boldLabel);

            if (_itemInspected != null)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Name: " + _itemInspected.name);
                CreateEditor(_itemInspected.inspectedScript).OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("No piece to edit", MessageType.Info);
            }
        }

        private void HandleMode()
        {
            switch (_selectedMode)
            {
                case Mode.Paint:
                case Mode.Edit:
                case Mode.Erase:
                    Tools.current = Tool.None;
                    break;
                case Mode.View:
                default:
                    Tools.current = Tool.View;
                    break;
            }

            if (_selectedMode != _currentMode)
            {
                _currentMode = _selectedMode;
                _itemInspected = null;
                Repaint();
            }

            SceneView.currentDrawingSceneView.in2DMode = true;
        }

        private void HandleEvent()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var camera = SceneView.currentDrawingSceneView.camera;
            var mousePosition = Event.current.mousePosition;
            mousePosition = new Vector2(mousePosition.x, camera.pixelHeight - mousePosition.y);

            var worldPos = camera.ScreenToWorldPoint(mousePosition);
            var gridPos = _myTarget.WorldToGridCoordinates(worldPos);

            switch (_currentMode)
            {
                case Mode.Paint:
                    Paint(Event.current, gridPos);
                    break;
                case Mode.Edit:
                    Edit(Event.current, gridPos);
                    break;
                case Mode.Erase:
                    Erase(Event.current, gridPos);
                    break;
                case Mode.View:
                default:
                    break;
            }
        }

        private bool IsDownOrDrag(Event e)
        {
            return e.type == EventType.MouseDown || e.type == EventType.MouseDrag;
        }

        private bool IsDown(Event e)
        {
            return e.type == EventType.MouseDown;
        }

        private void Paint(Event e, Vector2 point)
        {
            if (!IsDownOrDrag(e))
            {
                return;
            }

            Paint((int)point.x, (int)point.y);
        }

        private void Paint(int col, int row)
        {
            if (!_myTarget.IsInsideGridBounds(col, row) || _pieceSelected == null)
            {
                return;
            }

            if (_myTarget.GetPiece(col, row) != null)
            {
                DestroyImmediate(_myTarget.GetPiece(col, row).gameObject);
            }

            var gObj = PrefabUtility.InstantiatePrefab(_pieceSelected.gameObject) as GameObject;
            gObj.transform.parent = _myTarget.transform;
            gObj.name = string.Format("[{0},{1}][{2}]", col, row, gObj.name);
            gObj.transform.position = _myTarget.GridToWorldCoordinates(col, row);
            gObj.hideFlags = HideFlags.HideInHierarchy;
            _myTarget.SetPiece(col, row, gObj.GetComponent<LevelPiece>());
        }

        private void Erase(Event e, Vector2 point)
        {
            if (!IsDownOrDrag(e))
            {
                return;
            }

            Erase((int)point.x, (int)point.y);
        }

        private void Erase(int col, int row)
        {
            if (!_myTarget.IsInsideGridBounds(col, row) || _pieceSelected == null)
            {
                return;
            }

            if (_myTarget.GetPiece(col, row) != null)
            {
                DestroyImmediate(_myTarget.GetPiece(col, row).gameObject);
            }
        }

        private void Edit(Event e, Vector2 point)
        {
            if (IsDown(e))
            {
                UpdateInspectedItem((int)point.x, (int)point.y);
                _originalPosX = (int)point.x;
                _originalPosY = (int)point.y;
            }

            if ((e.type == EventType.MouseUp
                || e.type == EventType.Ignore)
                && _itemInspected != null)
            {
                Move();
            }

            if (_itemInspected != null)
            {
                _itemInspected.transform.position = Handles.FreeMoveHandle(
                    _itemInspected.transform.position,
                    _itemInspected.transform.rotation,
                    Level.GridCellSize / 2,
                    Level.GridCellSize / 2 * Vector3.one,
                    Handles.RectangleCap);

                Handles.color = Handles.xAxisColor;
                Handles.ArrowCap(0, _itemInspected.transform.position, _itemInspected.transform.rotation * Quaternion.Euler(0, 90, 0), 1);
                Handles.ArrowCap(0, _itemInspected.transform.position, _itemInspected.transform.rotation * Quaternion.Euler(0, -90, 0), 1);

                Handles.color = Handles.yAxisColor;
                Handles.ArrowCap(0, _itemInspected.transform.position, _itemInspected.transform.rotation * Quaternion.Euler(90, 0, 0), 1);
                Handles.ArrowCap(0, _itemInspected.transform.position, _itemInspected.transform.rotation * Quaternion.Euler(-90, 0, 0), 1);
            }
        }

        private void UpdateInspectedItem(int col, int row)
        {
            if (!_myTarget.IsInsideGridBounds(col, row) || _myTarget.GetPiece(col, row) == null)
            {
                _itemInspected = null;
            }
            else
            {
                _itemInspected = _myTarget.GetPiece(col, row).GetComponent<PaletteItem>() as PaletteItem;
            }

            Repaint();
        }

        private void Move()
        {
            var gridPoint = _myTarget.WorldToGridCoordinates(_itemInspected.transform.position);
            var newPosX = (int)gridPoint.x;
            var newPosY = (int)gridPoint.y;

            if (IsSameThanOriginalPosition(newPosX, newPosY))
            {
                return;
            }

            if (IsValidNewPosition(newPosX, newPosY))
            {
                _itemInspected.transform.position = _myTarget.GridToWorldCoordinates(_originalPosX, _originalPosY);
            }
            else
            {
                UpdateInspectedItemPosition(newPosX, newPosY);
            }
        }

        private void UpdateInspectedItemPosition(int newPosX, int newPosY)
        {
            _myTarget.SetPiece(_originalPosX, _originalPosY, null);

            var newPiece = _itemInspected.GetComponent<LevelPiece>();
            newPiece.transform.position = _myTarget.GridToWorldCoordinates(newPosX, newPosY);
            _myTarget.SetPiece(newPosX, newPosY, newPiece);
        }

        private bool IsSameThanOriginalPosition(int newPosX, int newPosY)
        {
            return newPosX == _originalPosX && newPosY == _originalPosY;
        }

        private bool IsValidNewPosition(int newPosX, int newPosY)
        {
            return !_myTarget.IsInsideGridBounds(newPosX, newPosY) || _myTarget.GetPiece(newPosX, newPosY) != null;
        }
    }
}
