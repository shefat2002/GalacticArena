namespace GalacticArena.Interfaces;

[GenerateSerializer]
[Alias("GalacticArena.Interfaces.MatchState")]
public class MatchState
{
    [Id(0)] public string Player1 { get; set; } = string.Empty;
    [Id(1)] public string Player2 { get; set; } = string.Empty;
    [Id(2)] public string Winner { get; set; } = string.Empty;
    [Id(3)] public bool IsActive { get; set; } = false;
}