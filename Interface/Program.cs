using Core.Service;
using Interface.DTO;

WarehouseManager _warehouse = new();
DataNotifier.HandleInventoryReplenishment(_warehouse);
DataNotifier.HandleOrderBoxes(_warehouse);
DataNotifier.HandleRemoveExpiredBoxes(_warehouse);

ConsoleKey currentKey = default;
while (currentKey != ConsoleKey.Escape)
{
    Console.Clear();
    Console.WriteLine("[Warehouse Main Menu]\n");
    Console.WriteLine("1. Replenish inventory with boxes");
    Console.WriteLine("2. Order boxes from warehouse");
    Console.WriteLine("3. Check availability of boxes");
    Console.WriteLine("4. Find and get rid of expired boxes");
    Console.WriteLine("\nSelect an option or press «Esc» to exit...\n");

    currentKey = Console.ReadKey().Key;

    if (currentKey != ConsoleKey.Escape)
    {
        if (IsValidOption(currentKey))
        {
            Console.Clear();
            HandleMenuOption(currentKey);
            Console.WriteLine("\nPress any button to continue...");
            Console.ReadKey();
        }
    }
}

void InventoryReplenishment()
{
    Console.WriteLine("[Inventory Replenishment]\n");
    double side = ConsoleDto.InputValidation("Enter side size: ", double.Parse, x => x > 0);
    double height = ConsoleDto.InputValidation("Enter height: ", double.Parse, x => x > 0);
    int qty = ConsoleDto.InputValidation("Enter quantity: ", int.Parse, x => x > 0);

    _warehouse.AddBoxes(side, height, qty);
}

void OrderBoxes()
{
    Console.WriteLine("[Order boxes from warehouse]\n");
    double currentSide = ConsoleDto.InputValidation("Enter side size: ", double.Parse, x => x > 0);
    double currentHeight = ConsoleDto.InputValidation("Enter height: ", double.Parse, x => x > 0);
    int remainingQty = ConsoleDto.InputValidation("Enter quantity: ", int.Parse, x => x > 0);
    double upperSideBound = _warehouse.CalculateSearchUpperBound(currentSide);
    double upperHeightBound = _warehouse.CalculateSearchUpperBound(currentHeight);
    bool firstIteration = true;

    while (remainingQty > 0)
    {
        if (firstIteration && _warehouse.TryRetrieveBoxes(currentSide, currentHeight, out var boxes))
        {
            if (!_warehouse.CanOrderBeCovered(boxes!, remainingQty))
            {
                Console.WriteLine($"\nOnly {boxes!.TotalQty} boxes of this size are in stock");
                if (!GetAgreementToContinue())
                    return;
            }
            remainingQty -= _warehouse.GetBoxes(boxes!, (currentSide, currentHeight), remainingQty);
            firstIteration = false;
        }

        if (firstIteration)
            Console.WriteLine($"\nThere are no boxes of this size {currentSide}x{currentHeight} in inventory, want to look for other sizes?");
        else
            Console.WriteLine($"\nUncovered boxes per order: {remainingQty}. Do you want to look for another size in stock?");
        if (!GetAgreementToContinue())
            return;

        var nextSize = _warehouse.FindMostSuitableSize(currentSide, currentHeight, upperSideBound, upperHeightBound);
        if (nextSize == default)
        {
            Console.WriteLine($"\nNo other sizes found, unfilled boxes in order: {remainingQty}");
            return;
        }

        (currentSide, currentHeight) = (nextSize.side, nextSize.height);
        boxes = _warehouse.RetriveBoxes(currentSide, currentHeight);
        remainingQty -= _warehouse.GetBoxes(boxes, (currentSide, currentHeight), remainingQty);

        if (remainingQty == 0)
            Console.WriteLine($"\nThe order was fully completed!");

        firstIteration = false;
    }
}

void CheckAvailability()
{
    Console.WriteLine("[Check availability of boxes]\n");
    double side = ConsoleDto.InputValidation("Enter side size: ", double.Parse, x => x > 0);
    double height = ConsoleDto.InputValidation("Enter height: ", double.Parse, x => x > 0);

    if (!_warehouse.TryRetrieveBoxes(side, height, out var boxes))
    {
        Console.WriteLine("Boxes of this size are not in inventory");
        return;
    }

    Console.WriteLine($"Found {boxes!.TotalQty} boxes of size {side}x{height}");
}

void CheckExpiredBoxes()
{
    Console.WriteLine("[Find and get rid of expired boxes]\n");
    _warehouse.RemoveExpiredBoxes();
}

bool GetAgreementToContinue()
{
    Console.WriteLine("To cancel an order press 1, to continue any button");
    if (Console.ReadKey().Key == ConsoleKey.D1)
        return false;

    return true;
}

bool IsValidOption(ConsoleKey key) => key is ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4;

void HandleMenuOption(ConsoleKey option)
{
    switch (option)
    {
        case ConsoleKey.D1:
            InventoryReplenishment();
            break;
        case ConsoleKey.D2:
            OrderBoxes();
            break;
        case ConsoleKey.D3:
            CheckAvailability();
            break;
        case ConsoleKey.D4:
            CheckExpiredBoxes();
            break;
    }
}