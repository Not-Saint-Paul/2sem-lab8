using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Lab8
{
    interface IView
    {
        string FirstDirectory();
        string SecondDirectory();

        void ShowFilesOfFirstDirectory(List<string> list);
        void ShowFilesOfSecondDirectory(List<string> list);
        void TrySynchronize(List<string> message);

        event EventHandler<EventArgs> SyncronizeDirectoriesEvent;
        event EventHandler<EventArgs> Inversion;
        event EventHandler<EventArgs> ShowFilesOfFirstDirectoryEvent;
        event EventHandler<EventArgs> ShowFilesOfSecondDirectoryEvent;

    }

    class Model
    {
        private bool isInverted = false;

        public void Invert()
        {
            isInverted = !isInverted;
        }
        public List<string> ListOfFilesInDirectory(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            List<string> resultOfList = new List<string>();
            foreach (FileInfo directoryFile in directoryInfo.GetFiles())
            {
                resultOfList.Add("{directoryFile.Name}");
            }

            return resultOfList;
        }

        public List<string> SynchronizeDirectories(string firstDirectory, string secondDirectory)
        {
            DirectoryInfo mainDirectoryInfo = new DirectoryInfo(firstDirectory);
            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(secondDirectory);
            List<string> resultOfSynchronize;

            if (!isInverted)
            {
                resultOfSynchronize = InnerSynchronizeDirectories(mainDirectoryInfo, targetDirectoryInfo);
                return resultOfSynchronize;
            }
            else
            {
                resultOfSynchronize = InnerSynchronizeDirectories(targetDirectoryInfo, mainDirectoryInfo);
                return resultOfSynchronize;
            }
        }

        private List<string> InnerSynchronizeDirectories(DirectoryInfo mainDirectoryInfo, DirectoryInfo targetDirectoryInfo)
        {
            List<string> innerResultOfSynchronize = new List<string>();
            bool isNeedToSynchronize = false;

            foreach (FileInfo directoryFile in mainDirectoryInfo.GetFiles())
            {
                FileInfo targetFileInOtherDirectory = new FileInfo(Path.Combine(targetDirectoryInfo.FullName, directoryFile.Name));

                if (!targetFileInOtherDirectory.Exists || targetFileInOtherDirectory.LastWriteTime < directoryFile.LastWriteTime) {
                    File.Copy(directoryFile.FullName, targetFileInOtherDirectory.FullName, true);
                    innerResultOfSynchronize.Add($"File {directoryFile.Name} was changed");
                    isNeedToSynchronize = true;
                }
            }

            foreach (FileInfo directoryFile in targetDirectoryInfo.GetFiles())
            {
                FileInfo fileInMainDirectory = new FileInfo(Path.Combine(mainDirectoryInfo.FullName, directoryFile.Name));

                if (!fileInMainDirectory.Exists)
                {
                    directoryFile.Delete();
                    innerResultOfSynchronize.Add($"File {directoryFile.Name} was deleted");
                    isNeedToSynchronize = true;
                }
            }

            if (!isNeedToSynchronize)
            {
                innerResultOfSynchronize.Add("No changes");
            }

            return innerResultOfSynchronize;
        }
    }

    class Presenter
    {
        private IView mainView;
        private Model model;

        public Presenter(IView inputView)
        {
            mainView = inputView;
            model = new Model();

            mainView.SyncronizeDirectoriesEvent += new EventHandler<EventArgs>(Synchronize);
            mainView.ShowFilesOfFirstDirectoryEvent += new EventHandler<EventArgs>(ShowFilesInFirstDirectory);
            mainView.ShowFilesOfSecondDirectoryEvent += new EventHandler<EventArgs>(ShowFilesInSecondDirectory);

        }

        private void Inversion(object sender, EventArgs inputEvent)
        {
            model.Invert();
        }

        private void Synchronize(object sender, EventArgs inputEvent)
        {
            List<string> resultOfSynchronization = model.SynchronizeDirectories(mainView.FirstDirectory(), mainView.SecondDirectory());

            mainView.TrySynchronize(resultOfSynchronization);
        }

        private void ShowFilesInFirstDirectory(object sender, EventArgs inputEvent)
        {
            List<string> listOfFilesInFirstDirectory = model.ListOfFilesInDirectory(mainView.FirstDirectory());

            mainView.ShowFilesOfFirstDirectory(listOfFilesInFirstDirectory);
        }

        private void ShowFilesInSecondDirectory(object sender, EventArgs inputEvent)
        {
            List<string> listOfFilesInFirstDirectory = model.ListOfFilesInDirectory(mainView.SecondDirectory());

            mainView.ShowFilesOfSecondDirectory(listOfFilesInFirstDirectory);
        }
    }
    internal static class Program
    {

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
