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

        private void OnDrawGizmosSelected()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = _selectedColor;
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
                var initialPoint = GetGridLinePoint(0, currentColumn);
                var finalPoint = GetGridLinePoint(rows, currentColumn);
                Gizmos.DrawLine(initialPoint, finalPoint);
            }
        }

        private static void DrawHorizontalGridLines(int rows, int columns)
        {
            for (int currentRow = 0; currentRow <= rows; currentRow++)
            {
                var initialPoint = GetGridLinePoint(currentRow, 0);
                var finalPoint = GetGridLinePoint(currentRow, columns);
                Gizmos.DrawLine(initialPoint, finalPoint);
            }
        }

        private static Vector3 GetGridLinePoint(int forRow, int forColumn)
        {
            return new Vector3(forColumn * GridCellSize, forRow * GridCellSize, 0);
        }

        public Vector3 WorldToGridCoordinates(Vector3 point)
        {
            var xCoordinate = (int) ((point.x - transform.position.x) / GridCellSize);
            var yCoordinate = (int) ((point.y - transform.position.y) / GridCellSize);

            return new Vector3(xCoordinate, yCoordinate, 0.0f);
        }

        public Vector3 GridToWorldCoordinates(int column, int row)
        {
            var xCoordinate = transform.position.x + (column * GridCellSize + GridCellSize / 2.0f);
            var yCoordinate = transform.position.y + (row * GridCellSize + GridCellSize / 2.0f);

            return new Vector3(xCoordinate, yCoordinate, 0.0f);
        }

        public bool IsInsideGridBounds(Vector3 point)
        {
            float minX = transform.position.x;
            float maxX = minX + _totalColumns * GridCellSize;
            float minY = transform.position.y;
            float maxY = minY + _totalRows * GridCellSize;

            return (minX <= point.x && point.x <= maxX) && (minY <= point.y && point.y <= maxY);
        }

        public bool IsInsideGridBounds(int col, int row)
        {
            return (0 <= col && col < _totalColumns) && (0 <= row && row < _totalRows);
        }

    }
}
