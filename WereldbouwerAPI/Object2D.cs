using System.ComponentModel.DataAnnotations;

namespace WereldbouwerAPI
{
    public class Object2D
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public Guid environmentId { get; set; }
        [Required]
        public string prefabId { get; set; }
        public float positionX { get; set; }
        public float positionY { get; set; }
        public float scaleX { get; set; }
        public float scaleY { get; set; }
        public float rotationZ { get; set; }
        public int sortingLayer { get; set; }
    }
}
