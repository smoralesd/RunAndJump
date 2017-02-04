using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace RunAndJump.LevelCreator
{
    public class PaletteWindow : EditorWindow
    {
        public static PaletteWindow instance;

        private IList<PaletteItem.Category> _categories;
        private IList<string> _categoryLabels;
        private PaletteItem.Category _selectedCategory;

        private string _levelPiecesPath = "Assets/Prefabs/LevelPieces";
        private IList<PaletteItem> _items;
        private IDictionary<PaletteItem.Category, IList<PaletteItem>> _categorizedItems;
        private IDictionary<PaletteItem, Texture2D> _previews;

        private Vector2 _scrollPosition;

        private const float ButtonWidth = 80;
        private const float ButtonHeight = 90;

        public void OnEnable()
        {
            if (_categories == null)
            {
                InitCategories();
            }

            if (_categorizedItems == null)
            {
                InitContent();
            }
        }

        public void OnGUI()
        {
            DrawTabs();
            DrawScroll();
        }

        public void Update()
        {
            if (_previews.Count != _items.Count)
            {
                GeneratePreviews();
            }
        }

        public void InitCategories()
        {
            _categories = EditorUtils.GetListFromEnum<PaletteItem.Category>();
            _categoryLabels = new List<string>();

            foreach (var category in _categories)
            {
                _categoryLabels.Add(category.ToString());
            }
        }

        public void InitContent()
        {
            _items = EditorUtils.GetAssestsWithScript<PaletteItem>(_levelPiecesPath);
            _categorizedItems = new Dictionary<PaletteItem.Category, IList<PaletteItem>>();
            _previews = new Dictionary<PaletteItem, Texture2D>();

            foreach (var category in _categories)
            {
                _categorizedItems.Add(category, new List<PaletteItem>());
            }

            foreach (var item in _items)
            {
                _categorizedItems[item.category].Add(item);
            }
        }

        public static void ShowPalette()
        {
            instance = GetWindow<PaletteWindow>();
            instance.titleContent = new GUIContent("Palette");
        }

        public void DrawTabs()
        {
            var index = (int)_selectedCategory;
            index = GUILayout.Toolbar(index, _categoryLabels.ToArray());
            _selectedCategory = _categories[index];
        }

        private void DrawScroll()
        {
            if (_categorizedItems[_selectedCategory].Count == 0)
            {
                EditorGUILayout.HelpBox("This category is empty!", MessageType.Info);
                return;
            }

            var rowCapacity = Mathf.FloorToInt(position.width / (ButtonWidth));
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            var selectionGridIndex = -1;
            selectionGridIndex = GUILayout.SelectionGrid(
                selectionGridIndex,
                GetGUIContentsFromItems(),
                rowCapacity,
                GetGUIStyle());

            GUILayout.EndScrollView();
        }

        private GUIContent[] GetGUIContentsFromItems()
        {
            var guiContents = new List<GUIContent>();

            if (_previews.Count == _items.Count)
            {
                var selectedCategory = _categorizedItems[_selectedCategory];

                foreach (var paletteItem in selectedCategory)
                {
                    var guiContent = new GUIContent();

                    guiContent.text = paletteItem.itemName;
                    guiContent.image = _previews[paletteItem];
                    guiContents.Add(guiContent);
                }
            }

            return guiContents.ToArray();
        }

        private GUIStyle GetGUIStyle()
        {
            var guiStyle = new GUIStyle(GUI.skin.button);
            guiStyle.alignment = TextAnchor.LowerCenter;
            guiStyle.imagePosition = ImagePosition.ImageAbove;
            guiStyle.fixedHeight = ButtonHeight;
            guiStyle.fixedWidth = ButtonWidth;

            return guiStyle;
        }

        private void GeneratePreviews()
        {
            foreach (var item in _items)
            {
                var preview = AssetPreview.GetAssetPreview(item.gameObject);
                if (preview != null)
                {
                    _previews.Add(item, preview);
                }
            }
        }
    }
}