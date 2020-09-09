using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helper;
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
        public async Task<IActionResult> GetUsers()
        {

            var users = await _rep.GetUsers();

            var usersToReturn = _mapp.Map<IEnumerable<UserForListDTO>>(users);

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
    }
}