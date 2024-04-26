using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PerfectBreakfast.Application.Models.MomoModels.Response;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.Application.Models.MomoModels.Request;

public class MomoOneTimePaymentRequest
{
    public string partnerCode { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public long amount { get; set; }
    public string orderId { get; set; } = string.Empty;
    public string orderInfo { get; set; } = string.Empty;
    public string redirectUrl { get; set; } = string.Empty;
    public string ipnUrl { get; set; } = string.Empty;
    public string requestType { get; set; } = string.Empty;
    public string extraData { get; set; } = string.Empty;
    public string lang { get; set; } = string.Empty;
    public string partnerName { get; set; } = string.Empty;
    public string signature { get; set; } = string.Empty;

    public MomoOneTimePaymentRequest(string partnerCode, string requestId,
        long amount, string orderId, string orderInfo, string redirectUrl,
        string ipnUrl, string requestType, string extraData, string lang = "vi", string partnerName = "Perfect Breakfast")
    {
        this.partnerCode = partnerCode;
        this.requestId = requestId;
        this.amount = amount;
        this.orderId = orderId;
        this.orderInfo = orderInfo;
        this.redirectUrl = redirectUrl;
        this.ipnUrl = ipnUrl;
        this.requestType = requestType;
        this.extraData = extraData;
        this.lang = lang;
        this.partnerName = partnerName;
    }
    
    public void MakeSignature(string accessKey, string secretKey)
    {
        var rawHash = "accessKey=" + accessKey +
                      "&amount=" + this.amount +
                      "&extraData=" + this.extraData +
                      "&ipnUrl=" + this.ipnUrl +
                      "&orderId=" + this.orderId +
                      "&orderInfo=" + this.orderInfo +
                      "&partnerCode=" + this.partnerCode +
                      "&redirectUrl=" + this.redirectUrl +
                      "&requestId=" + this.requestId +
                      "&requestType=" + this.requestType;
        this.signature = HashHelper.HmacSHA256(rawHash, secretKey);
    }
    
    public async Task<PaymentResponse> GetLink(string paymentUrl)
    {
        using var client = new HttpClient();
        var requestData = JsonConvert.SerializeObject(this, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        });
        var requestContent = new StringContent(requestData, Encoding.UTF8,"application/json");

        var createPaymentLinkRes = client.PostAsync(paymentUrl, requestContent).Result;

        if(createPaymentLinkRes.IsSuccessStatusCode)
        {
            var responseContent = createPaymentLinkRes.Content.ReadAsStringAsync().Result;
            var responseData = JsonConvert
                .DeserializeObject<MomoOneTimePaymentCreateLinkResponse>(responseContent);
            if(responseData.resultCode == 0)
            {
                return new PaymentResponse
                {
                    IsSuccess = true,
                    DeepLink = responseData.deeplink,
                    PaymentUrl = responseData.payUrl,
                    QrCode = responseData.qrCodeUrl
                };
            }
            else
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    DeepLink = null,
                    PaymentUrl = null,
                    QrCode = null
                };
            }
        }
        else
        {
            return new PaymentResponse
            {
                IsSuccess = false,
                DeepLink = null,
                PaymentUrl = null,
                QrCode = null
            };
        }
    }
}