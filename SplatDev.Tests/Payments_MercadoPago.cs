namespace UmbracoCms.Plugins.Tests
{
    using Xunit;

    using Newtonsoft.Json;

    using SplatDev.Payments.Interfaces;
    using SplatDev.Payments.MercadoPago.Enum;
    using SplatDev.Payments.MercadoPago.Models;
    using SplatDev.Payments.MercadoPago.Requests;

    [Trait("Category", "Integration")]
    public class Plugins_Payments_MercadoPago : IDisposable
    {
        private readonly string PUBLIC_KEY = "TEST-f29badf4-23ce-44f7-8031-1e6f6baac960";
        private readonly string ACCESS_TOKEN = "TEST-4536961032805127-081019-ccc05c8067fff40c2882338f1139caf5-62937701";
        //private readonly long APP_ID = 4536961032805127;

        private readonly string PUBLIC_KEY_SUBSCRIPTIONS = "TEST-447b82af-e39a-458c-bd6b-0d3b195a0e66";
        private readonly string ACCESS_TOKEN_SUBSCRIPTIONS = "TEST-1140570228911716-081320-24bd3828154b8f0ffcea97fac843fc3c-62937701";
        //private readonly long APP_ID_SUBSCRIPTIONS = 1140570228911716;

        //private readonly string PUBLIC_KEY_PROD_SUBSCRIPTIONS = "APP_USR-11a0a73e-f4bf-4f75-b12d-79653551fe92";
        private readonly string ACCESS_TOKEN_PROD_SUBSCRIPTIONS = "APP_USR-1140570228911716-081320-017e61fa373ac934752d0f6adf659286-62937701";

        private readonly string PUBLIC_KEY_TEST_SUBSCRIPTIONS = "APP_USR-3299527426657904-081914-ad39b83ce2ff75db7d782127656d63c1-810330061";
        private readonly string ACCESS_TOKEN_TEST_SUBSCRIPTIONS = "APP_USR-3299527426657904-081914-ad39b83ce2ff75db7d782127656d63c1-810330061";

        private readonly string REFERRER = "https://splatdev.com/";
        private IPayment model;
        private IPaymentMethod creditCard;
        private IPayment<MercadoPago.Resource.Payment.Payment> cardPayment;
        private IPayment<MercadoPago.Resource.Payment.Payment> cardPaymentSubscriptions;
        private PaymentRequests paymentsRequest;
        private SubscriptionRequest subscriptionRequests;
        private Payment paymentInfo;

        public Plugins_Payments_MercadoPago()
        {
            creditCard = new CreditCard
            {
                Cardholder = new CardHolder
                {
                    Name = "Test User",
                },
                CardNumber = "4235647728025682",
                ExpirationMonth = 11,
                ExpirationYear = 2030,
                PaymentMethodId = "visa",
                SecurityCode = "123"
            };

            model = new Payment
            {
                Email = "john.doe@email.com",
                Phone = "15991424586",
                TransactionAmount = 100.00m,
                TransactionDescription = "MercadoPago_InitializeTransaction Test",
                Details = creditCard,
                Installments = 3
            };

            paymentInfo = new Payment
            {
                AdditionalInfo = new AdditionalInfo
                {
                    Payer = new Payer
                    {
                        Address = new Address
                        {
                            StreetName = "Emerenciano Prestes de Barro",
                            StreetNumber = 28,
                            ZipCode = "18021-240",
                            CityName = "Sorocaba",
                            StateName = "Sao Paulo"
                        },
                        FirstName = "Carlos",
                        LastName = "Casalicchio",
                        Phone = new Phone
                        {
                            AreaCode = 15,
                            Number = "991424586"
                        },
                        EntityType = EntityTypes.Individual.StringName(),
                        Type = PayerTypes.Guest.StringName()
                    },
                    Items = new PaymentItem[]
                    {
                        new PaymentItem
                        {
                            CategoryId = "Test",
                            Description = "Test Product",
                            Id = "PROD-01",
                            Quantity = 4,
                            Title = "Test Product Title",
                            UnitPrice = 10.99m
                        }
                    },
                    Barcode = new Barcode
                    {
                        Type = BarcodeTypes.UCCEAN128.StringName(),
                        Content = "",
                        Height = 30,
                        Width = 120
                    },
                    Shipments = new Shipment
                    {
                        ReceiverAddress = new Address
                        {
                            CityName = "Sorocaba",
                            StateName = "Sao Paulo",
                            StreetName = "Emerenciano Prestes de Barro",
                            StreetNumber = 28,
                            ZipCode = "18021-240"
                        }
                    }
                },
                // Description = "Updating Payment Info",
                ExternalReference = "EXT001",
                Installments = 1,
                //Metadata = null,
                //Order = new Order
                //{
                //    Id = 100,
                //    Type = OrderTypes.MercadoPago.StringName()
                //},
                Payer = new Payer
                {
                    EntityType = EntityTypes.Individual.StringName(),
                    Identification = new Identification { Number = "747.340.780-50", Type = IdentificationTypes.CPF.ToString() },
                    Phone = new Phone
                    {
                        AreaCode = 15,
                        Number = "991424586"
                    },
                    Type = PayerTypes.Customer.StringName()
                },
                PaymentMethodId = "master",
                TransactionAmount = 58.8m,
                NotificationUrl = "https://localhost/notify",
                CallbackUrl = "https://localhost/callback"
            };

            cardPayment = new CardPaymentRequest(PUBLIC_KEY, ACCESS_TOKEN, REFERRER);

            cardPaymentSubscriptions = new CardPaymentRequest(PUBLIC_KEY_SUBSCRIPTIONS, ACCESS_TOKEN_SUBSCRIPTIONS, REFERRER);

            paymentsRequest = new PaymentRequests(PUBLIC_KEY, ACCESS_TOKEN);

            subscriptionRequests = new SubscriptionRequest(PUBLIC_KEY_TEST_SUBSCRIPTIONS, ACCESS_TOKEN_TEST_SUBSCRIPTIONS);
        }

        public void Dispose() { }

        [Fact]
        public void MercadoPago_SearchPaymentMethods()
        {
            // Arrange
            var cardNumber = "50314332";

            // Act
            var paymentTypes = paymentsRequest.SearchPaymentMethodAsync(cardNumber).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(paymentTypes);
        }

        [Fact]
        public void MercadoPago_GetAvailableIndentificationTypes()
        {
            // Arrange

            // Act
            var identTypes = paymentsRequest.GetIdentificationTypesAsync().GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(identTypes);
        }

        [Fact]
        public void MercadoPago_GetAvailablePaymentMethods()
        {
            // Arrange

            // Act
            var paymentTypes = paymentsRequest.GetAvailablePaymentMethodsAsync().GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(paymentTypes);
        }

        [Fact]
        public void MercadoPago_GetPayment()
        {
            // Arrange

            // Act
            var result = paymentsRequest.GetPaymentAsync(1239693994).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MercadoPago_GetAllPayments()
        {
            // Arrange

            // Act
            var payments = paymentsRequest.GetPaymentsAsync(sort: "date_created", criteria: "desc").GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(payments);
        }

        [Fact]
        public void MercadoPago_UpdatePayment()
        {
            // Arrange
            var update = new Payment
            {
                Description = "Changed description test"
            };

            // Act
            var updated = paymentsRequest.UpdatePaymentAsync(1239843994, update).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(updated);
        }

        [Fact]
        public void MercadoPago_CreateSubscriptionPlan()
        {
            // Arrange
            var plan = new PreApprovalPlan
            {
                BackUrl = "https://splatdev.com/subscribeto",
                Reason = "Monthly Plan Gold",
                AutoRecurring = new AutoRecurring
                {
                    CurrencyId = "BRL",
                    FreeTrial = new FreeTrial
                    {
                        Frequency = 1,
                        FrequencyType = FrequencyTypes.Months.StringName()
                    },
                    Repetitions = 12,
                    Frequency = 1,
                    FrequencyType = FrequencyTypes.Months.StringName(),
                    TransactionAmount = 29.99m
                },
            };

            // Act
            var response = subscriptionRequests.CreateSubscriptionPlanAsync(plan).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public void MercadoPago_CreateTestUser()
        {
            var userRequest = new UserRequest(ACCESS_TOKEN_PROD_SUBSCRIPTIONS);
            var user = userRequest.CreateTestUser().GetAwaiter().GetResult();

            Assert.NotNull(user);
        }

        [Fact]
        public void MercadoPago_CreateSubscriptionFromPlan()
        {
            // Arrange
            //var userRequest = new UserRequest(ACCESS_TOKEN_PROD_SUBSCRIPTIONS);
            //var user = userRequest.CreateTestUser().GetAwaiter().GetResult();
            var card = (CreditCard)creditCard;
            var cardPaymentRequest = new CardPaymentRequest(PUBLIC_KEY_TEST_SUBSCRIPTIONS, ACCESS_TOKEN_TEST_SUBSCRIPTIONS, REFERRER).GenerateCardTokenAsync(card).GetAwaiter().GetResult();
            var subscription = new Subscription
            {
                PayerEmail = "test_user_16089079@testuser.com",//user.Email,
                PreapprovalPlanId = "2c9380847b5a473c017b5ee4269202a9",
                CardTokenId = cardPaymentRequest.Id,
                //ApplicationId = APP_ID_SUBSCRIPTIONS
            };

            // Act
            var response = subscriptionRequests.CreateSubscriptionAsync(subscription).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public void MercadoPago_CreateSubscriptionWithoutPlan()
        {
            // Arrange
            var subscription = new Subscription
            {
                AutoRecurring = new AutoRecurring
                {
                    CurrencyId = "BRL",
                    EndDate = new System.DateTime(2021, 12, 31),
                    StartDate = System.DateTime.Now,
                    Frequency = 1,
                    FrequencyType = FrequencyTypes.Months.StringName(),
                    TransactionAmount = 29.99m
                },
                BackUrl = "https://splatdev.com/subscribeto",
                ExternalReference = "2354624623T6X4",
                PayerEmail = "john.doe@uol.com.br",
                Reason = "Subscribing to Gold Service",
                ApplicationId = 1140570228911716
            };

            // Act
            var response = subscriptionRequests.CreateSubscriptionAsync(subscription).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public void MercadoPago_GenerateCardToken()
        {
            // Arrange
            CreditCard details = (CreditCard)((Payment)model).Details;
            details.Installments = 3;

            // Act
            var result = ((CardPaymentRequest)cardPayment).GenerateCardTokenAsync(details).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MercadoPago_CreateCardPaymentRequest()
        {
            // Arrange
            cardPayment = new CardPaymentRequest(PUBLIC_KEY, ACCESS_TOKEN, REFERRER);
            var paymentModel = (Payment)model;
            var card = (CreditCard)paymentModel.Details;
            card.Installments = 3;

            // Act
            var result = cardPayment.CreatePaymentRequestAsync(paymentModel).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MercadoPago_CreatePixPaymentRequest()
        {
            // Arrange
            var gateway = new PixPaymentRequest(PUBLIC_KEY, ACCESS_TOKEN);
            var pix = new Pix
            {
                Description = "Pix Product Item #1",
                ExpirationDate = System.DateTime.Now.AddDays(5),
                Payer = new Payer
                {
                    Email = "john.doe@email.com.br",
                    FirstName = "John",
                    LastName = "Doe",
                    Identification = new Identification
                    {
                        Type = IdentificationTypes.CPF.ToString(),
                        Number = "747.340.780-50"
                    }
                },
                TransactionAmount = 99.99m
            };

            // Act
            var payment = gateway.CreatePaymentRequestAsync(pix).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(payment.PointOfInteraction.TransactionData.QrCode);
        }

        [Fact]
        public void MercadoPago_CreateTicketPaymentRequest()
        {
            // Arrange
            var gateway = new TicketPaymentRequest(PUBLIC_KEY, ACCESS_TOKEN);
            var boleto = new Ticket
            {
                Description = "Boleto Product Item #1",
                ExpirationDate = System.DateTime.Now.AddDays(5),
                Payer = new Payer
                {
                    Email = "john.doe@email.com.br",
                    FirstName = "John",
                    LastName = "Doe",
                    Identification = new Identification
                    {
                        Type = IdentificationTypes.CPF.ToString(),
                        Number = "747.340.780-50"
                    }
                },
                TransactionAmount = 99.99m
            };

            // Act
            var payment = gateway.CreatePaymentRequestAsync(boleto).GetAwaiter().GetResult();
            var apiResponse = JsonConvert.DeserializeObject<dynamic>(payment.ApiResponse.Content);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var barcode = apiResponse.barcode.content.Value;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            // Assert
            Assert.NotNull(payment.TransactionDetails.ExternalResourceUrl);
        }
    }
}
