public class SharedState
{
    // Private constructor to prevent other classes from instantiating it
    private SharedState() { }

    // Static variable to hold a reference to the single created instance
    private static SharedState _instance;

    // Public static means of getting the reference to the single created instance
    public static SharedState Instance
    {
        get
        {
            // Use lock to ensure thread safety
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SharedState();
                }
                return _instance;
            }
        }
    }

    // Field to hold the guess
    public string Guess { get; set; }

    public string targetWord {  get; set; }

    // Field to hold the lock object
    private static readonly object _lock = new object();
}