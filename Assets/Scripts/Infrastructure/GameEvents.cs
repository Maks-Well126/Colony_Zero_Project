using System;

public static class GameEvents
{
    public static event Action OnAsistentEndDialoge;
    public static event Action OnRadialOpen;
    public static event Action OnRobotStart;





    public static event Action OnPlayerBuiltBase;
    public static event Action OnArtefactFound;
    public static event Action OnEnemyWaveStarted;

    public static void PlayerBuiltBase()
    {
        OnPlayerBuiltBase?.Invoke();
    }

    public static void ArtefactFound()
    {
        OnArtefactFound?.Invoke();
    }

    public static void EnemyWaveStarted()
    {
        OnEnemyWaveStarted?.Invoke();
    }
}