using System.Text;

namespace Dispatcher.Tests
{
    public class MessageWriter
    {
        public readonly StringBuilder _stringBuilder = new();
        public void WriteMessage(string message)
        {
            _stringBuilder.AppendLine(message);
        }
        public string GetMessages()
        {
            return _stringBuilder.ToString();
        }
    }
}
