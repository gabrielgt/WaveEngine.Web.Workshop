using System;
using System.Diagnostics;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Mathematics;

namespace BetiJaiDemo.Behaviors
{
    internal class HotspotBehavior : Behavior
    {
        private readonly string _name;
        private Camera3D _activeCamera3D;
        private BoundingSphere _boundingSphere;
        private MouseDispatcher _mouseDispatcher;
        private Material _rayMaterial;
        private Material _sphereMaterial;

        [BindComponent]
        public Transform3D Transform3D;

        public HotspotBehavior(string name)
        {
            _name = name;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            _boundingSphere = new BoundingSphere(Transform3D.Position, (Transform3D.Scale.X / 2) + 0.1f);
            _activeCamera3D = Owner.Scene.Managers.RenderManager.ActiveCamera3D;
            _mouseDispatcher = _activeCamera3D.Display.MouseDispatcher;
            _mouseDispatcher.MouseButtonUp += MouseDispatcherOnMouseButtonUp;

#if DEBUG
            var assetsService = Application.Current.Container.Resolve<AssetsService>();
            _rayMaterial = assetsService.Load<Material>(WaveContent.Materials.RayMaterial);
            _sphereMaterial = assetsService.Load<Material>(WaveContent.Materials.SphereMaterial);
#endif
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            _mouseDispatcher.MouseButtonUp -= MouseDispatcherOnMouseButtonUp;
        }

        protected override void Update(TimeSpan gameTime)
        {
        }

        private void MouseDispatcherOnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var mousePosition = e.Position.ToVector2();
            _activeCamera3D.CalculateRay(ref mousePosition, out var ray);
            var intersection = _boundingSphere.Intersects(ray);
            var rayIntersects = intersection.HasValue && intersection.Value > 0;

            if (rayIntersects)
            {
                Trace.WriteLine($"'{_name}' clicked");
#if DEBUG
                DrawSphere();
#endif
                var notifier = Application.Current.Container.Resolve<IHotspotNotifier>();
                notifier?.Notify(_name);
            }
#if DEBUG
            DrawRay(ray);
#endif
        }

        private void DrawRay(Ray ray)
        {
            var rayEntity = new Entity()
                .AddComponent(new CylinderMesh())
                .AddComponent(new MaterialComponent {Material = _rayMaterial})
                .AddComponent(new MeshRenderer())
                .AddComponent(
                    new Transform3D
                    {
                        Orientation = Quaternion.CreateFromEuler(new Vector3(MathHelper.PiOver2, 0f, 0f)),
                        Scale = new Vector3(0.025f, 100f, 0.025f)
                    });

            var transform3D = new Transform3D
            {
                Position = ray.GetPoint(52f),
            };

            var wrapperEntity = new Entity()
                .AddChild(rayEntity)
                .AddComponent(transform3D);

            transform3D.LookAt(ray.GetPoint(100f));

            Managers.EntityManager.Add(wrapperEntity);
        }

        private void DrawSphere()
        {
            var sphereEntity = new Entity()
                .AddComponent(new SphereMesh())
                .AddComponent(new MaterialComponent {Material = _sphereMaterial})
                .AddComponent(new MeshRenderer())
                .AddComponent(new Transform3D {Position = _boundingSphere.Center, Scale = new Vector3(_boundingSphere.Radius * 2)});

            Managers.EntityManager.Add(sphereEntity);
        }
    }
}