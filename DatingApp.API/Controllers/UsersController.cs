using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _rep.GetUser(id);

            var userToRetun = _mapp.Map<UserForDetailDTO>(user);
            return Ok(userToRetun);
        }
    }
}