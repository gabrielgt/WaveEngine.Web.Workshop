using System;
using System.Diagnostics;
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

        private Camera3D _activeCamera3D;

        private BoundingSphere _boundingSphere;

        private MouseDispatcher _mouseDispatcher;

        [BindComponent]
        public Transform3D Transform3D;

        public HotspotBehavior(string name)
        {
            this.name = name;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            _boundingSphere = new BoundingSphere(Transform3D.Position, (Transform3D.Scale.X / 2) + 0.5f);

            _activeCamera3D = Owner.Scene.Managers.RenderManager.ActiveCamera3D;
            _mouseDispatcher = _activeCamera3D.Display.MouseDispatcher;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var leftButtonState = _mouseDispatcher.ReadButtonState(MouseButtons.Left);

            if (leftButtonState == ButtonState.Releasing)
            {
                var mousePosition = _mouseDispatcher.Position.ToVector2();
                _activeCamera3D.CalculateRay(ref mousePosition, out var ray);
                var intersection = _boundingSphere.Intersects(ray);
                var rayIntersects = intersection.HasValue && intersection.Value > 0;

                if (rayIntersects)
                {
                    Trace.WriteLine($"'{name}' clicked");

                    var notifier = Application.Current.Container.Resolve<IHotspotNotifier>();
                    notifier?.Notify(name);
                }
            }
        }
    }
}