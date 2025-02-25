public class PlayerEventHandler
{
    public static PlayerEventHandler instance;

    public delegate void OnPlayerJumpDelegate();
    public event OnPlayerJumpDelegate OnPlayerJump;

    public delegate void OnEnterWallDelegate();
    public event OnEnterWallDelegate OnEnterWall;

    public delegate void OnEnterInvisibleWallDelegate();
    public event OnEnterInvisibleWallDelegate OnEnterInvisibleWall;

    public delegate void OnLeaveInvisibleWallDelegate();
    public event OnEnterInvisibleWallDelegate OnLeaveInvisibleWall;

    public delegate void OnLeaveWallDelegate();
    public event OnEnterWallDelegate OnLeaveWall;

    public delegate void OnEnemyKilledDelegate();
    public event OnEnemyKilledDelegate OnEnemyKilled;

    public delegate void OnPlayerDamagedDelegate();
    public event OnPlayerDamagedDelegate OnPlayerDamaged;

    public delegate void OnPlayerDiedDelegate();
    public event OnPlayerDiedDelegate OnPlayerDied;


    public PlayerEventHandler()
    {
        instance = this;
    }

    public void PlayerJump()
    {
        OnPlayerJump?.Invoke();
    }

    public void TouchWall()
    {
        OnEnterWall?.Invoke();
    }

    public void TouchInvisibleWall()
    {
        OnEnterInvisibleWall?.Invoke();
    }

    public void LeaveWall()
    {
        OnLeaveWall?.Invoke();
    }

    public void LeaveInvisibleWall()
    {
        OnLeaveInvisibleWall?.Invoke();
    }

    public void EnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }

    public void PlayerDamaged()
    {
        OnPlayerDamaged?.Invoke();
    }

    public void PlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

}
