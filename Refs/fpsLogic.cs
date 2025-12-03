
using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        int targetFPS = 30;
        // The duration each frame should take in milliseconds (~33.33 ms for 30 FPS)
        double timePerFrameMs = 1000.0 / targetFPS; 

        Stopwatch stopwatch = new Stopwatch(); // neues stopwatch objekt
        stopwatch.Start(); // starte stopwatch

        while (true) // The main game loop
        {
            Console.SetCursorPosition(0, 0); // setzt den weißen konsolen-cursor nach oben links
            long startOfFrameMs = stopwatch.ElapsedMilliseconds; // frame start-zeit ist quasi 0

            // --- Update and Render ---
            // ...
            // --- End Update and Render ---

            long endOfFrameMs = stopwatch.ElapsedMilliseconds; // frame end-zeit ist ...
            double elapsedMs = endOfFrameMs - startOfFrameMs; // berechne zeit-länge des einzelnen frames
            double sleepTimeMs = timePerFrameMs - elapsedMs; // berechne wie lange gewartet muss bis zum nächsten frame

            // ein einzelner frame darf nicht zu schnell abgeschlossen werden, da dadurch das spiel mit besseren prozessoren schneller laufen würde.
            // deswegen soll der einzelne frame die vordefinierte zeit von zB 33,33ms abwarten, bevor der nächste frame losgeht. so sind alle frames 33.33ms lang
            if (sleepTimeMs > 0)  
            {
                // pausiere den thread bis zum nächsten frame
                Thread.Sleep((int)sleepTimeMs);
            }
        }
    }
}

/*
while (true): This creates a continuous loop that runs indefinitely.
Stopwatch: This measures exactly how much real time has passed since the loop started.
timePerFrameMs: This variable defines the time budget for each frame (e.g., 33ms).
Thread.Sleep(): This pauses the program for the remaining time required to hit the target FPS, preventing the program from running too fast on a quick computer.
*/