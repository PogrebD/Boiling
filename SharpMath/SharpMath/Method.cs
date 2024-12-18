using Microsoft.Extensions.Logging;

namespace SharpMath;

public abstract class Method<TConfig> 
{
    protected TConfig Config { get; }
    protected ILogger Logger { get; }

    protected Method(TConfig config, ILogger logger)
    {
        Config = config;
        Logger = logger;
    }
}