using Core.Models;

namespace Core.DataStructures
{
    /// <summary>
    /// Additional wrapper for the "Queue" structure to arrange batches of boxes
    /// The batch queue also allows you to control the total quantity of boxes regardless of batches
    /// </summary>
    public class Boxes
    {
        public readonly int MaxQty = 100;

        public Queue<BoxBatch> Batches { get; } = new();
        public int TotalQty { get; set; } = default;
    }
}
