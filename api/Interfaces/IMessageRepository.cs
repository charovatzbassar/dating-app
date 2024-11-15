using System;
using api.DTO;
using api.Entities;
using api.Helpers;

namespace api.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessage(int id);
    Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
}
