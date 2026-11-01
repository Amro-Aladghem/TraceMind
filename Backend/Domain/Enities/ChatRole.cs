using System.Collections.Generic;

namespace Domain.Entities
{
    public class ChatRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    }
}
