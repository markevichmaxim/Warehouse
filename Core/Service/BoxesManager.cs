using Core.Models;

namespace Core.Service
{
    public static class BoxesManager
    {
        public static BoxBatch CreateBatch(double size, double height, int qty) => new(size, height, qty);
    }
}
