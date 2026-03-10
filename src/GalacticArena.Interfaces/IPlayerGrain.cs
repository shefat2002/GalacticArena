namespace GalacticArena.Interfaces;

// IGrainWithStringKey means this Grain will be identified by a string (e.g., their Username or User ID).
public interface IPlayerGrain : IGrainWithStringKey
{
    Task<PlayerState> GetProfileAsync();

    Task AddExperienceAsync(int xp);

    Task RestoreHealthAsync();

    Task SetMatchStatusAsync(bool isInMatch);
}