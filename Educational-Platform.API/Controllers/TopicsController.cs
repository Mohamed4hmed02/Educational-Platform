using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Abstractions.TopicInterfaces;
using Educational_Platform.Application.Enums;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.API.Controllers
{
	[Route("api/v1/[controller]/")]
	[ApiController]
	public class TopicsController(
		ITopicCommandServices topicCommandServices,
		ITopicQueryServices topicQueryServices,
		IVideoHostServices hostServices,
		IUnitOfWork unitOfWork) : ControllerBase
	{
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async ValueTask AddTopic(CommandTopicModel topic)
		{
			await topicCommandServices.CreateAsync(topic);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut]
		public async ValueTask UpdateTopic(CommandTopicModel topic)
		{
			await topicCommandServices.UpdateAsync(topic);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{topicId}")]
		public async ValueTask DeleteTopic(int topicId)
		{
			await topicCommandServices.DeleteAsync(topicId);
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{topicId}/Reference")]
		public async ValueTask AddReference(IFormFile reference, int topicId)
		{
			await topicCommandServices.UploadPdfAsync(reference, topicId);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{topicId}/Reference")]
		public async ValueTask DeleteReference(int topicId)
		{
			await topicCommandServices.DeletePdfAsync(topicId);
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{topicId}")]
		public async ValueTask SetVideoPrivacy(int topicId)
		{
			await hostServices.SetVideoPrivacyAsync(topicId);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("{topicId}/Domain")]
		public async ValueTask AllowDomain(string domainName, int topicId)
		{
			await hostServices.AllowVideoDomainAsync(domainName, topicId);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{topicId}/Domain")]
		public async ValueTask DisallowDomain(string domainName, int topicId)
		{
			await hostServices.DisallowVideoDomainAsync(domainName, topicId);
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{topicId}/Thumbnail")]
		public async ValueTask SetVideoThumbnail(int topicId, IFormFile image)
		{
			await hostServices.SetVideoThumbnailAsync(topicId, image);
		}

		[AllowAnonymous]
		[HttpGet("{topicId}/Intro")]
		public async ValueTask<IActionResult> GetIntroVideoEmbed(int topicId)
		{
			// Check Is Topic Intro
			if (!await unitOfWork.CoursesRepository.IsExistAsync(c => c.IntroTopicId == topicId))
				return BadRequest("This Topic Isn't Intro");

			return Ok(await hostServices.GetVideoUrlPlayerAsync(topicId));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("{topicId}/Domain")]
		public async ValueTask<IActionResult> GetAllowedDomains(int topicId)
		{
			return Ok(await hostServices.GetVideoAllowedDomainsAsync(topicId));
		}

		[Authorize]
		[HttpGet("{unitId}/Topics")]
		public async ValueTask<IActionResult> GetUnitTopics(int unitId)
		{
			return Ok(await topicQueryServices.GetTopicsAsync(unitId, ModelTypeEnum.Query));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("{unitId}/CommandModels")]
		public async ValueTask<IActionResult> GetUnitTopicsCommandModel(int unitId)
		{
			return Ok(await topicQueryServices.GetTopicsAsync(unitId, ModelTypeEnum.Command));
		}

		[Authorize(Policy = "TopicAccess")]
		[HttpGet("{topicId}")]
		public async ValueTask<IActionResult> GetVideoUrlPlayer(int topicId)
		{
			return Ok(await hostServices.GetVideoUrlPlayerAsync(topicId));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("UploadTicket")]
		public async ValueTask<IActionResult> GetVideoUploadTicket(long videoSize)
		{
			return Ok(await hostServices.GetVideoUploadTicketAsync(videoSize));
		}
	}
}
