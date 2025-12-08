using System;
using System.Data;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using Microsoft.VisualBasic;

namespace FehlaRpg
{
    
    public enum Button // container mit konstanten "buttons" die aufgeruft werden können
    {None, Menu, Confirm, Next, Back}

    public enum Severity{Minor, Moderate, Severe, Critical, Blank}

    public enum DamageType{Dream, Memory, Goal, Meta}

    public enum MatchQuality{Perfect, Neutral, Terrible, None}

    static class Game // =======================================================
    {
        internal static bool runningGame = true;
        public static Encounter currentGameEncounter;
        public static string endingSpeechBubble;

        public static void RunGame() // container that 
        {
            var path = Path.Combine("..", "..", "..", "data", "encounters", "DummyFrog.json");
            currentGameEncounter = LoadEncounter(path);

            while (runningGame && currentGameEncounter != null)
            {
                // selects a random encounter attack for this turn
                currentGameEncounter.SelectRandomAttack();
                
                // encounter preAttackText appears, this is the indicator for the player to react to
                CanvasRenderer.DrawSpeechBubble(15, 20, 70, 5, currentGameEncounter.attackForThisTurn.preActionText);

                // player turn
                bool waitingForPlayerTurn = true;
                while (waitingForPlayerTurn)
                {
                    Player.CombatMenu();
                    // debugging output
                    // Console.WriteLine($"Player - HP: {Player.hope}, MP: {Player.metapower}");

                    waitingForPlayerTurn = false;
                }
                // debugging output
                // Console.WriteLine(  $"=== {currentGameEncounter.encName} ===[{currentGameEncounter.patienceCurrent}/{currentGameEncounter.patienceMax}]===" +
                //                    $"DreamHP: {currentGameEncounter.dreamHPc}/{currentGameEncounter} MemoryHP: {currentGameEncounter.memoryHPc}/ GoalHP: {currentGameEncounter.goalHPc}");
                

                // encounter postAttackText appears based on player action quality
                MatchQuality currentMatchQuality = GetMatchQuality(currentGameEncounter.attackForThisTurn, Player.playerAction);
                string postTextKey = currentMatchQuality.ToString(); // "Perfect", "Neutral", "Terrible", "None"
                CanvasRenderer.DrawSpeechBubble(15, 20, 70, 5, currentGameEncounter.attackForThisTurn.postActionText[postTextKey]);
                /* this switchvase was used to debug the postActionText output
                switch (postTextKey)
                {
                    case "Perfect": 
                        Console.WriteLine("==>" + currentGameEncounter.attackForThisTurn.postActionText[postTextKey]);
                        break;
                    case "Neutral":
                        Console.WriteLine("==>" + currentGameEncounter.attackForThisTurn.postActionText[postTextKey]);
                        break;
                    case "Terrible":
                        Console.WriteLine("==>" + currentGameEncounter.attackForThisTurn.postActionText[postTextKey]);
                        break;
                }
                */

                // check if any ENDING conditions are met and display specific ending
                if (currentGameEncounter.patienceCurrent <= 0) // if patience runs out
                {
                    string chosenEnding = currentGameEncounter.GetEnding();
                    switch (chosenEnding)
                    {
                        case "endDetermination":
                            endingSpeechBubble = currentGameEncounter.endDetermination;
                            break;
                        case "endSweetSpot":
                            endingSpeechBubble = currentGameEncounter.endSweetspot;
                            break;
                        case "endFollower":
                            endingSpeechBubble = currentGameEncounter.endFollower;
                            break;
                        case "endMadness":
                            endingSpeechBubble = currentGameEncounter.endMadness;
                            break;
                    }

                    CanvasRenderer.DrawSpeechBubble(15, 20, 70, 5, endingSpeechBubble);
                    // game over screen
                    // exit game loop
                    // return to main menu
                }

            }
            // Console.WriteLine("Game End."); // debug
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

        static MatchQuality GetMatchQuality(Attack encounterAttack, string playerAction)
        {
            DamageType encDmgType = encounterAttack.dmgType;

            switch ((playerAction.ToLower(), encDmgType))
            {
                // perfect matches 10:0, medium MP
                case ("defy ", DamageType.Dream):
                case ("mimic", DamageType.Memory):
                case ("grasp", DamageType.Goal):
                    return MatchQuality.Perfect;
                
                // neutral matches 5:5, large MP
                case ("defy ", DamageType.Goal):
                case ("mimic", DamageType.Dream):
                case ("grasp", DamageType.Memory):
                    return MatchQuality.Neutral;

                // terrible matches 0:10, little MP
                case ("defy ", DamageType.Memory):
                case ("mimic", DamageType.Goal):
                case ("grasp", DamageType.Dream):
                    return MatchQuality.Terrible;

                // no matching at tall for items, magic and waiting
                default:
                    return MatchQuality.None;
            }
        }

        public static void ExecuteTurn(Encounter encounter, string playerAction)
        {
            int encDmgTaken = 0;
            int hopeDmgTaken = 0;
            int mpGain = 0;
            
            // Note: this is where item texts are outputted before calculation
            // Console.WriteLine(currentGameEncounter.attackForThisTurn.preActionText);

            // check for damageType beats 
            MatchQuality currentMatchQuality = GetMatchQuality(encounter.attackForThisTurn, playerAction);


            // damage calculation for both encounter AND player
            switch (currentMatchQuality)
            {
                case MatchQuality.Perfect:
                    // for now this is in, but requires method that processes "perfect" actions
                    encDmgTaken = encounter.attackForThisTurn.baseDamage + encounter.attackPower;
                    hopeDmgTaken = 0;
                    mpGain = 10;
                    break;
                
                case MatchQuality.Neutral:
                    // requires method that processes "neutral" actions
                    encDmgTaken = (encounter.attackForThisTurn.baseDamage + encounter.attackPower) / 2;
                    hopeDmgTaken = (encounter.attackForThisTurn.baseDamage + encounter.attackPower) / 2;
                    mpGain = 20;
                    break;
                
                case MatchQuality.Terrible:
                    // for now this is in, but requires method that processes "Terrible" actions
                    encDmgTaken = 0;
                    hopeDmgTaken = encounter.attackForThisTurn.baseDamage + encounter.attackPower;
                    mpGain = 0;
                    break;
                
                case MatchQuality.None:
                    // method that processes "none" actions because there are many different ones
                    // such as "plots (items)", "stall", "magic (metapower)
                    // depending on that how much mp, hp, and how much hopeDmgTaken is reduced
                    // it could even be that a "none" action deals a specific DamageType
                    // ==> this means a method for this is mandatory!!
                    break;
            }

            // update encounter values
            encounter.TakeDamage(encDmgTaken, encounter.attackForThisTurn.dmgType);

            // update player values HP and MP
            Player.hope -= hopeDmgTaken;
            Player.metapower += mpGain;

            // Console.WriteLine($"You dealt {encDmgTaken} DMG and lost {hopeDmgTaken} HP.");
        }
    
        public static Encounter LoadEncounter(string filePath)
        {
            EncounterData encData = JsonSerializer.Deserialize<EncounterData>(File.ReadAllText(filePath));

            Encounter loadedEncounter = new Encounter(encData);
            return loadedEncounter;

            // return new Encounter(JsonSerializer.Deserialize<EncounterData>(File.ReadAllText(filePath)));
        }
    }

