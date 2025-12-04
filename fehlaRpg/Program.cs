using System;


namespace FehlaRpg
{    
    class Program
    {
        static void Main(string[] args)
        {
            
            CanvasRenderer.ClearCanvasBucket(' '); // ░ ▒ ▓ '\0' ' '

            CanvasRenderer.DrawSpeechBubble(10, 5, 50, 5, 
            @"Frogs are not real! They are actually government surveillance drones disguised as amphibians. Its true! The evidence is overwhelming and widely available if you know where to look."
            );

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