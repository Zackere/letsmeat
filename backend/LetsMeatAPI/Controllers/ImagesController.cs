using Azure.Storage.Blobs.Models;
using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class ImagesController : ControllerBase {
    public ImagesController(
      IUserManager userManager,
      LMDbContext context,
      IBlobClientFactory blobClientFactory,
      IPaidResourceGuard paidResourceGuard,
      ILogger<ImagesController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _blobClientFactory = blobClientFactory;
      _paidResourceGuard = paidResourceGuard;
      _logger = logger;
    }
    public class ImageInformationResponse {
      public class DebtFromImageInformation {
        public Guid id { get; set; }
        public uint amount { get; set; }
        [MaxLength(128)]
        public string description { get; set; }
      }
      public Guid image_id { get; set; }
      public Guid? event_id { get; set; }
      public Guid group_id { get; set; }
      public string image_url { get; set; }
      public string uploaded_by { get; set; }
      public DateTime uploaded_time { get; set; }
      public IEnumerable<DebtFromImageInformation> debts_from_image { get; set; }
    }
    public class ImageInformationBody {
      public Guid[] image_ids { get; set; }
    }
    [HttpPost]
    [Route("info")]
    public async Task<ActionResult<IEnumerable<ImageInformationResponse>>> Info(
      string token,
      [FromBody] ImageInformationBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(body.image_ids.Any(i => !_context.Images.Any(ii => ii.Id == i)))
        return NotFound();
      return await (from image in _context.Images.Include(i => i.DebtsFromImage)
                    where body.image_ids.Contains(image.Id)
                    select new ImageInformationResponse {
                      debts_from_image = from debt in image.DebtsFromImage
                                         select new ImageInformationResponse.DebtFromImageInformation {
                                           amount = debt.Amount,
                                           description = debt.Description,
                                           id = debt.Id,
                                         },
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
    public async Task<ActionResult<ImageInformationResponse>> Upload(
      string token,
      Guid event_id,
      IFormFile file
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      var canAccess = _paidResourceGuard.CanAccessPaidResource(user);
      if(file.Length > MaxFilesize)
        return new StatusCodeResult(418);
      var ev = await _context.Events.FindAsync(event_id);
      if(ev == null)
        return NotFound();
      var extension = Path.GetExtension(file.FileName);
      if(!(extension == ".png" || extension == ".jpeg" || extension == ".jpg"))
        return new StatusCodeResult(418);
      var filename = $"{DateTime.UtcNow:yyyyddmmssffff}{new Random().Next(0, 1000)}{extension}";
      var client = _blobClientFactory.GetImageClient(filename);
      using var stream = file.OpenReadStream();
      if(!await canAccess)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      await client.UploadAsync(stream);
      var debts = new List<DebtFromImage>(3);
      var image = new Image {
        DebtsFromImage = debts,
        EventId = event_id,
        GroupId = ev.GroupId,
        UploadedById = userId,
        UploadTime = DateTime.UtcNow,
        Url = client.Uri.ToString(),
      };
      // TODO(wreplin): Retrieve this data from Vision API
      var rnd = new Random();
      for(var i = 0; i < 3; ++i) {
        debts.Add(new() {
          Amount = (uint)rnd.Next(100, 1000),
          Description = $"Some very meaningful description that needs to be verified :) {i}",
          Image = image,
        });
      }
      await _context.DebtsFromImages.AddRangeAsync(debts);
      await _context.Images.AddAsync(image);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new ImageInformationResponse {
        debts_from_image = from debt in debts
                           select new ImageInformationResponse.DebtFromImageInformation {
                             amount = debt.Amount,
                             description = debt.Description,
                             id = debt.Id,
                           },
        event_id = event_id,
        group_id = image.GroupId,
        image_id = image.Id,
        image_url = image.Url,
        uploaded_by = image.UploadedById,
        uploaded_time = DateTime.SpecifyKind(image.UploadTime, DateTimeKind.Utc),
      };
    }
    public class ImageDeleteBody {
      public Guid id { get; set; }
    }
    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> Delete(
      string token,
      [FromBody] ImageDeleteBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var image = await _context.Images.FindAsync(body.id);
      if(image == null)
        return NotFound();
      _context.Remove(image);
      await _context.SaveChangesAsync();
      var client = _blobClientFactory.GetClientFromUri(new Uri(image.Url));
      try {
        await client.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
      } catch(Azure.RequestFailedException ex) {
        _logger.LogError(ex.ToString());
      }
      return Ok();
    }
    public class DeleteImageDebtBody {
      public Guid id { get; set; }
    }
    [HttpDelete]
    [Route("delete_image_debt")]
    public async Task<ActionResult> DeleteImageDebt(
      string token,
      [FromBody] DeleteImageDebtBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var debt = await _context.DebtsFromImages.FindAsync(body.id);
      if(debt == null)
        return NotFound();
      _context.DebtsFromImages.Remove(debt);
      await _context.SaveChangesAsync();
      return Ok();
    }
    [HttpPatch]
    [Route("update_image_debt")]
    public async Task<ActionResult> UpdateImageDebt(
      string token,
      [FromBody] ImageInformationResponse.DebtFromImageInformation body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var debt = await _context.DebtsFromImages.FindAsync(body.id);
      if(debt == null)
        return NotFound();
      debt.Amount = body.amount;
      debt.Description = body.description;
      _context.Entry(debt).State = EntityState.Modified;
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateConcurrencyException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    public class CreateImageDebtBody {
      public uint amount { get; set; }
      [MaxLength(128)]
      public string description { get; set; }
      public Guid image_id { get; set; }
    }
    [HttpPost]
    [Route("create_image_debt")]
    public async Task<ActionResult> CreateImageDebt(
      string token,
      [FromBody] CreateImageDebtBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      if(!await _context.Images.AnyAsync(i => i.Id == body.image_id))
        return NotFound();
      var debt = new DebtFromImage {
        Amount = body.amount,
        Description = body.description,
        ImageId = body.image_id,
      };
      await _context.DebtsFromImages.AddAsync(debt);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return Ok();
    }
    public const int MaxFilesize = (int)10e+6; // 10Mb
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly IBlobClientFactory _blobClientFactory;
    private readonly IPaidResourceGuard _paidResourceGuard;
    private readonly ILogger<ImagesController> _logger;
  }
}
