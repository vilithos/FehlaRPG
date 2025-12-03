// notes

// 1. limit consoe window size to fit canvas
// 2. make sure console is zoomed out enough to see whole canvas
// 3. use a monospace font for best results
// 4. hide cursor for better visual effect

// method that fills a rectangle with empty spaces (useful for hud background)
// FillRectangle(x, y, width, height)

// method that splits a very long string into multiple lines based on max_length and each string as an element in a string[]
// then these strings can be drawn line by line onto the canvas
// StringSplitter(string text, int maxLength) -> string[]

// method that draws the speech bubble box with limited rows of text inside, that waits for player input to continue with next part of text, until all text is shown
// when the speech bubble reached its end it does not disappear but stays on screen until cleared manually
// inside this method the StringSplitter is used to split a long string into multiple lines

// method that draws a rectangular border with characters specific to top/bottom, left/right sides, and corners
// DrawCustombox(x, y, width, height, char fillChar, char topChar, char bottomChar, char sidesChar))

// method that updates HP and MP bars based on current and max values, but how does it work?
// the code takes current and max values, calculates an integer which will fill a char[] with a specific char up to that integer length, and the rest with another char
// out comes a char[] converted to string can be used to draw the bar on the canvas
// UpdateHPBar(currentHP, maxHP)
// UpdateMPBar(currentMP, maxMP)



// Converting a CharArray into a string
char[] charArray = { 'H', 'e', 'l', 'l', 'o', ' ', 'W', 'o', 'r', 'l', 'd' };
string canvasLayer0 = new string(charArray);
// layers are used to overwrite in a specific order to create a final image
string canvasLayer1 = new string();
string canvasLayer2 = new string();
string canvasLayer3 = new string();


bool SetPixel(int x, int y, char c) // schaut ob pixel innerhalb canvas ist, wenn ja TRUE und setzt char "c" an position (x,y)
{
    if (x < 0 || x >= canvasX || y < 0 || y >= canvasY)
        {return false;}
    canvasBucket[x, y] = c;
    return true;
}

// I guess I only need... -------------------------------------------------






// DrawBubble(x, y, width, height, string text, int lineLength) it draws a textbox that wraps text and waits for input to continue
// UpdateHPBar() it generates a string for the HP bar based on current and max HP
// UpdateMPBar() it generates a string for the MP bar based on current and max MP
