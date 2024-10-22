using Braintree;
using Microsoft.Extensions.Configuration;

namespace UF5423_Aguas.Helpers
{
    public class PaymentHelper : IPaymentHelper
    {
        private readonly IConfiguration _config;

        public PaymentHelper(IConfiguration config)
        {
            _config = config;
        }

        public IBraintreeGateway CreateGateway()
        {
            var newGateway = new BraintreeGateway()
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = _config.GetValue<string>("BraintreeGateway:MerchantId"),
                PublicKey = _config.GetValue<string>("BraintreeGateway:PublicKey"),
                PrivateKey = _config.GetValue<string>("BraintreeGateway:PrivateKey")
            };

            return newGateway;
        }

        public IBraintreeGateway GetGateway()
        {
            return CreateGateway();
        }
    }
}
