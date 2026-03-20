using System;

public interface IPlayerDeathHandler
{
    public event Action OnPlayerDied;
}