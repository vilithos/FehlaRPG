using System;


namespace FehlaRpg
{    
    class Program
    {
        static void Main(string[] args)
        {
            // === WICHTIG FÜR DIE PROJEKTABGABE: Was ist KI generiert? ===
            // Texte in DummyFrog.json sind auf meinen ersten Entwürfen basierend generiert worden.
            // Ein paar modulo und zeilen-berechnungen sind vereinzelt KI generiert.
            // Ansonsten habe ich KI nur als Lehrer benutzt um C# zu lernen und mir Konzepte zu erklären.
            // Der restliche Code ist von mir selbst geschrieben. - N.J.

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            CanvasRenderer.SetGameWindow(CanvasRenderer.canvasX, CanvasRenderer.canvasY + 1);
            
            Console.CursorVisible = false;  // Hide the cursor
            
            CanvasRenderer.ClearCanvasBucket(' '); // ░ ▒ ▓ '\0' ' '
            CanvasRenderer.DrawBox(0,0,100,30); // game 

            // MAIN GAME LOOP --> runs until terminal is closed!
            while (true)
            {
                CanvasRenderer.DrawTitleScreen();
                
                Game.RunGame();

                while (Console.KeyAvailable) Console.ReadKey(intercept: true);
                Thread.Sleep(200);
            }
            

            // To-Do List:
            //DONE! 1. preAttackText must appear BEFORE combat menu 
            //DONE! --> damage calculation must work 
            //DONE! 6. game over/win conditions
            //DONE! Game over screen
            //DONE! Title screen
            // Game win screen (the story continues) "Fehla ... gave comfort to the world."
            // story bit screen (requires encounter loading logic) --> not implemented
            // combat visual effects through CanvasRenderer()
            //DONE! encounter patience, driveHP bar, name, attack indicator
            //DONE! 3. Make dummy frog Ascii appear on screen
            //DONE! 4. add healthbar to dummy frog
            //DONE! 5. make combat hud appear with hp/mp
            //DONE! call Console.clear() at specific points in code to avoid visual bugs
            // metapower implentation und submenu
            // plots (inventory) und items implementation
            // plotdevices (items) logik und ausführung
            // run debugger
        }
    }
}