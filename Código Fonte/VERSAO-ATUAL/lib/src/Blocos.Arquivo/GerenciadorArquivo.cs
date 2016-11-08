using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Arquivo.Data;

namespace Tecnomapas.Blocos.Arquivo
{
	public class GerenciadorArquivo
	{
		List<DirectoryInfo> Diretorios { get; set; }
		List<bool> DiretoriosAtivo { get; set; }
		private int _idxDir = 0;
		private string _esquema;

		public GerenciadorArquivo(Dictionary<Int32, String> diretorios, string esquema)
		{
			_esquema = esquema ?? "default";
			Diretorios = new List<DirectoryInfo>();
			DiretoriosAtivo = new List<bool>();

			foreach (KeyValuePair<Int32, String> item in diretorios)
			{
				DirectoryInfo info = new DirectoryInfo(item.Value);
				Diretorios.Add(info);
				DiretoriosAtivo.Add(true);
			}
		}

		private string GetDirAtual()
		{
			_idxDir = DiretoriosAtivo.IndexOf(true);

			if (_idxDir < 0)
			{
				throw new Exception("Não a local disponível para gravação de arquivo.");
			}

			if (!Diretorios[_idxDir].FullName.EndsWith("\\"))
				return Diretorios[_idxDir].FullName + "\\";
			else
				return Diretorios[_idxDir].FullName;
		}

		private string ObterNome()
		{
			string fileName = Path.GetRandomFileName();
			while (File.Exists(GetDirAtual() + fileName))
			{
				fileName = Path.GetRandomFileName();
			}
			return GetDirAtual() + fileName;
		}

		private string GerarPathFinal(string divPorQtd)
		{
			string anoMes = DateTime.Now.ToString("yyyy\\\\MM");
			string semanaDoAno = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString();

			return String.Join("\\", new[] { anoMes, semanaDoAno, divPorQtd }).Replace("\\\\", "\\");
		}

		public String Salvar(Stream st, string fileName)
		{
			if (st == null)
				throw new ArgumentNullException("Stream de arquivo é nulo.");

			FileStream fs = null;

			try
			{
				fs = File.Create(fileName);
				st.Seek(0, SeekOrigin.Begin);
				st.CopyTo(fs);
				fs.Flush();
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
					fs.Dispose();
				}

				st.Close();
				st.Dispose();
			}

			return fileName;
		}

		public void Deletar(string arquivo)
		{
			File.Delete(arquivo);
		}

		public Arquivo SalvarTemp(Stream st)
		{
			if (st == null)
			{
				throw new ArgumentNullException("Stream de arquivo é nulo.");
			}

			string fileName = ObterNome();

			try
			{
				Salvar(st, fileName);
			}
			catch (IOException)
			{
				DiretoriosAtivo[_idxDir] = false;

				_idxDir = DiretoriosAtivo.IndexOf(true);

				if (_idxDir >= 0)
				{
					return SalvarTemp(st);
				}
				else
				{
					throw;
				}
			}

			Arquivo arq = new Arquivo();
			arq.TemporarioNome = Path.GetFileName(fileName);
			arq.TemporarioPathNome = fileName;

			return arq;
		}

		public Arquivo Salvar(Arquivo arquivo, Dictionary<Int32, String> diretorios, string divisaoPorQtd)
		{
			Dictionary<Int32, String> diretoriosCopy = diretorios.ToDictionary(x => x.Key, y => y.Value);
			KeyValuePair<Int32, String> atual = diretoriosCopy.First();

			arquivo.TemporarioNome = Path.GetRandomFileName();
			arquivo.Diretorio = GerarPathFinal(divisaoPorQtd);
			arquivo.Raiz = atual.Key;
			arquivo.Caminho = String.Join("\\", new[] { atual.Value, arquivo.Diretorio, arquivo.TemporarioNome });
			arquivo.Extensao = Path.GetExtension(arquivo.TemporarioNome);

			if (!Directory.Exists(Path.GetDirectoryName(arquivo.Caminho)))
				Directory.CreateDirectory(Path.GetDirectoryName(arquivo.Caminho));

			Salvar(arquivo.Buffer, arquivo.Caminho);

			return arquivo;
		}

