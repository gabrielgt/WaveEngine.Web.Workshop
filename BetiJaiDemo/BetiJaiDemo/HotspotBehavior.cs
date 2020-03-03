using System;
using WaveEngine.Common.Input;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace BetiJaiDemo
{
    internal class HotspotBehavior : Behavior
    {
        private readonly string name;
        
        private Camera3D activeCamera3D;
        
        private BoundingSphere boundingSphere;
        
        private MouseDispatcher mouseDispatcher;

        [BindComponent]
        private Transform3D transform3D;

        public HotspotBehavior(string name)
        {
            this.name = name;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            this.activeCamera3D = this.Owner.Scene.Managers.RenderManager.ActiveCamera3D;
            this.mouseDispatcher = activeCamera3D.Display.MouseDispatcher;
        }

        protected override bool OnAttached()
        {
            this.boundingSphere = new BoundingSphere(this.transform3D.Position, 1);

            return base.OnAttached();
        }

        protected override void Update(TimeSpan gameTime)
        {
            var leftButtonState = this.mouseDispatcher.ReadButtonState(MouseButtons.Left);

            if (leftButtonState == ButtonState.Releasing)
            {
                var mousePosition = this.mouseDispatcher.Position.ToVector2();
                activeCamera3D.CalculateRay(ref mousePosition, out var ray);
                var intersection = this.boundingSphere.Intersects(ray);
                var rayIntersects = intersection.HasValue && intersection.Value > 0;
                
                if (rayIntersects)
                {
                    System.Diagnostics.Trace.WriteLine($"'{this.name}' clicked");
                }
            }
        }
    }
}