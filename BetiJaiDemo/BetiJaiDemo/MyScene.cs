using BetiJaiDemo.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Mathematics;

namespace BetiJaiDemo
{
    public class MyScene : Scene
    {
        private Entity hotspotsRootEntity;

        protected override void CreateScene()
        {
            var zones = this.LoadZones();

            this.CreateHotspots();
            
            this.DisplayZoneWithItsHotspots(zones.ElementAt(0));
            
            base.CreateScene();
        }

        private static void FixCoordinateSystemFromBabylonJS(ref Vector3 position) => position.Z *= -1;

        private static float Parse(string value) => float.Parse(value, CultureInfo.InvariantCulture);

        private static Vector3 ParseVector3(string value)
        {
            var valueSplit = value.Split(',');

            return new Vector3(
               Parse(valueSplit[0]),
               Parse(valueSplit[1]),
               Parse(valueSplit[2]));
        }

        private void CreateHotspots()
        {
            var hotspots = this.LoadHotspots();

            var assetsService = Application.Current.Container.Resolve<AssetsService>();
            // FIXME This line freezes in Wasm, I think because using async IO underneath
            Material defaultMaterial = assetsService.Load<Material>(WaveContent.Materials.DefaultMaterial);
            this.hotspotsRootEntity = this.Managers.EntityManager.Find("hotspots");

            foreach (var item in hotspots)
            {
                var rawPosition = ParseVector3(item.Location);
                FixCoordinateSystemFromBabylonJS(ref rawPosition);

                var hotspotEntity = new Entity($"hotspot{item.Id}-{item.ZoneId}")
                    .AddComponent(new CubeMesh())
                    .AddComponent(new MaterialComponent() { Material = defaultMaterial } )
                    .AddComponent(new MeshRenderer())
                    .AddComponent(new Transform3D { Position = rawPosition });
                this.hotspotsRootEntity.AddChild(hotspotEntity);
            }
        }

        private void DisplayZoneWithItsHotspots(Zone zone)
        {
            // Taken from comparing betijai.babylon meshes with those imported here through FBX
            const float ScaleFactor = 1 / 100f;

            var camera = this.Managers.EntityManager.Find("camera");
            var transform = camera.FindComponent<Transform3D>();

            var rawPosition = ParseVector3(zone.Location);
            FixCoordinateSystemFromBabylonJS(ref rawPosition);
            transform.Position = rawPosition * ScaleFactor;

            var rawRotation = ParseVector3(zone.Rotate);
            rawRotation *= -Vector3.One;
            transform.Rotation = rawRotation;

            foreach (var hotspot in this.hotspotsRootEntity.ChildEntities)
            {
                hotspot.IsEnabled = false;
            }

            var currentZoneHotspots = this.hotspotsRootEntity.ChildEntities
                .Where(hotspot => hotspot.Name.EndsWith(zone.Id.ToString()));

            foreach (var hotspot in currentZoneHotspots)
            {
                hotspot.IsEnabled = true;
            }
        }

        private IEnumerable<Hotspot> LoadHotspots()
        {
            var json = File.ReadAllText("Content/Raw/hotspots.json");
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var hotSpotList = JsonSerializer.Deserialize<HotspotList>(json, options);

            return hotSpotList.Hotspots;
        }

        private IEnumerable<Zone> LoadZones()
        {
            var json = File.ReadAllText("Content/Raw/zones.json");
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var zoneList = JsonSerializer.Deserialize<ZoneList>(json, options);

            return zoneList.Zones;
        }
    }
}