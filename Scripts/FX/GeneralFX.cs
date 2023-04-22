using Godot;

namespace Game
{
    public interface IFX
    {
        void Play();
    }

    public partial class GeneralFX : Node2D, IFX
    {
        [ExportCategory("Dependencies")]
        [Export]
        private NodePath[] fxNodes;

        public virtual void Play()
        {
            if (fxNodes != null)
                foreach (var x in fxNodes)
                {
                    var node = GetNode(x);
                    if (node is IFX fx)
                        fx.Play();
                    else if (node is CpuParticles2D cpuParticles)
                        cpuParticles.Emitting = true;
                    else if (node is GpuParticles2D gpuParticles)
                        gpuParticles.Emitting = true;
                    else if (node is AudioStreamPlayer audioPlayer)
                        audioPlayer.Play();
                    else if (node is AudioStreamPlayer2D audioPlayer2D)
                        audioPlayer2D.Play();
                    else if (node is AnimationPlayer animPlayer)
                        animPlayer.Play();
                }
        }
    }
}