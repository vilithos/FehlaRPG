using System;
using System.Runtime.CompilerServices;

namespace FehlaRpg
{
    
    public enum Button // container mit konstanten "buttons" die aufgeruft werden können
    {
        Menu, Confirm, Next, Back
    }

    static class Game
    {
        static void RunGame() // container that 
        {
            // What am I doing right now?
            // 
        }
    }

    static class Player
    {
        static int hope = 100;
        static int metapower = 100;
        static string action = "wait";

        // make a string[] with the options available to the player they can select
        // make userInput key affiliated with +1 and -1, I have to ensure it can not be a negative number AND if higher than max elements its set to 0 to cycle through options
        // a codeblock will then change the index of the string[] and stores that exact element in "action"
        // a method then can evaluate which "selection" is currently selected to fire code if pressed "confirmed"

        public static void CombatMenu()
        {
            string[] combatOptions = ["defy", "mimic", "grasp", "magic", "plots", "stall"]; // 6 options -> 0 to 5

            string cmSelect = MenuSelectionOnce(combatOptions); // current combat menu selection 
            // menu switchcase für jede selektion

            // defy
            

            // mimic

            // grasp


        }

        // Player Input Method
        public static Button ButtonInput()
        {
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:    return Button.Menu;
                    case ConsoleKey.DownArrow:  return Button.Confirm;
                    case ConsoleKey.LeftArrow:  return Button.Back;
                    case ConsoleKey.RightArrow: return Button.Next;
                    default:
                        // ingoriere jegliche anderen key inputs und warte
                        break;
                }
            }
        }

        // aktualisiert die selektion eines spezifischen string[] ein einziges mal, in einer schleife würde einmal pro schleife auf userInput warten
        static string MenuSelectionOnce(string[] currentMenu)
        {
            // set menu selection to the first option
            int currentSelection = 0;
            string menuSelection;
            
            Button buttonPressed = ButtonInput(); // einmal nach buttonInput fragen und abwarten

            // check if the currentSelection is below 0 or higher than currentMenu.length
            //      if its below 0, set it to currentMenu.length
            //      if its above currentMenu.length, set it to 0
            if (currentSelection < 0) { currentSelection = currentMenu.Length;}
            if (currentSelection > currentMenu.Length) { currentSelection = 0;}

            // when there is player input evaluate which one -> either LEFT or RIGHT key
            //      else if its RIGHT then increase currentSelection by 1
            //      else if its LEFT then decrease currentSelection by 1
            else if (buttonPressed == Button.Next)
            {
                currentSelection += 1;
            }
            else if (buttonPressed == Button.Back)
            {
                currentSelection -= 1;
            }

            // gibt die selektion als string aus anhand des spezifischen elements in string[]
            menuSelection = currentMenu[currentSelection];
            return menuSelection;
        }

        static void UpdateHP()
        {
            
        }

        static void UpdateMP()
        {
            
        }

        static void PerformAction()
        {
            
        }

    }

    class Encounter
    {
        
    }
    // npc klasse

    // 
    
    // rundenbasierte logik in welcher bestimmte aktionen in einer reihenfolge ausgeführt werden
    // z.b. gegneraktion -> spieleraktion -> zwischenphase -> neue runde

    // spielkalkulationen wie schaden berechnen, schadensfälle abfragen
    
    // spielerstatus berechnen und updaten

    // inventarverwaltung

        
    
}