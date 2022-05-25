using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureBlobStorage
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Azure Blob Storage v12 - .NET quickstart sample\n");

            // Blob Storage 連線字串
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=waynemachinele2852186040;AccountKey=KDWrY7BuJteNlWrhrJq/g7QbkVg8YUGBWoL2SFI2X4xV3UH9DpAJZYQPYiGUMweuyPVpoAEd3JxwYBruLZrU4A==;EndpointSuffix=core.windows.net";

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            Console.WriteLine("----------Container List----------");
            Console.WriteLine();
            GetBlobContainers(blobServiceClient);
            Console.WriteLine();

            Console.WriteLine("----------CreateSampleContainerAsync----------");
            Console.WriteLine();
            await CreateSampleContainerAsync(blobServiceClient, "test");
            Console.WriteLine();

            Console.WriteLine("----------Container List----------");
            Console.WriteLine();
            GetBlobContainers(blobServiceClient);
            Console.WriteLine();

            Console.WriteLine("----------DeleteContainersWithPrefixAsync----------");
            await DeleteContainersWithPrefixAsync(blobServiceClient, "test");
            Console.WriteLine();

            Console.WriteLine("----------Container List----------");
            Console.WriteLine();
            GetBlobContainers(blobServiceClient);
            Console.WriteLine();

            Console.ReadKey();
        }

        //-------------------------------------------------
        // Get container list
        //-------------------------------------------------
        public static void GetBlobContainers(BlobServiceClient blobServiceClient)
        {
            // 列出Blob Storage所有Container List
            var containerList = blobServiceClient.GetBlobContainers();

            foreach (BlobContainerItem blobcontainer in containerList)
            {
                Console.WriteLine(blobcontainer.Name);
            }
        }

        //-------------------------------------------------
        // Create a container
        //-------------------------------------------------
        private static async Task<BlobContainerClient> CreateSampleContainerAsync(BlobServiceClient blobServiceClient, string containerName)
        {
            // Name the sample container based on new GUID to ensure uniqueness.
            // The container name must be lowercase.
            containerName += Guid.NewGuid();

            try
            {
                // Create the container
                BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

                if (await container.ExistsAsync())
                {
                    Console.WriteLine("Created container {0}", container.Name);
                    return container;
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }

            return null;
        }


        //-------------------------------------------------
        // Delete a container
        //-------------------------------------------------
        private static async Task DeleteSampleContainerAsync(BlobServiceClient blobServiceClient, string containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

            try
            {
                // Delete the specified container and handle the exception.
                await container.DeleteAsync();
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }


        //-------------------------------------------------
        // Delete all containers with the specified prefix
        //-------------------------------------------------
        private static async Task DeleteContainersWithPrefixAsync(BlobServiceClient blobServiceClient, string prefix)
        {
            Console.WriteLine($"Delete all containers beginning with the specified prefix {prefix}");

            try
            {
                foreach (BlobContainerItem container in blobServiceClient.GetBlobContainers())
                {
                    if (container.Name.StartsWith(prefix))
                    {
                        Console.WriteLine("\tContainer:" + container.Name);
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container.Name);
                        await containerClient.DeleteAsync();
                    }
                }

                Console.WriteLine();
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
