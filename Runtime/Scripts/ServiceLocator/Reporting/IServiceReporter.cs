namespace TKO.Core.Services
{
    public interface IServiceReporter
    {
        void RegisterServices(ServiceLocator locator);
        void ReleaseServices(ServiceLocator locator);
    }
    
    public interface IRuntimeServiceReporter : IServiceReporter
    {
    }
    
    public interface IEditorServiceReporter : IServiceReporter
    {
    }
}