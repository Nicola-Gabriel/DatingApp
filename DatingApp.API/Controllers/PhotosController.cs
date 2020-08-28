using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helper;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapp;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private Cloudinary _cloudinary;
        public PhotosController(IDatingRepository repo, IMapper mapp,
        IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings;
            _mapp = mapp;
            _repo = repo;

            Account acc = new Account (
                
                    _cloudinarySettings.Value.CloudName,
                    _cloudinarySettings.Value.ApiKey,
                    _cloudinarySettings.Value.ApiSecret
                
            );

            _cloudinary = new Cloudinary(acc);

        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _repo.GetPhoto(id);
            var photoForReturn = _mapp.Map<PhotoToReturnDto>(photo);

            return Ok(photoForReturn);
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int userId,
         [FromForm]PhotoForUploadDto photoForUploadDto) 
         {

            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var userFromRepo = await _repo.GetUser(userId);

            var file = photoForUploadDto.File;
            var uploadResult = new ImageUploadResult();

            if(file.Length > 0) 
            {
                using (var stream = file.OpenReadStream()) 
                {
                    var uploadParams = new ImageUploadParams() 
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForUploadDto.Url = uploadResult.Uri.ToString();
            photoForUploadDto.PublicId = uploadResult.PublicId;
            
            var photo = _mapp.Map<Photo>(photoForUploadDto);

            if(!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if(await _repo.SaveAll())
            {
                var returnedPhoto = _mapp.Map<PhotoToReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id = photo.Id}, returnedPhoto);
            }

            return BadRequest("Could not add photo ");
         }


         [HttpPost("{id}/setMain")]
         public async Task<IActionResult> SetMainPhoto(int userId, int id)
         {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");
            
            var user = await _repo.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var currentPhoto = await _repo.GetMainPhotoForUser(userId);

            currentPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
                return NoContent();
            
            return BadRequest("Could not set main photo");


            
         }

         [HttpDelete("{id}")]
         public async Task<IActionResult> DeletePhoto(int userId, int id)
         {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain)
                return BadRequest("You can not delete your main photo");
            
            var user = await _repo.GetUser(userId);

            if(photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                 if (result.Result == "ok")
                     _repo.Delete(photoFromRepo);
            }
            
            if(photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if(await _repo.SaveAll())
                return Ok();
            
            return BadRequest("Failed to delete the photo");

         }

}
}