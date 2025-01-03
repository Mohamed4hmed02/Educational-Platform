using Educational_Platform.Application.Models.CommonModels;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Domain.Abstractions.InfrastructureAbstractions
{
    public interface IVideoHostServices : IDeleteable
    {
        /// <summary>
        /// Get The Token That Contains Info To Upload A Topic Using TUS Protocol
        /// </summary>
        /// <param name="videoSize"></param>
        /// <returns></returns>
        Task<UploadVideoTicketModel> GetVideoUploadTicketAsync(long videoSize);
        /// <summary>
        /// Updating Topic Title In Topic Host Platform
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        Task UpdateVideoTitleAsync(object videoId, string title);
        /// <summary>
        /// Allowing A Specfic Domain Name To Have The Access To Play Embed Topic
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        Task AllowVideoDomainAsync(string domainName, object videoId);
        /// <summary>
        /// Disable Allowing A Specfic Domain Name To Have The Access To Play Embed Topic
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        Task DisallowVideoDomainAsync(string domainName, object videoId);
        Task<IEnumerable<string>> GetVideoAllowedDomainsAsync(object videoId);
        /// <summary>
        /// Make No One Can Access The Topic Instead Of Allowed Domains
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        Task SetVideoPrivacyAsync(object videoId);
        /// <summary>
        /// Get The HTML Embed Code For A Specific Topic
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        Task<string> GetVideoUrlPlayerAsync(object videoId);
        Task SetVideoThumbnailAsync(int videoId, IFormFile photo);
        /// <summary>
        /// Delete A Topic
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Throw OuterServiceException : If An Error Occured In The Host Platform</returns>
        ValueTask DeleteAsync(object Id);
    }
}
