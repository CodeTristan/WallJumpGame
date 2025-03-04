public class GameSceneEventHandler
{
    public static GameSceneEventHandler instance;

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

    public delegate void OnPlayerDiedFRDelegate();
    public event OnPlayerDiedFRDelegate OnPlayerDiedFR;


    public GameSceneEventHandler()
    {
        instance = this;

        PlayerHealth playerHealth = PlayerManager.instance.playerHealth;
        PlayerPowerUps playerPowerUps = PlayerManager.instance.playerPowerUps;
        PlayerCollisionHandler playerCollisionHandler = PlayerManager.instance.playerCollisionHandler;
        PlayerMovement playerMovement = PlayerManager.instance.playerMovement;
        PlayerSprite playerSprite = PlayerManager.instance.playerSprite;
        



        OnPlayerJump += playerMovement.Jump;
        OnPlayerJump +=  GameSceneUIManager.instance.UpdateJumpCountText;

        OnEnterWall += playerMovement.OnEnterWall;

        OnEnterInvisibleWall += playerMovement.OnEnterInvisibleWall;

        OnLeaveWall += playerMovement.OnExitWall;

        OnLeaveInvisibleWall += playerMovement.OnExitInvisibleWall;

        OnEnemyKilled += playerMovement.EnemyKilled;
        OnEnemyKilled += playerCollisionHandler.EnemyKilled;

        OnPlayerDamaged += playerHealth.TakeDamage;

        OnPlayerDied += playerCollisionHandler.OnPlayerDie;
        OnPlayerDied += playerMovement.OnPlayerDie;
        OnPlayerDied += PlayerManager.instance.OnPlayerDied;

        OnPlayerDiedFR += playerHealth.Die;
        OnPlayerDiedFR += PlayerManager.instance.OnPlayerDieFR;
        OnPlayerDiedFR += AdManager.instance.OnPLayerDiedFR;
        OnPlayerDiedFR += GameSceneUIManager.instance.DeathScreen;
    }

    public void UnSubscribeAll()
    {
        PlayerHealth playerHealth = PlayerManager.instance.playerHealth;
        PlayerPowerUps playerPowerUps = PlayerManager.instance.playerPowerUps;
        PlayerCollisionHandler playerCollisionHandler = PlayerManager.instance.playerCollisionHandler;
        PlayerMovement playerMovement = PlayerManager.instance.playerMovement;
        PlayerSprite playerSprite = PlayerManager.instance.playerSprite;




        OnPlayerJump -= playerMovement.Jump;
        OnPlayerJump -= GameSceneUIManager.instance.UpdateJumpCountText;

        OnEnterWall -= playerMovement.OnEnterWall;

        OnEnterInvisibleWall -= playerMovement.OnEnterInvisibleWall;

        OnLeaveWall -= playerMovement.OnExitWall;

        OnLeaveInvisibleWall -= playerMovement.OnExitInvisibleWall;

        OnEnemyKilled -= playerMovement.EnemyKilled;
        OnEnemyKilled -= playerCollisionHandler.EnemyKilled;

        OnPlayerDamaged -= playerHealth.TakeDamage;

        OnPlayerDied -= PlayerManager.instance.OnPlayerDied;
        OnPlayerDied -= playerMovement.OnPlayerDie;
        OnPlayerDied -= playerCollisionHandler.OnPlayerDie;

        OnPlayerDiedFR -= GameSceneUIManager.instance.DeathScreen;
        OnPlayerDiedFR -= playerHealth.Die;
        OnPlayerDiedFR -= PlayerManager.instance.OnPlayerDieFR;
        OnPlayerDiedFR -= AdManager.instance.OnPLayerDiedFR;

    }


    public void PlayerJump()
    {
        OnPlayerJump?.Invoke();
    }

    public void EnterWall()
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

    public void PlayerDiedFR()
    {
        OnPlayerDiedFR?.Invoke();
    }

    public void PlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

}
