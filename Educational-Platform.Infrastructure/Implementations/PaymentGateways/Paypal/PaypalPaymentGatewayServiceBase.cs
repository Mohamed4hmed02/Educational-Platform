using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal.Data;
using Educational_Platform.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Educational_Platform.Infrastructure.Implementations.PaymentGateways.Paypal
{
	public sealed class PaypalPaymentGatewayServiceBase
	{
		private readonly string ClientId;
		private readonly string ClientSecret;
		private readonly HttpClient _httpClient;
		private static readonly JsonSerializerOptions _serializerOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
		};
		private readonly ILogger<PaypalPaymentGatewayServiceBase> log;

		public PaypalPaymentGatewayServiceBase(
			IOptionsMonitor<PaypalOptions> options,
			ILogger<PaypalPaymentGatewayServiceBase> logger)
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri(options.CurrentValue.Url)
			};
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			ClientId = options.CurrentValue.ClientId;
			ClientSecret = options.CurrentValue.ClientSecret;
			log = logger;
		}

		/// <summary>
		/// Throws Outerserviceexception if an error occured
		/// </summary>
		/// <param name="orderRequest"></param>
		/// <param name="referenceId"></param>
		/// <returns></returns>
		public async Task<Order> CreateOrderAsync(OrderRequest orderRequest, string referenceId)
		{

			string base64Credentials = Base64Credentials();
			var content = new StringContent(Serialize(orderRequest), null, "application/json");

			var request = new HttpRequestMessage(HttpMethod.Post, "v2/checkout/orders")
			{
				Content = content
			};
			request.Headers.Add("PayPal-Request-Id", referenceId);
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
			var response = await _httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				log.LogError("Error occured during retrieve create an order {errorMessage}", response.Content.ReadAsStringAsync().Result);
				throw new OuterServiceException()
					.AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
			}

			log.LogInformation("New Payment Order Is Created Or Modified {PaymentOrder}", response.Content.ReadAsStringAsync().Result);

			return Deserialize<Order>(response);
		}

		public async Task<Order> GetOrderDetailsAsync(string orderId)
		{
			string base64Credentials = Base64Credentials();
			var request = new HttpRequestMessage(HttpMethod.Get, $"v2/checkout/orders/{orderId}");
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
			var response = await _httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				log.LogError("Error occured during retrieve an order {order}, {errorMessage}", orderId, response.Content.ReadAsStringAsync().Result);
			}

			return Deserialize<Order>(response);
		}

		public async Task<bool> CapiturePaymentAsync(string orderId)
		{
			string base64Credentials = Base64Credentials();
			var content = new StringContent("", null, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Post, $"v2/checkout/orders/{orderId}/capture")
			{
				Content = content
			};
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
			var response = await _httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				log.LogError("Error occured during capiture a payment wiht order{order}, {errorMessage}", orderId, response.Content.ReadAsStringAsync().Result);
				return false;
			}

			log.LogInformation("Payment Order Has Been Capitured {PaymentOrder}", response.Content.ReadAsStringAsync().Result);
			return true;
		}

		public UpdateOrderBuilder GetUpdateOrderBuilder(string orderId, string referenceId)
		{
			string base64Credentials = Base64Credentials();

			var request = new HttpRequestMessage(HttpMethod.Patch, $"v2/checkout/orders/{orderId}");
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
			return new UpdateOrderBuilder(_httpClient, request, log, orderId, referenceId);
		}

		public async Task<AuthenticationResponse> GetAccessToken()
		{
			string base64Credentials = Base64Credentials();

			var content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");
			var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token")
			{
				Content = content
			};
			request.Headers.Add("Authorization", $"Basic {base64Credentials}");

			var response = await _httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				log.LogError("Error occured during retrieve access token {errorMessage}", response.Content.ReadAsStringAsync().Result);
			}

			return Deserialize<AuthenticationResponse>(response);
		}

		private string Base64Credentials()
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret));
		}

		public static string Serialize<T>(T obj)
		{
			return JsonSerializer.Serialize(obj, _serializerOptions);
		}

		public static T Deserialize<T>(HttpResponseMessage responseMessage)
		{
			return JsonSerializer.Deserialize<T>(responseMessage.Content.ReadAsStringAsync().Result, _serializerOptions);
		}
	}

	public class UpdateOrderBuilder(
		HttpClient httpClient,
		HttpRequestMessage httpRequest,
		ILogger<PaypalPaymentGatewayServiceBase> log,
		string orderId,
		string referenceId)
	{
		private readonly IList<object> contents = [];

		public UpdateOrderBuilder ReplaceAmounts(IEnumerable<Amount> amounts)
		{
			var content = new
			{
				op = "replace",
				path = $"/purchase_units/@reference_id==qout{referenceId}qout/amount",
				value = amounts
			};

			contents.Add(content);
			return this;
		}

		public async Task SendAsync()
		{
			httpRequest.Content = new StringContent(PaypalPaymentGatewayServiceBase.Serialize(contents).Replace("qout","'"), null, "application/json");
			var response = await httpClient.SendAsync(httpRequest);

			if (!response.IsSuccessStatusCode)
			{
				log.LogError("Error occured during updating an order {orderId}, {errorMessage}", orderId, response.Content.ReadAsStringAsync().Result);
			}
		}
	}

}
