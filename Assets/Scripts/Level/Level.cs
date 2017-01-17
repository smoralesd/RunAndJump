using UnityEngine;

namespace RunAndJump
{
    public partial class Level : MonoBehaviour
    {
        [SerializeField]
        public int _totalTime = 60;
        [SerializeField]
        private float _gravity = -30;
        [SerializeField]
        private AudioClip _bgm;
        [SerializeField]
        private Sprite _background;
        [SerializeField]
        private int _totalColumns = 25;
        [SerializeField]
        private int _totalRows = 10;

        public const float GridCellSize = 1.28f;

        private readonly Color _normalColor = Color.grey;
        private readonly Color _selectedColor = Color.yellow;

        public int TotalTime
        {
            get { return _totalTime; }
            set { _totalTime = value; }
        }

        public float Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        public AudioClip Bgm
        {
            get { return _bgm; }
            set { _bgm = value; }
        }

        public Sprite Background
        {
            get { return _background; }
            set { _background = value; }
        }

        public int TotalRows
        {
            get { return _totalRows; }
            set { _totalRows = value; }
        }

        public int TotalColumns
        {
            get { return _totalColumns; }
            set { _totalColumns = value; }
        }

        private void OnDrawGizmos()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = _normalColor;
            DrawGridGizmo(_totalRows, _totalColumns);
            Gizmos.color = oldColor;
        }

        private void DrawGridGizmo(int rows, int columns)
        {
            DrawVerticalGridLines(rows, columns);
            DrawHorizontalGridLines(rows, columns);
        }

        private static void DrawVerticalGridLines(int rows, int columns)
        {
            for (int currentColumn = 0; currentColumn <= columns; currentColumn++)
            {
                var initialPoint = GetVerticalLineInitialPoint(currentColumn);
                var finalPoint = GetVerticalLineFinalPoint(currentColumn, rows);
                Gizmos.DrawLine(initialPoint, finalPoint);
            }
        }

        private static Vector3 GetVerticalLineInitialPoint(int forColumn)
        {
            return new Vector3(forColumn * GridCellSize, 0, 0);
        }

        private static Vector3 GetVerticalLineFinalPoint(int forColumn, int forRow)
        {
            return new Vector3(forColumn * GridCellSize, forRow * GridCellSize, 0);
        }

        private static void DrawHorizontalGridLines(int rows, int columns)
        {
            for (int currentRow = 0; currentRow <= rows; currentRow++)
            {
                var initialPoint = GetHorizontalLineInitialPoint(currentRow);
                var finalPoint = GetHorizontalLineFinalPoint(currentRow, columns);
                Gizmos.DrawLine(initialPoint, finalPoint);
            }
        }

        private static Vector3 GetHorizontalLineInitialPoint(int forRow)
        {
            return new Vector3(0, forRow * GridCellSize, 0);
        }

        private static Vector3 GetHorizontalLineFinalPoint(int forRow, int forColumn)
        {
            return new Vector3(forColumn * GridCellSize, forRow * GridCellSize, 0);
        }
    }
}
