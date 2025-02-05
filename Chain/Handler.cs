using PWManager.Chain;

namespace PWManager.Chain;

public abstract class Handler
{
    private Handler? _nextHandler;

    // Setzt den nächsten Handler in der Kette
    public void SetNext(Handler next)
    {
        _nextHandler = next;
    }

    // Verarbeitet die Datenstruktur
    public void Handle(ChainContext data)
    {
        Process(data);

        // Weitergabe an den nächsten Handler, falls vorhanden
        _nextHandler?.Handle(data);
    }

    // Abstrakte Methode, die von den konkreten Handlers implementiert wird
    protected abstract void Process(ChainContext data);
}

public abstract class HandlerAsync
{
    private HandlerAsync? _nextHandler;

    // Setzt den nächsten Handler in der Kette
    public void SetNext(HandlerAsync next)
    {
        _nextHandler = next;
    }

    // Verarbeitet die Datenstruktur
    public async Task Handle(ChainContext data)
    {
        await Process(data);

        // Weitergabe an den nächsten Handler, falls vorhanden
        if (_nextHandler != null)
        {
            await _nextHandler.Handle(data);
        }
    }

    // Abstrakte Methode, die von den konkreten Handlers implementiert wird
    protected abstract Task Process(ChainContext data);
}