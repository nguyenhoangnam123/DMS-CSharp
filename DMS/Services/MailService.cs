using Common;
using DMS.Entities;
using DMS.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public interface IMailService
    {
        Task<Mail> Send(Mail Mail);
    }
    public class MailService : IServiceScoped
    {
        private ICurrentContext CurrentContext;
        public MailService(ICurrentContext CurrentContext)
        {
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> Send(Mail Mail)
        {
            RestClient restClient = new RestClient($"http://localhost:{Modules.Utils}");
            RestRequest restRequest = new RestRequest("/rpc/utils/mail/create");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            Mail.Attachments.ForEach(x => restRequest.AddFile("Files", x.Content, x.FileName, x.ContentType));
            Mail.Recipients.ForEach(x => restRequest.AddParameter("Recipients", x));
            restRequest.AddParameter("Subject", Mail.Subject);
            restRequest.AddParameter("Content", Mail.Body);
            try
            {
                var response = restClient.Execute<bool>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
