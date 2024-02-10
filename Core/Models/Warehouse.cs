using Core.DataStructures;

namespace Core.Models
{
    /// <summary>
    /// The warehouse model
    /// Has a dictionary whose key is the size and value will be the boxes themselves
    /// </summary>
    public class Warehouse
    {
        public Dictionary<(double size, double height), Boxes> Inventory { get; } = new();
        public SizeRegistry SizeRegistry { get; } = new();
        public Queue<BoxBatch> ExpirationDateRegistry { get; } = new();
    }
}
