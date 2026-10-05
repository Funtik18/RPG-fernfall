using System.IO;
using Fire.UI.Scroll;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fire.EditorTools.Scroll
{
    // Procedurally builds the Scroll wireframe scene/prefab so the layout doesn't have to be
    // hand-authored as scene/prefab YAML. Re-running this overwrites the scene and prefab.
    public static class ScrollWireframeBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/UI_ScrollWireframe.unity";
        private const string PrefabDir = "Assets/_Project/Prefabs/UI/Scroll";
        private const string PrefabPath = PrefabDir + "/RecordListItem.prefab";

        [MenuItem("Tools/Fire/Build Scroll Wireframe Scene")]
        public static void Build()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath) ?? "Assets/Scenes");
            Directory.CreateDirectory(PrefabDir);
            AssetDatabase.Refresh();

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // EventSystem
            var eventSystemGo = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            // Camera solely to silence Game view's "No cameras rendering" notice; the Canvas
            // below is Screen Space Overlay and renders without it, so this camera draws nothing.
            var cameraGo = new GameObject("Main Camera", typeof(UnityEngine.Camera));
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.GetComponent<UnityEngine.Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.12f, 0.12f);
            camera.cullingMask = 0;

            // Canvas
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var canvasRect = canvasGo.GetComponent<RectTransform>();

            // Left panel (~40%)
            var leftPanel = CreateStretchRect("LeftPanel", canvasRect, new Vector2(0f, 0f), new Vector2(0.4f, 1f));
            AddBackground(leftPanel, new Color(0.15f, 0.15f, 0.15f));
            var leftLayout = leftPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            ConfigurePanelLayout(leftLayout);

            var listTitle = CreateLabel("Title", leftPanel, "Свиток — записи отряда", font, 30, FontStyle.Bold);
            AddFixedHeight(listTitle.gameObject, 50);

            var listScrollGo = new GameObject("RecordScrollRect", typeof(RectTransform), typeof(ScrollRect), typeof(Image), typeof(Mask));
            listScrollGo.transform.SetParent(leftPanel, false);
            AddFlexible(listScrollGo);
            var listScrollRect = listScrollGo.GetComponent<ScrollRect>();
            var listScrollImage = listScrollGo.GetComponent<Image>();
            listScrollImage.color = new Color(0.1f, 0.1f, 0.1f);
            listScrollGo.GetComponent<Mask>().showMaskGraphic = true;
            listScrollRect.horizontal = false;
            listScrollRect.vertical = true;

            var listContent = CreateStretchRect("Content", listScrollGo.transform, Vector2.zero, Vector2.one);
            listContent.pivot = new Vector2(0.5f, 1f);
            listContent.anchorMin = new Vector2(0f, 1f);
            listContent.anchorMax = new Vector2(1f, 1f);
            listContent.offsetMin = new Vector2(0f, listContent.offsetMin.y);
            listContent.offsetMax = new Vector2(0f, listContent.offsetMax.y);
            var listContentLayout = listContent.gameObject.AddComponent<VerticalLayoutGroup>();
            listContentLayout.childForceExpandHeight = false;
            listContentLayout.childForceExpandWidth = true;
            listContentLayout.childControlHeight = true;
            listContentLayout.childControlWidth = true;
            listContentLayout.spacing = 6;
            var listContentFitter = listContent.gameObject.AddComponent<ContentSizeFitter>();
            listContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            listScrollRect.viewport = listScrollGo.GetComponent<RectTransform>();
            listScrollRect.content = listContent;

            // Right panel (~60%)
            var rightPanel = CreateStretchRect("RightPanel", canvasRect, new Vector2(0.4f, 0f), new Vector2(1f, 1f));
            AddBackground(rightPanel, new Color(0.2f, 0.2f, 0.2f));
            var rightLayout = rightPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            ConfigurePanelLayout(rightLayout);

            var header = CreateLabel("DetailHeader", rightPanel, "", font, 28, FontStyle.Bold);
            AddFixedHeight(header.gameObject, 60);

            var bodyScrollGo = new GameObject("DetailScrollRect", typeof(RectTransform), typeof(ScrollRect), typeof(Image), typeof(Mask));
            bodyScrollGo.transform.SetParent(rightPanel, false);
            AddFlexible(bodyScrollGo);
            var bodyScrollRect = bodyScrollGo.GetComponent<ScrollRect>();
            bodyScrollGo.GetComponent<Image>().color = new Color(0.16f, 0.16f, 0.16f);
            bodyScrollGo.GetComponent<Mask>().showMaskGraphic = true;
            bodyScrollRect.horizontal = false;
            bodyScrollRect.vertical = true;

            var bodyContent = CreateStretchRect("Content", bodyScrollGo.transform, Vector2.zero, Vector2.one);
            bodyContent.pivot = new Vector2(0.5f, 1f);
            bodyContent.anchorMin = new Vector2(0f, 1f);
            bodyContent.anchorMax = new Vector2(1f, 1f);
            var bodyContentLayout = bodyContent.gameObject.AddComponent<VerticalLayoutGroup>();
            bodyContentLayout.childForceExpandHeight = false;
            bodyContentLayout.childForceExpandWidth = true;
            bodyContentLayout.childControlHeight = true;
            bodyContentLayout.childControlWidth = true;
            bodyContentLayout.spacing = 8;
            bodyContentLayout.padding = new RectOffset(10, 10, 10, 10);
            var bodyContentFitter = bodyContent.gameObject.AddComponent<ContentSizeFitter>();
            bodyContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            bodyScrollRect.viewport = bodyScrollGo.GetComponent<RectTransform>();
            bodyScrollRect.content = bodyContent;

            var burnButtonGo = new GameObject("BurnButton", typeof(Image), typeof(Button));
            burnButtonGo.transform.SetParent(rightPanel, false);
            AddFixedHeight(burnButtonGo, 60);
            var burnButtonImage = burnButtonGo.GetComponent<Image>();
            burnButtonImage.color = new Color(0.4f, 0.15f, 0.1f);
            burnButtonGo.GetComponent<Button>().targetGraphic = burnButtonImage;
            var burnLabel = CreateLabel("Label", burnButtonGo.transform, "Сжечь", font, 26, FontStyle.Normal);
            var burnLabelRect = burnLabel.GetComponent<RectTransform>();
            burnLabelRect.anchorMin = Vector2.zero;
            burnLabelRect.anchorMax = Vector2.one;
            burnLabelRect.offsetMin = Vector2.zero;
            burnLabelRect.offsetMax = Vector2.zero;
            burnLabel.alignment = TextAnchor.MiddleCenter;

            // Controller
            var controllerGo = new GameObject("ScrollWireframeController", typeof(ScrollWireframeController));
            var controller = controllerGo.GetComponent<ScrollWireframeController>();
            controller.ListContent = listContent;
            controller.DetailHeaderText = header;
            controller.BodyContent = bodyContent;
            controller.BurnButton = burnButtonGo.GetComponent<Button>();
            controller.BurnButtonLabel = burnLabel;
            controller.ListItemPrefab = BuildRecordListItemPrefab(font);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Scroll wireframe scene built at {ScenePath}, list item prefab at {PrefabPath}.");
        }

        private static RecordListItem BuildRecordListItemPrefab(Font font)
        {
            var root = new GameObject("RecordListItem", typeof(Image), typeof(Button), typeof(LayoutElement), typeof(RecordListItem));
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(0, 90);
            root.GetComponent<LayoutElement>().minHeight = 90;
            var background = root.GetComponent<Image>();
            background.color = new Color(0.25f, 0.25f, 0.25f);
            root.GetComponent<Button>().targetGraphic = background;

            var layout = root.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 6, 6);
            layout.spacing = 2;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            var nameText = CreateLabel("NameText", root.transform, "Name", font, 22, FontStyle.Bold);
            var manorText = CreateLabel("ManorText", root.transform, "Manor", font, 18, FontStyle.Normal);
            var statusText = CreateLabel("StatusText", root.transform, "Status", font, 18, FontStyle.Italic);

            var indicatorGo = new GameObject("ConditionIndicator", typeof(Image));
            indicatorGo.transform.SetParent(root.transform, false);
            var indicatorRect = indicatorGo.GetComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(1f, 1f);
            indicatorRect.anchorMax = new Vector2(1f, 1f);
            indicatorRect.pivot = new Vector2(1f, 1f);
            indicatorRect.sizeDelta = new Vector2(20, 20);
            indicatorRect.anchoredPosition = new Vector2(-8, -8);
            var indicatorImage = indicatorGo.GetComponent<Image>();
            indicatorImage.color = Color.gray;

            var item = root.GetComponent<RecordListItem>();
            item.NameText = nameText;
            item.ManorText = manorText;
            item.StatusText = statusText;
            item.ConditionIndicator = indicatorImage;
            item.SelectButton = root.GetComponent<Button>();
            item.Background = background;

            var prefabAsset = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            return prefabAsset.GetComponent<RecordListItem>();
        }

        private static RectTransform CreateStretchRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static void AddBackground(RectTransform rect, Color color)
        {
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
        }

        private static void ConfigurePanelLayout(VerticalLayoutGroup layout)
        {
            layout.padding = new RectOffset(16, 16, 16, 16);
            layout.spacing = 10;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
        }

        private static void AddFixedHeight(GameObject go, float height)
        {
            var layoutElement = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;
            layoutElement.flexibleHeight = 0;
        }

        private static void AddFlexible(GameObject go)
        {
            var layoutElement = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1;
        }

        private static Text CreateLabel(string name, Transform parent, string text, Font font, int size, FontStyle style)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var label = go.GetComponent<Text>();
            label.font = font;
            label.fontSize = size;
            label.fontStyle = style;
            label.color = Color.white;
            label.text = text;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            return label;
        }
    }
}
