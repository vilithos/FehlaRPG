using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace FehlaRpg
{
    
    public enum Button // container mit konstanten "buttons" die aufgeruft werden können
    {
        None, Menu, Confirm, Next, Back
    }

    public enum Severity
    {
        Blank, Minor, Moderate, Severe, Critical
    }

    public enum DamageType
    {
        Dream, Memory, Goal
    }

    static class Game // =======================================================
    {
        internal static bool runningGame = true;
        public static Encounter currentGameEncounter;

        public static void RunGame() // container that 
        {
            InitializeTestEncounter();

            while (runningGame)
            {
                // TestMatchingSystem();
                Player.CombatMenu();

                Console.WriteLine($"Encounter - DreamHP: {currentGameEncounter.dreamHPc}, MemoryHP: {currentGameEncounter.memoryHPc}, GoalHP: {currentGameEncounter.goalHPc}");
                Console.WriteLine($"Player - HP: {Player.hope}, MP: {Player.metapower}");

            }
        }

        static void TestMatchingSystem()
        {
            Console.WriteLine("=== TESTING MATCHING SYSTEM ===");

            // Encounter uses as "dream" attack
            Console.WriteLine("--- DREAM ATTACKS ---");
            Attack dreamAtk = new Attack(DamageType.Dream, Severity.Minor, 10, "Encounter uses Minor Dream Attack!");
            Encounter dreamEnc = new Encounter(0, "Dream Test", 1, 50, 50, 50, dreamAtk);

            Console.WriteLine("Test 1: Defy vs Dream - expecting player to deal 10 DMG");
            ExecuteTurn(dreamEnc,"defy ");
            Console.WriteLine($"DreamHP after {dreamEnc.dreamHPc} (should be 40)\n");

            Console.WriteLine("Test 2: Mimic vs Dream - expecting player to deal 2 DMG");
            ExecuteTurn(dreamEnc,"mimic");
            Console.WriteLine($"DreamHP after {dreamEnc.dreamHPc} (should be 38)\n");

            Console.WriteLine("Test 3: Grasp vs Dream - expecting player to deal 2 DMG");
            ExecuteTurn(dreamEnc,"grasp");
            Console.WriteLine($"DreamHP after {dreamEnc.dreamHPc} (should be 36)\n");


            // Encounter uses as "memory" attack
            Console.WriteLine("--- MEMORY ATTACKS ---");
            Attack memoryAtk = new Attack(DamageType.Memory, Severity.Minor, 10, "Encounter uses Minor Memory Attack!");
            Encounter memoryEnc = new Encounter(0, "Memory Test", 1, 50, 50, 50, memoryAtk);

            Console.WriteLine("Test 4: Mimic vs Memory - expecting player to deal 10 DMG");
            ExecuteTurn(memoryEnc,"mimic");
            Console.WriteLine($"MemoryHP after {memoryEnc.memoryHPc} (should be 40)\n");

            Console.WriteLine("Test 5: Defy vs Memory - expecting player to deal 2 DMG");
            ExecuteTurn(memoryEnc,"defy ");
            Console.WriteLine($"MemoryHP after {memoryEnc.memoryHPc} (should be 38)\n");

            Console.WriteLine("Test 6: Grasp vs Memory - expecting player to deal 2 DMG");
            ExecuteTurn(memoryEnc,"grasp");
            Console.WriteLine($"MemoryHP after {memoryEnc.memoryHPc} (should be 36)\n");


            // Encounter uses as "goal" attack
            Console.WriteLine("--- GOAL ATTACKS ---");
            Attack goalAtk = new Attack(DamageType.Goal, Severity.Minor, 10, "Encounter uses Minor Goal Attack!");
            Encounter goalEnc = new Encounter(0, "Goal Test", 1, 50, 50, 50, goalAtk);

            Console.WriteLine("Test 7: Grasp vs Goal - expecting player to deal 10 DMG");
            ExecuteTurn(goalEnc,"grasp");
            Console.WriteLine($"GoalHP after {goalEnc.goalHPc} (should be 40)\n");

            Console.WriteLine("Test 8: Defy vs Goal - expecting player to deal 2 DMG");
            ExecuteTurn(goalEnc,"defy ");
            Console.WriteLine($"GoalHP after {goalEnc.goalHPc} (should be 38)\n");

            Console.WriteLine("Test 9: Mimic vs Goal - expecting player to deal 2 DMG");
            ExecuteTurn(goalEnc,"mimic");
            Console.WriteLine($"GoalHP after {goalEnc.goalHPc} (should be 36)\n");
        }
        static void InitializeTestEncounter()
        {
            Attack testAttack = new Attack(DamageType.Dream, Severity.Minor, 10, "The frog croaked while staring into the sky...");
            Encounter testEncounter = new Encounter(0, "Dummy Frog", 1,50,50,50, testAttack);
            currentGameEncounter = testEncounter;
        }   

        static bool DoesActionBeatAttack(Attack encounterAttack, string playerAction)
        {
            DamageType encDmgType = encounterAttack.dmgType;

            switch ((playerAction.ToLower(), encDmgType))
            {
                case ("defy ", DamageType.Dream): return true;
                case ("mimic", DamageType.Memory): return true;
                case ("grasp", DamageType.Goal): return true;
                default: return false;
            }
        }

        public static void ExecuteTurn(Encounter encounter, string playerAction)
        {
            // display player action text
            Console.WriteLine(encounter.attackForThisTurn.preActionText);

            // check for damageType beats, 
            bool actionBeatsAttack = DoesActionBeatAttack(encounter.attackForThisTurn, playerAction);

            // calculate damage
            int damageDealt = 0;
            if (actionBeatsAttack)
            {
                // player reaction is matches
                damageDealt = encounter.attackForThisTurn.baseDamage;
            }
            else
            {
                // player reaction is NO match
                damageDealt = (int)(encounter.attackForThisTurn.baseDamage * 0.2m);
            }

            // update encounter values
            encounter.TakeDamage(damageDealt, encounter.attackForThisTurn.dmgType);

            // update player values HP and MP
            Player.metapower += 10;

            // postActionText of encounter
            Console.WriteLine($"You dealt {damageDealt} DMG.");

            // ? not sure but is this where the indicator for the next encounter text comes???
        }
    

    }

    static class Player
    {
        public static int hope = 100;
        public static int metapower = 0;
        static string action = "wait";
        static int currentMenuSelection = 0;
        static Button pressedButton;

        // make a string[] with the options available to the player they can select
        // make userInput key affiliated with +1 and -1, I have to ensure it can not be a negative number AND if higher than max elements its set to 0 to cycle through options
        // a codeblock will then change the index of the string[] and stores that exact element in "action"
        // a method then can evaluate which "selection" is currently selected to fire code if pressed "confirmed"
    
        public static void CombatMenu()
        {
            string[] combatOptions = ["defy ", "mimic", "grasp", "magic", "plots", "stall"]; // 6 options -> 0 to 5
            string cmSelect; // stores menu option as string

            while (true)
            {
                Console.WriteLine("Combat Menu: ");

                for (int i = 0; i < combatOptions.Length; i++)
                {
                    if (i == currentMenuSelection)
                    {
                        // Highlighted word has white background and black text
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write($" {combatOptions[i]} ");
                        Console.ResetColor(); // Resets colors of console
                        Console.Write(" ");
                    }

                    else
                    {
                        // normal console colors so white on black background
                        Console.Write($" {combatOptions[i]} " + " ");
                    }
                }
                Console.WriteLine(); // new line after menu
                

                cmSelect = MenuSelectionOnce(combatOptions, false); // current combat menu selection 
                
                if (pressedButton == Button.Confirm)
                {
                    switch (cmSelect.ToLower())
                    {
                        case "defy ":  
                        case "mimic":
                        case "grasp": 
                            Game.ExecuteTurn(Game.currentGameEncounter, cmSelect);      
                            break;

                        case "magic": 
                            // TODO: magic submenu for "metapower" moves
                            break;

                        case "plots": 
                            // TODO: plots submenu is basically a simple inventory system with items
                            break;

                        case "stall": 
                            // TODO: player waits a turn (maybe takes less damage)

                            // for testing purposes only:
                            Game.runningGame = false;
                            break;

                    }
                }
                break; // break the while loop else player is stuck after CONFIRM action
            
            }


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
        static string MenuSelectionOnce(string[] currentMenu, bool resetSelection)
        {
            // set menu selection to the first option
            if (resetSelection) { currentMenuSelection = 0;}
            
            pressedButton = ButtonInput(); // einmal nach buttonInput fragen und abwarten

            // when there is player input evaluate which one -> either LEFT or RIGHT key
            //      else if its RIGHT then increase currentSelection by 1
            //      else if its LEFT then decrease currentSelection by 1
            if (pressedButton == Button.Next)
            {
                currentMenuSelection += 1;
                if (currentMenuSelection > currentMenu.Length - 1) { currentMenuSelection = 0;}
            }
            else if (pressedButton == Button.Back)
            {
                currentMenuSelection -= 1;
                if (currentMenuSelection < 0) { currentMenuSelection = currentMenu.Length - 1;}
            }
            else if (pressedButton == Button.Confirm)
            {
                // IMPORTANT: because "buttonPressed" in the class field this methods returns the current selection and is done
                return currentMenu[currentMenuSelection];
            }

            // gibt die selektion als string aus anhand des spezifischen elements in string[]
            return currentMenu[currentMenuSelection];
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

    class Attack
    {
        public DamageType dmgType {get;} // dream, memory, goal
        public Severity dmgSeverity; // minor, moderate, severe, critical
        public int baseDamage; // base damage before its modified
        public string preActionText; 
        // string postActionText;

        public Attack(DamageType dmgType, Severity dmgSeverity, int baseDamage, string preActionText)
        {
            this.dmgType = dmgType;
            this.dmgSeverity = dmgSeverity;
            this.baseDamage = baseDamage;
            this.preActionText = preActionText;
        }
    }

    class Encounter
    {
        private int encID;
        private string encName;        

        public Attack attackForThisTurn; // the attack the encounter will perform this turn
        int attackPower; // base attack power of the encounter
        // double absorbResist; // (saved for later) modifies how much mp player absorbs with actions 
        // currentAttack or something that stores the "attack" with everything thats part of an attack (damage type, severity, text before player acts, text after player acts) 
        // pool of attacks the encounter can use, which are randomly selected during its turn
        
        // Note: Severity of the damage types (probably make an enum for "minor", "moderate", "severe" and "critical")


        int driveHPmax; // total sum of all 3 hp types
        // int dreamHPmax; // max dream hp
        // int memoryHPmax; // max memory hp
        // int goalHPmax; // max goal hp

        int driveHPc; // current sum of all 3 hp types
        public int dreamHPc; // current dream hp
        public int memoryHPc; // current memory hp
        public int goalHPc; // current goal hp
        
        // int patienceCurrent;
        // int patienceMax;
        // int patienceLostPerTurn;

        public Encounter(int encID, string encName, int attackPower, int dreamHPc, int memoryHPc, int goalHPc, Attack attack)
        {
            this.encID = encID;
            this.encName = encName;
            this.attackPower = attackPower;
            
            this.dreamHPc = dreamHPc;
            this.memoryHPc = memoryHPc;
            this.goalHPc = goalHPc;
            driveHPc = dreamHPc + memoryHPc + goalHPc;

            this.attackForThisTurn = attack;

        }

        public void TakeDamage(int amount, DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Dream:  dreamHPc -= amount; break;
                case DamageType.Memory: memoryHPc -= amount; break;
                case DamageType.Goal:   goalHPc -= amount; break;
            }
            driveHPc = dreamHPc + memoryHPc + goalHPc;
        }

        public void SetCurrentAttack(Attack newAttack)
        {
            attackForThisTurn = newAttack;
        }

    }
    // npc klasse

    // 
    
    // rundenbasierte logik in welcher bestimmte aktionen in einer reihenfolge ausgeführt werden
    // z.b. gegneraktion -> spieleraktion -> zwischenphase -> neue runde

    // spielkalkulationen wie schaden berechnen, schadensfälle abfragen
    
    // spielerstatus berechnen und updaten

    // inventarverwaltung

        
    
}