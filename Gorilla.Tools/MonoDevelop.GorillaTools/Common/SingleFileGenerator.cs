using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;

namespace MonoDevelop.GorillaTools.Common
{
    public abstract class SingleFileGenerator : ISingleFileCustomTool
    {
        public abstract FilePath Generate(ProjectFile file);
        public abstract void HandleException(Exception ex, ProjectFile file, SingleFileCustomToolResult result);

        public IAsyncOperation Generate(IProgressMonitor monitor, ProjectFile file, SingleFileCustomToolResult result)
        {
            return new ThreadAsyncOperation(i =>
            {

                FilePath output = file.FilePath;

                try
                {
                    output = Generate(file);
                }
                catch (Exception ex)
                {
                    //File.WriteAllText(@"C:\Users\Vlad Durnea\Developer\Logs\monodevelop." + GetType().Name + ".log","exception while compiling:" + output.FullPath + "\n" + ex.Message + "\nStack:\n" + ex.StackTrace );
                    HandleException(ex, file, result);
                }

                result.GeneratedFilePath = output;

            }, result, !result.Success);
        }
    }
}
