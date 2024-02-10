using Core.DataStructures;
using Core.Models;
using Core.Models.DTO;
using Core.Models.Enums;

namespace Core.Service
{
    /// <summary>
    /// Business logic of the warehouse
    /// </summary>
    public class WarehouseManager
    {
        private readonly double SIZE_SEARCH_LIMIT_PERCENTAGE = 1.3;
        private readonly InventoryManager _inventoryManager = new();
        public readonly EventManager<ReplenishStatus> ReplenishEvent = new();
        public readonly EventManager<OrderStatus> OrderEvent = new();
        public readonly EventManager<bool> ExpiredBoxesEvent = new();

        public void AddBoxes(double side, double height, int qty)
        {
            var key = (side, height);
            if (!_inventoryManager.DoesKeyExist(key))
            {
                _inventoryManager.AddKey(key);
                _inventoryManager.RegisterSize(key);
            }

            Boxes boxes = _inventoryManager.PeekBoxes(key);
            int qtyForReplenished = _inventoryManager.CalculateCapacity(boxes, qty);
            int qtyToReturn = qty - qtyForReplenished;
            ReplenishStatus status = ReplenishStatus.WarehouseIsFull;

            if (qtyForReplenished > 0)
            {
                status = qtyToReturn > 0 ? ReplenishStatus.PartialAcceptance : ReplenishStatus.FullAcceptance;
                var batch = BoxesManager.CreateBatch(side, height, qtyForReplenished);
                _inventoryManager.AddBatch(batch);
                _inventoryManager.AddTotalQty(batch);
                _inventoryManager.EnterBatchInExpirationRegistry(batch);
            }

            ReplenishEvent.InvokeBoxEvent(key, status, qtyForReplenished, qtyToReturn);
        }

        public bool TryRetrieveBoxes(double side, double height, out Boxes? boxes)
        {
            var key = (side, height);
            if (!_inventoryManager.DoesKeyExist(key))
            {
                boxes = null;
                return false;
            }

            boxes = _inventoryManager.PeekBoxes(key);
            return true;
        }

        public Boxes RetriveBoxes(double side, double height) => _inventoryManager.PeekBoxes((side, height));

        public bool CanOrderBeCovered(Boxes boxes, int qty) => boxes.TotalQty >= qty;

        public double CalculateSearchUpperBound(double size) => size * SIZE_SEARCH_LIMIT_PERCENTAGE;

        public int GetBoxes(Boxes boxes, (double, double) key, int qty)
        {
            OrderStatus status = OrderStatus.FullCover;
            int existingQtyToCover = qty;
            int remainingQtyToCover = default;
            if (boxes.TotalQty < qty)
            {
                existingQtyToCover = boxes.TotalQty;
                remainingQtyToCover = qty - existingQtyToCover;
                status = OrderStatus.PartiallyCover;
            }

            _inventoryManager.SubtractBoxes(boxes, existingQtyToCover);
            boxes.TotalQty -= existingQtyToCover;

            if (boxes.TotalQty == 0)
            {
                _inventoryManager.RemoveKey(key);
                _inventoryManager.UnregisterSize(key);
            }

            OrderEvent.InvokeBoxEvent(key, status, existingQtyToCover, remainingQtyToCover);
            return existingQtyToCover;
        }

        public (double side, double height) FindMostSuitableSize(double side, double height, double upperSideBound, double upperHeightBound)
        {
            double nextHeight = _inventoryManager.FindNextHeightForSide(side, height, upperHeightBound);
            if (nextHeight != default)
                return (side, nextHeight);

            double nextSide = _inventoryManager.FindNextSide(side, upperSideBound);
            if (nextSide != default)
            {
                nextHeight = _inventoryManager.FindNextHeightForSide(nextSide, height, upperHeightBound);
                if (nextHeight != default)
                    return (nextSide, nextHeight);
            }

            return default;
        }

        public void RemoveExpiredBoxes()
        {
            while (_inventoryManager.AnyBoxesInExpirationRegistry())
            {
                BoxBatch batch = _inventoryManager.PeekBatchInExpirationRegistry();

                if (batch.Expiration > DateTime.Now)
                    break;

                ExpiredBoxesEvent.InvokeExpiredBoxesEvent((batch.Side, batch.Height), true, batch.Qty, batch.Expiration);
                _inventoryManager.UnregisterBatchFromExpirationRegistry();
                _inventoryManager.RemoveBatchFromInventory((batch.Side, batch.Height));
            }

            ExpiredBoxesEvent.InvokeExpiredBoxesEvent((default(float), default(float)), false, default, default);
        }
    }
}
