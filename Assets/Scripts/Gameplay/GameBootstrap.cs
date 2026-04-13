using CaravanRoguelite.Generation;
using CaravanRoguelite.Map;
using CaravanRoguelite.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CaravanRoguelite.Gameplay
{
    public class GameBootstrap : MonoBehaviour
    {
        private GameContext _context;

        private void Start()
        {
            EnsureCamera();
            EnsureEventSystem();
            var canvas = BuildCanvas();
            BuildBackdrop(canvas.transform);

            _context = new GameContext();
            _context.Hud = new GameHud(canvas.transform);
            _context.MapView = new MapView(canvas.transform);
            _context.Panel = new ChoicePanel(canvas.transform);
            _context.Graph = new MapGenerator().Create(Random.Range(0, 999999));
            _context.CurrentNodeId = 0;

            _context.Victory = () => _context.StateMachine.ChangeState(new EndState(_context, true));
            _context.Defeat = () => _context.StateMachine.ChangeState(new EndState(_context, false));

            _context.StateMachine.ChangeState(new TravelState(_context));
        }

        private void Update()
        {
            _context?.StateMachine.Tick();
            _context?.MapView.Tick(Time.time);
        }

        private void EnsureCamera()
        {
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = VisualTheme.BackgroundBottom;
                return;
            }

            var cameraGo = new GameObject("Main Camera", typeof(Camera));
            cameraGo.tag = "MainCamera";
            var cam = cameraGo.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = VisualTheme.BackgroundBottom;
            cam.orthographic = true;
        }

        private Canvas BuildCanvas()
        {
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);

            return canvas;
        }

        private void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private void BuildBackdrop(Transform parent)
        {
            var bg = new GameObject("Backdrop", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            bg.SetParent(parent, false);
            bg.anchorMin = Vector2.zero;
            bg.anchorMax = Vector2.one;
            bg.offsetMin = Vector2.zero;
            bg.offsetMax = Vector2.zero;
            bg.SetAsFirstSibling();

            var image = bg.GetComponent<Image>();
            image.sprite = ProceduralSpriteFactory.CreateProceduralBackdrop(VisualTheme.BackgroundTop, VisualTheme.BackgroundBottom, VisualTheme.Accent, 768, 512, Random.Range(0, 99999));
            image.type = Image.Type.Simple;
            image.raycastTarget = false;

            var haze = new GameObject("BackdropHaze", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            haze.SetParent(bg, false);
            haze.anchorMin = new Vector2(-0.1f, -0.05f);
            haze.anchorMax = new Vector2(1.1f, 1.05f);
            haze.offsetMin = Vector2.zero;
            haze.offsetMax = Vector2.zero;
            var hazeImage = haze.GetComponent<Image>();
            hazeImage.sprite = ProceduralSpriteFactory.CreateSoftCircle(new Color(0.2f, 0.4f, 0.7f, 0.18f), 512, 2.3f);
            hazeImage.raycastTarget = false;
        }
    }
}
