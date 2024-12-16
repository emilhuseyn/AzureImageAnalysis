using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomAINLP
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Initialize the ComputerVision client
            var client = CreateComputerVisionClient();

            // URL of the image to be analyzed
            string imageUrl = "YOUR_IMAGE_URL_HERE";  // Replace with the image URL

            // Analyze the image and print the result
            bool isImageSafe = await ImageAnalyze.AnalyzeImageAsync(client, imageUrl);
            Console.WriteLine($"Is the image safe? {isImageSafe}");
        }

        // Helper method to create and configure the ComputerVisionClient
        private static ComputerVisionClient CreateComputerVisionClient()
        {
            string apiKey = "******";  // Replace with your API key
            string endpoint = "******";  // Replace with your endpoint

            return new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey))
            {
                Endpoint = endpoint
            };
        }
    }

    public static class ImageAnalyze
    {
        // Define the list of visual features for analysis
        private static readonly List<VisualFeatureTypes?> Features = new List<VisualFeatureTypes?>()
        {
            VisualFeatureTypes.Adult
        };

        // Main method to analyze the image and check for adult content
        public static async Task<bool> AnalyzeImageAsync(ComputerVisionClient client, string imageUrl)
        {
            try
            {
                // Download the image content
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(imageUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Analyze the image stream
                        using (Stream imageStream = await response.Content.ReadAsStreamAsync())
                        {
                            ImageAnalysis results = await client.AnalyzeImageInStreamAsync(imageStream, visualFeatures: Features);

                            // Check if the content is adult or racy
                            return !results.Adult.IsAdultContent && !results.Adult.IsRacyContent;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to download the image.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
