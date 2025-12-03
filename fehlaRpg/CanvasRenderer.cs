using System;
using System.Runtime.Serialization.Formatters;

namespace FehlaRpg
{
    static class CanvasRenderer
    {
        public static int canvasX = 100; // weite, columns
        public static int canvasY = 40; // höhe, rows
        public static char[,] canvasBucket = new char[canvasX, canvasY];

        // leert und ersetzt jede zelle mit null oder angegebenem char
        public static void ClearCanvas(char fill = ' ')
        {
            for (int y = 0; y < canvasY; y++)
                for (int x = 0; x < canvasX; x++)
                    canvasBucket[x, y] = fill;
        }

        // schaut ob pixel innerhalb canvas ist, wenn ja TRUE und setzt char "c" an position (x,y)
        public static bool SetPixel(int x, int y, char c)
        {
            if (x < 0 || x >= canvasX || y < 0 || y >= canvasY)
            { return false; }
            canvasBucket[x, y] = c;
            return true;
        }

        // bereitet den canvas als string zum ausgeben vor und gibt ihn zurück
        public static string RenderCanvas()
        {
            var sb = new System.Text.StringBuilder(canvasX * canvasY + canvasY);
            for (int y = 0; y < canvasY; y++) // rows
            {
                for (int x = 0; x < canvasX; x++) // columns
                    sb.Append(canvasBucket[x, y]); // fügt char an position (x,y) dem stringbuilder hinzu
                sb.AppendLine(); // fügt dem sb eine neue zeile hinzu, ist sozusagen der zeilenumbruch wie "\r\n"
            }
            return sb.ToString(); // gibt den fertigen string zurück
        }

        // DrawString(x, y, string text, int maxLength) it draws any string at any position
        public static bool DrawString(int x, int y, string text, int maxLength)
        {
            if (text.Length > maxLength) { text = text.Substring(0, maxLength); } // truncate text if too long

            bool placedAny = false;
            for (int c = 0; c < text.Length; c++) // counter for each char in "text" string
            {
                if (SetPixel(x + c, y, text[c])) { placedAny = true; } // "text[c]" is basically string[indexNumber]
            }
            return placedAny; // returns true if at least one char was placed on canvas
        }

        // DrawBox(x, y, width, height, char fill) it draws a rectangle at any position with customizable chars for corners and sides, fills space with a char
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


        /*
        // output test
        ClearCanvas('.');                    // fill with visible filler

        // render once (Console.Write writes the whole ASCII image)
        Console.Write(RenderCanvas());
        */
    }
}