		public Arquivo Copiar(Arquivo arquivo, Dictionary<Int32, String> dirOrigem, Dictionary<Int32, String> diretorios, string divisaoPorQtd)
		{
			arquivo.TemporarioPathNome = dirOrigem.Single(x => File.Exists(x.Value + "\\" + arquivo.TemporarioNome)).Value;
			arquivo.Extensao = Path.GetExtension(arquivo.Nome);
			return Copiar(arquivo, diretorios, divisaoPorQtd, null);
		}

		private Arquivo Copiar(Arquivo arquivoOrigem, Dictionary<Int32, String> diretorios, string divisaoPorQtd, Exception excParm = null)
		{
			Dictionary<Int32, String> diretoriosCopy = diretorios.ToDictionary(x => x.Key, y => y.Value);

			if (diretoriosCopy.Count <= 0)
				throw new Exception("Erro ao mover arquivo", excParm);

			KeyValuePair<Int32, String> atual = diretoriosCopy.First();

			arquivoOrigem.Diretorio = GerarPathFinal(divisaoPorQtd);
			arquivoOrigem.Raiz = atual.Key;
			arquivoOrigem.Caminho = String.Join("\\", new[] { atual.Value, arquivoOrigem.Diretorio, arquivoOrigem.TemporarioNome });

			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(arquivoOrigem.Caminho)))
					Directory.CreateDirectory(Path.GetDirectoryName(arquivoOrigem.Caminho));

				if (!File.Exists(arquivoOrigem.Caminho))
				{
					File.Copy(arquivoOrigem.TemporarioPathNome + "\\" + arquivoOrigem.TemporarioNome, arquivoOrigem.Caminho);
				}
			}
			catch (Exception exc)
			{
				diretoriosCopy.Remove(atual.Key);
				return Copiar(arquivoOrigem, diretoriosCopy, divisaoPorQtd, exc);
			}

			return arquivoOrigem;
		}

		public Arquivo Obter(Arquivo arquivo)
		{
			ArquivoDa _da = null;
			string strNovoCaminho = string.Empty;

			if (!File.Exists(arquivo.Caminho))
			{
				strNovoCaminho = arquivo.DiretorioConfiguracao.TrimEnd('\\') + "\\" + arquivo.Diretorio.TrimEnd('\\') + "\\" + arquivo.TemporarioNome;

				if (!File.Exists(strNovoCaminho))
				{
					throw new Exception("Arquivo não encontrado");
				}
				else
				{
					arquivo.Caminho = strNovoCaminho;
					_da = new ArquivoDa(_esquema);
					_da.AtualizarCaminho(arquivo);
				}
			}

			arquivo.Buffer = File.Open(arquivo.Caminho, FileMode.Open, FileAccess.Read, FileShare.Read);
			return arquivo;
		}

		public Arquivo ObterTemporario(Arquivo arquivo)
		{
			if (arquivo == null)
				throw new ArgumentNullException();

			DirectoryInfo dirTemp = null;

			if (String.IsNullOrEmpty(arquivo.TemporarioPathNome))
			{
				dirTemp = Diretorios.FirstOrDefault(x => File.Exists(String.Format("{0}\\{1}", x, arquivo.TemporarioNome)));

				arquivo.TemporarioPathNome = dirTemp.FullName;

				if (dirTemp == null)
					throw new Exception("Diretório temporário não encontrado");
			}

			if (!File.Exists(String.Format("{0}\\{1}", arquivo.TemporarioPathNome, arquivo.TemporarioNome)))
				throw new Exception("Arquivo não encontrado");

			arquivo.Buffer = File.Open(String.Format("{0}\\{1}", arquivo.TemporarioPathNome, arquivo.TemporarioNome), FileMode.Open, FileAccess.Read, FileShare.Read);
			return arquivo;
		}
	}
}