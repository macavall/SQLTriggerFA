namespace SQLTriggerFA
{
    public class MyService : IMyService
    {
        public int count = 0;
        public int GetCount()
        {
            count++;
            return count;
        }
        public void AddOneToCount()
        {
            count++;
        }
    }
}