using Braintree;

namespace UF5423_Aguas.Helpers
{
    public interface IPaymentHelper
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();
    }
}
