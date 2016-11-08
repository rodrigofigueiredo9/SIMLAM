using System.Collections.Generic;
using System.IO;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.Blocos.Etx.ModuloArquivo.Business
{
	public class ArquivoBus
	{
		ConfiguracaoArquivo _config = new ConfiguracaoArquivo();
		GerenciadorArquivo _gerenciador;
		GerenciadorArquivo _gerenciadorTemp;
		Dictionary<int, string> _diretorio;
		Dictionary<int, string> _diretorioTemp;
		ArquivoDa _arquivoDa;

		public ArquivoBus(eExecutorTipo modulo, Dictionary<int, string> diretorio = null, Dictionary<int, string> diretorioTemp = null, string usuarioCredenciado = null)
		{
			string schema = null;

			switch (modulo)
			{
				case eExecutorTipo.Interno:
					schema = "default";
					_diretorio = _config.DiretoriosArquivo;
					_diretorioTemp = _config.DiretoriosArquivoTemp;
					break;
				case eExecutorTipo.Credenciado:
					schema = usuarioCredenciado;
					_diretorio = diretorio;
					_diretorioTemp = diretorioTemp;
					break;
				default:
					break;
			}

			_arquivoDa = new ArquivoDa(schema);
			_gerenciador = new GerenciadorArquivo(_diretorio, schema);
			_gerenciadorTemp = new GerenciadorArquivo(_diretorioTemp, schema);

		}

		public ArquivoBus(eExecutorTipo modulo)
		{
			string schema = null;

			switch (modulo)
			{
				case eExecutorTipo.Interno:
					schema = "default";
					_diretorio = _config.DiretoriosArquivo;
					_diretorioTemp = _config.DiretoriosArquivoTemp;
					break;
				case eExecutorTipo.Credenciado:
					ConfiguracaoSistema configSys = new ConfiguracaoSistema();
					schema = configSys.UsuarioCredenciado;
					_diretorio = _config.CredenciadoDiretoriosArquivo;
					_diretorioTemp = _config.CredenciadoDiretoriosArquivoTemp;
					break;
				default:
					break;
			}

			_arquivoDa = new ArquivoDa(schema);
			_gerenciador = new GerenciadorArquivo(_diretorio, schema);
			_gerenciadorTemp = new GerenciadorArquivo(_diretorioTemp, schema);
		}

		public Arquivo.Arquivo SalvarTemp(HttpPostedFileBase filePosted)
		{
			if (filePosted == null)
			{
				Validacao.Add(Mensagem.Arquivo.ArquivoInvalido);
				return null;
			}

			Arquivo.Arquivo arq = _gerenciadorTemp.SalvarTemp(filePosted.InputStream);

			arq.Id = 0;
			arq.Nome = Path.GetFileName(filePosted.FileName);
			arq.Extensao = Path.GetExtension(filePosted.FileName);
			arq.TemporarioPathNome = string.Empty;
			arq.ContentLength = filePosted.ContentLength;
			arq.ContentType = filePosted.ContentType;

			Validacao.Add(Mensagem.Arquivo.ArquivoTempSucesso(arq.Nome));

			return arq;
		}

		public Arquivo.Arquivo SalvarTemp(Arquivo.Arquivo arquivo)
		{
			Arquivo.Arquivo arq = _gerenciadorTemp.SalvarTemp(arquivo.Buffer);

			arquivo.TemporarioNome = arq.TemporarioNome;
			arquivo.TemporarioPathNome = arq.TemporarioPathNome;

			return arquivo;
		}

		public Arquivo.Arquivo Salvar(Arquivo.Arquivo arquivo)
		{
			if (arquivo == null)
			{
				Validacao.Add(Mensagem.Arquivo.ArquivoInvalido);
				return null;
			}

			arquivo.Nome = Path.GetFileName(arquivo.Nome);

			Arquivo.Arquivo arq = _gerenciador.Salvar(arquivo, _diretorio, _arquivoDa.ObterSeparadorQtd().ToString());

			return arquivo;
		}

		public Arquivo.Arquivo Copiar(Arquivo.Arquivo arquivo)
		{
			return _gerenciadorTemp.Copiar(arquivo, _diretorioTemp, _diretorio, _arquivoDa.ObterSeparadorQtd().ToString());
		}

		public bool Existe(int id)
		{
			return _arquivoDa.Existe(id);
		}

		public Arquivo.Arquivo Obter(int id)
		{
			Arquivo.Arquivo arquivo = _arquivoDa.Obter(id);
			return _gerenciadorTemp.Obter(arquivo);
		}

		public Arquivo.Arquivo ObterTemporario(Arquivo.Arquivo arquivo)
		{
			return _gerenciadorTemp.ObterTemporario(arquivo);
		}

		public Arquivo.Arquivo ObterDados(int id)
		{
			return _arquivoDa.Obter(id);
		}

		public void Deletar(string arquivo)
		{
			_gerenciador.Deletar(arquivo);
		}
	}
}