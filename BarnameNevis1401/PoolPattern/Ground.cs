namespace BarnameNevis1401.PoolPattern;

public class Ground
{
    public void Connect()
    {
        Thread.Sleep(5000);
        IsClosed = true;
        PoolSource.AddGround(this);
    }
    public DateTime CreationDate { get; set; }=DateTime.Now;
    public bool IsClosed { get; set; }

    public void Reset()
    {
        IsClosed = false;
    }
}

public static class PoolSource
{
    private static Queue<Ground> _closedgrounds = new();
    //private static Stack<Ground> stack = new();

    public static void AddGround(Ground ground)
    {
       // stack.Push(ground);
        _closedgrounds.Enqueue(ground);
    }
    public static Ground GetGround()
    {
        {
            if (_closedgrounds.Count > 0)
            {
                var first = _closedgrounds.Dequeue();
                //var last = stack.Pop();
                
                first.Reset();
                return first;
            }

            return new Ground();
        }
    }
}