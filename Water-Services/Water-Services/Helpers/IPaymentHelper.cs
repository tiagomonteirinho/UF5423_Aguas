using Braintree;

namespace Water_Services.Helpers
{
    public interface IPaymentHelper
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();
    }
}
