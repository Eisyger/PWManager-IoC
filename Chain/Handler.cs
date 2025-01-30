using PWManager.Chain;

public abstract class Handler
{
    protected Handler NextHandler;

    // Setzt den nächsten Handler in der Kette
    public void SetNext(Handler next)
    {
        NextHandler = next;
    }

    // Verarbeitet die Datenstruktur
    public void Handle(ChainContext data)
    {
        Process(data);

        // Weitergabe an den nächsten Handler, falls vorhanden
        NextHandler?.Handle(data);
    }

    // Abstrakte Methode, die von den konkreten Handlers implementiert wird
    protected abstract void Process(ChainContext data);
}