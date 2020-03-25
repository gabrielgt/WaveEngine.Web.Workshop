using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace BetiJaiDemo.Behaviors
{
    public class LookAtCameraBehavior : Behavior
    {
        [BindComponent]
        public Transform3D transform;

        private Camera3D camera;

        protected override void OnActivated()
        {
            base.OnActivated();

            this.camera = this.Owner.Scene.Managers.RenderManager.ActiveCamera3D;
        }

        protected override void Update(TimeSpan gameTime) => this.transform.LookAt(this.camera.Transform.Position);
    }
}