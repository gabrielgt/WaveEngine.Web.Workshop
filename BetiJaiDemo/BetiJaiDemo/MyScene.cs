using BetiJaiDemo.Behaviors;
using BetiJaiDemo.Models;
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

        protected override void CreateScene()
        {
            DisableMultithreadingStuff();
            CreateHotspots();
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
                    .AddComponent(new MeshRenderer());

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