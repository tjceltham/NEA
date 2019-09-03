
//Skeleton Program code for the AQA A Level Paper 1 Summer 2019 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace TextAdventuresCS
{
    class Program
    {
        const int Inventory = 1001;
        const int MinimumIDForItem = 2001;
        const int IDDifferenceForObjectInTwoLocations = 10000;
        static Random Rnd = new Random();

        class Place
        {
            public string Description;
            public int id, North, East, South, West, Up, Down;
        }

        class Character
        {
            public string Name, Description;
            public int ID, CurrentLocation;
        }

        class Item
        {
            public int ID, Location;
            public string Description, Status, Name, Commands, Results;
        }

        private static string GetInstruction()
        {
            string instruction;
            Console.Write("\n> ");
            instruction = Console.ReadLine().ToLower();
            return instruction;
        }

        private static string ExtractCommand(ref string instruction)
        {
            string command = "";
            if (!instruction.Contains(" "))
            {
                return instruction;
            }
            while (instruction.Length > 0 && instruction[0] != ' ')
            {
                command += instruction[0];
                instruction = instruction.Remove(0, 1);
            }
            while (instruction.Length > 0 && instruction[0] == ' ')
            {
                instruction = instruction.Remove(0, 1);
            }
            return command;
        }

        private static bool Go(Character you, string direction, Place currentPlace)
        {
            bool moved = true;
            switch (direction)
            {
                case "north":
                    if (currentPlace.North == 0)
                    {
                        moved = false;
                    }
                    else
                    {
                        you.CurrentLocation = currentPlace.North;
                    }
                    break;

                case "east":
                    if (currentPlace.East == 0)
                    {
                        moved = false;
                    }
                    else
                    {
                        you.CurrentLocation = currentPlace.East;
                    }
                    break;
                case "south":
                    if (currentPlace.South == 0)
                    {
                        moved = false;
                    }
                    else
                    {
                        you.CurrentLocation = currentPlace.South;
                    }
                    break;
                case "west":
                    if (currentPlace.West == 0)
                    {
                        moved = false;
                    }
                    else
                    {
                        you.CurrentLocation = currentPlace.West;
                    }
                    break;
                case "up":
                    if (currentPlace.Up == 0)
                    {
                        moved = false;
                    }
                    else
                    {
                        you.CurrentLocation = currentPlace.Up;
                    }
                    break;
                case "down":
                    if (currentPlace.Down == 0)
                    {
                        moved = false;
                    }
                    else
                    {
                        you.CurrentLocation = currentPlace.Down;
                    }
                    break;
                default:
                    moved = false;
                    break;
            }
            if (!moved)
            {
                Console.WriteLine("You are not able to go in that direction.");
            }
            return moved;
        }

        private static void DisplayDoorStatus(string status)
        {
            if (status == "open")
            {
                Console.WriteLine("The door is open.");
            }
            else
            {
                Console.WriteLine("The door is closed.");
            }
        }

        private static void DisplayContentsOfContainerItem(List<Item> items, int containerID)
        {
            Console.Write("It contains: ");
            bool containsItem = false;
            foreach (var thing in items)
            {
                if (thing.Location == containerID)
                {
                    if (containsItem)
                    {
                        Console.Write(", ");
                    }
                    containsItem = true;
                    Console.Write(thing.Name);
                }
            }
            if (containsItem)
            {
                Console.WriteLine(".");
            }
            else
            {
                Console.WriteLine("nothing.");
            }
        }

        private static void Examine(List<Item> items, List<Character> characters, string itemToExamine, int currentLocation)
        {
            int Count = 0;
            if (itemToExamine == "inventory")
            {
                DisplayInventory(items);
            }
            else
            {
                int IndexOfItem = GetIndexOfItem(itemToExamine, -1, items);
                if (IndexOfItem != -1)
                {
                    if (items[IndexOfItem].Name == itemToExamine && (items[IndexOfItem].Location == Inventory || items[IndexOfItem].Location == currentLocation))
                    {
                        Console.WriteLine(items[IndexOfItem].Description);
                        if (items[IndexOfItem].Name.Contains("door"))
                        {
                            DisplayDoorStatus(items[IndexOfItem].Status);
                        }
                        if (items[IndexOfItem].Status.Contains("container"))
                        {
                            DisplayContentsOfContainerItem(items, items[IndexOfItem].ID);
                        }
                        return;
                    }
                }
                while (Count < characters.Count)
                {
                    if (characters[Count].Name == itemToExamine && characters[Count].CurrentLocation == currentLocation)
                    {
                        Console.WriteLine(characters[Count].Description);
                        return;
                    }
                    Count++;
                }
                Console.WriteLine("You cannot find " + itemToExamine + " to look at.");
            }
        }

        private static int GetPositionOfCommand(string commandList, string command)
        {
            int position = 0, count = 0;
            while (count <= commandList.Length - command.Length)
            {
                if (commandList.Substring(count, command.Length) == command)
                {
                    return position;
                }
                else if (commandList[count] == ',')
                {
                    position++;
                }
                count++;
            }
            return position;
        }

        private static string GetResultForCommand(string results, int position)
        {
            int count = 0, currentPosition = 0;
            string ResultForCommand = "";
            while (currentPosition < position && count < results.Length)
            {
                if (results[count] == ';')
                {
                    currentPosition++;
                }
                count++;
            }
            while (count < results.Length)
            {
                if (results[count] == ';')
                {
                    break;
                }
                ResultForCommand += results[count];
                count++;
            }
            return ResultForCommand;
        }

        private static void Say(string speech)
        {
            Console.WriteLine();
            Console.WriteLine(speech);
            Console.WriteLine();
        }

        private static void ExtractResultForCommand(ref string subCommand, ref string subCommandParameter, string resultForCommand)
        {
            int Count = 0;
            while (Count < resultForCommand.Length && resultForCommand[Count] != ',')
            {
                subCommand += resultForCommand[Count];
                Count++;
            }
            Count++;
            while (Count < resultForCommand.Length)
            {
                if (resultForCommand[Count] != ',' && resultForCommand[Count] != ';')
                {
                    subCommandParameter += resultForCommand[Count];
                }
                else
                {
                    return;
                }
                Count++;
            }
        }

        private static void ChangeLocationReference(string direction, int newLocationReference, List<Place> places, int indexOfCurrentLocation, bool opposite)
        {
            Place thisPlace = places[indexOfCurrentLocation];
            if (direction == "north" && !opposite || direction == "south" && opposite)
            {
                thisPlace.North = newLocationReference;
            }
            else if (direction == "east" && !opposite || direction == "west" && opposite)
            {
                thisPlace.East = newLocationReference;
            }
            else if (direction == "south" && !opposite || direction == "north" && opposite)
            {
                thisPlace.South = newLocationReference;
            }
            else if (direction == "west" && !opposite || direction == "east" && opposite)
            {
                thisPlace.West = newLocationReference;
            }
            else if (direction == "up" && !opposite || direction == "down" && opposite)
            {
                thisPlace.Up = newLocationReference;
            }
            else if (direction == "down" && !opposite || direction == "up" && opposite)
            {
                thisPlace.Down = newLocationReference;
            }
            places[indexOfCurrentLocation] = thisPlace;
        }

        private static int OpenClose(bool open, List<Item> items, List<Place> places, string itemToOpenClose, int currentLocation)
        {
            string command, resultForCommand, direction = "", directionChange = "";
            int count = 0, position, count2;
            bool actionWorked = false;
            if (open)
            {
                command = "open";
            }
            else
            {
                command = "close";
            }
            while (count < items.Count && !actionWorked)
            {
                if (items[count].Name == itemToOpenClose)
                {
                    if (items[count].Location == currentLocation)
                    {
                        if (items[count].Commands.Length >= 4)
                        {
                            if (items[count].Commands.Contains(command))
                            {
                                if (items[count].Status == command)
                                {
                                    return -2;
                                }
                                else if (items[count].Status == "locked")
                                {
                                    return -3;
                                }
                                position = GetPositionOfCommand(items[count].Commands, command);
                                resultForCommand = GetResultForCommand(items[count].Results, position);
                                ExtractResultForCommand(ref direction, ref directionChange, resultForCommand);
                                ChangeStatusOfItem(items, count, command);
                                count2 = 0;
                                actionWorked = true;
                                while (count2 < places.Count)
                                {
                                    if (places[count2].id == Convert.ToInt32(currentLocation))
                                    {
                                        ChangeLocationReference(direction, Convert.ToInt32(directionChange), places, count2, false);
                                    }
                                    else if (places[count2].id == Convert.ToInt32(directionChange))
                                    {
                                        ChangeLocationReference(direction, currentLocation, places, count2, true);
                                    }
                                    count2++;
                                }
                                int IndexOfOtherSideOfDoor;
                                if (items[count].ID > IDDifferenceForObjectInTwoLocations)
                                {
                                    IndexOfOtherSideOfDoor = GetIndexOfItem("", items[count].ID - IDDifferenceForObjectInTwoLocations, items);
                                }
                                else
                                {
                                    IndexOfOtherSideOfDoor = GetIndexOfItem("", items[count].ID + IDDifferenceForObjectInTwoLocations, items);
                                }
                                ChangeStatusOfItem(items, IndexOfOtherSideOfDoor, command);
                                count = items.Count + 1;
                            }
                        }
                    }
                }
                count++;
            }
            if (!actionWorked)
            {
                return -1;
            }
            return Convert.ToInt32(directionChange);
        }

        private static int GetIndexOfItem(string itemNameToGet, int itemIDToGet, List<Item> items)
        {
            int count = 0;
            bool stopLoop = false;
            while (!stopLoop && count < items.Count)
            {
                if ((itemIDToGet == -1 && items[count].Name == itemNameToGet) || items[count].ID == itemIDToGet)
                {
                    stopLoop = true;
                }
                else
                {
                    count++;
                }
            }
            if (!stopLoop)
            {
                return -1;
            }
            else
            {
                return count;
            }
        }

        private static void ChangeLocationOfItem(List<Item> items, int indexOfItem, int newLocation)
        {
            Item thisItem = items[indexOfItem];
            thisItem.Location = newLocation;
            items[indexOfItem] = thisItem;
        }

        private static void ChangeStatusOfItem(List<Item> items, int indexOfItem, string newStatus)
        {
            Item thisItem = items[indexOfItem];
            thisItem.Status = newStatus;
            items[indexOfItem] = thisItem;
        }

        private static int GetRandomNumber(int lowerLimitValue, int upperLimitValue)
        {
            return Rnd.Next(lowerLimitValue, upperLimitValue + 1);
        }

        private static int RollDie(string lower, string upper)
        {
            int lowerLimitValue = 0;
            if (!int.TryParse(lower, out lowerLimitValue))
            {
                while (lowerLimitValue < 1 || lowerLimitValue > 6)
                {
                    Console.Write("Enter minimum: ");
                    int.TryParse(Console.ReadLine(), out lowerLimitValue);
                }
            }
            int upperLimitValue = 0;
            if (!int.TryParse(upper, out upperLimitValue))
            {
                while (upperLimitValue < lowerLimitValue || upperLimitValue > 6)
                {
                    Console.Write("Enter maximum: ");
                    int.TryParse(Console.ReadLine(), out upperLimitValue);
                }
            }
            return GetRandomNumber(lowerLimitValue, upperLimitValue);
        }

        private static void ChangeStatusOfDoor(List<Item> items, int currentLocation, int indexOfItemToLockUnlock, int indexOfOtherSideItemToLockUnlock)
        {
            if (currentLocation == items[indexOfItemToLockUnlock].Location || currentLocation == items[indexOfOtherSideItemToLockUnlock].Location)
            {
                if (items[indexOfItemToLockUnlock].Status == "locked")
                {
                    ChangeStatusOfItem(items, indexOfItemToLockUnlock, "close");
                    ChangeStatusOfItem(items, indexOfOtherSideItemToLockUnlock, "close");
                    Say(items[indexOfItemToLockUnlock].Name + " now unlocked.");
                }
                else if (items[indexOfItemToLockUnlock].Status == "close")
                {
                    ChangeStatusOfItem(items, indexOfItemToLockUnlock, "locked");
                    ChangeStatusOfItem(items, indexOfOtherSideItemToLockUnlock, "locked");
                    Say(items[indexOfItemToLockUnlock].Name + " now locked.");
                }
                else
                {
                    Say(items[indexOfItemToLockUnlock].Name + " is open so can't be locked.");
                }
            }
            else
            {
                Say("Can't use that key in this location.");
            }
        }

        private static void UseItem(List<Item> items, string itemToUse, int currentLocation, ref bool stopGame, List<Place> places)
        {
            int position, indexOfItem;
            string resultForCommand, subCommand = "", subCommandParameter = "";
            indexOfItem = GetIndexOfItem(itemToUse, -1, items);
            if (indexOfItem != -1)
            {
                if (items[indexOfItem].Location == Inventory || (items[indexOfItem].Location == currentLocation && items[indexOfItem].Status.Contains("usable")))
                {
                    position = GetPositionOfCommand(items[indexOfItem].Commands, "use");
                    resultForCommand = GetResultForCommand(items[indexOfItem].Results, position);
                    ExtractResultForCommand(ref subCommand, ref subCommandParameter, resultForCommand);
                    if (subCommand == "say")
                    {
                        Say(subCommandParameter);
                    }
                    else if (subCommand == "lockunlock")
                    {
                        int IndexOfItemToLockUnlock, IndexOfOtherSideItemToLockUnlock;
                        IndexOfItemToLockUnlock = GetIndexOfItem("", Convert.ToInt32(subCommandParameter), items);
                        IndexOfOtherSideItemToLockUnlock = GetIndexOfItem("", Convert.ToInt32(subCommandParameter) + IDDifferenceForObjectInTwoLocations, items);
                        ChangeStatusOfDoor(items, currentLocation, IndexOfItemToLockUnlock, IndexOfOtherSideItemToLockUnlock);
                    }
                    else if (subCommand == "roll")
                    {
                        Say("You have rolled a " + RollDie(resultForCommand[5].ToString(), resultForCommand[7].ToString()));
                    }
                    return;
                }
            }
            Console.WriteLine("You can't use that!");
        }

        private static void ReadItem(List<Item> items, string itemToRead, int currentLocation)
        {
            string SubCommand = "", SubCommandParameter = "", ResultForCommand;
            int IndexOfItem, Position;
            IndexOfItem = GetIndexOfItem(itemToRead, -1, items);
            if (IndexOfItem == -1)
            {
                Console.WriteLine("You can't find " + itemToRead + ".");
            }
            else if (!items[IndexOfItem].Commands.Contains("read"))
            {
                Console.WriteLine("You can't read " + itemToRead + ".");
            }
            else if (items[IndexOfItem].Location != currentLocation && items[IndexOfItem].Location != Inventory)
            {
                Console.WriteLine("You can't find " + itemToRead + ".");
            }
            else
            {
                Position = GetPositionOfCommand(items[IndexOfItem].Commands, "read");
                ResultForCommand = GetResultForCommand(items[IndexOfItem].Results, Position);
                ExtractResultForCommand(ref SubCommand, ref SubCommandParameter, ResultForCommand);
                if (SubCommand == "say")
                {
                    Say(SubCommandParameter);
                }
            }
        }

        private static void GetItem(List<Item> items, string itemToGet, int currentLocation, ref bool stopGame)
        {
            string resultForCommand, subCommand = "", subCommandParameter = "";
            int indexOfItem, position;
            bool canGet = false;
            indexOfItem = GetIndexOfItem(itemToGet, -1, items);
            if (indexOfItem == -1)
            {
                Console.WriteLine("You can't find " + itemToGet + ".");
            }
            else if (items[indexOfItem].Location == Inventory)
            {
                Console.WriteLine("You have already got that!");
            }
            else if (!items[indexOfItem].Commands.Contains("get"))
            {
                Console.WriteLine("You can't get " + itemToGet + ".");
            }
            else if (items[indexOfItem].Location >= MinimumIDForItem && items[GetIndexOfItem("", items[indexOfItem].Location, items)].Location != currentLocation)
            {
                Console.WriteLine("You can't find " + itemToGet + ".");
            }
            else if (items[indexOfItem].Location < MinimumIDForItem && items[indexOfItem].Location != currentLocation)
            {
                Console.WriteLine("You can't find " + itemToGet + ".");
            }
            else
            {
                canGet = true;
            }
            if (canGet)
            {
                position = GetPositionOfCommand(items[indexOfItem].Commands, "get");
                resultForCommand = GetResultForCommand(items[indexOfItem].Results, position);
                ExtractResultForCommand(ref subCommand, ref subCommandParameter, resultForCommand);
                if (subCommand == "say")
                {
                    Say(subCommandParameter);
                }
                else if (subCommand == "win")
                {
                    Say("You have won the game");
                    stopGame = true;
                    return;
                }
                if (items[indexOfItem].Status.Contains("gettable"))
                {
                    ChangeLocationOfItem(items, indexOfItem, Inventory);

                    Console.WriteLine("You have got that now.");
                }
            }
        }

        private static bool CheckIfDiceGamePossible(List<Item> items, List<Character> characters, ref int indexOfPlayerDie, ref int indexOfOtherCharacter, ref int indexOfOtherCharacterDie, string otherCharacterName)
        {
            bool playerHasDie = false, playersInSameRoom = false, otherCharacterHasDie = false;
            foreach (var thing in items)
            {
                if (thing.Location == Inventory && thing.Name.Contains("die"))
                {
                    playerHasDie = true;
                    indexOfPlayerDie = GetIndexOfItem("", thing.ID, items);
                }
            }
            int Count = 1;
            while (Count < characters.Count && !playersInSameRoom)
            {
                if (characters[0].CurrentLocation == characters[Count].CurrentLocation && characters[Count].Name == otherCharacterName)
                {
                    playersInSameRoom = true;
                    foreach (var thing in items)
                    {
                        if (thing.Location == characters[Count].ID && thing.Name.Contains("die"))
                        {
                            otherCharacterHasDie = true;
                            indexOfOtherCharacterDie = GetIndexOfItem("", thing.ID, items);
                            indexOfOtherCharacter = Count;
                        }
                    }
                }
                Count++;
            }
            return playerHasDie && playersInSameRoom && otherCharacterHasDie;
        }

        private static void TakeItemFromOtherCharacter(List<Item> items, int idOfOtherCharacter)
        {
            List<int> ListofIndicesOfItemsInInventory = new List<int>();
            List<string> ListOfNamesOfItemsInInventory = new List<string>();
            int Count = 0;
            while (Count < items.Count)
            {
                if (items[Count].Location == idOfOtherCharacter)
                {
                    ListofIndicesOfItemsInInventory.Add(Count);
                    ListOfNamesOfItemsInInventory.Add(items[Count].Name);
                }
                Count++;
            }
            Count = 1;
            Console.Write("Which item do you want to take?  They have: ");
            Console.Write(ListOfNamesOfItemsInInventory[0]);
            while (Count < ListOfNamesOfItemsInInventory.Count - 1)
            {
                Console.Write(", " + ListOfNamesOfItemsInInventory[Count]);
                Count++;
            }
            Console.WriteLine(".");
            string ChosenItem = Console.ReadLine();
            if (ListOfNamesOfItemsInInventory.Contains(ChosenItem))
            {
                Console.WriteLine("You have that now.");
                int pos = ListOfNamesOfItemsInInventory.IndexOf(ChosenItem);
                ChangeLocationOfItem(items, Convert.ToInt32(ListofIndicesOfItemsInInventory[pos]), Inventory);
            }
            else
            {
                Console.WriteLine("They don't have that item, so you don't take anything this time.");
            }
        }

        private static void TakeRandomItemFromPlayer(List<Item> items, int otherCharacterID)
        {
            List<int> listOfIndicesOfItemsInInventory = new List<int>();
            int count = 0;
            while (count < items.Count)
            {
                if (items[count].Location == Inventory)
                {
                    listOfIndicesOfItemsInInventory.Add(count);
                }
                count++;
            }
            int rNo = GetRandomNumber(0, listOfIndicesOfItemsInInventory.Count - 1);
            Console.WriteLine("They have taken your " + items[Convert.ToInt32(listOfIndicesOfItemsInInventory[rNo])].Name + ".");
            ChangeLocationOfItem(items, Convert.ToInt32(listOfIndicesOfItemsInInventory[rNo]), otherCharacterID);
        }

        private static void PlayDiceGame(List<Character> characters, List<Item> Items, string otherCharacterName)
        {
            int playerScore = 0, otherCharacterScore = 0, indexOfPlayerDie = 0, indexOfOtherCharacterDie = 0, position = 0, indexOfOtherCharacter = 0;
            string ResultForCommand;
            bool diceGamePossible = CheckIfDiceGamePossible(Items, characters, ref indexOfPlayerDie, ref indexOfOtherCharacter, ref indexOfOtherCharacterDie, otherCharacterName);
            if (!diceGamePossible)
            {
                Console.WriteLine("You can't play a dice game.");
            }
            else
            {
                position = GetPositionOfCommand(Items[indexOfPlayerDie].Commands, "use");
                ResultForCommand = GetResultForCommand(Items[indexOfPlayerDie].Results, position);
                playerScore = RollDie(ResultForCommand[5].ToString(), ResultForCommand[7].ToString());
                Console.WriteLine("You rolled a " + playerScore + ".");
                position = GetPositionOfCommand(Items[indexOfOtherCharacterDie].Commands, "use");
                ResultForCommand = GetResultForCommand(Items[indexOfOtherCharacterDie].Results, position);
                otherCharacterScore = RollDie(ResultForCommand[5].ToString(), ResultForCommand[7].ToString());
                Console.WriteLine("They rolled a " + otherCharacterScore + ".");
                if (playerScore > otherCharacterScore)
                {
                    Console.WriteLine("You win!");
                    TakeItemFromOtherCharacter(Items, characters[indexOfOtherCharacter].ID);
                }
                else if (playerScore < otherCharacterScore)
                {
                    Console.WriteLine("You lose!");
                    TakeRandomItemFromPlayer(Items, characters[indexOfOtherCharacter].ID);
                }
                else
                {
                    Console.WriteLine("Draw!");
                }
            }
        }

        private static void MoveItem(List<Item> items, string itemToMove, int currentLocation)
        {
            int position;
            string resultForCommand, subCommand = "", subCommandParameter = "";
            int indexOfItem = GetIndexOfItem(itemToMove, -1, items);
            if (indexOfItem != -1)
            {
                if (items[indexOfItem].Location == currentLocation)
                {
                    if (items[indexOfItem].Commands.Length >= 4)
                    {
                        if (items[indexOfItem].Commands.Contains("move"))
                        {
                            position = GetPositionOfCommand(items[indexOfItem].Commands, "move");
                            resultForCommand = GetResultForCommand(items[indexOfItem].Results, position);
                            ExtractResultForCommand(ref subCommand, ref subCommandParameter, resultForCommand);
                            if (subCommand == "say")
                            {
                                Say(subCommandParameter);
                            }
                        }
                        else
                        {
                            Console.WriteLine("You can't move " + itemToMove + ".");
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can't move " + itemToMove + ".");
                    }
                    return;
                }
            }
            Console.WriteLine("You can't find " + itemToMove + ".");
        }

        private static void DisplayInventory(List<Item> items)
        {
            Console.WriteLine();
            Console.WriteLine("You are currently carrying the following items:");
            foreach (var thing in items)
            {
                if (thing.Location == Inventory)
                {
                    Console.WriteLine(thing.Name);
                }
            }
            Console.WriteLine();
        }

        private static void DisplayGettableItemsInLocation(List<Item> items, int currentLocation)
        {
            bool containsGettableItems = false;
            string listOfItems = "On the floor there is: ";
            foreach (var thing in items)
            {
                if (thing.Location == currentLocation && thing.Status.Contains("gettable"))
                {
                    if (containsGettableItems)
                    {
                        listOfItems += ", ";
                    }
                    listOfItems += thing.Name;
                    containsGettableItems = true;
                }
            }
            if (containsGettableItems)
            {
                Console.WriteLine(listOfItems + ".");
            }
        }

        private static void DisplayOpenCloseMessage(int resultOfOpenClose, bool openCommand)
        {
            if (resultOfOpenClose >= 0)
            {
                if (openCommand)
                {
                    Say("You have opened it.");
                }
                else
                {
                    Say("You have closed it.");
                }
            }
            else if (resultOfOpenClose == -3)
            {
                Say("You can't do that, it is locked.");
            }
            else if (resultOfOpenClose == -2)
            {
                Say("It already is.");
            }
            else if (resultOfOpenClose == -1)
            {
                Say("You can't open that.");
            }
        }

        private static void PlayGame(List<Character> characters, List<Item> items, List<Place> places)
        {
            bool stopGame = false;
            string instruction, Command;
            bool moved = true;
            int resultOfOpenClose;
            while (!stopGame)
            {
                if (moved)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(places[characters[0].CurrentLocation - 1].Description);
                    DisplayGettableItemsInLocation(items, characters[0].CurrentLocation);
                    moved = false;
                }
                instruction = GetInstruction();
                Command = ExtractCommand(ref instruction);
                switch (Command)
                {
                    case "get":
                        GetItem(items, instruction, characters[0].CurrentLocation, ref stopGame);
                        break;
                    case "use":
                        UseItem(items, instruction, characters[0].CurrentLocation, ref stopGame, places);
                        break;
                    case "go":
                        moved = Go(characters[0], instruction, places[characters[0].CurrentLocation - 1]);
                        break;
                    case "read":
                        ReadItem(items, instruction, characters[0].CurrentLocation);
                        break;
                    case "examine":
                        Examine(items, characters, instruction, characters[0].CurrentLocation);
                        break;
                    case "open":
                        resultOfOpenClose = OpenClose(true, items, places, instruction, characters[0].CurrentLocation);
                        DisplayOpenCloseMessage(resultOfOpenClose, true);
                        break;
                    case "close":
                        resultOfOpenClose = OpenClose(false, items, places, instruction, characters[0].CurrentLocation);
                        DisplayOpenCloseMessage(resultOfOpenClose, false);
                        break;
                    case "move":
                        MoveItem(items, instruction, characters[0].CurrentLocation);
                        break;
                    case "say":
                        Say(instruction);
                        break;
                    case "playdice":
                        PlayDiceGame(characters, items, instruction);
                        break;
                    case "quit":
                        Say("You decide to give up, try again another time.");
                        stopGame = true;
                        break;
                    default:
                        Console.WriteLine("Sorry, you don't know how to " + Command + ".");
                        break;
                }
            }
            Console.ReadLine();
        }

        private static bool LoadGame(string filename, List<Character> characters, List<Item> items, List<Place> places)
        {
            int noOfCharacters, noOfPlaces, NoOfItems;
            try
            {
                using (BinaryReader Reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    noOfCharacters = Reader.ReadInt32();
                    for (int Count = 0; Count < noOfCharacters; Count++)
                    {
                        Character tempCharacter = new Character();
                        tempCharacter.ID = Reader.ReadInt32();
                        tempCharacter.Name = Reader.ReadString();
                        tempCharacter.Description = Reader.ReadString();
                        tempCharacter.CurrentLocation = Reader.ReadInt32();
                        characters.Add(tempCharacter);
                    }
                    noOfPlaces = Reader.ReadInt32();
                    for (int Count = 0; Count < noOfPlaces; Count++)
                    {
                        Place tempPlace = new Place();
                        tempPlace.id = Reader.ReadInt32();
                        tempPlace.Description = Reader.ReadString();
                        tempPlace.North = Reader.ReadInt32();
                        tempPlace.East = Reader.ReadInt32();
                        tempPlace.South = Reader.ReadInt32();
                        tempPlace.West = Reader.ReadInt32();
                        tempPlace.Up = Reader.ReadInt32();
                        tempPlace.Down = Reader.ReadInt32();
                        places.Add(tempPlace);
                    }
                    NoOfItems = Reader.ReadInt32();
                    for (int Count = 0; Count < NoOfItems; Count++)
                    {
                        Item tempItem = new Item();
                        tempItem.ID = Reader.ReadInt32();
                        tempItem.Description = Reader.ReadString();
                        tempItem.Status = Reader.ReadString();
                        tempItem.Location = Reader.ReadInt32();
                        tempItem.Name = Reader.ReadString();
                        tempItem.Commands = Reader.ReadString();
                        tempItem.Results = Reader.ReadString();
                        items.Add(tempItem);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static void Main(string[] args)
        {
            string filename;
            List<Place> places = new List<Place>();
            List<Item> items = new List<Item>();
            List<Character> characters = new List<Character>();
            Console.Write("Enter filename> ");
            filename = Console.ReadLine() + ".gme";
            if (LoadGame(filename, characters, items, places))
            {
                PlayGame(characters, items, places);
            }
            else
            {
                Console.WriteLine("Unable to load game.");
                Console.ReadLine();
            }
        }
    }
}