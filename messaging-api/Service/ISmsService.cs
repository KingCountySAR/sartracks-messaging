using System.Threading.Tasks;

namespace SarData.Messaging.Api.Service
{
  public interface ISmsService
  {
    Task SendMessage(string to, string message);
  }
}
