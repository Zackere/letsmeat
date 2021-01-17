using Azure.Storage.Blobs.Models;
using LetsMeatAPI.Models;
using LetsMeatAPI.RecieptExtractor;
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
      IUriRecieptExtractor recieptExtractor,
      Random rnd,
      ILogger<ImagesController> logger
    ) {
      _userManager = userManager;
      _context = context;
      _blobClientFactory = blobClientFactory;
      _paidResourceGuard = paidResourceGuard;
      _recieptExtractor = recieptExtractor;
      _rnd = rnd;
      _logger = logger;
    }
    public class ImageInformationResponse {
      public class DebtFromImageInformation {
        public class PendingDebtInformation {
          public Guid? id { get; set; }
          public Guid group_id { get; set; }
          public Guid? event_id { get; set; }
          public string from_id { get; set; }
          public string to_id { get; set; }
          public uint amount { get; set; }
          public string description { get; set; }
          public Guid? image_id { get; set; }
          public DateTime timestamp { get; set; }
          public DateTime? approved_on { get; set; }
          public Guid? image_debt_id { get; set; }
        }
        public Guid id { get; set; }
        public uint amount { get; set; }
        [MaxLength(128)]
        public string description { get; set; }
        public bool satisfied { get; set; }
        public PendingDebtInformation? pending_debt { get; set; }
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
      if(userId is null)
        return Unauthorized();
      return await (from image in _context.Images.Include(i => i.DebtsFromImage)
                    where body.image_ids.Contains(image.Id)
                    select new ImageInformationResponse {
                      debts_from_image = from d in image.DebtsFromImage
                                         select new ImageInformationResponse.DebtFromImageInformation {
                                           amount = d.Amount,
                                           description = d.Description,
                                           id = d.Id,
                                           satisfied = d.Satisfied,
                                           pending_debt = d.Bound == null
                                           ? (from h in _context.DebtHistory
                                              where h.ImageDebtId == d.Id
                                              select new ImageInformationResponse.DebtFromImageInformation.PendingDebtInformation {
                                                amount = h.Amount,
                                                approved_on = h.HistoryEntryCreatedOn,
                                                description = h.Description,
                                                event_id = h.EventId,
                                                from_id = h.FromId,
                                                group_id = h.GroupId,
                                                image_debt_id = h.ImageDebtId,
                                                image_id = h.ImageId,
                                                timestamp = h.Timestamp,
                                                to_id = h.ToId,
                                              }).SingleOrDefault()
                                            : new ImageInformationResponse.DebtFromImageInformation.PendingDebtInformation {
                                              amount = d.Bound.PendingDebt.Amount,
                                              description = d.Bound.PendingDebt.Description,
                                              event_id = d.Bound.PendingDebt.EventId,
                                              from_id = d.Bound.PendingDebt.FromId,
                                              group_id = d.Bound.PendingDebt.GroupId,
                                              id = d.Bound.PendingDebt.Id,
                                              image_debt_id = d.Id,
                                              image_id = d.Bound.PendingDebt.ImageId,
                                              timestamp = d.Bound.PendingDebt.Timestamp,
                                              to_id = d.Bound.PendingDebt.ToId,
                                            }
                                         },
                      event_id = image.EventId,
                      group_id = image.GroupId,
                      image_id = image.Id,
                      image_url = image.Url,
                      uploaded_by = image.UploadedById,
                      uploaded_time = DateTime.SpecifyKind(image.UploadTime, DateTimeKind.Utc),
                    }).ToListAsync();
    }
#if DEBUG
    [HttpPost]
    [Route("mock_upload")]
    public async Task<ActionResult<Guid>> MockUpload(
      string token,
      Guid event_id,
      string name
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId == null)
        return Unauthorized();
      var ev = await _context.Events.FindAsync(event_id);
      var img = new Image {
        DebtsFromImage = new List<DebtFromImage>(3),
        EventId = ev.Id,
        GroupId = ev.GroupId,
        UploadedById = userId,
        UploadTime = DateTime.UtcNow,
        Url = $"https://example.com/lol/{name}",
      };
      for(var i = 0; i < 3; ++i) {
        img.DebtsFromImage.Add(new() {
          Amount = (uint)_rnd.Next(100, 1000),
          Description = $"Some very meaningful description that needs to be verified :) {i}",
          Image = img,
          Satisfied = false,
        });
      }
      await _context.Images.AddAsync(img);
      await _context.SaveChangesAsync();
      return img.Id;
    }
