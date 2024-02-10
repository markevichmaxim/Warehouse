using Core.Models.Enums;
using Core.Service;

namespace Interface.DTO
{
    public class DataNotifier
    {
        public static void HandleInventoryReplenishment(WarehouseManager warehouse)
        {
            warehouse.ReplenishEvent.OnBoxEvent += (key, status, qtyForReplenished, qtyToReturn) =>
            {
                string message = default!;
                switch (status)
                {
                    case ReplenishStatus.WarehouseIsFull:
                        message = $"\nThe warehouse is full, so all {qtyToReturn} boxes of size {key.side}x{key.height} have been returned";
                        break;
                    case ReplenishStatus.PartialAcceptance:
                        message = $"\nThe warehouse was only able to accept {qtyForReplenished} boxes of size {key.side}x{key.height}, the remaining {qtyToReturn} were returned";
                        break;
                    case ReplenishStatus.FullAcceptance:
                        message = $"\nAll {qtyForReplenished} boxes of size {key.side}x{key.height} were accepted into the warehouse";
                        break;
                }
                Console.WriteLine(message);
            };
        }

        public static void HandleOrderBoxes(WarehouseManager warehouse)
        {
            warehouse.OrderEvent.OnBoxEvent += (key, status, existingQtyToCover, remainingQtyToCover) =>
            {
                string message = default!;
                switch (status)
                {
                    case OrderStatus.FullCover:
                        message = $"\nThe order of {existingQtyToCover} boxes of size {key.side}x{key.height} was fulfilled";
                        break;
                    case OrderStatus.PartiallyCover:
                        message = $"\nPartial fulfillment of an order for {existingQtyToCover + remainingQtyToCover} boxes of size {key.side}x{key.height}, {existingQtyToCover} boxes have been fulfilled and {remainingQtyToCover} remain";
                        break;
                }
                Console.WriteLine(message);
            };
        }

        public static void HandleRemoveExpiredBoxes(WarehouseManager warehouse)
        {
            warehouse.ExpiredBoxesEvent.OnRemoveExpiredBoxesEvent += (key, status, qty, date) =>
            {
                if (status == true)
                    Console.WriteLine($"Batch of {qty} boxes of size {key.side}x{key.height} expired on {date.Day}/{date.Month} at {date.ToShortTimeString()}, so they were removed from the warehouse\n");
                else
                    Console.WriteLine("No more expired boxes found");
            };
        }
    }
}
