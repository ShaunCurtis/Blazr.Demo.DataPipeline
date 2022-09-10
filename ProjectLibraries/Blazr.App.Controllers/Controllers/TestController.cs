/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Controllers;

[ApiController]
public class TestController
    : ControllerBase
{
    public TestController() { }

    [Mvc.HttpGet]
    [Mvc.Route("/api/[controller]")]
    public async Task<string> Test(CancellationToken cancellationToken)
    {
        try
        {
            Debug.WriteLine("Started Async API Test method");
            await Task.Delay(5000, cancellationToken);
            Debug.WriteLine("Completed Async API Test method");
            return "Test completed";
        }
        catch (Exception e)
        {
            return $"Something went seriously wrong - error detail: {e.Message}";
        }
    }

    [Mvc.HttpPost]
    [Mvc.Route("/api/[controller]/test")]
    public async Task<string> Test1(CancellationToken cancellationToken)
    {
        try
        {
            Debug.WriteLine("Started Async API Test method");
            await Task.Delay(5000, cancellationToken);
            Debug.WriteLine("Completed Async API Test method");
            return "Test completed";
        }
        catch (Exception e)
        {
            return $"Something went seriously wrong - error detail: {e.Message}";
        }
    }

    [Mvc.HttpPost]
    [Mvc.Route("/api/[controller]/authtest")]
    [Authorize(Roles = "VisitorRole, UserRole, AdminRole")]
    public async Task<string> Test2(CancellationToken cancellationToken)
    {
        try
        {
            Debug.WriteLine("Started Async API Test method");
            await Task.Delay(5000, cancellationToken);
            Debug.WriteLine("Completed Async API Test method");
            return "Test completed";
        }
        catch (Exception e)
        {
            return $"Something went seriously wrong - error detail: {e.Message}";
        }
    }
}