    static class Player // =================================================================
    {
        public static int hope = 100;
        public static int hopeMax = 100;
        public static int metapower = 0;
        public static int metapowerMax = 100;
        public static string playerAction;
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
            // Console.WriteLine(Game.currentGameEncounter.attackForThisTurn.preActionText); // show the attack of the encounter

            while (true)
            {
                CanvasRenderer.DrawCombatMenu(28, 25, combatOptions, currentMenuSelection);
                /*
                Console.WriteLine("---------- Combat Menu ----------");

                // prints menu options
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
                */

                cmSelect = MenuSelectionOnce(combatOptions, false); // current combat menu selection 
                playerAction = cmSelect.ToLower(); // set player "action" string to the selected combat menu option

                if (pressedButton == Button.Confirm)
                {
                    switch (cmSelect.ToLower())
                    {
                        case "defy ":  
                        case "mimic":
                        case "grasp": 
                            Game.ExecuteTurn(Game.currentGameEncounter, cmSelect);      
                            return;

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
                            return;

                    }
                }
                // break; // break the while loop else player is stuck after CONFIRM action
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

    }

    class Attack // ===================================================================
    {
        public string id;
        public DamageType dmgType {get;} // dream, memory, goal
        public Severity dmgSeverity {get;} // minor, moderate, severe, critical
        // public int attackPower; // base damage of the encounter
        public int baseDamage; // base damage of the attack before
        public string preActionText;
        public Dictionary<string, string> postActionText; 
        public bool isSpecialAttack;
        public List<string> attackConditions;
        public bool entersPoolAfterUse;

