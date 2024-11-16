using System;
using api.DTO;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var username = User.GetUsername();

        if (username == createMessageDTO.RecipientUsername.ToLower()) return BadRequest("You cannot message yourself!");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (recipient == null || sender == null) return BadRequest("Cannot send message right now");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.Username,
            RecipientUsername = recipient.Username,
            Content = createMessageDTO.Content
        };

        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDTO>(message));

        return BadRequest("Could not send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        return Ok(await messageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await messageRepository.GetMessage(id);

        if (message == null) return BadRequest("Message does not exist");

        if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid("You are not allowed to do that.");

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true }) // same as message.SenderDeleted && message.RecipientDeleted
        {
            messageRepository.DeleteMessage(message);
        }

        if (await messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Error deleting message.");
    }
}
