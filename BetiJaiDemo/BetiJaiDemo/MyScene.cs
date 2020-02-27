using BetiJaiDemo.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace BetiJaiDemo
{
    public class MyScene : Scene
    {
        protected override async void CreateScene()
        {
            var zones = this.LoadZones();

            base.CreateScene();

            var camera = this.Managers.EntityManager.Find("camera");
            var transform = camera.FindComponent<Transform3D>();

            // Taken from comparing betijai.babylon meshes with those imported here through FBX
            const float ScaleFactor = 1 / 100f;

            foreach (var item in zones)
            {
                transform.Position = ParseVector3(item.Location) * ScaleFactor;
                //transform.LookAt(Vector3.Zero);
                var rotation = ParseVector3(item.Rotate);
                rotation.Y = rotation.Y + MathHelper.Pi;
                transform.Rotation = rotation;

                await System.Threading.Tasks.Task.Delay(3000);
            }
        }

        private static float Parse(string value) => float.Parse(value, CultureInfo.InvariantCulture);

        private static Vector3 ParseVector3(string value)
        {
            var valueSplit = value.Split(',');

            return new Vector3(
               Parse(valueSplit[0]),
               Parse(valueSplit[1]),
               Parse(valueSplit[2]));
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