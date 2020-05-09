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
        private const float HotspotSideMeters = 2;
        private IEnumerable<Zone> _zones;

        protected override void CreateScene()
        {
            DisableMultithreadingStuff();
            CreateHotspots();
            CreateZones();
        }

        protected override void Start()
        {
            base.Start();
            DisplayZoneWithItsHotspots(_zones.First(), isAnimated: false);
        }

        internal void DisplayZone(int id)
        {
            var zone = _zones.First(item => item.Id == id);
            DisplayZoneWithItsHotspots(zone);
        }

        private void CreateZones()
        {
            var zoneList = JsonHelper.Deserialize<ZoneList>("Content/Raw/zones.json");
            _zones = zoneList.Zones;
        }

        private void DisplayZoneWithItsHotspots(Zone zone, bool isAnimated = true)
        {
            // Taken from comparing betijai.babylon meshes with those imported here through FBX
            const float scaleFactor = 1 / 100f;

            var position = JsonHelper.ParseVector3(zone.Location);
            FixCoordinateSystemFromBabylonJs(ref position);
            position *= scaleFactor;

            var rotation = JsonHelper.ParseVector3(zone.Rotate);
            rotation *= -Vector3.One;

            var camera = Managers.RenderManager.ActiveCamera3D;
            var cameraTravelling = camera.Owner.FindComponent<CameraTravellingBehavior>();

            if (isAnimated)
            {
                cameraTravelling.AnimateTo(position, rotation);
            }
            else
            {
                cameraTravelling.FixTo(position, rotation);
            }

            ShowHotspotsByZone(zone);
        }

        private void ShowHotspotsByZone(Zone zone)
        {
            var hotspots = Managers.EntityManager.FindAllByTag(HotspotTag).ToList();
            hotspots.ForEach(h => h.IsEnabled = h.Name.EndsWith($"-{zone.Id}"));
        }

        private void CreateHotspots()
        {
            var hotspotList = JsonHelper.Deserialize<HotspotList>("Content/Raw/hotspots.json");
            var hotspots = hotspotList.Hotspots;

            var assetsService = Application.Current.Container.Resolve<AssetsService>();
            var material = assetsService.Load<Material>(WaveContent.Materials.HotspotMaterial);

            foreach (var item in hotspots)
            {
                var rawPosition = JsonHelper.ParseVector3(item.Location);
                FixCoordinateSystemFromBabylonJs(ref rawPosition);

                var wrapperEntity = new Entity()
                    .AddComponent(new LookAtCameraBehavior())
                    .AddComponent(new Transform3D {Position = rawPosition});

                var hotspotEntity = new Entity($"hotspot{item.Id}-{item.ZoneId}") {Tag = HotspotTag}
                    .AddComponent(new PlaneMesh())
                    .AddComponent(new MaterialComponent {Material = material})
                    .AddComponent(
                        new Transform3D
                        {
                            Rotation = new Vector3(MathHelper.PiOver2, MathHelper.Pi, 0),
                            Scale = new Vector3(HotspotSideMeters, 1, HotspotSideMeters)
                        })
                    .AddComponent(new MeshRenderer())
                    .AddComponent(new HotspotBehavior(item.Name));

                wrapperEntity.AddChild(hotspotEntity);
                Managers.EntityManager.Add(wrapperEntity);
            }
        }

        private void DisableMultithreadingStuff()
        {
            var meshRenderFeature = Managers.RenderManager.FindRenderFeature<MeshRenderFeature>();
            var dynamicBatchMeshProcessor = meshRenderFeature.FindMeshProcessor<DynamicBatchMeshProcessor>();
            dynamicBatchMeshProcessor.IsActivated = false;
        }

        private static void FixCoordinateSystemFromBabylonJs(ref Vector3 position)
        {
            position.Z *= -1;
        }
    }
}