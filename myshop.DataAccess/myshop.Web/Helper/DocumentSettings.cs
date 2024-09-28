namespace myshop.Web.Helper
{
    public static class DocumentSettings
    {
        public static string UploadFile (IFormFile file, string folderName)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", folderName);

            var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";

            var filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);
            return filePath;   
        }
        public static string EditFile (IFormFile file, string filePath, string folderName)
        {
            if (file !=null)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return UploadFile(file, folderName);
            }
            return null;
        }
        public static void DeleteFile (string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
