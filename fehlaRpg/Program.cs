using System;


namespace FehlaRpg
{    
    class Program
    {
        static void Main(string[] args)
        {
            /*
            CanvasRenderer.ClearCanvasBucket(' '); // ░ ▒ ▓ '\0' ' '
            CanvasRenderer.DrawBox(0,0,100,40);
            CanvasRenderer.DrawSpeechBubble(10, 30, 80, 5, 
            @"Frogs are not real! They are actually government surveillance drones disguised as amphibians. Its true! The evidence is overwhelming and widely available if you know where to look."
            );
            */

            //DONE! 1. preAttackText must appear BEFORE combat menu 
            //DONE! --> damage calculation must work 
            // 6. game over/win conditions
            // 2. make combat appear through CanvasRenderer()
            // 3. Make dummy frog Ascii appear on screen
            // 4. add healthbar to dummy frog
            // 5. make combat hud appear with hp/mp
          

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