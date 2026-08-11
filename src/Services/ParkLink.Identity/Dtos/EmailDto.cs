namespace ParkLink.Identity.Dtos
{
    public class EmailDto
    {
        public string smtpFrom { get; set; } = string.Empty;
        public string strSubject { get; set; } = string.Empty;
        public string strMessage { get; set; } = string.Empty;
        public string smtpServer { get; set; } = string.Empty;
        public int smtPort { get; set; }
        public string smtpUser { get; set; } = string.Empty;
        public string smtPwd { get; set; } = string.Empty;
        public bool bUseSSL { get; set; }
        public bool bStartTls { get; set; }
        public string strUserEmail { get; set; } = string.Empty;
    }
}
