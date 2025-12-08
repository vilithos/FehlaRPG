using System;
using System.Diagnostics;
using System.Threading;

namespace FehlaRpg
{
    class ENGINE
    {
        // diese klasse generiert frames pro sekunde und in welcher methoden verschiedene komponente des spiels in echtzeit aufgerufen werden
        // sie verwaltet auch die reihenfolge der renderobjekte und deren aktualisierung
        // außerdem gibt es win paar methoden die kleine grafische effekte erzeugen können wie bildschirm flackern, blinkende symbole etc.
        

        // methode die 30 frames pro sekunde methoden ausführt
        public static void AtRealtime()
        {
            int targetFPS = 1;
            // The duration each frame should take in milliseconds (~33.33 ms for 30 FPS)
            double timePerFrameMs = 1000.0 / targetFPS; 

            Stopwatch stopwatch = new Stopwatch(); // neues stopwatch objekt
            stopwatch.Start(); // starte stopwatch

            while (true) // The main game loop
            {
                long startOfFrameMs = stopwatch.ElapsedMilliseconds; // frame start-zeit ist quasi 0

                // ------------------------------------------- Code und Methoden die in Echtzeit ausgeführt werden sollen --------------------------------------------------
                



                

                // ------------------------------------------- Ende für in Echtzeit ausgeführte Methoden und codeblöcke ----------------------------------------------------
                
                // frame timing berechnung zum synchronisieren der frames pro sekunde
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
}