using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LixoZero.Specs.Support
{
    public static class TestLog
    {
        // Usa a pasta da build (bin/.../net8.0/TestResults)
        private static readonly string Dir     = System.IO.Path.Combine(AppContext.BaseDirectory, "TestResults");
        private static readonly string LogPath = System.IO.Path.Combine(Dir, "bdd-http.log");

        public static async Task WriteAsync(string message)
        {
            Directory.CreateDirectory(Dir);
            await File.AppendAllTextAsync(
                LogPath,
                $"{DateTime.Now:O} {message}{Environment.NewLine}",
                Encoding.UTF8
            );
        }

        // Se quiser acessar o caminho do arquivo em algum lugar:
        public static string PathToLog => LogPath;
    }
}
