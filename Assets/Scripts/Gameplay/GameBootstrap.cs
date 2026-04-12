using CaravanRoguelite.Generation;
using CaravanRoguelite.Map;
using CaravanRoguelite.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.Gameplay
{
    public class GameBootstrap : MonoBehaviour
    {
        private GameContext _context;

        private void Start()
        {
            EnsureCamera();
            var canvas = BuildCanvas();

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
        }

        private void EnsureCamera()
        {
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = new Color(0.05f, 0.06f, 0.08f);
                return;
            }

            var cameraGo = new GameObject("Main Camera", typeof(Camera));
            cameraGo.tag = "MainCamera";
            var cam = cameraGo.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.06f, 0.08f);
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
    }
}
