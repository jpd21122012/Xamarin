using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using FaceIdXamarinSample.Clases;
using System.Diagnostics;
using System.IO;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Plugin.Media.Abstractions;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;

namespace FaceIdXamarinSample
{
    public partial class MainPage : ContentPage
    {

        TodoItemManager manager;
        public MainPage()
        {
            InitializeComponent();
            manager = TodoItemManager.DefaultManager;
        }

        private async void UploadPictureButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Select an existing picture 
                // (check if this is supported)
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
                    return;
                }

                // Get a reference to the image file
                var file = await CrossMedia.Current.PickPhotoAsync();
                if (file == null)
                    return;
                this.Indicator1.IsVisible = true;
                this.Indicator1.IsRunning = true;

                // Get a stream from the image file
                Image1.Source = ImageSource.FromStream(() => file.GetStream());

                // Analyze the specified image and then bind
                // the result to the UI
                FaceDetectionResult theData = await DetectFacesAsync(file.GetStream());
                this.BindingContext = theData;

                this.Indicator1.IsRunning = false;
                this.Indicator1.IsVisible = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in upload picture: " + ex);
                Result.Text = "Error in upload picture...: " + ex;
            }

        }

        private async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Initialize the camera and then allows for taking a picture 
                // (check if this is supported)
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.
                  IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", "No camera available.", "OK");
                    return;
                }

                //Take the picture and save it to the camera roll
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Name = "test.jpg"

                });
                if (file == null)
                    return;
                this.Indicator1.IsVisible = true;
                this.Indicator1.IsRunning = true;

                // Get a stream from the image file
                Image1.Source = ImageSource.FromStream(() => file.GetStream());


                // Analyze the specified image and then bind
                // the result to the UI
                FaceDetectionResult theData = await DetectFacesAsync(file.GetStream());
                this.BindingContext = theData;

                this.Indicator1.IsRunning = false;
                this.Indicator1.IsVisible = false;
            }
            catch (Exception ex)
            {

                Debug.WriteLine("Error taking photo: " + ex);
                Result.Text = "Error taking photo...: " + ex;
            }

        }

        private async void Query(string idBuscar)
        {
            //li_nom.ItemsSource = await manager.GetTodoItemsAsync(idBuscar);
            Collection<UsersUPT> s = new Collection<UsersUPT>();
            s = await manager.GetTodoItemsAsync(idBuscar);
            Debug.WriteLine(s[0].PID + "/" + s[0].nombre + "/" + s[0].edad + "/" + s[0].descripcion);
            labelNom.Text = s[0].nombre;
            labelAge.Text = s[0].edad.ToString();
            labelDesc.Text = s[0].descripcion;

        }

        private async Task<FaceDetectionResult> DetectFacesAsync(Stream image)
        {

            Result.Text = "";

            FaceServiceClient faceService = new FaceServiceClient("2bddec152651472a8cb690e00db31a43");

            FaceDetectionResult faceDetectionResult = new FaceDetectionResult();

            var requiredFaceAttributes = new FaceAttributeType[]
                {
                    FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Glasses
                };
            Face[] faces = await faceService.DetectAsync(image, returnFaceLandmarks: true, returnFaceAttributes: requiredFaceAttributes);



            if (faces.Length >= 1)
            {
                var edad = faces[0].FaceAttributes.Age;
                var genero = faces[0].FaceAttributes.Gender;
                int roundedAge = (int)Math.Round(edad);
                faceDetectionResult.FaceId = faces[0].FaceId.ToString();
                faceDetectionResult.Age = faces[0].FaceAttributes.Age;
                faceDetectionResult.Glasses = faces[0].FaceAttributes.Glasses.ToString();

                Debug.WriteLine("ID de rostro: " + faces[0].FaceId);
                Debug.WriteLine("Edad: " + edad);
                Debug.WriteLine("Género: " + genero);
                Debug.WriteLine("Lentes: " + faces[0].FaceAttributes.Glasses);
                if (faceDetectionResult.Glasses == "NoGlasses")
                {
                    Guid idGuid = Guid.Parse(faces[0].FaceId.ToString());
                    SimilarPersistedFace[] facescomp = await faceService.FindSimilarAsync(idGuid, "21122011", 1);

                    double conf = Double.Parse(facescomp[0].Confidence.ToString());
                    string pid = facescomp[0].PersistedFaceId.ToString();
                    Debug.WriteLine("conf: " + conf);

                    if (conf >= .67)
                    {
                        Result.Text = "Posible coincidencia";

                        try
                        {
                            Query(pid);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(" ex: " + ex);
                        }
                    }
                    else
                    {
                        Result.Text = "No hay coincidencias";
                    }
                }
                else
                {
                    Result.Text = "Try again without glasses!";
                }
            }
            else
            {
                Debug.WriteLine("No faces detected: {0} ", faces.Length);
                Result.Text = faces.Length + " faces detected";
            }
            return faceDetectionResult;
        }
    }
}
