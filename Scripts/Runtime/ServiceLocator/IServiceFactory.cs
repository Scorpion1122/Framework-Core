namespace Thijs.Core.Services
{
    public interface IServiceFactory
    {
        object GetInstance(IServiceLocator source);
        void DisposeOfInstance(object instance);
    }
}
