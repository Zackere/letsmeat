using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class ImagesController : ControllerBase {
    public ImagesController(
      UserManager userManager,
      LMDbContext context,
      BlobClientFactory blobClientFactory,
      ILogger<ImagesController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _blobClientFactory = blobClientFactory;
      _logger = logger;
    }
    public class ImageInfoResponse {
      public Guid image_id { get; set; }
      public Guid? event_id { get; set; }
      public Guid group_id { get; set; }
      public string image_url { get; set; }
      public string uploaded_by { get; set; }
      public DateTime uploaded_time { get; set; }
    }
    public class ImageInformationBody {
      public Guid[] image_ids { get; set; }
    }
    [HttpPost]
    [Route("info")]
    public async Task<ActionResult<IEnumerable<ImageInfoResponse>>> Info(
      string token,
      [FromBody] ImageInformationBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(body.image_ids.Any(i => !_context.Images.Any(ii => ii.Id == i)))
        return NotFound();
      return await (from image in _context.Images
                    where body.image_ids.Contains(image.Id)
                    select new ImageInfoResponse() {
                      event_id = image.EventId,
                      group_id = image.GroupId,
                      image_id = image.Id,
                      image_url = image.Url,
                      uploaded_by = image.UploadedById,
                      uploaded_time = DateTime.SpecifyKind(image.UploadTime, DateTimeKind.Utc),
                    }).ToListAsync();
    }
    [HttpPost]
    [Route("upload")]
    public async Task<ActionResult<ImageInfoResponse>> Upload(
      string token,
      Guid event_id,
      IFormFile file
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(token.StartsWith(LoginController.FakeTokenPrefix))
        return Forbid();
      var ev = await _context.Events.FindAsync(event_id);
      if(ev == null)
        return NotFound();
      var extension = Path.GetExtension(file.FileName);
      if(!(extension == ".png" || extension == ".jpeg" || extension == ".jpg"))
        return new StatusCodeResult(418);
      var filename = $"{DateTime.UtcNow:yyyyddmmssffff}{new Random().Next(0, 1000)}{extension}";
      var client = _blobClientFactory.GetImageClient(filename);
      using var stream = file.OpenReadStream();
      await client.UploadAsync(stream);
      var image = new Image() {
        EventId = event_id,
        GroupId = ev.GroupId,
        UploadedById = userId,
        UploadTime = DateTime.UtcNow,
        Url = client.Uri.ToString(),
      };
      await _context.Images.AddAsync(image);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new ImageInfoResponse() {
        event_id = event_id,
        group_id = image.GroupId,
        image_id = image.Id,
        image_url = image.Url,
        uploaded_by = image.UploadedById,
        uploaded_time = DateTime.SpecifyKind(image.UploadTime, DateTimeKind.Utc),
      };
    }
    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly BlobClientFactory _blobClientFactory;
    private readonly ILogger<ImagesController> _logger;
  }
}
