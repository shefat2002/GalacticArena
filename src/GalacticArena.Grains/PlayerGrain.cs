using GalacticArena.Interfaces;
using Microsoft.Extensions.Logging;


namespace GalacticArena.Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    // IPersistentState wrapper holds your data and handles database read/writes
    private readonly IPersistentState<PlayerState> _playerState;
    private readonly ILogger<PlayerGrain> _logger;
    
    public PlayerGrain(
        // The attributes tell Orleans where to save this data. 
        // "playerStore" is a name we will map to PostgreSQL later in the Silo config.
        [PersistentState(stateName: "profile", storageName: "playerStore")] IPersistentState<PlayerState> playerState,
        ILogger<PlayerGrain> logger)
    {
        _playerState = playerState;
        _logger = logger;
    }
    
    public Task<PlayerState> GetProfileAsync()
    {
        return Task.FromResult(_playerState.State);
    }

    public async Task AddExperienceAsync(int xp)
    {
        _playerState.State.Experience += xp;
        
        if (_playerState.State.Experience >= 100 * _playerState.State.Level)
        {
            _playerState.State.Level++;
            _playerState.State.Experience = 0;
            
            _logger.LogInformation("Player {PlayerId} leveled up to {Level}!", this.GetPrimaryKeyString(), _playerState.State.Level);
        }

        await _playerState.WriteStateAsync();
    }

    public async Task RestoreHealthAsync()
    {
        _playerState.State.Health = 100;
        await _playerState.WriteStateAsync();
    }

    public async Task SetMatchStatusAsync(bool isInMatch)
    {
        _playerState.State.IsInMatch = isInMatch;
        await _playerState.WriteStateAsync();
    }
}