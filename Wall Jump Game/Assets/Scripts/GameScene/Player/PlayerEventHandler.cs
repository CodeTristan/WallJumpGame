public class PlayerEventHandler
{
    public delegate void OnPlayerJumpDelegate();
    public static event OnPlayerJumpDelegate OnPlayerJump;

    public delegate void OnEnterWallDelegate();
    public static event OnEnterWallDelegate OnEnterWall;

    public delegate void OnEnterInvisibleWallDelegate();
    public static event OnEnterInvisibleWallDelegate OnEnterInvisibleWall;

    public delegate void OnLeaveInvisibleWallDelegate();
    public static event OnEnterInvisibleWallDelegate OnLeaveInvisibleWall;

    public delegate void OnLeaveWallDelegate();
    public static event OnEnterWallDelegate OnLeaveWall;

    public delegate void OnEnemyKilledDelegate();
    public static event OnEnemyKilledDelegate OnEnemyKilled;

    public delegate void OnPlayerDamagedDelegate();
    public static event OnPlayerDamagedDelegate OnPlayerDamaged;

    public delegate void OnPlayerDiedDelegate();
    public static event OnPlayerDiedDelegate OnPlayerDied;



    public static void PlayerJump()
    {
        OnPlayerJump?.Invoke();
    }

    public static void TouchWall()
    {
        OnEnterWall?.Invoke();
    }

    public static void TouchInvisibleWall()
    {
        OnEnterInvisibleWall?.Invoke();
    }

    public static void LeaveWall()
    {
        OnLeaveWall?.Invoke();
    }

    public static void LeaveInvisibleWall()
    {
        OnLeaveInvisibleWall?.Invoke();
    }

    public static void EnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }

    public static void PlayerDamaged()
    {
        OnPlayerDamaged?.Invoke();
    }

    public static void PlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

}
