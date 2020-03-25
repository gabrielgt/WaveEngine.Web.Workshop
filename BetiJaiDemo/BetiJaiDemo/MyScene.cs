using BetiJaiDemo.Behaviors;
using BetiJaiDemo.Models;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Graphics.Batchers;
using WaveEngine.Framework.Services;
using WaveEngine.Mathematics;

namespace BetiJaiDemo
{
    public class MyScene : Scene
    {
        private const string HotspotTag = "hotspot";

        private IEnumerable<Zone> zones;

        internal void DisplayZone(int id)
        {
            var zone = this.zones
                .Where(item => item.Id == id)
                .First();

            this.DisplayZoneWithItsHotspots(zone);
        }

        protected override void CreateScene()
        {
            base.CreateScene();

            this.DisableMultithreadingStuff();

            var zoneList = JsonHelper.Deserialize<ZoneList>("Content/Raw/zones.json");
            this.zones = zoneList.Zones;

            this.CreateHotspots();
        }

        protected override void Start()
        {
            base.Start();

            this.DisplayZoneWithItsHotspots(this.zones.First(), isAnimated: false);
        }

        private static void FixCoordinateSystemFromBabylonJS(ref Vector3 position) => position.Z *= -1;

        private void CreateHotspots()
        {
            var hotspotList = JsonHelper.Deserialize<HotspotList>("Content/Raw/hotspots.json");
            var hotspots = hotspotList.Hotspots;

            var assetsService = Application.Current.Container.Resolve<AssetsService>();
            var material = assetsService.Load<Material>(WaveContent.Materials.HotspotMaterial);
            
            const float hotspotSideMeters = 2.5f;

            foreach (var item in hotspots)
            {
                var rawPosition = JsonHelper.ParseVector3(item.Location);
                FixCoordinateSystemFromBabylonJS(ref rawPosition);

                var wrapperEntity = new Entity()
                    .AddComponent(new LookAtCameraBehavior())
                    .AddComponent(new Transform3D { Position = rawPosition });

                var hotspotEntity = new Entity($"hotspot{item.Id}-{item.ZoneId}") { Tag = HotspotTag }
                    .AddComponent(new HotspotBehavior(item.Name))
                    .AddComponent(new MaterialComponent() { Material = material } )
                    .AddComponent(new MeshRenderer())
                    .AddComponent(new PlaneMesh())
                    // Spinner is quite usefull to understand how rotation happens in each axe
                    //.AddComponent(new Spinner { AxisIncrease = Vector3.UnitY })
                    .AddComponent(new Transform3D
                    {
                        Orientation = Quaternion.CreateFromEuler(new Vector3(MathHelper.PiOver2, MathHelper.Pi, 0)),
                        Scale = new Vector3(hotspotSideMeters, 1, hotspotSideMeters)
                    });

                wrapperEntity.AddChild(hotspotEntity);
                this.Managers.EntityManager.Add(wrapperEntity);
            }
        }

        private void DisableMultithreadingStuff()
        {
            var meshRenderFeature = this.Managers.RenderManager.FindRenderFeature<MeshRenderFeature>();
            var dynamicBatchMeshProcessor = meshRenderFeature.FindMeshProcessor<DynamicBatchMeshProcessor>();
            dynamicBatchMeshProcessor.IsActivated = false;
        }

        private void DisplayZoneWithItsHotspots(Zone zone, bool isAnimated = true)
        {
            // Taken from comparing betijai.babylon meshes with those imported here through FBX
            const float ScaleFactor = 1 / 100f;

            var camera = this.Managers.EntityManager.Find("camera");
            var cameraTravelling = camera.FindComponent<CameraTravellingBehavior>();

            var position = JsonHelper.ParseVector3(zone.Location);
            FixCoordinateSystemFromBabylonJS(ref position);
            position *= ScaleFactor;

            var rotation = JsonHelper.ParseVector3(zone.Rotate);
            rotation *= -Vector3.One;

            if (isAnimated)
            {
                cameraTravelling.AnimateTo(position, rotation);
            }
            else
            {
                cameraTravelling.FixTo(position, rotation);
            }

            var hotspots = this.Managers.EntityManager.FindAllByTag(HotspotTag);

            foreach (var item in hotspots)
            {
                item.IsEnabled = false;
            }

            var currentZoneHotspots = hotspots.Where(hotspot => hotspot.Name.EndsWith(zone.Id.ToString()));

            foreach (var item in currentZoneHotspots)
            {
                item.IsEnabled = true;
            }
        }
    }
}