        // constructor for JSON file loading
        
        public Attack(AttackData attackData)
        {
            id = attackData.atkID; //"this." is not necessary because of the spiderman meme
            dmgType = Enum.Parse<DamageType>(attackData.dmgType);
            dmgSeverity = Enum.Parse<Severity>(attackData.dmgSeverity);
            baseDamage = attackData.baseDamage;
            preActionText = attackData.preActionText; // single string
            postActionText = attackData.postActionText; // dictionary
            isSpecialAttack = attackData.isSpecialAttack;
            attackConditions = attackData.attackConditions; // list
            entersPoolAfterUse = attackData.entersPoolAfterUse;
        }


        // Old constructor for backward compatibility
        public Attack(DamageType dmgType, Severity dmgSeverity, int baseDamage, string preActionText)
        {
            this.dmgType = dmgType;
            this.dmgSeverity = dmgSeverity;
            this.baseDamage = baseDamage;
            this.preActionText = preActionText;
            // Leave other fields with default values
            this.postActionText = new Dictionary<string, string>();
            this.isSpecialAttack = false;
            this.attackConditions = new List<string>();
            this.entersPoolAfterUse = false;
        }
    }

    class Encounter // ====================================================================
    {
        public string encID;
        public string encName;        
        public Attack attackForThisTurn; // the attack the encounter will perform this turn
        public int attackPower; // base attack power of the encounter
        public double absorbResist; // (saved for later) modifies how much mp player absorbs with actions         
        
        // pool of attacks the encounter can use, which are randomly selected during its turn
        // Note: Severity of the damage types (probably make an enum for "minor", "moderate", "severe" and "critical")
        public List<Attack> allAttacksPool = new List<Attack>();

        public int driveHPmax; // total sum of all 3 hp types
        int dreamHPmax; // max dream hp
        int memoryHPmax; // max memory hp
        int goalHPmax; // max goal hp

        public int driveHPc; // current sum of all 3 hp types
        public int dreamHPc; // current dream hp
        public int memoryHPc; // current memory hp
        public int goalHPc; // current goal hp
        
        // patience system
        public int patienceCurrent;
        public int patienceMax;
        public int patienceLostEveryXTurn;
        private int turnCount = 0;


        // different possible endings based on how the encounter was defeated
        public string endDetermination;
        public string endSweetspot;
        public string endFollower;
        public string endMadness;

