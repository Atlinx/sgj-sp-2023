using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// Transitions between scenes
    /// </summary>
    public partial class SceneManager : Node, IService
    {
        [Signal]
        public delegate void TransitionFinishedEventHandler(PackedScene scene);
        [Signal]
        public delegate void SceneLoadedEventHandler(PackedScene scene);
        [Signal]
        public delegate void TransitionStartedEventHandler(PackedScene scene);

        public enum Scene
        {
            MainMenu,
            Gameplay
        }

        public bool IsTransitioning { get; private set; } = false;
        public PackedScene TransitionToScene { get; private set; }

        [ExportCategory("Settings")]
        [Export]
        private PackedScene[] scenes;
        [Export]
        private float fadeDuration = 0.5f;
        [Export]
        private Color fadeColor = Colors.White;
        [Export]
        private Scene startScene = Scene.MainMenu;
        [Export]
        private float titleDuration = 0.5f;

        [ExportCategory("Dependencies")]
        [Export]
        private ColorRect fadeRect;

        public override async void _Ready()
        {
            await ToSignal(GetTree().CreateTimer(titleDuration), "timeout");
            await TransitionTo(startScene);
        }

        public Task TransitionTo(Scene scene, Action finishedCallback = null, Action loadedCallback = null) => TransitionTo(Enum.GetName(scene), finishedCallback, loadedCallback);
        public Task TransitionTo(string name, Action finishedCallback = null, Action loadedCallback = null)
        {
            var targetScene = scenes.FirstOrDefault(x => x.ResourcePath.GetFile().GetBaseName() == name);
            if (targetScene == null)
            {
                GD.PushError($"{nameof(SceneManager)}: Scene name could not be found");
                return null;
            }
            return TransitionTo(targetScene, finishedCallback, loadedCallback);
        }

        public async Task TransitionTo(PackedScene scene, Action finishedCallback = null, Action loadedCallback = null)
        {
            if (IsTransitioning) return;
            IsTransitioning = true;
            TransitionToScene = scene;
            var tween = GetTree().CreateTween();
            await TransitionFirstHalf(tween, finishedCallback);
            _ = TransitionLastHalf(tween, loadedCallback); // Fire and forget
        }

        public Task TransitionToAwaitFull(Scene scene, Action finishedCallback = null, Action loadedCallback = null) => TransitionToAwaitFull(Enum.GetName(scene), finishedCallback, loadedCallback);
        public Task TransitionToAwaitFull(string name, Action finishedCallback = null, Action loadedCallback = null)
        {
            var targetScene = scenes.FirstOrDefault(x => x.ResourcePath.GetFile().GetBaseName() == name);
            if (targetScene == null)
            {
                GD.PushError($"{nameof(SceneManager)}: Scene name could not be found");
                return null;
            }
            return TransitionToAwaitFull(targetScene, finishedCallback, loadedCallback);
        }
        public async Task TransitionToAwaitFull(PackedScene scene, Action finishedCallback = null, Action loadedCallback = null)
        {
            if (IsTransitioning) return;
            IsTransitioning = true;
            TransitionToScene = scene;
            var tween = GetTree().CreateTween();
            await TransitionFirstHalf(tween, finishedCallback);
            await TransitionLastHalf(tween, loadedCallback);
        }

        private async Task TransitionFirstHalf(Tween tween, Action loadedCallback = null)
        {
            EmitSignal(nameof(TransitionStarted), TransitionToScene);

            var fadeColorTransparent = fadeColor;
            fadeColorTransparent.A = 0;

            fadeRect.Color = fadeColorTransparent;

            tween.TweenProperty(fadeRect, "color", fadeColor, fadeDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            tween.Play();

            await ToSignal(tween, "finished");
            tween.Stop();

            GetTree().ChangeSceneToPacked(TransitionToScene);
            await ToSignal(GetTree(), "process_frame");

            EmitSignal(nameof(SceneLoaded), TransitionToScene);
            if (loadedCallback != null)
                loadedCallback();
        }

        private async Task TransitionLastHalf(Tween tween, Action finishedCallback = null)
        {
            var fadeColorTransparent = fadeColor;
            fadeColorTransparent.A = 0;

            tween.TweenProperty(fadeRect, "color", fadeColorTransparent, fadeDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            tween.Play();

            await ToSignal(tween, "finished");
            tween.Stop();

            IsTransitioning = false;
            if (finishedCallback != null)
                finishedCallback();
            EmitSignal(nameof(TransitionFinished), TransitionToScene);
        }
    }
}