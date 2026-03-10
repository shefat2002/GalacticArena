namespace GalacticArena.Interfaces;

public interface IMatchGrain : IGrainWithGuidKey
{
    // Attempts to add a player to the match. Returns true if successful.
    Task<bool> JoinMatchAsync(string playerId);
    
    // Concludes the match, awards XP, and frees up the players.
    Task EndMatchAsync(string winnerId);
    
    Task<MatchState> GetMatchStateAsync();
}