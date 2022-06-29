
using Utilities;

public class ObjectiveKillEnemies : Objective
{

    #region Fields

    public bool MustKillAllEnemies = true;
    public int KillsToCompleteObjective = 5;
    public int NotificationEnemiesRemainingThreshold = 3;

    int killTotal;

    #endregion
    
    protected override void Start()
    {
        base.Start();

        EventUtil.AddListener<EnemyKillEvent>(OnEnemyKilled);

        // set a title and description specific for this type of objective, if it hasn't one
        if (string.IsNullOrEmpty(Title))
            Title = "Eliminate " + (MustKillAllEnemies ? "all the" : KillsToCompleteObjective.ToString()) +
                    " enemies";

        if (string.IsNullOrEmpty(Description))
            Description = GetUpdatedCounterAmount();
    }

    void OnEnemyKilled(EnemyKillEvent evt)
    {
        if (IsCompleted)
            return;

        killTotal++;

        if (MustKillAllEnemies)
            KillsToCompleteObjective = evt.RemainingEnemyCount + killTotal;

        int targetRemaining = MustKillAllEnemies ? evt.RemainingEnemyCount : KillsToCompleteObjective - killTotal;

        // update the objective text according to how many enemies remain to kill
        if (targetRemaining == 0)
        {
            CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "任务完成 : " + Title);
        }
        else if (targetRemaining == 1)
        {
            string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                ? "只剩下最后一个怪物了"
                : string.Empty;
            UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
        }
        else
        {
            // create a notification text if needed, if it stays empty, the notification will not be created
            string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                ? "剩下" + targetRemaining + " 个怪物需要消灭 "
                : string.Empty;

            UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
        }
    }

    string GetUpdatedCounterAmount()
    {
        return killTotal + " / " + KillsToCompleteObjective;
    }

    void OnDestroy()
    {
        EventUtil.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
    }
}

