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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _reppo;
        public MessageController(IMapper mapper, IDatingRepository reppo)
        {
            _reppo = reppo;
            _mapper = mapper;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var messageFromRepo = await _reppo.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();
            
            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage (int userId,
             MessageForCreationDto messageForCreationDto)
             {
                 var sender = await _reppo.GetUser(userId);
                if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                     return Unauthorized();
                
                messageForCreationDto.SenderId = userId;

                var recipient = await _reppo.GetUser(messageForCreationDto.ReceiverId);

                if (recipient == null)
                    return BadRequest("Could not find user");
                
                var message = _mapper.Map<Message>(messageForCreationDto);

                _reppo.Add(message);
               

                if (await _reppo.SaveAll())
                {
                    var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                     return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
                }
                    
                
                throw new Exception("Creating the message failed on save");
             }  

        [HttpGet]

        public async Task<IActionResult> GetMessagesForUser (int userId,
        [FromQuery]MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            messageParams.UserId = userId;
            
            var messagesFromRepo = await _reppo.GetMessagesForUser(messageParams);
            var messagesToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                 messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
                
            return Ok(messagesToReturn);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var messageFromRepo = await _reppo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            return Ok(messageThread);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var messageFromRepo = await _reppo.GetMessage(id);
            
            if (messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;
            
            if (messageFromRepo.ReceiverId == userId)
                messageFromRepo.ReceiverDeleted = true;
            
            if (messageFromRepo.SenderDeleted && messageFromRepo.ReceiverDeleted)
                _reppo.Delete(messageFromRepo);
            
            if (await _reppo.SaveAll())
                return NoContent();
            
            throw new Exception("Error deleting message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id, int userId) {
            
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            
            var message = await _reppo.GetMessage(id);

            if (message.ReceiverId != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _reppo.SaveAll();

            return NoContent();
        }

    }
}