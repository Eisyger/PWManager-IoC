using PWManager.Entity;
using PWManager.Interfaces;
using PWManager.Services;

public abstract class Handler
{
    protected Handler NextHandler;

    // Setzt den nächsten Handler in der Kette
    public void SetNext(Handler next)
    {
        NextHandler = next;
    }

    // Verarbeitet die Datenstruktur
    public void Handle(IContextService context)
    {
        Process(context);

        // Weitergabe an den nächsten Handler, falls vorhanden
        NextHandler?.Handle(context);
    }

    // Abstrakte Methode, die von den konkreten Handlers implementiert wird
    protected abstract void Process(IContextService context);
}