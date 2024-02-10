namespace Core.Models
{
    /// <summary>
    /// The square box batch model has side size, height, expiration date and quantity
    /// </summary>
    public class BoxBatch
    {
        public double Side { get; }
        public double Height { get; }
        public DateTime Expiration { get; }
        public int Qty { get; set; }

        public BoxBatch(double side, double height, int qty)
        {
            Side = side;
            Height = height;
            Qty = qty;
            Expiration = DateTime.Now.AddSeconds(60);
        }
    }
}
