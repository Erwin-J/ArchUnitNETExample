using Domain;

namespace Infrastructure
{
    // Imagen that the Client's interface was correctly implemented, but that the implementing class has a different name.
    public class ExternalPartySender : IExternalClient
    {
        public void SendToExternalPartner()
        {
            // Comment out to fail the associated test.
            //var domainClass = new DomainClass();
        }
    }
}
