using System;
using System.Runtime;
using System.Runtime.Serialization.Formatters;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace FehlaRpg
{
    public struct ColoredChar
    {
        public char character;
        public ConsoleColor foregroundColor;
        public ConsoleColor backgroundColor;

        // constructor to make it easier to create ColoredChar instances
        public ColoredChar(char c, ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
            this.character = c;
            this.foregroundColor = fg;
            this.backgroundColor = bg;
        }
    }
    static class CanvasRenderer
    {
        
        public static int canvasX = 100; // weite, columns
        public static int canvasY = 30; // höhe, rows
        public static ColoredChar[,] canvasBucket = new ColoredChar[canvasX, canvasY];

        // leert und ersetzt jede zelle mit null oder angegebenem char
        public static void ClearCanvasBucket(char fill = ' ')
        {
            for (int y = 0; y < canvasY; y++)
                for (int x = 0; x < canvasX; x++)
                    canvasBucket[x, y] = new ColoredChar(fill);
        }

        // schaut ob pixel innerhalb canvas ist, wenn ja TRUE und setzt char "c" an position (x,y)
        public static bool SetPixel(int x, int y, char c, ConsoleColor fg, ConsoleColor bg)
        {
            if (x < 0 || x >= canvasX || y < 0 || y >= canvasY)
            { return false; }
            canvasBucket[x, y] = new ColoredChar(c, fg, bg);
            return true;
        }
        public static bool SetPixel(int x, int y, char c) // overload ohne farben, benutzt standardfarben
        {
            if (x < 0 || x >= canvasX || y < 0 || y >= canvasY)
            { return false; }
            canvasBucket[x, y] = new ColoredChar(c);
            return true;
        }

        // bereitet den canvas als string zum ausgeben vor und gibt ihn zurück
        public static string RenderCanvas()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            
            var sb = new StringBuilder();
            ConsoleColor currentFg = ConsoleColor.White;
            ConsoleColor currentBg = ConsoleColor.Black;

            Console.SetCursorPosition(0, 0); // setzt console cursor and den anfang, damit überschrieben wird statt neue zeilen zu erstellen

            for (int y = 0; y < canvasY; y++) // rows
            {
                for (int x = 0; x < canvasX; x++) // columns
                {
                    ColoredChar pixel = canvasBucket[x, y];

                    // only change color IF they actually differ from current color
                    if (pixel.foregroundColor != currentFg || pixel.backgroundColor != currentBg)
                    {
                        Console.Write(sb.ToString());
                        sb.Clear();
                        // set colors for this character
                        Console.ForegroundColor = pixel.foregroundColor;
                        Console.BackgroundColor = pixel.backgroundColor;
                        currentFg = pixel.foregroundColor;
                        currentBg = pixel.backgroundColor;
                    }
                    sb.Append(pixel.character);
                }
                // new line after each row is done
                sb.Append('\n');
                // Console.WriteLine(); 
            }
            Console.Write(sb.ToString());
            Console.ResetColor(); // reset colors after rendering
            return ""; // return empty string because rendering is done directly to console
        }

        // draws any string at any position of canvas, maxLength limits how many chars are drawn
        public static bool DrawString(int x, int y, string text, int maxLength)
        {
            if (text.Length > maxLength) { text = text.Substring(0, maxLength); }

            bool placedAny = false;
            for (int c = 0; c < text.Length; c++)
            {
                if (SetPixel(x + c, y, text[c])) { placedAny = true; }
            }
            return placedAny;
        }
        public static bool DrawString(int x, int y, string text, int maxLength, ConsoleColor fg, ConsoleColor bg)
        {
            if (text.Length > maxLength) { text = text.Substring(0, maxLength); } // truncate text if too long

            bool placedAny = false;
            for (int c = 0; c < text.Length; c++) // counter for each char in "text" string
            {
                if (SetPixel(x + c, y, text[c], fg, bg)) { placedAny = true; } // "text[c]" is basically string[indexNumber]
            }
            return placedAny; // returns true if at least one char was placed on canvas
        }

        // draws a rectangle at any position with customizable chars for corners and sides, fills space with a char (style is fixed for now)
        public static bool DrawBox(int startX, int startY, int width, int height)
        {
            // chars that are used to draw the rectangle
            char topLeftChar = '┌'; char topRightChar = '┐';
            char bottomLeftChar = '└'; char bottomRightChar = '┘';
            char horizChar = '─'; char vertChar = '│';
            char fillChar = ' '; // do not use '\0' here, it will not be visible

            if (width < 0 || height < 0) { return false; } // invalid dimensions

            bool placedAny = false;

            for (int rowOffset = 0; rowOffset < height; rowOffset++) // counting row offset within a rectangle
            {
                for (int colOffset = 0; colOffset < width; colOffset++) // counting column offset within a rectangle
                {
                    // add offset and start-position to get the canvas position to place char
                    int canvasPosX = startX + colOffset;
                    int canvasPosY = startY + rowOffset;
                    char toPlaceChar;

                    // these booleans are necessary to evaluate cases to place specific chars
                    bool isTop = rowOffset == 0; // is top row of rectangle
                    bool isBottom = rowOffset == height - 1; // the bottom row of rectangle requires -1 because offset starts at 0
                    bool isLeft = colOffset == 0; // is left column of rectangle
                    bool isRight = colOffset == width - 1; // is right column of rectangle requires -1 because offset starts at 0

                    if (isTop && isLeft) { toPlaceChar = topLeftChar; } // top-left corner
                    else if (isTop && isRight) { toPlaceChar = topRightChar; } // top-right corner
                    else if (isBottom && isLeft) { toPlaceChar = bottomLeftChar; } // bottom-left corner
                    else if (isBottom && isRight) { toPlaceChar = bottomRightChar; } // bottom-right corner
                    else if (isTop || isBottom) { toPlaceChar = horizChar; } // top/bottom sides
                    else if (isLeft || isRight) { toPlaceChar = vertChar; } // left/right sides
                    else { toPlaceChar = fillChar; } // fill rectangle space

                    if (SetPixel(canvasPosX, canvasPosY, toPlaceChar)) { placedAny = true; }
                }
            }
            return placedAny; // returns true if at least one char was placed on canvas
        }

        public static bool DrawBorderBoxOnly(int startX, int startY, int width, int height)
        {
            // chars that are used to draw the rectangle
            char topLeftChar = '┌'; char topRightChar = '┐';
            char bottomLeftChar = '└'; char bottomRightChar = '┘';
            char horizChar = '─'; char vertChar = '│';
            // char fillChar = ' '; // do not use '\0' here, it will not be visible

            if (width < 0 || height < 0) { return false; } // invalid dimensions

            bool placedAny = false;

            for (int rowOffset = 0; rowOffset < height; rowOffset++) // counting row offset within a rectangle
            {
                for (int colOffset = 0; colOffset < width; colOffset++) // counting column offset within a rectangle
                {
                    // add offset and start-position to get the canvas position to place char
                    int canvasPosX = startX + colOffset;
                    int canvasPosY = startY + rowOffset;
                    char toPlaceChar;

                    // these booleans are necessary to evaluate cases to place specific chars
                    bool isTop = rowOffset == 0; // is top row of rectangle
                    bool isBottom = rowOffset == height - 1; // the bottom row of rectangle requires -1 because offset starts at 0
                    bool isLeft = colOffset == 0; // is left column of rectangle
                    bool isRight = colOffset == width - 1; // is right column of rectangle requires -1 because offset starts at 0

                    if (isTop && isLeft) { toPlaceChar = topLeftChar; } // top-left corner
                    else if (isTop && isRight) { toPlaceChar = topRightChar; } // top-right corner
                    else if (isBottom && isLeft) { toPlaceChar = bottomLeftChar; } // bottom-left corner
                    else if (isBottom && isRight) { toPlaceChar = bottomRightChar; } // bottom-right corner
                    else if (isTop || isBottom) { toPlaceChar = horizChar; } // top/bottom sides
                    else if (isLeft || isRight) { toPlaceChar = vertChar; } // left/right sides
                    else { continue; } // skip filling rectangle space

                    if (SetPixel(canvasPosX, canvasPosY, toPlaceChar)) { placedAny = true; }
                }
            }
            return placedAny; // returns true if at least one char was placed on canvas
        }

        public static void DrawAsciiArt(int startX, int startY, string asciiArt) // ascciArt as one single string for copy-pasting - FULLSCREEN MUST BE 98x28 chars!!!
        {
            string[] artLines = asciiArt.Replace("\r", "").Split('\n'); // splits asciiArt at new lines into array and removes \r characters
            for (int lineIndex = 0; lineIndex < artLines.Length; lineIndex++) // note: \r has to be removed cuz it messes up line length calculation on windows
            {
                DrawString(startX, startY + lineIndex, artLines[lineIndex], artLines[lineIndex].Length);
            }
        }
        
        public static void DrawAsciiArt(int startX, int startY, string[] asciiArtAsLines) // asciiArt as array of lines - FULLSCREEN MUST BE 98x28 chars!!!
        {
            for (int lineIndex = 0; lineIndex < asciiArtAsLines.Length; lineIndex++)
            {
                string line = asciiArtAsLines[lineIndex];
                DrawString(startX, startY + lineIndex, line, line.Length);
            }
        }

        public static void DrawCombatMenu(int posX, int posY, string[] combatOptions, int currentSelection)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            // draw box with ornaments
            DrawBox(posX, posY, 44, 4); // plain box with border
            DrawBox(posX, posY, 2, 4);  // draw left ornament
            DrawBox(posX + 42, posY, 2, 4); // draw right ornament

            // draw hp bar // HP [■■■■■■■■▪▪]
            DrawString(posX + 3, posY + 1, "HP", 2);
            SetPixel(posX + 6, posY + 1, '[');
            int hpFill = (int)((float)Player.hope / Player.hopeMax * 10); // calculates how many filled blocks to draw
            for (int i = 0; i < 10; i++)
            {
                char barChar = i < hpFill ? '■' : '▪';
                SetPixel(posX + 7 + i, posY + 1, barChar, ConsoleColor.Yellow, ConsoleColor.Black);
            }
            SetPixel(posX + 17, posY + 1, ']');

            // draw mp bar // MP [■■■■■■■■▪▪]
            DrawString(posX + 3, posY + 2, "MP", 2);
            SetPixel(posX + 6, posY + 2, '[');
            int mpFill = (int)((float)Player.metapower / Player.metapowerMax * 10); // calculates how many filled blocks to draw
            for (int i = 0; i < 10; i++)
            {
                char barChar = i < mpFill ? '■' : '▪';
                SetPixel(posX + 7 + i, posY + 2, barChar, ConsoleColor.Cyan, ConsoleColor.Black);
            }
            SetPixel(posX + 17, posY + 2, ']');

            // draw actions in first row
                // action1 at (posX + 21, posY + 1)
                // action2 at (posX + 21 + 5 + 2, posY + 1)
                // action3 at (posX + 21 + 5 + 2 + 5 + 2, posY + 1)
            // draw actions in second row
                // action4 at (posX + 21, posY + 2)
                // action5 at (posX + 21 + 5 + 2, posY + 2)
                // action6 at (posX + 21 + 5 + 2 + 5 + 2, posY + 2)
            for (int i = 0; i < combatOptions.Length; i++) // loop made with help of AI, the math was too tricky for me 
            {
                int row = i / 3; // which row (0 or 1)
                int col = i % 3; // which column (0,1 or 2)

                int actionPosX = posX + 21 + (col * 7); // start at 21, then add 7 for each column
                int actionPosY = posY + 1 + row; // start at row 1, then add 1 for second row

                if (i == currentSelection) // highlight selected option
                {
                    SetPixel(actionPosX - 1, actionPosY, ' ', ConsoleColor.White, ConsoleColor.Red);
                    DrawString(actionPosX, actionPosY, combatOptions[i].ToUpper(), 5, ConsoleColor.White, ConsoleColor.Red);
                    SetPixel(actionPosX + 5, actionPosY, ' ', ConsoleColor.White, ConsoleColor.Red);
                }
                else // not selected option has normal colors
                {
                    SetPixel(actionPosX - 1, actionPosY, ' ', ConsoleColor.White, ConsoleColor.Black);
                    DrawString(actionPosX, actionPosY, combatOptions[i].ToUpper(), 5, ConsoleColor.White, ConsoleColor.Black);
                    SetPixel(actionPosX + 5, actionPosY, ' ', ConsoleColor.White, ConsoleColor.Black);
                }
            }

            RenderCanvas();
        }

        public static void DrawEncounterStatus(int posX, int posY)
        {
            Console.OutputEncoding = Encoding.UTF8; // enable utf8 for special chars
            
            // zeichne die encounter box im hintergrund
            DrawBox(posX, posY, 70, 3);
            DrawString(posX + 2, posY + 1, Game.currentGameEncounter.encName, 20); // name des encounters
            
            // attack indicator an stelle (posX + 22, posY + 1), 3 chars breit
            switch (Game.currentGameEncounter.attackForThisTurn.dmgSeverity)
            {
                case Severity.Minor:
                    DrawString(posX + 22, posY + 1, "[●]", 3, ConsoleColor.Yellow, ConsoleColor.Black);
                    break;
                case Severity.Moderate:
                    DrawString(posX + 22, posY + 1, "[■]", 3, ConsoleColor.Yellow, ConsoleColor.Black);
                    break;
                case Severity.Severe:
                    DrawString(posX + 22, posY + 1, "[▲]", 3, ConsoleColor.Red, ConsoleColor.Black);
                    break;
                case Severity.Critical:
                    DrawString(posX + 22, posY + 1, "[!]", 3, ConsoleColor.DarkRed, ConsoleColor.Black);
                    break;
                default:
                    DrawString(posX + 22, posY + 1, "[-]", 3, ConsoleColor.Gray, ConsoleColor.Black);
                    break;
            }

            // DMG [■■■■■■■■  ] // wie viel schaden encounter bereits bekommen hat
            DrawString(posX + 27, posY + 1, "DMG", 3); 
            // driveHP bar linkes symbol an stelle (posX + 31, posY + 1),
            SetPixel(posX + 31, posY + 1, '[');
            // driveHP bar an stelle (posX + 32, posY + 1), länge 13
            int driveFill = (int)((float)Game.currentGameEncounter.driveHPc / Game.currentGameEncounter.driveHPmax * 13); // calculates how many filled blocks to draw
            for (int i = 0; i < 13; i++)
            {
                char barChar = i < driveFill ? '■' : ' ';
                SetPixel(posX + 32 + i, posY + 1, barChar, ConsoleColor.Green, ConsoleColor.Black);
            }
            // driveHP bar rechtes symbol an stelle (x + 45, y + 1),
            SetPixel(posX + 45, posY + 1, ']');

            // PAT [■■■■■■■■  ] // wie viel patience encounter noch hat
            DrawString(posX + 48, posY + 1, "PAT", 3); // patience label
            // patience bar linkes symbol an stelle (x + 52, y + 1), 
            SetPixel(posX + 52, posY + 1, '[');
            // patience bar an stelle (x + 53, y + 1), länge 13
            int patienceFill = (int)((float)Game.currentGameEncounter.patienceCurrent / Game.currentGameEncounter.patienceMax * 13); // calculates how many filled blocks to draw
            for (int i = 0; i < 13; i++)
            {
                char barChar = i < patienceFill ? '■' : ' ';
                SetPixel(posX + 53 + i, posY + 1, barChar, ConsoleColor.Green, ConsoleColor.Black);
            }
            // patience bar rechtes symbol an stelle (x + 67, y + 1)
            SetPixel(posX + 67, posY + 1, ']');

            RenderCanvas();
        }

        // DrawSpeechBubble(x, y, width, height, string paragraph) it draws a speech bubble with word-wrapping and pagination for long text
        public static void DrawSpeechBubble(int startX, int startY, int boxWidth, int boxHeight, string paragraph, bool waitForConfirm = false)
        {
            int maxLineLength = boxWidth - 4; // leave space for box borders

            // Split into words
            var words = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Wrap words into lines
            var lines = new List<string>(); // how many lines were already written?
            string currentLine = "";
            foreach (var word in words)
            {
                // modulo the amount of already written lines by 3 and get the rest, which is the the line to write
                // then if its line 3 (index 2) shorten max length and pass it into the if-statement for wrapping
                int effectiveLineLength = (lines.Count % 3 == 2) ? maxLineLength - 4 : maxLineLength;

                if ((currentLine.Length + word.Length + (currentLine.Length > 0 ? 1 : 0)) <= effectiveLineLength)
                {
                    currentLine += (currentLine.Length > 0 ? " " : "") + word;
                }
                else
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
            }
            if (!string.IsNullOrEmpty(currentLine)) lines.Add(currentLine);


            // Draw Box then draw lines into it
            int maxLinesPerBubble = 3; // how many lines are in one speechBubble
            int linesDrawnSoFar = 0; // how many lines have that have already been drawn to canvas

            while (linesDrawnSoFar < lines.Count)
            {
                DrawBox(startX, startY, boxWidth, boxHeight); // draw the basic rectangle for speechBubble

                for (int sbLineCurrent = 0; sbLineCurrent < maxLinesPerBubble; sbLineCurrent++) // sbLineCurrent is the current line where text is supposed to be drawn 
                {
                    int absoluteIndex = linesDrawnSoFar + sbLineCurrent; // calculates the absolute index in "lines" list
                    if (absoluteIndex >= lines.Count) break; // when exceeding the total amount of lines in "lines" list, break the loop to stop DrawString()
                    
                    // if its line 3 (index 2) then adjust DrawString() maxlength by - 4, if not keep maxLineLength
                    int adjustedMaxLength = (sbLineCurrent == 2) ? maxLineLength - 4 : maxLineLength;
                    DrawString(startX + 2, startY + 1 + sbLineCurrent, lines[absoluteIndex], adjustedMaxLength); 
                }
                
                // are there any more lines to draw after this bubble? if yes, wait for key press to continue
                if (linesDrawnSoFar + maxLinesPerBubble < lines.Count)
                {   
                    DrawString(startX + boxWidth - 5, startY + boxHeight - 2, " ▼ ", 3, ConsoleColor.White, ConsoleColor.Red);
                    Console.OutputEncoding = Encoding.UTF8;
                    WaitForConfirm(); // wait for player to confirm before drawing next bubble
                }
                else 
                {
                    if (waitForConfirm)
                    {
                        DrawString(startX + boxWidth - 5, startY + boxHeight - 2, " ▼ ", 3, ConsoleColor.White, ConsoleColor.Red);
                    }
                    else
                    {
                        DrawString(startX + boxWidth - 5, startY + boxHeight - 2, "...", 3, ConsoleColor.White, ConsoleColor.Black);
                    }
                    Console.OutputEncoding = Encoding.UTF8;
                }

                RenderCanvas();
                linesDrawnSoFar += maxLinesPerBubble; // move to next chunk of 3 lines for next bubble
            }
        }
        public static void DrawSpeechBubbleSequence(int startX, int startY, int boxWidth, int boxHeight, IEnumerable<string> bubbleTexts)
        {
            foreach (var text in bubbleTexts)
            {
                DrawSpeechBubble(startX, startY, boxWidth, boxHeight, text);
                WaitForConfirm();
            }
        }
        public static void ColorPixel(int x, int y, ConsoleColor fg, ConsoleColor bg)
        {
            if (x >= 0 && x < canvasX && y >= 0 && y < canvasY) // is it inside canvas?
            {
                // Get existing char/pixel/cell and update its colors
                char existingChar = canvasBucket[x, y].character;
                canvasBucket[x, y] = new ColoredChar(existingChar, fg, bg);
            }
        }
        public static void ColorRectangle(int startX, int startY, int width, int height, ConsoleColor fg, ConsoleColor bg)
        {
            // iterate through each pixel/cell in the rectangle
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int posX = startX + x;
                    int posY = startY + y;
                    
                    if (posX >= 0 && posX < canvasX && posY >= 0 && posY < canvasY) // is it inside canvas?
                    {
                        // Get existing chars/pixels/cells and update its colors
                        char existingChar = canvasBucket[posX, posY].character;
                        canvasBucket[posX, posY] = new ColoredChar(existingChar, fg, bg);
                    }
                }
            }
        }
        public static void WaitForConfirm()
        {
            RenderCanvas();
            while (true)
            {
                if (Player.ButtonInput() == Button.Confirm) break;
            }

            while (Console.KeyAvailable) Console.ReadKey(intercept: true); // clear inputbuffer of all inputs

        }
        public static void SetGameWindow(int width, int height)
        {
            try
            {
                Console.BufferWidth = width;
                Console.BufferHeight = height;
                Console.SetWindowSize(width + 10, height + 5); // extra space to avoid scrollbars
            }
            catch 
            {    
                try
                {    
                    Console.SetBufferSize(width, height);
                    Console.SetWindowSize(width + 10, height + 5); // extra space to avoid scrollbars
                } catch { }
            }
        } 
        public static void DrawTitleScreen()
        {
            ClearCanvasBucket(' ');
            DrawBox(0, 0, 100, 30); // draw border

            Console.OutputEncoding = Encoding.UTF8;
            string fehlaTitleArt = @" /$$$$$$$$        /$$       /$$    ___   
| $$_____/       | $$      | $$    \\/   
| $$     /$$$$$$ | $$$$$$$ | $$ /'$$$$$$ 
| $$$$$ /$$__  $$| $$__  $$| $$| $$__  $$
| $$__/| $$$$$$$$| $$  \ $$| $$| $$  | $$
| $$   | $$_____/| $$  | $$| $$| $$$$$$$$
| $$   |  $$$$$$$| $$  | $$| $$| $$  | $$
|__/    \_______/|__/  |__/|__/|__/  |__/";
            // 41 chars weit, 9 lines hoch
            DrawAsciiArt(30, 5, fehlaTitleArt);
            DrawString(39, 18, "PRESS  DOWN  TO START ...", 25);
            DrawString(42, 25, "A game by Vilithos", 18);

            // DrawString(40, 20, "", 25); // subline for the title screen
            
            ColorRectangle(61, 5, 10, 8, ConsoleColor.Red, ConsoleColor.Black); // kleines "a" rot einfärben
            ColorRectangle(65, 5, 3, 2, ConsoleColor.Yellow, ConsoleColor.Black); // kleines "r" gelb einfärben
            ColorRectangle(45, 18, 6, 1, ConsoleColor.White, ConsoleColor.Red); // "DOWN" hintergrund rot einfärben
            RenderCanvas();
        
            WaitForConfirm();

            ClearCanvasBucket(' ');
            DrawBox(0, 0, 100, 30);
            RenderCanvas();
        }
        public static void DrawGameOverScreen()
        {
            // roter bg, weißer text
            ColorRectangle(0, 0, canvasX, canvasY - 1, ConsoleColor.White, ConsoleColor.Red);
            RenderCanvas();
            Thread.Sleep(200);   
            
            // schwarzer bg, dunkelroter text
            ColorRectangle(0, 0, canvasX, canvasY, ConsoleColor.DarkRed, ConsoleColor.Black);
            RenderCanvas();
            Thread.Sleep(100);   

            // dunkelroter bg, dunkelgrauer text damit text verschwindet
            ColorRectangle(0, 0, canvasX, canvasY - 1, ConsoleColor.DarkRed, ConsoleColor.DarkRed);
            RenderCanvas();
            Thread.Sleep(200);   

            // canvas komplett schwarz
            ClearCanvasBucket(' ');
            ColorRectangle(0,0,canvasX,canvasY, ConsoleColor.Black, ConsoleColor.Black);
            RenderCanvas();
            Thread.Sleep(1000);

            // GAME OVER text taucht auf
            ColorRectangle(0, 0, canvasX, canvasY, ConsoleColor.DarkRed, ConsoleColor.Black);
            DrawBox(37, 14, 26, 3);
            ColorRectangle(37, 14, 26, 3, ConsoleColor.DarkRed, ConsoleColor.Black);
            DrawString(39, 15, "THE GAME IS OVER  ... ", 22, ConsoleColor.DarkRed, ConsoleColor.Black);
            RenderCanvas();

            // dramatisch ... highlighten mit rot
            Thread.Sleep(5000);
            ColorRectangle(56, 15, 5, 1, ConsoleColor.White, ConsoleColor.Red);
            while (Console.KeyAvailable) Console.ReadKey(intercept: true);
            RenderCanvas();
            Thread.Sleep(2000);

            while (true) // wait for confirm key
            {
                WaitForConfirm(); break;
            }

            // canvas komplett schwarz
            ClearCanvasBucket(' ');
            ColorRectangle(0,0,canvasX,canvasY, ConsoleColor.Black, ConsoleColor.Black);
            RenderCanvas();
            Thread.Sleep(1000);
            

            DrawString(39, 15, " ...  only Fehla prevails. ", 27, ConsoleColor.White, ConsoleColor.Black);
            ColorPixel(39 + 15, 15, ConsoleColor.Red, ConsoleColor.Black); // färbe "a" rot
            RenderCanvas();
            Thread.Sleep(3000);

            ClearCanvasBucket(' ');
            DrawBox(0, 0, 100, 30);
            ColorRectangle(0, 0, canvasX, canvasY, ConsoleColor.White, ConsoleColor.Black);
            RenderCanvas();

            while (Console.KeyAvailable) Console.ReadKey(intercept: true);
            Thread.Sleep(200);
            Game.runningGame = false;
            
        }
        public static void DrawOpeningScene()
        {
            DrawBox(0, 0, 100, 30); // draw border
            
        }
    }
}