using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace BetiJaiDemo.Behaviors
{
    public class LookAtCameraBehavior : Behavior
    {
        private Camera3D _camera;

        [BindComponent] public Transform3D Transform;

        protected override void OnActivated()
        {
            base.OnActivated();

            _camera = Owner.Scene.Managers.RenderManager.ActiveCamera3D;
        }

        protected override void Update(TimeSpan gameTime)
        {
            Transform.LookAt(_camera.Transform.Position);
        }
    }
}