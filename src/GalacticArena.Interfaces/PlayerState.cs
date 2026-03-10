namespace GalacticArena.Interfaces;

// [GenerateSerializer] tells Orleans to build highly optimized serialization code for this class.
[GenerateSerializer]
[Alias("GalacticArena.Interfaces.PlayerState")]
public class PlayerState    
{
    [Id(0)]
    public string Username { get; set; } = string.Empty;

    [Id(1)]
    public int Level { get; set; } = 1;

    [Id(2)]
    public int Experience { get; set; } = 0;

    [Id(3)]
    public int Health { get; set; } = 100;
    
    [Id(4)]
    public bool IsInMatch { get; set; } = false;
}