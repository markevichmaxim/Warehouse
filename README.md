<h4 align="center">Project Overview</h4>
<p align="center">Portfolio project showcasing proficiency in working with data structures and algorithms.</p>

## Description

**Warehouse Simulation Project**

This project is a simulation of a warehouse for storing square boxes. The goal is to organize the storage of these boxes in such a way that each action is optimally optimized through the selection of efficient data structures. The main tasks to be performed in the warehouse are:

1. Replenishing the warehouse with new boxes.
2. Fulfilling orders for boxes of the required size from the warehouse.
3. Checking the availability of boxes in the warehouse.
4. Clearing the warehouse from boxes with expired dates.

**Key Points**

- **Properties:** Each box should have side size (boxes are square), height size, expiration date.
- **Limit on quantity of boxes:** Limitation on the maximum allowed quantity of boxes of the same size in the warehouse.
- **Search algorithm:** Algorithm for finding a box to fulfill an order. If there are no boxes of the required size or an insufficient quantity of boxes of that size, the system should propose an option to search for another most suitable size available in the warehouse. This process continues until the order is fully fulfilled or until there are no boxes of a suitable size left or the search is stopped by the user.
- **Search limitation:** The search for other sizes should be limited, meaning there should be a limit on the sizes of the boxes. For example, if an order is placed for the initial size of 4x2 and this size is not available in the warehouse or there are not enough boxes of this size, the system begins searching for the next smallest size after the initial one, but it should not be larger than x% of the initial size (If x = 10%, then the search is limited to a size of 4.4x2.2).

**Technical Details**

The project consists of several parts within one solution:

- Folder "Core" - Contains models, custom extensions of data structures for project purposes, as well as the service part where all the business logic of the warehouse resides.
- Folder "Interface" - Contains a console application through which interaction with the user is carried out, as well as DTO classes for data transfer between the backend part and the interface.

## Launch Instructions

Instructions for running the project:
1. From the root directory of the project, open the **`"Warehouse.sln"`** file using Microsoft Visual Studio, Visual Studio Code, or other .NET development environments.
2. Set the **`"Interface"`** project as the **`startup project`** by accessing solution settings (right-click on "Solution Warehouse" in the "Solution Explorer" tab in Microsoft Visual Studio, choose the option in the dropdown menu "Configure Startup Projects...", set "Interface" in the "Single startup project" field, and save).
3. Press **`"F5"`** or the green start button (by default in Microsoft Visual Studio).