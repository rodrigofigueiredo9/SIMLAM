using System.IO;
using System.Security.AccessControl;

namespace Tecnomapas.EtramiteX.WindowsService.Utilitarios
{
	public class DiretorioLog
	{
		public static void Criar(string DirectoryName)
		{
			DirectoryName += "\\log";

			Directory.CreateDirectory(DirectoryName);

			DirectoryInfo dInfo = new DirectoryInfo(DirectoryName);

			DirectorySecurity dSecurity = dInfo.GetAccessControl();

			dSecurity.AddAccessRule(new FileSystemAccessRule("LOCAL SERVICE", FileSystemRights.FullControl, AccessControlType.Allow));

			dInfo.SetAccessControl(dSecurity);
		}
	}
}