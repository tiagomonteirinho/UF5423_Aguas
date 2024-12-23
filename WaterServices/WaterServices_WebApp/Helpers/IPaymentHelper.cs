using Braintree;

namespace WaterServices_WebApp.Helpers
{
    public interface IPaymentHelper
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();
    }
}
