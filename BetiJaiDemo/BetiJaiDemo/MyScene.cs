using BetiJaiDemo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        private const int CameraSmoothTimeMilliseconds = 1000;

        private Vector3 cameraPositionCurrentVelocity;
        
        private Vector3 cameraRotationCurrentVelocity;
        
        private Vector3 cameraTargetPosition;
        
        private Vector3 cameraTargetRotation;
        
        private Transform3D cameraTransform;
        
        private Entity hotspotsRootEntity;
        
        private bool isCameraAnimationInProgress;
        
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
            
            this.zones = this.LoadZones();

            this.CreateHotspots();
            
            this.DisplayZoneWithItsHotspots(this.zones.First(), isAnimated: false);
        }

        protected override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            if (this.isCameraAnimationInProgress)
            {
                this.cameraTransform.Position = Vector3.SmoothDamp(
                    this.cameraTransform.Position,
                    this.cameraTargetPosition,
                    ref this.cameraPositionCurrentVelocity,
                    CameraSmoothTimeMilliseconds,
                    (float)gameTime.TotalMilliseconds);

                this.cameraTransform.Rotation = Vector3.SmoothDamp(
                    this.cameraTransform.Rotation,
                    this.cameraTargetRotation,
                    ref this.cameraRotationCurrentVelocity,
                    CameraSmoothTimeMilliseconds / 2,
                    (float)gameTime.TotalMilliseconds);

                if ((this.cameraTransform.Position == this.cameraTargetPosition) &&
                    (this.cameraTransform.Rotation == this.cameraTargetRotation))
                {
                    this.isCameraAnimationInProgress = false;
                }
            }
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
            var defaultMaterial = assetsService.Load<Material>(WaveContent.Materials.DefaultMaterial);
            this.hotspotsRootEntity = this.Managers.EntityManager.Find("hotspots");

            foreach (var item in hotspots)
            {
                var rawPosition = ParseVector3(item.Location);
                FixCoordinateSystemFromBabylonJS(ref rawPosition);

                var hotspotEntity = new Entity($"hotspot{item.Id}-{item.ZoneId}")
                    .AddComponent(new CubeMesh())
                    .AddComponent(new HotspotBehavior(item.Name))
                    .AddComponent(new MaterialComponent() { Material = defaultMaterial } )
                    .AddComponent(new MeshRenderer())
                    .AddComponent(new Transform3D { Position = rawPosition });
                this.hotspotsRootEntity.AddChild(hotspotEntity);
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
            this.cameraTransform = camera.FindComponent<Transform3D>();

            var position = ParseVector3(zone.Location);
            FixCoordinateSystemFromBabylonJS(ref position);
            position *= ScaleFactor;

            var rotation = ParseVector3(zone.Rotate);
            rotation *= -Vector3.One;

            if (isAnimated)
            {
                this.cameraTargetPosition = position;
                this.cameraTargetRotation = rotation;
                this.isCameraAnimationInProgress = true;
            }
            else
            {
                this.cameraTransform.Position = position;
                this.cameraTransform.Rotation = rotation;
                this.isCameraAnimationInProgress = false;
            }

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