using Firebase.Auth;
using Firebase.Storage;
using Google.Apis.Storage.v1.Data;
using Microsoft.AspNetCore.Hosting;

namespace SmartClinics.Helers
{
    public class SaveToFirebase
    {
        private static string ApiKey = "";
        private static string Bucket = "";
        private static string AuthEmail = "";
        private static string AuthPassword = "";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public SaveToFirebase(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> ImageToFirebase(IFormFile file)
        {
            if (file.Length > 0)
            {
                // Save file to local server (optional)
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var authLink = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string localFilePath = Path.Combine(uploadsFolder, file.FileName);

                using (var stream = new FileStream(localFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Initialize stream from local file
                using (var stream = new FileStream(localFilePath, FileMode.Open))
                {
                    // Upload to Firebase Storage

                    var cancellation = new CancellationTokenSource();

                    var uploadTask = new FirebaseStorage(
                        Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(authLink.FirebaseToken),
                            ThrowOnCancel = true,
                        })
                        .Child("uploads")
                        .Child(file.FileName)
                        .PutAsync(stream, cancellation.Token);

                    return await uploadTask;
                }
            }
            return null;
        }
    }
}
