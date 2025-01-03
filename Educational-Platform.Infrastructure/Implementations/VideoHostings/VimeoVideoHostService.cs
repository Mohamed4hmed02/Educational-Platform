using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Models.CommonModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Educational_Platform.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VimeoDotNet;
using VimeoDotNet.Net;
namespace Educational_Platform.Infrastructure.Implementations.VideoHostings
{
    public partial class VimeoVideoHostService : IVideoHostServices
    {
        private readonly VimeoClient _vimeoClient;
        private readonly string _vimeoErrorMessage = "Error With Vimeo";
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<VimeoVideoHostService> log;
        private readonly string[]? _allowedDomains;

        public VimeoVideoHostService(
            IOptions<VimeoOptions> options,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<VimeoVideoHostService> log)
        {
            _vimeoClient = new(options.Value.VimeoAccessToken);
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.VimeoAccessToken);
            this.unitOfWork = unitOfWork;
            this.log = log;
            _allowedDomains = configuration.GetRequiredSection("APISecurity:AllowedDomains").Get<string[]>();
        }
        public async Task<UploadVideoTicketModel> GetVideoUploadTicketAsync(long videoSize)
        {
            var ticket = await _vimeoClient.GetUploadTicketAsync(videoSize);
            var Id = Convert.ToInt64(ticket.Id);
            await SetPrivacy(Id, _allowedDomains);

            log.LogInformation("A New Ticket {@Ticket} Is Created For Uploading New Topic", ticket);
            return new UploadVideoTicketModel
            {
                UploadLink = ticket.Upload.UploadLink,
                OnHostId = ticket.Id,
                VideoSize = ticket.Upload.Size
            };
        }

        public async Task AllowVideoDomainAsync(string domainName, object videoId)
        {
            var onHostId = await GetVideoOnHostIdAsync(videoId);
            try
            {
                await _vimeoClient.AllowEmbedVideoOnDomainAsync(onHostId, domainName);
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
            log.LogInformation("A New AllowedDomain {Domain} Was Added For Topic {@Topic}", domainName, onHostId);
        }

        public async Task<string> GetVideoUrlPlayerAsync(object videoId)
        {
            var onHostId = await GetVideoOnHostIdAsync(videoId);
            try
            {
                var videoOnHost = await _vimeoClient.GetVideoAsync(onHostId);
                return videoOnHost.Player_Embed_Url;
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
        }

        public async Task SetVideoThumbnailAsync(int videoId, IFormFile photo)
        {
            var onHostId = await GetVideoOnHostIdAsync(videoId);
            IBinaryContent content = new BinaryContent(photo.OpenReadStream(), photo.ContentType);
            try
            {
                await _vimeoClient.UploadThumbnailAsync(onHostId, content);
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
            log.LogInformation("A Thumbnail Is Set To Topic {@Topic}", onHostId);
        }

        public async Task UpdateVideoTitleAsync(object videoId, string title)
        {
            var onHostId = await GetVideoOnHostIdAsync(videoId);
            try
            {
                var updateRequest = new
                {
                    name = title
                };

                var response = await _httpClient.PatchAsync(
                    $"{BaseUrl}/videos/{onHostId}",
                    new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
            log.LogInformation("A Name Of Topic {@Topic} Was Updated On VimeoVideoHosting", onHostId);
        }

        public async ValueTask DeleteAsync(object Id)
        {
            var onHostId = await GetVideoOnHostIdAsync(Id);
            try
            {
                await _vimeoClient.DeleteVideoAsync(onHostId);
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
            log.LogInformation("A Topic {Topic} Was Deleted From VimeoVideoHosting", onHostId);
        }

        public async Task DisallowVideoDomainAsync(string domainName, object videoId)
        {
            var onHostId = await GetVideoOnHostIdAsync(videoId);
            try
            {
                await _vimeoClient.DisallowEmbedVideoOnDomainAsync(onHostId, domainName);
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
        }

        public async Task<IEnumerable<string>> GetVideoAllowedDomainsAsync(object videoId)
        {
            var onHostId = await GetVideoOnHostIdAsync(videoId);
            try
            {
                var res = await _vimeoClient.GetAllowedDomainsForEmbeddingVideoAsync(onHostId);
                var domains = res.Data;
                return domains.Select(d => d.Domain);
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
        }

        public async Task SetVideoPrivacyAsync(object videoId)
        {
            var onHostId = await GetVideoOnHostIdAsync(videoId);
            await SetPrivacy(onHostId, _allowedDomains);
        }
    }
    public partial class VimeoVideoHostService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.vimeo.com";

        private async Task SetPrivacy(long videoId, string[] allowedDomains)
        {
            try
            {
                var privacySettings = new
                {
                    privacy = new
                    {
                        add = false,
                        view = "disable",
                        embed = "whitelist",
                        comments = "nobody",
                        download = false
                    },
                    embed = new
                    {
                        buttons = new
                        {
                            like = false,
                            watchlater = false,
                            share = false
                        },
                        logos = new
                        {
                            vimeo = false
                        },
                        title = new
                        {
                            name = "hide",
                            owner = "hide",
                            portrait = "hide"
                        }
                    },
                    embed_domains = allowedDomains,
                    hide_from_vimeo = true
                };
                var response = await _httpClient.PatchAsync(
                    $"{BaseUrl}/videos/{videoId}",
                    new StringContent(JsonSerializer.Serialize(privacySettings), Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                log.LogError("Viemo Error :{@Message}", ex.Message);
                throw new OuterServiceException(_vimeoErrorMessage)
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status503ServiceUnavailable);
            }
        }

        private async Task<long> GetVideoOnHostIdAsync(object videoId)
        {
            long Id = 0;
            try
            {
                Id = Convert.ToInt64((await unitOfWork.TopicsRepository.ReadAsync(videoId)).OnHostVideoId);
            }
            catch
            {

            }

            if (Id == 0)
                throw new NotExistException("There Is No OnHost Topic For The Given Topic");

            return Id;
        }
    }
}
