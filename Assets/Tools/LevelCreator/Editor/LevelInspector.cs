using UnityEngine;
using UnityEditor;
using System;

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

        private void InitLevel()
        {
            _mySerializedObject = new SerializedObject(_myTarget);
            _serializedTotalTime = _mySerializedObject.FindProperty("_totalTime");

            if (_myTarget.Pieces == null || _myTarget.Pieces.Length == 0)
            {
                _myTarget.Pieces = new LevelPiece[_myTarget.TotalRows * _myTarget.TotalColumns];
            }
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
            _myTarget.Pieces = CreateNewPieces();
            _myTarget.TotalColumns = _newTotalColumns;
            _myTarget.TotalRows = _newTotalRows;
        }

        private LevelPiece[] CreateNewPieces()
        {
            LevelPiece[] newPieces = new LevelPiece[_newTotalRows * _newTotalColumns];

            for (int column = 0; column < _myTarget.TotalColumns; ++column)
            {
                for (int row = 0; row < _myTarget.TotalRows; ++row)
                {
                    var targetIndex = column + row * _myTarget.TotalColumns;
                    var targetPiece = _myTarget.Pieces[targetIndex];

                    if (column < _newTotalColumns && row < _newTotalRows)
                    {
                        newPieces[column + row * _newTotalColumns] = targetPiece;
                    }
                    else
                    {
                        if (targetPiece != null)
                        {
                            DestroyImmediate(targetPiece.gameObject);
                        }
                    }
                }
            }

            return newPieces;
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
    }
}
