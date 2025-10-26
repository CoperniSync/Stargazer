
public interface IUserContextObserver
{
    void OnUserContextSubmitted(double latitudeDeg, double longitudeDeg, System.DateTime utcTime);
}


public interface IUserContextSubject
{
    void Register(IUserContextObserver observer);
    void Unregister(IUserContextObserver observer);
}
