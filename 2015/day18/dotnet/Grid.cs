namespace day18;

internal class Grid
{
    public int Width { get; }
    public int Height { get; }
    public Light[,] Lights { get; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        Lights = new Light[width, height];
    }

    public void Print()
    {
        //Console.Clear();

        int numOn = 0;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var isOn = Lights[x, y].State;
                
                Console.Write(isOn ? '#' : '.');

                if (isOn)
                    numOn++;
            }
            Console.WriteLine();
        }
        
        Console.WriteLine($"NumOn: {numOn}");
    }
}