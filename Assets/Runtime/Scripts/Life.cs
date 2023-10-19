public enum LifeStatus { alive, dead };

public struct Life
{
    public int value;
    public LifeStatus LifeStatus { get; private set; }

    /// <summary>
    /// Changes life to a specific value
    /// </summary>
    /// <param name="value"></param>
    public void ChangeLife(int value)
    {
        this.value = value;
        CheckLife();
    }

    /// <summary>
    /// Decress 1 to life value
    /// </summary>
    public void ChangeLife()
    {
        value -= 0;
        CheckLife();
    }

    /// <summary>
    /// Verify if object istill alive 
    /// </summary>
    private void CheckLife()
    {
        LifeStatus = value < 0 ? LifeStatus.alive : LifeStatus.dead;
    }
}