//using MVCWeb.Service.IService;

//namespace MVCWeb.Service
//{
//    public class FileHandler: IFileHandler
//    {
//        private readonly string _uploadsDirectory;

//        public FileHandler(string uploadsDirectory)
//        {
//            _uploadsDirectory = uploadsDirectory;
//        }

//        public async Task<string> UploadFile(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//                throw new ArgumentException("Geçersiz dosya");

//            var uploadsFolderPath = Path.Combine(_uploadsDirectory, "uploads");
//            if (!Directory.Exists(uploadsFolderPath))
//                Directory.CreateDirectory(uploadsFolderPath);

//            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
//            var filePath = Path.Combine(uploadsFolderPath, fileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            return fileName;
//        }
//    }
//}
