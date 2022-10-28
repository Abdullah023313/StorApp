namespace StorApp.Services
{
    public interface IMailService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task SendEmailAsync(string mailTo, string subject, string body, string displayName, IList<IFormFile> attachments = null);
    }
}