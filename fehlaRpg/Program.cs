using System;


namespace FehlaRpg
{    
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            CanvasRenderer.SetGameWindow(CanvasRenderer.canvasX, CanvasRenderer.canvasY);

            
            Console.CursorVisible = false;  // Hide the cursor
            
            CanvasRenderer.ClearCanvasBucket(' '); // ░ ▒ ▓ '\0' ' '
            CanvasRenderer.DrawBox(0,0,100,30); // game 
            
            //DONE! 1. preAttackText must appear BEFORE combat menu 
            //DONE! --> damage calculation must work 
            //DONE! 6. game over/win conditions
            //DONE! Game over screen
            //DONE! Title screen
            // Game win screen (the story continues) "Fehla ... gave comfort to the world."
            // story bit screen (requires encounter loading logic) --> not implemented
            // 2. make combat appear through CanvasRenderer()
            // encounter patience, driveHP bar, name, attack indicator
            //DONE! 3. Make dummy frog Ascii appear on screen
            //DONE! 4. add healthbar to dummy frog
            //DONE! 5. make combat hud appear with hp/mp
            //DONE! call Console.clear() at specific points in code to avoid visual bugs
            // run debugger


            // MAIN GAME LOOP --> runs until terminal is closed!
            while (true)
            {
                CanvasRenderer.DrawTitleScreen();
                
                Game.RunGame();

                while (Console.KeyAvailable) Console.ReadKey(intercept: true);
                Thread.Sleep(200);
            }
            

            /*
            int bubbleX = 10; int bubbleY = 5; 
            int bubbleWidth = 50; int bubbleHeight = 7;
            int bubbleLineMaxLength = bubbleWidth - 2;

            // draw a test box and text
            CanvasRenderer.DrawBox(bubbleX,bubbleY,bubbleWidth,bubbleHeight);
            CanvasRenderer.DrawString(bubbleX+2,bubbleY+1, "Frogs are not real!", bubbleLineMaxLength);
            CanvasRenderer.DrawString(bubbleX+2,bubbleY+2, "No really... they are not.", bubbleLineMaxLength);
            */

            // render to console
        }
    }
}