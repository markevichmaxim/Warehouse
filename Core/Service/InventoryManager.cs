using Core.DataStructures;
using Core.Models;

namespace Core.Service
{
    /// <summary>
    /// Managing and accessing data structures
    /// </summary>
    public class InventoryManager
    {
        private readonly Warehouse _warehouse = new();

        public bool DoesKeyExist((double side, double height) key) => _warehouse.Inventory.ContainsKey(key);

        public void AddKey((double side, double height) key) => _warehouse.Inventory.Add(key, new());

        public void AddBatch(BoxBatch boxBatch) => _warehouse.Inventory[(boxBatch.Side, boxBatch.Height)].Batches.Enqueue(boxBatch);

        public void AddTotalQty(BoxBatch boxBatch) => _warehouse.Inventory[(boxBatch.Side, boxBatch.Height)].TotalQty += boxBatch.Qty;

        public Boxes PeekBoxes((double side, double height) key) => _warehouse.Inventory[key];

        public int CalculateCapacity(Boxes boxes, int qty)
        {
            int remainingCapacity = boxes.MaxQty - boxes.TotalQty;

            if (remainingCapacity > 0 && remainingCapacity > qty)
                return qty;

            return remainingCapacity;
        }

        public void SubtractBoxes(Boxes boxes, int qty)
        {
            while (qty > 0)
            {
                BoxBatch batch = boxes.Batches.Peek();
                if (batch.Qty > qty)
                {
                    batch.Qty -= qty;
                    break;
                }
                qty -= batch.Qty;
                batch.Qty = default;
                boxes.Batches.Dequeue();
            }
        }

        public void RemoveKey((double side, double height) key) => _warehouse.Inventory.Remove(key);

        public void EnterBatchInExpirationRegistry(BoxBatch batch) => _warehouse.ExpirationDateRegistry.Enqueue(batch);

        public bool AnyBoxesInExpirationRegistry() => _warehouse.ExpirationDateRegistry.Any();

        public BoxBatch PeekBatchInExpirationRegistry() => _warehouse.ExpirationDateRegistry.Peek();

        public BoxBatch UnregisterBatchFromExpirationRegistry() => _warehouse.ExpirationDateRegistry.Dequeue();

        public void RemoveBatchFromInventory((double side, double height) key)
        {
            Boxes boxes = _warehouse.Inventory[key];

            BoxBatch batch = boxes.Batches.Dequeue();

            boxes.TotalQty -= batch.Qty;

            if (boxes.TotalQty == default)
                _warehouse.Inventory.Remove(key);
        }

        public void RegisterSize((double side, double height) key)
        {
            _warehouse.SizeRegistry.Sides.Add(key.side);

            if (!_warehouse.SizeRegistry.Heights.TryGetValue(key.side, out var heights))
            {
                heights = new SortedSet<double>();
                _warehouse.SizeRegistry.Heights[key.side] = heights;
            }

            heights.Add(key.height);
        }

        public void UnregisterSize((double size, double height) key)
        {
            if (_warehouse.SizeRegistry.Heights.TryGetValue(key.size, out var heights))
            {
                heights.Remove(key.height);

                if (heights.Count == 0)
                {
                    _warehouse.SizeRegistry.Heights.Remove(key.size);
                    _warehouse.SizeRegistry.Sides.Remove(key.size);
                }
            }
        }

        public double FindNextSide(double side, double upperSideBound)
        {
            return _warehouse.SizeRegistry.Sides.GetViewBetween(side, upperSideBound).FirstOrDefault();
        }

        public double FindNextHeightForSide(double side, double height, double upperHeightBound)
        {
            if (!_warehouse.SizeRegistry.Heights.ContainsKey(side))
                return default;

            return _warehouse.SizeRegistry.Heights[side].GetViewBetween(height, upperHeightBound).FirstOrDefault();
        }
    }
}
