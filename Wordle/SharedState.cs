public class SharedState
{
    private static readonly Dictionary<string, SharedState> _instances = new Dictionary<string, SharedState>();
    private static readonly object _lock = new object();

    public static SharedState GetInstance(string matchId)
    {
        lock (_lock)
        {
            if (!_instances.ContainsKey(matchId))
            {
                _instances[matchId] = new SharedState();
            }
            return _instances[matchId];
        }
    }
    private string _guess;
    public string Guess
    {
        get
        {
            lock (_lock)
            {
                return _guess;
            }
        }
        set
        {
            lock (_lock)
            {
                _guess = value;
            }
        }
    }

    private string _targetWord;
    public string TargetWord
    {
        get
        {
            lock (_lock)
            {
                return _targetWord;
            }
        }
        set
        {
            lock (_lock)
            {
                _targetWord = value;
            }
        }
    }
}