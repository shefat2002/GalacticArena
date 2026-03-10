using GalacticArena.Interfaces;
using Microsoft.Extensions.Logging;

namespace GalacticArena.Grains;

public class MatchGrain : Grain, IMatchGrain
{
    private readonly IPersistentState<MatchState> _matchState;
    private readonly ILogger<MatchGrain> _logger;

    public MatchGrain(
        [PersistentState(stateName: "match", storageName: "matchStore")] IPersistentState<MatchState> matchState,
        ILogger<MatchGrain> logger)
    {
        _matchState = matchState;
        _logger = logger;
    }

    public async Task<bool> JoinMatchAsync(string playerId)
    {
        // 1. Check if the room is full
        if (!string.IsNullOrEmpty(_matchState.State.Player1) && !string.IsNullOrEmpty(_matchState.State.Player2))
        {
            _logger.LogWarning("Match {MatchId} is already full.", this.GetPrimaryKey());
            return false;
        }

        // 2. Cross-Grain Communication: Get the Player Grain
        var playerGrain = this.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        
        // Check if the player is already in another match
        var playerProfile = await playerGrain.GetProfileAsync();
        if (playerProfile.IsInMatch)
        {
            _logger.LogWarning("Player {PlayerId} is already in a match!", playerId);
            return false;
        }

        // 3. Assign to a slot
        if (string.IsNullOrEmpty(_matchState.State.Player1))
            _matchState.State.Player1 = playerId;
        else
            _matchState.State.Player2 = playerId;

        // 4. Update the player's status so they can't join another game
        await playerGrain.SetMatchStatusAsync(true);

        if (!string.IsNullOrEmpty(_matchState.State.Player1) && !string.IsNullOrEmpty(_matchState.State.Player2))
        {
            _matchState.State.IsActive = true;
            _logger.LogInformation("Match {MatchId} has started between {P1} and {P2}!", this.GetPrimaryKey(), _matchState.State.Player1, _matchState.State.Player2);
        }

        await _matchState.WriteStateAsync();
        return true;
    }

    public async Task EndMatchAsync(string winnerId)
    {
        if (!_matchState.State.IsActive) return;

        _matchState.State.Winner = winnerId;
        _matchState.State.IsActive = false;

        // Reward the winner
        var winnerGrain = this.GrainFactory.GetGrain<IPlayerGrain>(winnerId);
        await winnerGrain.AddExperienceAsync(50);

        // Free up both players
        var p1Grain = this.GrainFactory.GetGrain<IPlayerGrain>(_matchState.State.Player1);
        var p2Grain = this.GrainFactory.GetGrain<IPlayerGrain>(_matchState.State.Player2);
        
        await p1Grain.SetMatchStatusAsync(false);
        await p2Grain.SetMatchStatusAsync(false);

        await _matchState.WriteStateAsync();
        
        _logger.LogInformation("Match {MatchId} ended. Winner: {WinnerId}", this.GetPrimaryKey(), winnerId);
    }

    public Task<MatchState> GetMatchStateAsync()
    {
        return Task.FromResult(_matchState.State);
    }
}