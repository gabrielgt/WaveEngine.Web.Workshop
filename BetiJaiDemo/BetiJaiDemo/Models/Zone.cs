using System.Collections.Generic;

namespace BetiJaiDemo.Models
{
    public class Zone
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string Rotate { get; set; }

        public List<object> CameraProperties { get; set; }
    }
}
