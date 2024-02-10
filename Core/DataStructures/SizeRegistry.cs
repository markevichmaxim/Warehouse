namespace Core.DataStructures
{
    /// <summary>
    /// Register for recording existing box sizes in the warehouse
    /// </summary>
    public class SizeRegistry
    {
        public SortedSet<double> Sides { get; } = new();
        public Dictionary<double, SortedSet<double>> Heights { get; } = new();
    }
}
