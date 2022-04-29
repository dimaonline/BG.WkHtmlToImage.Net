using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BG.WkHtmlToImage.Net;

public class HtmlToImageConverter
{
    public string Convert(string url, string outputFile, string arguments = "")
    {
        try
        {
            ReadResourceNameFromAssembly();
            string workDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine($"Work directory: {workDirectory}");
            string exePath = ExtractAndGetExePath(workDirectory);
            Console.WriteLine($"ExePath: {exePath}");
            string output = RunWkHtmlToImage(exePath, arguments, url, outputFile);
            Console.WriteLine($"Output: \n{output}");
            // File.Delete(exePath);
            return output;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return $"[Error]: {e.Message} {e.InnerException}";
        }
    }

    private string ExtractAndGetExePath(string workDirectory)
    {
        string exePath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            exePath = Path.Combine(workDirectory, "wkhtmltoimage.exe");
            ExtractResource("BG.WkHtmlToImage.Net.executables.windows.wkhtmltoimage.exe", exePath);
    
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            exePath = Path.Combine(workDirectory, "wkhtmltoimage");
            ExtractResource("BG.WkHtmlToImage.Net.executables.mac.wkhtmltoimage", exePath);
            Exec($"chmod 755 {exePath}");
        }
        else
        {
            // exePath = Path.Combine(workDirectory, "wkhtmltoimage");
            // ExtractResource("BG.WkHtmlToImage.Net.executables.linux.wkhtmltoimage", exePath);
            // Exec($"sudo chmod 755 {exePath}");
            // Exec($"chmod 755 {exePath}");
            // Exec("sudo apt-get -y install libgdiplus libc6-dev");
            // Exec("apt-get update -qq && apt-get -y install libgdiplus libc6-dev");
            
            string packagePath = Path.Combine(workDirectory, "wkhtmltox_0.12.6-1.buster_amd64.deb");
            if (!File.Exists(packagePath))
            {
                ExtractResource("BG.WkHtmlToImage.Net.executables.linux.wkhtmltox_0.12.6-1.buster_amd64.deb", packagePath);
                Exec("apt-get update -qq");
                Exec("dpkg -i wkhtmltox_0.12.6-1.buster_amd64.deb");
                Exec("apt-get -y install -f");
                Exec("ln -s /usr/local/bin/wkhtmltoimage /usr/bin");
            }

            exePath = "/usr/bin/wkhtmltoimage";
        }

        return exePath;
    }

    private void ReadResourceNameFromAssembly()
    {
        Console.WriteLine("Reading resources list from assembly:");
        foreach (var resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
        {
            Console.WriteLine(resourceName);
        }
    }
    
    private void ExtractResource(string resource, string extractPath)
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource) ?? throw new InvalidOperationException($"Resource not found: {resource}");
        byte[] bytes = new byte[(int)stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        File.WriteAllBytes(extractPath, bytes);
    }
    
    private void Exec(string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\""
            }
        };

        process.Start();
        process.WaitForExit();
    }

    private string RunWkHtmlToImage(string wkHtmlToImagePath, string arguments, string url, string outputFile)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            arguments = $"{arguments} \"{url}\" \"{outputFile}\"";
                
            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = wkHtmlToImagePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    // RedirectStandardInput = true,
                    CreateNoWindow = true
                };

                proc.Start();

                using (var ms = new MemoryStream())
                {
                    using (var sOut = proc.StandardOutput.BaseStream)
                    {
                        byte[] buffer = new byte[4096];
                        int read;

                        while ((read = sOut.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                    }

                    if (!proc.WaitForExit(60000))
                    {
                        proc.Kill();
                    }
                        
                    return proc.StandardError.ReadToEnd(); 
                }
            }
        }
        else
        {
            arguments = $"{arguments} \"{url}\" \"{outputFile}\"";

            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = wkHtmlToImagePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    //RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    //RedirectStandardInput = true,
                    CreateNoWindow = true
                };

                proc.Start();

                using (var ms = new MemoryStream())
                {
                    string consoleOutput = proc.StandardError.ReadToEnd();

                    if (!proc.WaitForExit(60000))
                    {
                        proc.Kill();
                    }

                    return consoleOutput;
                }
            }
        }
    }
}