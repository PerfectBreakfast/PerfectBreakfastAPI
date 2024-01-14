namespace PerfectBreakfast.Application.Models.MailModels;

public class MailDataViewModel
{
    // Receiver
    public List<string> To { get; }

    // Content
    public string Subject { get; }
    public string? Body { get; }

    // Attachment
    public Stream? ExcelAttachmentStream { get; set; }
    public string? ExcelAttachmentFileName { get; set; }

    // Constructor with attachment
    public MailDataViewModel(List<string> to, string subject, string? body = null,
                             Stream? excelAttachmentStream = null, string? excelAttachmentFileName = null)
    {
        // Receiver
        To = to;

        // Content
        Subject = subject;
        Body = body;

        // Attachment
        ExcelAttachmentStream = excelAttachmentStream;
        ExcelAttachmentFileName = excelAttachmentFileName;
    }
}