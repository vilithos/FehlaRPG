using System;


namespace FehlaRpg
{    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 100;
            Console.WindowHeight = 30;
            Console.BufferWidth = 100;
            Console.BufferHeight = 30;

            Console.CursorVisible = false;  // Hide the cursor
            
            CanvasRenderer.ClearCanvasBucket(' '); // ░ ▒ ▓ '\0' ' '
            CanvasRenderer.DrawBox(0,0,100,30); // game 

            CanvasRenderer.DrawTitleScreen();
            
            CanvasRenderer.DrawGameOverScreen();
            
            //DONE! 1. preAttackText must appear BEFORE combat menu 
            //DONE! --> damage calculation must work 
            // 6. game over/win conditions
            // 2. make combat appear through CanvasRenderer()
            // 3. Make dummy frog Ascii appear on screen
            //DONE! 4. add healthbar to dummy frog
            //DONE! 5. make combat hud appear with hp/mp
            // run debugger

            Game.RunGame();

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