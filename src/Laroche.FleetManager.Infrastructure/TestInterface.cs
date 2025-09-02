using Laroche.FleetManager.Application.Interfaces;

namespace Laroche.FleetManager.Infrastructure.Test;

public class TestInterface
{
    public void TestMethod()
    {
        // Test simple pour voir si les interfaces sont accessibles
        IAuthenticationService? auth = null;
        ILoginAuditService? audit = null;
        
        Console.WriteLine($"Auth: {auth}, Audit: {audit}");
    }
}
