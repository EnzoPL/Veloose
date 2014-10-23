using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Veloose.Model;

namespace Veloose.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        private string _helloWorld;

        public string HelloWorld
        {
            get { return _helloWorld; }
            set { Set(() => HelloWorld, ref _helloWorld, value); }
        }

        

        /// <summary>
        /// The <see cref="StationContainer" /> property's name.
        /// </summary>
        public const string StationContainerPropertyName = "StationContainer";

        private RootObject _stationContaineRootObject = new RootObject();   

        /// <summary>
        /// Sets and gets the StationContainer property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public RootObject StationContainer
        {
            get
            {
                return _stationContaineRootObject;
            }

            set
            {
                if (_stationContaineRootObject == value)
                {
                    return;
                }

//                RaisePropertyChanging(StationContainerPropertyName);
                _stationContaineRootObject = value;
                RaisePropertyChanged(StationContainerPropertyName);
            }
        }

        public async void LoadJson()
        {
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            folder = await folder.GetFolderAsync("Data");
            StorageFile file = await folder.GetFileAsync("Velo_Toulouse.json");
            string rtfString = await Windows.Storage.FileIO.ReadTextAsync(file);

            StationContainer = JsonConvert.DeserializeObject<RootObject>(rtfString);
        }

        public void LoadLocal()
        {
            StationContainer = new RootObject
            {
                features =
                    new List<Feature>
                        {
                            new Feature {properties = new Properties2 {num_station = 1, nom = "Duclos"}},
                            new Feature {properties = new Properties2 {num_station = 2, nom = "Herbette"}}
                        }
            };
        }

        private async Task UnZipFileAsync(StorageFile zipFile, StorageFolder unzipFolder)
        {
                await UnZipFileAync(zipFile, unzipFolder);
        } 
        
        #region private helper functions

        public static IAsyncAction UnZipFileAync(StorageFile zipFile, StorageFolder destinationFolder)
        {
            return UnZipFileHelper(zipFile, destinationFolder).AsAsyncAction();
        } 
        private static async Task UnZipFileHelper(StorageFile zipFile, StorageFolder destinationFolder)
        {
            if (zipFile == null || destinationFolder == null ||
                !Path.GetExtension(zipFile.DisplayName).Equals(".zip", StringComparison.CurrentCultureIgnoreCase)
                )
            {
                throw new ArgumentException("Invalid argument...");
            }
            Stream zipMemoryStream = await zipFile.OpenStreamForReadAsync();
            // Create zip archive to access compressed files in memory stream 
            using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
            {
                // Unzip compressed file iteratively. 
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    await UnzipZipArchiveEntryAsync(entry, entry.FullName, destinationFolder);
                }
            }
        }
        /// <summary> 
        /// It checks if the specified path contains directory. 
        /// </summary> 
        /// <param name="entryPath">The specified path</param> 
        /// <returns></returns> 
        private static bool IfPathContainDirectory(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                return false;
            }
            return entryPath.Contains("/");
        }
        /// <summary> 
        /// It checks if the specified folder exists. 
        /// </summary> 
        /// <param name="storageFolder">The container folder</param> 
        /// <param name="subFolderName">The sub folder name</param> 
        /// <returns></returns> 
        private static async Task<bool> IfFolderExistsAsync(StorageFolder storageFolder, string subFolderName)
        {
            try
            {
                await storageFolder.GetFolderAsync(subFolderName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
        /// <summary> 
        /// Unzips ZipArchiveEntry asynchronously. 
        /// </summary> 
        /// <param name="entry">The entry which needs to be unzipped</param> 
        /// <param name="filePath">The entry's full name</param> 
        /// <param name="unzipFolder">The unzip folder</param> 
        /// <returns></returns> 
        private static async Task UnzipZipArchiveEntryAsync(ZipArchiveEntry entry, string filePath, StorageFolder unzipFolder)
        {
            if (IfPathContainDirectory(filePath))
            {
                // Create sub folder 
                string subFolderName = Path.GetDirectoryName(filePath);
                bool isSubFolderExist = await IfFolderExistsAsync(unzipFolder, subFolderName);
                StorageFolder subFolder;
                if (!isSubFolderExist)
                {
                    // Create the sub folder. 
                    subFolder =
                        await unzipFolder.CreateFolderAsync(subFolderName, CreationCollisionOption.ReplaceExisting);
                }
                else
                {
                    // Just get the folder. 
                    subFolder =
                        await unzipFolder.GetFolderAsync(subFolderName);
                }
                // All sub folders have been created. Just pass the file name to the Unzip function. 
                string newFilePath = Path.GetFileName(filePath);
                if (!string.IsNullOrEmpty(newFilePath))
                {
                    // Unzip file iteratively. 
                    await UnzipZipArchiveEntryAsync(entry, newFilePath, subFolder);
                }
            }
            else
            {
                // Read uncompressed contents 
                using (Stream entryStream = entry.Open())
                {
                    byte[] buffer = new byte[entry.Length];
                    entryStream.Read(buffer, 0, buffer.Length);
                    // Create a file to store the contents 
                    StorageFile uncompressedFile = await unzipFolder.CreateFileAsync
                    (entry.Name, CreationCollisionOption.ReplaceExisting);
                    // Store the contents 
                    using (IRandomAccessStream uncompressedFileStream =
                    await uncompressedFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (Stream outstream = uncompressedFileStream.AsStreamForWrite())
                        {
                            outstream.Write(buffer, 0, buffer.Length);
                            outstream.Flush();
                        }
                    }
                }
            }
        }
        #endregion

        private StorageFolder GetFolder(string name)
        {
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            return folder.GetFolderAsync(name).GetResults();
        }

        private StorageFile GetFile(StorageFolder folder, string fileName)
        {
            return folder.GetFileAsync(fileName).GetResults();
        }

        private async void UnZip()
        {
            var fileName ="12546-velo-toulouse.zip";
            var folder = GetFolder("Data");
           
            StorageFile localFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName); 
            var unzipFolder =
                await folder.CreateFolderAsync(Path.GetFileNameWithoutExtension(fileName),
                CreationCollisionOption.GenerateUniqueName);
            await UnZipFileAsync(localFile, unzipFolder);
        }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
//            UnZip();
            LoadJson();

            HelloWorld = IsInDesignMode
                ? "Runs in design mode"
                : "Runs in runtime mode";
        }
    }
}
