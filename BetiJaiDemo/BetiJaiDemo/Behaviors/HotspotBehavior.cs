using System;
using WaveEngine.Common.Input;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace BetiJaiDemo.Behaviors
{
    internal class HotspotBehavior : Behavior
    {
        private readonly string name;
        
        [BindComponent]
        public Transform3D transform3D;

        private Camera3D activeCamera3D;
        
        private BoundingSphere boundingSphere;
        
        private MouseDispatcher mouseDispatcher;

        public HotspotBehavior(string name)
        {
            this.name = name;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            this.boundingSphere = new BoundingSphere(this.transform3D.Position, (this.transform3D.Scale.X / 2) + 0.5f);

            this.activeCamera3D = this.Owner.Scene.Managers.RenderManager.ActiveCamera3D;
            this.mouseDispatcher = activeCamera3D.Display.MouseDispatcher;
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

                    var notifier = Application.Current.Container.Resolve<IHotspotNotifier>();
                    notifier?.Notify(this.name);
                }
            }
        }
    }
}