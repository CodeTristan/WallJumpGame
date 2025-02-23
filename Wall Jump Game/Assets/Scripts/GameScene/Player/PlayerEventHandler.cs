public class PlayerEventHandler
{
    public delegate void OnPlayerJumpDelegate();
    public static event OnPlayerJumpDelegate OnPlayerJump;

    public delegate void OnTouchWallDelegate();
    public static event OnTouchWallDelegate OnTouchWall;

    public delegate void OnLeaveWallDelegate();
    public static event OnTouchWallDelegate OnLeaveWall;

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
        OnTouchWall?.Invoke();
    }

    public static void LeaveWall()
    {
        OnLeaveWall?.Invoke();
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