        public Encounter(EncounterData encounterData)
        {
            encID = encounterData.encID;
            encName = encounterData.encName;
            attackPower = encounterData.attackPower;
        
            // set current hp
            dreamHPc = encounterData.dreamHP;
            memoryHPc = encounterData.memoryHP;
            goalHPc = encounterData.goalHP;
            driveHPc = dreamHPc + memoryHPc + goalHPc;

            // set max hp
            dreamHPmax = encounterData.dreamHP;
            memoryHPmax = encounterData.memoryHP;
            goalHPmax = encounterData.goalHP;
            driveHPmax = dreamHPmax + memoryHPmax + goalHPmax;

            // set endings
            endDetermination = encounterData.endDetermination;
            endSweetspot = encounterData.endSweetspot;
            endFollower = encounterData.endFollower;
            endMadness = encounterData.endMadness;

            // set patience values
            patienceCurrent = encounterData.patienceTotalValue;
            patienceMax = encounterData.patienceTotalValue;
            patienceLostEveryXTurn = encounterData.patienceLostEveryXTurn;

            foreach (AttackData attkData in encounterData.allAttacks)
            {
                // this.attackForThisTurn = allAttacksData;
                Attack attack = new Attack(attkData);
                allAttacksPool.Add(attack);
            }
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

        public void SetSpecificAttack(Attack newAttack)
        {
            attackForThisTurn = newAttack;
        }

        public void SelectRandomAttack() // selects a random attack from available attack pools based on current HP types
        {
            // build a list of available attack pools from the three attackType pools (which happen to be lists)
            List<Attack> availableAttacks = new List<Attack>();

            // check if HP is left and if there are any attacks left inside an attack pool
            availableAttacks = allAttacksPool.Where(attack => { // check list of all attacks and .Where returns the Attacks after the switch case
            switch(attack.dmgType) // filter and check what kind of attack it is and if its available
                {
                    case DamageType.Dream: return dreamHPc > 0;
                    case DamageType.Memory: return memoryHPc > 0;
                    case DamageType.Goal: return goalHPc > 0;
                }
                return false;
            }).ToList(); // make sure its converted into a list
           

            // if no attacks are available at all, return (encounter defeated)
            if (availableAttacks.Count == 0) return;

            // pick random available pool
            Random rng = new Random();
            int attackPoolIndex = rng.Next(availableAttacks.Count);
            // pick random attack from the chosen attack pool
            attackForThisTurn = availableAttacks[attackPoolIndex];

        }

        public void UpdateHealth() // updates current HP and makes sure max HP is not exceeded
        {
            // make sure that current HP does not exceed max HP
            if (dreamHPc > dreamHPmax) dreamHPc = dreamHPmax;
            if (memoryHPc > memoryHPmax) memoryHPc = memoryHPmax;
            if (goalHPc > goalHPmax) goalHPc = goalHPmax;

            // update driveHPc, make sure its maximum is not exceeded
            driveHPc = dreamHPc + memoryHPc + goalHPc;
            if (driveHPc > driveHPmax) driveHPc = driveHPmax;
        }

        public void LosePatience() // counts turns and reduces patience accordingly
        {
            turnCount += 1;

            if (turnCount >= patienceLostEveryXTurn)
            {
                patienceCurrent -= 1;
                turnCount = 0; // reset turn count after patience loss
            }
        }
        
        public string GetEnding() // returns the ending string based on driveHP percentage
        {
            // calculate remaining driveHP percentage
            float driveHPpercent = (float)driveHPc / driveHPmax;

            if (driveHPpercent > 0.6f) { return endDetermination; } // above 60% then determination ending
            else if (driveHPpercent > 0.3f && driveHPpercent <= 0.6f) { return endSweetspot; } // between 30% and 60% then sweetspot ending
            else if (driveHPpercent > 0.1f && driveHPpercent <= 0.3f) { return endFollower; } // between 10% and 30% then follower ending
            else { return endMadness; } // below 10% then madness ending          
        }
    }
    
    class EncounterData
    {
        public string encID {get; set;}
        public string encName {get; set;}
        public string _description {get; set;}
        public int attackPower {get; set;}
        public double absorbResist {get; set;}
        public int dreamHP {get; set;}
        public int memoryHP {get; set;}
        public int goalHP {get; set;}
        public int patienceTotalValue {get; set;}
        public int patienceLostEveryXTurn {get; set;}
        public List<AttackData> allAttacks {get; set;}

        public string endDetermination {get; set;}
        public string endSweetspot {get; set;}
        public string endFollower {get; set;}
        public string endMadness {get; set;}
    }
    class AttackData
    {
        public string atkID {get; set;}
        public string dmgType {get; set;}
        public string dmgSeverity {get; set;}
        public int baseDamage {get; set;}
        public string preActionText {get; set;}
        public Dictionary<string,string> postActionText {get; set;}
        public bool isSpecialAttack {get; set;}
        public List<string> attackConditions {get; set;}
        public bool entersPoolAfterUse {get; set;}
    }
    
}