[System.Serializable]
public class ResourceValue
{
    public InGameResource code;
    public int amount = 0;

    public ResourceValue(InGameResource code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}