#endif
    [HttpPost]
    [Route("upload")]
    public async Task<ActionResult<ImageInformationResponse>> Upload(
      string token,
      Guid event_id,
      IFormFile file
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var user = await _context.Users.FindAsync(userId);
      var canAccess = _paidResourceGuard.CanAccessPaidResource(user);
      if(file.Length > MaxFilesize)
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      var ev = await _context.Events.FindAsync(event_id);
      if(ev is null)
        return NotFound();
      if(!ev.Group.Users.Any(u => u.Id == userId))
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      var extension = Path.GetExtension(file.FileName);
      if(!(extension == ".png" || extension == ".jpeg" || extension == ".jpg"))
        return new StatusCodeResult(StatusCodes.Status418ImATeapot);
      var filename = $"{DateTime.UtcNow:yyyyddmmssffff}{_rnd.Next(0, 1000)}{extension}";
      var client = _blobClientFactory.GetImageClient(filename);
      using var stream = file.OpenReadStream();
      if(!await canAccess)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
      await client.UploadAsync(stream);
      var image = new Image {
        DebtsFromImage = new List<DebtFromImage>(),
        EventId = event_id,
        GroupId = ev.GroupId,
        UploadedById = userId,
        UploadTime = DateTime.UtcNow,
        Url = client.Uri.ToString(),
      };
      try {
        foreach(var purchase in await _recieptExtractor.ExtractPurchases(client.Uri)) {
          image.DebtsFromImage.Add(new() {
            Amount = purchase.Amount,
            Description = purchase.Description,
            Image = image,
            Satisfied = false,
          });
        }
      } catch(Exception ex) {
        _logger.LogError(ex.ToString());
      }
      await _context.DebtsFromImages.AddRangeAsync(image.DebtsFromImage);
      await _context.Images.AddAsync(image);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new ImageInformationResponse {
        debts_from_image = from debt in image.DebtsFromImage
                           select new ImageInformationResponse.DebtFromImageInformation {
                             amount = debt.Amount,
                             description = debt.Description,
                             id = debt.Id,
                             satisfied = debt.Satisfied,
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
      if(userId is null)
        return Unauthorized();
      var image = await _context.Images.FindAsync(body.id);
      if(image is null)
        return NotFound();
      if(image.UploadedById != userId && image.Group.OwnerId != userId)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
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
      if(userId is null)
        return Unauthorized();
      var debt = await _context.DebtsFromImages.FindAsync(body.id);
      if(debt is null)
        return NotFound();
      _context.DebtsFromImages.Remove(debt);
      await _context.SaveChangesAsync();
      return Ok();
    }
    public class UpdateImageDebtBody {
      public Guid id { get; set; }
      public uint amount { get; set; }
      [MaxLength(128)]
      public string description { get; set; }
    }
    [HttpPatch]
    [Route("update_image_debt")]
    public async Task<ActionResult> UpdateImageDebt(
      string token,
      [FromBody] UpdateImageDebtBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      var debt = await _context.DebtsFromImages.FindAsync(body.id);
      if(debt is null)
        return NotFound();
      if(debt.Satisfied)
        return new StatusCodeResult((int)HttpStatusCode.Forbidden);
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
    public class CreateImageDebtResponse {
      public uint amount { get; set; }
      [MaxLength(128)]
      public string description { get; set; }
      public Guid id { get; set; }
      public Guid image_id { get; set; }
      public bool satisfied { get; set; }
      public Guid? pending_debt_id { get; set; }
    }
    [HttpPost]
    [Route("create_image_debt")]
    public async Task<ActionResult<CreateImageDebtResponse>> CreateImageDebt(
      string token,
      [FromBody] CreateImageDebtBody body
    ) {
      var userId = _userManager.IsLoggedIn(token);
      if(userId is null)
        return Unauthorized();
      if(!await _context.Images.AnyAsync(i => i.Id == body.image_id))
        return NotFound();
      var debt = new DebtFromImage {
        Amount = body.amount,
        Description = body.description,
        ImageId = body.image_id,
        Satisfied = false,
      };
      await _context.DebtsFromImages.AddAsync(debt);
      try {
        await _context.SaveChangesAsync();
      } catch(DbUpdateException ex) {
        _logger.LogError(ex.ToString());
        return Conflict();
      }
      return new CreateImageDebtResponse {
        amount = debt.Amount,
        description = debt.Description,
        id = debt.Id,
        image_id = debt.ImageId,
        pending_debt_id = null,
        satisfied = debt.Satisfied,
      };
    }
    public const int MaxFilesize = (int)10e+6; // 10Mb
    private readonly IUserManager _userManager;
    private readonly LMDbContext _context;
    private readonly IBlobClientFactory _blobClientFactory;
    private readonly IPaidResourceGuard _paidResourceGuard;
    private readonly IUriRecieptExtractor _recieptExtractor;
    private readonly Random _rnd;
    private readonly ILogger<ImagesController> _logger;
  }
}
