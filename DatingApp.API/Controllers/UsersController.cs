using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helper;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{   
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _rep;
        private readonly IMapper _mapp;
        public UsersController(IDatingRepository rep, IMapper mapp)
        {
            _mapp = mapp;
            _rep = rep;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _rep.GetUser(currentUserId);

            userParams.UserId = currentUserId;
            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male"? "female" : "male";
            }
            var users = await _rep.GetUsers(userParams);

            var usersToReturn = _mapp.Map<IEnumerable<UserForListDTO>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,
                 users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _rep.GetUser(id);

            var userToRetun = _mapp.Map<UserForDetailDTO>(user);
            return Ok(userToRetun);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO userForUpdateDTO)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var userFromRepo = await _rep.GetUser(id);

            _mapp.Map(userForUpdateDTO, userFromRepo);

            if(await _rep.SaveAll())
                return NoContent();
            
            throw new Exception($"Updating user {id} failed");
            
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();

            var like = await _rep.GetLike(id, recipientId);

            if(like != null) 
                return BadRequest("You already like this user");
            
            
            if(await _rep.GetUser(recipientId) == null)
                return NotFound();
            
            like = new Like {

                LikerId = id,
                LikeeId = recipientId
            };

            _rep.Add<Like>(like);

            if(await _rep.SaveAll())
                return Ok();
            
            return BadRequest("Failed to like user");

        }
    }
}